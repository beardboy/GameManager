using GameManager.Maui.Views;

namespace GameManager.Maui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        Routing.RegisterRoute("favorite", typeof(FavoritesPage));
        Routing.RegisterRoute("game", typeof(GamePage));
        Routing.RegisterRoute("home", typeof(HomePage));

        InitializeComponent();
    }
}
