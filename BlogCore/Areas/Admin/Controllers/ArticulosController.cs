using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Models;
using BlogCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BlogCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticulosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ArticulosController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnvironment)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ArticuloVM articuloVM = new ArticuloVM()
            {
                Articulo = new BlogCore.Models.Articulo(),
                CategoriaLista = _contenedorTrabajo.Categoria.GetListaCategorias()
            };
            return View(articuloVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ArticuloVM articuloVM)
        {

            if (ModelState.IsValid)
            {
                string rutaPrincipal = _hostingEnvironment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;
                if (articuloVM.Articulo.Id == 0 && archivos.Count() > 0)
                {
                    //Se crea nuevo articulo
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, @"imagenes\articulos");
                    var extension = Path.GetExtension(archivos[0].FileName);

                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams);
                    }

                    articuloVM.Articulo.UrlImagen = @"\imagenes\articulos\" + nombreArchivo + extension;
                    articuloVM.Articulo.FechaCreacion = DateTime.Now.ToString();

                    _contenedorTrabajo.Articulo.Add(articuloVM.Articulo);
                    _contenedorTrabajo.Save();

                    return RedirectToAction(nameof(Index));
                } else { ModelState.AddModelError("Imagen", "Debes seleccionar una imagen"); }

            }

            articuloVM.CategoriaLista = _contenedorTrabajo.Categoria.GetListaCategorias();
            return View(articuloVM);
        }

        // GET: /Admin/Articulos/Edit/5
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var articulo = _contenedorTrabajo.Articulo.Get(id.Value);
            if (articulo == null) return NotFound();

            var vm = new ArticuloVM
            {
                Articulo = articulo,
                CategoriaLista = _contenedorTrabajo.Categoria.GetListaCategorias()
            };

            return View(vm);
        }

        // POST: /Admin/Articulos/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ArticuloVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.CategoriaLista = _contenedorTrabajo.Categoria.GetListaCategorias();
                return View(vm);
            }

            var articuloDesdeDb = _contenedorTrabajo.Articulo.Get(vm.Articulo.Id);
            if (articuloDesdeDb == null) return NotFound();

            string rutaPrincipal = _hostingEnvironment.WebRootPath;
            var archivos = HttpContext.Request.Form.Files;

            // Si subieron una imagen nueva, reemplazar
            if (archivos.Count > 0 && archivos[0].Length > 0)
            {
                string subidas = Path.Combine(rutaPrincipal, @"imagenes\articulos");
                Directory.CreateDirectory(subidas);

                // Borrar anterior si existía
                if (!string.IsNullOrWhiteSpace(articuloDesdeDb.UrlImagen))
                {
                    var rutaImagenVieja = Path.Combine(
                        rutaPrincipal,
                        articuloDesdeDb.UrlImagen.TrimStart('\\', '/')
                    );
                    if (System.IO.File.Exists(rutaImagenVieja))
                        System.IO.File.Delete(rutaImagenVieja);
                }

                string nombreArchivo = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(archivos[0].FileName);

                using (var fs = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                {
                    archivos[0].CopyTo(fs);
                }

                vm.Articulo.UrlImagen = @"\imagenes\articulos\" + nombreArchivo + extension;
            }
            else
            {
                // Conservar imagen actual
                vm.Articulo.UrlImagen = articuloDesdeDb.UrlImagen;
            }

            // Conservar fecha de creación (no se toca en Edit)
            vm.Articulo.FechaCreacion = articuloDesdeDb.FechaCreacion;

            _contenedorTrabajo.Articulo.Update(vm.Articulo);
            _contenedorTrabajo.Save();

            return RedirectToAction(nameof(Index));
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var articuloDesdeDb = _contenedorTrabajo.Articulo.Get(id);
            string rutaDirectorioPrincipal = _hostingEnvironment.WebRootPath;
            var rutaImagen = Path.Combine(rutaDirectorioPrincipal, articuloDesdeDb.UrlImagen.TrimStart('\\', '/'));

            if (articuloDesdeDb == null)
            {
                return Json(new { success = false, message = "Error borrando articulo" });
            }
            _contenedorTrabajo.Articulo.Remove(articuloDesdeDb);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Articulo borrado exitosamente" });
        }

        #region Llamadas a la API
        [HttpGet]
            public IActionResult GetAll()
            {
                var todos = _contenedorTrabajo.Articulo.GetAll(includeProperties: "Categoria");
                return Json(new { data = todos });
            }

            #endregion
    }
}
