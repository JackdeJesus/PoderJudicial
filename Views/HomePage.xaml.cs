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
using PoderJudicial.Models;
using System.Windows.Input;
using System.Windows.Media;

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

        // Antes, todas las consultas del Home (totales del mes, audiencias
        // de hoy, actividad reciente — varias de ellas recorren TODAS las
        // tablas de Audiencias) se ejecutaban de forma síncrona en el
        // constructor de la página, en el hilo de UI. Como Home es la
        // pantalla más visitada (se vuelve a crear cada vez que se navega
        // a ella, incluida la recarga automática tras un cambio de base de
        // datos), eso significaba congelar la interfaz en cada visita
        // mientras Access respondía. Se movió el trabajo de BD a un hilo de
        // fondo con Task.Run; el "await" retoma en el hilo de UI
        // automáticamente (SynchronizationContext de WPF), así que asignar
        // las propiedades del ViewModel abajo sigue siendo seguro sin
        // Dispatcher.Invoke manual.
        private async void CargarDashboard()
        {
            try
            {
                var datos = await Task.Run(() =>
                {
                    DashboardData dashboard = new DashboardData();

                    return new
                    {
                        TotalAudiencias = dashboard.ObtenerTotalAudienciasMes(),
                        TotalEjecuciones = dashboard.ObtenerTotalEjecucionesMes(),
                        TotalCopias = dashboard.ObtenerTotalCopiasMes(),
                        AudienciasHoy = dashboard.ObtenerAudienciasHoy(),
                        Version = dashboard.ObtenerVersionSistema(),
                        NombreBD = dashboard.ObtenerNombreBaseDatos(),
                        Estado = dashboard.ObtenerEstadoSistema(),
                        Actividades = dashboard.ObtenerActividadesRecientes()
                    };
                });

                vm.TotalAudienciasMes = datos.TotalAudiencias;
                vm.TotalEjecucionesMes = datos.TotalEjecuciones;
                vm.TotalCopiasMes = datos.TotalCopias;
                vm.AudienciasHoy = datos.AudienciasHoy;
                vm.VersionSistema = datos.Version;
                vm.NombreBaseDatos = datos.NombreBD;
                vm.EstadoSistema = datos.Estado;

                // Temporal mientras no exista la lógica de respaldos
                vm.UltimaCopiaSeguridad = "No disponible";

                vm.Actividades = new ObservableCollection<ActividadReciente>(datos.Actividades);
            }
            catch (Exception ex)
            {
                // Mismo criterio que el resto de la app: avisar sin tirar
                // la pantalla completa. Home sigue mostrando reloj y
                // navegación aunque el panel de indicadores no cargue.
                MessageBox.Show(
                    "No se pudo cargar la información del panel principal:\n" + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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

        // ══════════════════════════════════════════════
        //  TARJETAS DE ESTADÍSTICAS → accesos directos a
        //  Consultar Registros, ya filtrados.
        // ══════════════════════════════════════════════
        private void CardAudienciasMes_Click(object sender, RoutedEventArgs e)
        {
            var (desde, hasta) = RangoMesActual();

            ObtenerDashboard()?.AbrirConsultarRegistros(
                TableDetector.TablaActual,
                new FiltroConsulta { FechaDesde = desde, FechaHasta = hasta });
        }

        private void CardEjecucionesMes_Click(object sender, RoutedEventArgs e)
        {
            var (desde, hasta) = RangoMesActual();

            ObtenerDashboard()?.AbrirConsultarRegistros(
                "Ejecucion",
                new FiltroConsulta { FechaDesde = desde, FechaHasta = hasta });
        }

        private void CardCopiasMes_Click(object sender, RoutedEventArgs e)
        {
            var (desde, hasta) = RangoMesActual();

            // Copias: lo que importa es cuándo se ENTREGÓ la copia
            // (Fecha de Recibo), no la fecha de la audiencia original.
            ObtenerDashboard()?.AbrirConsultarRegistros(
                "CopiasAudiencias",
                new FiltroConsulta { FechaReciboDesde = desde, FechaReciboHasta = hasta });
        }

        private void CardAudienciasHoy_Click(object sender, RoutedEventArgs e)
        {
            ObtenerDashboard()?.AbrirConsultarRegistros(
                TableDetector.TablaActual,
                new FiltroConsulta { FechaDesde = DateTime.Today, FechaHasta = DateTime.Today });
        }

        private static (DateTime desde, DateTime hasta) RangoMesActual()
        {
            DateTime hoy = DateTime.Now;
            DateTime primerDia = new DateTime(hoy.Year, hoy.Month, 1);
            DateTime ultimoDia = primerDia.AddMonths(1).AddDays(-1);
            return (primerDia, ultimoDia);
        }


        private void Actividad_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#F8FAFC"));
            }
        }

        private void Actividad_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = Brushes.Transparent;
            }
        }


        private void Actividad_Click(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;

            if (border == null)
                return;

            ActividadReciente actividad =
                border.DataContext as ActividadReciente;

            if (actividad == null)
                return;

            ObtenerDashboard()?
                .AbrirConsultarRegistros(
                    actividad.TablaDestino);
        }


    }
}