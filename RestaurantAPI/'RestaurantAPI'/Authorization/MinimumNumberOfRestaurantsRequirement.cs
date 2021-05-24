using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace _RestaurantAPI_.Authorization
{
    public class MinimumNumberOfRestaurantsRequirement : IAuthorizationRequirement
    {
        public int MinimumRestaurants { get; }

        public MinimumNumberOfRestaurantsRequirement(int minimumRestaurants)
        {
            MinimumRestaurants = minimumRestaurants;
        }

    }
}
