
using GymManager.Data;
using GymManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManager.Filters;

namespace GymManager.Controllers
{
    [AdminFilter]
    public class SuscripcionesController : Controller
    {
        private readonly GymDbContext _context;

        public SuscripcionesController(GymDbContext context)
        {
            _context = context;
        }

        // LISTAR SUSCRIPCIONES
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var suscripciones = await _context.Suscripciones
                .Include(s => s.Socio)
                .Include(s => s.Plan)
                .AsNoTracking()
                .OrderByDescending(s => s.FechaCreacion)
                .ToListAsync();

            return View(suscripciones);
        }

        // MOSTRAR FORMULARIO
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CargarListas();
            return View();
        }

        // REGISTRAR SUSCRIPCIÓN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            uint idSocio,
            uint idPlan,
            DateTime fechaInicio,
            string? observaciones)
        {
            var socio = await _context.Socios.FindAsync(idSocio);
            var plan = await _context.Planes.FindAsync(idPlan);

            if (socio == null || plan == null || !plan.Activo)
            {
                ModelState.AddModelError("", "Selecciona un socio y un plan válidos.");
            }

            if (fechaInicio == default)
            {
                ModelState.AddModelError("", "Selecciona una fecha de inicio.");
            }

            if (!ModelState.IsValid)
            {
                await CargarListas();
                return View();
            }

            var suscripcion = new Suscripcion
            {
                IdSocio = idSocio,
                IdPlan = idPlan,
                FechaInicio = fechaInicio.Date,
                FechaFin = fechaInicio.Date.AddDays(plan!.DuracionDias),
                PrecioContratado = plan.Precio,
                Estado = "Pendiente",
                Observaciones = observaciones
            };

            _context.Suscripciones.Add(suscripcion);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Suscripción registrada correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // EDITAR SUSCRIPCIÓN
        [HttpGet]
        public async Task<IActionResult> Edit(uint? id)
        {
            if (id == null)
                return NotFound();

            var suscripcion = await _context.Suscripciones.FindAsync(id);

            if (suscripcion == null)
                return NotFound();

            await CargarListas();
            return View(suscripcion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            uint id,
            uint idSocio,
            uint idPlan,
            DateTime fechaInicio,
            DateTime fechaFin,
            decimal precioContratado,
            string estado,
            string? observaciones)
        {
            var suscripcion = await _context.Suscripciones.FindAsync(id);

            if (suscripcion == null)
                return NotFound();

            var socio = await _context.Socios.FindAsync(idSocio);
            var plan = await _context.Planes.FindAsync(idPlan);

            string[] estadosValidos =
            {
                "Pendiente", "Activa", "Vencida", "Cancelada"
            };

            if (socio == null || plan == null ||
                fechaFin.Date < fechaInicio.Date ||
                precioContratado <= 0 ||
                !estadosValidos.Contains(estado))
            {
                ModelState.AddModelError("", "Verifica los datos de la suscripción.");
                await CargarListas();
                return View(suscripcion);
            }

            suscripcion.IdSocio = idSocio;
            suscripcion.IdPlan = idPlan;
            suscripcion.FechaInicio = fechaInicio.Date;
            suscripcion.FechaFin = fechaFin.Date;
            suscripcion.PrecioContratado = precioContratado;
            suscripcion.Estado = estado;
            suscripcion.Observaciones = observaciones;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Suscripción actualizada correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // CAMBIAR ESTADO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(uint id, string estado)
        {
            string[] estadosValidos =
            {
                "Pendiente", "Activa", "Vencida", "Cancelada"
            };

            if (!estadosValidos.Contains(estado))
                return BadRequest();

            var suscripcion = await _context.Suscripciones.FindAsync(id);

            if (suscripcion == null)
                return NotFound();

            suscripcion.Estado = estado;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Estado actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // CARGAR SOCIOS Y PLANES
        private async Task CargarListas()
        {
            ViewBag.Socios = await _context.Socios
                .Where(s => s.Estado == "Activo")
                .OrderBy(s => s.Nombre)
                .ToListAsync();

            ViewBag.Planes = await _context.Planes
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }
    }
}
