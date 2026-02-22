using Microsoft.Extensions.Logging;
using Tetris4Dummies.Core.Helpers;
using Tetris4Dummies.Core.Models;
using Tetris4Dummies.Presentation.Services;
using Tetris4Dummies.Presentation.ViewModels;
using Tetris4Dummies.Presentation.Views;

namespace Tetris4Dummies;

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

		// Register services
		builder.Services.AddSingleton<IRandomProvider, RandomProvider>();
		builder.Services.AddSingleton<IScoringService, ScoringService>();
		builder.Services.AddSingleton<IMainThreadDispatcher, MainThreadDispatcher>();
		
		// Register ViewModels
		builder.Services.AddTransient<GameState>();
		builder.Services.AddTransient<GameViewModel>();
		
		// Register Pages
		builder.Services.AddTransient<MainPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
