using Relacao.Classes;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for CadSubTipoMateriaPrima.xaml
    /// </summary>
    public partial class CadSubTipoMateriaPrima : Window
    {
        private ObsCollection<SubTipoMateriaPrima> subtiposList = new ObsCollection<SubTipoMateriaPrima>();
        private SubTipoMateriaPrima subtipoOriginal = new SubTipoMateriaPrima();

        public CadSubTipoMateriaPrima()
        {
            InitializeComponent();
        }

        private void Insert_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!txtDescricao.Text.Trim().Equals("") && !comboTipoMateriaPrima.SelectedIndex.Equals(-1))
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
            SubTipoMateriaPrima subtipo = new SubTipoMateriaPrima();

            long tipoID = Convert.ToInt64(comboTipoMateriaPrima.SelectedValue);

            subtipo.Tipo = sqlite.GetTipoMateriaPrima(tipoID);
            subtipo.Descricao = txtDescricao.Text.Trim().ToUpper();

            if (txtBtnInserir.Text.Equals("Inserir"))
            {
                if (!sqlite.ExistSubTipoMateriaPrima(subtipo))
                {
                    InsertSubTipoMateriaPrima(subtipo);
                }
                else
                {
                    MessageBox.Show("O Sub-Tipo de Matéria-Prima Informado Já Existe no Cadastro",
                    "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (txtBtnInserir.Text.Equals("Salvar"))
            {
                SubTipoMateriaPrima subtipoAlterado = new SubTipoMateriaPrima();
                subtipoAlterado = subtipo;

                subtipo.ID = ((SubTipoMateriaPrima)gridDados.SelectedItem).ID;

                if (subtipoAlterado.Tipo.ID != subtipoOriginal.Tipo.ID || subtipoAlterado.Descricao != subtipoOriginal.Descricao)
                {
                    if (!sqlite.ExistSubTipoMateriaPrima(subtipo))
                    {
                        UpdateSubTipoMateriaPrima(subtipo);
                    }
                    else
                    {
                        MessageBox.Show("O Sub-Tipo de Matéria-Prima Informado Já Existe no Cadastro",
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    UpdateSubTipoMateriaPrima(subtipo);
                }
            }

            txtDescricao.Focus();
        }

        private void UpdateSubTipoMateriaPrima(SubTipoMateriaPrima subtipo)
        {
            SQLite sqlite = new SQLite();

            string query = "UPDATE SUBTIPOMATERIAPRIMA SET DESCRICAO='" + subtipo.Descricao + "', IDTIPOMATERIAPRIMA=" +
                subtipo.Tipo.ID.ToString() + " WHERE ID=" + subtipo.ID;

            if (sqlite.Connect())
            {
                if (sqlite.UpdateQuery(query))
                {
                    gridDados.IsEnabled = true;
                    ChangeItemValues(subtiposList, subtipo);
                    gridDados.Items.Refresh();
                    gridDados.ScrollIntoView(subtipo);

                    txtBtnInserir.Text = "Inserir";
                    comboTipoMateriaPrima.SelectedIndex = -1;
                    txtDescricao.Clear();
                }

                sqlite.Disconnect();
                sqlite = null;
            }
        }

        private void InsertSubTipoMateriaPrima(SubTipoMateriaPrima subtipo)
        {
            SQLite sqlite = new SQLite();

            string query = "INSERT INTO SUBTIPOMATERIAPRIMA (IDTIPOMATERIAPRIMA,DESCRICAO) VALUES (" +
                subtipo.Tipo.ID.ToString() + ",'" + subtipo.Descricao + "')";

            if (sqlite.Connect())
            {
                subtipo.ID = sqlite.InsertQuery(query, "SUBTIPOMATERIAPRIMA");

                subtiposList.Add(subtipo);
                subtiposList.UpdateCollection();
                gridDados.Items.Refresh();
                gridDados.ScrollIntoView(subtipo);

                comboTipoMateriaPrima.SelectedIndex = -1;
                txtDescricao.Clear();

                sqlite.Disconnect();
                sqlite = null;
            }
        }

        private void ChangeItemValues(ObsCollection<SubTipoMateriaPrima> subtiposList, SubTipoMateriaPrima subtipo)
        {
            foreach (SubTipoMateriaPrima item in subtiposList)
            {
                if (item.ID.Equals(subtipo.ID))
                {
                    item.Descricao = subtipo.Descricao;
                    item.Tipo = subtipo.Tipo;
                }
            }
        }

        private void menuAlterar_Click(object sender, RoutedEventArgs e)
        {
            SubTipoMateriaPrima subtipo = new SubTipoMateriaPrima();
            subtipo = (SubTipoMateriaPrima)gridDados.SelectedItem;

            txtBtnInserir.Text = "Salvar";

            comboTipoMateriaPrima.SelectedValue = subtipo.Tipo.ID;
            txtDescricao.Text = subtipo.Descricao;

            gridDados.IsEnabled = false;

            subtipoOriginal = subtipo;

            txtDescricao.Focus();
        }

        private void menuExcluir_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            SubTipoMateriaPrima subtipo = new SubTipoMateriaPrima();
            string query;

            subtipo = (SubTipoMateriaPrima)gridDados.SelectedItem;
            query = "DELETE FROM SUBTIPOMATERIAPRIMA WHERE ID=" + subtipo.ID;

            if (MessageBox.Show("Confirma exclusão?\n" + subtipo.ID.ToString() + " - " + subtipo.Tipo.Descricao + " - " + subtipo.Tipo.Unidade + " - " + subtipo.Descricao,
                "Confirmação de Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (sqlite.Connect())
                {
                    if (sqlite.DeleteQuery(query))
                    {
                        subtiposList.Remove(subtipo);
                        gridDados.Items.Refresh();

                        comboTipoMateriaPrima.SelectedIndex = -1;
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
            DataTable tipos = new DataTable();
            DataTable subtipos = new DataTable();

            string queryTipos =
                "SELECT ID,DESCRICAO FROM TIPOMATERIAPRIMA ORDER BY DESCRICAO";
            string querySubTipos =
                "SELECT SUBTIPOMATERIAPRIMA.ID, " +
                "       SUBTIPOMATERIAPRIMA.IDTIPOMATERIAPRIMA, " +
                "       TIPOMATERIAPRIMA.DESCRICAO, " +
                "       TIPOMATERIAPRIMA.UNIDADE, " +
                "       SUBTIPOMATERIAPRIMA.DESCRICAO " +
                "  FROM SUBTIPOMATERIAPRIMA, " +
                "       TIPOMATERIAPRIMA " +
                " WHERE SUBTIPOMATERIAPRIMA.IDTIPOMATERIAPRIMA = TIPOMATERIAPRIMA.ID";

            if (sqlite.Connect())
            {
                tipos = sqlite.GetTable(queryTipos);
                subtipos = sqlite.GetTable(querySubTipos);

                comboTipoMateriaPrima.ItemsSource = tipos.DefaultView;

                if (subtipos.Rows.Count > 0)
                {
                    foreach (DataRow row in subtipos.Rows)
                    {
                        SubTipoMateriaPrima subtipo = new SubTipoMateriaPrima();

                        subtipo.ID = Convert.ToInt64(row[0]);
                        subtipo.Tipo.ID = Convert.ToInt64(row[1]);
                        subtipo.Tipo.Descricao = Convert.ToString(row[2]);
                        subtipo.Tipo.Unidade = Convert.ToString(row[3]);
                        subtipo.Descricao = Convert.ToString(row[4]);

                        subtiposList.Add(subtipo);
                    }
                }

                gridDados.ItemsSource = subtiposList;

                sqlite.Disconnect();
                sqlite = null;
            }

            txtDescricao.Focus();
        }

        private void btnInserirTPMP_Click(object sender, RoutedEventArgs e)
        {
            CadTipoMateriaPrima formulario = new CadTipoMateriaPrima();
            formulario.ShowDialog();

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
                comboTipoMateriaPrima.SelectedIndex = -1;
                txtDescricao.Clear();

                txtDescricao.Focus();
            }
        }

        private void btnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable subtipos = new DataTable();

            string querySubTipos =
                "SELECT SUBTIPOMATERIAPRIMA.ID, " +
                "       SUBTIPOMATERIAPRIMA.IDTIPOMATERIAPRIMA, " +
                "       TIPOMATERIAPRIMA.DESCRICAO, " +
                "       TIPOMATERIAPRIMA.UNIDADE, " +
                "       SUBTIPOMATERIAPRIMA.DESCRICAO " +
                "  FROM SUBTIPOMATERIAPRIMA, " +
                "       TIPOMATERIAPRIMA " +
                " WHERE SUBTIPOMATERIAPRIMA.IDTIPOMATERIAPRIMA = TIPOMATERIAPRIMA.ID ";

            int selTipoMateriaPrima = comboTipoMateriaPrima.SelectedIndex;

            if (selTipoMateriaPrima >= 0)
            {
                DataRowView result = (DataRowView)comboTipoMateriaPrima.SelectedItem;
                string tipomateriaprima = result.Row[0].ToString();

                querySubTipos += "AND SUBTIPOMATERIAPRIMA.IDTIPOMATERIAPRIMA = " + tipomateriaprima + " ";
            }

            if (sqlite.Connect())
            {
                subtiposList.Clear();

                subtipos = sqlite.GetTable(querySubTipos);

                if (subtipos.Rows.Count > 0)
                {
                    foreach (DataRow row in subtipos.Rows)
                    {
                        SubTipoMateriaPrima subtipo = new SubTipoMateriaPrima();

                        subtipo.ID = Convert.ToInt64(row[0]);
                        subtipo.Tipo.ID = Convert.ToInt64(row[1]);
                        subtipo.Tipo.Descricao = Convert.ToString(row[2]);
                        subtipo.Tipo.Unidade = Convert.ToString(row[3]);
                        subtipo.Descricao = Convert.ToString(row[4]);

                        subtiposList.Add(subtipo);
                    }
                }

                gridDados.ItemsSource = subtiposList;

                sqlite.Disconnect();
                sqlite = null;
            }

            btnCancelarFiltro.IsEnabled = true;

            txtDescricao.Focus();
        }

        private void btnCancelarFiltro_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable subtipos = new DataTable();

            string querySubTipos =
                "SELECT SUBTIPOMATERIAPRIMA.ID, " +
                "       SUBTIPOMATERIAPRIMA.IDTIPOMATERIAPRIMA, " +
                "       TIPOMATERIAPRIMA.DESCRICAO, " +
                "       TIPOMATERIAPRIMA.UNIDADE, " +
                "       SUBTIPOMATERIAPRIMA.DESCRICAO " +
                "  FROM SUBTIPOMATERIAPRIMA, " +
                "       TIPOMATERIAPRIMA " +
                " WHERE SUBTIPOMATERIAPRIMA.IDTIPOMATERIAPRIMA = TIPOMATERIAPRIMA.ID";

            comboTipoMateriaPrima.SelectedIndex = -1;

            if (sqlite.Connect())
            {
                subtiposList.Clear();

                subtipos = sqlite.GetTable(querySubTipos);

                if (subtipos.Rows.Count > 0)
                {
                    foreach (DataRow row in subtipos.Rows)
                    {
                        SubTipoMateriaPrima subtipo = new SubTipoMateriaPrima();

                        subtipo.ID = Convert.ToInt64(row[0]);
                        subtipo.Tipo.ID = Convert.ToInt64(row[1]);
                        subtipo.Tipo.Descricao = Convert.ToString(row[2]);
                        subtipo.Tipo.Unidade = Convert.ToString(row[3]);
                        subtipo.Descricao = Convert.ToString(row[4]);

                        subtiposList.Add(subtipo);
                    }
                }

                gridDados.ItemsSource = subtiposList;

                sqlite.Disconnect();
                sqlite = null;
            }

            btnCancelarFiltro.IsEnabled = false;
            btnFiltrar.IsEnabled = false;

            txtDescricao.Focus();
        }

        private void comboTipoMateriaPrima_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            btnFiltrar.IsEnabled = true;
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
