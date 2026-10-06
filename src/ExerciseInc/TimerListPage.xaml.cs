using ExerciseInc.Core.Storage;

namespace ExerciseInc;

public partial class TimerListPage : ContentPage
{
	private readonly TimerStore _store;
	private readonly TimeProvider _clock;

	/// <summary>The timer to select when the list shows again, such as one just saved.</summary>
	private int? _select;

	public TimerListPage(TimerStore store, TimeProvider clock)
	{
		InitializeComponent();
		_store = store;
		_clock = clock;
#if DEBUG
		ToolbarItems.Add(new ToolbarItem("Style preview", null, OnStylePreview, ToolbarItemOrder.Secondary));
#endif
	}

	private SavedTimer? Selected => Timers.SelectedItem as SavedTimer;

	protected override void OnAppearing()
	{
		base.OnAppearing();
		Show(_select ?? Selected?.Id);
		_select = null;
	}

	private void Show(int? select)
	{
		var timers = _store.All();
		Timers.ItemsSource = timers;
		Timers.SelectedItem = timers.FirstOrDefault(timer => timer.Id == select);
		ShowButtons();
	}

	private void ShowButtons()
	{
		StartButton.IsEnabled = Selected is not null;
		DeleteButton.IsEnabled = Selected is not null;
	}

	private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e) => ShowButtons();

	private async void OnNew(object? sender, EventArgs e) =>
		await Navigation.PushAsync(new NewTimerPage(_store, saved => _select = saved.Id));

	private async void OnStart(object? sender, EventArgs e)
	{
		if (Selected is { } timer)
			await Navigation.PushAsync(new TimerPage(timer.Settings, _clock));
	}

	private async void OnDelete(object? sender, EventArgs e)
	{
		if (Selected is not { } timer)
			return;
		if (await DisplayAlertAsync("Delete timer?", timer.Settings.Name, "Delete", "Cancel"))
		{
			_store.Delete(timer.Id);
			Show(null);
		}
	}

#if DEBUG
	private async void OnStylePreview() => await Navigation.PushAsync(new StylePreviewPage());
#endif
}
