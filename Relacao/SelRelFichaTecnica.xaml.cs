using CrystalDecisions.CrystalReports.Engine;
using Relacao.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for SelRelFichaTecnica.xaml
    /// </summary>
    public partial class SelRelFichaTecnica : Window
    {
        public SelRelFichaTecnica()
        {
            InitializeComponent();
        }

        private void Buscar_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (txtReferencia.Text.Trim().Count() > 0)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void Buscar_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            BuscarExecuted();
        }

        private void BuscarExecuted()
        {
            SQLite sqlite = new SQLite();
            Produto produto = new Produto();

            string referencia = txtReferencia.Text.Trim();
            int produtoID = sqlite.GetIDByReferencia(referencia);

            if (produtoID > 0)
            {
                produto = sqlite.GetProdutoByID(produtoID);

                txtRefConfirmada.Text = produto.Referencia;
                txtDescricao.Text = produto.Descricao;

                txtQuantidade.Focus();
            }
            else
            {
                MessageBox.Show("Não existe um produto cadastrado com a referência informada",
                "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);

                txtReferencia.Focus();
            }
        }

        private void Confirm_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (txtRefConfirmada.Text.Trim() != "" && txtDescricao.Text.Trim() != "")
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
            string reportFile;
            ReportDocument relatorio = new ReportDocument();
            WindowCrystalReports formulario = new WindowCrystalReports();
            Dictionary<string, string> parametros = new Dictionary<string, string>(); ;

            string referencia = txtRefConfirmada.Text;
            string quantidade = txtQuantidade.Value.ToString();
            string observacoes = txtObservacoes.Text;

            bool desmembrada = checkDesmembrada.IsChecked.Value;

            if (desmembrada)
                reportFile = "RelFichaTecnica Desmembrada.rpt";
            else
                reportFile = "RelFichaTecnica.rpt";

            parametros.Add("Referencia", referencia);
            parametros.Add("Quantidade", quantidade);
            parametros.Add("Observacoes", observacoes);
            parametros.Add("Unidade", radioMilimetro.IsChecked == true ? "Milimetro" : "Centimetro");

            formulario.Titulo = "RELAÇÃO DE PEÇAS À PRODUZIR";

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

        private void txtReferencia_GotFocus(object sender, RoutedEventArgs e)
        {
            btnBuscarProduto.IsDefault = true;
            btnConfirmar.IsDefault = false;
            txtReferencia.SelectAll();
        }

        private void txtReferencia_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtReferencia.Text.Trim() != "")
            {
                int cursorPos = txtReferencia.SelectionStart;

                txtReferencia.Text = txtReferencia.Text.ToUpper();
                txtReferencia.SelectionStart = cursorPos;
                txtReferencia.SelectionLength = 0;
            }
        }

        private void txtQuantidade_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !OnlyNumeric(e.Text);
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtReferencia.Focus();
        }

        private void txtReferencia_LostFocus(object sender, RoutedEventArgs e)
        {
            txtReferencia.Text = txtReferencia.Text.ToUpper();
            btnBuscarProduto.IsDefault = false;
            btnConfirmar.IsDefault = true;
        }

        public static bool OnlyNumeric(string text)
        {
            Regex regex = new Regex("[^0-9]+");

            return !regex.IsMatch(text);
        }

        private void btnInserirProduto_Click(object sender, RoutedEventArgs e)
        {
            CadProduto formulario = new CadProduto();
            formulario.ShowDialog();

            if (formulario.Produto != null)
            {
                txtReferencia.Text = formulario.Produto.Referencia;

                BuscarExecuted();
            }
            else
            {
                txtReferencia.Focus();
            }

            GC.Collect();
        }

        private void txtObservacoes_GotFocus(object sender, RoutedEventArgs e)
        {
            txtObservacoes.SelectAll();
        }

        private void txtObservacoes_LostFocus(object sender, RoutedEventArgs e)
        {
            txtObservacoes.Text = txtObservacoes.Text.ToUpper();
        }

        private void txtObservacoes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtObservacoes.Text.Trim() != "")
            {
                int cursorPos = txtObservacoes.SelectionStart;

                txtObservacoes.Text = txtObservacoes.Text.ToUpper();
                txtObservacoes.SelectionStart = cursorPos;
                txtObservacoes.SelectionLength = 0;
            }
        }

    }
}
