using GymManager.Data;
using GymManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManager.Filters;

namespace GymManager.Controllers
{
    [AdminFilter]
    public class PagosController : Controller
    {
        private readonly GymDbContext _context;

        public PagosController(GymDbContext context)
        {
            _context = context;
        }

        // Lista los pagos
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var pagos = await _context.Pagos
                .Include(p => p.Suscripcion)
                    .ThenInclude(s => s.Socio)
                .Include(p => p.Suscripcion)
                    .ThenInclude(s => s.Plan)
                .AsNoTracking()
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();

            return View(pagos);
        }

        // ver formulario
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CargarSuscripciones();
            return View();
        }

        // registrar pagos 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            uint idSuscripcion,
            decimal monto,
            DateTime fechaPago,
            string metodo,
            string? referencia,
            string? notas)
        {
            var suscripcion = await _context.Suscripciones
                .Include(s => s.Socio)
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.IdSuscripcion == idSuscripcion);

            string[] metodosValidos =
            {
                "Efectivo",
                "Transferencia",
                "Tarjeta",
                "Otro"
            };

            if (suscripcion == null)
            {
                ModelState.AddModelError("", "Selecciona una suscripción válida.");
            }

            if (monto <= 0)
            {
                ModelState.AddModelError("", "El monto debe ser mayor a cero.");
            }

            if (fechaPago == default)
            {
                ModelState.AddModelError("", "Selecciona una fecha de pago.");
            }

            if (!metodosValidos.Contains(metodo))
            {
                ModelState.AddModelError("", "Selecciona un método de pago válido.");
            }

            if (!ModelState.IsValid)
            {
                await CargarSuscripciones();
                return View();
            }

            var pago = new Pago
            {
                IdSuscripcion = idSuscripcion,
                Monto = monto,
                FechaPago = fechaPago,
                Metodo = metodo,
                Estado = "Registrado",
                Referencia = referencia,
                Notas = notas
            };

            _context.Pagos.Add(pago);

            // Cuando registras pago se activa.
            if (suscripcion!.FechaInicio <= fechaPago.Date &&
                suscripcion.FechaFin >= fechaPago.Date &&
                suscripcion.Estado != "Cancelada")
            {
                suscripcion.Estado = "Activa";
            }

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Pago registrado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // mostrar detalles
        [HttpGet]
        public async Task<IActionResult> Details(uint? id)
        {
            if (id == null)
                return NotFound();

            var pago = await _context.Pagos
                .Include(p => p.Suscripcion)
                    .ThenInclude(s => s.Socio)
                .Include(p => p.Suscripcion)
                    .ThenInclude(s => s.Plan)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdPago == id);

            if (pago == null)
                return NotFound();

            return View(pago);
        }

        // quitar pago 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Anular(uint id)
        {
            var pago = await _context.Pagos.FindAsync(id);

            if (pago == null)
                return NotFound();

            pago.Estado = "Anulado";

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Pago anulado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // cargar subs 
        private async Task CargarSuscripciones()
        {
            ViewBag.Suscripciones = await _context.Suscripciones
                .Include(s => s.Socio)
                .Include(s => s.Plan)
                .Where(s =>
                    s.Estado != "Cancelada" &&
                    s.Estado != "Vencida")
                .OrderByDescending(s => s.FechaCreacion)
                .ToListAsync();
        }
    }
}