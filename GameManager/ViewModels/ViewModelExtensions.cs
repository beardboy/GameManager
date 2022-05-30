using GameManager.Maui.Interfaces.ViewModels;

namespace GameManager.Maui.ViewModels;

public static class ViewModelExtensions
{
    public static MauiAppBuilder ConfigureViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<IFavoritesViewModel, FavoritesViewModel>();
        builder.Services.AddTransient<IHeaderViewModel, HeaderViewModel>();
        builder.Services.AddTransient<IHomeViewModel, HomeViewModel>();
        builder.Services.AddTransient<IGameViewModel, GameViewModel>();
        builder.Services.AddTransient<IMainViewModel, MainViewModel>();

        return builder;
    }
}
