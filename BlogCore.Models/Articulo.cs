using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCore.Models
{
    public class Articulo
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe ingresar un nombre")]
        [Display(Name = "Nombre del Artículo")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Debe ingresar una descripcion")]
        [Display(Name = "Descripcion del Artículo")]
        public string Descripcion { get; set; }

        [Display(Name = "Fecha de Creacion")]
        [ValidateNever]
        public string FechaCreacion { get; set; }

        // Se setea después de guardar el archivo → no validar en el POST
        [DataType(DataType.ImageUrl)]
        [ValidateNever]
        [Display(Name = "Imagen")]
        public string UrlImagen { get; set; }

        // Validación de selección de categoría:
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría válida")]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        [ValidateNever] // entidad navegación no viene en el POST
        public Categoria Categoria { get; set; }
    }
}
