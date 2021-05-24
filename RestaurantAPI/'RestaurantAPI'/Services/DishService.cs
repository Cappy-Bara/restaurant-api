using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _RestaurantAPI_.Entities;
using _RestaurantAPI_.Exceptions;
using _RestaurantAPI_.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace _RestaurantAPI_.Services
{
    public class DishService : IDishService
    {
        private readonly RestaurantDbContext _context;
        private readonly IMapper _mapper;

        public DishService(RestaurantDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public int Create(int restaurantId, CreateDishDto dto)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dish = _mapper.Map<Dish>(dto);
            dish.RestaurantId = restaurantId;
            _context.Dishes.Add(dish);
            _context.SaveChanges();
            return dish.Id;
        }

        public IEnumerable<DishDto> GetAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            return _mapper.Map<IEnumerable<DishDto>>(restaurant.Dishes);
        }

        public DishDto GetById(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dish = _context.Dishes.FirstOrDefault(k => k.RestaurantId == restaurantId && k.Id == dishId);
            if (dish == null)
                throw new NotFoundException("Dish not found");
            var result = _mapper.Map<DishDto>(dish);
            return result;
        }

        public void DeleteAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            _context.Dishes.RemoveRange(restaurant.Dishes);
            _context.SaveChanges();
        }

        public void DeleteSingle(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            _context.Dishes.Remove(restaurant.Dishes.FirstOrDefault(d => d.Id == dishId));
            _context.SaveChanges();
        }

        private Restaurant GetRestaurantById(int restaurantId)
        {
            var restaurant = _context.Restaurants
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.id == restaurantId);

            if (restaurant is null)
                throw new NotFoundException("restaurant not found");

            return restaurant;
        }


    }
}