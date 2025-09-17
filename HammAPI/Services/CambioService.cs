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

        //retornar a cotacao da moeda especifica
        public async Task<CambioCotacaoDto?> ObterCotacaoAsync(string moeda)
        {
            var response = await _httpClient.GetAsync(moeda);
            if (!response.IsSuccessStatusCode) return null;

            var cotacao = await response.Content.ReadFromJsonAsync<CambioCotacaoDto>();
            return cotacao;
        }

        //retornar a conversao de um valor em reais para uma moeda especificado
        public async Task<ConversaoResultadoDto?> ConverterAsync(string moeda, decimal valorEmReais)
        {
            var cotacao = await ObterCotacaoAsync(moeda);
            if (cotacao == null) return null;

            return new ConversaoResultadoDto
            {
                Moeda = cotacao.Moeda,
                Nome = cotacao.Nome,
                ValorEmReais = valorEmReais,
                ValorConvertido = Math.Round((valorEmReais / cotacao.Compra), 3), // usar taxa de compra
                TaxaCompra = cotacao.Compra,
                DataAtualizacao = cotacao.DataAtualizacao
            };
        }
    }
}
