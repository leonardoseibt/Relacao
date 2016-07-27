using Relacao.Classes;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for CadParticularidade.xaml
    /// </summary>
    public partial class CadParticularidade : Window
    {
        private ObsCollection<Particularidade> particularidadesList = new ObsCollection<Particularidade>();
        private Particularidade particularidadeOriginal = new Particularidade();

        public CadParticularidade()
        {
            InitializeComponent();
        }

        private void Insert_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!txtDescricao.Text.Trim().Equals("") && !comboTipoProduto.SelectedIndex.Equals(-1))
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
            Particularidade particularidade = new Particularidade();

            long tipoID = Convert.ToInt64(comboTipoProduto.SelectedValue);

            particularidade.Tipo = sqlite.GetTipoProduto(tipoID);
            particularidade.Descricao = txtDescricao.Text.Trim().ToUpper();

            if (txtBtnInserir.Text.Equals("Inserir"))
            {
                if (!sqlite.ExistParticularidade(particularidade))
                {
                    InsertParticularidade(particularidade);
                }
                else
                {
                    MessageBox.Show("A Particularidade Informada Já Existe no Cadastro",
                    "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (txtBtnInserir.Text.Equals("Salvar"))
            {
                Particularidade particularidadeAlterada = new Particularidade();
                particularidadeAlterada = particularidade;

                particularidade.ID = ((Particularidade)gridDados.SelectedItem).ID;

                if (particularidadeAlterada.Tipo.ID != particularidadeOriginal.Tipo.ID ||
                    particularidadeAlterada.Descricao != particularidadeOriginal.Descricao)
                {
                    if (!sqlite.ExistParticularidade(particularidade))
                    {
                        UpdateParticularidade(particularidade);
                    }
                    else
                    {
                        MessageBox.Show("A Particularidade Informada Já Existe no Cadastro",
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    UpdateParticularidade(particularidade);
                }
            }

            txtDescricao.Focus();
        }

        private void UpdateParticularidade(Particularidade particularidade)
        {
            SQLite sqlite = new SQLite();

            string query = "UPDATE PARTICULARIDADE SET DESCRICAO='" + particularidade.Descricao + "', IDTIPOPRODUTO=" +
                particularidade.Tipo.ID.ToString() + " WHERE ID=" + particularidade.ID;

            if (sqlite.Connect())
            {
                if (sqlite.UpdateQuery(query))
                {
                    gridDados.IsEnabled = true;
                    ChangeItemValues(particularidadesList, particularidade);
                    gridDados.Items.Refresh();
                    gridDados.ScrollIntoView(particularidade);

                    txtBtnInserir.Text = "Inserir";
                    comboTipoProduto.SelectedIndex = -1;
                    txtDescricao.Clear();
                }

                sqlite.Disconnect();
                sqlite = null;
            }

        }

        private void InsertParticularidade(Particularidade particularidade)
        {
            SQLite sqlite = new SQLite();

            string query = "INSERT INTO PARTICULARIDADE (IDTIPOPRODUTO,DESCRICAO) VALUES (" +
                particularidade.Tipo.ID.ToString() + ",'" + particularidade.Descricao + "')";

            if (sqlite.Connect())
            {
                particularidade.ID = sqlite.InsertQuery(query, "PARTICULARIDADE");

                particularidadesList.Add(particularidade);
                particularidadesList.UpdateCollection();
                gridDados.Items.Refresh();
                gridDados.ScrollIntoView(particularidade);

                comboTipoProduto.SelectedIndex = -1;
                txtDescricao.Clear();

                sqlite.Disconnect();
                sqlite = null;
            }

        }

        private void ChangeItemValues(ObsCollection<Particularidade> particularidadesList, Particularidade particularidade)
        {
            foreach (Particularidade item in particularidadesList)
            {
                if (item.ID.Equals(particularidade.ID))
                {
                    item.Descricao = particularidade.Descricao;
                    item.Tipo = particularidade.Tipo;
                }
            }
        }

        private void txtDescricao_GotFocus(object sender, RoutedEventArgs e)
        {
            txtDescricao.SelectAll();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable tipos = new DataTable();
            DataTable particularidades = new DataTable();

            string queryTipos =
                "SELECT ID,DESCRICAO FROM TIPOPRODUTO ORDER BY DESCRICAO";
            string queryParticularidades =
                "SELECT PARTICULARIDADE.ID, " +
                "       PARTICULARIDADE.IDTIPOPRODUTO, " +
                "       TIPOPRODUTO.DESCRICAO, " +
                "       PARTICULARIDADE.DESCRICAO " +
                "  FROM PARTICULARIDADE, " +
                "       TIPOPRODUTO " +
                " WHERE PARTICULARIDADE.IDTIPOPRODUTO = TIPOPRODUTO.ID";

            if (sqlite.Connect())
            {
                tipos = sqlite.GetTable(queryTipos);
                particularidades = sqlite.GetTable(queryParticularidades);

                comboTipoProduto.ItemsSource = tipos.DefaultView;

                if (particularidades.Rows.Count > 0)
                {
                    foreach (DataRow row in particularidades.Rows)
                    {
                        Particularidade particularidade = new Particularidade();

                        particularidade.ID = (long)row[0];
                        particularidade.Tipo.ID = (long)row[1];
                        particularidade.Tipo.Descricao = (string)row[2];
                        particularidade.Descricao = (string)row[3];

                        particularidadesList.Add(particularidade);
                    }
                }

                gridDados.ItemsSource = particularidadesList;

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
                comboTipoProduto.SelectedIndex = -1;
                txtDescricao.Clear();

                txtDescricao.Focus();
            }
        }

        private void menuAlterar_Click(object sender, RoutedEventArgs e)
        {
            Particularidade particularidade = new Particularidade();
            particularidade = (Particularidade)gridDados.SelectedItem;

            txtBtnInserir.Text = "Salvar";

            comboTipoProduto.SelectedValue = particularidade.Tipo.ID;
            txtDescricao.Text = particularidade.Descricao;

            gridDados.IsEnabled = false;

            particularidadeOriginal = particularidade;

            txtDescricao.Focus();
        }

        private void menuExcluir_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            Particularidade particularidade = new Particularidade();
            string query;

            particularidade = (Particularidade)gridDados.SelectedItem;
            query = "DELETE FROM PARTICULARIDADE WHERE ID=" + particularidade.ID;

            if (MessageBox.Show("Confirma exclusão?\n" + particularidade.ID.ToString() + " - " + particularidade.Tipo.Descricao + " - " + particularidade.Descricao,
                "Confirmação de Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (sqlite.Connect())
                {
                    if (sqlite.DeleteQuery(query))
                    {
                        particularidadesList.Remove(particularidade);
                        gridDados.Items.Refresh();

                        comboTipoProduto.SelectedIndex = -1;
                        txtDescricao.Clear();
                    }

                    sqlite.Disconnect();
                    sqlite = null;
                }

                txtDescricao.Focus();
            }
        }

        private void btnInserirPARTIC_Click(object sender, RoutedEventArgs e)
        {
            CadTipoProduto formulario = new CadTipoProduto();
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
        }

        private void comboTipoProduto_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            btnFiltrar.IsEnabled = true;
        }

        private void btnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable particularidades = new DataTable();

            string queryParticularidades =
                "SELECT PARTICULARIDADE.ID, " +
                "       PARTICULARIDADE.IDTIPOPRODUTO, " +
                "       TIPOPRODUTO.DESCRICAO, " +
                "       PARTICULARIDADE.DESCRICAO " +
                "  FROM PARTICULARIDADE, " +
                "       TIPOPRODUTO " +
                " WHERE PARTICULARIDADE.IDTIPOPRODUTO = TIPOPRODUTO.ID ";

            int selTipoProduto = comboTipoProduto.SelectedIndex;
            
            if (selTipoProduto >= 0)
            {
                DataRowView result = (DataRowView)comboTipoProduto.SelectedItem;
                string tipoproduto = result.Row[0].ToString();

                queryParticularidades += "AND PARTICULARIDADE.IDTIPOPRODUTO = " + tipoproduto + " ";
            }

            if (sqlite.Connect())
            {
                particularidadesList.Clear();

                particularidades = sqlite.GetTable(queryParticularidades);

                if (particularidades.Rows.Count > 0)
                {
                    foreach (DataRow row in particularidades.Rows)
                    {
                        Particularidade particularidade = new Particularidade();

                        particularidade.ID = (long)row[0];
                        particularidade.Tipo.ID = (long)row[1];
                        particularidade.Tipo.Descricao = (string)row[2];
                        particularidade.Descricao = (string)row[3];

                        particularidadesList.Add(particularidade);
                    }
                }

                gridDados.ItemsSource = particularidadesList;

                sqlite.Disconnect();
                sqlite = null;
            }

            btnCancelarFiltro.IsEnabled = true;

            txtDescricao.Focus();
        }

        private void btnCancelarFiltro_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable particularidades = new DataTable();

            string queryParticularidades =
                "SELECT PARTICULARIDADE.ID, " +
                "       PARTICULARIDADE.IDTIPOPRODUTO, " +
                "       TIPOPRODUTO.DESCRICAO, " +
                "       PARTICULARIDADE.DESCRICAO " +
                "  FROM PARTICULARIDADE, " +
                "       TIPOPRODUTO " +
                " WHERE PARTICULARIDADE.IDTIPOPRODUTO = TIPOPRODUTO.ID";

            comboTipoProduto.SelectedIndex = -1;

            if (sqlite.Connect())
            {
                particularidadesList.Clear();

                particularidades = sqlite.GetTable(queryParticularidades);

                if (particularidades.Rows.Count > 0)
                {
                    foreach (DataRow row in particularidades.Rows)
                    {
                        Particularidade particularidade = new Particularidade();

                        particularidade.ID = (long)row[0];
                        particularidade.Tipo.ID = (long)row[1];
                        particularidade.Tipo.Descricao = (string)row[2];
                        particularidade.Descricao = (string)row[3];

                        particularidadesList.Add(particularidade);
                    }
                }

                gridDados.ItemsSource = particularidadesList;

                sqlite.Disconnect();
                sqlite = null;
            }

            btnCancelarFiltro.IsEnabled = false;
            btnFiltrar.IsEnabled = false;

            txtDescricao.Focus();
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
