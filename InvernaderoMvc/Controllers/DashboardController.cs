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

        // —————————————————————————————————————————————
        // VISTA INICIAL: Enlaces a paneles según el rol
        // —————————————————————————————————————————————
        public IActionResult Index()
        {
            return View();
        }

        // —————————————————————————————————————————————
        // PANEL ADMIN: Promedios diarios por fecha
        // —————————————————————————————————————————————
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AdminPanel()
        {
            var client = _httpFactory.CreateClient("NodeRed");
            try
            {
                var sensores = await client.GetFromJsonAsync<List<Sensor>>("/datos");

                if (sensores == null || sensores.Count == 0)
                {
                    ViewBag.Error = "No se encontraron datos.";
                    return View(new List<object>());
                }

                var promediosPorDia = sensores
                    .GroupBy(s => s.FechaHora.Date)
                    .Select(g => new
                    {
                        Fecha     = g.Key,
                        TempProm  = g.Average(x => x.temperatue),
                        HumProm   = g.Average(x => x.humidity),
                        NivelProm = g.Average(x => x.nivel),
                        PhProm    = g.Average(x => x.ph)
                    })
                    .OrderBy(x => x.Fecha)
                    .ToList();

                return View(promediosPorDia);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al obtener datos: {ex.Message}";
                return View(new List<object>());
            }
        }

        // —————————————————————————————————————————————
        // PANEL COORDINADOR: Datos en tiempo real
        // —————————————————————————————————————————————
        [Authorize(Roles = "Coordinator")]
        public IActionResult CoordinatorPanel()
        {
            return View();
        }

        // —————————————————————————————————————————————
        // API PARA FETCH EN JAVASCRIPT (DATOS)
        // —————————————————————————————————————————————

        [AllowAnonymous]
        [HttpGet("/datos")]
        public async Task<IActionResult> GetDatos()
        {
            var client = _httpFactory.CreateClient("NodeRed");
            try
            {
                var json = await client.GetStringAsync("/datos");
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener datos de Node-RED: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("/datos1")]
        public async Task<IActionResult> GetDatos1()
        {
            var client = _httpFactory.CreateClient("NodeRed");
            try
            {
                var json = await client.GetStringAsync("/datos1");
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener datos1 de Node-RED: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("/datos2")]
        public async Task<IActionResult> GetDatos2()
        {
            var client = _httpFactory.CreateClient("NodeRed");
            try
            {
                var json = await client.GetStringAsync("/datos2");
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener datos2 de Node-RED: {ex.Message}");
            }
        }
    }
}
