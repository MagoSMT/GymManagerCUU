using GymManager.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly GymDbContext _context;

        public AccountController(GymDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

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

            HttpContext.Session.SetString(
                "Administrador",
                administrador.Nombre);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}