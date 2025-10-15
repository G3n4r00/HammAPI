using HammAPI.DTOs;
using HammAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace HammAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RelatoriosController : ControllerBase
    {
        private readonly RelatorioService _relatoriosService;

        public RelatoriosController(RelatorioService relatoriosService)
        {
            _relatoriosService = relatoriosService;
        }

        /// <summary>
        /// Gera um relatório financeiro para o usuário informado.
        /// </summary>
        /// <param name="mes">Mês de referência (opcional)</param>
        /// <param name="ano">Ano de referência (opcional)</param>
        /// <returns>Retorna o relatório em formato JSON no corpo da resposta</returns>
        [HttpGet]
        public async Task<ActionResult<RelatorioDTO>> GerarRelatorio([FromQuery] int? mes, [FromQuery] int? ano)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (usuarioId == null) return Unauthorized();

            var relatorio = await _relatoriosService.GerarRelatorioAsync(Guid.Parse(usuarioId), mes, ano);
            return Ok(relatorio);
        }

        /// <summary>
        /// Gera e baixa o relatório financeiro como arquivo JSON.
        /// </summary>
        /// <param name="mes">Mês de referência (opcional)</param>
        /// <param name="ano">Ano de referência (opcional)</param>
        /// <returns>Arquivo JSON para download</returns>
        [HttpGet("download")]
        public async Task<IActionResult> DownloadRelatorio([FromQuery] int? mes, [FromQuery] int? ano)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (usuarioId == null) return Unauthorized();

            var relatorio = await _relatoriosService.GerarRelatorioAsync(Guid.Parse(usuarioId), mes, ano);

            var json = System.Text.Json.JsonSerializer.Serialize(relatorio,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            var fileName = $"relatorio_{usuarioId}_{mes ?? 0}_{ano ?? DateTime.UtcNow.Year}.json";

            return File(bytes, "application/json", fileName);
        }
    }
}
