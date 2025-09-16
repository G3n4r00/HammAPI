using HammAPI.DTOs;
using HammAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HammAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RelatoriosController : ControllerBase
    {
        private readonly RelatorioService _relatoriosService;

        public RelatoriosController(RelatorioService relatoriosService)
        {
            _relatoriosService = relatoriosService;
        }

        [HttpGet("{usuarioId}")]
        public async Task<ActionResult<RelatorioDTO>> GerarRelatorio(
            Guid usuarioId, [FromQuery] int? mes, [FromQuery] int? ano)
        {
            var relatorio = await _relatoriosService.GerarRelatorioAsync(usuarioId, mes, ano);
            return Ok(relatorio);
        }

        [HttpGet("{usuarioId}/download")]
        public async Task<IActionResult> DownloadRelatorio(
        Guid usuarioId, [FromQuery] int? mes, [FromQuery] int? ano)
        {
            var relatorio = await _relatoriosService.GerarRelatorioAsync(usuarioId, mes, ano);

            // Serializa para JSON
            var json = System.Text.Json.JsonSerializer.Serialize(relatorio,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

            // Converte para bytes
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);

            // Define nome do arquivo
            var fileName = $"relatorio_{usuarioId}_{mes ?? 0}_{ano ?? DateTime.UtcNow.Year}.json";

            return File(bytes, "application/json", fileName);
        }
    }
}
