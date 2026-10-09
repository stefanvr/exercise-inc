// dotnet run scripts/trace.cs [--changed]
//
// Each section of domain, app and style with the tests that name it, the
// sections no test names, and the tests that name no section or one that does
// not exist (.claude/harness/documents.md › Traceability). With --changed, only
// the sections whose text changed since main. Exits 1 when a test names no
// section, or one that does not exist or is ambiguous.
//
// A test names sections by its DisplayName: `<document> › <heading>: <the
// behaviour>`, a heading by its path among the headings above it
// (`app › Timer list › Wording`), several joined by `; `.
using System.Diagnostics;
using System.Text.RegularExpressions;

string[] traced = ["domain", "app", "style"];
const string Base = "main";
const string Separator = " › ";

if (args.Any(a => a != "--changed"))
{
    Console.Error.WriteLine("usage: dotnet run scripts/trace.cs [--changed]");
    return 2;
}
var changedOnly = args.Contains("--changed");

// README.md, each doc/<name>.md, and each doc/<name>/ with its index.md and parts.
var documents = new Dictionary<string, List<Heading>> { ["README"] = HeadingsOf("README.md", titled: true) };
foreach (var path in Directory.GetFiles("doc", "*.md").Order())
    documents[Path.GetFileNameWithoutExtension(path)] = HeadingsOf(path, titled: true);
foreach (var dir in Directory.GetDirectories("doc").Order().Where(d => File.Exists(Path.Combine(d, "index.md"))))
    documents[Path.GetFileName(dir)] =
    [
        .. HeadingsOf(Path.Combine(dir, "index.md"), titled: true),
        .. Directory.GetFiles(dir, "*.md").Where(p => Path.GetFileName(p) != "index.md").Order()
            .SelectMany(p => HeadingsOf(p, titled: false)),
    ];

var tests = Directory.GetFiles("tests", "*.cs", SearchOption.AllDirectories)
    .Where(p => !p.Split(Path.DirectorySeparatorChar).Any(part => part is "bin" or "obj"))
    .Order()
    .SelectMany(TestsOf)
    .ToList();

var changed = changedOnly ? ChangedLines() : null;
var sections = traced.Where(documents.ContainsKey)
    .SelectMany(name => documents[name])
    .Where(h => h.Own && !h.Title)
    .Where(h => changed is null || h.Lines.Any(line => changed.Contains((h.Path, line))))
    .ToList();

var named = new Dictionary<Heading, List<Test>>();
var unnamed = new List<Test>();
var unresolved = new List<(Test Test, string Reference, string Reason)>();
foreach (var test in tests)
{
    var references = ReferencesOf(test.Name);
    if (references.Count == 0)
        unnamed.Add(test);
    foreach (var (document, path, text) in references)
    {
        var fits = documents[document].Where(h => h.Own && !h.Title && Fits(path, h)).ToList();
        if (fits.Count == 1)
            (named.TryGetValue(fits[0], out var list) ? list : named[fits[0]] = []).Add(test);
        else
            unresolved.Add((test, text, fits.Count == 0 ? "does not exist" : "is ambiguous"));
    }
}

if (changed is not null)
    Console.WriteLine($"Sections changed since {Base}: {sections.Count}\n");
foreach (var section in sections)
{
    var naming = named.GetValueOrDefault(section) ?? [];
    Console.WriteLine($"{Show(section)}  ({(naming.Count == 0 ? "no test" : Plural(naming.Count, "test"))})");
    foreach (var test in naming)
        Console.WriteLine($"    {test.Path}:{test.Line}  {test.Name}");
}
var untested = sections.Where(s => !named.ContainsKey(s)).ToList();
Console.WriteLine($"\nSections no test names: {untested.Count}");
untested.ForEach(s => Console.WriteLine($"    {Show(s)}"));
Console.WriteLine($"\nTests that name no section: {unnamed.Count}");
unnamed.ForEach(t => Console.WriteLine($"    {t.Path}:{t.Line}  {t.Name}"));
Console.WriteLine($"\nTests that name a section that does not exist, or is ambiguous: {unresolved.Count}");
unresolved.ForEach(u => Console.WriteLine($"    {u.Test.Path}:{u.Test.Line}  {u.Reference} {u.Reason}: {u.Test.Name}"));
return unnamed.Count + unresolved.Count == 0 ? 0 : 1;

static string Show(Heading h) => $"{h.Path}:{h.Line}  {new string('#', h.Level)} {h.Text}";

static string Plural(int n, string one) => $"{n} {one}{(n == 1 ? "" : "s")}";

