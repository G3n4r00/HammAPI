using HammAPI.DTOs;
using HammAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HammAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CambiosController : ControllerBase
    {
        private readonly CambioService _cambioService;

        public CambiosController(CambioService cambioService)
        {
            _cambioService = cambioService;
        }

        /// <summary>
        /// Consulta a cotação atual de uma moeda (ARS, USD, EUR, CLP, UYU)
        /// </summary>
        [HttpGet("cotacao/{moeda}")]
        public async Task<ActionResult<CambioCotacaoDto>> GetCotacao(string moeda)
        {
            var cotacao = await _cambioService.ObterCotacaoAsync(moeda.ToUpper());
            if (cotacao == null) return NotFound(new { message = "Moeda não encontrada." });
            return Ok(cotacao);
        }

        /// <summary>
        /// Converte um valor em reais para a moeda especificada
        /// </summary>
        [HttpGet("converter/{moeda}")]
        public async Task<ActionResult<ConversaoResultadoDto>> Converter(
            string moeda, [FromQuery] decimal valor)
        {
            var resultado = await _cambioService.ConverterAsync(moeda.ToUpper(), valor);
            if (resultado == null) return NotFound(new { message = "Moeda não encontrada." });
            return Ok(resultado);
        }
    }
}
