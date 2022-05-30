using GameManager.Maui.Interfaces;
using GameManager.Maui.Interfaces.ViewModels;

namespace GameManager.Maui.ViewModels;

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