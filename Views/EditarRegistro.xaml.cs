using PoderJudicial.Data;
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
        private readonly AudienciaData _data = new AudienciaData();

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

            // ComboBoxes — seleccionar el valor actual si existe
            SeleccionarCombo(CmbJuzgado, audienciaActual.Juzgado);
            SeleccionarCombo(CmbTipoCausa, audienciaActual.TipoCausa);
            SeleccionarCombo(CmbSala, audienciaActual.Sala);
        }

        // Busca el ComboBoxItem cuyo Content coincida y lo selecciona
        private void SeleccionarCombo(ComboBox combo, string valor)
        {
            if (string.IsNullOrEmpty(valor)) return;

            foreach (ComboBoxItem item in combo.Items)
            {
                if (item.Content?.ToString() == valor)
                {
                    combo.SelectedItem = item;
                    return;
                }
            }
        }

        // ACTUALIZAR 
        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            // Recoger valores del formulario al modelo
            audienciaActual.NoCausa = TxtNoCausa.Text.Trim();
            audienciaActual.NUC = TxtNUC.Text.Trim();
            audienciaActual.Imputado = TxtImputado.Text.Trim();
            audienciaActual.Agraviado = TxtAgraviado.Text.Trim();
            audienciaActual.Delito = TxtDelito.Text.Trim();
            audienciaActual.TipoAudiencia = TxtTipoAudiencia.Text.Trim();

            audienciaActual.Juzgado = (CmbJuzgado.SelectedItem as ComboBoxItem)?.Content?.ToString();
            audienciaActual.TipoCausa = (CmbTipoCausa.SelectedItem as ComboBoxItem)?.Content?.ToString();
            audienciaActual.Sala = (CmbSala.SelectedItem as ComboBoxItem)?.Content?.ToString();

            try
            {
                _data.Actualizar(audienciaActual);

                MessageBox.Show(
                    "Registro actualizado correctamente.",
                    "Éxito",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al actualizar:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        //  CANCELAR 
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        //  RELOJ 
        private void IniciarReloj()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            ActualizarFechaHora();
        }

        private void Timer_Tick(object? sender, EventArgs e) => ActualizarFechaHora();

        private void ActualizarFechaHora()
        {
            var ahora = DateTime.Now;
            var cultura = new CultureInfo("es-MX");
            TxtHora.Text = ahora.ToString("hh:mm tt");
            TxtFecha.Text = ahora.ToString("dddd, dd MMMM yyyy", cultura);
        }
    }
}