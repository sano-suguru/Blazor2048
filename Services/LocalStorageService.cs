using System.Text.Json;
using Microsoft.JSInterop;

namespace Blazor2048.Services;

public class LocalStorageService(IJSRuntime jsRuntime) : ILocalStorageService
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;

    public async ValueTask<T?> GetItemAsync<T>(string key)
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
            return json == null ? default : JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception)
        {
            return default;
        }
    }

    public async ValueTask SetItemAsync<T>(string key, T value)
    {
        await _jsRuntime.InvokeVoidAsync(
            "localStorage.setItem",
            key,
            JsonSerializer.Serialize(value)
        );
    }

    public async ValueTask RemoveItemAsync(string key)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }
}
