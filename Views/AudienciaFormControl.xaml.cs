using PoderJudicial.Helpers;
using PoderJudicial.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PoderJudicial.Views
{
    /// <summary>
    /// Formulario de captura de una audiencia (C, CP, JO o EXP).
    /// Es un UserControl para poder instanciarse varias veces dentro de
    /// NuevoRegistro.xaml.cs cuando se usan "Audiencias Concentradas",
    /// sin duplicar código: los controles, validaciones y comportamiento
    /// por tipo de causa viven una sola vez aquí.
    /// </summary>
    public partial class AudienciaFormControl : UserControl
    {
        private DispatcherTimer timer;
        internal NuevoRegistroViewModel VM;

        /// <summary>Se dispara cuando el usuario presiona "Concentrada" en este formulario.</summary>
        public event EventHandler ConcentradaClick;

        /// <summary>Se dispara cuando el usuario presiona "Guardar Registro" en este formulario.</summary>
        public event EventHandler GuardarClick;

        public AudienciaFormControl()
        {
            InitializeComponent();

            VM = new NuevoRegistroViewModel();

            CargarIdVisual();
            IniciarReloj();
            AplicarPlaceholders();
        }

        // ── Identificación del formulario dentro de la cadena de concentradas ─
        /// <summary>Etiqueta visual "Formulario N" usada en mensajes de validación.</summary>
        public int Numero { get; private set; } = 1;

        public void EstablecerNumero(int numero)
        {
            Numero = numero;
            TxtSubtitulo.Text = numero == 1
                ? "Complete los campos en el orden indicado."
                : $"Audiencia concentrada #{numero} — complete los campos en el orden indicado.";
        }

        /// <summary>Muestra/oculta el botón "Concentrada" (se oculta al llegar al máximo o si no es el último formulario).</summary>
        public void MostrarBotonConcentrada(bool mostrar)
            => BtnConcentrada.Visibility = mostrar ? Visibility.Visible : Visibility.Collapsed;

        // ── ID visual ────────────────────────────────────
        // Nota: este ID es solo una referencia visual mientras se captura.
        // El ID real que se inserta en la base de datos se recalcula en el
        // host (NuevoRegistro) al momento de guardar, para no duplicar
        // folios cuando hay varios formularios concentrados pendientes.
        internal void CargarIdVisual()
        {
            try
            {
                string tipoCausa = UIHelper.ObtenerValorCombo(CmbTipoCausa);
                TxtId.Text = VM.ObtenerSiguienteId(tipoCausa).ToString();
            }
            catch { TxtId.Text = "---"; }
        }

        internal void EstablecerIdVisual(int id) => TxtId.Text = id.ToString();

        // ── Reloj ─────────────────────────────────────────
        private void IniciarReloj()
        {
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (s, e) => ActualizarFechaHora();
            timer.Start();
            ActualizarFechaHora();
        }

        private void ActualizarFechaHora()
        {
            DateTime ahora = DateTime.Now;
            CultureInfo cultura = new CultureInfo("es-MX");
            TxtHora.Text = ahora.ToString("hh:mm tt");
            TxtFecha.Text = ahora.ToString("dddd, dd MMMM yyyy", cultura);
            TxtFechaRecibo.Text = ahora.ToString("dd/MM/yyyy");
            TxtHoraRecibo.Text = ahora.ToString("hh:mm tt");
        }

        /// <summary>Detiene el reloj interno. El host debe llamarlo al eliminar un formulario.</summary>
        internal void DetenerReloj() => timer?.Stop();

        // ── Placeholders ──────────────────────────────────
        private void AplicarPlaceholders()
        {
            PlaceholderHelper.AddPlaceholder(TxtId);
            PlaceholderHelper.AddPlaceholder(TxtNoCausa, "Ej: 123/2024");
            PlaceholderHelper.AddPlaceholder(TxtNUC, "Ej: 12-2024-00567");
            PlaceholderHelper.AddPlaceholder(TxtNoCausaJuicio, "Ej: 89/2024");
            PlaceholderHelper.AddPlaceholder(TxtTipoAudiencia, "Escriba el tipo de audiencia");
            PlaceholderHelper.AddPlaceholder(TxtImputado, "Nombre del imputado");
            PlaceholderHelper.AddPlaceholder(TxtDelito, "Tipo de delito");
            PlaceholderHelper.AddPlaceholder(TxtAgraviado, "Nombre del agraviado");
            PlaceholderHelper.AddPlaceholder(TxtHoraConclusion, "hh:mm");
            PlaceholderHelper.AddPlaceholder(TxtFechaAudiencia, "dd/MM/yyyy");
            PlaceholderHelper.AddPlaceholder(TxtHoraAudiencia, "hh:mm");
            PlaceholderHelper.AddPlaceholder(TxtJuez, "Nombre del juez");
            PlaceholderHelper.AddPlaceholder(TxtExpediente, "Número de expediente");
            PlaceholderHelper.AddPlaceholder(TxtObservaciones, "Observaciones");
        }

        // ── Combos con opción "Otra..." / "Otro..." ───────
        private void CmbJuzgado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtJuzgadoOtro == null) return;
            var item = CmbJuzgado.SelectedItem as ComboBoxItem;
            TxtJuzgadoOtro.Visibility = item?.Content?.ToString() == "Otra..."
                ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CmbTotDiscoAudiencia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtTotDiscoAudienciaOtro == null) return;
            var item = CmbTotDiscoAudiencia.SelectedItem as ComboBoxItem;
            TxtTotDiscoAudienciaOtro.Visibility = item?.Content?.ToString() == "Otro..."
                ? Visibility.Visible : Visibility.Collapsed;
        }

        // ── Tipo causa ────────────────────────────────────
        private void CmbTipoCausa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (CmbTipoCausa.SelectedItem is not ComboBoxItem item) return;

            string tipo = item.Content.ToString();

            PanelNUC.Visibility = Visibility.Visible;
            PanelNoCausaJuicio.Visibility = Visibility.Collapsed;
            PanelCamposEXP.Visibility = Visibility.Collapsed;
            TxtNoCausaJuicio.Text = "";
            TxtExpediente.Text = "";
            TxtObservaciones.Text = "";
            PanelJuecesExtra.Children.Clear();

            switch (tipo)
            {
                case "C":
                case "CP":
                    BtnAgregarJuez.Visibility = Visibility.Collapsed;
                    break;

                case "JO":
                    PanelNoCausaJuicio.Visibility = Visibility.Visible;
                    BtnAgregarJuez.Visibility = Visibility.Visible;
                    if (PanelJuecesExtra.Children.Count == 0)
                        AgregarCampoJuez();
                    break;

                case "EXP":
                    BtnAgregarJuez.Visibility = Visibility.Collapsed;
                    PanelCamposEXP.Visibility = Visibility.Visible;
                    PanelNUC.Visibility = Visibility.Collapsed;
                    TxtObservaciones.Text = SesionActual.Usuario;
                    TxtObservaciones.IsReadOnly = true;
                    break;
            }

            CargarIdVisual();
        }

        // ── Botones: delegan al host (NuevoRegistro) ──────
        private void BtnConcentrada_Click(object sender, RoutedEventArgs e)
            => ConcentradaClick?.Invoke(this, EventArgs.Empty);

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
            => GuardarClick?.Invoke(this, EventArgs.Empty);
    }
}