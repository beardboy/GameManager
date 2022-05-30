namespace GameManager.Maui.Views
{
    public static class ViewExtensions
    {
        public static MauiAppBuilder ConfigureViews(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<FavoritesPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<GamePage>();
            builder.Services.AddTransient<MainPage>();

            return builder;
        }
    }
}
