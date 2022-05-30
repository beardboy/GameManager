namespace GameManager.Interfaces.ViewModels;

public interface IHomeViewModel
{
    Task InitializeAsync();

    void OnNavigatedTo(NavigatedToEventArgs args);
}