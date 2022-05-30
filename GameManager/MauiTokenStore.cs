using System.Text.Json;
using IGDB;

namespace GameManager;

public class MauiTokenStore : ITokenStore
{
    private static TwitchAccessToken CurrentToken { get; set; }

    public async Task<TwitchAccessToken> GetTokenAsync()
    {
        var tokenString = await SecureStorage.GetAsync("token");
        if (string.IsNullOrEmpty(tokenString))
        {
            return null;
        }

        var token = JsonSerializer.Deserialize<TwitchAccessToken>(tokenString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });

        return token;
    }

    public async Task<TwitchAccessToken> StoreTokenAsync(TwitchAccessToken token)
    {
        var tokenString = JsonSerializer.Serialize(token, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        await SecureStorage.SetAsync("token", tokenString);
        return token;
    }
}