using PoderJudicial.Data;
using PoderJudicial.Helpers;
using PoderJudicial.Models;
using PoderJudicial.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private HomePageViewModel vm;

        private DispatcherTimer timer;
        public HomePage()
        {
            
            InitializeComponent();

            vm = new HomePageViewModel();
            DataContext = vm;

            IniciarReloj();
            CargarDashboard();
            
        }

        private Dashboard ObtenerDashboard()
        {
            return Window.GetWindow(this) as Dashboard;
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

        private void CargarDashboard()
        {
            DashboardData dashboard =
                new DashboardData();

            vm.TotalAudienciasMes =
                dashboard.ObtenerTotalAudienciasMes();

            vm.TotalEjecucionesMes =
                dashboard.ObtenerTotalEjecucionesMes();


            vm.TotalCopiasMes =
    dashboard.ObtenerTotalCopiasMes();


            vm.AudienciasHoy = dashboard.ObtenerAudienciasHoy();


            vm.VersionSistema = dashboard.ObtenerVersionSistema();
            vm.NombreBaseDatos = dashboard.ObtenerNombreBaseDatos();
            vm.EstadoSistema = dashboard.ObtenerEstadoSistema();


            // Temporal mientras no exista la lógica de respaldos
            vm.UltimaCopiaSeguridad = "No disponible";

            vm.Actividades = new ObservableCollection<ActividadReciente>(
            dashboard.ObtenerActividadesRecientes());

        }


        private void CardNuevoRegistro_Click(
     object sender,
     RoutedEventArgs e)
        {
            ObtenerDashboard()?.AbrirNuevoRegistro();
        }

        private void CardConsultar_Click(
    object sender,
    RoutedEventArgs e)
        {
            ObtenerDashboard()?.AbrirConsultarRegistros();
        }
        private void CardCopias_Click(
    object sender,
    RoutedEventArgs e)
        {
            ObtenerDashboard()?.AbrirRegistroCopias();
        }

        private void CardReportes_Click(
    object sender,
    RoutedEventArgs e)
        {
            ObtenerDashboard()?.AbrirReportes();
        }

        private void CardConfiguracion_Click(
    object sender,
    RoutedEventArgs e)
        {
            ObtenerDashboard()?.AbrirConfiguracion();
        }



    }
}
