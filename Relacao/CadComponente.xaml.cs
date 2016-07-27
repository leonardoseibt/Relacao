using Relacao.Classes;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for CadComponente.xaml
    /// </summary>
    public partial class CadComponente : Window
    {
        private ObsCollection<Componente> componentesList = new ObsCollection<Componente>();
        private Componente componenteOriginal = new Componente();

        public CadComponente()
        {
            InitializeComponent();
        }

        private void Insert_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!comboTipoComponente.SelectedIndex.Equals(-1) &&
                !comboMateriaPrima.SelectedIndex.Equals(-1) &&
                !txtComprimento.Value.Equals(null) &&
                !txtLargura.Value.Equals(null) &&
                !txtEspessura.Value.Equals(null))
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
            Componente componente = new Componente();

            long tipoID = (long)comboTipoComponente.SelectedValue;
            long materiaprimaID = (long)comboMateriaPrima.SelectedValue;

            componente.Tipo = sqlite.GetTipoComponente(tipoID);
            componente.MateriaPrima = sqlite.GetMateriaPrima(materiaprimaID);
            componente.Comprimento = (int)txtComprimento.Value;
            componente.Largura = (int)txtLargura.Value;
            componente.Espessura = (int)txtEspessura.Value;
            componente.Codigo = txtCodigo.Text.ToString().Trim();

            if (txtBtnInserir.Text.Equals("Inserir"))
            {
                if (!sqlite.ExistComponente(componente))
                {
                    InsertComponente(componente);
                }
                else
                {
                    MessageBox.Show("O Componente Informado Já Existe no Cadastro",
                    "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (txtBtnInserir.Text.Equals("Salvar"))
            {
                Componente componenteAlterado = new Componente();
                componenteAlterado.Tipo = componente.Tipo;
                componenteAlterado.MateriaPrima = componente.MateriaPrima;
                componenteAlterado.Comprimento = componente.Comprimento;
                componenteAlterado.Largura = componente.Largura;
                componenteAlterado.Espessura = componente.Espessura;
                componenteAlterado.Codigo = componente.Codigo;

                componente.ID = ((Componente)gridDados.SelectedItem).ID;

                if (componenteAlterado.Tipo.ID != componenteOriginal.Tipo.ID ||
                    componenteAlterado.MateriaPrima.ID != componenteOriginal.MateriaPrima.ID ||
                    componenteAlterado.Codigo != componenteOriginal.Codigo ||
                    componenteAlterado.Comprimento != componenteOriginal.Comprimento ||
                    componenteAlterado.Largura != componenteOriginal.Largura ||
                    componenteAlterado.Espessura != componenteOriginal.Espessura)
                {
                    if (!sqlite.ExistComponente(componente))
                    {
                        UpdateComponente(componente);
                    }
                    else
                    {
                        MessageBox.Show("O Componente Informado Já Existe no Cadastro",
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    UpdateComponente(componente);
                }
            }

            txtComprimento.Focus();
        }

        private void UpdateComponente(Componente componente)
        {
            SQLite sqlite = new SQLite();

            string query = "UPDATE COMPONENTE SET " +
                "IDTIPOCOMPONENTE=" + componente.Tipo.ID.ToString() + "," +
                "IDMATERIAPRIMA=" + componente.MateriaPrima.ID.ToString() + "," +
                "CODIGO='" + componente.Codigo + "'," +
                "COMPRIMENTO=" + componente.Comprimento + "," +
                "LARGURA=" + componente.Largura + "," +
                "ESPESSURA=" + componente.Espessura + " " +
                "WHERE ID=" + componente.ID;

            if (sqlite.Connect())
            {
                if (sqlite.UpdateQuery(query))
                {
                    gridDados.IsEnabled = true;
                    ChangeItemValues(componentesList, componente);
                    gridDados.Items.Refresh();
                    gridDados.ScrollIntoView(componente);

                    txtBtnInserir.Text = "Inserir";

                    comboTipoComponente.SelectedIndex = -1;
                    comboMateriaPrima.SelectedIndex = -1;
                    txtComprimento.Value = null;
                    txtLargura.Value = null;
                    txtEspessura.Value = null;
                }

                sqlite.Disconnect();
                sqlite = null;
            }

        }

        private void InsertComponente(Componente componente)
        {
            SQLite sqlite = new SQLite();

            string query = "INSERT INTO COMPONENTE (" +
                "IDTIPOCOMPONENTE," +
                "IDMATERIAPRIMA," +
                "CODIGO," +
                "COMPRIMENTO," +
                "LARGURA," +
                "ESPESSURA) VALUES (" +
                componente.Tipo.ID.ToString() + "," +
                componente.MateriaPrima.ID.ToString() + ",'" +
                componente.Codigo + "'," +
                componente.Comprimento + "," +
                componente.Largura + "," +
                componente.Espessura + ")";

            if (sqlite.Connect())
            {
                componente.ID = sqlite.InsertQuery(query, "COMPONENTE");

                componentesList.Add(componente);
                componentesList.UpdateCollection();
                gridDados.Items.Refresh();
                gridDados.ScrollIntoView(componente);

                comboTipoComponente.SelectedIndex = -1;
                comboMateriaPrima.SelectedIndex = -1;
                txtComprimento.Value = null;
                txtLargura.Value = null;
                txtEspessura.Value = null;

                sqlite.Disconnect();
                sqlite = null;
            }
        }

        private void ChangeItemValues(ObsCollection<Componente> componentesList, Componente componente)
        {
            foreach (Componente item in componentesList)
            {
                if (item.ID.Equals(componente.ID))
                {
                    item.Tipo.ID = componente.Tipo.ID;
                    item.Tipo.Descricao = componente.Tipo.Descricao;
                    item.MateriaPrima.ID = componente.MateriaPrima.ID;
                    item.MateriaPrima.Descricao = componente.MateriaPrima.Descricao;
                    item.Codigo = componente.Codigo;
                    item.Comprimento = componente.Comprimento;
                    item.Largura = componente.Largura;
                    item.Espessura = componente.Espessura;
                }
            }
        }

        private void btnInserirTPCOMP_Click(object sender, RoutedEventArgs e)
        {
            CadTipoComponente formulario = new CadTipoComponente();
            formulario.ShowDialog();

            SQLite sqlite = new SQLite();
            DataTable tipos = new DataTable();

            string queryTipos = "SELECT ID,DESCRICAO FROM TIPOCOMPONENTE ORDER BY DESCRICAO";

            if (sqlite.Connect())
            {
                tipos = sqlite.GetTable(queryTipos);
                comboTipoComponente.ItemsSource = tipos.DefaultView;

                sqlite.Disconnect();
                sqlite = null;
            }
        }

        private void btnInserirMP_Click(object sender, RoutedEventArgs e)
        {
            CadMateriaPrima formulario = new CadMateriaPrima();
            formulario.ShowDialog();

            SQLite sqlite = new SQLite();
            DataTable materiasprimas = new DataTable();

            string queryTipos = "SELECT ID,DESCRICAO FROM MATERIAPRIMA ORDER BY DESCRICAO";

            if (sqlite.Connect())
            {
                materiasprimas = sqlite.GetTable(queryTipos);
                comboMateriaPrima.ItemsSource = materiasprimas.DefaultView;

                sqlite.Disconnect();
                sqlite = null;
            }
        }

        private void menuAlterar_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            Componente componente = new Componente();
            componente = (Componente)gridDados.SelectedItem;

            if (sqlite.ComponenteIsUsed(componente.ID) > 1)
            {
                if (MessageBox.Show("Este Componente é usado em mais de um Produto.\n" +
                    "Esta alteração irá afetar todos os Produtos que o utilizam.\n\n" +
                    "Confirma a alteração mesmo assim?",
                    "Confirmação de Alteração", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    return;
                }
            }

            txtBtnInserir.Text = "Salvar";

            comboTipoComponente.SelectedValue = componente.Tipo.ID;
            comboMateriaPrima.SelectedValue = componente.MateriaPrima.ID;
            txtComprimento.Value = componente.Comprimento;
            txtLargura.Value = componente.Largura;
            txtEspessura.Value = componente.Espessura;
            txtCodigo.Text = componente.Codigo;

            gridDados.IsEnabled = false;

            componenteOriginal = componente;

            txtComprimento.Focus();
        }

        private void menuExcluir_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            Componente componente = new Componente();
            string query;

            componente = (Componente)gridDados.SelectedItem;

            if (sqlite.ComponenteIsUsed(componente.ID) > 0)
            {
                MessageBox.Show("Exclusão Não Permitida.\nEste Componente é Usado em Pelo Menos um Produto.",
                    "Problemas na Exclusão", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                query = "DELETE FROM COMPONENTE WHERE ID=" + componente.ID;

                if (MessageBox.Show("Confirma Exclusão?\n" + componente.ID.ToString() + " - " + componente.Codigo + " - " + componente.Tipo.Descricao + " - " +
                     componente.MateriaPrima.Descricao + " - " + componente.Comprimento + " X " + componente.Largura + " X " + componente.Espessura,
                    "Confirmação de Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (sqlite.Connect())
                    {
                        if (sqlite.DeleteQuery(query))
                        {
                            componentesList.Remove(componente);
                            gridDados.Items.Refresh();

                            comboTipoComponente.SelectedIndex = -1;
                            comboMateriaPrima.SelectedIndex = -1;
                            txtComprimento.Value = null;
                            txtLargura.Value = null;
                            txtEspessura.Value = null;
                            txtCodigo.Clear();
                        }

                        sqlite.Disconnect();
                        sqlite = null;
                    }
                }
            }

            txtComprimento.Focus();
        }

        public static bool OnlyNumeric(string text)
        {
            Regex regex;

            //if (unidade == "Milimetro")
            regex = new Regex("[^0-9]+");
            //else
            //    regex = new Regex("[^0-9,]+");

            return !regex.IsMatch(text);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable tipos = new DataTable();
            DataTable materiasprimas = new DataTable();
            DataTable componentes = new DataTable();

            string queryTipos =
                "SELECT ID,DESCRICAO FROM TIPOCOMPONENTE ORDER BY DESCRICAO";
            string queryMateriasPrimas =
                "SELECT ID,DESCRICAO FROM MATERIAPRIMA ORDER BY DESCRICAO";
            string queryComponentes =
                "SELECT COMPONENTE.ID, " +
                "       COMPONENTE.IDTIPOCOMPONENTE IDTIPO, " +
                "       TIPOCOMPONENTE.DESCRICAO TIPO, " +
                "       COMPONENTE.IDMATERIAPRIMA IDMATERIAPRIMA, " +
                "       MATERIAPRIMA.DESCRICAO MATERIAPRIMA, " +
                "       COMPONENTE.COMPRIMENTO, " +
                "       COMPONENTE.LARGURA, " +
                "       COMPONENTE.ESPESSURA, " +
                "       IFNULL(COMPONENTE.CODIGO,'') CODIGO " +
                "  FROM COMPONENTE, " +
                "       TIPOCOMPONENTE, " +
                "       MATERIAPRIMA " +
                " WHERE TIPOCOMPONENTE.ID = COMPONENTE.IDTIPOCOMPONENTE AND " +
                "       MATERIAPRIMA.ID = COMPONENTE.IDMATERIAPRIMA";

            if (sqlite.Connect())
            {
                tipos = sqlite.GetTable(queryTipos);
                materiasprimas = sqlite.GetTable(queryMateriasPrimas);
                componentes = sqlite.GetTable(queryComponentes);

                comboTipoComponente.ItemsSource = tipos.DefaultView;
                comboMateriaPrima.ItemsSource = materiasprimas.DefaultView;

                if (componentes.Rows.Count > 0)
                {
                    foreach (DataRow row in componentes.Rows)
                    {
                        Componente componente = new Componente();

                        componente.ID = (long)row[0];
                        componente.Tipo.ID = (long)row[1];
                        componente.Tipo.Descricao = row[2].ToString();
                        componente.MateriaPrima.ID = (long)row[3];
                        componente.MateriaPrima.Descricao = row[4].ToString();
                        componente.Comprimento = Convert.ToInt32(row[5]);
                        componente.Largura = Convert.ToInt32(row[6]);
                        componente.Espessura = Convert.ToInt32(row[7]);
                        componente.Codigo = row[8].ToString();

                        componentesList.Add(componente);
                    }
                }

                gridDados.ItemsSource = componentesList;

                sqlite.Disconnect();
                sqlite = null;
            }

            txtComprimento.Focus();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                gridDados.IsEnabled = true;

                txtBtnInserir.Text = "Inserir";

                comboTipoComponente.SelectedIndex = -1;
                comboMateriaPrima.SelectedIndex = -1;
                txtComprimento.Value = txtComprimento.DefaultValue;
                txtLargura.Value = txtLargura.DefaultValue;
                txtEspessura.Value = txtEspessura.DefaultValue;
                txtCodigo.Clear();

                txtComprimento.Focus();
            }
        }

        private void txtComprimento_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !OnlyNumeric(e.Text);
        }

        private void txtLargura_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !OnlyNumeric(e.Text);
        }

        private void txtEspessura_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !OnlyNumeric(e.Text);
        }

        private void menuWhereUsed_Click(object sender, RoutedEventArgs e)
        {
            WindowWhereUsed formulario = new WindowWhereUsed();
            Componente componente = new Componente();

            componente = (Componente)gridDados.SelectedItem;
            formulario.ComponenteID = componente.ID;

            formulario.ShowDialog();
        }

        private void txtCodigo_GotFocus(object sender, RoutedEventArgs e)
        {
            txtCodigo.SelectAll();
        }

        private void btnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable componentes = new DataTable();

            string queryComponentes =
                "SELECT COMPONENTE.ID, " +
                "       COMPONENTE.IDTIPOCOMPONENTE IDTIPO, " +
                "       TIPOCOMPONENTE.DESCRICAO TIPO, " +
                "       COMPONENTE.IDMATERIAPRIMA IDMATERIAPRIMA, " +
                "       MATERIAPRIMA.DESCRICAO MATERIAPRIMA, " +
                "       COMPONENTE.COMPRIMENTO, " +
                "       COMPONENTE.LARGURA, " +
                "       COMPONENTE.ESPESSURA, " +
                "       IFNULL(COMPONENTE.CODIGO,'') CODIGO " +
                "  FROM COMPONENTE, " +
                "       TIPOCOMPONENTE, " +
                "       MATERIAPRIMA " +
                " WHERE TIPOCOMPONENTE.ID = COMPONENTE.IDTIPOCOMPONENTE AND " +
                "       MATERIAPRIMA.ID = COMPONENTE.IDMATERIAPRIMA ";

            int selTipoComponente = comboTipoComponente.SelectedIndex;
            int selMateriaPrima = comboMateriaPrima.SelectedIndex;
            
            if (selTipoComponente >= 0)
            {
                DataRowView result = (DataRowView)comboTipoComponente.SelectedItem;
                string tipocomponente = result.Row[0].ToString();

                queryComponentes += "AND COMPONENTE.IDTIPOCOMPONENTE = " + tipocomponente + " ";
            }

            if (selMateriaPrima >= 0)
            {
                DataRowView result = (DataRowView)comboMateriaPrima.SelectedItem;
                string materiaprima = result.Row[0].ToString();

                queryComponentes += "AND COMPONENTE.IDMATERIAPRIMA = " + materiaprima + " ";
            }

            if (sqlite.Connect())
            {
                componentesList.Clear();

                componentes = sqlite.GetTable(queryComponentes);

                if (componentes.Rows.Count > 0)
                {
                    foreach (DataRow row in componentes.Rows)
                    {
                        Componente componente = new Componente();

                        componente.ID = (long)row[0];
                        componente.Tipo.ID = (long)row[1];
                        componente.Tipo.Descricao = row[2].ToString();
                        componente.MateriaPrima.ID = (long)row[3];
                        componente.MateriaPrima.Descricao = row[4].ToString();
                        componente.Comprimento = Convert.ToInt32(row[5]);
                        componente.Largura = Convert.ToInt32(row[6]);
                        componente.Espessura = Convert.ToInt32(row[7]);
                        componente.Codigo = row[8].ToString();

                        componentesList.Add(componente);
                    }
                }

                gridDados.ItemsSource = componentesList;

                sqlite.Disconnect();
                sqlite = null;
            }

            btnCancelarFiltro.IsEnabled = true;

            txtComprimento.Focus();
        }

        private void btnCancelarFiltro_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable componentes = new DataTable();

            string queryComponentes =
                "SELECT COMPONENTE.ID, " +
                "       COMPONENTE.IDTIPOCOMPONENTE IDTIPO, " +
                "       TIPOCOMPONENTE.DESCRICAO TIPO, " +
                "       COMPONENTE.IDMATERIAPRIMA IDMATERIAPRIMA, " +
                "       MATERIAPRIMA.DESCRICAO MATERIAPRIMA, " +
                "       COMPONENTE.COMPRIMENTO, " +
                "       COMPONENTE.LARGURA, " +
                "       COMPONENTE.ESPESSURA, " +
                "       IFNULL(COMPONENTE.CODIGO,'') CODIGO " +
                "  FROM COMPONENTE, " +
                "       TIPOCOMPONENTE, " +
                "       MATERIAPRIMA " +
                " WHERE TIPOCOMPONENTE.ID = COMPONENTE.IDTIPOCOMPONENTE AND " +
                "       MATERIAPRIMA.ID = COMPONENTE.IDMATERIAPRIMA";

            comboTipoComponente.SelectedIndex = -1;
            comboMateriaPrima.SelectedIndex = -1;

            if (sqlite.Connect())
            {
                componentesList.Clear();

                componentes = sqlite.GetTable(queryComponentes);

                if (componentes.Rows.Count > 0)
                {
                    foreach (DataRow row in componentes.Rows)
                    {
                        Componente componente = new Componente();

                        componente.ID = (long)row[0];
                        componente.Tipo.ID = (long)row[1];
                        componente.Tipo.Descricao = row[2].ToString();
                        componente.MateriaPrima.ID = (long)row[3];
                        componente.MateriaPrima.Descricao = row[4].ToString();
                        componente.Comprimento = Convert.ToInt32(row[5]);
                        componente.Largura = Convert.ToInt32(row[6]);
                        componente.Espessura = Convert.ToInt32(row[7]);
                        componente.Codigo = row[8].ToString();

                        componentesList.Add(componente);
                    }
                }

                gridDados.ItemsSource = componentesList;

                sqlite.Disconnect();
                sqlite = null;
            }

            btnCancelarFiltro.IsEnabled = false;
            btnFiltrar.IsEnabled = false;

            txtComprimento.Focus();
        }

        private void comboTipoComponente_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            btnFiltrar.IsEnabled = true;
        }

        private void comboMateriaPrima_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            btnFiltrar.IsEnabled = true;
        }

        private void txtCodigo_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtCodigo.Text.Trim() != "")
            {
                int cursorPos = txtCodigo.SelectionStart;

                txtCodigo.Text = txtCodigo.Text.ToUpper();
                txtCodigo.SelectionStart = cursorPos;
                txtCodigo.SelectionLength = 0;
            }
        }

    }
}
