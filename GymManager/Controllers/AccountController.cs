using GymManager.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManager.Controllers
{
    // Hereda de controller para poder manejar las páginas y acciones del sistema
    public class AccountController : Controller
    {
        private readonly GymDbContext _context;

        public AccountController(GymDbContext context)
        {
            _context = context;
        }

        // Muestra la pantalla de inicio de sesión
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Comprueba el correo y contraseña del administrador
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string correo, string password)
        {
            var administrador = await _context.Administradores
                .FirstOrDefaultAsync(a =>
                    a.Correo == correo &&
                    a.Activo);

            if (administrador == null)
            {
                ModelState.AddModelError("", "Correo o contraseña incorrectos.");
                return View();
            }

            if (administrador.PasswordHash != password)
            {
                ModelState.AddModelError("", "Correo o contraseña incorrectos.");
                return View();
            }

            // Guarda al administrador para mantener la sesión iniciada
            HttpContext.Session.SetString(
                "Administrador",
                administrador.Nombre);

            return RedirectToAction("Index", "Home");
        }

        // Cierra la sesión y regresa al login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}
