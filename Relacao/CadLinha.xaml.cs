using Relacao.Classes;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for CadLinha.xaml
    /// </summary>
    public partial class CadLinha : Window
    {
        private ObsCollection<Linha> linhasList = new ObsCollection<Linha>();
        private string descricaoOriginal;

        public CadLinha()
        {
            InitializeComponent();
        }

        private void txtDescricao_GotFocus(object sender, RoutedEventArgs e)
        {
            txtDescricao.SelectAll();
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
            Linha linha = new Linha();

            linha.Descricao = txtDescricao.Text.Trim().ToUpper();

            if (txtBtnInserir.Text.Equals("Inserir"))
            {
                if (!sqlite.ExistLinha(linha.Descricao))
                {
                    InsertLinha(linha);
                }
                else
                {
                    MessageBox.Show("A Linha Informada Já Existe no Cadastro",
                    "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (txtBtnInserir.Text.Equals("Salvar"))
            {
                string descricaoAlterada = linha.Descricao;

                linha.ID = ((Linha)gridDados.SelectedItem).ID;

                if (descricaoAlterada != descricaoOriginal)
                {
                    if (!sqlite.ExistLinha(linha.Descricao))
                    {
                        UpdateLinha(linha);
                    }
                    else
                    {
                        MessageBox.Show("A Linha Informada Já Existe no Cadastro",
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    UpdateLinha(linha);
                }
            }

            txtDescricao.Focus();
        }

        private void UpdateLinha(Linha linha)
        {
            SQLite sqlite = new SQLite();

            string query = "UPDATE LINHA SET DESCRICAO='" + linha.Descricao + "' WHERE ID=" + linha.ID;

            if (sqlite.Connect())
            {
                if (sqlite.UpdateQuery(query))
                {
                    gridDados.IsEnabled = true;
                    ChangeItemValues(linhasList, linha);
                    gridDados.Items.Refresh();
                    gridDados.ScrollIntoView(linha);

                    txtBtnInserir.Text = "Inserir";
                    txtDescricao.Clear();
                }

                sqlite.Disconnect();
                sqlite = null;
            }

        }

        private void InsertLinha(Linha linha)
        {
            SQLite sqlite = new SQLite();

            string query = "INSERT INTO LINHA (DESCRICAO) VALUES ('" + linha.Descricao + "')";

            if (sqlite.Connect())
            {
                linha.ID = sqlite.InsertQuery(query, "LINHA");

                linhasList.Add(linha);
                linhasList.UpdateCollection();
                gridDados.Items.Refresh();
                gridDados.ScrollIntoView(linha);

                txtDescricao.Clear();

                sqlite.Disconnect();
                sqlite = null;
            }

        }

        private void ChangeItemValues(ObsCollection<Linha> linhasList, Linha linha)
        {
            foreach (Linha item in linhasList)
            {
                if (item.ID.Equals(linha.ID))
                {
                    item.Descricao = linha.Descricao;
                }
            }
        }

        private void menuAlterar_Click(object sender, RoutedEventArgs e)
        {
            Linha linha = new Linha();
            linha = (Linha)gridDados.SelectedItem;

            txtBtnInserir.Text = "Salvar";

            txtDescricao.Text = linha.Descricao;

            gridDados.IsEnabled = false;

            descricaoOriginal = linha.Descricao;

            txtDescricao.Focus();
        }

        private void menuExcluir_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            Linha linha = new Linha();
            string query;

            linha = (Linha)gridDados.SelectedItem;
            query = "DELETE FROM LINHA WHERE ID=" + linha.ID;

            if (MessageBox.Show("Confirma exclusão?\n" + linha.ID.ToString() + " - " + linha.Descricao,
                "Confirmação de Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (sqlite.Connect())
                {
                    if (sqlite.DeleteQuery(query))
                    {
                        linhasList.Remove(linha);
                        gridDados.Items.Refresh();

                        txtDescricao.Clear();
                    }

                    sqlite.Disconnect();
                    sqlite = null;
                }

                txtDescricao.Focus();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable table = new DataTable();
            string query = "SELECT ID,DESCRICAO FROM LINHA ORDER BY ID";

            if (sqlite.Connect())
            {
                table = sqlite.GetTable(query);

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        Linha linha = new Linha();

                        linha.ID = Convert.ToInt64(row[0]);
                        linha.Descricao = Convert.ToString(row[1]);

                        linhasList.Add(linha);
                    }
                }

                gridDados.ItemsSource = linhasList;

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
