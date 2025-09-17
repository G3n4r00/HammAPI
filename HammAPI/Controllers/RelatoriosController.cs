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

        /// <summary>
        /// Gera um relatório financeiro para o usuário informado.
        /// </summary>
        /// <param name="usuarioId">ID do usuário</param>
        /// <param name="mes">Mês de referência (opcional)</param>
        /// <param name="ano">Ano de referência (opcional)</param>
        /// <returns>Retorna o relatório em formato JSON no corpo da resposta</returns>
        [HttpGet("{usuarioId}")]
        public async Task<ActionResult<RelatorioDTO>> GerarRelatorio(
            Guid usuarioId, [FromQuery] int? mes, [FromQuery] int? ano)
        {
            var relatorio = await _relatoriosService.GerarRelatorioAsync(usuarioId, mes, ano);
            return Ok(relatorio);
        }

        /// <summary>
        /// Gera e baixa o relatório financeiro como arquivo JSON.
        /// </summary>
        /// <param name="usuarioId">ID do usuário</param>
        /// <param name="mes">Mês de referência (opcional)</param>
        /// <param name="ano">Ano de referência (opcional)</param>
        /// <returns>Arquivo JSON para download</returns>
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
