using Relacao.Classes;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for CadMateriaPrima.xaml
    /// </summary>
    public partial class CadMateriaPrima : Window
    {
        private ObsCollection<MateriaPrima> materiasprimasList = new ObsCollection<MateriaPrima>();
        private string descricaoOriginal;

        public CadMateriaPrima()
        {
            InitializeComponent();
        }

        private void Insert_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!comboTipoMateriaPrima.SelectedIndex.Equals(-1) &&
                !comboSubTipoMateriaPrima.SelectedIndex.Equals(-1) && !txtBitola.Text.Equals(""))
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
            MateriaPrima materiaprima = new MateriaPrima();

            long tipoID = Convert.ToInt64(comboTipoMateriaPrima.SelectedValue);
            long subtipoID = Convert.ToInt64(comboSubTipoMateriaPrima.SelectedValue);

            materiaprima.SubTipo = sqlite.GetSubTipoMateriaPrima(subtipoID);
            materiaprima.SubTipo.Tipo = sqlite.GetTipoMateriaPrima(tipoID);
            materiaprima.Descricao = txtDescricao.Text.Trim().ToUpper();
            materiaprima.Bitola = txtBitola.Text.ToString();
            materiaprima.Perda = Convert.ToInt32(txtPerda.Value.Equals(null) ? 0 : txtPerda.Value);
            materiaprima.Observacoes = txtObservacoes.Text.Trim().ToUpper();

            if (txtBtnInserir.Text.Equals("Inserir"))
            {
                if (!sqlite.ExistMateriaPrima(materiaprima))
                {
                    InsertMateriaPrima(materiaprima);
                }
                else
                {
                    MessageBox.Show("A Matéria-Prima Informada Já Existe no Cadastro",
                    "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (txtBtnInserir.Text.Equals("Salvar"))
            {
                string descricaoAlterada = materiaprima.Descricao;

                materiaprima.ID = ((MateriaPrima)gridDados.SelectedItem).ID;

                if (descricaoAlterada != descricaoOriginal)
                {
                    if (!sqlite.ExistMateriaPrima(materiaprima))
                    {
                        UpdateMateriaPrima(materiaprima);
                    }
                    else
                    {
                        MessageBox.Show("Nada foi alterado!!!",
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    UpdateMateriaPrima(materiaprima);
                }
            }

            txtBitola.Focus();
        }

        private void UpdateMateriaPrima(MateriaPrima materiaprima)
        {
            SQLite sqlite = new SQLite();

            string query = "UPDATE MATERIAPRIMA SET " +
                        "IDTIPOMATERIAPRIMA=" + materiaprima.SubTipo.Tipo.ID.ToString() + "," +
                        "IDSUBTIPOMATERIAPRIMA=" + materiaprima.SubTipo.ID.ToString() + "," +
                        "DESCRICAO='" + materiaprima.Descricao + "'," +
                        "BITOLA='" + materiaprima.Bitola + "'," +
                        "PERDA=" + materiaprima.Perda.ToString() + "," +
                        "OBSERVACOES='" + materiaprima.Observacoes + "' " +
                        "WHERE ID=" + materiaprima.ID;

            if (sqlite.Connect())
            {
                if (sqlite.UpdateQuery(query))
                {
                    gridDados.IsEnabled = true;
                    ChangeItemValues(materiasprimasList, materiaprima);
                    gridDados.Items.Refresh();
                    gridDados.ScrollIntoView(materiaprima);

                    txtBtnInserir.Text = "Inserir";

                    comboTipoMateriaPrima.SelectedIndex = -1;
                    comboSubTipoMateriaPrima.SelectedIndex = -1;
                    txtBitola.Clear();
                    txtPerda.Value = null;
                    txtDescricao.Clear();
                    txtObservacoes.Clear();
                }

                sqlite.Disconnect();
                sqlite = null;
            }
        }

        private void InsertMateriaPrima(MateriaPrima materiaprima)
        {
            SQLite sqlite = new SQLite();

            string query = "INSERT INTO MATERIAPRIMA (" +
                "IDTIPOMATERIAPRIMA," +
                "IDSUBTIPOMATERIAPRIMA," +
                "DESCRICAO," +
                "BITOLA," +
                "PERDA," +
                "OBSERVACOES) VALUES (" +
                materiaprima.SubTipo.Tipo.ID.ToString() + "," +
                materiaprima.SubTipo.ID.ToString() + ",'" +
                materiaprima.Descricao + "','" +
                materiaprima.Bitola + "'," +
                materiaprima.Perda + ",'" +
                materiaprima.Observacoes + "')";

            if (sqlite.Connect())
            {
                materiaprima.ID = sqlite.InsertQuery(query, "MATERIAPRIMA");

                materiasprimasList.Add(materiaprima);
                materiasprimasList.UpdateCollection();
                gridDados.Items.Refresh();
                gridDados.ScrollIntoView(materiaprima);

                comboTipoMateriaPrima.SelectedIndex = -1;
                comboSubTipoMateriaPrima.SelectedIndex = -1;
                txtBitola.Clear();
                txtPerda.Value = null;
                txtDescricao.Clear();
                txtObservacoes.Clear();

                sqlite.Disconnect();
                sqlite = null;
            }
        }

        private void ChangeItemValues(ObsCollection<MateriaPrima> materiasprimasList, MateriaPrima materiaprima)
        {
            foreach (MateriaPrima item in materiasprimasList)
            {
                if (item.ID.Equals(materiaprima.ID))
                {
                    item.Descricao = materiaprima.Descricao;
                    item.Bitola = materiaprima.Bitola;
                    item.Perda = materiaprima.Perda;
                    item.Observacoes = materiaprima.Observacoes;
                    item.SubTipo = materiaprima.SubTipo;
                }
            }
        }

        private void menuAlterar_Click(object sender, RoutedEventArgs e)
        {
            MateriaPrima materiaprima = new MateriaPrima();
            materiaprima = (MateriaPrima)gridDados.SelectedItem;

            txtBtnInserir.Text = "Salvar";

            comboTipoMateriaPrima.SelectedValue = materiaprima.SubTipo.Tipo.ID;
            comboSubTipoMateriaPrima.SelectedValue = materiaprima.SubTipo.ID;
            txtBitola.Text = materiaprima.Bitola;
            txtPerda.Value = materiaprima.Perda;
            txtDescricao.Text = materiaprima.Descricao;
            txtObservacoes.Text = materiaprima.Observacoes;

            gridDados.IsEnabled = false;

            descricaoOriginal = materiaprima.Descricao;

            txtBitola.Focus();
        }

        private void menuExcluir_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            MateriaPrima materiaprima = new MateriaPrima();
            string query;

            materiaprima = (MateriaPrima)gridDados.SelectedItem;
            query = "DELETE FROM MATERIAPRIMA WHERE ID=" + materiaprima.ID;

            if (MessageBox.Show("Confirma exclusão?\n" + materiaprima.ID.ToString() + " - " + materiaprima.Descricao,
                "Confirmação de Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (sqlite.Connect())
                {
                    if (sqlite.DeleteQuery(query))
                    {
                        materiasprimasList.Remove(materiaprima);
                        gridDados.Items.Refresh();

                        comboTipoMateriaPrima.SelectedIndex = -1;
                        comboSubTipoMateriaPrima.SelectedIndex = -1;
                        txtBitola.Clear();
                        txtPerda.Value = null;
                        txtDescricao.Clear();
                        txtObservacoes.Clear();
                    }

                    sqlite.Disconnect();
                    sqlite = null;
                }

                txtBitola.Focus();
            }
        }

        public static bool OnlyNumeric(string text)
        {
            Regex regex = new Regex("[^0-9.X'\"/]+");

            return !regex.IsMatch(text);
        }

        private void txtPerda_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !OnlyNumeric(e.Text);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable tipos = new DataTable();
            DataTable materiasprimas = new DataTable();

            string queryTipos =
                "SELECT ID,DESCRICAO FROM TIPOMATERIAPRIMA ORDER BY DESCRICAO";
            string queryMateriasPrimas =
                "SELECT MATERIAPRIMA.ID, " +
                "       MATERIAPRIMA.IDTIPOMATERIAPRIMA IDTIPO, " +
                "       TIPOMATERIAPRIMA.DESCRICAO TIPO, " +
                "       TIPOMATERIAPRIMA.UNIDADE, " +
                "       MATERIAPRIMA.IDSUBTIPOMATERIAPRIMA IDSUBTIPO, " +
                "       SUBTIPOMATERIAPRIMA.DESCRICAO SUBTIPO, " +
                "       MATERIAPRIMA.DESCRICAO, " +
                "       MATERIAPRIMA.BITOLA, " +
                "       MATERIAPRIMA.PERDA, " +
                "       MATERIAPRIMA.OBSERVACOES " +
                "  FROM MATERIAPRIMA, " +
                "       TIPOMATERIAPRIMA, " +
                "       SUBTIPOMATERIAPRIMA " +
                " WHERE TIPOMATERIAPRIMA.ID = MATERIAPRIMA.IDTIPOMATERIAPRIMA AND " +
                "       SUBTIPOMATERIAPRIMA.ID = MATERIAPRIMA.IDSUBTIPOMATERIAPRIMA";

            if (sqlite.Connect())
            {
                tipos = sqlite.GetTable(queryTipos);
                materiasprimas = sqlite.GetTable(queryMateriasPrimas);

                comboTipoMateriaPrima.ItemsSource = tipos.DefaultView;

                if (materiasprimas.Rows.Count > 0)
                {
                    foreach (DataRow row in materiasprimas.Rows)
                    {
                        MateriaPrima materiaprima = new MateriaPrima();

                        materiaprima.ID = Convert.ToInt64(row[0]);
                        materiaprima.SubTipo.Tipo.ID = Convert.ToInt64(row[1]);
                        materiaprima.SubTipo.Tipo.Descricao = Convert.ToString(row[2]);
                        materiaprima.SubTipo.Tipo.Unidade = Convert.ToString(row[3]);
                        materiaprima.SubTipo.ID = Convert.ToInt64(row[4]);
                        materiaprima.SubTipo.Descricao = Convert.ToString(row[5]);
                        materiaprima.Descricao = Convert.ToString(row[6]);
                        materiaprima.Bitola = Convert.ToString(row[7]);
                        materiaprima.Perda = Convert.ToInt32(row[8]);
                        materiaprima.Observacoes = Convert.ToString(row[9]);

                        materiasprimasList.Add(materiaprima);
                    }
                }

                gridDados.ItemsSource = materiasprimasList;

                sqlite.Disconnect();
                sqlite = null;
            }

            txtBitola.Focus();
        }

        private void comboTipoMateriaPrima_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!comboTipoMateriaPrima.SelectedIndex.Equals(-1))
            {
                DataRowView tipoRow = (DataRowView)comboTipoMateriaPrima.SelectedItem;

                SQLite sqlite = new SQLite();
                DataTable subtipos = new DataTable();

                string querySubTipos =
                    "SELECT ID, " +
                    "       DESCRICAO " +
                    "  FROM SUBTIPOMATERIAPRIMA " +
                    " WHERE IDTIPOMATERIAPRIMA = " + tipoRow.Row[0].ToString();

                if (sqlite.Connect())
                {
                    subtipos = sqlite.GetTable(querySubTipos);
                    comboSubTipoMateriaPrima.ItemsSource = subtipos.DefaultView;

                    sqlite.Disconnect();
                    sqlite = null;
                }

                btnFiltrar.IsEnabled = true;

                txtBitola.Focus();
            }

            AtualizaDescricao();
        }

        private void AtualizaDescricao()
        {
            DataRowView tipoRow = (DataRowView)comboTipoMateriaPrima.SelectedItem;
            DataRowView subtipoRow = (DataRowView)comboSubTipoMateriaPrima.SelectedItem;

            string tipo = tipoRow != null ? tipoRow.Row[1].ToString() : "";
            string subtipo = subtipoRow != null ? subtipoRow.Row[1].ToString() : "";
            string bitola = txtBitola.Text.ToString();

            string descricao = tipo + " " + subtipo + " " + bitola;

            ThreadPool.QueueUserWorkItem((o) =>
            {
                Dispatcher.Invoke((Action)(() => txtDescricao.Text = descricao));
            });

        }

        private void comboSubTipoMateriaPrima_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            btnFiltrar.IsEnabled = true;

            AtualizaDescricao();

            txtBitola.Focus();
        }

        private void btnInserirSTPMP_Click(object sender, RoutedEventArgs e)
        {
            CadSubTipoMateriaPrima formulario = new CadSubTipoMateriaPrima();
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

            comboSubTipoMateriaPrima.ItemsSource = null;
            txtBitola.Focus();
        }

        private void txtObservacoes_GotFocus(object sender, RoutedEventArgs e)
        {
            txtObservacoes.SelectAll();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                gridDados.IsEnabled = true;

                txtBtnInserir.Text = "Inserir";

                comboTipoMateriaPrima.SelectedIndex = -1;
                comboSubTipoMateriaPrima.SelectedIndex = -1;
                txtBitola.Clear();
                txtPerda.Value = null;
                txtDescricao.Clear();
                txtObservacoes.Clear();

                txtBitola.Focus();
            }
        }

        private void txtBitola_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtBitola.Text.Trim() != "")
            {
                txtBitola.Text = txtBitola.Text.ToUpper();
                txtBitola.SelectionStart = txtBitola.Text.Length;
                txtBitola.SelectionLength = 0;
            }

            AtualizaDescricao();
        }

        private void txtBitola_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBitola.SelectAll();
        }

        private void txtBitola_LostFocus(object sender, RoutedEventArgs e)
        {
            AtualizaDescricao();
        }

        private void btnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable materiasprimas = new DataTable();

            string queryMateriasPrimas =
                "SELECT MATERIAPRIMA.ID, " +
                "       MATERIAPRIMA.IDTIPOMATERIAPRIMA IDTIPO, " +
                "       TIPOMATERIAPRIMA.DESCRICAO TIPO, " +
                "       TIPOMATERIAPRIMA.UNIDADE, " +
                "       MATERIAPRIMA.IDSUBTIPOMATERIAPRIMA IDSUBTIPO, " +
                "       SUBTIPOMATERIAPRIMA.DESCRICAO SUBTIPO, " +
                "       MATERIAPRIMA.DESCRICAO, " +
                "       MATERIAPRIMA.BITOLA, " +
                "       MATERIAPRIMA.PERDA, " +
                "       MATERIAPRIMA.OBSERVACOES " +
                "  FROM MATERIAPRIMA, " +
                "       TIPOMATERIAPRIMA, " +
                "       SUBTIPOMATERIAPRIMA " +
                " WHERE TIPOMATERIAPRIMA.ID = MATERIAPRIMA.IDTIPOMATERIAPRIMA AND " +
                "       SUBTIPOMATERIAPRIMA.ID = MATERIAPRIMA.IDSUBTIPOMATERIAPRIMA ";

            int selTipoMateriaPrima = comboTipoMateriaPrima.SelectedIndex;
            int selSubTipoMateriaPrima = comboSubTipoMateriaPrima.SelectedIndex;

            if (selTipoMateriaPrima >= 0)
            {
                DataRowView result = (DataRowView)comboTipoMateriaPrima.SelectedItem;
                string tipomateriaprima = result.Row[0].ToString();

                queryMateriasPrimas += "AND MATERIAPRIMA.IDTIPOMATERIAPRIMA = " + tipomateriaprima + " ";
            }

            if (selSubTipoMateriaPrima >= 0)
            {
                DataRowView result = (DataRowView)comboSubTipoMateriaPrima.SelectedItem;
                string subtipomateriaprima = result.Row[0].ToString();

                queryMateriasPrimas += "AND MATERIAPRIMA.IDSUBTIPOMATERIAPRIMA = " + subtipomateriaprima + " ";
            }

            if (sqlite.Connect())
            {
                materiasprimasList.Clear();

                materiasprimas = sqlite.GetTable(queryMateriasPrimas);

                if (materiasprimas.Rows.Count > 0)
                {
                    foreach (DataRow row in materiasprimas.Rows)
                    {
                        MateriaPrima materiaprima = new MateriaPrima();

                        materiaprima.ID = Convert.ToInt64(row[0]);
                        materiaprima.SubTipo.Tipo.ID = Convert.ToInt64(row[1]);
                        materiaprima.SubTipo.Tipo.Descricao = Convert.ToString(row[2]);
                        materiaprima.SubTipo.Tipo.Unidade = Convert.ToString(row[3]);
                        materiaprima.SubTipo.ID = Convert.ToInt64(row[4]);
                        materiaprima.SubTipo.Descricao = Convert.ToString(row[5]);
                        materiaprima.Descricao = Convert.ToString(row[6]);
                        materiaprima.Bitola = Convert.ToString(row[7]);
                        materiaprima.Perda = Convert.ToInt32(row[8]);
                        materiaprima.Observacoes = Convert.ToString(row[9]);

                        materiasprimasList.Add(materiaprima);
                    }
                }

                gridDados.ItemsSource = materiasprimasList;

                sqlite.Disconnect();
                sqlite = null;
            }

            btnCancelarFiltro.IsEnabled = true;

            txtBitola.Focus();
        }

        private void btnCancelarFiltro_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable materiasprimas = new DataTable();

            string queryMateriasPrimas =
                "SELECT MATERIAPRIMA.ID, " +
                "       MATERIAPRIMA.IDTIPOMATERIAPRIMA IDTIPO, " +
                "       TIPOMATERIAPRIMA.DESCRICAO TIPO, " +
                "       TIPOMATERIAPRIMA.UNIDADE, " +
                "       MATERIAPRIMA.IDSUBTIPOMATERIAPRIMA IDSUBTIPO, " +
                "       SUBTIPOMATERIAPRIMA.DESCRICAO SUBTIPO, " +
                "       MATERIAPRIMA.DESCRICAO, " +
                "       MATERIAPRIMA.BITOLA, " +
                "       MATERIAPRIMA.PERDA, " +
                "       MATERIAPRIMA.OBSERVACOES " +
                "  FROM MATERIAPRIMA, " +
                "       TIPOMATERIAPRIMA, " +
                "       SUBTIPOMATERIAPRIMA " +
                " WHERE TIPOMATERIAPRIMA.ID = MATERIAPRIMA.IDTIPOMATERIAPRIMA AND " +
                "       SUBTIPOMATERIAPRIMA.ID = MATERIAPRIMA.IDSUBTIPOMATERIAPRIMA";

            comboTipoMateriaPrima.SelectedIndex = -1;
            comboSubTipoMateriaPrima.ItemsSource = null;

            if (sqlite.Connect())
            {
                materiasprimasList.Clear();

                materiasprimas = sqlite.GetTable(queryMateriasPrimas);

                if (materiasprimas.Rows.Count > 0)
                {
                    foreach (DataRow row in materiasprimas.Rows)
                    {
                        MateriaPrima materiaprima = new MateriaPrima();

                        materiaprima.ID = Convert.ToInt64(row[0]);
                        materiaprima.SubTipo.Tipo.ID = Convert.ToInt64(row[1]);
                        materiaprima.SubTipo.Tipo.Descricao = Convert.ToString(row[2]);
                        materiaprima.SubTipo.Tipo.Unidade = Convert.ToString(row[3]);
                        materiaprima.SubTipo.ID = Convert.ToInt64(row[4]);
                        materiaprima.SubTipo.Descricao = Convert.ToString(row[5]);
                        materiaprima.Descricao = Convert.ToString(row[6]);
                        materiaprima.Bitola = Convert.ToString(row[7]);
                        materiaprima.Perda = Convert.ToInt32(row[8]);
                        materiaprima.Observacoes = Convert.ToString(row[9]);

                        materiasprimasList.Add(materiaprima);
                    }
                }

                gridDados.ItemsSource = materiasprimasList;

                sqlite.Disconnect();
                sqlite = null;
            }

            btnCancelarFiltro.IsEnabled = false;
            btnFiltrar.IsEnabled = false;
            
            txtBitola.Focus();
        }

        private void txtObservacoes_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
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
