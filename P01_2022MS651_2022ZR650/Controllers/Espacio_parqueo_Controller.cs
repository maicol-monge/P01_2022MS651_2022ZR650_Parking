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

        //CRUD de SUCURSALES

        [HttpGet]
        [Route("GetSucursales")]
        public IActionResult GetSucursales()
        {
            List<Sucursal> listadoSucursales = (from e in _parkingContext.sucursal
                                              select e).ToList();


            if (listadoSucursales.Count() == 0)
            {
                return NotFound();
            }

            return Ok(listadoSucursales);
        }

        [HttpPost]
        [Route("AddSucursal")]
        public IActionResult AddSucursal([FromBody] Sucursal sucursal)
        {
            try
            {
                //valida que el administrador_id si sea empleado a manera de autenticacion
                var usuario = (from uu in _parkingContext.usuario
                                  where uu.id.Equals(sucursal.administrador_id) && uu.rol.Equals("empleado")
                                   select uu).FirstOrDefault();

                if (usuario != null)
                {
                    _parkingContext.sucursal.Add(sucursal);
                    _parkingContext.SaveChanges();
                    
                }
                else
                {
                    return BadRequest("El administrador no es empleado");
                }

                return Ok(sucursal);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("actualizarSucursal/{id}")]
        public IActionResult actualizarSucursal(int id, [FromBody] Sucursal sucursalModificar)
        {
            Sucursal? sucursalActual = (from s in _parkingContext.sucursal
                                       where s.id == id
                                       select s).FirstOrDefault();


            //valida que el administrador_id si sea empleado a manera de autenticacion
            var usuario = (from uu in _parkingContext.usuario
                           join ss in _parkingContext.sucursal
                                    on uu.id equals ss.administrador_id    
                           where uu.id.Equals(sucursalModificar.administrador_id) && uu.rol.Equals("empleado")
                           select uu).FirstOrDefault();


            if (sucursalActual == null || usuario == null)
            {
                return NotFound();
            }
            else
            {
                sucursalActual.nombre = sucursalModificar.nombre;
                sucursalActual.direccion = sucursalModificar.direccion;
                sucursalActual.telefono = sucursalModificar.telefono;
                sucursalActual.administrador_id = sucursalModificar.administrador_id;
                sucursalActual.num_espacios = sucursalModificar.num_espacios;


                _parkingContext.Entry(sucursalActual).State = EntityState.Modified;
                _parkingContext.SaveChanges();
            }

            


            return Ok(sucursalModificar);
        }

        [HttpDelete]
        [Route("eliminarSucursal/{id}")]
        public IActionResult EliminarUsuario(int id)
        {
            Sucursal? sucursal= (from s in _parkingContext.sucursal
                                         where s.id == id
                                         select s).FirstOrDefault();



            if (sucursal == null)
            {
                return NotFound();
            }



            _parkingContext.sucursal.Attach(sucursal);
            _parkingContext.sucursal.Remove(sucursal);
            _parkingContext.SaveChanges();



            return Ok(sucursal);
        }

        //=====================Fin CRUD de Sucursal==================//\

        //Registrar nuevos espacios de parqueo con número, ubicación, costo por hora
        //y estado (disponible/ocupado) por sucursal.
        //En estado solo permite poner "disponible" o "ocupado" 
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

        //Mostrar una lista de todos los espacios de parqueo disponibles.
        [HttpGet]
        [Route("ParqueosDisponibles/{sucursal_id},{fecha}")]
        public IActionResult ParqueosDisponibles(int sucursal_id, DateTime fecha)
        {
            var listadoEspacios = (from esp in _parkingContext.espacio_parqueo
                                   join ss in _parkingContext.sucursal
                                       on esp.sucursal_id equals ss.id
                                   join rr in _parkingContext.reserva
                                       on esp.id equals rr.espacio_id into reservas
                                   from rr in reservas.DefaultIfEmpty() // Left Join
                                   where esp.sucursal_id == sucursal_id
                                         && (rr == null || rr.fecha.Date != fecha.Date) // Filtra si no hay reserva o si la fecha de reserva es diferente
                                   select new
                                   {
                                       id_espacio = esp.id,
                                       esp.numero,
                                       esp.ubicacion,
                                       esp.costo_por_hora,
                                       esp.estado,
                                       id_sucursal = ss.id,
                                       ss.nombre,
                                       ss.direccion,
                                   }).ToList();


            if (listadoEspacios.Count == 0)
            {
                return NotFound("No hay espacios de parqueo disponibles para esta fecha y sucursal.");
            }

            return Ok(listadoEspacios);
        }

        //Actualizar información de un espacio de parqueo.
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
            espacioActual.estado = espacioModificar.estado;


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
