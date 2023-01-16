using System.Net.Http.Json;

namespace BlazorApp2.Client.Pages;

public sealed partial class Index
{
    private List<User> _users;
    private User _currentUser = new();
    private string _state = "Please input user data";

    protected override async Task OnInitializedAsync() => _users = await GetUsers();

    private async Task<List<User>> GetUsers() => await Http.GetFromJsonAsync<List<User>>("users");

    private async Task AddUser()
    {
        var isValid = true;
        if (_currentUser.Id < 1)
        {
            _state = "Id is not valid";
            isValid = false;
        }
        if (string.IsNullOrWhiteSpace(_currentUser.FirstName))
        {
            _state = "FirstName is not valid";
            isValid = false;
        }
        if (!isValid) return;
        var result = await Http.PutAsJsonAsync("createuser", _currentUser);
        _state = result.IsSuccessStatusCode ? "User added" : "User add error";
        if (!result.IsSuccessStatusCode) return;
        _users = await GetUsers();
    }

    private async Task EditUser()
    {
        await Http.PatchAsJsonAsync("updateuser", _currentUser);
        _users = await GetUsers();
    }

    private void AddInfo() => _currentUser.Info += "button clicked!";

    private async Task LogoutHandler()
    {
        var status = await Http.GetAsync("logout");
        if (status.IsSuccessStatusCode)
        {
            (_authStateProvider as CustomAuthStateProvider).RemoveAuth();
            await Js.RemoveItemAsync("isauthenticated");
            Nav.NavigateTo("/");
        }
    }
}