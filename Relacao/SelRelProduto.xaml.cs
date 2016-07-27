using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for SelRelProduto.xaml
    /// </summary>
    public partial class SelRelProduto : Window
    {
        public SelRelProduto()
        {
            InitializeComponent();
        }

        private void Confirm_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((checkLinha.IsChecked == true || checkLinha.IsChecked == false && !comboLinha.SelectedIndex.Equals(-1)) &&
                (checkTipoProduto.IsChecked == true || checkTipoProduto.IsChecked == false && !comboTipoProduto.SelectedIndex.Equals(-1)) &&
                (checkParticularidade.IsChecked == true || checkParticularidade.IsChecked == false && !comboParticularidade.SelectedIndex.Equals(-1)))
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
            string reportFile = "RelProduto.rpt";
            ReportDocument relatorio = new ReportDocument();
            WindowCrystalReports formulario = new WindowCrystalReports();
            Dictionary<string, string> parametros = new Dictionary<string, string>(); ;

            string linha;
            string tipo;
            string particularidade;

            if (checkLinha.IsChecked == true)
                linha = "*";
            else
                linha = comboLinha.Text.Trim();

            parametros.Add("Linha", linha);

            if (checkTipoProduto.IsChecked == true)
                tipo = "*";
            else
                tipo = comboTipoProduto.Text.Trim();

            parametros.Add("Tipo", tipo);

            if (checkParticularidade.IsChecked == true)
                particularidade = "*";
            else
                particularidade = comboParticularidade.Text.Trim();

            parametros.Add("Particularidade", particularidade);

            formulario.Titulo = "Listagem de PRODUTOS";

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

        private void checkLinha_Checked(object sender, RoutedEventArgs e)
        {
            comboLinha.IsEnabled = false;
        }

        private void checkLinha_Unchecked(object sender, RoutedEventArgs e)
        {
            comboLinha.IsEnabled = true;
        }

        private void checkTipoProduto_Checked(object sender, RoutedEventArgs e)
        {
            comboTipoProduto.IsEnabled = false;
        }

        private void checkTipoProduto_Unchecked(object sender, RoutedEventArgs e)
        {
            comboTipoProduto.IsEnabled = true;
        }

        private void checkParticularidade_Checked(object sender, RoutedEventArgs e)
        {
            comboParticularidade.IsEnabled = false;
        }

        private void checkParticularidade_Unchecked(object sender, RoutedEventArgs e)
        {
            comboParticularidade.IsEnabled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable tipos = new DataTable();
            DataTable linhas = new DataTable();

            string queryTipos = "SELECT ID,DESCRICAO FROM TIPOPRODUTO ORDER BY DESCRICAO";
            string queryLinhas = "SELECT ID,DESCRICAO FROM LINHA ORDER BY DESCRICAO";

            if (sqlite.Connect())
            {
                tipos = sqlite.GetTable(queryTipos);
                linhas = sqlite.GetTable(queryLinhas);

                comboTipoProduto.ItemsSource = tipos.DefaultView;
                comboLinha.ItemsSource = linhas.DefaultView;

                sqlite.Disconnect();
                sqlite = null;
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void comboTipoProduto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable particularidades = new DataTable();

            long tipoID = Convert.ToInt64(comboTipoProduto.SelectedValue);
            string queryParticularidades = "SELECT ID,DESCRICAO FROM PARTICULARIDADE WHERE IDTIPOPRODUTO=" + tipoID.ToString() + " ORDER BY DESCRICAO";

            if (sqlite.Connect())
            {
                particularidades = sqlite.GetTable(queryParticularidades);

                comboParticularidade.ItemsSource = particularidades.DefaultView;

                sqlite.Disconnect();
                sqlite = null;
            }
        }


    }
}
