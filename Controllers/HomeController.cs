using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP.Server.Controllers
{
    [Route("/")]
    [ApiController]
    public class HomeController:ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Witaj w serwisie");
        }
    }
}
