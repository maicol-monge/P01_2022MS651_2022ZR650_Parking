using System.ComponentModel.DataAnnotations;

namespace P01_2022MS651_2022ZR650.Models
{
    public class Sucursal
    {
        [Key]
        public int id { get; set; }
        public string nombre { get; set; }
        public string direccion { get; set; }
        public string? telefono { get; set; }

        [Required]
        public int administrador_id { get; set; }
        public int num_espacios { get; set; }
    }
}
