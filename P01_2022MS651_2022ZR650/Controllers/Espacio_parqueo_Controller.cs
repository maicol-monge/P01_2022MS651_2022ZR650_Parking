using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using P01_2022MS651_2022ZR650.Models;

namespace P01_2022MS651_2022ZR650.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Espacio_parqueo_Controller : ControllerBase
    {
        private readonly parkingContext _parkingContext;

        public Espacio_parqueo_Controller(parkingContext parkingContext)
        {
            _parkingContext = parkingContext;
        }
    }
}
