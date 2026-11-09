
using GymManager.Data;
using GymManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManager.Filters;

namespace GymManager.Controllers
{
    [AdminFilter]

    public class SociosController : Controller
    {
        private readonly GymDbContext _context;

        public SociosController(GymDbContext context)
        {
            _context = context;
        }

        // CONSULTAR SOCIOS
        [HttpGet]
        public async Task<IActionResult> Index(string? buscar)
        {
            var consulta = _context.Socios.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                consulta = consulta.Where(s =>
                    s.Nombre.Contains(buscar) ||
                    s.Apellidos.Contains(buscar));
            }

            var socios = await consulta
                .OrderBy(s => s.Apellidos)
                .ThenBy(s => s.Nombre)
                .ToListAsync();

            ViewBag.Buscar = buscar;

            return View(socios);
        }

        // MOSTRAR FORMULARIO DE REGISTRO
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // GUARDAR NUEVO SOCIO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Socio socio)
        {
            ModelState.Remove(nameof(Socio.FechaRegistro));

            if (ModelState.IsValid)
            {
                socio.IdSocio = 0;
                socio.FechaRegistro = DateTime.Today;
                socio.Estado = "Activo";

                _context.Socios.Add(socio);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Socio registrado correctamente.";

                return RedirectToAction(nameof(Index));
            }

            return View(socio);
        }

        // MOSTRAR FORMULARIO DE EDICIÓN
        [HttpGet]
        public async Task<IActionResult> Edit(uint? id)
        {
            if (id == null)
                return NotFound();

            var socio = await _context.Socios.FindAsync(id);

            if (socio == null)
                return NotFound();

            return View(socio);
        }

        // GUARDAR CAMBIOS
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            uint id,
            string nombre,
            string apellidos,
            string? telefono,
            string? correo,
            DateTime? fechaNacimiento,
            string? notas)
        {
            var socio = await _context.Socios.FindAsync(id);

            if (socio == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellidos))
            {
                ModelState.AddModelError("", "Nombre y apellidos son obligatorios.");
                return View(socio);
            }

            socio.Nombre = nombre.Trim();
            socio.Apellidos = apellidos.Trim();
            socio.Telefono = telefono;
            socio.Correo = correo;
            socio.FechaNacimiento = fechaNacimiento;
            socio.Notas = notas;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Socio actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // CAMBIAR ESTADO DEL SOCIO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(uint id)
        {
            var socio = await _context.Socios.FindAsync(id);

            if (socio == null)
                return NotFound();

            socio.Estado = socio.Estado == "Activo"
                ? "Inactivo"
                : "Activo";

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Estado del socio actualizado.";

            return RedirectToAction(nameof(Index));
        }

        // CONSULTAR DETALLES
        [HttpGet]
        public async Task<IActionResult> Details(uint? id)
        {
            if (id == null)
                return NotFound();

            var socio = await _context.Socios
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.IdSocio == id);

            if (socio == null)
                return NotFound();

            return View(socio);
        }
    }
}
