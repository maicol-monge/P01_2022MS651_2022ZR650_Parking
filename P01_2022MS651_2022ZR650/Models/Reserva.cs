using System.ComponentModel.DataAnnotations;

namespace P01_2022MS651_2022ZR650.Models
{
    public class Reserva
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int usuario_id { get; set; }
        [Required]
        public int espacio_id {  get; set; }
        public DateTime fecha { get; set; }
        public int cantidad_horas { get; set; }
    }
}
