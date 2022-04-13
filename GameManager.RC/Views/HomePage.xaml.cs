using GameManager.Interfaces.ViewModels;
using GameManager.ViewModels;
using Microsoft.Maui.Controls;

namespace GameManager.Views;

public partial class HomePage : ContentPage
{

    private IHomeViewModel vm => BindingContext as HomeViewModel;

    public HomePage(IHomeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing(); 
        await vm.InitializeAsync();
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {

    }
}