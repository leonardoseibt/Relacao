
namespace Relacao.Classes
{
    class FichaTecnicaAgrupada
    {
        public Relatorio Relatorio { get; set; }
        public Produto Produto { get; set; }
        public int Quantidade { get; set; }

        public FichaTecnicaAgrupada()
        {
            this.Relatorio = new Relatorio();
            this.Produto = new Produto();
        }
    }
}
