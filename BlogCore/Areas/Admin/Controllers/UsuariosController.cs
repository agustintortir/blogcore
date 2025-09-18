using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Models;
using BlogCore.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogCore.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    [Authorize(Roles = CNT.Administrador)]
    public class UsuariosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly UserManager<ApplicationUser> _userManager;


        public UsuariosController(IContenedorTrabajo contenedorTrabajo, UserManager<ApplicationUser> userManager)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _userManager = userManager;
        }


        [HttpGet]
        public IActionResult Index()
        {
            //var usuarios = _contenedorTrabajo.Usuario.GetAll();
            //return View(usuarios);

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return View(_contenedorTrabajo.Usuario.GetAll(u => u.Id != usuarioActual.Value));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var currentUserId = _userManager.GetUserId(User);
            var users = _userManager.Users
                .Where(u => currentUserId == null || u.Id != currentUserId)
                .Select(u => new {
                    id = u.Id,
                    nombre = u.Nombre,
                    email = u.Email,
                    estaBloqueado = u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.Now
                })
                .ToList();

            return Json(new { data = users });
        }

        [HttpGet]
        public IActionResult Bloquear(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            _contenedorTrabajo.Usuario.BloquearUsuario(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Desbloquear(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            _contenedorTrabajo.Usuario.DesbloquearUsuario(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Json(new { success = false, message = "Id inválido." });

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return Json(new { success = false, message = "Usuario no encontrado." });

            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId)
                return Json(new { success = false, message = "No podés borrarte a vos mismo." });

            // (Opcional) proteger admins
            // if (await _userManager.IsInRoleAsync(user, CNT.Administrador))
            //     return Json(new { success = false, message = "No se puede borrar un Administrador." });

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return Json(new { success = false, message = string.Join(" | ", result.Errors.Select(e => e.Description)) });

            return Json(new { success = true, message = "Usuario borrado correctamente." });
        }
    }
}
