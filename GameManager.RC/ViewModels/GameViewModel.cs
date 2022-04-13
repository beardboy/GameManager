using GameManager.Interfaces;
using GameManager.Interfaces.ViewModels;
using IGDB;
using System.Windows.Input;

namespace GameManager.ViewModels
{
    public class GameViewModel : BaseViewModel, IGameViewModel
    {
        private string _gameArtwork;
        public string GameArtwork
        {
            get => _gameArtwork;
            set => SetProperty(ref _gameArtwork, value);
        }

        private string _favoriteIcon;
        public string FavoriteIcon
        {
            get => _favoriteIcon;
            set => SetProperty(ref _favoriteIcon, value);
        }

        private ICommand _favoriteCommand;
        public ICommand FavoriteCommand => _favoriteCommand ?? new Command(() => FavoriteGame());

        private void FavoriteGame()
        {
            FavoriteIcon = FavoriteIcon == "heart_outline.png" ? "heart.png" : "heart_outline.png";
        }

        public GameViewModel(IIgdbController igdbController) : base(igdbController)
        {
            FavoriteIcon = "heart_outline.png";
            GameArtwork = "https://images.igdb.com/igdb/image/upload/t_logo_med_2x/j0izprggutart2oqbhkh.jpg";
        }

        public Task InitializeAsync()
        {
            //var game = new IGDB.Models.Game();
            //var artwork = game.Artworks.Values.FirstOrDefault().ImageId;
            var artwork = "widmy38l57vkcqsijgjt";
            var logoMed = IGDB.ImageHelper.GetImageUrl(artwork, ImageSize.LogoMed, true);

            return Task.CompletedTask;
        }
    }
}
