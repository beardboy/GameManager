namespace GameManager.Views
{
    public static class ViewExtensions
    {
        public static MauiAppBuilder ConfigureViews(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<GamePage>();
            builder.Services.AddTransient<MainPage>();

            return builder;
        }
    }
}
