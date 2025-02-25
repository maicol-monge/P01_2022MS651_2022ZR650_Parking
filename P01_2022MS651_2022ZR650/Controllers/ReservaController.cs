using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P01_2022MS651_2022ZR650.Models;

namespace P01_2022MS651_2022ZR650.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservaController : ControllerBase
    {
        private readonly parkingContext _parkingContext;

        public ReservaController(parkingContext parkingContext)
        {
            _parkingContext = parkingContext;
        }


        [HttpGet]
        [Route("ObtenerReservas")]
        public IActionResult Get()
        {
            List<Reserva> listadoReserva = (from a in _parkingContext.reserva select a).ToList();
            if (listadoReserva.Count() == 0)
            {
                return NotFound();
            }
            return Ok(listadoReserva);
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
        [Route("AgregarReserva")]
        public IActionResult guardarReserva([FromBody] Reserva reserva)
        {
            try
            {
                _parkingContext.reserva.Add(reserva);
                _parkingContext.SaveChanges();
                return Ok(reserva);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut]
        [Route("actualizarReserva/{id}")]
        public IActionResult ActualizarReserva(int id, [FromBody] Reserva reservaModificar)
        {
            Reserva? reservaActual = (from a in _parkingContext.reserva
                                                  where a.id == id
                                                  select a).FirstOrDefault();

            if (reservaActual == null)
            {
                return NotFound();
            }
            reservaActual.usuario_id = reservaModificar.usuario_id;
            reservaActual.espacio_id = reservaModificar.espacio_id;
            reservaActual.fecha = reservaModificar.fecha;
            reservaActual.cantidad_horas = reservaModificar.cantidad_horas;


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
