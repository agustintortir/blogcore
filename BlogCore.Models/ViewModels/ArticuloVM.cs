using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCore.Models.ViewModels
{
    public class ArticuloVM
    {
        public Articulo Articulo { get; set; }

        [ValidateNever]                     // no viene en el POST, solo para el DropDown
        public IEnumerable<SelectListItem> CategoriaLista { get; set; }
    }
}
