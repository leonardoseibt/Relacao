
namespace Relacao.Classes
{
    class Produto
    {
        public long ID { get; set; }
        public Particularidade Particularidade { get; set; }
        public Linha Linha { get; set; }
        public string Referencia { get; set; }
        public string Medidas { get; set; }
        public string Descricao { get; set; }
        public string Observacoes { get; set; }

        public Produto()
        {
            Particularidade = new Particularidade();
            Linha = new Linha();
        }
    }
}
