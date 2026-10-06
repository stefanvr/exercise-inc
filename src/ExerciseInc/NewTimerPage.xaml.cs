using ExerciseInc.Core.Storage;
using ExerciseInc.Core.Timers;

namespace ExerciseInc;

public partial class NewTimerPage : ContentPage
{
	private readonly TimerStore _store;
	private readonly Action<SavedTimer> _saved;
	private readonly IReadOnlyList<TimerSettings> _existing;

	public NewTimerPage(TimerStore store, Action<SavedTimer> saved)
	{
		InitializeComponent();
		_store = store;
		_saved = saved;
		_existing = store.All().Select(timer => timer.Settings).ToList();
	}

	private TimerDraft Draft() => TimerDraft.Of(
		StartDelay.Text ?? "", Work.Text ?? "", Rest.Text ?? "", Repeats.Text ?? "", _existing);

	private void OnChanged(object? sender, TextChangedEventArgs e)
	{
		var draft = Draft();

		ShowRange(StartDelayRange, draft.StartDelay, $"empty, or {Range(TimerSettings.StartDelayRange)}");
		ShowRange(WorkRange, draft.Work, Range(TimerSettings.WorkRange));
		ShowRange(RestRange, draft.Rest, Range(TimerSettings.RestRange));
		ShowRange(RepeatsRange, draft.Repeats, Range(TimerSettings.RepeatsRange));

		NameLine.IsVisible = draft.Settings is not null;
		NameLine.Text = draft.Settings is { } settings ? $"Name: {settings.Name}" : "";
		Duplicate.IsVisible = draft.IsDuplicate;
		SaveButton.IsEnabled = draft.CanSave;
	}

	private static string Range(ValueRange range) => $"{range.Min} to {range.Max}";

	private static void ShowRange(Label label, FieldState state, string text)
	{
		label.Text = text;
		label.IsVisible = state == FieldState.OutOfRange;
	}

	private async void OnSave(object? sender, EventArgs e)
	{
		if (Draft() is not { CanSave: true, Settings: { } settings })
			return;
		_saved(_store.Add(settings));
		await Navigation.PopAsync();
	}

	private async void OnCancel(object? sender, EventArgs e) => await Navigation.PopAsync();
}
