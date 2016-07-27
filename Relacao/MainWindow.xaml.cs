using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string empresa = ConfigurationManager.AppSettings["NomeEmpresa"];

        public MainWindow()
        {
            InitializeComponent();
        }

        private void menuCadTipoMateriaPrima_Click(object sender, RoutedEventArgs e)
        {
            CadTipoMateriaPrima formulario = new CadTipoMateriaPrima();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void menuCadSubTipoMateriaPrima_Click(object sender, RoutedEventArgs e)
        {
            CadSubTipoMateriaPrima formulario = new CadSubTipoMateriaPrima();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void menuCadMateriaPrima_Click(object sender, RoutedEventArgs e)
        {
            CadMateriaPrima formulario = new CadMateriaPrima();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void btnCadMateriaPrima_Click(object sender, RoutedEventArgs e)
        {
            CadMateriaPrima formulario = new CadMateriaPrima();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void menuCadTipoProduto_Click(object sender, RoutedEventArgs e)
        {
            CadTipoProduto formulario = new CadTipoProduto();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void menuCadParticularidade_Click(object sender, RoutedEventArgs e)
        {
            CadParticularidade formulario = new CadParticularidade();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void menuCadLinha_Click(object sender, RoutedEventArgs e)
        {
            CadLinha formulario = new CadLinha();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void menuCadTipoComponente_Click(object sender, RoutedEventArgs e)
        {
            CadTipoComponente formulario = new CadTipoComponente();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void menuCadProduto_Click(object sender, RoutedEventArgs e)
        {
            CadProduto formulario = new CadProduto();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void btnCadProdutos_Click(object sender, RoutedEventArgs e)
        {
            CadProduto formulario = new CadProduto();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void btnCadComponentes_Click(object sender, RoutedEventArgs e)
        {
            CadComponente formulario = new CadComponente();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void menuCadComponente_Click(object sender, RoutedEventArgs e)
        {
            CadComponente formulario = new CadComponente();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void btnCadFichaTecnica_Click(object sender, RoutedEventArgs e)
        {
            CadFichaTecnica formulario = new CadFichaTecnica();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void menuManFichaTecnica_Click(object sender, RoutedEventArgs e)
        {
            CadFichaTecnica formulario = new CadFichaTecnica();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void menuSair_Click(object sender, RoutedEventArgs e)
        {
            GC.Collect();

            this.Close();
        }

        private void menuRelLinha_Click(object sender, RoutedEventArgs e)
        {
            string path = "";
            string reportFile = "RelLinha.rpt";
            ReportDocument relatorio = new ReportDocument();
            WindowCrystalReports formulario = new WindowCrystalReports();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            formulario.Titulo = "Listagem de LINHAS/COLEÇÕES";

            if (System.Diagnostics.Debugger.IsAttached)
            {
                path = @"C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\Relatorios\" + reportFile;
            }
            else
            {
                path = System.AppDomain.CurrentDomain.BaseDirectory + @"Relatorios\" + reportFile;
            }

            try
            {
                relatorio.Load(path);

                if (relatorio.IsLoaded)
                {
                    formulario.Relatorio = relatorio;

                    if (parametros != null)
                    {
                        foreach (KeyValuePair<string, string> par in parametros)
                        {
                            formulario.Parametros.Add(par.Key, par.Value);
                        }
                    }

                    formulario.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Erro ao abrir relatório\n" + ex.ToString(),
                    "Relatório", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            relatorio.Dispose();
            formulario = null;
            parametros = null;

            GC.Collect();
        }

        private void menuRelTipoComponente_Click(object sender, RoutedEventArgs e)
        {
            string path;
            string reportFile = "RelTipoComponente.rpt";
            ReportDocument relatorio = new ReportDocument();
            WindowCrystalReports formulario = new WindowCrystalReports();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            formulario.Titulo = "Listagem de TIPOS DE COMPONENTE";

            if (System.Diagnostics.Debugger.IsAttached)
            {
                path = @"C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\Relatorios\" + reportFile;
            }
            else
            {
                path = System.AppDomain.CurrentDomain.BaseDirectory + @"Relatorios\" + reportFile;
            }

            try
            {
                relatorio.Load(path);

                if (relatorio.IsLoaded)
                {
                    formulario.Relatorio = relatorio;

                    if (parametros != null)
                    {
                        foreach (KeyValuePair<string, string> par in parametros)
                        {
                            formulario.Parametros.Add(par.Key, par.Value);
                        }
                    }

                    formulario.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Erro ao abrir relatório\n" + ex.ToString(),
                    "Relatório", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            relatorio.Dispose();
            formulario = null;
            parametros = null;

            GC.Collect();
        }

        private void menuRelTipoMateriaPrima_Click(object sender, RoutedEventArgs e)
        {
            string path;
            string reportFile = "RelTipoMateriaPrima.rpt";
            ReportDocument relatorio = new ReportDocument();
            WindowCrystalReports formulario = new WindowCrystalReports();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            formulario.Titulo = "Listagem de TIPOS DE MATÉRIA-PRIMA";

            if (System.Diagnostics.Debugger.IsAttached)
            {
                path = @"C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\Relatorios\" + reportFile;
            }
            else
            {
                path = System.AppDomain.CurrentDomain.BaseDirectory + @"Relatorios\" + reportFile;
            }

            try
            {
                relatorio.Load(path);

                if (relatorio.IsLoaded)
                {
                    formulario.Relatorio = relatorio;

                    if (parametros != null)
                    {
                        foreach (KeyValuePair<string, string> par in parametros)
                        {
                            formulario.Parametros.Add(par.Key, par.Value);
                        }
                    }

                    formulario.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Erro ao abrir relatório\n" + ex.ToString(),
                    "Relatório", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            relatorio.Dispose();
            formulario = null;
            parametros = null;

            GC.Collect();
        }

        private void menuRelTipoProduto_Click(object sender, RoutedEventArgs e)
        {
            string path;
            string reportFile = "RelTipoProduto.rpt";
            ReportDocument relatorio = new ReportDocument();
            WindowCrystalReports formulario = new WindowCrystalReports();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            formulario.Titulo = "Listagem de TIPOS DE PRODUTO";

            if (System.Diagnostics.Debugger.IsAttached)
            {
                path = @"C:\Users\Leonardo Seibt\Documents\Visual Studio 2013\Projects\Relacao\Relacao\Relatorios\" + reportFile;
            }
            else
            {
                path = System.AppDomain.CurrentDomain.BaseDirectory + @"Relatorios\" + reportFile;
            }

            try
            {
                relatorio.Load(path);

                if (relatorio.IsLoaded)
                {
                    formulario.Relatorio = relatorio;

                    if (parametros != null)
                    {
                        foreach (KeyValuePair<string, string> par in parametros)
                        {
                            formulario.Parametros.Add(par.Key, par.Value);
                        }
                    }

                    formulario.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Erro ao abrir relatório\n" + ex.ToString(),
                    "Relatório", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            relatorio.Dispose();
            formulario = null;
            parametros = null;

            GC.Collect();
        }

        private void menuRelProduto_Click(object sender, RoutedEventArgs e)
        {
            SelRelProduto formulario = new SelRelProduto();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void menuRelComponente_Click(object sender, RoutedEventArgs e)
        {
            SelRelComponente formulario = new SelRelComponente();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void menuRelMateriaPrima_Click(object sender, RoutedEventArgs e)
        {
            SelRelMateriaPrima formulario = new SelRelMateriaPrima();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void menuRelFichaTecnica_Click(object sender, RoutedEventArgs e)
        {
            SelRelFichaTecnica formulario = new SelRelFichaTecnica();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void btnRelFichaTecnica_Click(object sender, RoutedEventArgs e)
        {
            SelRelFichaTecnica formulario = new SelRelFichaTecnica();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void btnConfiguracoes_Click(object sender, RoutedEventArgs e)
        {
            WindowConfiguracoes formulario = new WindowConfiguracoes();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void menuConfiguracoes_Click(object sender, RoutedEventArgs e)
        {
            WindowConfiguracoes formulario = new WindowConfiguracoes();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void menuRelFichaTecnicaAgrupada_Click(object sender, RoutedEventArgs e)
        {
            SelRelFichaTecnicaAgrupada formulario = new SelRelFichaTecnicaAgrupada();
            formulario.ShowDialog();
        }

        private void btnRelFichaTecnicaAgrupada_Click(object sender, RoutedEventArgs e)
        {
            SelRelFichaTecnicaAgrupada formulario = new SelRelFichaTecnicaAgrupada();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void btnAlterarMedidas_Click(object sender, RoutedEventArgs e)
        {
            ManutMedidas formulario = new ManutMedidas();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void menuAlterarMedidas_Click(object sender, RoutedEventArgs e)
        {
            ManutMedidas formulario = new ManutMedidas();
            formulario.ShowDialog();

            GC.Collect();
        }

    }
}
