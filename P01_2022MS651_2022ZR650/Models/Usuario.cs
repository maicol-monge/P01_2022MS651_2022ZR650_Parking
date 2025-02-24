using System.ComponentModel.DataAnnotations;

namespace P01_2022MS651_2022ZR650.Models
{
    public class Usuario
    {

        [Key]
        public int id { get; set; }
        public string nombre { get; set; }
        public string correo { get; set; }
        public string? telefono { get; set; }
        public string contrasena { get; set; }
        public string rol { get; set; }

    }
}
