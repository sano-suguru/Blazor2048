public interface ILocalStorageService
{
    ValueTask<T?> GetItemAsync<T>(string key);
    ValueTask SetItemAsync<T>(string key, T value);
    ValueTask RemoveItemAsync(string key);
}
