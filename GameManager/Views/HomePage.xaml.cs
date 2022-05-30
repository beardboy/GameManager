using GameManager.Maui.Interfaces.ViewModels;
using GameManager.Maui.ViewModels;

namespace GameManager.Maui.Views;

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

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        vm.OnNavigatedTo(args);
    }
}