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
           var listadoEspacios = (from a in _parkingContext.espacio_parqueo
                                  join ss in _parkingContext.sucursal
                                  on a.sucursal_id equals ss.id
                                  select new
                                  {
                                      a.id,
                                      ss.nombre,
                                      a.numero,
                                      a.ubicacion,
                                      a.costo_por_hora,
                                      a.estado,
                                      

                                  }).ToList();
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
            espacioActual.sucursal_id = espacioModificar.sucursal_id;
            espacioActual.numero = espacioModificar.numero;
            espacioActual.ubicacion = espacioModificar.ubicacion;
            espacioActual.costo_por_hora = espacioModificar.costo_por_hora;


            _parkingContext.Entry(espacioActual).State = EntityState.Modified;
            _parkingContext.SaveChanges();

            return Ok(espacioModificar);
        }

        [HttpDelete]
        [Route("eliminarEspacio/{id}")]
        public IActionResult EliminarEspacio(int id)
        {
            Espacio_parqueo? espacio = (from a in _parkingContext.espacio_parqueo
                                where a.id == id
                                select a).FirstOrDefault();

            if (espacio == null)
            {
                return NotFound();
            }

            _parkingContext.espacio_parqueo.Attach(espacio);
            _parkingContext.espacio_parqueo.Remove(espacio);
            _parkingContext.SaveChanges();

            return Ok(espacio);
        }
    }
}
