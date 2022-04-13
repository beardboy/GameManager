using GameManager.Interfaces;

namespace GameManager.Controller;

public static class ControllerExtensions
{
    public static MauiAppBuilder ConfigureControllers(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<IIgdbController, IgdbController>();

        return builder;
    }

}