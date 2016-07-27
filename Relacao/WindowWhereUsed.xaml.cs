using Relacao.Classes;
using System.Data;
using System.Windows;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for WindowWhereUsed.xaml
    /// </summary>
    public partial class WindowWhereUsed : Window
    {
        internal long ComponenteID { get; set; }

        private ObsCollection<Produto> produtosList = new ObsCollection<Produto>();

        public WindowWhereUsed()
        {
            InitializeComponent();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            DataTable produtos = new DataTable();

            string query =
                "SELECT PRODUTO.ID, " +
                "       PRODUTO.REFERENCIA, " +
                "       PRODUTO.DESCRICAO " +
                "  FROM COMPONENTE, " +
                "       FICHATECNICA, " +
                "       PRODUTO " +
                " WHERE FICHATECNICA.IDCOMPONENTE = COMPONENTE.ID AND " +
                "       FICHATECNICA.IDPRODUTO = PRODUTO.ID AND " +
                "       COMPONENTE.ID = " + this.ComponenteID.ToString();

            if (sqlite.Connect())
            {
                produtos = sqlite.GetTable(query);

                if (produtos.Rows.Count > 0)
                {
                    foreach (DataRow row in produtos.Rows)
                    {
                        Produto produto = new Produto();

                        produto.ID = (long)row[0];
                        produto.Referencia = row[1].ToString();
                        produto.Descricao = row[2].ToString();

                        produtosList.Add(produto);
                    }
                }

                gridDados.ItemsSource = produtosList;

                sqlite.Disconnect();
                sqlite = null;
            }
        }

        private void gridDados_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CadFichaTecnica formulario = new CadFichaTecnica();
            Produto produto = new Produto();

            produto = (Produto)gridDados.SelectedItem;
            formulario.Produto = produto;

            formulario.ShowDialog();
        }

    }
}
