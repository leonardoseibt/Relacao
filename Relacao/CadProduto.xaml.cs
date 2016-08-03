using Relacao.Classes;
using System;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for CadProduto.xaml
    /// </summary>
    public partial class CadProduto : Window
    {
        internal Produto Produto { get; set; }

        private ObsCollection<Produto> produtosList = new ObsCollection<Produto>();
        private string descricaoOriginal;
        private string referenciaOriginal;

        public CadProduto()
        {
            InitializeComponent();
        }

        private void txtMedidas_GotFocus(object sender, RoutedEventArgs e)
        {
            txtMedidas.SelectAll();
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

            AtualizaDescricao();
        }

        private void txtReferencia_GotFocus(object sender, RoutedEventArgs e)
        {
            txtReferencia.SelectAll();
        }

        private void comboTipoProduto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!comboTipoProduto.SelectedIndex.Equals(-1))
            {
                DataRowView tipoRow = (DataRowView)comboTipoProduto.SelectedItem;

                SQLite sqlite = new SQLite();
                DataTable particularidades = new DataTable();

                string queryParticularidades =
                    "SELECT ID, " +
                    "       DESCRICAO " +
                    "  FROM PARTICULARIDADE " +
                    " WHERE IDTIPOPRODUTO = " + tipoRow.Row[0].ToString();

                if (sqlite.Connect())
                {
                    particularidades = sqlite.GetTable(queryParticularidades);
                    comboParticularidade.ItemsSource = particularidades.DefaultView;

                    sqlite.Disconnect();
                    sqlite = null;
                }

                btnFiltrar.IsEnabled = true;

                txtReferencia.Focus();
            }

            AtualizaDescricao();
        }

        private void AtualizaDescricao()
        {
            DataRowView tipoRow = (DataRowView)comboTipoProduto.SelectedItem;
            DataRowView particularidadeRow = (DataRowView)comboParticularidade.SelectedItem;
            DataRowView linhaRow = (DataRowView)comboLinha.SelectedItem;

            string tipo = tipoRow != null ? tipoRow.Row[1].ToString() : "";
            string particularidade = particularidadeRow != null ? particularidadeRow.Row[1].ToString() : "";
            string linha = linhaRow != null ? linhaRow.Row[1].ToString() : "";
            string medidas = txtMedidas.Text.ToUpper();

            string descricao = tipo + " " + linha + " " + particularidade + " " + medidas;

            ThreadPool.QueueUserWorkItem((o) =>
            {
                Dispatcher.Invoke((Action)(() => txtDescricao.Text = descricao));
            });
        }

        private void comboParticularidade_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnFiltrar.IsEnabled = true;

            AtualizaDescricao();
        }

        private void btnInserirPART_Click(object sender, RoutedEventArgs e)
        {
            CadParticularidade formulario = new CadParticularidade();
            formulario.ShowDialog();

            SQLite sqlite = new SQLite();
            DataTable tipos = new DataTable();

            string queryTipos = "SELECT ID,DESCRICAO FROM TIPOPRODUTO ORDER BY DESCRICAO";

            if (sqlite.Connect())
            {
                tipos = sqlite.GetTable(queryTipos);
                comboTipoProduto.ItemsSource = tipos.DefaultView;

                sqlite.Disconnect();
                sqlite = null;
            }

            comboParticularidade.ItemsSource = null;
            txtReferencia.Focus();
        }

        private void comboLinha_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AtualizaDescricao();

            btnFiltrar.IsEnabled = true;

            txtReferencia.Focus();
        }

        private void btnInserirLINHA_Click(object sender, RoutedEventArgs e)
        {
            CadLinha formulario = new CadLinha();
            formulario.ShowDialog();

            SQLite sqlite = new SQLite();
            DataTable linhas = new DataTable();

            string queryLinhas = "SELECT ID,DESCRICAO FROM LINHA ORDER BY DESCRICAO";

            if (sqlite.Connect())
            {
                linhas = sqlite.GetTable(queryLinhas);
                comboLinha.ItemsSource = linhas.DefaultView;

                sqlite.Disconnect();
                sqlite = null;
            }
        }

        private void menuAlterar_Click(object sender, RoutedEventArgs e)
        {
            Produto produto = new Produto();
            produto = (Produto)gridDados.SelectedItem;

            txtBtnInserir.Text = "Salvar";

            comboTipoProduto.SelectedValue = produto.Particularidade.Tipo.ID;
            comboParticularidade.SelectedValue = produto.Particularidade.ID;
            comboLinha.SelectedValue = produto.Linha.ID;
            txtReferencia.Text = produto.Referencia;
            txtMedidas.Text = produto.Medidas;
            txtDescricao.Text = produto.Descricao;
            txtObservacoes.Text = produto.Observacoes;

            gridDados.IsEnabled = false;

            descricaoOriginal = produto.Descricao;
            referenciaOriginal = produto.Referencia;

            txtReferencia.Focus();
        }

        private void menuExcluir_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            Produto produto = new Produto();
            string query;

            produto = (Produto)gridDados.SelectedItem;
            query = "DELETE FROM PRODUTO WHERE ID=" + produto.ID;

            if (MessageBox.Show("Confirma exclusão?\n" + produto.ID.ToString() + " - " + produto.Descricao,
                "Confirmação de Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (sqlite.Connect())
                {
                    if (sqlite.DeleteQuery(query))
                    {
                        produtosList.Remove(produto);
                        gridDados.Items.Refresh();

                        comboTipoProduto.SelectedIndex = -1;
                        comboParticularidade.SelectedIndex = -1;
                        comboLinha.SelectedIndex = -1;
                        txtReferencia.Clear();
                        txtMedidas.Clear();
                        txtDescricao.Clear();
                        txtObservacoes.Clear();
                    }

                    sqlite.Disconnect();
                    sqlite = null;
                }

                txtReferencia.Focus();
            }
        }

        private void txtObservacoes_GotFocus(object sender, RoutedEventArgs e)
        {
            txtObservacoes.SelectAll();
        }

        private void Insert_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            Produto produto = new Produto();

            long tipoID = Convert.ToInt64(comboTipoProduto.SelectedValue);
            long particularidadeID = Convert.ToInt64(comboParticularidade.SelectedValue);
            long linhaID = Convert.ToInt64(comboLinha.SelectedValue);

            produto.Particularidade = sqlite.GetParticularidade(particularidadeID);
            produto.Particularidade.Tipo = sqlite.GetTipoProduto(tipoID);
            produto.Linha = sqlite.GetLinha(linhaID);
            produto.Descricao = txtDescricao.Text.Trim().ToUpper();
            produto.Referencia = txtReferencia.Text.Trim().ToUpper();
            produto.Medidas = txtMedidas.Text.Trim().ToUpper();
            produto.Observacoes = txtObservacoes.Text.Trim().ToUpper();

            if (txtBtnInserir.Text.Equals("Inserir"))
            {
                if (!sqlite.ExistProduto(produto))
                {
                    InsertProduto(produto);
                }
                else
                {
                    MessageBox.Show("O Produto Informado Já Existe no Cadastro",
                    "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (txtBtnInserir.Text.Equals("Salvar"))
            {
                string descricaoAlterada = produto.Descricao;
                string referenciaAlterada = produto.Referencia;

                produto.ID = ((Produto)gridDados.SelectedItem).ID;

                if (descricaoAlterada != descricaoOriginal && referenciaAlterada == referenciaOriginal)
                {
                    //if (!sqlite.ExistProduto(produto))
                    //{
                    UpdateProduto(produto);
                    //}
                    //else
                    //{
                    //    MessageBox.Show("O Produto Informado Já Existe no Cadastro",
                    //    "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                    //}
                }
                else if (referenciaAlterada != referenciaOriginal)
                {
                    if (!sqlite.ExistReferenciaProduto(produto))
                    {
                        UpdateProduto(produto);
                    }
                    else
                    {
                        MessageBox.Show("A Referência Informada Já Existe no Cadastro",
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    UpdateProduto(produto);
                }
            }

            txtReferencia.Focus();
        }

        private void InsertProduto(Produto produto)
        {
            SQLite sqlite = new SQLite();

            string query = "INSERT INTO PRODUTO (" +
                "IDTIPOPRODUTO," +
                "IDPARTICULARIDADE," +
                "IDLINHA," +
                "DESCRICAO," +
                "REFERENCIA," +
                "MEDIDAS," +
                "OBSERVACOES) VALUES (" +
                produto.Particularidade.Tipo.ID.ToString() + "," +
                produto.Particularidade.ID.ToString() + "," +
                produto.Linha.ID.ToString() + ",'" +
                produto.Descricao + "','" +
                produto.Referencia + "','" +
                produto.Medidas + "','" +
                produto.Observacoes + "')";

            if (sqlite.Connect())
            {
                produto.ID = sqlite.InsertQuery(query, "PRODUTO");

                produtosList.Add(produto);
                produtosList.UpdateCollection();
                gridDados.Items.Refresh();
                gridDados.ScrollIntoView(produto);

                comboTipoProduto.SelectedIndex = -1;
                comboParticularidade.SelectedIndex = -1;
                comboLinha.SelectedIndex = -1;
                txtReferencia.Clear();
                txtMedidas.Clear();
                txtDescricao.Clear();
                txtObservacoes.Clear();

                sqlite.Disconnect();
                sqlite = null;
            }
        }

        private void UpdateProduto(Produto produto)
        {
            SQLite sqlite = new SQLite();
            string query;

            query =
                "UPDATE PRODUTO SET " +
                "IDTIPOPRODUTO=" + produto.Particularidade.Tipo.ID.ToString() + "," +
                "IDPARTICULARIDADE=" + produto.Particularidade.ID.ToString() + "," +
                "IDLINHA=" + produto.Linha.ID.ToString() + "," +
                "DESCRICAO='" + produto.Descricao + "'," +
                "REFERENCIA='" + produto.Referencia + "'," +
                "MEDIDAS='" + produto.Medidas + "'," +
                "OBSERVACOES='" + produto.Observacoes + "' " +
                "WHERE ID=" + produto.ID;

            if (sqlite.Connect())
            {
                if (sqlite.UpdateQuery(query))
                {
                    gridDados.IsEnabled = true;
                    ChangeItemValues(produtosList, produto);
                    gridDados.Items.Refresh();
                    gridDados.ScrollIntoView(produto);

                    txtBtnInserir.Text = "Inserir";

                    comboTipoProduto.SelectedIndex = -1;
                    comboParticularidade.SelectedIndex = -1;
                    comboLinha.SelectedIndex = -1;
                    txtReferencia.Clear();
                    txtMedidas.Clear();
                    txtDescricao.Clear();
                    txtObservacoes.Clear();
                }

                sqlite.Disconnect();
                sqlite = null;
            }
        }

        private void ChangeItemValues(ObsCollection<Produto> produtosList, Produto produto)
        {
            foreach (Produto item in produtosList)
            {
                if (item.ID.Equals(produto.ID))
                {
                    item.Particularidade = produto.Particularidade;
                    item.Linha = produto.Linha;
                    item.Referencia = produto.Referencia;
                    item.Medidas = produto.Medidas;
                    item.Descricao = produto.Descricao;
                    item.Observacoes = produto.Observacoes;
                }
            }
        }

        private void Insert_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!comboTipoProduto.SelectedIndex.Equals(-1) &&
                !comboParticularidade.SelectedIndex.Equals(-1) &&
                !comboLinha.SelectedIndex.Equals(-1) &&
                !txtReferencia.Text.Trim().Equals(""))
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable tipos = new DataTable();
            DataTable linhas = new DataTable();
            DataTable produtos = new DataTable();

            string queryTipos =
                "SELECT ID,DESCRICAO FROM TIPOPRODUTO ORDER BY DESCRICAO";
            string queryLinhas =
                "SELECT ID,DESCRICAO FROM LINHA ORDER BY DESCRICAO";
            string queryProdutos =
                "SELECT PRODUTO.ID, " +
                "       PRODUTO.IDTIPOPRODUTO IDTIPO, " +
                "       TIPOPRODUTO.DESCRICAO TIPO, " +
                "       PRODUTO.IDPARTICULARIDADE IDPARTIC, " +
                "       PARTICULARIDADE.DESCRICAO PARTIC, " +
                "       PRODUTO.IDLINHA, " +
                "       LINHA.DESCRICAO LINHA, " +
                "       PRODUTO.REFERENCIA, " +
                "       PRODUTO.DESCRICAO, " +
                "       PRODUTO.MEDIDAS, " +
                "       PRODUTO.OBSERVACOES " +
                "  FROM PRODUTO, " +
                "       TIPOPRODUTO, " +
                "       PARTICULARIDADE, " +
                "       LINHA " +
                " WHERE PRODUTO.IDTIPOPRODUTO = TIPOPRODUTO.ID AND " +
                "       PRODUTO.IDPARTICULARIDADE = PARTICULARIDADE.ID AND " +
                "       PRODUTO.IDLINHA = LINHA.ID";

            if (sqlite.Connect())
            {
                tipos = sqlite.GetTable(queryTipos);
                linhas = sqlite.GetTable(queryLinhas);
                produtos = sqlite.GetTable(queryProdutos);

                comboTipoProduto.ItemsSource = tipos.DefaultView;
                comboLinha.ItemsSource = linhas.DefaultView;

                if (produtos.Rows.Count > 0)
                {
                    foreach (DataRow row in produtos.Rows)
                    {
                        Produto produto = new Produto();

                        produto.ID = (long)row[0];
                        produto.Particularidade.Tipo.ID = (long)row[1];
                        produto.Particularidade.Tipo.Descricao = (string)row[2];
                        produto.Particularidade.ID = (long)row[3];
                        produto.Particularidade.Descricao = (string)row[4];
                        produto.Linha.ID = (long)row[5];
                        produto.Linha.Descricao = (string)row[6];
                        produto.Referencia = (string)row[7];
                        produto.Descricao = (string)row[8];
                        produto.Medidas = (string)row[9];
                        produto.Observacoes = Convert.ToString(row[10]);

                        produtosList.Add(produto);
                    }
                }

                gridDados.ItemsSource = produtosList;

                sqlite.Disconnect();
                sqlite = null;
            }

            txtReferencia.Focus();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                gridDados.IsEnabled = true;

                txtBtnInserir.Text = "Inserir";

                comboTipoProduto.SelectedIndex = -1;
                comboParticularidade.SelectedIndex = -1;
                comboLinha.SelectedIndex = -1;
                txtReferencia.Clear();
                txtMedidas.Clear();
                txtDescricao.Clear();
                txtObservacoes.Clear();

                txtReferencia.Focus();
            }
        }

        private void txtMedidas_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtMedidas.Text.Trim() != "")
            {
                int cursorPos = txtMedidas.SelectionStart;

                txtMedidas.Text = txtMedidas.Text.ToUpper();
                txtMedidas.SelectionStart = cursorPos;
                txtMedidas.SelectionLength = 0;
            }

            AtualizaDescricao();
        }

        private void btnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable produtos = new DataTable();

            string queryProdutos =
                "SELECT PRODUTO.ID, " +
                "       PRODUTO.IDTIPOPRODUTO IDTIPO, " +
                "       TIPOPRODUTO.DESCRICAO TIPO, " +
                "       PRODUTO.IDPARTICULARIDADE IDPARTIC, " +
                "       PARTICULARIDADE.DESCRICAO PARTIC, " +
                "       PRODUTO.IDLINHA, " +
                "       LINHA.DESCRICAO LINHA, " +
                "       PRODUTO.REFERENCIA, " +
                "       PRODUTO.DESCRICAO, " +
                "       PRODUTO.MEDIDAS, " +
                "       PRODUTO.OBSERVACOES " +
                "  FROM PRODUTO, " +
                "       TIPOPRODUTO, " +
                "       PARTICULARIDADE, " +
                "       LINHA " +
                " WHERE PRODUTO.IDTIPOPRODUTO = TIPOPRODUTO.ID AND " +
                "       PRODUTO.IDPARTICULARIDADE = PARTICULARIDADE.ID AND " +
                "       PRODUTO.IDLINHA = LINHA.ID ";

            int selTipoProduto = comboTipoProduto.SelectedIndex;
            int selParticularidade = comboParticularidade.SelectedIndex;
            int selLinha = comboLinha.SelectedIndex;

            if (selTipoProduto >= 0)
            {
                DataRowView result = (DataRowView)comboTipoProduto.SelectedItem;
                string tipoproduto = result.Row[0].ToString();

                queryProdutos += "AND PRODUTO.IDTIPOPRODUTO = " + tipoproduto + " ";
            }

            if (selParticularidade >= 0)
            {
                DataRowView result = (DataRowView)comboParticularidade.SelectedItem;
                string particularidade = result.Row[0].ToString();

                queryProdutos += "AND PRODUTO.IDPARTICULARIDADE = " + particularidade + " ";
            }

            if (selLinha >= 0)
            {
                DataRowView result = (DataRowView)comboLinha.SelectedItem;
                string linha = result.Row[0].ToString();

                queryProdutos += "AND PRODUTO.IDLINHA = " + linha + " ";
            }

            if (sqlite.Connect())
            {
                produtosList.Clear();

                produtos = sqlite.GetTable(queryProdutos);

                if (produtos.Rows.Count > 0)
                {
                    foreach (DataRow row in produtos.Rows)
                    {
                        Produto produto = new Produto();

                        produto.ID = (long)row[0];
                        produto.Particularidade.Tipo.ID = (long)row[1];
                        produto.Particularidade.Tipo.Descricao = (string)row[2];
                        produto.Particularidade.ID = (long)row[3];
                        produto.Particularidade.Descricao = (string)row[4];
                        produto.Linha.ID = (long)row[5];
                        produto.Linha.Descricao = (string)row[6];
                        produto.Referencia = (string)row[7];
                        produto.Descricao = (string)row[8];
                        produto.Medidas = (string)row[9];
                        produto.Observacoes = Convert.ToString(row[10]);

                        produtosList.Add(produto);
                    }
                }

                gridDados.ItemsSource = produtosList;

                sqlite.Disconnect();
                sqlite = null;
            }

            btnCancelarFiltro.IsEnabled = true;

            txtReferencia.Focus();
        }

        private void btnCancelarFiltro_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable produtos = new DataTable();

            string queryProdutos =
                "SELECT PRODUTO.ID, " +
                "       PRODUTO.IDTIPOPRODUTO IDTIPO, " +
                "       TIPOPRODUTO.DESCRICAO TIPO, " +
                "       PRODUTO.IDPARTICULARIDADE IDPARTIC, " +
                "       PARTICULARIDADE.DESCRICAO PARTIC, " +
                "       PRODUTO.IDLINHA, " +
                "       LINHA.DESCRICAO LINHA, " +
                "       PRODUTO.REFERENCIA, " +
                "       PRODUTO.DESCRICAO, " +
                "       PRODUTO.MEDIDAS, " +
                "       PRODUTO.OBSERVACOES " +
                "  FROM PRODUTO, " +
                "       TIPOPRODUTO, " +
                "       PARTICULARIDADE, " +
                "       LINHA " +
                " WHERE PRODUTO.IDTIPOPRODUTO = TIPOPRODUTO.ID AND " +
                "       PRODUTO.IDPARTICULARIDADE = PARTICULARIDADE.ID AND " +
                "       PRODUTO.IDLINHA = LINHA.ID";

            comboTipoProduto.SelectedIndex = -1;
            comboParticularidade.ItemsSource = null;
            comboLinha.SelectedIndex = -1;

            if (sqlite.Connect())
            {
                produtosList.Clear();

                produtos = sqlite.GetTable(queryProdutos);

                if (produtos.Rows.Count > 0)
                {
                    foreach (DataRow row in produtos.Rows)
                    {
                        Produto produto = new Produto();

                        produto.ID = (long)row[0];
                        produto.Particularidade.Tipo.ID = (long)row[1];
                        produto.Particularidade.Tipo.Descricao = (string)row[2];
                        produto.Particularidade.ID = (long)row[3];
                        produto.Particularidade.Descricao = (string)row[4];
                        produto.Linha.ID = (long)row[5];
                        produto.Linha.Descricao = (string)row[6];
                        produto.Referencia = (string)row[7];
                        produto.Descricao = (string)row[8];
                        produto.Medidas = (string)row[9];
                        produto.Observacoes = Convert.ToString(row[10]);

                        produtosList.Add(produto);
                    }
                }

                gridDados.ItemsSource = produtosList;

                sqlite.Disconnect();
                sqlite = null;
            }

            btnCancelarFiltro.IsEnabled = false;
            btnFiltrar.IsEnabled = false;

            txtReferencia.Focus();
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

        private void gridDados_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                DataGrid grid = sender as DataGrid;
                if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                {
                    DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;

                    this.Produto = (Produto)dgr.Item;

                    this.Close();
                }
            }
        }

    }
}
