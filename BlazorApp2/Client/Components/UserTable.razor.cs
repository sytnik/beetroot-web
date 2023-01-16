using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace BlazorApp2.Client.Components;

public sealed partial class UserTable
{
    [Inject] public HttpClient _httpClient { get; set; }
    [Parameter] public List<User> Users { get; set; }
    private User _currentUser;
    private void SelectUser(User user) => _currentUser = user;

    private async Task DeleteUser(User user)
    {
        if(!await Js.InvokeAsync<bool>("confirm", $"Are you sure to delete {user.FullName}?")) return;
        await _httpClient.DeleteAsync($"deleteuser?id={user.Id}");
        Users = await GetUsers();
    }
    
    private async Task<List<User>> GetUsers() => await _httpClient.GetFromJsonAsync<List<User>>("users");
}