using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_2022MS651_2022ZR650.Models
{
    public class Espacio_parqueo
    {
        [Key]
        public int id {  get; set; }
        [Required]
        public int sucursal_id { get; set; }
        public int numero {  get; set; }
        public string ubicacion { get; set; }
        public decimal costo_por_hora { get; set; }
        public string estado {  get; set; }

    }
}
