using System;
using System.Collections.Generic;
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
using System.Globalization;


namespace PoderJudicial.Views
{
    public partial class ConsultarRegistros : Page
    {
        private DispatcherTimer timer;
        public ConsultarRegistros()
        {
            InitializeComponent();
            IniciarReloj();


            // CARGA TEMPORAL
            CargarDatosTemporales();
        }


        // ─────────────────────────────────────────────────────────
        //  Inicialización del reloj (fecha y hora actualizados cada segundo)
        // ─────────────────────────────────────────────────────────

        private void IniciarReloj()
        {
            timer = new DispatcherTimer();

            // Actualizar cada segundo
            timer.Interval = TimeSpan.FromSeconds(1);

            // Evento
            timer.Tick += Timer_Tick;

            // Iniciar
            timer.Start();

            // Mostrar inmediatamente
            ActualizarFechaHora();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            ActualizarFechaHora();
        }

        private void ActualizarFechaHora()
        {
            DateTime ahora = DateTime.Now;

            CultureInfo cultura = new CultureInfo("es-MX");

            // Hora
            TxtHora.Text = ahora.ToString("hh:mm tt");

            // Fecha
            TxtFecha.Text = ahora.ToString(
                "dddd, dd MMMM yyyy",
                cultura);
        }

        /// <summary>
        /// #region DATOS TEMPORALES ELIMINAR CUANDO SE CONECTE ACCESS
        /// </summary>

        private void CargarDatosTemporales()
        {
            var registros = new List<Audiencia>()
            {
                new Audiencia
                {
                    NoCausa = "PJH-001/2026",
                    NUC = "12-2026-45871",
                    FechaAudiencia = "15/05/2026",
                    TipoAudiencia = "Inicial",
                    Juzgado = "Juzgado Penal Oral",
                    Imputado = "Carlos Méndez",
                    Sala = "Sala 1"
                },

                new Audiencia
                {
                    NoCausa = "PJH-002/2026",
                    NUC = "12-2026-45872",
                    FechaAudiencia = "16/05/2026",
                    TipoAudiencia = "Intermedia",
                    Juzgado = "Juzgado de Control",
                    Imputado = "Juan Pérez",
                    Sala = "Sala 2"
                },

                new Audiencia
                {
                    NoCausa = "PJH-003/2026",
                    NUC = "12-2026-45873",
                    FechaAudiencia = "18/05/2026",
                    TipoAudiencia = "Juicio Oral",
                    Juzgado = "Tribunal de Enjuiciamiento",
                    Imputado = "Luis Hernández",
                    Sala = "Sala 4"
                },

                new Audiencia
                {
                    NoCausa = "PJH-004/2026",
                    NUC = "12-2026-45874",
                    FechaAudiencia = "20/05/2026",
                    TipoAudiencia = "Revisión",
                    Juzgado = "Juzgado Mixto",
                    Imputado = "Miguel Torres",
                    Sala = "Sala 3"
                }
            };

            dgAudiencias.ItemsSource = registros;

            txtTotalRegistros.Text = $"{registros.Count} registro(s) encontrado(s)";
        }

       

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnVer_Click(object sender, RoutedEventArgs e)
        {

            VerDetalleRegistro verDetalleRegistro = new VerDetalleRegistro();
            verDetalleRegistro.ShowDialog();


        }
    }

    // MODELO TEMPORAL
    public class Audiencia
    {
        public string NoCausa { get; set; }
        public string NUC { get; set; }
        public string FechaAudiencia { get; set; }
        public string TipoAudiencia { get; set; }
        public string Juzgado { get; set; }
        public string Imputado { get; set; }
        public string Sala { get; set; }
    }
}