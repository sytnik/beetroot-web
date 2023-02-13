using BlazorServerMud.Logic;

namespace BlazorServerMud.DAO;

public record AuthModel
{
    public string Login { get; set; }
    public string Password { get; set; }
}

public sealed record Manager : AuthModel
{
    public int Id { get; set; }
    public string Role { get; set; }
    public List<Purchase> Purchases { get; set; }
}