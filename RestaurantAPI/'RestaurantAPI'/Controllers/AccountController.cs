using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _RestaurantAPI_.Models;
using _RestaurantAPI_.Services;
using Microsoft.AspNetCore.Mvc;

namespace _RestaurantAPI_.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public IAccountService _service { get; set; }
        public AccountController(IAccountService service)
        {
            _service = service;
        }


        [HttpPost("register")]
        public ActionResult RegisterUser([FromBody] RegisterUserDto dto)
        {
            _service.RegisterUser(dto);
            return Ok();
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginUserDto dto)
        {
            string token = _service.GenerateJwt(dto);
            return Ok(token);
        }








    }
}
