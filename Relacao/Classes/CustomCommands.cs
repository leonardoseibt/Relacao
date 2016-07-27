using System.Windows.Input;

namespace Relacao
{
    public static class CustomCommands
    {
        private static RoutedUICommand _InsertMateriaPrima;
        private static RoutedUICommand _InsertTipoMateriaPrima;
        private static RoutedUICommand _InsertSubTipoMateriaPrima;
        private static RoutedUICommand _InsertTipoProduto;
        private static RoutedUICommand _InsertParticularidade;
        private static RoutedUICommand _InsertLinha;
        private static RoutedUICommand _InsertRelatorio;
        private static RoutedUICommand _InsertTipoComponente;
        private static RoutedUICommand _InsertProduto;
        private static RoutedUICommand _InsertComponente;
        private static RoutedUICommand _BuscarProduto;
        private static RoutedUICommand _BuscarRelatorio;
        private static RoutedUICommand _CadastrarProduto;
        private static RoutedUICommand _InsertComponenteFichaTecnica;
        private static RoutedUICommand _ConfirmaSelecaoRelProduto;
        private static RoutedUICommand _ConfirmaSelecaoRelComponente;
        private static RoutedUICommand _ConfirmaSelecaoRelMateriaPrima;
        private static RoutedUICommand _ConfirmaSelecaoRelFichaTecnica;
        private static RoutedUICommand _GravarConfiguracoes;
        private static RoutedUICommand _AgrupaProduto;
        private static RoutedUICommand _ImprimeSelecaoRelFichaTecnicaAgrupada;
        private static RoutedUICommand _ConfirmarRelatorio;
        private static RoutedUICommand _CopiarFichaTecnica;
        private static RoutedUICommand _AlterarMedidasFichaTecnica;
        
        static CustomCommands()
        {
            _InsertMateriaPrima = new RoutedUICommand("Inserir Matéria Prima", "InserirMateriaPrima", typeof(CustomCommands));
            _InsertTipoMateriaPrima = new RoutedUICommand("Inserir Tipo Matéria Prima", "InserirTipoMateriaPrima", typeof(CustomCommands));
            _InsertSubTipoMateriaPrima = new RoutedUICommand("Inserir SubTipo Matéria Prima", "InserirSubTipoMateriaPrima", typeof(CustomCommands));
            _InsertTipoProduto = new RoutedUICommand("Inserir Tipo Produto", "InserirTipoProduto", typeof(CustomCommands));
            _InsertParticularidade = new RoutedUICommand("Inserir Particularidade", "InserirParticvularidade", typeof(CustomCommands));
            _InsertLinha = new RoutedUICommand("Inserir Linha", "InserirLinha", typeof(CustomCommands));
            _InsertRelatorio = new RoutedUICommand("Inserir Relatório", "InserirRelatorio", typeof(CustomCommands));
            _InsertTipoComponente = new RoutedUICommand("Inserir Tipo Componente", "InserirTipoComponente", typeof(CustomCommands));
            _InsertProduto = new RoutedUICommand("Inserir Produto", "InserirProduto", typeof(CustomCommands));
            _InsertComponente = new RoutedUICommand("Inserir Componente", "InserirComponente", typeof(CustomCommands));
            _BuscarProduto = new RoutedUICommand("Buscar Produto", "BuscarProduto", typeof(CustomCommands));
            _BuscarRelatorio = new RoutedUICommand("Buscar Relatório", "BuscarRelatorio", typeof(CustomCommands));
            _CadastrarProduto = new RoutedUICommand("Cadastrar Produto", "CadastrarProduto", typeof(CustomCommands));
            _InsertComponenteFichaTecnica = new RoutedUICommand("Inserir Componente Ficha Tecnica", "InserirComponenteFichaTecnica", typeof(CustomCommands));
            _ConfirmaSelecaoRelProduto = new RoutedUICommand("Confirma Seleção Rel Produto", "ConfirmaSelecaoRelProduto", typeof(CustomCommands));
            _ConfirmaSelecaoRelComponente = new RoutedUICommand("Confirma Seleção Rel Componente", "ConfirmaSelecaoRelComponente", typeof(CustomCommands));
            _ConfirmaSelecaoRelMateriaPrima = new RoutedUICommand("Confirma Seleção Rel Materia Prima", "ConfirmaSelecaoRelMateriaPrima", typeof(CustomCommands));
            _ConfirmaSelecaoRelFichaTecnica = new RoutedUICommand("Confirma Seleção Rel Ficha Tecnica", "ConfirmaSelecaoRelFichaTecnica", typeof(CustomCommands));
            _GravarConfiguracoes = new RoutedUICommand("Gravar Configurações", "GravarConfiguracoes", typeof(CustomCommands));
            _AgrupaProduto = new RoutedUICommand("Agrupar Produto", "AgrupaProduto", typeof(CustomCommands));
            _ImprimeSelecaoRelFichaTecnicaAgrupada = new RoutedUICommand("Imprimir Seleção", "ImprimeSelecao", typeof(CustomCommands));
            _ConfirmarRelatorio = new RoutedUICommand("Confirmar Relatório", "ConfirmarRelatorio", typeof(CustomCommands));
            _CopiarFichaTecnica = new RoutedUICommand("Copiar Ficha Técnica", "CopiarFIchaTecnica", typeof(CustomCommands));
            _AlterarMedidasFichaTecnica = new RoutedUICommand("Alterar Medidas Ficha Técnica", "AlterarMedidasFichaTecnica", typeof(CustomCommands));
            
        }

