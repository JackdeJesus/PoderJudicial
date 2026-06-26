using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PoderJudicial.Views
{
    /// <summary>
    /// Lógica de interacción para HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {

        private DispatcherTimer timer;
        public HomePage()
        {
            
            InitializeComponent();
            IniciarReloj();
        }

        private void ActualizarFechaHora()
        {
            DateTime ahora = DateTime.Now;
            CultureInfo cultura = new CultureInfo("es-MX");
            TxtHora.Text = ahora.ToString("hh:mm tt");
            TxtFecha.Text = ahora.ToString("dddd, dd MMMM yyyy", cultura);
            
        }

        private void IniciarReloj()
        {
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (s, e) => ActualizarFechaHora();
            timer.Start();
            ActualizarFechaHora();
        }


        private void CardNuevoRegistro_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para manejar el evento de clic en "Nuevo Registro"
            MessageBox.Show("Nuevo Registro clicado.");
        }

        private void CardConsultar_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para manejar el evento de clic en "Consultar Registros"
            MessageBox.Show("Consultar Registros clicado.");
        }

        private void CardCopias_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para manejar el evento de clic en "Registro de Copias"
            MessageBox.Show("Registro de Copias clicado.");
        }

        private void CardReportes_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para manejar el evento de clic en "Reportes"
            MessageBox.Show("Reportes clicado.");
        }

        private void CardConfiguracion_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para manejar el evento de clic en "Configuración"
            MessageBox.Show("Configuración clicada.");
        }



    }
}
