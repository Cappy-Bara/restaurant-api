using System.Security.Claims;

namespace _RestaurantAPI_.Services
{
    public interface IUserContextService
    {
        int? GetUserId { get; }
        ClaimsPrincipal User { get; }
    }
}