using GameManager.Interfaces;
using GameManager.Interfaces.ViewModels;
using GameManager.Models;
using IGDB;
using IGDB.Models;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;

namespace GameManager.ViewModels;

public class HomeViewModel : BaseViewModel, IHomeViewModel
{
    private IIgdbController _igdbController;
    private readonly Command _refreshGamesCommand;
    private JsonSerializerOptions _jsonSerializerOptions;

    //private ObservableCollection<Cover> _covers;
    private ObservableCollection<ReleaseDateWithCovers> _newGames;
    private ObservableCollection<ReleaseDateWithCovers> _popularGames;

    public ObservableCollection<ReleaseDateWithCovers> NewGames
    {
        get => _newGames;
        set => SetProperty(ref _newGames, value);
    }

    public ObservableCollection<ReleaseDateWithCovers> PopularGames
    {
        get => _popularGames;
        set => SetProperty(ref _popularGames, value);
    }

    public ICommand RefreshGamesCommand => _refreshGamesCommand ?? new Command(async () => await RefreshGames());



    //public ObservableCollection<Cover> Covers 
    //{
    //    get => _covers;
    //    set => SetProperty(ref _covers, value); 
    //}

    public HomeViewModel(IIgdbController igdbController) : base(igdbController)
    {
        _igdbController = igdbController;
        _jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

        NewGames = new ObservableCollection<ReleaseDateWithCovers>();
        PopularGames = new ObservableCollection<ReleaseDateWithCovers>();
    }

    public async Task InitializeAsync()
    {
        await RefreshGames();
    }

    public async Task RefreshGames()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;

        var path = FileSystem.AppDataDirectory;
        var newGamesPath = Path.Combine(path, "newGames.json");
        var popularGamesPath = Path.Combine(path, "popularGames.json");

        //if (File.Exists(newGamesPath))
        //{
        //    try
        //    {
        //        var result = await FilePicker.PickAsync(PickOptions.Default);
        //        if (result != null)
        //        {
        //            string text = "";
        //            var stream = await result.OpenReadAsync();
        //            stream.Position = 0;
        //            using (var stringReader = new StreamReader(stream, encoding: System.Text.Encoding.UTF8))
        //            text = stringReader.ReadToEnd();

        //            //var newGamesString = await File.ReadAllTextAsync(path);
        //            var newGamesString = text;

        //            var newGames = JsonSerializer.Deserialize<List<ReleaseDateWithCovers>>(newGamesString, _jsonSerializerOptions);

        //            if (newGames != null)
        //            {
        //                NewGames = new ObservableCollection<ReleaseDateWithCovers>(newGames);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // The user canceled or something went wrong
        //    }
        //}


        //if (File.Exists(popularGamesPath))
        //{
        //    var popularGamesString = await File.ReadAllTextAsync(path);
        //    var popularGames = JsonSerializer.Deserialize<List<Game>>(popularGamesString, _jsonSerializerOptions);

        //    if (popularGames != null)
        //    {
        //        PopularGames = new ObservableCollection<Game>(popularGames);
        //    }
        //}

        try
        {
            var newReleases = await _igdbController.GetAsync<ReleaseDate>(IGDBClient.Endpoints.ReleaseDates,
                $"fields id, date, game.id; where date < {DateTimeOffset.UtcNow.ToUnixTimeSeconds()} & platform = 130; sort date desc;",
                500);

            var gameIds = new List<long>();
            foreach (var releaseDate in newReleases)
            {
                if (releaseDate.Game.Value.Id != null)
                {
                    gameIds.Add(releaseDate.Game.Value.Id ?? 0);
                }
            }
            var gameIdsString = string.Join("','", gameIds);

            var releaseGames = await _igdbController.GetAsync<Game>(
                IGDBClient.Endpoints.Games,
                $"fields id, name, rating, artworks.image_id; where id = ({gameIdsString}) & platforms = 130;",
                500);

            var covers = await _igdbController.GetAsync<Cover>(
                IGDBClient.Endpoints.Covers,
                $"fields id, game, image_id; where game = ({gameIdsString});",
                500);

            var releaseDateWithCovers = new List<ReleaseDateWithCovers>();
            foreach (var game in releaseGames)
            {
                var cover = covers.FirstOrDefault(c => c?.Game?.Id == game?.Id)?.ImageId ?? string.Empty;
                var date = newReleases.FirstOrDefault(n => n?.Game?.Value?.Id == game?.Id)?.Date;

                if (date != null && !string.IsNullOrEmpty(cover))
                {
                    releaseDateWithCovers.Add(new ReleaseDateWithCovers
                    {
                        CoverUrl = $"https://images.igdb.com/igdb/image/upload/t_cover_big/{cover}.jpg",
                        Date = date ?? DateTimeOffset.MinValue,
                        Name = game.Name,
                    });
                }
            }

            var popularGames = await _igdbController.GetAsync<Game>(
                IGDBClient.Endpoints.Games,
                $"fields id, name, rating, artworks.image_id; where rating > 75 & platforms = 130; sort rating desc;",
                100);

            gameIds.Clear();
            foreach (var popularGame in popularGames)
            {
                if (popularGame.Id != null)
                {
                    gameIds.Add(popularGame.Id ?? 0);
                }
            }
            gameIdsString = string.Join("','", gameIds);

            var covers2 = await _igdbController.GetAsync<Cover>(
                IGDBClient.Endpoints.Covers,
                $"fields id, game, image_id; where game = ({gameIdsString});",
                100);

            var popularDateWithCovers = new List<ReleaseDateWithCovers>();
            foreach (var game in popularGames)
            {
                var cover = covers2.FirstOrDefault(c => c?.Game?.Id == game?.Id)?.ImageId ?? string.Empty;

                if (!string.IsNullOrEmpty(cover))
                {
                    popularDateWithCovers.Add(new ReleaseDateWithCovers
                    {
                        CoverUrl = $"https://images.igdb.com/igdb/image/upload/t_cover_big/{cover}.jpg",
                        Date = DateTimeOffset.Now.AddMonths(-1),
                        Rating = game.Rating ?? 0,
                        Name = game.Name,
                    });
                }
            }

            //var covers = await _igdbController.GetAsync<Cover>(IGDBClient.Endpoints.Covers,
            //    $"fields id,url,height,width,game.*; where game = ({string.Join(", ", gameIds)});");

            //foreach (var cover in covers)
            //{
            //    var date = newReleases.FirstOrDefault(n => n.Game.Value.Id == cover.Game.Value.Id).Date;

            //    releaseDateWithCovers.Add(new ReleaseDateWithCovers
            //    {
            //        CoverUrl = $"http:{cover.Url}",
            //        Date = date ?? DateTimeOffset.MinValue,
            //        Name = cover.Game.Value.Name,
            //    });
            //}


            if (newReleases != null && newReleases.Any())
            {
                NewGames = new ObservableCollection<ReleaseDateWithCovers>(releaseDateWithCovers.OrderByDescending(r => r.Date).Take(50));
                PopularGames = new ObservableCollection<ReleaseDateWithCovers>(popularDateWithCovers.OrderByDescending(r => r.Rating).Take(40));

                await File.WriteAllTextAsync(Path.Combine(path, "newGames.json"), JsonSerializer.Serialize(releaseDateWithCovers, _jsonSerializerOptions));
            }
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
        }

        IsBusy = false;
    }
}