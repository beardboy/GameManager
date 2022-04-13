using GameManager.Interfaces;
using GameManager.Interfaces.ViewModels;
using System.Windows.Input;

namespace GameManager.ViewModels;

public class MainViewModel : BaseViewModel, IMainViewModel
{
    private readonly IIgdbController _igdbController;
    private int _count = 0;
    private bool _isLoggedIn = false;

    public int Count
    {
        get => _count;
        set => SetProperty(ref _count, value);
    }

    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set => SetProperty(ref _isLoggedIn, value);
    }

    public ICommand IncreaseCountCommand => new Command(IncreaseCount);

    public MainViewModel(IIgdbController igdbController) : base(igdbController)
    {
        _igdbController = igdbController;
    }

    private void IncreaseCount()
    {
        Count++;
        // SemanticScreenReader.Announce(CounterLabel.Text);
    }


    public async Task InitializeAsync()
    {
        if (_isLoggedIn || IsBusy)
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

}