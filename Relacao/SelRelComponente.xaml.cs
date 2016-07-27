using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for SelRelComponente.xaml
    /// </summary>
    public partial class SelRelComponente : Window
    {
        public SelRelComponente()
        {
            InitializeComponent();
        }

        private void checkTipoComponente_Checked(object sender, RoutedEventArgs e)
        {
            comboTipoComponente.IsEnabled = false;
        }

        private void checkTipoComponente_Unchecked(object sender, RoutedEventArgs e)
        {
            comboTipoComponente.IsEnabled = true;
        }

        private void checkMateriaPrima_Checked(object sender, RoutedEventArgs e)
        {
            comboMateriaPrima.IsEnabled = false;
        }

        private void checkMateriaPrima_Unchecked(object sender, RoutedEventArgs e)
        {
            comboMateriaPrima.IsEnabled = true;
        }

        private void Confirm_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((checkTipoComponente.IsChecked == true || checkTipoComponente.IsChecked == false && !comboTipoComponente.SelectedIndex.Equals(-1)) &&
                (checkMateriaPrima.IsChecked == true || checkMateriaPrima.IsChecked == false && !comboMateriaPrima.SelectedIndex.Equals(-1)))
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void Confirm_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string path;
            string reportFile = "RelComponente.rpt";
            ReportDocument relatorio = new ReportDocument();
            WindowCrystalReports formulario = new WindowCrystalReports();
            Dictionary<string, string> parametros = new Dictionary<string, string>(); ;

            string tipocomponente;
            string materiaprima;

            if (checkTipoComponente.IsChecked == true)
                tipocomponente = "*";
            else
                tipocomponente = comboTipoComponente.Text.Trim();

            parametros.Add("TipoComponente", tipocomponente);

            if (checkMateriaPrima.IsChecked == true)
                materiaprima = "*";
            else
                materiaprima = comboMateriaPrima.Text.Trim();

            parametros.Add("MateriaPrima", materiaprima);

            formulario.Titulo = "Listagem de COMPONENTES";

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
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable tipos = new DataTable();
            DataTable materiasprimas = new DataTable();

            string queryTipos = "SELECT ID,DESCRICAO FROM TIPOCOMPONENTE ORDER BY DESCRICAO";
            string queryLinhas = "SELECT ID,DESCRICAO FROM MATERIAPRIMA ORDER BY DESCRICAO";

            if (sqlite.Connect())
            {
                tipos = sqlite.GetTable(queryTipos);
                materiasprimas = sqlite.GetTable(queryLinhas);

                comboTipoComponente.ItemsSource = tipos.DefaultView;
                comboMateriaPrima.ItemsSource = materiasprimas.DefaultView;

                sqlite.Disconnect();
                sqlite = null;
            }
        }

    }
}
