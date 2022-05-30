#region Usings

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameManager.Maui.Interfaces;
using GameManager.Maui.Interfaces.ViewModels;
using System.Windows.Input;

#endregion

namespace GameManager.Maui.ViewModels;

public partial class MainViewModel : BaseViewModel, IMainViewModel
{
    #region Fields

    private readonly IIgdbController _igdbController;

    #endregion

    #region Properties

    [ObservableProperty] int count = 0;
    [ObservableProperty] bool isLoggedIn = false;

    #endregion

    #region Constructors

    public MainViewModel(IIgdbController igdbController) : base(igdbController)
    {
        _igdbController = igdbController;
    }

    #endregion

    #region Commands

    [ICommand]
    void IncreaseCount()
    {
        Count++;
        // SemanticScreenReader.Announce(CounterLabel.Text);
    }

    #endregion

    #region Methods

    public async Task InitializeAsync()
    {
        if (IsLoggedIn || IsBusy)
        {
            return;
        }

        IsBusy = true;

        // var platforms = await _igdbController.GetAsync<IGDB.Models.Platform>(IGDBClient.Endpoints.Platforms);
        // var platformsCount = await _igdbController.GetCountAsync(IGDBClient.Endpoints.Platforms);
        // var allPlatforms = await _igdbController.GetAsync<IGDB.Models.Platform>(IGDBClient.Endpoints.Platforms, limit: platformsCount);

        // var ssb = await _igdbController.GetAsync<IGDB.Models.Game>(IGDBClient.Endpoints.Games, "search \"super smash\"; fields *;", 200);

        // var recent = await _igdbController.GetAsync<IGDB.Models.ReleaseDate>(IGDBClient.Endpoints.ReleaseDates, $"fields id, date, game.*, human, platform.*; where date < {DateTimeOffset.UtcNow.ToUnixTimeSeconds()}; sort date desc;", 200);
        // var upcoming = await _igdbController.GetAsync<IGDB.Models.ReleaseDate>(IGDBClient.Endpoints.ReleaseDates, $"fields id, date, game.*, human, platform.*; where date > {DateTimeOffset.UtcNow.ToUnixTimeSeconds()}; sort date asc;", 200);

        IsBusy = false;
    }

    #endregion
}