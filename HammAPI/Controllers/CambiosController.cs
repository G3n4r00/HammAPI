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
        /// <param name="moeda">sigla da moeda alvo</param>
        [HttpGet("cotacao/{moeda}")]
        public async Task<ActionResult<CambioCotacaoDto>> GetCotacao(string moeda)
        {
            var cotacao = await _cambioService.ObterCotacaoAsync(moeda.ToLower());
            if (cotacao == null) return NotFound(new { message = "Moeda não encontrada." });
            return Ok(cotacao);
        }

        /// <summary>
        /// Converte um valor em reais para a moeda especificada
        /// </summary>
        /// <param name="moeda">sigla da moeda alvo</param>
        [HttpGet("converter/{moeda}")]
        public async Task<ActionResult<ConversaoResultadoDto>> Converter(
            string moeda, [FromQuery] decimal valor)
        {
            var resultado = await _cambioService.ConverterAsync(moeda.ToLower(), valor);
            if (resultado == null) return NotFound(new { message = "Moeda não encontrada." });
            return Ok(resultado);
        }
    }
}
