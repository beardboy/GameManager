using GameManager.Interfaces.ViewModels;
using GameManager.ViewModels;

namespace GameManager.Views;

public partial class GamePage : ContentPage
{
	private IGameViewModel vm => BindingContext as GameViewModel;

	public GamePage(IGameViewModel vm)
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