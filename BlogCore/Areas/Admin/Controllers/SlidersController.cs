using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Models;
using BlogCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BlogCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlidersController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public SlidersController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnvironment)
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Slider slider)
        {

            if (ModelState.IsValid)
            {
                string rutaPrincipal = _hostingEnvironment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                if( archivos.Count() > 0)
                {
                    //Se crea nuevo slider
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, @"imagenes\sliders");
                    var extension = Path.GetExtension(archivos[0].FileName);

                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams);
                    }

                    slider.UrlImagen = @"\imagenes\sliders\" + nombreArchivo + extension;

                    _contenedorTrabajo.Slider.Add(slider);
                    _contenedorTrabajo.Save();

                    return RedirectToAction(nameof(Index));
                }
                else { ModelState.AddModelError("Imagen", "Debes seleccionar una imagen"); }

            }
            return View(slider);
        }

        // GET: /Admin/Articulos/Edit/5
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id != null)
            {

                var slider = _contenedorTrabajo.Slider.Get(id.Value);
                return View(slider);
            }
            return View();

        }
        // POST: /Admin/Articulos/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Slider slider)
        {
            if (!ModelState.IsValid)
            {
                return View(slider);
            }

            var sliderDesdeDb = _contenedorTrabajo.Slider.Get(slider.Id);
            if (sliderDesdeDb == null) return NotFound();

            string rutaPrincipal = _hostingEnvironment.WebRootPath;
            var archivos = HttpContext.Request.Form.Files;

            // Si subieron una imagen nueva, reemplazar
            if (archivos.Count > 0 && archivos[0].Length > 0)
            {
                string subidas = Path.Combine(rutaPrincipal, @"imagenes\sliders");
                Directory.CreateDirectory(subidas);

                // Borrar anterior si existía
                if (!string.IsNullOrWhiteSpace(sliderDesdeDb.UrlImagen))
                {
                    var rutaImagenVieja = Path.Combine(
                        rutaPrincipal,
                        sliderDesdeDb.UrlImagen.TrimStart('\\', '/')
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

                slider.UrlImagen = @"\imagenes\sliders\" + nombreArchivo + extension;
            }
            else
            {
                // Conservar imagen actual
                slider.UrlImagen = sliderDesdeDb.UrlImagen;
            }

            _contenedorTrabajo.Slider.Update(slider);
            _contenedorTrabajo.Save();

            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var sliderDesdeDb = _contenedorTrabajo.Slider.Get(id);
            string rutaDirectorioPrincipal = _hostingEnvironment.WebRootPath;
            var rutaImagen = Path.Combine(rutaDirectorioPrincipal, sliderDesdeDb.UrlImagen.TrimStart('\\', '/'));

            if (sliderDesdeDb == null)
            {
                return Json(new { success = false, message = "Error borrando slider" });
            }
            _contenedorTrabajo.Slider.Remove(sliderDesdeDb);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Slider borrado exitosamente" });
        }

        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            var todos = _contenedorTrabajo.Slider.GetAll();
            return Json(new { data = todos });
        }

        #endregion
    }
}
