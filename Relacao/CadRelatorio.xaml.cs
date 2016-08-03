using Relacao.Classes;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for CadRelatorio.xaml
    /// </summary>
    public partial class CadRelatorio : Window
    {
        //internal bool Seleciona { get; set; }
        internal Relatorio Relatorio { get; set; }

        private ObsCollection<Relatorio> relatoriosList = new ObsCollection<Relatorio>();
        private string descricaoOriginal;

        public CadRelatorio()
        {
            InitializeComponent();
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
            Relatorio relatorio = new Relatorio();

            relatorio.Descricao = txtDescricao.Text.Trim().ToUpper();

            if (txtBtnInserir.Text.Equals("Inserir"))
            {
                if (!sqlite.ExistRelatorio(relatorio.Descricao))
                {
                    InsertRelatorio(relatorio);
                }
                else
                {
                    MessageBox.Show("A Linha Informada Já Existe no Cadastro",
                    "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (txtBtnInserir.Text.Equals("Salvar"))
            {
                string descricaoAlterada = relatorio.Descricao;

                relatorio.ID = ((Relatorio)gridDados.SelectedItem).ID;

                if (descricaoAlterada != descricaoOriginal)
                {
                    if (!sqlite.ExistRelatorio(relatorio.Descricao))
                    {
                        UpdateRelatorio(relatorio);
                    }
                    else
                    {
                        MessageBox.Show("O Relatório Informado Já Existe no Cadastro",
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    UpdateRelatorio(relatorio);
                }
            }

            txtDescricao.Focus();
        }

        private void UpdateRelatorio(Relatorio relatorio)
        {
            SQLite sqlite = new SQLite();

            string query = "UPDATE RELATORIO SET DESCRICAO='" + relatorio.Descricao + "' WHERE ID=" + relatorio.ID;

            if (sqlite.Connect())
            {
                if (sqlite.UpdateQuery(query))
                {
                    gridDados.IsEnabled = true;
                    ChangeItemValues(relatoriosList, relatorio);
                    gridDados.Items.Refresh();
                    gridDados.ScrollIntoView(relatorio);

                    txtBtnInserir.Text = "Inserir";
                    txtDescricao.Clear();
                }

                sqlite.Disconnect();
                sqlite = null;
            }
        }

        private void ChangeItemValues(ObsCollection<Relatorio> relatoriosList, Relatorio relatorio)
        {
            foreach (Relatorio item in relatoriosList)
            {
                if (item.ID.Equals(relatorio.ID))
                {
                    item.Descricao = relatorio.Descricao;
                }
            }
        }

        private void InsertRelatorio(Relatorio relatorio)
        {
            SQLite sqlite = new SQLite();

            string query = "INSERT INTO RELATORIO (DESCRICAO) VALUES ('" + relatorio.Descricao + "')";

            if (sqlite.Connect())
            {
                relatorio.ID = sqlite.InsertQuery(query, "RELATORIO");

                relatoriosList.Add(relatorio);
                relatoriosList.UpdateCollection();
                gridDados.Items.Refresh();
                gridDados.ScrollIntoView(relatorio);

                txtDescricao.Clear();

                sqlite.Disconnect();
                sqlite = null;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable table = new DataTable();
            string query = "SELECT ID,DESCRICAO FROM RELATORIO ORDER BY ID";

            if (sqlite.Connect())
            {
                table = sqlite.GetTable(query);

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        Relatorio relatorio = new Relatorio();

                        relatorio.ID = Convert.ToInt64(row[0]);
                        relatorio.Descricao = Convert.ToString(row[1]);

                        relatoriosList.Add(relatorio);
                    }
                }

                gridDados.ItemsSource = relatoriosList;

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

        private void menuAlterar_Click(object sender, RoutedEventArgs e)
        {
            Relatorio relatorio = new Relatorio();
            relatorio = (Relatorio)gridDados.SelectedItem;

            txtBtnInserir.Text = "Salvar";

            txtDescricao.Text = relatorio.Descricao;

            gridDados.IsEnabled = false;

            descricaoOriginal = relatorio.Descricao;

            txtDescricao.Focus();
        }

        private void menuExcluir_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            Relatorio relatorio = new Relatorio();
            string query;

            relatorio = (Relatorio)gridDados.SelectedItem;
            query = "DELETE FROM RELATORIO WHERE ID=" + relatorio.ID;

            if (MessageBox.Show("Confirma exclusão?\n" + relatorio.ID.ToString() + " - " + relatorio.Descricao,
                "Confirmação de Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (sqlite.Connect())
                {
                    if (sqlite.DeleteQuery(query))
                    {
                        relatoriosList.Remove(relatorio);
                        gridDados.Items.Refresh();

                        txtDescricao.Clear();
                    }

                    sqlite.Disconnect();
                    sqlite = null;
                }

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

        private void gridDados_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                DataGrid grid = sender as DataGrid;
                if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                {
                    DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;

                    this.Relatorio = (Relatorio)dgr.Item;

                    this.Close();
                }
            }
        }

    }
}
