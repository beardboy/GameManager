using GameManager.Views;

namespace GameManager;

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
