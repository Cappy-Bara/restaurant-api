using System.Collections.Generic;
using RestaurantAPI;

namespace _RestaurantAPI_.Controllers
{
    public interface IWeatherForecastService
    {
        IEnumerable<WeatherForecast> Get(int results, int tempMin, int tempMax);
    }
}