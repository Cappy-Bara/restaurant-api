using System.Collections.Generic;
using System.Security.Claims;
using _RestaurantAPI_.Models;
using AutoMapper;

namespace _RestaurantAPI_
{
    public interface IRestaurantService
    {
        int Create(CreateRestaurantDto dto);
        PagedResult<RestaurantDto> getAll(RestaurantQuery query);
        RestaurantDto getById(int id);
        public void Delete(int id);
        public void Modify(int id, ModfiyRestaurantDto request);
    }
}