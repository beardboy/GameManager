using GameManager.Interfaces;
using GameManager.Interfaces.ViewModels;

namespace GameManager.ViewModels;

public class HeaderViewModel : BaseViewModel, IHeaderViewModel
{
    private readonly IIgdbController _igdbController;

    public HeaderViewModel(IIgdbController igdbController) : base(igdbController)
    {
        _igdbController = igdbController;
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }
}