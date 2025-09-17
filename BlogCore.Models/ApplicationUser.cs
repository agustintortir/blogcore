using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCore.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Debe ingresar un nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Debe ingresar una direccion")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Debe ingresar una ciudad")]
        public string Ciudad { get; set; }

        [Required(ErrorMessage = "Debe ingresar un pais")]
        public string Pais { get; set; }

        // Teléfono (IdentityUser ya lo trae)
    }
}
