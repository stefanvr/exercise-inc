using AndroidX.Core.View;

namespace ExerciseInc;

/// <summary>Hides the phone's status and navigation bars for the timer screen: doc/app.md › Timer screen.</summary>
public static class SystemBars
{
	public static void Hide()
	{
		if (Controller() is not { } controller)
			return;
		controller.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;
		controller.Hide(WindowInsetsCompat.Type.SystemBars());
	}

	public static void Show() => Controller()?.Show(WindowInsetsCompat.Type.SystemBars());

	private static WindowInsetsControllerCompat? Controller() =>
		Platform.CurrentActivity?.Window is { } window
			? WindowCompat.GetInsetsController(window, window.DecorView)
			: null;
}
