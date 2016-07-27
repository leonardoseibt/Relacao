using Relacao.Classes;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for CadTipoMateriaPrima.xaml
    /// </summary>
    public partial class CadTipoMateriaPrima : Window
    {
        private ObsCollection<TipoMateriaPrima> tiposList = new ObsCollection<TipoMateriaPrima>();
        private string descricaoOriginal;

        public CadTipoMateriaPrima()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable table = new DataTable();
            string query = "SELECT ID,DESCRICAO,UNIDADE FROM TIPOMATERIAPRIMA ORDER BY ID";

            if (sqlite.Connect())
            {
                table = sqlite.GetTable(query);

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        TipoMateriaPrima tipo = new TipoMateriaPrima();

                        tipo.ID = Convert.ToInt64(row[0]);
                        tipo.Descricao = Convert.ToString(row[1]);
                        tipo.Unidade = Convert.ToString(row[2]);

                        tiposList.Add(tipo);
                    }
                }

                gridDados.ItemsSource = tiposList;

                sqlite.Disconnect();
                sqlite = null;
            }

            txtDescricao.Focus();
        }

        private void Insert_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //if (!txtDescricao.Text.Trim().Equals("") && !txtUnidade.Text.Trim().Equals(""))
            if (!txtDescricao.Text.Trim().Equals("") && !comboUnidade.SelectedIndex.Equals(-1))
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

            TipoMateriaPrima tipo = new TipoMateriaPrima();

            tipo.Descricao = txtDescricao.Text.Trim().ToUpper();
            //tipo.Unidade = txtUnidade.Text.Trim().ToUpper();
            tipo.Unidade = comboUnidade.Text;

            if (txtBtnInserir.Text.Equals("Inserir"))
            {
                if (!sqlite.ExistTipoMateriaPrima(tipo.Descricao))
                {
                    InsertTipoMateriaPrima(tipo);
                }
                else
                {
                    MessageBox.Show("O Tipo de Matéria-Prima Informado Já Existe no Cadastro",
                    "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (txtBtnInserir.Text.Equals("Salvar"))
            {
                string descricaoAlterada = tipo.Descricao;

                tipo.ID = ((TipoMateriaPrima)gridDados.SelectedItem).ID;

                if (descricaoAlterada != descricaoOriginal)
                {
                    if (!sqlite.ExistTipoMateriaPrima(tipo.Descricao))
                    {
                        UpdateTipoMateriaPrima(tipo);
                    }
                    else
                    {
                        MessageBox.Show("O Tipo de Matéria-Prima Informado Já Existe no Cadastro",
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    UpdateTipoMateriaPrima(tipo);
                }
            }

            txtDescricao.Focus();
        }

        private void UpdateTipoMateriaPrima(TipoMateriaPrima tipo)
        {
            SQLite sqlite = new SQLite();

            string query = "UPDATE TIPOMATERIAPRIMA SET DESCRICAO='" + tipo.Descricao + "', UNIDADE='" + tipo.Unidade + "' WHERE ID=" + tipo.ID;

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
                    //txtUnidade.Clear();
                    comboUnidade.SelectedIndex = -1;
                }

                sqlite.Disconnect();
                sqlite = null;
            }
        }

        private void InsertTipoMateriaPrima(TipoMateriaPrima tipo)
        {
            SQLite sqlite = new SQLite();

            string query = "INSERT INTO TIPOMATERIAPRIMA (DESCRICAO,UNIDADE) VALUES ('" + tipo.Descricao + "','" + tipo.Unidade + "')";

            if (sqlite.Connect())
            {
                tipo.ID = sqlite.InsertQuery(query, "TIPOMATERIAPRIMA");

                tiposList.Add(tipo);
                tiposList.UpdateCollection();
                gridDados.Items.Refresh();
                gridDados.ScrollIntoView(tipo);

                txtDescricao.Clear();
                //txtUnidade.Clear();
                comboUnidade.SelectedIndex = -1;

                sqlite.Disconnect();
                sqlite = null;
            }
        }

        private void ChangeItemValues(ObsCollection<TipoMateriaPrima> tiposList, TipoMateriaPrima tipo)
        {
            foreach (TipoMateriaPrima item in tiposList)
            {
                if (item.ID.Equals(tipo.ID))
                {
                    item.Descricao = tipo.Descricao;
                    item.Unidade = tipo.Unidade;
                }
            }
        }

        private void menuAlterar_Click(object sender, RoutedEventArgs e)
        {
            TipoMateriaPrima tipo = new TipoMateriaPrima();
            tipo = (TipoMateriaPrima)gridDados.SelectedItem;

            txtBtnInserir.Text = "Salvar";

            txtDescricao.Text = tipo.Descricao;
            //txtUnidade.Text = tipo.Unidade;
            comboUnidade.Text = tipo.Unidade;

            gridDados.IsEnabled = false;

            descricaoOriginal = tipo.Descricao;

            txtDescricao.Focus();
        }

        private void menuExcluir_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            TipoMateriaPrima tipo = new TipoMateriaPrima();
            string query;

            tipo = (TipoMateriaPrima)gridDados.SelectedItem;
            query = "DELETE FROM TIPOMATERIAPRIMA WHERE ID=" + tipo.ID;

            if (MessageBox.Show("Confirma exclusão?\n" + tipo.ID.ToString() + " - " + tipo.Descricao + " - " + tipo.Unidade,
                "Confirmação de Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (sqlite.Connect())
                {
                    if (sqlite.DeleteQuery(query))
                    {
                        tiposList.Remove(tipo);
                        gridDados.Items.Refresh();

                        txtDescricao.Clear();
                        //txtUnidade.Clear();
                        comboUnidade.SelectedIndex = -1;
                    }

                    sqlite.Disconnect();
                    sqlite = null;
                }

                txtDescricao.Focus();
            }
        }

        private void txtDescricao_GotFocus(object sender, RoutedEventArgs e)
        {
            txtDescricao.SelectAll();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                gridDados.IsEnabled = true;

                txtBtnInserir.Text = "Inserir";
                txtDescricao.Clear();
                //txtUnidade.Clear();
                comboUnidade.SelectedIndex = -1;

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
