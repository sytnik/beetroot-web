namespace BlazorApp2.Client.Logic;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _claimsPrincipal = new(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
        Task.FromResult(new AuthenticationState(_claimsPrincipal));

    public void SetAuthInfo(Manager manager)
    {
        var identity = new ClaimsIdentity(new Claim[]
        {
            new(ClaimsIdentity.DefaultNameClaimType, manager.Login),
            new(ClaimsIdentity.DefaultRoleClaimType, manager.Role)
        }, "AuthCookie");
        _claimsPrincipal = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void RemoveAuth()
    {
        _claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}