using GameManager.Interfaces;
using IGDB;

namespace GameManager.Controller;

public class IgdbController : IIgdbController
{
    // private readonly string IGDB_CLIENT_ID = "ENTER TWITCH CLIENT ID";
    // private readonly string IGDB_CLIENT_SECRET = "ENTER TWITCH CLIENT SECRET";
    private readonly string IGDB_CLIENT_ID = "y1ycbde2zc3hhf7de296kfoujq6qt3";
    private readonly string IGDB_CLIENT_SECRET = "nq0l8wzf2spx2cj5jq07s49ml33u1d";

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

    public string GetImage(string id)
    {
        return ImageHelper.GetImageUrl(imageId: id, size: ImageSize.ScreenshotHuge, retina: true);
    }

}