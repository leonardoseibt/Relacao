using Microsoft.Win32;
using System.Configuration;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for WindowConfiguracoes.xaml
    /// </summary>
    public partial class WindowConfiguracoes : Window
    {
        const string registryPath = @"HKEY_CURRENT_USER\Software\ODBC\ODBC.INI\SQLite ODBC Driver - Relacao";
        const string keyName = "Database";

        public WindowConfiguracoes()
        {
            InitializeComponent();
        }

        private void Salvar_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (txtNomeEmpresa.Text.Trim() != "" && txtPathDB.Text.Trim() != "")
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void Salvar_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            string empresa = txtNomeEmpresa.Text.Trim();
            string pathDB = txtPathDB.Text.Trim();

            config.AppSettings.Settings["NomeEmpresa"].Value = empresa;
            config.AppSettings.Settings["PathDB"].Value = pathDB;

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            Registry.SetValue(registryPath, keyName, pathDB + @"\Relacao.s3db");

            this.Close();
        }

        private void txtNomeEmpresa_GotFocus(object sender, RoutedEventArgs e)
        {
            txtNomeEmpresa.SelectAll();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string empresa = ConfigurationManager.AppSettings["NomeEmpresa"];
            string pathDB = ConfigurationManager.AppSettings["PathDB"];

            if (pathDB.Trim() == "")
            {
                pathDB = System.AppDomain.CurrentDomain.BaseDirectory;
            }

            txtNomeEmpresa.Text = empresa;
            txtPathDB.Text = pathDB;

            txtNomeEmpresa.Focus();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog formulario = new FolderBrowserDialog();
            DialogResult result = formulario.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                txtPathDB.Text = formulario.SelectedPath + @"\";
            }

            formulario.Dispose();
        }

        private void txtPathDB_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPathDB.SelectAll();
        }

    }
}
