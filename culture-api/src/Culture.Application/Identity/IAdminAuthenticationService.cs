namespace Culture.Application.Identity;

public interface IAdminAuthenticationService
{
    Task<AdminLoginResult> LoginLocalAsync(string email, string password, CancellationToken cancellationToken);
}
