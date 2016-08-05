
using System.Windows.Media;
namespace Relacao.Classes
{
    class FichaTecnica
    {
        public long ID { get; set; }
        public Produto Produto { get; set; }
        public Componente Componente { get; set; }
        public decimal Quantidade { get; set; }
        public bool Lixada { get; set; }
        public bool Aproveitamento { get; set; }
        public string Agrupamento { get; set; }
        public string Observacoes { get; set; }

        public SolidColorBrush CorComprimento { get; set; }
        public SolidColorBrush CorLargura { get; set; }

        public bool Alterado { get; set; }

        public FichaTecnica()
        {
            Produto = new Produto();
            Componente = new Componente();
        }

    }
}
