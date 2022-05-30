#region Usings

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameManager.Maui.Interfaces;
using GameManager.Maui.Interfaces.ViewModels;
using GameManager.Maui.Models;
using IGDB;
using IGDB.Models;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;

#endregion

namespace GameManager.Maui.ViewModels;

public partial class HomeViewModel : BaseViewModel, IHomeViewModel
{
    #region Fields

    private readonly IIgdbController _igdbController;

    #endregion

    #region Properties

    [ObservableProperty] JsonSerializerOptions jsonOptions;
    [ObservableProperty] IEnumerable<ReleaseDateWithCovers> newGames;
    [ObservableProperty] IEnumerable<ReleaseDateWithCovers> popularGames;
    [ObservableProperty] ReleaseDateWithCovers popularGame;
    [ObservableProperty] ReleaseDateWithCovers newGame;

    #endregion

    #region Constructors

    public HomeViewModel(IIgdbController igdbController) : base(igdbController)
    {
        _igdbController = igdbController;
        JsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

        NewGames = new ObservableCollection<ReleaseDateWithCovers>();
        PopularGames = new ObservableCollection<ReleaseDateWithCovers>();
    }

    #endregion

    #region Commands

    [ICommand]
    protected override async Task SelectedGame(object param)
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
    async Task RefreshGames()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;

        try
        {
            await GetNewReleases();
            await GetPopularGames();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        IsBusy = false;
    }

    #endregion

    #region Methods

    public async Task InitializeAsync()
    {
        // await RefreshGames();
    }

    public void OnNavigatedTo(NavigatedToEventArgs args)
    {
        Task.Run(async () => await RefreshGames());
    }

    private async Task GetPopularGames()
    {
        var path = FileSystem.AppDataDirectory;
        var popularGamesPath = Path.Combine(path, "popularGames.json");

        if (File.Exists(popularGamesPath))
        {
            var lastUpdated = File.GetLastWriteTime(popularGamesPath);
            var timeDiff = (DateTime.Now - lastUpdated).TotalHours;

            if (timeDiff < 24 && PopularGames.Count() > 0)
            {
                return;
            }

            var json = await File.ReadAllTextAsync(popularGamesPath);
            var popularDateWithCoversLoaded = JsonSerializer.Deserialize<List<ReleaseDateWithCovers>>(json, JsonOptions);

            if (popularDateWithCoversLoaded?.Count > 0 && timeDiff < 24)
            {
                PopularGames = new ObservableCollection<ReleaseDateWithCovers>(popularDateWithCoversLoaded.OrderByDescending(r => r.Rating).Take(40));
                return;
            }
        }

        var popularGames = await _igdbController.GetAsync<Game>(
            IGDBClient.Endpoints.Games,
            $"fields id, name, summary, involved_companies, rating, artworks.image_id, release_dates.*; where rating > 75 & platforms = 130; sort rating desc;",
            100);

        var gameIds = new List<long>();
        gameIds.AddRange(from popularGame in popularGames
                         where popularGame.Id != null
                         select popularGame.Id ?? 0);

        var gameIdsString = string.Join("','", gameIds);

        var covers2 = await _igdbController.GetAsync<Cover>(
            IGDBClient.Endpoints.Covers,
            $"fields id, game, image_id; where game = ({gameIdsString});",
            100);

        var artworks2 = await _igdbController.GetAsync<Artwork>(
            IGDBClient.Endpoints.Artworks,
            $"fields image_id, game; where game = ({gameIdsString});",
            500);

        var involvedCompanies2 = await _igdbController.GetAsync<InvolvedCompany>(
            IGDBClient.Endpoints.InvolvedCompanies,
            $"fields company.*, game.*; where game = ({gameIdsString});",
            500);

        var smash = popularGames.FirstOrDefault(g => g.Id == 90101);
        var smashFirstReleaseDate = smash.FirstReleaseDate;
        var smashDate = smash.ReleaseDates.Values.FirstOrDefault(g => g.Platform.Id == 130);
        

        var popularDateWithCovers = (from game in popularGames
                                     let cover = covers2.FirstOrDefault(c => c?.Game?.Id == game?.Id)?.ImageId ?? string.Empty
                                     let artwork = artworks2.FirstOrDefault(c => c?.Game?.Id == game?.Id)?.ImageId ?? string.Empty
                                     let company = involvedCompanies2.FirstOrDefault(c => c?.Game?.Id == game?.Id)?.Company?.Value?.Name ??
                                                   string.Empty
                                     where !string.IsNullOrEmpty(cover)
                                     select new ReleaseDateWithCovers
                                     {
                                         Id = game.Id ?? 0,
                                         CoverUrl = $"https://images.igdb.com/igdb/image/upload/t_cover_big/{cover}.jpg",
                                         BannerUrl = $"https://images.igdb.com/igdb/image/upload/t_screenshot_huge_2x/{artwork}.jpg",
                                         Date = game?.ReleaseDates?.Values?.FirstOrDefault(g => g.Platform.Id == 130)?.Date ?? DateTimeOffset.Now.AddMonths(-1),
                                         Rating = game.Rating ?? 0,
                                         Name = game.Name,
                                         Summary = game.Summary,
                                         Company = company,
                                     }).ToList();

        if (popularDateWithCovers != null && popularDateWithCovers.Any())
        {
            PopularGames = new ObservableCollection<ReleaseDateWithCovers>(popularDateWithCovers.OrderByDescending(r => r.Rating).Take(40));
            await File.WriteAllTextAsync(popularGamesPath, JsonSerializer.Serialize(popularDateWithCovers.OrderByDescending(r => r.Rating).Take(40), JsonOptions));
        }
    }

