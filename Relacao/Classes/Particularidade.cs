
namespace Relacao.Classes
{
    class Particularidade
    {
        public long ID { get; set; }
        public TipoProduto Tipo { get; set; }
        public string Descricao { get; set; }

        public Particularidade()
        {
            Tipo = new TipoProduto();
        }

    }
}
