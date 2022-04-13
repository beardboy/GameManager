using GameManager.Interfaces;
using IGDB;

namespace GameManager.Controller;

public class IgdbController : IIgdbController
{
    private readonly string IGDB_CLIENT_ID = "ENTER TWITCH CLIENT ID";
    private readonly string IGDB_CLIENT_SECRET = "ENTER TWITCH CLIENT SECRET";

    public IGDBClient Client { get; set; }

    public IgdbController()
    {
        Client = new IGDBClient(IGDB_CLIENT_ID, IGDB_CLIENT_SECRET, new MauiTokenStore());
    }

    public async Task<List<T>> GetAsync<T>(string endpoint, string query = "", int limit = 20)
    {
        var newQuery = string.IsNullOrEmpty(query) ? $"fields *; limit {limit};" : $"{query} limit {limit};";
        var t = await Client.QueryAsync<T>(endpoint, string.IsNullOrEmpty(query) ? $"fields *; limit {limit};" : $"{query} limit {limit};");
        return t.ToList();
    }

    public async Task<int> GetCountAsync(string endpoint)
    {
        var result = await Client.CountAsync(endpoint, "");
        return result.Count;
    }

}