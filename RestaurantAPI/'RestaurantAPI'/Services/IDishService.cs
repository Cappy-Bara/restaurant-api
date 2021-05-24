using System.Collections.Generic;
using _RestaurantAPI_.Models;

namespace _RestaurantAPI_.Services
{
    public interface IDishService
    {
        int Create(int restaurantId, CreateDishDto dto);
        public IEnumerable<DishDto> GetAll(int restaurantId);
        public DishDto GetById(int restaurantId, int dishId);
        public void DeleteAll(int restaurantId);
        public void DeleteSingle(int restaurantId, int dishId);
    }
}