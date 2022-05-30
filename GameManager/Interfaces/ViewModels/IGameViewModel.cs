using GameManager.Maui.Models;

namespace GameManager.Maui.Interfaces.ViewModels
{
    public interface IGameViewModel
    {
        Task InitializeAsync(ReleaseDateWithCovers game);
    }
}
