namespace SteamAuthenticatorOnline.Manager.Services;

/// <summary>
/// Http方法扩展
/// </summary>
public sealed class HttpService
{
    private readonly HttpClient _httpClient;

    public HttpService()
    {
        _httpClient = new();
    }

    public async Task<string> GetProxyFromUrl(Uri uri)
    {
        var response = await _httpClient.GetAsync(uri);
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
}