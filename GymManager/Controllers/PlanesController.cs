
using GymManager.Data;
using GymManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManager.Filters;

namespace GymManager.Controllers
{
    [AdminFilter]
    public class PlanesController : Controller
    {
        private readonly GymDbContext _context;

        public PlanesController(GymDbContext context)
        {
            _context = context;
        }

        // CONSULTAR PLANES
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var planes = await _context.Planes
                .AsNoTracking()
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            return View(planes);
        }

        // MOSTRAR FORMULARIO
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // REGISTRAR PLAN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Plan plan)
        {
            ModelState.Remove(nameof(Plan.FechaCreacion));
            ModelState.Remove(nameof(Plan.FechaActualizacion));

            if (ModelState.IsValid)
            {
                plan.IdPlan = 0;
                plan.Activo = true;
                plan.FechaCreacion = DateTime.Now;
                plan.FechaActualizacion = DateTime.Now;

                _context.Planes.Add(plan);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Plan registrado correctamente.";

                return RedirectToAction(nameof(Index));
            }

            return View(plan);
        }

        // MOSTRAR FORMULARIO DE EDICIÓN
        [HttpGet]
        public async Task<IActionResult> Edit(uint? id)
        {
            if (id == null)
                return NotFound();

            var plan = await _context.Planes.FindAsync(id);

            if (plan == null)
                return NotFound();

            return View(plan);
        }

        // GUARDAR CAMBIOS
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            uint id,
            string nombre,
            string? descripcion,
            decimal precio,
            ushort duracionDias)
        {
            var plan = await _context.Planes.FindAsync(id);

            if (plan == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(nombre) ||
                precio <= 0 || duracionDias == 0)
            {
                ModelState.AddModelError("", "Verifica los datos del plan.");
                return View(plan);
            }

            plan.Nombre = nombre.Trim();
            plan.Descripcion = descripcion;
            plan.Precio = precio;
            plan.DuracionDias = duracionDias;
            plan.FechaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Plan actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // ACTIVAR O DESACTIVAR PLAN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(uint id)
        {
            var plan = await _context.Planes.FindAsync(id);

            if (plan == null)
                return NotFound();

            plan.Activo = !plan.Activo;
            plan.FechaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Estado del plan actualizado.";

            return RedirectToAction(nameof(Index));
        }
    }
}
    