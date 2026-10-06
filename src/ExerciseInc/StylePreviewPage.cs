#if DEBUG
namespace ExerciseInc;

/// <summary>
/// Dev only: every colour token in Resources/Styles/Colors.xaml with its name,
/// read from the app's resources, plus the timer screen's text on each timer
/// colour. Reached from the timer list's overflow menu in Debug builds.
/// </summary>
public class StylePreviewPage : ContentPage
{
	public StylePreviewPage()
	{
		Title = "Style preview";
		var rows = new VerticalStackLayout { Padding = 16, Spacing = 8 };
		foreach (var (key, color) in Tokens())
		{
			rows.Add(new Grid
			{
				ColumnDefinitions = [new ColumnDefinition(96), new ColumnDefinition(GridLength.Star)],
				ColumnSpacing = 12,
				Children =
				{
					new Border
					{
						BackgroundColor = color,
						HeightRequest = 48,
						Stroke = Colors.Gray,
						Content = new Label
						{
							Text = "0:23",
							TextColor = Token("TimerText"),
							FontAttributes = FontAttributes.Bold,
							HorizontalOptions = LayoutOptions.Center,
							VerticalOptions = LayoutOptions.Center,
						},
					},
					Column(new Label { Text = $"{key}\n{color.ToArgbHex()}", VerticalOptions = LayoutOptions.Center }, 1),
				},
			});
		}
		Content = new ScrollView { Content = rows };
	}

	private static View Column(View view, int column)
	{
		Grid.SetColumn(view, column);
		return view;
	}

	private static Color Token(string key) => (Color)Application.Current!.Resources[key];

	/// <summary>
	/// The colour tokens of the token source. A dictionary merged through its
	/// Source does not list its entries, so the preview loads its own instance.
	/// </summary>
	private static IEnumerable<(string, Color)> Tokens() =>
		new global::ExerciseInc.Resources.Styles.Colors()
			.Where(entry => entry.Value is Color)
			.Select(entry => (entry.Key, (Color)entry.Value));
}
#endif
