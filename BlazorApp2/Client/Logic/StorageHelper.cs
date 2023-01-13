namespace BlazorApp2.Client.Logic;

public static class StorageHelper
{
    public static async Task<T> GetItemAsync<T>(this IJSRuntime runtime, string key)
    {
        var data = await runtime.InvokeAsync<string>("localStorage.getItem", key);
        if (string.IsNullOrWhiteSpace(data)) return default;
        try
        {
            return JsonSerializer.Deserialize<T>(data);
        }
        catch
        {
            return (T)(object)data;
        }
    }

    public static async Task SetItemAsync<T>(this IJSRuntime runtime, string key, T data) =>
        await runtime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(data));
    
    public static async Task RemoveItemAsync(this IJSRuntime runtime, string key) =>
        await runtime.InvokeVoidAsync("localStorage.removeItem", key);
}