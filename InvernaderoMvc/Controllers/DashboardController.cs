using ArcgisLoginApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace ArcgisLoginApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;

        public DashboardController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        // Vista común: enlaces a paneles según rol
        public IActionResult Index()
        {
            return View();
        }

        // —————————————————————————————————————————————
        // ADMIN: Promedios diarios
        // —————————————————————————————————————————————
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AdminPanel()
        {
            var client = _httpFactory.CreateClient("NodeRed");
            // 1) Llamamos a /datos para obtener todos los registros
            var sensores = await client.GetFromJsonAsync<List<Sensor>>("/datos");

            // 2) Agrupamos por fecha (sólo día) y calculamos promedios
            var promediosPorDia = sensores!
                .GroupBy(s => s.FechaHora.Date)
                .Select(g => new
                {
                    Fecha      = g.Key,
                    TempProm   = g.Average(x => x.temperatue),
                    HumProm    = g.Average(x => x.humidity),
                    NivelProm  = g.Average(x => x.nivel),
                    PhProm     = g.Average(x => x.ph)
                })
                .OrderBy(x => x.Fecha)
                .ToList();

            return View(promediosPorDia);
        }

        // —————————————————————————————————————————————
        // COORD: Datos en tiempo real + alertas
        // —————————————————————————————————————————————
        [Authorize(Roles = "Coordinator")]
        public IActionResult CoordinatorPanel()
        {
            return View();
        }
    }
}
