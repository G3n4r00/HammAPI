using HammAPI.DTOs;

namespace HammAPI.Services
{
    public class CambioService
    {
        private readonly HttpClient _httpClient;

        public CambioService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://br.dolarapi.com/v1/cotacoes/");
        }

        public async Task<CambioCotacaoDto?> ObterCotacaoAsync(string moeda)
        {
            var response = await _httpClient.GetAsync(moeda);
            if (!response.IsSuccessStatusCode) return null;

            var cotacao = await response.Content.ReadFromJsonAsync<CambioCotacaoDto>();
            return cotacao;
        }

        public async Task<ConversaoResultadoDto?> ConverterAsync(string moeda, decimal valorEmReais)
        {
            var cotacao = await ObterCotacaoAsync(moeda);
            if (cotacao == null) return null;

            return new ConversaoResultadoDto
            {
                Moeda = cotacao.Moeda,
                Nome = cotacao.Nome,
                ValorEmReais = valorEmReais,
                ValorConvertido = valorEmReais / cotacao.Compra, // usar taxa de compra
                TaxaCompra = cotacao.Compra,
                DataAtualizacao = cotacao.DataAtualizacao
            };
        }
    }
}
