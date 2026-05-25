using System.Windows;
using System.Windows.Controls;

namespace PoderJudicial.Views
{
    public partial class ReportesView : Page
    {
        public ReportesView()
        {
            InitializeComponent();
        }

        // Filtros — sin lógica por ahora
        private void Filtro_Changed(object sender, SelectionChangedEventArgs e) { }

        // Exportar Excel — sin lógica por ahora
        private void BtnExportarExcel_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Exportación a Excel próximamente.",
                "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Exportar PDF — sin lógica por ahora
        private void BtnExportarPdf_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Exportación a PDF próximamente.",
                "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
