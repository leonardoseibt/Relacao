using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for SelRelMateriaPrima.xaml
    /// </summary>
    public partial class SelRelMateriaPrima : Window
    {
        public SelRelMateriaPrima()
        {
            InitializeComponent();
        }

        private void Confirm_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (checkTipoMateriaPrima.IsChecked == true || checkTipoMateriaPrima.IsChecked == false && !comboTipoMateriaPrima.SelectedIndex.Equals(-1))
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
            string reportFile = "RelMateriaPrima.rpt";
            ReportDocument relatorio = new ReportDocument();
            WindowCrystalReports formulario = new WindowCrystalReports();
            Dictionary<string, string> parametros = new Dictionary<string, string>(); ;

            string tipomateriaprima;

            if (checkTipoMateriaPrima.IsChecked == true)
                tipomateriaprima = "*";
            else
                tipomateriaprima = comboTipoMateriaPrima.Text.Trim();

            parametros.Add("Tipo", tipomateriaprima);

            formulario.Titulo = "Listagem de MATÉRIAS-PRIMAS";

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

        private void checkTipoMateriaPrima_Checked(object sender, RoutedEventArgs e)
        {
            comboTipoMateriaPrima.IsEnabled = false;
        }

        private void checkTipoMateriaPrima_Unchecked(object sender, RoutedEventArgs e)
        {
            comboTipoMateriaPrima.IsEnabled = true;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable tipos = new DataTable();

            string queryTipos = "SELECT ID,DESCRICAO FROM TIPOMATERIAPRIMA ORDER BY DESCRICAO";

            if (sqlite.Connect())
            {
                tipos = sqlite.GetTable(queryTipos);

                comboTipoMateriaPrima.ItemsSource = tipos.DefaultView;

                sqlite.Disconnect();
                sqlite = null;
            }
        }

    }
}