// The headings of a markdown file outside fenced blocks, each with the headings
// above it, the lines of its own text, and whether it is the document's title.
static List<Heading> HeadingsOf(string path, bool titled)
{
    var headings = new List<Heading>();
    var stack = new List<Heading>();
    var fenced = false;
    var lines = File.ReadAllLines(path);
    for (var i = 0; i < lines.Length; i++)
    {
        if (Regex.IsMatch(lines[i], @"^\s*(```|~~~)"))
            fenced = !fenced;
        var match = fenced ? Match.Empty : Regex.Match(lines[i], @"^(#{1,6}) +(.+?) *#*$");
        if (match.Success)
        {
            var level = match.Groups[1].Length;
            stack.RemoveAll(h => h.Level >= level);
            var heading = new Heading(path, i + 1, level, match.Groups[2].Value, [.. stack.Select(h => h.Text)],
                titled && level == 1 && !headings.Any(h => h.Level == 1));
            headings.Add(heading);
            stack.Add(heading);
        }
        else if (lines[i].Trim().Length > 0 && headings.Count > 0)
            headings[^1].Lines.Add(i + 1);
    }
    return headings;
}

// The tests of a test file: each [Fact] or [Theory] with its DisplayName, or its
// method name when it has none.
static IEnumerable<Test> TestsOf(string path)
{
    var lines = File.ReadAllLines(path);
    for (var i = 0; i < lines.Length; i++)
    {
        var attribute = Regex.Match(lines[i], @"\[(Fact|Theory)\b(.*)");
        if (!attribute.Success)
            continue;
        var display = Regex.Match(attribute.Groups[2].Value, @"DisplayName\s*=\s*""((?:[^""\\]|\\.)*)""");
        if (display.Success)
        {
            yield return new Test(path, i + 1, Regex.Unescape(display.Groups[1].Value));
            continue;
        }
        var method = lines.Skip(i + 1).Select(l => Regex.Match(l, @"\b(\w+)\s*\(")).FirstOrDefault(m => m.Success);
        yield return new Test(path, i + 1, method?.Groups[1].Value ?? "?");
    }
}

// The sections a test name starts with: none when it does not start with a
// known document.
List<(string Document, string[] Path, string Text)> ReferencesOf(string name)
{
    var references = new List<(string, string[], string)>();
    var rest = name;
    while (true)
    {
        var separator = rest.IndexOf(Separator, StringComparison.Ordinal);
        if (separator < 0 || !documents.ContainsKey(rest[..separator]))
            break;
        var end = Regex.Match(rest, @": |; |$").Index;
        var document = rest[..separator];
        var path = rest[(separator + Separator.Length)..end].Split(Separator).Select(s => s.Trim()).ToArray();
        references.Add((document, path, rest[..end]));
        if (!rest[end..].StartsWith("; "))
            break;
        rest = rest[(end + 2)..];
    }
    return references;
}

// A heading fits a reference when it is the reference's last part and the parts
// before it are among the headings above it, in order.
static bool Fits(string[] path, Heading heading)
{
    if (heading.Text != path[^1])
        return false;
    var at = 0;
    foreach (var above in heading.Above)
        if (at < path.Length - 1 && above == path[at])
            at++;
    return at == path.Length - 1;
}

// The changed lines of doc/ since the merge base with main, by path and line.
static HashSet<(string, int)> ChangedLines()
{
    var git = Process.Start(new ProcessStartInfo("git", ["diff", "--merge-base", Base, "-U0", "--", "doc/"])
    {
        RedirectStandardOutput = true,
    })!;
    var lines = new HashSet<(string, int)>();
    string? path = null;
    foreach (var line in git.StandardOutput.ReadToEnd().Split('\n'))
    {
        if (line.StartsWith("+++ "))
            path = line == "+++ /dev/null" ? null : line[6..];
        var hunk = Regex.Match(line, @"^@@ -\S+ \+(\d+)(?:,(\d+))? @@");
        if (hunk.Success && path is not null)
        {
            var start = int.Parse(hunk.Groups[1].Value);
            var count = hunk.Groups[2].Success ? int.Parse(hunk.Groups[2].Value) : 1;
            for (var n = start; n < start + count; n++)
                lines.Add((path, n));
        }
    }
    git.WaitForExit();
    return lines;
}

record Heading(string Path, int Line, int Level, string Text, string[] Above, bool Title)
{
    public List<int> Lines { get; } = [];
    public bool Own => Lines.Count > 0;
}

record Test(string Path, int Line, string Name);
