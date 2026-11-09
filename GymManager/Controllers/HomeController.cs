
using System.Diagnostics;
using GymManager.Data;
using GymManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManager.Filters;

namespace GymManager.Controllers
{
    [AdminFilter]
    public class HomeController : Controller
    {
        private readonly GymDbContext _context;

        public HomeController(GymDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var hoy = DateTime.Today;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
            var inicioMesSiguiente = inicioMes.AddMonths(1);

            // Total de socios
            ViewBag.TotalSocios = await _context.Socios.CountAsync();

            // Socios activos
            ViewBag.SociosActivos = await _context.Socios
                .CountAsync(s => s.Estado == "Activo");

            // Suscripciones vigentes
            ViewBag.SuscripcionesVigentes = await _context.Suscripciones
                .CountAsync(s =>
                    s.Estado == "Activa" &&
                    s.FechaInicio <= hoy &&
                    s.FechaFin >= hoy);

            // Ingresos del mes
            ViewBag.IngresosMes = await _context.Pagos
                .Where(p =>
                    p.Estado == "Registrado" &&
                    p.FechaPago >= inicioMes &&
                    p.FechaPago < inicioMesSiguiente)
                .SumAsync(p => (decimal?)p.Monto) ?? 0m;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
