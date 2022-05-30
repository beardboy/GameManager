using GameManager.Maui.Interfaces.ViewModels;
using GameManager.Maui.ViewModels;

namespace GameManager.Maui.Views;

public partial class FavoritesPage : ContentPage
{
    private IFavoritesViewModel vm => BindingContext as FavoritesViewModel;

    public FavoritesPage(IFavoritesViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await vm.InitializeAsync();
    }

}