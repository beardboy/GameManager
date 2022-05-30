using GameManager.Maui.Interfaces;

namespace GameManager.Maui.Controller;

public static class ControllerExtensions
{
    public static MauiAppBuilder ConfigureControllers(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<IIgdbController, IgdbController>();

        return builder;
    }

}