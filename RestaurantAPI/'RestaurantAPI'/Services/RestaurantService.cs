using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _RestaurantAPI_.Entities;
using _RestaurantAPI_.Models;
using _RestaurantAPI_.Exceptions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using _RestaurantAPI_.Authorization;
using _RestaurantAPI_.Services;
using System.Linq.Expressions;

namespace _RestaurantAPI_
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IUserContextService _userContextService;
        public readonly RestaurantDbContext _dbContext;
        public readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;
        private readonly IAuthorizationService _authorizationService;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger,
            IAuthorizationService authorizationService, IUserContextService userContextService )
        {
            _userContextService = userContextService;
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
        }

        public RestaurantDto getById(int id)
        {
            var restaurant = _dbContext
            .Restaurants
            .Include(r => r.Address)
            .Include(r => r.Dishes)
            .FirstOrDefault(r => r.id == id);
            if (restaurant is null) throw new NotFoundException("Restaurant not found");

            var result = _mapper.Map<RestaurantDto>(restaurant);
            return result;
        }

        public PagedResult<RestaurantDto> getAll(RestaurantQuery query)
        {
            var baseQuery = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .Where(r => query.searchPhrase == null || (r.Description.ToLower().Contains(query.searchPhrase.ToLower())
                            || r.Name.ToLower().Contains(query.searchPhrase.ToLower())));

            if(!string.IsNullOrEmpty(query.SortBy))
                {
                var columnsSelector = new Dictionary<string, Expression<Func<Restaurant, object>>>
                {
                    {nameof(Restaurant.Name), r => r.Name},
                    {nameof(Restaurant.Description), r => r.Description},
                    {nameof(Restaurant.Category), r => r.Category},
                };

                var selectedColumn = columnsSelector[query.SortBy];

                baseQuery = query.SortDirection == SortDirection.ASC ?
                    baseQuery.OrderBy(selectedColumn) 
                    : baseQuery.OrderByDescending(selectedColumn);
                }

            var restaurants = baseQuery
                .Skip(query.pageSize * (query.pageNumber - 1))
                .Take(query.pageSize)
                .ToList();
            var restaurantResult = _mapper.Map<List<RestaurantDto>>(restaurants);
            var result = new PagedResult<RestaurantDto>(restaurantResult, baseQuery.Count(),
                query.pageSize, query.pageNumber);
            return result;
        }

        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = _userContextService.GetUserId;
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();
            return restaurant.id;
        }


        public void Delete(int id)
        {
            _logger.LogError($"Restaurant with id: {id} DELETE action invoked");

            var restaurant = _dbContext
                 .Restaurants
                 .FirstOrDefault(r => r.id == id);

            var authorizationResult = _authorizationService
            .AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException("Wrong operation");
            }


            if (restaurant is null) throw new NotFoundException("Restaurant not found");

            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();
        }

        public void Modify(int id, ModfiyRestaurantDto request)
        {

            var restaurant = _dbContext
            .Restaurants
            .FirstOrDefault(r => r.id == id);
            if (restaurant is null) throw new NotFoundException("Restaurant not found");

            var authorizationResult = _authorizationService
                .AuthorizeAsync(_userContextService.User, restaurant,new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException("Wrong operation");
            }

            restaurant.Name = request.Name;
            restaurant.Description = request.Description;
            restaurant.HasDelivery = request.HasDelivery;
            _dbContext.Restaurants.Update(restaurant);
            _dbContext.SaveChanges();
            }
    }
}
