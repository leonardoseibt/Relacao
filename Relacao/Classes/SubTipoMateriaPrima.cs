
namespace Relacao.Classes
{
    class SubTipoMateriaPrima
    {
        public long ID { get; set; }
        public TipoMateriaPrima Tipo { get; set; }
        public string Descricao { get; set; }

        public SubTipoMateriaPrima()
        {
            Tipo = new TipoMateriaPrima();
        }

    }
}
