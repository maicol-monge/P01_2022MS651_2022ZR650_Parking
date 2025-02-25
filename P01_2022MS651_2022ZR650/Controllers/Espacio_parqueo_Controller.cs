using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet]
        [Route("ObtenerEspacios")]
        public IActionResult Get()
        {
            List<Espacio_parqueo> listadoEspacios = (from a in _parkingContext.espacio_parqueo select a).ToList();
            if (listadoEspacios.Count() == 0)
            {
                return NotFound();
            }
            return Ok(listadoEspacios);
        }

        //[HttpGet]
        //[Route("ObtenerCalificacionesByPublicacion/{id}")]
        //public IActionResult ObtenerCalificacionesByPublicacion(int id)
        //{
        //    List<Calificaciones> listadoCalificaciones = (from a in _blogContext.calificaciones
        //                                                  where a.publicacionId == id
        //                                                  select a).ToList();
        //    if (listadoCalificaciones.Count() == 0)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(listadoCalificaciones);
        //}

        [HttpPost]
        [Route("AgregarEspacios")]
        public IActionResult guardarEspacio([FromBody] Espacio_parqueo espacio_)
        {
            try
            {
                _parkingContext.espacio_parqueo.Add(espacio_);
                _parkingContext.SaveChanges();
                return Ok(espacio_);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut]
        [Route("actualizarEspacio/{id}")]
        public IActionResult ActualizarEspacio(int id, [FromBody] Espacio_parqueo espacioModificar)
        {
            Espacio_parqueo? espacioActual = (from a in _parkingContext.espacio_parqueo
                                      where a.id == id
                                      select a).FirstOrDefault();

            if (espacioActual == null)
            {
                return NotFound();
            }
            espacioActual.sucursal_id = reservaModificar.usuario_id;
            espacioActual.espacio_id = reservaModificar.espacio_id;
            espacioActual.fecha = reservaModificar.fecha;
            espacioActual.cantidad_horas = reservaModificar.cantidad_horas;


            _parkingContext.Entry(reservaActual).State = EntityState.Modified;
            _parkingContext.SaveChanges();

            return Ok(reservaModificar);
        }

        [HttpDelete]
        [Route("eliminarReserva/{id}")]
        public IActionResult EliminarReserva(int id)
        {
            Reserva? reserva = (from a in _parkingContext.reserva
                                where a.id == id
                                select a).FirstOrDefault();

            if (reserva == null)
            {
                return NotFound();
            }

            _parkingContext.reserva.Attach(reserva);
            _parkingContext.reserva.Remove(reserva);
            _parkingContext.SaveChanges();

            return Ok(reserva);
        }
    }
}
