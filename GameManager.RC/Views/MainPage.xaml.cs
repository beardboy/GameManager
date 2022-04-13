using GameManager.Interfaces.ViewModels;
using GameManager.ViewModels;
using Microsoft.Maui.Controls;

namespace GameManager.Views;

public partial class MainPage : ContentPage
{
    private IMainViewModel vm => BindingContext as MainViewModel;

    public MainPage(IMainViewModel vm)
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

