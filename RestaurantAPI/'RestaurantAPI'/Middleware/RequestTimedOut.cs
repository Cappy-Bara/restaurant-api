using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace _RestaurantAPI_.Middleware
{
    public class RequestTimedOut :IMiddleware
    {
        private readonly ILogger<RequestTimedOut> _logger;
        private readonly Stopwatch _stopwatch;

        public RequestTimedOut(ILogger<RequestTimedOut> logger)
        {
            _logger = logger;
            _stopwatch = new Stopwatch();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _stopwatch.Start();
            await next.Invoke(context);
            _stopwatch.Stop();

            if (_stopwatch.ElapsedMilliseconds >= 4000)
                {
                var message = $"Request [{context.Request.Method}] at {context.Request.Path} took {_stopwatch.ElapsedMilliseconds}ms";
                _logger.LogInformation(message);
            }
        }
    }
}
