
namespace Relacao.Classes
{
    class RelatorioFichaTecnicaAgrupada
    {
        public Relatorio Relatorio { get; set; }
        public MateriaPrima MateriaPrima { get; set; }
        public decimal Quantidade { get; set; }
        public string Medidas { get; set; }
        public decimal Metragem { get; set; }

        public RelatorioFichaTecnicaAgrupada()
        {
            this.Relatorio = new Relatorio();
            this.MateriaPrima = new MateriaPrima();

        }
    }
}
