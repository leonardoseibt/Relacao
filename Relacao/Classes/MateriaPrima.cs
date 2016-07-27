
namespace Relacao.Classes
{
    class MateriaPrima
    {
        public long ID { get; set; }
        public SubTipoMateriaPrima SubTipo { get; set; }
        public string Descricao { get; set; }
        public string Bitola { get; set; }
        public int Perda { get; set; }
        public string Observacoes { get; set; }

        public MateriaPrima()
        {
            SubTipo = new SubTipoMateriaPrima();
        }
    }
}
