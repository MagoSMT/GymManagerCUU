using GymManager.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManager.Filters;

namespace GymManager.Controllers
{
    [AdminFilter]
    public class ReportesController : Controller
    {
        private readonly GymDbContext _context;

        public ReportesController(GymDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var totalSocios = await _context.Socios.CountAsync();

            var sociosActivos = await _context.Socios
                .CountAsync(s => s.Estado == "Activo");

            var totalPlanes = await _context.Planes.CountAsync();

            var suscripcionesActivas = await _context.Suscripciones
                .CountAsync(s => s.Estado == "Activa");

            var pagosRegistrados = await _context.Pagos
                .CountAsync(p => p.Estado == "Registrado");

            var ingresos = await _context.Pagos
                .Where(p => p.Estado == "Registrado")
                .SumAsync(p => (decimal?)p.Monto) ?? 0;

            ViewBag.TotalSocios = totalSocios;
            ViewBag.SociosActivos = sociosActivos;
            ViewBag.TotalPlanes = totalPlanes;
            ViewBag.SuscripcionesActivas = suscripcionesActivas;
            ViewBag.PagosRegistrados = pagosRegistrados;
            ViewBag.Ingresos = ingresos;

            return View();
        }
    }
}