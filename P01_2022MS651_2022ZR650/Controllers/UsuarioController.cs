using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P01_2022MS651_2022ZR650.Models;

namespace P01_2022MS651_2022ZR650.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly parkingContext _parkingContexto;

        public UsuarioController(parkingContext parkingContexto)
        {
            _parkingContexto = parkingContexto;
        }


        //En rol solo permite poner "cliente" o "empleado" para mas
        //adelante aplicar la autenticacion
        [HttpPost]
        [Route("Add")]
        public IActionResult CrearUsuario([FromBody] Usuario usuario)
        {
            try
            {
                _parkingContexto.usuario.Add(usuario);
                _parkingContexto.SaveChanges();
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("Login/{correo},{contrasena}")]
        public IActionResult login(string correo, string contrasena)
        {
            var autor = (from uu in _parkingContexto.usuario
                         where uu.correo.Equals(correo) && uu.contrasena.Equals(contrasena)
                         select uu).FirstOrDefault(); 

            if (autor == null)
            {
                return NotFound("Credenciales invalidas");


            }
            

            return Ok("Credenciales validas");
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<Usuario> listadoUsuarios = (from e in _parkingContexto.usuario
                                              select e).ToList();
            if (listadoUsuarios.Count() == 0)
            {
                return NotFound();
            }

            return Ok(listadoUsuarios);
        }

        [HttpPut]
        [Route("actualizar/{id}")]
        public IActionResult ActualizarUsuario(int id, [FromBody] Usuario usuarioModificar)
        {
            Usuario? usuarioActual = (from e in _parkingContexto.usuario
                                       where e.id == id
                                       select e).FirstOrDefault();



            if (usuarioActual == null)
            {
                return NotFound();
            }

            usuarioActual.nombre = usuarioModificar.nombre;
            usuarioActual.correo = usuarioModificar.correo;
            usuarioActual.telefono = usuarioModificar.telefono;
            usuarioActual.contrasena = usuarioModificar.contrasena;
            usuarioActual.rol = usuarioModificar.rol;


            _parkingContexto.Entry(usuarioActual).State = EntityState.Modified;
            _parkingContexto.SaveChanges();


            return Ok(usuarioModificar);
        }

        [HttpDelete]
        [Route("eliminar/{id}")]
        public IActionResult EliminarUsuario(int id)
        {
            Usuario? usuario = (from e in _parkingContexto.usuario
                                 where e.id == id
                                 select e).FirstOrDefault();



            if (usuario == null)
            {
                return NotFound();
            }



            _parkingContexto.usuario.Attach(usuario);
            _parkingContexto.usuario.Remove(usuario);
            _parkingContexto.SaveChanges();

            return Ok(usuario);
        }
    }
}
