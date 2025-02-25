using Microsoft.AspNetCore.Authorization;
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
            var listadoReserva = (from a in _parkingContext.reserva
                                  join uu in _parkingContext.usuario
                                  on a.usuario_id equals uu.id
                                  join ee in _parkingContext.espacio_parqueo
                                  on a.espacio_id equals ee.id
                                  join ss in _parkingContext.sucursal
                                  on ee.sucursal_id equals ss.id
                                  select new
                                  {
                                      Id_Reserva = a.id,
                                      Fecha = a.fecha.Date,
                                      a.cantidad_horas,
                                      Nombre_Cliente = uu.nombre,
                                      Sucursal = ss.nombre,
                                      Direccion_sucursal = ss.direccion,
                                      Numero_Espacio_Parqueo = ee.numero,
                                      Ubicacion_Parqueo = ee.ubicacion,
                                      Costo_Por_Hora = ee.costo_por_hora

                                  }).ToList();
            if (listadoReserva.Count() == 0)
            {
                return NotFound();
            }
            return Ok(listadoReserva);
        }
        // Obtener reservas activas de un usuario
        [HttpGet("reservasActivas/{usuarioId}")]
        public IActionResult ObtenerReservasActivas(int usuarioId)
        {
            var reservas = _parkingContext.reserva
                .Where(r => r.usuario_id == usuarioId && r.fecha >= DateTime.Now)
                .ToList();

            return Ok(reservas);
        }


        [HttpGet("reservados-por-dia/{fecha}")]
        public IActionResult EspaciosReservadosPorDia(DateTime fecha)
        {
            var reservas = from r in _parkingContext.reserva
                           join e in _parkingContext.espacio_parqueo on r.espacio_id equals e.id
                           join s in _parkingContext.sucursal on e.sucursal_id equals s.id
                           where r.fecha.Date == fecha.Date
                          
                           select new
                           {
                               r.id, 
                               EspacioId = e.id, 
                               EspacioNumero = e.numero, 
                               FechaReserva = r.fecha, 
                               SucursalNombre = s.nombre
                           };

            var resultados = reservas.ToList();

            // Si no se encuentran resultados, retornar un mensaje adecuado
            if (!resultados.Any())
            {
                return NotFound("No se encontraron espacios reservados para el día especificado.");
            }

            return Ok(resultados);
        }


        [HttpGet("reservados-entre-fechas/{sucursalId}/{fechaInicio}/{fechaFin}")]
        public IActionResult EspaciosReservadosEntreFechas(int sucursalId, DateTime fechaInicio, DateTime fechaFin)
        {
            var reservas = from r in _parkingContext.reserva
                           join e in _parkingContext.espacio_parqueo on r.espacio_id equals e.id
                           join s in _parkingContext.sucursal on e.sucursal_id equals s.id
                           where e.sucursal_id == sucursalId
                           
                           && r.fecha >= fechaInicio // Y las reservas dentro del intervalo de fechas
                           && r.fecha <= fechaFin
                           select new
                           {
                               r.id, // ID de la reserva
                               EspacioId = e.id, // ID del espacio de parqueo
                               EspacioNumero = e.numero, // Número del espacio de parqueo
                               FechaReserva = r.fecha, // Fecha de la reserva
                               Sucursal = s.nombre, // Nombre de la sucursal
                               
                           };

            var resultados = reservas.ToList();

            // Si no se encuentran resultados, retornar un mensaje apropiado
            if (!resultados.Any())
            {
                return NotFound("No se encontraron espacios ocupados en el intervalo de fechas.");
            }

            return Ok(resultados);
        }




        [HttpPost]
        [Route("AgregarReserva")]
        public IActionResult ReservarEspacio([FromBody] Reserva reserva)
        {
            var usuario = _parkingContext.usuario.FirstOrDefault(u => u.id == reserva.usuario_id && u.rol == "cliente");
            if (usuario == null)
            {
                return Unauthorized("Solo los clientes pueden realizar reservas.");
            }

            var espacio = _parkingContext.espacio_parqueo.FirstOrDefault(e => e.id == reserva.espacio_id && e.estado == "disponible");
            if (espacio == null)
            {
                return BadRequest("El espacio no está disponible.");
            }

            _parkingContext.reserva.Add(reserva);
            espacio.estado = "ocupado";
            _parkingContext.SaveChanges();

            return Ok("Reserva realizada con éxito.");
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
        [HttpDelete("cancelar/{reservaId}")]
        public IActionResult CancelarReserva(int reservaId)
        {
            var reserva = _parkingContext.reserva
                .FirstOrDefault(r => r.id == reservaId);

            if (reserva == null || reserva.fecha < DateTime.Now)
            {
                return BadRequest("No se puede cancelar la reserva.");
            }

            var espacio = _parkingContext.espacio_parqueo
                .FirstOrDefault(e => e.id == reserva.espacio_id);

            if (espacio != null)
            {
                espacio.estado = "disponible";
                _parkingContext.SaveChanges();
            }

            _parkingContext.reserva.Remove(reserva);
            _parkingContext.SaveChanges();

            return Ok("Reserva cancelada.");
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
