using Relacao.Classes;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for CadFichaTecnica.xaml
    /// </summary>
    public partial class CadFichaTecnica : Window
    {
        internal Produto Produto { get; set; }

        private ObsCollection<FichaTecnica> fichatecnicaList = new ObsCollection<FichaTecnica>();

        private ArrayList itensAgrupamento = new ArrayList();
        private string xmlAgrupamento = "Agrupamento.xml";
        private XmlDocument xmlDoc = new XmlDocument();
        private string xmlPath;

        public CadFichaTecnica()
        {
            InitializeComponent();
        }

        private void Buscar_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (txtReferencia.Text.Trim().Count() > 0)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void Buscar_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            BuscarProdutoExecuted();
        }

        private void BuscarProdutoExecuted()
        {
            fichatecnicaList.Clear();

            SQLite sqlite = new SQLite();
            DataTable table = new DataTable();
            Produto produto = new Produto();

            string corComp = "";
            string corLarg = "";
            string queryFichaTecnica;
            string referencia = txtReferencia.Text.Trim();
            int produtoID = sqlite.GetIDByReferencia(referencia);

            if (produtoID > 0)
            {
                produto = sqlite.GetProdutoByID(produtoID);

                txtID.Text = produto.ID.ToString();
                txtDescricao.Text = produto.Descricao;

                queryFichaTecnica =
                    "  SELECT FICHATECNICA.ID, " +
                    "         FICHATECNICA.IDPRODUTO, " +
                    "         PRODUTO.DESCRICAO PRODUTO, " +
                    "         FICHATECNICA.QUANTIDADE, " +
                    "         FICHATECNICA.IDCOMPONENTE, " +
                    "         COMPONENTE.IDTIPOCOMPONENTE, " +
                    "         TIPOCOMPONENTE.DESCRICAO TIPO, " +
                    "         COMPONENTE.COMPRIMENTO, " +
                    "         COMPONENTE.LARGURA, " +
                    "         COMPONENTE.ESPESSURA, " +
                    "         COMPONENTE.IDMATERIAPRIMA, " +
                    "         MATERIAPRIMA.DESCRICAO MATERIAPRIMA, " +
                    "         FICHATECNICA.LIXADA, " +
                    "         FICHATECNICA.APROVEITAMENTO, " +
                    "         FICHATECNICA.OBSERVACOES, " +
                    "         COMPONENTE.CODIGO, " +
                    "         FICHATECNICA.AGRUPAMENTO, " +
                    "         FICHATECNICA.CORCOMPRIMENTO, " +
                    "         FICHATECNICA.CORLARGURA " +
                    "    FROM FICHATECNICA, " +
                    "         PRODUTO, " +
                    "         COMPONENTE, " +
                    "         TIPOCOMPONENTE, " +
                    "         MATERIAPRIMA " +
                    "   WHERE PRODUTO.ID = FICHATECNICA.IDPRODUTO AND " +
                    "         COMPONENTE.ID = FICHATECNICA.IDCOMPONENTE AND " +
                    "         TIPOCOMPONENTE.ID = COMPONENTE.IDTIPOCOMPONENTE AND " +
                    "         MATERIAPRIMA.ID = COMPONENTE.IDMATERIAPRIMA AND " +
                    "         FICHATECNICA.IDPRODUTO=" + produtoID + " " +
                    "ORDER BY TIPOCOMPONENTE.DESCRICAO, " +
                    "         COMPONENTE.ESPESSURA, " +
                    "         COMPONENTE.LARGURA, " +
                    "         COMPONENTE.COMPRIMENTO";

                if (sqlite.Connect())
                {
                    table = sqlite.GetTable(queryFichaTecnica);

                    if (table.Rows.Count > 0)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            FichaTecnica fichatecnica = new FichaTecnica();

                            fichatecnica.ID = (long)row[0];
                            fichatecnica.Produto.ID = (long)row[1];
                            fichatecnica.Produto.Descricao = (string)row[2];
                            fichatecnica.Quantidade = (decimal)row[3];
                            fichatecnica.Componente.ID = (long)row[4];
                            fichatecnica.Componente.Tipo.ID = (long)row[5];
                            fichatecnica.Componente.Tipo.Descricao = (string)row[6];
                            fichatecnica.Componente.Comprimento = Convert.ToInt32(row[7]);
                            fichatecnica.Componente.Largura = Convert.ToInt32(row[8]);
                            fichatecnica.Componente.Espessura = Convert.ToInt32(row[9]);
                            fichatecnica.Componente.MateriaPrima.ID = (long)row[10];
                            fichatecnica.Componente.MateriaPrima.Descricao = (string)row[11];
                            fichatecnica.Lixada = (bool)row[12];
                            fichatecnica.Aproveitamento = (bool)row[13];
                            fichatecnica.Observacoes = (string)row[14];

                            if (row[15] == DBNull.Value)
                                fichatecnica.Componente.Codigo = "";
                            else
                                fichatecnica.Componente.Codigo = (string)row[15];

                            if (row[16] == DBNull.Value)
                                fichatecnica.Agrupamento = "";
                            else
                                fichatecnica.Agrupamento = (string)row[16];

                            corComp = row[17] as string;
                            corLarg = row[18] as string;

                            if (corComp == "Blue")
                                fichatecnica.CorComprimento = Brushes.Blue;
                            else if (corComp == "Orange")
                                fichatecnica.CorComprimento = Brushes.Orange;
                            else
                                fichatecnica.CorComprimento = Brushes.Black;

                            if (corLarg == "Blue")
                                fichatecnica.CorLargura = Brushes.Blue;
                            else if (corLarg == "Orange")
                                fichatecnica.CorLargura = Brushes.Orange;
                            else
                                fichatecnica.CorLargura = Brushes.Black;

                            fichatecnicaList.Add(fichatecnica);
                        }
                    }

                    gridDados.ItemsSource = fichatecnicaList;

                    sqlite.Disconnect();
                    sqlite = null;
                }

                groupInserir.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Não existe um produto cadastrado com a referência informada",
                "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            txtReferencia.Focus();
        }

        private void InserirComponente_CanExecute(object sender, CanExecuteRoutedEventArgs e)
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

        private void InserirComponente_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            FichaTecnica fichatecnica = new FichaTecnica();

            string queryComponente;
            long produtoID = Convert.ToInt64(txtID.Text);
            long tipoID = Convert.ToInt64(comboTipoComponente.SelectedValue);
            long materiaprimaID = Convert.ToInt64(comboMateriaPrima.SelectedValue);

            fichatecnica.Produto.ID = produtoID;
            fichatecnica.Produto.Descricao = txtDescricao.Text;

            fichatecnica.Componente.Tipo = sqlite.GetTipoComponente(tipoID);
            fichatecnica.Componente.MateriaPrima = sqlite.GetMateriaPrima(materiaprimaID);
            fichatecnica.Componente.Comprimento = (int)txtComprimento.Value;
            fichatecnica.Componente.Largura = (int)txtLargura.Value;
            fichatecnica.Componente.Espessura = (int)txtEspessura.Value;
            fichatecnica.Componente.Codigo = txtCodigo.Text.ToString();

            fichatecnica.Quantidade = (Decimal)txtQuantidade.Value;
            fichatecnica.Observacoes = txtObservacoes.Text.Trim().ToUpper();
            //fichatecnica.Aproveitamento = (Boolean)checkAproveitamento.IsChecked;
            fichatecnica.Lixada = (Boolean)checkLixada.IsChecked;
            fichatecnica.Agrupamento = comboFatorAgrup.Text.Trim().ToUpper();

            if (fichatecnica.Agrupamento.Length > 20)
                fichatecnica.Agrupamento = fichatecnica.Agrupamento.Substring(0, 20);

            comboFatorAgrup.Text = fichatecnica.Agrupamento;

            fichatecnica.Componente.ID = CreateOrUseComponente(fichatecnica.Componente);

            if (!fichatecnica.Agrupamento.Equals(String.Empty) && !itensAgrupamento.Contains(fichatecnica.Agrupamento))
            {
                XmlNode root = xmlDoc.DocumentElement;
                XmlElement item = xmlDoc.CreateElement("item");
                item.InnerText = fichatecnica.Agrupamento;
                root.InsertAfter(item, root.LastChild);

                try
                {
                    xmlDoc.Save(xmlPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao salvar no arquivo Agrupamento.xml\n" + ex.ToString(),
                        "Erro de Abertura de Arquivo", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                itensAgrupamento.Add(fichatecnica.Agrupamento);
                itensAgrupamento.Sort();

                comboFatorAgrup.Items.Refresh();
            }

            if (txtBtnInserir.Text.Equals("Inserir"))
            {
                queryComponente = "INSERT INTO FICHATECNICA (" +
                    "IDPRODUTO," +
                    "IDCOMPONENTE," +
                    "QUANTIDADE," +
                    "LIXADA," +
                    "APROVEITAMENTO," +
                    "AGRUPAMENTO," +
                    "CORCOMPRIMENTO," +
                    "CORLARGURA," +
                    "OBSERVACOES) VALUES (" +
                    fichatecnica.Produto.ID.ToString() + "," +
                    fichatecnica.Componente.ID.ToString() + "," +
                    fichatecnica.Quantidade.ToString().Replace(',', '.') + "," +
                    (fichatecnica.Lixada.Equals(true) ? 1 : 0).ToString() + "," +
                    (fichatecnica.Aproveitamento.Equals(true) ? 1 : 0).ToString() + ",'" +
                    fichatecnica.Agrupamento + "','" +
                    "Black','" +
                    "Black','" +
                    fichatecnica.Observacoes + "')";

                if (sqlite.Connect())
                {
                    fichatecnica.ID = sqlite.InsertQuery(queryComponente, "FICHATECNICA");

                    fichatecnicaList.Add(fichatecnica);
                    fichatecnicaList.UpdateCollection();
                    gridDados.Items.Refresh();
                    gridDados.ScrollIntoView(fichatecnica);

                    sqlite.Disconnect();
                    sqlite = null;
                }
            }
            else if (txtBtnInserir.Text.Equals("Salvar"))
            {
                //fichatecnica.ID = ((FichaTecnica)gridDados.SelectedItem).ID;
                fichatecnica.ID = ((FichaTecnica)gridDados.SelectedCells[0].Item).ID;

                queryComponente = "UPDATE FICHATECNICA SET " +
                    "IDPRODUTO=" + fichatecnica.Produto.ID.ToString() + "," +
                    "IDCOMPONENTE=" + fichatecnica.Componente.ID.ToString() + "," +
                    "QUANTIDADE=" + fichatecnica.Quantidade.ToString().Replace(',', '.') + "," +
                    "LIXADA=" + (fichatecnica.Lixada.Equals(true) ? 1 : 0).ToString() + "," +
                    "APROVEITAMENTO=" + (fichatecnica.Aproveitamento.Equals(true) ? 1 : 0).ToString() + "," +
                    "OBSERVACOES='" + fichatecnica.Observacoes + "'," +
                    "AGRUPAMENTO='" + fichatecnica.Agrupamento + "' " +
                    "WHERE ID=" + fichatecnica.ID;

                if (sqlite.Connect())
                {
                    if (sqlite.UpdateQuery(queryComponente))
                    {
                        gridDados.IsEnabled = true;
                        ChangeItemValues(fichatecnicaList, fichatecnica);
                        gridDados.Items.Refresh();
                        gridDados.ScrollIntoView(fichatecnica);

                        txtBtnInserir.Text = "Inserir";

                        comboTipoComponente.SelectedIndex = -1;
                        comboMateriaPrima.SelectedIndex = -1;
                        txtQuantidade.Value = txtQuantidade.DefaultValue;
                        txtComprimento.Value = txtComprimento.DefaultValue;
                        txtLargura.Value = txtLargura.DefaultValue;
                        txtEspessura.Value = txtEspessura.DefaultValue;
                        txtObservacoes.Clear();
                        comboFatorAgrup.Text = "";
                        txtCodigo.Clear();
                        //checkAproveitamento.IsChecked = false;
                        checkLixada.IsChecked = false;
                    }

                    sqlite.Disconnect();
                    sqlite = null;
                }
            }

            txtQuantidade.Focus();
        }

        private void ChangeItemValues(ObsCollection<FichaTecnica> fichatecnicaList, FichaTecnica fichatecnica)
        {
            foreach (FichaTecnica item in fichatecnicaList)
            {
                if (item.ID.Equals(fichatecnica.ID))
                {
                    item.Componente = fichatecnica.Componente;
                    item.Quantidade = fichatecnica.Quantidade;
                    item.Aproveitamento = fichatecnica.Aproveitamento;
                    item.Lixada = fichatecnica.Lixada;
                    item.Observacoes = fichatecnica.Observacoes;
                    item.Agrupamento = fichatecnica.Agrupamento;
                }
            }
        }

        private long CreateOrUseComponente(Componente componente)
        {
            SQLite sqlite = new SQLite();
            long retorno = 0;

            if (!sqlite.ExistComponente(componente))
            {
                string query = "INSERT INTO COMPONENTE (" +
                    "IDTIPOCOMPONENTE," +
                    "IDMATERIAPRIMA," +
                    "COMPRIMENTO," +
                    "LARGURA," +
                    "ESPESSURA," +
                    "CODIGO) VALUES (" +
                    componente.Tipo.ID.ToString() + "," +
                    componente.MateriaPrima.ID.ToString() + "," +
                    componente.Comprimento + "," +
                    componente.Largura + "," +
                    componente.Espessura + ",'" +
                    componente.Codigo + "')";

                if (sqlite.Connect())
                {
                    retorno = sqlite.InsertQuery(query, "COMPONENTE");

                    sqlite.Disconnect();
                    sqlite = null;
                }
            }
            else
            {
                retorno = sqlite.GetComponenteIDByComponente(componente);
            }

            return retorno;
        }

        private void txtReferencia_GotFocus(object sender, RoutedEventArgs e)
        {
            btnBuscarProduto.IsDefault = true;
            btnInserir.IsDefault = false;
            txtReferencia.SelectAll();
        }

        private void menuAlterar_Click(object sender, RoutedEventArgs e)
        {
            FichaTecnica fichatecnica = new FichaTecnica();
            //fichatecnica = (FichaTecnica)gridDados.SelectedItem;
            fichatecnica = (FichaTecnica)gridDados.SelectedCells[0].Item;

            txtBtnInserir.Text = "Salvar";

            comboTipoComponente.SelectedValue = fichatecnica.Componente.Tipo.ID;
            comboMateriaPrima.SelectedValue = fichatecnica.Componente.MateriaPrima.ID;
            txtQuantidade.Value = fichatecnica.Quantidade;
            txtComprimento.Value = fichatecnica.Componente.Comprimento;
            txtLargura.Value = fichatecnica.Componente.Largura;
            txtEspessura.Value = fichatecnica.Componente.Espessura;
            txtObservacoes.Text = fichatecnica.Observacoes;
            comboFatorAgrup.Text = fichatecnica.Agrupamento;
            txtCodigo.Text = fichatecnica.Componente.Codigo;
            //checkAproveitamento.IsChecked = fichatecnica.Aproveitamento;
            checkLixada.IsChecked = fichatecnica.Lixada;

            gridDados.IsEnabled = false;

            txtComprimento.Focus();
        }

        private void menuExcluir_Click(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            FichaTecnica fichatecnica = new FichaTecnica();
            string query;

            //fichatecnica = (FichaTecnica)gridDados.SelectedItem;
            fichatecnica = (FichaTecnica)gridDados.SelectedCells[0].Item;
            query = "DELETE FROM FICHATECNICA WHERE ID=" + fichatecnica.ID;

            if (MessageBox.Show("Confirma exclusão?\n" + fichatecnica.Componente.Tipo.Descricao + " - " +
                 fichatecnica.Componente.MateriaPrima.Descricao + " - " + fichatecnica.Componente.Comprimento + " X " +
                 fichatecnica.Componente.Largura + " X " + fichatecnica.Componente.Espessura,
                "Confirmação de Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (sqlite.Connect())
                {
                    if (sqlite.DeleteQuery(query))
                    {
                        fichatecnicaList.Remove(fichatecnica);
                        gridDados.Items.Refresh();

                        comboTipoComponente.SelectedIndex = -1;
                        comboMateriaPrima.SelectedIndex = -1;
                        txtQuantidade.Value = txtQuantidade.DefaultValue;
                        txtComprimento.Value = txtComprimento.DefaultValue;
                        txtLargura.Value = txtLargura.DefaultValue;
                        txtEspessura.Value = txtEspessura.DefaultValue;
                        txtObservacoes.Clear();
                        comboFatorAgrup.Text = "";
                        txtCodigo.Clear();
                        //checkAproveitamento.IsChecked = false;
                        checkLixada.IsChecked = false;
                    }

                    sqlite.Disconnect();
                    sqlite = null;
                }

                txtQuantidade.Focus();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable tiposcomponentes = new DataTable();
            DataTable materiasprimas = new DataTable();

            string queryTiposComponentes = "SELECT ID,DESCRICAO FROM TIPOCOMPONENTE ORDER BY DESCRICAO";
            string queryMateriasPrimas = "SELECT ID,DESCRICAO FROM MATERIAPRIMA ORDER BY DESCRICAO";

            PopularComboXML();

            if (sqlite.Connect())
            {
                tiposcomponentes = sqlite.GetTable(queryTiposComponentes);
                comboTipoComponente.ItemsSource = tiposcomponentes.DefaultView;

                materiasprimas = sqlite.GetTable(queryMateriasPrimas);
                comboMateriaPrima.ItemsSource = materiasprimas.DefaultView;

                gridDados.ItemsSource = fichatecnicaList;

                sqlite.Disconnect();
                sqlite = null;
            }

            txtReferencia.Focus();

            if (this.Produto != null)
            {
                txtReferencia.Text = this.Produto.Referencia;
            }
        }

        private void PopularComboXML()
        {
            itensAgrupamento.Clear();

            xmlPath = ConfigurationManager.AppSettings["PathDB"];

            if (xmlPath.Trim() == "")
            {
                xmlPath = System.AppDomain.CurrentDomain.BaseDirectory + xmlAgrupamento;
            }
            else
            {
                xmlPath += xmlAgrupamento;
            }

            try
            {
                xmlDoc.Load(xmlPath);

                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    itensAgrupamento.Add(node.InnerText);
                }

                itensAgrupamento.Sort();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao abrir arquivo Agrupamento.xml\n" + ex.ToString(),
                    "Erro de Abertura de Arquivo", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            comboFatorAgrup.ItemsSource = itensAgrupamento;
            comboFatorAgrup.Items.Refresh();
        }

        private void btnInserirTipoComponente_Click(object sender, RoutedEventArgs e)
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

        private void btnInserirMateriaPrima_Click(object sender, RoutedEventArgs e)
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

        private void txtQuantidade_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !OnlyNumericAndComma(e.Text);
        }

        public static bool OnlyNumeric(string text)
        {
            Regex regex = new Regex("[^0-9]+");

            return !regex.IsMatch(text);
        }

        public static bool OnlyNumericAndComma(string text)
        {
            Regex regex = new Regex("[^0-9,]+");

            return !regex.IsMatch(text);
        }

        private void txtObservacoes_GotFocus(object sender, RoutedEventArgs e)
        {
            txtObservacoes.SelectAll();
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
            else
            {
                CancelarFicha();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (txtBtnInserir.Text == "Inserir")
                    CancelarFicha();
                else
                    CancelarAlteracao();
            }
        }

        private void CancelarAlteracao()
        {
            gridDados.IsEnabled = true;

            txtBtnInserir.Text = "Inserir";

            comboTipoComponente.SelectedIndex = -1;
            comboMateriaPrima.SelectedIndex = -1;
            txtQuantidade.Value = txtQuantidade.DefaultValue;
            txtComprimento.Value = txtComprimento.DefaultValue;
            txtLargura.Value = txtLargura.DefaultValue;
            txtEspessura.Value = txtEspessura.DefaultValue;
            txtObservacoes.Clear();
            comboFatorAgrup.Text = "";
            txtCodigo.Clear();
            //checkAproveitamento.IsChecked = false;
            checkLixada.IsChecked = false;

            txtQuantidade.Focus();
        }

        private void CancelarFicha()
        {
            fichatecnicaList.Clear();

            gridDados.IsEnabled = true;
            gridDados.ItemsSource = null;
            gridDados.Items.Clear();

            txtReferencia.Clear();
            txtID.Clear();
            txtDescricao.Clear();
            groupInserir.IsEnabled = false;
            comboTipoComponente.SelectedIndex = -1;
            comboMateriaPrima.SelectedIndex = -1;
            txtQuantidade.Value = txtQuantidade.DefaultValue;
            txtComprimento.Value = txtComprimento.DefaultValue;
            txtLargura.Value = txtLargura.DefaultValue;
            txtEspessura.Value = txtEspessura.DefaultValue;
            txtObservacoes.Clear();
            comboFatorAgrup.Text = "";
            txtCodigo.Clear();
            //checkAproveitamento.IsChecked = false;
            checkLixada.IsChecked = false;

            txtReferencia.Focus();
        }

        private void btnInserirProduto_Click(object sender, RoutedEventArgs e)
        {
            CadProduto formulario = new CadProduto();
            formulario.ShowDialog();

            if (formulario.Produto != null)
            {
                txtReferencia.Text = formulario.Produto.Referencia;

                BuscarProdutoExecuted();
            }
            else
            {
                txtReferencia.Focus();
            }

            GC.Collect();
        }

        private void txtReferencia_LostFocus(object sender, RoutedEventArgs e)
        {
            btnBuscarProduto.IsDefault = false;
            btnInserir.IsDefault = true;
        }

        private void txtCodigo_GotFocus(object sender, RoutedEventArgs e)
        {
            txtCodigo.SelectAll();
        }

        private void CopiarFichaTecnica_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!txtID.Text.Equals("") && !txtDescricao.Text.Equals("") && gridDados.Items.Count > 0)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void CopiarFichaTecnica_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CopyFichaTecnica formulario = new CopyFichaTecnica();
            Produto produto = new Produto();

            produto.ID = Convert.ToInt64(txtID.Text);
            produto.Referencia = txtReferencia.Text;
            produto.Descricao = txtDescricao.Text;

            formulario.Origem = produto;
            formulario.Ficha = fichatecnicaList;

            formulario.ShowDialog();
        }

        private void AlterarMedidasFichaTecnica_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!txtID.Text.Equals("") && !txtDescricao.Text.Equals("") && gridDados.Items.Count > 0)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void AlterarMedidasFichaTecnica_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ManutMedidas formulario = new ManutMedidas();
            Produto produto = new Produto();

            produto.ID = Convert.ToInt64(txtID.Text);
            produto.Referencia = txtReferencia.Text;
            produto.Descricao = txtDescricao.Text;

            formulario.Produto = produto;
            formulario.ShowDialog();

            GC.Collect();

            BuscarProdutoExecuted();
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

        private void menuSort_Click(object sender, RoutedEventArgs e)
        {
            gridDados.Items.SortDescriptions.Clear();

            gridDados.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Componente.Tipo.Descricao",
                    System.ComponentModel.ListSortDirection.Ascending));
            gridDados.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Componente.Espessura",
                    System.ComponentModel.ListSortDirection.Ascending));
            gridDados.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Componente.Largura",
                                System.ComponentModel.ListSortDirection.Ascending));
            gridDados.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Componente.Comprimento",
                                System.ComponentModel.ListSortDirection.Ascending));

            gridDados.Items.Refresh();
            gridDados.SelectedIndex = 0;
        }

        private void comboFatorAgrup_DropDownOpened(object sender, EventArgs e)
        {
            PopularComboXML();
        }

    }
}
