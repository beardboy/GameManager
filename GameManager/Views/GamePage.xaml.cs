using GameManager.Maui.Interfaces.ViewModels;
using GameManager.Maui.Models;
using GameManager.Maui.ViewModels;

namespace GameManager.Maui.Views;

[QueryProperty(nameof(Game), "Game")]
public partial class GamePage : ContentPage
{
    private ReleaseDateWithCovers _game;

    public ReleaseDateWithCovers Game
    {
        get => _game;
        set
        {
            _game = value;
            OnPropertyChanged();
        }
    }

    private IGameViewModel vm => BindingContext as GameViewModel;

	public GamePage(IGameViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await vm.InitializeAsync(Game);
	}

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("..");
        return true;
//        return base.OnBackButtonPressed();
    }
}