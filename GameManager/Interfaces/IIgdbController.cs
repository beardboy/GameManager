namespace GameManager.Interfaces;

public interface IIgdbController
{
    Task<int> GetCountAsync(string endpoint);

    Task<List<T>> GetAsync<T>(string endpoint, string query = "", int limit = 20);

    string GetImage(string id);
}