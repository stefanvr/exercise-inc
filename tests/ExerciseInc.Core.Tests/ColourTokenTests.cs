using System.Xml.Linq;
using ExerciseInc.Core.Timers;

namespace ExerciseInc.Core.Tests;

public class ColourTokenTests
{
    private static readonly XNamespace X = "http://schemas.microsoft.com/winfx/2009/xaml";

    /// <summary>The keys in the app's token source, read from the file itself.</summary>
    private static HashSet<string> TokenKeys()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (!File.Exists(Path.Combine(dir.FullName, "ExerciseInc.slnx")))
            dir = dir.Parent ?? throw new InvalidOperationException("Repository root not found.");
        var path = Path.Combine(dir.FullName, "src/ExerciseInc/Resources/Styles/Colors.xaml");
        return XDocument.Load(path).Descendants()
            .Select(element => (string?)element.Attribute(X + "Key"))
            .OfType<string>()
            .ToHashSet();
    }

    [Fact(DisplayName = "style › Phase colours: every phase has its colour, a light version and a dimmed one")]
    public void EveryPhaseHasItsColours()
    {
        var keys = TokenKeys();

        foreach (var phase in Enum.GetNames<Phase>())
            Assert.Superset(new HashSet<string> { phase, phase + "Lit", phase + "Dimmed" }, keys);
        Assert.Contains("Done", keys);
        Assert.Contains("TimerText", keys);
    }
}
