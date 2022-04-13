using GameManager.Interfaces;
using GameManager.Models;
using IGDB;
using IGDB.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;

namespace GameManager.ViewModels;

public class BaseViewModel : INotifyPropertyChanged
{
    private bool _isBusy;
    private string _title;
    private readonly IIgdbController _igdbController;
    private readonly Command _searchCommand;
    private ObservableCollection<ReleaseDateWithCovers> _searchGames;
    private string _searchBarText;
    private bool _isSearchGamesVisible;
    private bool _isSearchBarVisible;
    private Command _toggleSearchCommand;
    private JsonSerializerOptions _jsonSerializerOptions;

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public ObservableCollection<ReleaseDateWithCovers> SearchGames
    {
        get => _searchGames;
        set => SetProperty(ref _searchGames, value);
    }

    public JsonSerializerOptions JsonSerializerOptions
    {
        get => _jsonSerializerOptions;
        set => SetProperty(ref _jsonSerializerOptions, value);
    }

    public bool IsSearchBarVisible
    {
        get => _isSearchBarVisible;
        set => SetProperty(ref _isSearchBarVisible, value);
    }

    public bool IsSearchGamesVisible
    {
        get => _isSearchGamesVisible;
        set => SetProperty(ref _isSearchGamesVisible, value);
    }

    public string SearchBarText
    {
        get => _searchBarText;
        set
        {
            _searchBarText = value;
            if (string.IsNullOrEmpty(_searchBarText))
            {
                IsSearchGamesVisible = false;
                SearchGames.Clear();
                return;
            }

            //SearchCommand.Execute(null);
        }
    }

    public ICommand SearchCommand => _searchCommand ?? new Command(async () => await Search());

    public ICommand ToggleSearchCommand => _toggleSearchCommand ?? new Command(() => ToggleSearch());

    public BaseViewModel(IIgdbController igdbController)
    {
        _igdbController = igdbController;
        _jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

        SearchGames = new ObservableCollection<ReleaseDateWithCovers>();
    }

    private void ToggleSearch()
    {
        IsSearchBarVisible = !IsSearchBarVisible;
        if (!IsSearchBarVisible)
        {
            IsSearchGamesVisible = false;
        }
    }

    private async Task Search()
    {
        if (string.IsNullOrEmpty(_searchBarText))
        {
            SearchGames.Clear();
            IsSearchGamesVisible = false;
            return;
        }

        IsSearchGamesVisible = true;
        SearchGames.Clear();
        var searchGames = await _igdbController.GetAsync<Game>(
            IGDBClient.Endpoints.Games,
            $"search \"{_searchBarText}\"; fields id, name, rating, artworks.image_id; where platforms = 130;",
            100);

        if (!searchGames.Any())
        {
            return;
        }

        var gameIds = new List<long>();
        foreach (var game in searchGames)
        {
            if (game.Id != null)
            {
                gameIds.Add(game.Id ?? 0);
            }
        }
        var gameIdsString = string.Join("','", gameIds);

        var covers = await _igdbController.GetAsync<Cover>(
            IGDBClient.Endpoints.Covers,
            $"fields id, game, image_id; where game = ({gameIdsString});",
            100);

        var releases = await _igdbController.GetAsync<ReleaseDate>(IGDBClient.Endpoints.ReleaseDates,
            $"fields id, date, game; where game = ({gameIdsString});",
            100);

        foreach (var game in searchGames)
        {
            var cover = covers.FirstOrDefault(c => c?.Game?.Id == game?.Id)?.ImageId ?? string.Empty;
            var release = releases.FirstOrDefault(r => r?.Game?.Id == game?.Id)?.Date ?? DateTimeOffset.Now.AddMonths(-1);

            if (!string.IsNullOrEmpty(cover))
            {
                SearchGames.Add(new ReleaseDateWithCovers
                {
                    CoverUrl = $"https://images.igdb.com/igdb/image/upload/t_cover_big/{cover}.jpg",
                    Date = release,
                    Rating = game?.Rating ?? 0,
                    Name = game?.Name ?? String.Empty,
                });
            }
        }
    }

    #region INotifyPropertyChanged

    protected bool SetProperty<T>(
        ref T backingStore,
        T value,
        [CallerMemberName] string propertyName = "",
        Action onChanged = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
        {
            return false;
        }

        backingStore = value;
        onChanged?.Invoke();
        OnPropertyChanged(propertyName);
        return true;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        var changed = PropertyChanged;
        if (changed == null)
            return;

        changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

}