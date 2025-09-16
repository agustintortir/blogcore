using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCore.Models
{
    public class Slider
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe ingresar un nombre")]
        [Display(Name = "Nombre del Slider")]
        public string Nombre { get; set; }

        [Required]
        public bool Estado { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Imagen")]
        [ValidateNever]                // <-- no validar en el POST
        public string UrlImagen { get; set; }
    }
}
