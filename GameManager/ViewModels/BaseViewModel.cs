#region Usings

using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameManager.Interfaces;
using GameManager.Models;
using IGDB;
using IGDB.Models;

#endregion


namespace GameManager.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    #region Fields

    private readonly IIgdbController _igdbController;

    #endregion

    #region Properties

    [ObservableProperty] bool isBusy;
    [ObservableProperty] bool isSearchGamesVisible;
    [ObservableProperty] bool isSearchBarVisible;
    [ObservableProperty] bool isFilterVisible;
    [ObservableProperty] string title;
    [ObservableProperty] List<ReleaseDateWithCovers> searchGames;
    [ObservableProperty] ReleaseDateWithCovers searchGame;
    [ObservableProperty] JsonSerializerOptions jsonOptions;

    private string _searchBarText;

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

    #endregion

    #region Constructors

    public BaseViewModel(IIgdbController igdbController)
    {
        _igdbController = igdbController;
        JsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

        SearchGames = new List<ReleaseDateWithCovers>();
    }

    #endregion

    #region Commands

    [ICommand]
    protected virtual async Task SelectedGame(object param)
    {
        var game = param as ReleaseDateWithCovers;

        var navigationParameter = new Dictionary<string, object>
        {
            { "Game", game }
        };

        await Shell.Current.GoToAsync("game", navigationParameter);
        return;
    }

    [ICommand]
    void ToggleSearch()
    {
        IsSearchBarVisible = !IsSearchBarVisible;
        if (!IsSearchBarVisible)
        {
            IsSearchGamesVisible = false;
        }
    }

    [ICommand]
    void ToggleFilter()
    {
        IsFilterVisible = !IsFilterVisible;
    }

    [ICommand]
    async Task Search()
    {
        SearchGames = new List<ReleaseDateWithCovers>();

        if (string.IsNullOrEmpty(_searchBarText))
        {
            IsSearchGamesVisible = false;
            return;
        }

        IsSearchGamesVisible = true;

        var games = await _igdbController.GetAsync<Game>(
            IGDBClient.Endpoints.Games,
            $"search \"{_searchBarText}\"; fields id, name, summary, involved_companies, rating, artworks.image_id; where platforms = 130;",
            100);

        if (!games.Any())
        {
            return;
        }

        var gameIds = (from game in games
                       where game.Id != null
                       select game.Id ?? 0).ToList();

        var gameIdsString = string.Join("','", gameIds);

        var covers = await _igdbController.GetAsync<Cover>(
            IGDBClient.Endpoints.Covers,
            $"fields id, game, image_id; where game = ({gameIdsString});",
            100);

        var artworks = await _igdbController.GetAsync<Artwork>(
            IGDBClient.Endpoints.Artworks,
            $"fields image_id, game; where game = ({gameIdsString});",
            500);

        var involvedCompanies = await _igdbController.GetAsync<InvolvedCompany>(
            IGDBClient.Endpoints.InvolvedCompanies,
            $"fields company.*, game.*; where game = ({gameIdsString});",
            500);

        var releases = await _igdbController.GetAsync<ReleaseDate>(IGDBClient.Endpoints.ReleaseDates,
            $"fields id, date, game; where game = ({gameIdsString});",
            100);

        var newGamesList = (from game in games
                            let cover = covers.FirstOrDefault(c => c?.Game?.Id == game?.Id)?.ImageId ?? string.Empty
                            let artwork = artworks.FirstOrDefault(c => c?.Game?.Id == game?.Id)?.ImageId ?? string.Empty
                            let release = releases.FirstOrDefault(r => r?.Game?.Id == game?.Id)?.Date ?? DateTimeOffset.Now.AddMonths(-1)
                            let company = involvedCompanies.FirstOrDefault(c => c?.Game?.Id == game?.Id)?.Company?.Value?.Name ?? string.Empty
                            where !string.IsNullOrEmpty(cover)
                            select new ReleaseDateWithCovers
                            {
                                BannerUrl = $"https://images.igdb.com/igdb/image/upload/t_screenshot_huge_2x/{artwork}.jpg",
                                CoverUrl = $"https://images.igdb.com/igdb/image/upload/t_cover_big/{cover}.jpg",
                                Date = release,
                                Rating = game?.Rating ?? 0,
                                Name = game?.Name ?? String.Empty,
                                Summary = game.Summary,
                                Company = company,
                            }).ToList();

        SearchGames = newGamesList;
    }

    #endregion

}