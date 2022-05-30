#region Usings

using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameManager.Interfaces;
using GameManager.Interfaces.ViewModels;
using GameManager.Models;

#endregion

namespace GameManager.ViewModels;

public partial class FavoritesViewModel : BaseViewModel, IFavoritesViewModel
{
    #region Fields

    private readonly IIgdbController _igdbController;

    #endregion

    #region Properties

    [ObservableProperty] JsonSerializerOptions jsonOptions;
    [ObservableProperty] IEnumerable<ReleaseDateWithCovers> favoriteGames;
    [ObservableProperty] ReleaseDateWithCovers favoriteGame;

    #endregion

    #region Constructors

    public FavoritesViewModel(IIgdbController igdbController) : base(igdbController)
    {
        _igdbController = igdbController;
        JsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

        // IsSearchGamesVisible = false;
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

    #endregion

    #region Methods

    public async Task InitializeAsync()
    {
        var path = FileSystem.AppDataDirectory;
        var favoriteGamesPath = Path.Combine(path, "favoriteGames.json");

        var favoriteGames = new List<ReleaseDateWithCovers>();

        if (File.Exists(favoriteGamesPath))
        {
            var json = await File.ReadAllTextAsync(favoriteGamesPath);
            favoriteGames.AddRange(JsonSerializer.Deserialize<List<ReleaseDateWithCovers>>(json, JsonOptions) ?? new List<ReleaseDateWithCovers>());
            FavoriteGames = new ObservableCollection<ReleaseDateWithCovers>(favoriteGames.OrderBy(r => r.Name));
        }
    }

    #endregion

}