        public static RoutedUICommand InsertMateriaPrima
        {
            get { return _InsertMateriaPrima; }
        }

        public static RoutedUICommand InsertTipoMateriaPrima
        {
            get { return _InsertTipoMateriaPrima; }
        }

        public static RoutedUICommand InsertSubTipoMateriaPrima
        {
            get { return _InsertSubTipoMateriaPrima; }
        }

        public static RoutedUICommand InsertTipoProduto
        {
            get { return _InsertTipoProduto; }
        }

        public static RoutedUICommand InsertParticularidade
        {
            get { return _InsertParticularidade; }
        }

        public static RoutedUICommand InsertLinha
        {
            get { return _InsertLinha; }
        }

        public static RoutedUICommand InsertTipoComponente
        {
            get { return _InsertTipoComponente; }
        }

        public static RoutedUICommand InsertProduto
        {
            get { return _InsertProduto; }
        }

        public static RoutedUICommand InsertComponente
        {
            get { return _InsertComponente; }
        }

        public static RoutedUICommand BuscarProduto
        {
            get { return _BuscarProduto; }
        }

        public static RoutedUICommand BuscarRelatorio
        {
            get { return _BuscarRelatorio; }
        }

        public static RoutedUICommand CadastrarProduto
        {
            get { return _CadastrarProduto; }
        }

        public static RoutedUICommand InsertComponenteFichaTecnica
        {
            get { return _InsertComponenteFichaTecnica; }
        }

        public static RoutedUICommand ConfirmaSelecaoRelProduto
        {
            get { return _ConfirmaSelecaoRelProduto; }
        }

        public static RoutedUICommand ConfirmaSelecaoRelComponente
        {
            get { return _ConfirmaSelecaoRelComponente; }
        }

        public static RoutedUICommand ConfirmaSelecaoRelMateriaPrima
        {
            get { return _ConfirmaSelecaoRelMateriaPrima; }
        }

        public static RoutedUICommand ConfirmaSelecaoRelFichaTecnica
        {
            get { return _ConfirmaSelecaoRelFichaTecnica; }
        }

        public static RoutedUICommand GravarConfiguracoes
        {
            get { return _GravarConfiguracoes; }
        }

        public static RoutedUICommand AgruparProduto
        {
            get { return _AgrupaProduto; }
        }

        public static RoutedUICommand ImprimeSelecaoRelFichaTecnicaAgrupada
        {
            get { return _ImprimeSelecaoRelFichaTecnicaAgrupada; }
        }

        public static RoutedUICommand ConfirmarRelatorio
        {
            get { return _ConfirmarRelatorio; }
        }

        public static RoutedUICommand InsertRelatorio
        {
            get { return _InsertRelatorio; }
        }

        public static RoutedUICommand CopiarFichaTecnica
        {
            get { return _CopiarFichaTecnica; }
        }

        public static RoutedUICommand AlterarMedidasFichaTecnica
        {
            get { return _AlterarMedidasFichaTecnica; }
        }

    }
}
