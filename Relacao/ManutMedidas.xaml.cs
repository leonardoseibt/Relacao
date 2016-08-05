using Relacao.Classes;
using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for ManutMedidas.xaml
    /// </summary>
    public partial class ManutMedidas : Window
    {
        internal Produto Produto { get; set; }

        private ObsCollection<FichaTecnica> fichatecnicaList = new ObsCollection<FichaTecnica>();

        private bool alterado = false;

        public ManutMedidas()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Produto == null)
            {
                txtReferencia.Focus();
            }
            else
            {
                txtReferencia.Text = this.Produto.Referencia;
                BuscarProdutoExecuted();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CancelarFicha();
            }
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
            }
            else
            {
                MessageBox.Show("Não existe um produto cadastrado com a referência informada",
                "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            txtReferencia.Focus();
        }

        private void AlterarMedidasFichaTecnica_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((txtComprimento.Value != 0 || txtLargura.Value != 0 || txtEspessura.Value != 0) && gridDados.SelectedCells.Count > 0)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void AlterarMedidasFichaTecnica_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            alterado = true;

            Valores valores = new Valores();
            valores.Comprimento = Convert.ToInt32(txtComprimento.Value);
            valores.Largura = Convert.ToInt32(txtLargura.Value);
            valores.Espessura = Convert.ToInt32(txtEspessura.Value);

            foreach (DataGridCellInfo cell in gridDados.SelectedCells)
            {
                FichaTecnica item = (FichaTecnica)cell.Item;

                if (cell.Column.Header.Equals("COMP"))
                {
                    item.Componente.Comprimento += valores.Comprimento;
                    item.Alterado = true;
                }
                else if (cell.Column.Header.Equals("LARG"))
                {
                    item.Componente.Largura += valores.Largura;
                    item.Alterado = true;
                }
                else if (cell.Column.Header.Equals("ESPES"))
                {
                    item.Componente.Espessura += valores.Espessura;
                    item.Alterado = true;
                }
            }

            gridDados.Items.Refresh();
        }

        private void txtReferencia_GotFocus(object sender, RoutedEventArgs e)
        {
            btnBuscarProduto.IsDefault = true;
            txtReferencia.SelectAll();
        }

        private void txtReferencia_LostFocus(object sender, RoutedEventArgs e)
        {
            btnBuscarProduto.IsDefault = false;
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

        private void CancelarFicha()
        {
            alterado = false;

            fichatecnicaList.Clear();

            gridDados.IsEnabled = true;
            gridDados.ItemsSource = null;
            gridDados.Items.Clear();

            txtReferencia.Clear();
            txtID.Clear();
            txtDescricao.Clear();

            txtComprimento.Value = 0;
            txtLargura.Value = 0;
            txtEspessura.Value = 0;

            txtReferencia.Focus();
        }

        private void gridDados_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while ((dep != null) && !(dep is DataGridCell) && !(dep is DataGridColumnHeader))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
                return;

            if (dep is DataGridCell)
            {
                foreach (DataGridCellInfo cellInfo in gridDados.SelectedCells)
                {
                    string coluna = cellInfo.Column.Header.ToString();

                    if (!coluna.Equals("COMP") && !coluna.Equals("LARG") && !coluna.Equals("ESPES"))
                    {
                        gridDados.SelectedCells.Remove(cellInfo);
                    }
                }
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

        public static bool OnlyNumeric(string text)
        {
            Regex regex = new Regex("[^0-9-]+");

            return !regex.IsMatch(text);
        }

        static string GetColorName(Color color)
        {
            PropertyInfo colorProperty = typeof(Colors).GetProperties().FirstOrDefault(p => Color.AreClose((Color)p.GetValue(null), color));
            
            return colorProperty != null ? colorProperty.Name : "Cor Sem Nome";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (alterado)
            {
                if (MessageBox.Show("Alguns items foram alterados!\nDeseja salvar estas alterações?",
                    "Confirmação de Alteração", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    foreach (FichaTecnica fichatecnica in gridDados.Items)
                    {
                        if (fichatecnica.Alterado)
                        {
                            SQLite sqlite = new SQLite();

                            Componente componente = new Componente();
                            componente = fichatecnica.Componente;

                            long newComponenteID = CreateOrUseComponente(componente);

                            string queryDelete =
                                "DELETE FROM FICHATECNICA WHERE ID=" + fichatecnica.ID;

                            string queryInsert =
                                "INSERT INTO FICHATECNICA (" +
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
                                newComponenteID.ToString() + "," +
                                fichatecnica.Quantidade.ToString().Replace(',', '.') + "," +
                                (fichatecnica.Lixada.Equals(true) ? 1 : 0).ToString() + "," +
                                (fichatecnica.Aproveitamento.Equals(true) ? 1 : 0).ToString() + ",'" +
                                fichatecnica.Agrupamento + "','" +
                                GetColorName(fichatecnica.CorComprimento.Color) + "','" +
                                GetColorName(fichatecnica.CorLargura.Color) + "','" +
                                fichatecnica.Observacoes + "')";

                            if (sqlite.Connect())
                            {
                                if (sqlite.DeleteQuery(queryDelete))
                                {
                                    sqlite.InsertQuery(queryInsert, "FICHATECNICA");
                                }

                                sqlite.Disconnect();
                                sqlite = null;
                            }
                        }
                    }
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

        private void menuMarcarComprimento_Click(object sender, RoutedEventArgs e)
        {
            alterado = true;

            foreach (DataGridCellInfo cell in gridDados.SelectedCells)
            {
                FichaTecnica item = (FichaTecnica)cell.Item;

                if (cell.Column.Header.Equals("COMP"))
                {
                    item.CorComprimento = Brushes.Blue;
                    item.Alterado = true;
                }
                else if (cell.Column.Header.Equals("LARG"))
                {
                    item.CorLargura = Brushes.Blue;
                    item.Alterado = true;
                }
            }

            gridDados.Items.Refresh();
        }

        private void menuMarcarProfundidade_Click(object sender, RoutedEventArgs e)
        {
            alterado = true;

            foreach (DataGridCellInfo cell in gridDados.SelectedCells)
            {
                FichaTecnica item = (FichaTecnica)cell.Item;

                if (cell.Column.Header.Equals("COMP"))
                {
                    item.CorComprimento = Brushes.Orange;
                    item.Alterado = true;
                }
                else if (cell.Column.Header.Equals("LARG"))
                {
                    item.CorLargura = Brushes.Orange;
                    item.Alterado = true;
                }
            }

            gridDados.Items.Refresh();
        }

        private void menuDesmarcar_Click(object sender, RoutedEventArgs e)
        {
            alterado = true;

            foreach (DataGridCellInfo cell in gridDados.SelectedCells)
            {
                FichaTecnica item = (FichaTecnica)cell.Item;

                if (cell.Column.Header.Equals("COMP"))
                {
                    item.CorComprimento = Brushes.Black;
                    item.Alterado = true;
                }
                else if (cell.Column.Header.Equals("LARG"))
                {
                    item.CorLargura = Brushes.Black;
                    item.Alterado = true;
                }
            }

            gridDados.Items.Refresh();
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
        }

    }
}
