using GameManager.Models;

namespace GameManager.Interfaces.ViewModels
{
    public interface IGameViewModel
    {
        Task InitializeAsync(ReleaseDateWithCovers game);
    }
}
