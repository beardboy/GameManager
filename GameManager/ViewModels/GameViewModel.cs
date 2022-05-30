#region Usings

using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameManager.Interfaces;
using GameManager.Interfaces.ViewModels;
using GameManager.Models;
using IGDB;
using IGDB.Models;

#endregion

namespace GameManager.ViewModels;

public partial class GameViewModel : BaseViewModel, IGameViewModel
{
    #region Fields

    private readonly IIgdbController _igdbController;

    #endregion

    #region Properties

    [ObservableProperty] ReleaseDateWithCovers game;
    [ObservableProperty] string gameArtwork;
    [ObservableProperty] string favoriteIcon;

    #endregion

    #region Constructors

    public GameViewModel(IIgdbController igdbController) : base(igdbController)
    {
        _igdbController = igdbController;
        FavoriteIcon = "heart_outline.png";
        GameArtwork = "https://images.igdb.com/igdb/image/upload/t_logo_med_2x/j0izprggutart2oqbhkh.jpg";
    }

    #endregion

    #region Commands

    [ICommand]
    async Task FavoriteGame()
    {
        FavoriteIcon = FavoriteIcon == "heart_outline.png" ? "heart.png" : "heart_outline.png";
        var isFavorite = FavoriteIcon == "heart.png";

        var path = FileSystem.AppDataDirectory;
        var favoriteGamesPath = Path.Combine(path, "favoriteGames.json");

        var favoriteGames = new List<ReleaseDateWithCovers>();

        if (File.Exists(favoriteGamesPath))
        {
            var json = await File.ReadAllTextAsync(favoriteGamesPath);
            favoriteGames.AddRange(JsonSerializer.Deserialize<List<ReleaseDateWithCovers>>(json, JsonOptions) ?? new List<ReleaseDateWithCovers>());
        }

        if (isFavorite)
        {
            if (favoriteGames.Any(favoriteGame => favoriteGame.Id == Game.Id))
            {
                return;
            }

            favoriteGames?.Add(Game);
            await File.WriteAllTextAsync(favoriteGamesPath, JsonSerializer.Serialize(favoriteGames.OrderByDescending(r => r.Name), JsonOptions));
        }
        else
        {
            var newFavorites = favoriteGames?.Where(favoriteGame => favoriteGame.Id != Game.Id).ToList();
            await File.WriteAllTextAsync(favoriteGamesPath, JsonSerializer.Serialize(newFavorites.OrderByDescending(r => r.Name), JsonOptions));
        }

    }

    #endregion

    #region Methods

    public async Task InitializeAsync(ReleaseDateWithCovers game)
    {
        var companies = await _igdbController.GetAsync<InvolvedCompany>(
            IGDBClient.Endpoints.InvolvedCompanies,
            $"fields company.*, game.*; where game = ({game.Id});",
            500);

        var company = companies?.LastOrDefault()?.Company?.Value?.Name ?? string.Empty;
        game.Company = company;

        Game = game;
        await SetIsFavorite();

        //var game = new IGDB.Models.Game();
        //var artwork = game.Artworks.Values.FirstOrDefault().ImageId;
        var artwork = "widmy38l57vkcqsijgjt";
        var logoMed = IGDB.ImageHelper.GetImageUrl(artwork, ImageSize.LogoMed, true);

    }

    private async Task SetIsFavorite()
    {
        var path = FileSystem.AppDataDirectory;
        var favoriteGamesPath = Path.Combine(path, "favoriteGames.json");

        var favoriteGames = new List<ReleaseDateWithCovers>();

        if (File.Exists(favoriteGamesPath))
        {
            var json = await File.ReadAllTextAsync(favoriteGamesPath);
            favoriteGames.AddRange(JsonSerializer.Deserialize<List<ReleaseDateWithCovers>>(json, JsonOptions) ?? new List<ReleaseDateWithCovers>());
        }

        FavoriteIcon = favoriteGames.Any(f => f.Id == Game.Id) 
            ? "heart.png" 
            : "heart_outline.png";
    }

    #endregion
}
