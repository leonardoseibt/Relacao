using Relacao.Classes;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for CopyFichaTecnica.xaml
    /// </summary>
    public partial class CopyFichaTecnica : Window
    {
        internal Produto Origem { get; set; }
        internal Produto Destino { get; set; }
        internal ObsCollection<FichaTecnica> Ficha { get; set; }

        public CopyFichaTecnica()
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
            SQLite sqlite = new SQLite();
            Produto produto = new Produto();

            string referencia = txtReferencia.Text.Trim();
            int produtoID = sqlite.GetIDByReferencia(referencia);

            if (produtoID > 0)
            {
                produto = sqlite.GetProdutoByID(produtoID);

                txtID.Text = produto.ID.ToString();
                txtDescricao.Text = produto.Descricao;
            }
            else
            {
                MessageBox.Show("Não existe um produto cadastrado com a referência informada",
                "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);

                txtReferencia.Focus();
            }

            sqlite = null;
        }

        private void Confirm_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (txtID.Text.Trim() != "" && txtDescricao.Text.Trim() != "")
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void Confirm_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SQLite sqlite = new SQLite();
            Produto produto = new Produto();

            produto.ID = Convert.ToInt64(txtID.Text);
            produto.Referencia = txtReferencia.Text;
            produto.Descricao = txtDescricao.Text;

            this.Destino = produto;

            if (sqlite.Connect())
            {
                if (sqlite.HasFichaTecnica(this.Destino))
                {
                    if (MessageBox.Show("O Produto destino já possui uma Ficha Técnica!\nA mesma será sobreposta. Deseja continuar?",
                        "Mensagem de Aviso", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        LimpaFichaTecnica(this.Destino, sqlite);
                        InsereFichaTecnica(this.Ficha, this.Destino, sqlite);
                    }
                }
                else
                {
                    InsereFichaTecnica(this.Ficha, this.Destino, sqlite);
                }

                sqlite.Disconnect();
                sqlite = null;

                this.Close();
            }
        }

        private void InsereFichaTecnica(ObsCollection<FichaTecnica> itens, Produto produto, SQLite sqlite)
        {
            string query;

            if (MessageBox.Show("Confirma a cópia da Ficha Técnica? (" + this.Origem.Referencia + " >>> " + this.Destino.Referencia + ")",
                "Mensagem de Aviso", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (FichaTecnica fichatecnica in itens)
                {
                    query = "INSERT INTO FICHATECNICA (" +
                        "IDPRODUTO," +
                        "IDCOMPONENTE," +
                        "QUANTIDADE," +
                        "LIXADA," +
                        "APROVEITAMENTO," +
                        "OBSERVACOES) VALUES (" +
                        produto.ID.ToString() + "," +
                        fichatecnica.Componente.ID.ToString() + "," +
                        fichatecnica.Quantidade.ToString().Replace(',', '.') + "," +
                        (fichatecnica.Lixada.Equals(true) ? 1 : 0).ToString() + "," +
                        (fichatecnica.Aproveitamento.Equals(true) ? 1 : 0).ToString() + ",'" +
                        fichatecnica.Observacoes + "')";

                    fichatecnica.ID = sqlite.InsertQuery(query, "FICHATECNICA");
                }
            }
        }

        private void LimpaFichaTecnica(Produto produto, SQLite sqlite)
        {
            string query = "DELETE FROM FICHATECNICA WHERE IDPRODUTO = " + produto.ID;

            if (!sqlite.DeleteQuery(query))
            {
                MessageBox.Show("Ocorreu um erro ao tentar excluir a Ficha Técnica deste Produto",
                        "Mensagem de Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtReferencia_GotFocus(object sender, RoutedEventArgs e)
        {
            txtReferencia.SelectAll();
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
        }

        private void btnInserirProduto_Click(object sender, RoutedEventArgs e)
        {
            CadProduto formulario = new CadProduto();
            formulario.ShowDialog();

            GC.Collect();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtReferencia.Focus();
        }

    }
}
