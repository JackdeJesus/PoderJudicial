using PoderJudicial.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PoderJudicial.Views
{
    public partial class EditarRegistro : Page
    {
        private DispatcherTimer timer;

        private Audiencia audienciaActual;

        public EditarRegistro(Audiencia audiencia)
        {
            InitializeComponent();

            audienciaActual = audiencia;

            IniciarReloj();

            CargarDatos();
        }

        
        // CARGAR DATOS
        

        private void CargarDatos()
        {
            TxtId.Text = audienciaActual.Id.ToString();

            TxtNoCausa.Text = audienciaActual.NoCausa;

            TxtNUC.Text = audienciaActual.NUC;

            TxtImputado.Text = audienciaActual.Imputado;

            TxtAgraviado.Text = audienciaActual.Agraviado;

            TxtDelito.Text = audienciaActual.Delito;

            TxtTipoAudiencia.Text = audienciaActual.TipoAudiencia;

            TxtNoCausa.Text = audienciaActual.NoCausaJuicio;
        }

       
        // RELOJ
       

        private void IniciarReloj()
        {
            timer = new DispatcherTimer();

            timer.Interval = TimeSpan.FromSeconds(1);

            timer.Tick += Timer_Tick;

            timer.Start();

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

            TxtHora.Text = ahora.ToString("hh:mm tt");

            TxtFecha.Text = ahora.ToString(
                "dddd, dd MMMM yyyy",
                cultura);
        }

       
        // ACTUALIZAR
       
        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Registro actualizado correctamente.",
                "Éxito",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

       
        // CANCELAR
        

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}