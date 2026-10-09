using ExerciseInc.Core.Timers;

namespace ExerciseInc;

/// <summary>The timer screen: doc/app.md › Timer screen. It renders the started timer's moment.</summary>
public partial class TimerPage : ContentPage
{
	private readonly StartedTimer _timer;
	private readonly TimeProvider _clock;
	private readonly IDispatcherTimer _ticker;
	private Window? _window;
	private bool _ending;

	public TimerPage(TimerSettings settings, TimeProvider clock)
	{
		InitializeComponent();
		_clock = clock;
		_timer = StartedTimer.Start(settings, Now);
		// Often enough for the flash's half seconds to start on time.
		_ticker = Dispatcher.CreateTimer();
		_ticker.Interval = TimeSpan.FromMilliseconds(50);
		_ticker.Tick += (_, _) => Render();
	}

	private DateTimeOffset Now => _clock.GetUtcNow();

	protected override void OnAppearing()
	{
		base.OnAppearing();
		_window = Window;
		if (_window is not null)
			_window.Resumed += OnWindowResumed;
		SystemBars.Hide();
		_ticker.Start();
		Render();
	}

	protected override void OnDisappearing()
	{
		_ticker.Stop();
		if (_window is not null)
			_window.Resumed -= OnWindowResumed;
		SystemBars.Show();
		DeviceDisplay.Current.KeepScreenOn = false;
		base.OnDisappearing();
	}

	// Android shows the system bars again when the app comes back.
	private void OnWindowResumed(object? sender, EventArgs e)
	{
		SystemBars.Hide();
		Render();
	}

	private void Render()
	{
		var moment = _timer.At(Now);
		var running = moment.State == TimerState.Running;
		var paused = moment.State == TimerState.Paused;

		if (DeviceDisplay.Current.KeepScreenOn != running)
			DeviceDisplay.Current.KeepScreenOn = running;

		DoneLabel.IsVisible = moment.Phase is null;
		PhaseLabel.IsVisible = moment.Phase is not null;
		CountdownLabel.IsVisible = moment.Phase is not null;
		RepeatLabel.IsVisible = moment.Repeat is not null;
		PausedLabel.IsVisible = paused;
		RunningButtons.IsVisible = running;
		PausedButtons.IsVisible = paused;
		BackgroundColor = Token(ColourOf(moment));

		if (moment.Phase is not { } phase)
		{
			_ticker.Stop();
			return;
		}
		PhaseLabel.Text = NameOf(phase).ToUpperInvariant();
		CountdownLabel.Text = moment.Countdown;
		RepeatLabel.Text = $"repeat {moment.Repeat} / {moment.Repeats}";
	}

	private static string NameOf(Phase phase) => phase switch
	{
		Phase.StartDelay => "Start delay",
		Phase.Work => "Work",
		_ => "Rest",
	};

	/// <summary>The colour token for the moment: doc/style.md › Phase colours, Last seconds, Paused, Done.</summary>
	private static string ColourOf(TimerMoment moment)
	{
		if (moment.Phase is not { } phase)
			return "Done";
		var key = phase.ToString();
		if (moment.State == TimerState.Paused)
			return key + "Dimmed";
		return moment.IsFlashLit ? key + "Lit" : key;
	}

	private static Color Token(string key) =>
		Application.Current!.Resources.TryGetValue(key, out var value)
			? (Color)value
			: throw new InvalidOperationException($"No colour token {key}.");

	private void OnPause(object? sender, EventArgs e)
	{
		var now = Now;
		if (_timer.At(now).State == TimerState.Running)
			_timer.Pause(now);
		Render();
	}

	private void OnResume(object? sender, EventArgs e)
	{
		_timer.Resume(Now);
		_ticker.Start();
		Render();
	}

	private void OnRestart(object? sender, EventArgs e)
	{
		_timer.Restart(Now);
		_ticker.Start();
		Render();
	}

	private void OnEnd(object? sender, EventArgs e) => End();

	private void OnTapped(object? sender, TappedEventArgs e)
	{
		if (_timer.At(Now).State == TimerState.Done)
			End();
	}

	/// <summary>The back gesture pauses a running timer and ends a paused or done one.</summary>
	protected override bool OnBackButtonPressed()
	{
		var now = Now;
		if (_timer.At(now).State == TimerState.Running)
		{
			_timer.Pause(now);
			Render();
		}
		else
			End();
		return true;
	}

	private async void End()
	{
		if (_ending)
			return;
		_ending = true;
		await Navigation.PopAsync();
	}
}
