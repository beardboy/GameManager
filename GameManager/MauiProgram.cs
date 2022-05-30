#region Usings

using CommunityToolkit.Maui;
using GameManager.Maui.Controller;
using GameManager.Maui.ViewModels;
using GameManager.Maui.Views;

#endregion

namespace GameManager.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureControllers()
            .ConfigureViewModels()
            .ConfigureViews()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		return builder.Build();
	}
}
