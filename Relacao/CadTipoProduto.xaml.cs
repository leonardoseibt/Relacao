using Relacao.Classes;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for CadTipoProduto.xaml
    /// </summary>
    public partial class CadTipoProduto : Window
    {
        private ObsCollection<TipoProduto> tiposprodutoList = new ObsCollection<TipoProduto>();
        private string descricaoOriginal;

        public CadTipoProduto()
        {
            InitializeComponent();
        }

        private void menuAlterar_Click(object sender, RoutedEventArgs e)
        {
            TipoProduto tipo = new TipoProduto();
            tipo = (TipoProduto)gridDados.SelectedItem;

            txtBtnInserir.Text = "Salvar";

            txtDescricao.Text = tipo.Descricao;

            gridDados.IsEnabled = false;

            descricaoOriginal = tipo.Descricao;

            txtDescricao.Focus();
        }

        private void menuExcluir_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            TipoProduto tipo = new TipoProduto();
            string query;

            tipo = (TipoProduto)gridDados.SelectedItem;
            query = "DELETE FROM TIPOPRODUTO WHERE ID=" + tipo.ID;

            if (MessageBox.Show("Confirma exclusão?\n" + tipo.ID.ToString() + " - " + tipo.Descricao,
                "Confirmação de Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (sqlite.Connect())
                {
                    if (sqlite.DeleteQuery(query))
                    {
                        tiposprodutoList.Remove(tipo);
                        gridDados.Items.Refresh();

                        txtDescricao.Clear();
                    }

                    sqlite.Disconnect();
                    sqlite = null;
                }

                txtDescricao.Focus();
            }
        }

        private void Insert_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!txtDescricao.Text.Trim().Equals(""))
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void Insert_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            TipoProduto tipo = new TipoProduto();

            tipo.Descricao = txtDescricao.Text.Trim().ToUpper();

            if (txtBtnInserir.Text.Equals("Inserir"))
            {
                if (!sqlite.ExistTipoProduto(tipo.Descricao))
                {
                    InsertTipoProduto(tipo);
                }
                else
                {
                    MessageBox.Show("O Tipo de Produto Informado Já Existe no Cadastro",
                    "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (txtBtnInserir.Text.Equals("Salvar"))
            {
                string descricaoAlterada = tipo.Descricao;

                tipo.ID = ((TipoProduto)gridDados.SelectedItem).ID;

                if (descricaoAlterada != descricaoOriginal)
                {
                    if (!sqlite.ExistTipoProduto(tipo.Descricao))
                    {
                        UpdateTipoProduto(tipo);
                    }
                    else
                    {
                        MessageBox.Show("O Tipo de Produto Informado Já Existe no Cadastro",
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    UpdateTipoProduto(tipo);
                }
            }

            txtDescricao.Focus();
        }

        private void UpdateTipoProduto(TipoProduto tipo)
        {
            SQLite sqlite = new SQLite();

            string query = "UPDATE TIPOPRODUTO SET DESCRICAO='" + tipo.Descricao + "' WHERE ID=" + tipo.ID;

            if (sqlite.Connect())
            {
                if (sqlite.UpdateQuery(query))
                {
                    gridDados.IsEnabled = true;
                    ChangeItemValues(tiposprodutoList, tipo);
                    gridDados.Items.Refresh();
                    gridDados.ScrollIntoView(tipo);

                    txtBtnInserir.Text = "Inserir";
                    txtDescricao.Clear();
                }

                sqlite.Disconnect();
                sqlite = null;
            }
        }

        private void InsertTipoProduto(TipoProduto tipo)
        {
            SQLite sqlite = new SQLite();

            string query = "INSERT INTO TIPOPRODUTO (DESCRICAO) VALUES ('" + tipo.Descricao + "')";

            if (sqlite.Connect())
            {
                tipo.ID = sqlite.InsertQuery(query, "TIPOPRODUTO");

                tiposprodutoList.Add(tipo);
                tiposprodutoList.UpdateCollection();
                gridDados.Items.Refresh();
                gridDados.ScrollIntoView(tipo);

                txtDescricao.Clear();

                sqlite.Disconnect();
                sqlite = null;
            }
        }

        private void ChangeItemValues(ObsCollection<TipoProduto> tiposprodutoList, TipoProduto tipo)
        {
            foreach (TipoProduto item in tiposprodutoList)
            {
                if (item.ID.Equals(tipo.ID))
                {
                    item.Descricao = tipo.Descricao;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable table = new DataTable();
            string query = "SELECT ID,DESCRICAO FROM TIPOPRODUTO ORDER BY ID";

            if (sqlite.Connect())
            {
                table = sqlite.GetTable(query);

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        TipoProduto tipo = new TipoProduto();

                        tipo.ID = Convert.ToInt64(row[0]);
                        tipo.Descricao = Convert.ToString(row[1]);

                        tiposprodutoList.Add(tipo);
                    }
                }

                gridDados.ItemsSource = tiposprodutoList;

                sqlite.Disconnect();
                sqlite = null;
            }

            txtDescricao.Focus();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                gridDados.IsEnabled = true;

                txtBtnInserir.Text = "Inserir";
                txtDescricao.Clear();

                txtDescricao.Focus();
            }
        }

        private void txtDescricao_GotFocus(object sender, RoutedEventArgs e)
        {
            txtDescricao.SelectAll();
        }

        private void txtDescricao_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtDescricao.Text.Trim() != "")
            {
                int cursorPos = txtDescricao.SelectionStart;

                txtDescricao.Text = txtDescricao.Text.ToUpper();
                txtDescricao.SelectionStart = cursorPos;
                txtDescricao.SelectionLength = 0;
            }
        }

    }
}
