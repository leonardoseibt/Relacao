using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;

namespace Relacao
{
    /// <summary>
    /// Interaction logic for WindowCrystalReports.xaml
    /// </summary>
    public partial class WindowCrystalReports : Window
    {
        internal ReportDocument Relatorio { get; set; }
        internal Dictionary<string, string> Parametros { get; set; }
        internal string FullPath { get; set; }
        internal string Titulo { get; set; }

        public WindowCrystalReports()
        {
            this.Parametros = new Dictionary<string, string>();

            InitializeComponent();

            crystalViewer.Owner = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ParameterFields parameterFields = new ParameterFields();
            ParameterField parameterField;
            ParameterDiscreteValue discreteValue;

            string empresa = ConfigurationManager.AppSettings["NomeEmpresa"];

            this.Relatorio.SummaryInfo.ReportAuthor = empresa;
            this.Relatorio.SummaryInfo.ReportTitle = this.Titulo;

            foreach (KeyValuePair<string, string> parametro in this.Parametros)
            {
                parameterField = new ParameterField();
                discreteValue = new ParameterDiscreteValue();

                parameterField.Name = parametro.Key;
                discreteValue.Value = parametro.Value;
                parameterField.CurrentValues.Add(discreteValue);

                parameterFields.Add(parameterField);
            }

            crystalViewer.Owner = Window.GetWindow(this);

            crystalViewer.ViewerCore.ReportSource = this.Relatorio;
            crystalViewer.ViewerCore.ParameterFieldInfo = parameterFields;
        }

    }
}