    private async Task GetNewReleases()
    {
        var path = FileSystem.AppDataDirectory;
        var newGamesPath = Path.Combine(path, "newGames.json");

        if (File.Exists(newGamesPath))
        {
            var lastUpdated = File.GetLastWriteTime(newGamesPath);
            var timeDiff = (DateTime.Now - lastUpdated).TotalHours;

            if (timeDiff < 24 && NewGames.Count() > 0)
            {
                return;
            }

            var json = await File.ReadAllTextAsync(newGamesPath);
            var releaseDateWithCoversLoaded = JsonSerializer.Deserialize<List<ReleaseDateWithCovers>>(json, JsonOptions);

            if (releaseDateWithCoversLoaded?.Count > 0 && timeDiff < 24)
            {
                NewGames = new ObservableCollection<ReleaseDateWithCovers>(releaseDateWithCoversLoaded.OrderByDescending(r => r.Date).Take(50));
                return;
            }
        }

        var newReleases = await _igdbController.GetAsync<ReleaseDate>(IGDBClient.Endpoints.ReleaseDates,
            $"fields id, date, game.id; where date < {DateTimeOffset.UtcNow.ToUnixTimeSeconds()} & platform = 130; sort date desc;",
            500);

        var gameIds = (from releaseDate in newReleases
                       where releaseDate.Game.Value.Id != null
                       select releaseDate.Game.Value.Id ?? 0).ToList();

        var gameIdsString = string.Join("','", gameIds);

        var releaseGames = await _igdbController.GetAsync<Game>(
            IGDBClient.Endpoints.Games,
            $"fields id, name, summary, involved_companies, rating, artworks.image_id; where id = ({gameIdsString}) & platforms = 130;",
            500);

        var covers = await _igdbController.GetAsync<Cover>(
            IGDBClient.Endpoints.Covers,
            $"fields id, game, image_id; where game = ({gameIdsString});",
            500);

        var artworks = await _igdbController.GetAsync<Artwork>(
            IGDBClient.Endpoints.Artworks,
            $"fields image_id, game; where game = ({gameIdsString});",
            500);

        var involvedCompanies = await _igdbController.GetAsync<InvolvedCompany>(
            IGDBClient.Endpoints.InvolvedCompanies,
            $"fields company.*, game.*; where game = ({gameIdsString});",
            500);

        var releaseDateWithCovers = (from game in releaseGames
                                     let cover = covers.FirstOrDefault(c => c?.Game?.Id == game?.Id)?.ImageId ?? string.Empty
                                     let artwork = artworks.FirstOrDefault(c => c?.Game?.Id == game?.Id)?.ImageId ?? string.Empty
                                     let company = involvedCompanies.FirstOrDefault(c => c?.Game?.Id == game?.Id)?.Company?.Value?.Name ??
                                                   string.Empty
                                     let date = newReleases.FirstOrDefault(n => n?.Game?.Value?.Id == game?.Id)?.Date
                                     where date != null && !string.IsNullOrEmpty(cover)
                                     select new ReleaseDateWithCovers
                                     {
                                         Id = game.Id ?? 0,
                                         CoverUrl = $"https://images.igdb.com/igdb/image/upload/t_cover_big/{cover}.jpg",
                                         BannerUrl = $"https://images.igdb.com/igdb/image/upload/t_screenshot_huge_2x/{artwork}.jpg",
                                         Date = date ?? DateTimeOffset.MinValue,
                                         Name = game.Name,
                                         Summary = game.Summary,
                                         Company = company,
                                     }).ToList();

        if (releaseDateWithCovers != null && releaseDateWithCovers.Any())
        {
            NewGames = new ObservableCollection<ReleaseDateWithCovers>(releaseDateWithCovers.OrderByDescending(r => r.Date).Take(50));
            await File.WriteAllTextAsync(newGamesPath, JsonSerializer.Serialize(releaseDateWithCovers.OrderByDescending(r => r.Date).Take(50), JsonOptions));
        }
    }

    #endregion
}