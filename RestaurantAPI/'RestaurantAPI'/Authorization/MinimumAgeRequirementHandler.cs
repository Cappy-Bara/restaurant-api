using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Web;

namespace _RestaurantAPI_.Authorization
{
    public class MinimumAgeRequirementHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        private readonly ILogger<MinimumAgeRequirementHandler> _logger;

        public MinimumAgeRequirementHandler(ILogger<MinimumAgeRequirementHandler> logger)
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            var dateOfBirth = DateTime.Parse(context.User.FindFirst(n => n.Type == "dateOfBirth").Value);
            var userEmail = context.User.FindFirst(c => c.Type == ClaimTypes.Name).Value;
            _logger.LogInformation($"user {userEmail} with date of birth: {dateOfBirth} today, which is {DateTime.Now}");
            if(dateOfBirth.AddYears(requirement.MinimumAge) <= DateTime.Now)
            {
                context.Succeed(requirement);
                _logger.LogInformation($"Authorisation succedded");
            }
            else
            {
                _logger.LogInformation($"Authorisation failed");
            }

            return Task.CompletedTask;
        }
    }
}
