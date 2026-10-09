#if DEBUG
using ExerciseInc.Core.Timers;

namespace ExerciseInc;

/// <summary>
/// Dev only: one section per pattern of doc/style.md, under its name and in its
/// order, drawn from the real sources: the colour tokens in
/// Resources/Styles/Colors.xaml, the timer screen's styles in
/// Resources/Styles/TimerScreen.xaml and its choice of colour. Then every other token, and the
/// sounds. Reached from the timer list's overflow menu in Debug builds;
/// scripts/check-style-preview.sh checks it.
/// </summary>
public class StylePreviewPage : ContentPage
{
	private static readonly Phase[] Phases = Enum.GetValues<Phase>();

	private readonly IDispatcherTimer _ticker;
	private StartedTimer? _demo;

	public StylePreviewPage()
	{
		Title = "Style preview";
		_ticker = Dispatcher.CreateTimer();
		_ticker.Interval = TimeSpan.FromMilliseconds(50);
		var tokens = Tokens();
		var shown = new HashSet<string>();
		Color Use(string key)
		{
			shown.Add(key);
			return tokens[key];
		}

		var page = new VerticalStackLayout { Padding = 16, Spacing = 8 };

		page.Add(Heading("Phase colours"));
		foreach (var phase in Phases)
			page.Add(TokenRow(phase.ToString(), Use(phase.ToString()), PhaseLabel(phase)));
		page.Add(Demo(tokens));

		page.Add(Heading("Last seconds"));
		foreach (var phase in Phases)
			page.Add(TokenRow($"{phase}Lit", Use($"{phase}Lit"), PhaseLabel(phase)));

		page.Add(Heading("Paused"));
		foreach (var phase in Phases)
			page.Add(TokenRow($"{phase}Dimmed", Use($"{phase}Dimmed"), PhaseLabel(phase)));

		page.Add(Heading("Done"));
		page.Add(TokenRow("Done", Use("Done"), new Label()));

		page.Add(Heading("Countdown"));
		page.Add(Swatch(tokens[nameof(Phase.Work)], TimerLabel("0:23", "Countdown")));

		page.Add(Heading("Phase label"));
		foreach (var phase in Phases)
			page.Add(Swatch(tokens[phase.ToString()], PhaseLabel(phase)));

		page.Add(Heading("On colour"));
		foreach (var background in Phases.Select(phase => phase.ToString()).Append("Done"))
			page.Add(Swatch(tokens[background], new VerticalStackLayout
			{
				Spacing = 8,
				Children = { new Label { Text = "Text" }, new Button { Text = "Button" } },
			}));
		page.Add(Caption("TimerText", Use("TimerText")));

		page.Add(Heading("Error text"));
		page.Add(new Label { Text = "This timer already exists.", TextColor = Use("FieldError") });
		page.Add(Caption("FieldError", tokens["FieldError"]));

		page.Add(Heading("Other tokens"));
		foreach (var (key, color) in tokens.Where(token => !shown.Contains(token.Key)))
			page.Add(TokenRow(key, color, new Label()));

		page.Add(Heading("Audible"));
		page.Add(new Label { Text = "None yet." });

		Content = new ScrollView { Content = page };
	}

	protected override void OnDisappearing()
	{
		_ticker.Stop();
		base.OnDisappearing();
	}

	/// <summary>
	/// A started timer of 8 s work and 6 s rest, run on request, coloured as the
	/// timer screen colours it: Phase colours, Last seconds and Done in motion.
	/// </summary>
	private View Demo(IReadOnlyDictionary<string, Color> tokens)
	{
		var phase = TimerLabel("", "PhaseLabel");
		var countdown = TimerLabel("", "Countdown");
		var swatch = Swatch(tokens["Done"], new VerticalStackLayout { phase, countdown });
		swatch.IsVisible = false;
		var run = new Button { Text = "Run 8 s work, 6 s rest" };
		run.Clicked += (_, _) =>
		{
			_demo = StartedTimer.Start(new TimerSettings(null, 8, 6, 1), DateTimeOffset.UtcNow);
			swatch.IsVisible = true;
			_ticker.Start();
		};
		_ticker.Tick += (_, _) =>
		{
			if (_demo is null)
				return;
			var moment = _demo.At(DateTimeOffset.UtcNow);
			swatch.BackgroundColor = tokens[TimerPage.ColourOf(moment)];
			if (moment.Phase is { } current)
			{
				phase.Text = TimerPage.NameOf(current);
				countdown.Text = moment.Countdown;
				return;
			}
			phase.Text = countdown.Text = "";
			_ticker.Stop();
		};
		return new VerticalStackLayout { Spacing = 8, Children = { run, swatch } };
	}

	private static Label Heading(string pattern) =>
		new() { Text = pattern, FontSize = 22, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 16, 0, 0) };

	private static Label PhaseLabel(Phase phase) => TimerLabel(TimerPage.NameOf(phase), "PhaseLabel");

	/// <summary>A label in a style of TimerScreen.xaml, found through the swatch it is drawn on.</summary>
	private static Label TimerLabel(string text, string style)
	{
		var label = new Label { Text = text };
		label.SetDynamicResource(StyleProperty, style);
		return label;
	}

	/// <summary>A colour with the timer screen's styles, as the timer page has them.</summary>
	private static Border Swatch(Color color, View content) => new()
	{
		BackgroundColor = color,
		Stroke = Colors.Gray,
		Padding = 8,
		Content = content,
		Resources = new global::ExerciseInc.Resources.Styles.TimerScreen(),
	};

	/// <summary>A swatch of the token, the full width, its name and value below it.</summary>
	private static VerticalStackLayout TokenRow(string key, Color color, View sample)
	{
		var swatch = Swatch(color, sample);
		swatch.MinimumHeightRequest = 40;
		return new() { Spacing = 2, Children = { swatch, Caption(key, color) } };
	}

	private static HorizontalStackLayout Caption(string key, Color color) => new()
	{
		Spacing = 12,
		Children = { new Label { Text = key }, new Label { Text = color.ToArgbHex() } },
	};

	/// <summary>
	/// The tokens of the token source, colours and brushes, in its order, each
	/// with its colour. A dictionary merged through its Source does not list its
	/// entries, so the preview loads its own instance.
	/// </summary>
	private static Dictionary<string, Color> Tokens() =>
		new global::ExerciseInc.Resources.Styles.Colors()
			.Select(entry => (entry.Key, Color: entry.Value switch
			{
				Color color => color,
				SolidColorBrush brush => brush.Color,
				_ => null,
			}))
			.Where(token => token.Color is not null)
			.ToDictionary(token => token.Key, token => token.Color!);
}
#endif
