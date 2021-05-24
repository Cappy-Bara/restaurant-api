using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _RestaurantAPI_.Models;
using _RestaurantAPI_.Services;
using Microsoft.AspNetCore.Mvc;

namespace _RestaurantAPI_.Controllers
{

    [Route("api/restaurant/{restaurantId}/dish")]
    [ApiController]     //automatyczna walidacja modeli
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;

        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        [HttpPost]
        public ActionResult Post([FromRoute]int restaurantId,[FromBody]CreateDishDto dish)
        {
            int newDishId = _dishService.Create(restaurantId, dish);
            return Created($"api/restaurant/{restaurantId}/dish/{newDishId}",null);
        }


        [HttpGet]
        public ActionResult<IEnumerable<DishDto>> GetAll([FromRoute]int restaurantId)
        {
            return Ok(_dishService.GetAll(restaurantId));
        }

        
        [HttpGet("{dishId}")]
        public ActionResult<DishDto> GetById([FromRoute]int restaurantId, [FromRoute]int dishId)
        {
            return Ok(_dishService.GetById(restaurantId,dishId));
        }

        [HttpDelete]
        public ActionResult RemoveAll([FromRoute]int restaurantId)
        {
            _dishService.DeleteAll(restaurantId);
            return NoContent();
        }

        [HttpDelete("{dishId}")]
        public ActionResult RemoveById([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            _dishService.DeleteSingle(restaurantId, dishId);
            return NoContent();
        }
    }
}
