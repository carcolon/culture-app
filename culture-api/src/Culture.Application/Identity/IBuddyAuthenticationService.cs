namespace Culture.Application.Identity;

public interface IBuddyAuthenticationService
{
    Task<BuddyLoginResult> LoginAsync(string email, string password, CancellationToken cancellationToken);
}
