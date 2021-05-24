using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _RestaurantAPI_.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]

    //PRZENOSIMY STĄD OBSŁUGĘ DO SERVICE, ŻEBY ODDZIELIĆ JE OD SIEBIE


    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherForecastService _service;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecastService service)
        {
            _logger = logger;
            _service = service;
        }
        /*
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var result = _service.Get();
            return result;
        }
        
        //Dodanie kolejnej ścieżki - sposób 1
        [HttpGet]
        [Route("CurrentDay")]
        public IEnumerable<WeatherForecast> Get2()
        {
            var result = _service.Get();
            return result;
        }

        //Dodanie kolejnej ścieżki - sposób 2
        [HttpGet("NextDay/{max}")]      //przyjecie parametru max from route - ze ścieżki.
        public IEnumerable<WeatherForecast> Get3([FromQuery]int take, [FromRoute]int max)       //From query - przekazane z pytajnikiem i nazwą w adresie
        {
            var result = _service.Get(); 
            return result;
        }


        [HttpPost]
        /*
        public string Hello([FromBody] string name)         //FromBody - przesyłane w Body, np. w formacie JSON
        {
            return $"Hello {name}!";
            //return String.Format("Hello {0}", name);
        }
        //NIE MUSIMY TU DODAWAĆ SPECJALNEJ ŚCIEŻKI, Może zostać taka sama jak przy gecie, bo odpowiada na inne czasowniki HTTP.
        */
        public ActionResult<string> Hello([FromBody] string name)   //Chodzi o to, by funkcja otrzymała z jakim kodem została wykonana operacja
        {
            //druga opcja:
            return StatusCode(401, $"Hello {name}!");

            /*
            HttpContext.Response.StatusCode = 401;      //Przypisanie kodu do tego response'a. 
                                                        //HttpContext to własność każdego kontrolera, przez który mamy dostęp do zapytań i odp. HTTp
            return $"Hello {name}!";    */  //pierwsza opcja
            //Można też znaleźć konkretną metodę wysyłającą konkretny błąd.
        }

        [HttpPost("generate")]
        public ActionResult<IEnumerable<WeatherForecast>> Generate([FromQuery] int amountOfResults, [FromBody]TemperatureRequest request)
        {
            int statusCode = (amountOfResults > 0 && request.Max > request.Min) ? 200 : 400; 
            var result = _service.Get(amountOfResults, request.Min, request.Max);
            return StatusCode(statusCode, result);
        }

    }
}
