
namespace Relacao.Classes
{
    class Componente
    {
        public long ID { get; set; }
        public TipoComponente Tipo { get; set; }
        public MateriaPrima MateriaPrima { get; set; }
        public string Codigo { get; set; }
        public int Comprimento { get; set; }
        public int Largura { get; set; }
        public int Espessura { get; set; }
        
        public Componente()
        {
            Tipo = new TipoComponente();
            MateriaPrima = new MateriaPrima();
            Codigo = "";
        }
    }
}
