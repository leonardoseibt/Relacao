using CrystalDecisions.CrystalReports.Engine;
using Relacao.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for SelRelFichaTecnicaAgrupada.xaml
    /// </summary>
    public partial class SelRelFichaTecnicaAgrupada : Window
    {
        ObsCollection<FichaTecnicaAgrupada> itensFichaTecnica = new ObsCollection<FichaTecnicaAgrupada>();

        public SelRelFichaTecnicaAgrupada()
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
            BuscarProdutoExecuted();
        }

        private void BuscarProdutoExecuted()
        {
            SQLite sqlite = new SQLite();
            Produto produto = new Produto();

            string referencia = txtReferencia.Text.Trim();
            long produtoID = sqlite.GetIDByReferencia(referencia);

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

        private void AgrupaProduto_CanExecute(object sender, CanExecuteRoutedEventArgs e)
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

        private void AgrupaProduto_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            FichaTecnicaAgrupada fichatecnica = new FichaTecnicaAgrupada();

            string query;

            fichatecnica.Relatorio.ID = Convert.ToInt64(txtRelConfirmado.Text);
            fichatecnica.Relatorio.Descricao = txtRelDescricao.Text;
            fichatecnica.Produto.Referencia = txtRefConfirmada.Text;
            fichatecnica.Produto.Descricao = txtDescricao.Text;
            fichatecnica.Produto.ID = sqlite.GetIDByReferencia(fichatecnica.Produto.Referencia);
            fichatecnica.Quantidade = Convert.ToInt32(txtQuantidade.Value);

            if (txtBtnInserir.Text.Equals("Inserir"))
            {
                if (!sqlite.ExistProdutoNoRelatorio(fichatecnica))
                {
                    query = "INSERT INTO FICHATECNICAAGRUPADA (" +
                    "IDRELATORIO," +
                    "IDPRODUTO," +
                    "QUANTIDADE) VALUES (" +
                    fichatecnica.Relatorio.ID.ToString() + "," +
                    fichatecnica.Produto.ID.ToString() + "," +
                    fichatecnica.Quantidade.ToString().Replace(',', '.') + ")";

                    if (sqlite.Connect())
                    {
                        sqlite.InsertQuery(query, "FICHATECNICAAGRUPADA");

                        itensFichaTecnica.Add(fichatecnica);
                        itensFichaTecnica.UpdateCollection();
                        gridDados.Items.Refresh();
                        gridDados.ScrollIntoView(fichatecnica);

                        sqlite.Disconnect();
                        sqlite = null;
                    }
                }
                else
                {
                    MessageBox.Show("O Produto Informado Já Existe no Cadastro",
                    "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (txtBtnInserir.Text.Equals("Salvar"))
            {
                query = "UPDATE FICHATECNICAAGRUPADA SET " +
                    "QUANTIDADE=" + fichatecnica.Quantidade.ToString().Replace(',', '.') + " " +
                    "WHERE IDRELATORIO=" + fichatecnica.Relatorio.ID.ToString() + " AND " +
                    "IDPRODUTO=" + fichatecnica.Produto.ID.ToString();

                if (sqlite.Connect())
                {
                    if (sqlite.UpdateQuery(query))
                    {
                        gridDados.IsEnabled = true;
                        groupProduto.IsEnabled = true;
                        groupQuantidade.IsEnabled = true;
                        ChangeItemValues(itensFichaTecnica, fichatecnica);
                        gridDados.Items.Refresh();
                        gridDados.ScrollIntoView(fichatecnica);

                        txtBtnInserir.Text = "Inserir";

                        txtRefConfirmada.Clear();
                        txtDescricao.Clear();
                        txtQuantidade.Value = txtQuantidade.DefaultValue;
                    }

                    sqlite.Disconnect();
                    sqlite = null;
                }
            }

            txtReferencia.Focus();
        }

        private void ChangeItemValues(ObsCollection<FichaTecnicaAgrupada> itensFichaTecnica, FichaTecnicaAgrupada fichatecnica)
        {
            foreach (FichaTecnicaAgrupada item in itensFichaTecnica)
            {
                if (item.Produto.Referencia.Equals(fichatecnica.Produto.Referencia))
                {
                    item.Quantidade = fichatecnica.Quantidade;
                }
            }
        }

        private void ImprimeSelecaoRelFichaTecnicaAgrupada_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (itensFichaTecnica.Count > 0)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void ImprimeSelecaoRelFichaTecnicaAgrupada_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();

            if (sqlite.Connect())
            { 
                //sqlite.DeleteQueryConnect("DELETE FROM RELATORIOFICHATECNICAAGRUPADA");
                sqlite.DeleteQuery("DELETE FROM RELATORIOFICHATECNICAAGRUPADA"); // Inserida

                string path;
                string reportFile;
                ReportDocument relatorio = new ReportDocument();
                WindowCrystalReports formulario = new WindowCrystalReports();
                Dictionary<string, string> parametros = new Dictionary<string, string>(); ;

                string unidade = radioCentimetro.IsChecked == true ? "Centimetro" : "Milimetro";

                long relatorioID = Convert.ToInt64(txtRelConfirmado.Text);

                string queryFichaTecnica;
                string query;
                string querySearch;

                long produtoID;
                int qtdLote;

                long materiaprimaID;
                string materiaprimaUN;
                string agrupamento;
                decimal materiaprimaPerda;
                decimal qtdPorMovel;

                decimal comprimento;
                decimal largura;
                decimal espessura;

                decimal metragem;

                string medidas;

                decimal qtdTotal = 0;

                foreach (FichaTecnicaAgrupada item in itensFichaTecnica)
                {
                    DataTable table = new DataTable();

                    produtoID = item.Produto.ID;
                    qtdLote = item.Quantidade;

                    queryFichaTecnica =
                        "SELECT FICHATECNICA.QUANTIDADE, " +
                        "       COMPONENTE.COMPRIMENTO, " +
                        "       COMPONENTE.LARGURA, " +
                        "       COMPONENTE.ESPESSURA, " +
                        "       COMPONENTE.IDMATERIAPRIMA IDMP, " +
                        "       TIPOMATERIAPRIMA.UNIDADE, " +
                        "       MATERIAPRIMA.PERDA, " +
                        "       FICHATECNICA.AGRUPAMENTO " +
                        "  FROM FICHATECNICA, " +
                        "       TIPOCOMPONENTE, " +
                        "       COMPONENTE, " +
                        "       MATERIAPRIMA, " +
                        "       TIPOMATERIAPRIMA " +
                        " WHERE COMPONENTE.ID = FICHATECNICA.IDCOMPONENTE AND " +
                        "       TIPOCOMPONENTE.ID = COMPONENTE.IDTIPOCOMPONENTE AND " +
                        "       MATERIAPRIMA.ID = COMPONENTE.IDMATERIAPRIMA AND " +
                        "       TIPOMATERIAPRIMA.ID = MATERIAPRIMA.IDTIPOMATERIAPRIMA AND " +
                        "       FICHATECNICA.IDPRODUTO=" + produtoID.ToString();

                    //table = sqlite.GetTableConnect(queryFichaTecnica);
                    table = sqlite.GetTable(queryFichaTecnica); // Inserida

                    if (table.Rows.Count > 0)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            //sqlite = new SQLite();

                            qtdPorMovel = Convert.ToDecimal(row[0]);
                            comprimento = Convert.ToDecimal(row[1]);
                            largura = Convert.ToDecimal(row[2]);
                            espessura = Convert.ToDecimal(row[3]);
                            materiaprimaID = Convert.ToInt64(row[4]);
                            materiaprimaUN = row[5].ToString();
                            materiaprimaPerda = Convert.ToDecimal(row[6]);
                            agrupamento = row[7].ToString();

                            qtdTotal = qtdPorMovel * qtdLote;

                            if (unidade.Equals("Centimetro"))
                            {
                                medidas =
                                    (comprimento / 10).ToString("N1").Replace('.', ',') + " X " +
                                    (largura / 10).ToString("N1").Replace('.', ',') + " X " +
                                    (espessura / 10).ToString("N1").Replace('.', ',');
                            }
                            else
                            {
                                medidas =
                                    comprimento.ToString("N0") + " X " +
                                    largura.ToString("N0") + " X " +
                                    espessura.ToString("N0");
                            }

                            if (materiaprimaUN.Equals("M3"))
                                metragem = (comprimento * largura * espessura / 1000000000) * qtdTotal;
                            else if (materiaprimaUN.Equals("M2"))
                                metragem = (comprimento * largura / 1000000) * qtdTotal;
                            else if (materiaprimaUN.Equals("MT"))
                                metragem = (comprimento / 1000) * qtdTotal;
                            else
                                metragem = comprimento * qtdTotal;

                            if (checkPerda.IsChecked == true)
                                metragem += metragem * materiaprimaPerda / 100;

                            querySearch =
                                "SELECT IDRELATORIO, " +
                                "       IDMATERIAPRIMA, " +
                                "       QTDPECAS, " +
                                "       MEDIDAS, " +
                                "       METRAGEM, " +
                                "       AGRUPAMENTO " +
                                "  FROM RELATORIOFICHATECNICAAGRUPADA " +
                                " WHERE IDRELATORIO=" + relatorioID.ToString() + " AND " +
                                "       IDMATERIAPRIMA=" + materiaprimaID.ToString() + " AND " +
                                "       MEDIDAS='" + medidas + "' AND " +
                                "       AGRUPAMENTO='" + agrupamento + "'";

                            RelatorioFichaTecnicaAgrupada ficha = new RelatorioFichaTecnicaAgrupada();
                            //ficha = sqlite.GetRelatorioFichaAgrupadaConnect(querySearch);
                            ficha = sqlite.GetRelatorioFichaAgrupada(querySearch); // Inserida

                            if (ficha.Relatorio.ID == 0)
                            {
                                query =
                                    "INSERT INTO RELATORIOFICHATECNICAAGRUPADA ( " +
                                    "       IDRELATORIO, " +
                                    "       IDMATERIAPRIMA, " +
                                    "       QTDPECAS, " +
                                    "       MEDIDAS, " +
                                    "       METRAGEM, " +
                                    "       AGRUPAMENTO) " +
                                    "VALUES ( " +
                                    relatorioID.ToString() + ", " +
                                    materiaprimaID.ToString() + ", " +
                                    qtdTotal.ToString().Replace(',', '.') + ", '" +
                                    medidas + "', " +
                                    metragem.ToString().Replace(',', '.') + ", '" +
                                    agrupamento + "')";
                            }
                            else
                            {
                                qtdTotal += ficha.Quantidade;
                                metragem += ficha.Metragem;

                                query =
                                    "UPDATE RELATORIOFICHATECNICAAGRUPADA SET " +
                                    "       QTDPECAS = " + qtdTotal.ToString().Replace(',', '.') + ", " +
                                    "       METRAGEM = " + metragem.ToString().Replace(',', '.') + ", " +
                                    "       AGRUPAMENTO = '" + agrupamento + "' " +
                                    " WHERE IDRELATORIO=" + relatorioID.ToString() + " AND " +
                                    "       IDMATERIAPRIMA=" + materiaprimaID.ToString() + " AND " +
                                    "       MEDIDAS='" + medidas + "' AND " +
                                    "       AGRUPAMENTO='" + agrupamento + "'";
                            }

                            //if (sqlite.Connect())
                            //{
                            sqlite.UpdateQuery(query);

                            //    sqlite.Disconnect();
                            //}
                        }
                    }
                }

                sqlite.Disconnect(); // Inserida
                sqlite = null;

                formulario.Titulo = "RELAÇÃO DE PEÇAS AGRUPADAS POR MEDIDA";

                if (checkDesmembrada.IsChecked == true)
                {
                    reportFile = "RelProdutosAgrupadosDesmembrados.rpt";
                }
                else
                {
                    reportFile = "RelProdutosAgrupados.rpt";
                }

                parametros.Add("Relatorio", relatorioID.ToString());
                parametros.Add("Autor", ConfigurationManager.AppSettings["NomeEmpresa"]);
                parametros.Add("Titulo", formulario.Titulo);

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

                NovoRelatorio();
            } 
        }
        
        private void txtReferencia_GotFocus(object sender, RoutedEventArgs e)
        {
            btnBuscarProduto.IsDefault = true;
            btnBuscarRelatorio.IsDefault = false;
            btnImprimir.IsDefault = false;
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

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menuAlterar_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            Produto produto = new Produto();

            FichaTecnicaAgrupada ficha = new FichaTecnicaAgrupada();
            ficha = (FichaTecnicaAgrupada)gridDados.SelectedItem;

            txtBtnInserir.Text = "Salvar";

            txtQuantidade.Value = ficha.Quantidade;
            txtReferencia.Clear();
            groupProduto.IsEnabled = false;
            gridDados.IsEnabled = false;

            long produtoID = sqlite.GetIDByReferencia(ficha.Produto.Referencia);

            if (produtoID > 0)
            {
                produto = sqlite.GetProdutoByID(produtoID);

                txtRefConfirmada.Text = produto.Referencia;
                txtDescricao.Text = produto.Descricao;
            }
            else
            {
                MessageBox.Show("Não existe um produto cadastrado com a referência informada",
                "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            txtQuantidade.Focus();
        }

        private void menuExcluir_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            FichaTecnicaAgrupada ficha = new FichaTecnicaAgrupada();
            string query;

            ficha = (FichaTecnicaAgrupada)gridDados.SelectedItem;
            query = "DELETE FROM FICHATECNICAAGRUPADA WHERE " +
                "IDRELATORIO=" + ficha.Relatorio.ID.ToString() + " AND " +
                "IDPRODUTO=" + ficha.Produto.ID.ToString();

            if (MessageBox.Show("Confirma exclusão?\n" + ficha.Produto.Descricao,
                "Confirmação de Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (sqlite.Connect())
                {
                    if (sqlite.DeleteQuery(query))
                    {
                        itensFichaTecnica.Remove(ficha);
                        gridDados.Items.Refresh();
                    }

                    sqlite.Disconnect();
                    sqlite = null;
                }

                txtReferencia.Focus();
            }
        }

        private void txtQuantidade_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !OnlyNumeric(e.Text);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gridDados.ItemsSource = itensFichaTecnica;

            txtRelatorio.Focus();
        }

        public static bool OnlyNumeric(string text)
        {
            Regex regex = new Regex("[^0-9]+");

            return !regex.IsMatch(text);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                NovoRelatorio();
            }
        }

        private void NovoRelatorio()
        {
            gridDados.IsEnabled = true;
            gridDados.ItemsSource = null;

            txtBtnInserir.Text = "Inserir";

            txtRelatorio.Clear();
            txtRelConfirmado.Clear();
            txtRelDescricao.Clear();
            txtReferencia.Clear();
            groupRelatorio.IsEnabled = true;
            groupProduto.IsEnabled = false;
            groupQuantidade.IsEnabled = false;
            txtRefConfirmada.Clear();
            txtDescricao.Clear();
            txtQuantidade.Value = txtQuantidade.DefaultValue;

            txtRelatorio.Focus();
        }

        private void txtRelatorio_GotFocus(object sender, RoutedEventArgs e)
        {
            btnBuscarRelatorio.IsDefault = true;
            btnBuscarProduto.IsDefault = false;
            btnImprimir.IsDefault = false;
            txtRelatorio.SelectAll();
        }

        private void txtRelatorio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !OnlyNumeric(e.Text);
        }

        private void BuscarRelatorio_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (txtRelatorio.Text.Trim().Count() > 0)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void BuscarRelatorio_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            BuscarRelatorioExecuted();
        }

        private void BuscarRelatorioExecuted()
        {
            itensFichaTecnica.Clear();
            SQLite sqlite = new SQLite();
            DataTable table = new DataTable();
            Relatorio relatorio = new Relatorio();

            string query;

            long relatorioID = Convert.ToInt64(txtRelatorio.Text.Trim());

            relatorio = sqlite.GetRelatorioByID(relatorioID);

            if (relatorio.ID > 0)
            {
                txtRelConfirmado.Text = relatorio.ID.ToString();
                txtRelDescricao.Text = relatorio.Descricao;

                query =
                    "SELECT FICHATECNICAAGRUPADA.IDRELATORIO, " +
                    "       RELATORIO.DESCRICAO RELATORIO, " +
                    "       FICHATECNICAAGRUPADA.IDPRODUTO, " +
                    "       PRODUTO.REFERENCIA, " +
                    "       PRODUTO.DESCRICAO, " +
                    "       FICHATECNICAAGRUPADA.QUANTIDADE " +
                    "  FROM FICHATECNICAAGRUPADA, " +
                    "       PRODUTO, " +
                    "       RELATORIO " +
                    " WHERE PRODUTO.ID = FICHATECNICAAGRUPADA.IDPRODUTO AND " +
                    "       RELATORIO.ID = FICHATECNICAAGRUPADA.IDRELATORIO AND " +
                    "       FICHATECNICAAGRUPADA.IDRELATORIO = " + relatorio.ID.ToString();

                if (sqlite.Connect())
                {
                    table = sqlite.GetTable(query);

                    if (table.Rows.Count > 0)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            FichaTecnicaAgrupada fichatecnica = new FichaTecnicaAgrupada();

                            fichatecnica.Relatorio.ID = (long)row[0];
                            fichatecnica.Relatorio.Descricao = row[1].ToString();
                            fichatecnica.Produto.ID = (long)row[2];
                            fichatecnica.Produto.Referencia = row[3].ToString();
                            fichatecnica.Produto.Descricao = row[4].ToString();
                            fichatecnica.Quantidade = Convert.ToInt32(row[5]);

                            itensFichaTecnica.Add(fichatecnica);
                        }
                    }

                    gridDados.ItemsSource = itensFichaTecnica;

                    sqlite.Disconnect();
                    sqlite = null;

                    groupRelatorio.IsEnabled = false;
                    groupProduto.IsEnabled = true;
                    groupQuantidade.IsEnabled = true;

                }

                txtReferencia.Focus();
            }
            else
            {
                MessageBox.Show("Não existe um relatório cadastrado com a ID informada",
                "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);

                txtRelatorio.Focus();
            }
        }

        private void btnInserirRelatorio_Click(object sender, RoutedEventArgs e)
        {
            CadRelatorio formulario = new CadRelatorio();
            formulario.ShowDialog();

            if (formulario.Relatorio != null)
            {
                txtRelatorio.Text = formulario.Relatorio.ID.ToString();

                BuscarRelatorioExecuted();
            }
            else
            {
                txtRelatorio.Focus();
            }

            GC.Collect();
        }

        private void btnInserirProduto_Click(object sender, RoutedEventArgs e)
        {
            CadProduto formulario = new CadProduto();
            formulario.ShowDialog();

            if (formulario.Produto != null)
            {
                txtReferencia.Text = formulario.Produto.Referencia;

                BuscarProdutoExecuted();
            }
            else
            {
                txtReferencia.Focus();
            }

            GC.Collect();
        }

    }
}
