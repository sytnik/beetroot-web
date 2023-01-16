using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Routing;

namespace BlazorApp2.Client;

public sealed partial class App
{
    private async Task OnNavigateAsync(NavigationContext args)
    {
        var auth = await Js.GetItemAsync<string>("isauthenticated");
        var user = (await (_authStateProvider as CustomAuthStateProvider)?
            .GetAuthenticationStateAsync()!).User;
        if (!string.IsNullOrEmpty(auth) && user.Identity is {IsAuthenticated: false})
        {
            var manager = await Http.GetFromJsonAsync<Manager>("getmanager");
            if (manager is not null)
                (_authStateProvider as CustomAuthStateProvider)?.SetAuthInfo(manager);
            else await Js.RemoveItemAsync("isauthenticated");
        }
    }
}