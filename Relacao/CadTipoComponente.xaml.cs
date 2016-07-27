using Relacao.Classes;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for CadTipoComponente.xaml
    /// </summary>
    public partial class CadTipoComponente : Window
    {
        private ObsCollection<TipoComponente> tiposList = new ObsCollection<TipoComponente>();
        private string descricaoOriginal;

        public CadTipoComponente()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable table = new DataTable();
            string query = "SELECT ID,DESCRICAO FROM TIPOCOMPONENTE ORDER BY ID";

            if (sqlite.Connect())
            {
                table = sqlite.GetTable(query);

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        TipoComponente tipo = new TipoComponente();

                        tipo.ID = Convert.ToInt64(row[0]);
                        tipo.Descricao = Convert.ToString(row[1]);

                        tiposList.Add(tipo);
                    }
                }

                gridDados.ItemsSource = tiposList;

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

        private void Insert_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            TipoComponente tipo = new TipoComponente();

            tipo.Descricao = txtDescricao.Text.Trim().ToUpper();

            if (txtBtnInserir.Text.Equals("Inserir"))
            {
                if (!sqlite.ExistTipoComponente(tipo.Descricao))
                {
                    InsertTipoComponente(tipo);
                }
                else
                {
                    MessageBox.Show("O Tipo de Componente Informado Já Existe no Cadastro",
                    "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (txtBtnInserir.Text.Equals("Salvar"))
            {
                string descricaoAlterada = tipo.Descricao;

                tipo.ID = ((TipoComponente)gridDados.SelectedItem).ID;

                if (descricaoAlterada != descricaoOriginal)
                {
                    if (!sqlite.ExistTipoComponente(tipo.Descricao))
                    {
                        UpdateTipoComponente(tipo);
                    }
                    else
                    {
                        MessageBox.Show("O Tipo de Componente Informado Já Existe no Cadastro",
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    UpdateTipoComponente(tipo);
                }
            }

            txtDescricao.Focus();
        }

        private void UpdateTipoComponente(TipoComponente tipo)
        {
            SQLite sqlite = new SQLite();

            string query = "UPDATE TIPOCOMPONENTE SET DESCRICAO='" + tipo.Descricao + "' WHERE ID=" + tipo.ID;

            if (sqlite.Connect())
            {
                if (sqlite.UpdateQuery(query))
                {
                    gridDados.IsEnabled = true;
                    ChangeItemValues(tiposList, tipo);
                    gridDados.Items.Refresh();
                    gridDados.ScrollIntoView(tipo);

                    txtBtnInserir.Text = "Inserir";
                    txtDescricao.Clear();
                }

                sqlite.Disconnect();
                sqlite = null;
            }
        }

        private void InsertTipoComponente(TipoComponente tipo)
        {
            SQLite sqlite = new SQLite();

            string query = "INSERT INTO TIPOCOMPONENTE (DESCRICAO) VALUES ('" + tipo.Descricao + "')";

            if (sqlite.Connect())
            {
                tipo.ID = sqlite.InsertQuery(query, "TIPOCOMPONENTE");

                tiposList.Add(tipo);
                tiposList.UpdateCollection();
                gridDados.Items.Refresh();
                gridDados.ScrollIntoView(tipo);

                txtDescricao.Clear();

                sqlite.Disconnect();
                sqlite = null;
            }
        }

        private void ChangeItemValues(ObsCollection<TipoComponente> tiposList, TipoComponente tipo)
        {
            foreach (TipoComponente item in tiposList)
            {
                if (item.ID.Equals(tipo.ID))
                {
                    item.Descricao = tipo.Descricao;
                }
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

        private void txtDescricao_GotFocus(object sender, RoutedEventArgs e)
        {
            txtDescricao.SelectAll();
        }

        private void menuAlterar_Click(object sender, RoutedEventArgs e)
        {
            TipoComponente tipo = new TipoComponente();
            tipo = (TipoComponente)gridDados.SelectedItem;

            txtBtnInserir.Text = "Salvar";

            txtDescricao.Text = tipo.Descricao;

            gridDados.IsEnabled = false;

            descricaoOriginal = tipo.Descricao;

            txtDescricao.Focus();
        }

        private void menuExcluir_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            TipoComponente tipo = new TipoComponente();
            string query;

            tipo = (TipoComponente)gridDados.SelectedItem;
            query = "DELETE FROM TIPOCOMPONENTE WHERE ID=" + tipo.ID;

            if (MessageBox.Show("Confirma exclusão?\n" + tipo.ID.ToString() + " - " + tipo.Descricao,
                "Confirmação de Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (sqlite.Connect())
                {
                    if (sqlite.DeleteQuery(query))
                    {
                        tiposList.Remove(tipo);
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

    }
}
