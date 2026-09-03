namespace Culture.Application.Identity;

public sealed record AuthenticatedAdmin(Guid Id, string Email, string DisplayName, string Role);
