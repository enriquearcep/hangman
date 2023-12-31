﻿using SkiaSharp.Views.Maui.Controls.Hosting;

namespace Hangman;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
        var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("Donkin.otf", "Donkin");
				fonts.AddFont("Mistage.ttf", "Mistage");
				fonts.AddFont("Fontello.ttf", "Icons");
			});

		return builder.Build();
	}
}
