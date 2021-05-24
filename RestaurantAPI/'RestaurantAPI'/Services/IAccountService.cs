using _RestaurantAPI_.Entities;
using _RestaurantAPI_.Models;

namespace _RestaurantAPI_.Services
{
    public interface IAccountService
    {
        RestaurantDbContext _dbContext { get; set; }

        void RegisterUser(RegisterUserDto dto);
        string GenerateJwt(LoginUserDto dto);

    }
}