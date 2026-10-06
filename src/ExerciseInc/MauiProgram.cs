using ExerciseInc.Core.Storage;
using Microsoft.Extensions.Logging;

namespace ExerciseInc;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton(TimeProvider.System);
		builder.Services.AddSingleton(_ =>
			new TimerStore(Path.Combine(FileSystem.AppDataDirectory, "exercise-inc.db3")));
		builder.Services.AddTransient<TimerListPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
