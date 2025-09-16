namespace HammAPI.DTOs
{
    public class CambioCotacaoDto
    {
        public string Moeda { get; set; }
        public string Nome { get; set; }
        public decimal Compra { get; set; }
        public decimal Venda { get; set; }
        public decimal FechoAnterior { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }

    public class ConversaoResultadoDto
    {
        public string Moeda { get; set; }
        public string Nome { get; set; }
        public decimal ValorEmReais { get; set; }
        public decimal ValorConvertido { get; set; }
        public decimal TaxaCompra { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}
