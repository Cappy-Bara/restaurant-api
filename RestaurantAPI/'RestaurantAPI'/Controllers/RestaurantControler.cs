using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using _RestaurantAPI_.Entities;
using _RestaurantAPI_.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace _RestaurantAPI_.Controllers
{
    [Route("api/restaurant")]
    [ApiController]     //Powoduje walidację Modelu, przesyłanego w ciele automatycznie, bez zbędnego kodu. Wywołuje walidatory
    [Authorize]     //Jeśli zapytanie nie będzie zawierać nagłówka autoryzacji, to nie zadziała (tokenu)
    public class RestaurantControler : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantControler(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult<Restaurant> CreateRestaurant([FromBody]CreateRestaurantDto dto)
        {
            int id = _restaurantService.Create(dto);
            return Created($"/api/restaurant/{id}",null);
        }

        [HttpGet]
        [Authorize(Policy = "MinimumNumberOfRestaurants")]
        public ActionResult<IEnumerable<RestaurantDto>> GetAll([FromQuery]RestaurantQuery query)
        {
            return Ok(_restaurantService.getAll(query));
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute]int Id)
        {
            _restaurantService.Delete(Id);
            return NoContent();
        }


        [HttpGet("{Id}")]
        [AllowAnonymous] //Umożliwia na wykonanie zapytania bez autoryzacji.
        public ActionResult<RestaurantDto> GetId([FromRoute] int Id)
        {
            var restaurant = _restaurantService.getById(Id);
            return Ok(restaurant);
        }

        [HttpPut("{Id}")]
        public ActionResult Modify([FromRoute]int id, [FromBody]ModfiyRestaurantDto request)
        {
            _restaurantService.Modify(id, request);
            return BadRequest();
        }

    }
}
