using BlazorServerMud.DAO;

namespace BlazorServerMud.Pages;

public sealed partial class Index
{
    private List<User> _users;
    private Department _department;
    private readonly User _currentUser = new();
    private string _state = "Please input user data";

    protected override async Task OnInitializedAsync()
    {
        _department = await Repository.GetDepartmentWithCounter();
        _users = await GetUsers();
    }

    private async Task<List<User>> GetUsers() => await Repository.GetUsersAsync();

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
        var result = await Repository.CreateUserAsync(_currentUser);
        _state = result ? "User added" : "User add error";
        if (!result) return;
        _users = await GetUsers();
    }

    private async Task EditUser()
    {
        await Repository.UpdateUserAsync(_currentUser);
        _users = await GetUsers();
    }

    private void AddInfo() => _currentUser.Info += "button clicked!";
}