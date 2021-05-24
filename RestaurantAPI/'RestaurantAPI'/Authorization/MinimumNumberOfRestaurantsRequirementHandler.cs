using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Web;
using _RestaurantAPI_.Entities;

namespace _RestaurantAPI_.Authorization
{
    public class MinimumNumberOfRestaurantsRequirementHandler : AuthorizationHandler<MinimumNumberOfRestaurantsRequirement>
    {
        private readonly ILogger<MinimumNumberOfRestaurantsRequirementHandler> _logger;
        private readonly RestaurantDbContext _dbContext;

        public MinimumNumberOfRestaurantsRequirementHandler(ILogger<MinimumNumberOfRestaurantsRequirementHandler> logger, RestaurantDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }


        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumNumberOfRestaurantsRequirement requirement)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
                var numberOfRestaurants = _dbContext.Restaurants.Count(r => r.CreatedById == userId);
                _logger.LogInformation($"User with id {userId} with {numberOfRestaurants} restaurants.");
                if (numberOfRestaurants >= requirement.MinimumRestaurants)
                {
                    context.Succeed(requirement);
                    _logger.LogInformation($"Authorisation succedded");
                }

                else
                {
                    _logger.LogInformation($"Authorisation Failed");
                }
            }
            else
            {
                _logger.LogInformation($"Authorisation Failed");

            }
            return Task.CompletedTask;
        }
    }
}
