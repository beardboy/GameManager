using GameManager.Maui.Interfaces.ViewModels;
using GameManager.Maui.ViewModels;

namespace GameManager.Maui.Views;

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

