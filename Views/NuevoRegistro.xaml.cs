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

using System.Text.RegularExpressions;


namespace PoderJudicial.Views
{
    public partial class NuevoRegistro : Page
    {
        // Contador de jueces extra agregados dinámicamente
        private int _juecesExtra = 0;

        public NuevoRegistro()
        {
            InitializeComponent();

            // Placeholder visual para los TextBox con Tag
            AddPlaceholderBehavior(TxtId);
            AddPlaceholderBehavior(TxtHoraAudiencia, "HH:MM");
            AddPlaceholderBehavior(TxtHoraRecibo, "HH:MM");
            AddPlaceholderBehavior(TxtHoraConclusion, "HH:MM");
            AddPlaceholderBehavior(TxtNoCausa, "Ej: 123/2024");
            AddPlaceholderBehavior(TxtNUC, "Ej: 12-2024-00567");
            AddPlaceholderBehavior(TxtNoCausaJuicio, "Ej: 89/2024");
            AddPlaceholderBehavior(TxtTipoAudiencia, "Escriba el tipo de audiencia");
            AddPlaceholderBehavior(TxtImputado, "Nombre del imputado");
            AddPlaceholderBehavior(TxtDelito, "Tipo de delito");
            AddPlaceholderBehavior(TxtAgraviado, "Nombre del agraviado");
            AddPlaceholderBehavior(TxtDiferida, "Puede quedar vacío");
        }

        // ─────────────────────────────────────────────────────────
        //  PLACEHOLDER HELPER
        // ─────────────────────────────────────────────────────────
        private void AddPlaceholderBehavior(TextBox tb, string? placeholder = null)
        {
            string ph = placeholder ?? (tb.Tag as string ?? "");
            if (string.IsNullOrEmpty(ph)) return;

            tb.Foreground = new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));
            tb.Text = ph;

            tb.GotFocus += (s, e) =>
            {
                if (tb.Text == ph)
                {
                    tb.Text = "";
                    tb.Foreground = new SolidColorBrush(Color.FromRgb(0x37, 0x41, 0x51));
                }
            };

            tb.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    tb.Text = ph;
                    tb.Foreground = new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));
                }
            };
        }

        // Obtiene el texto real (sin placeholder)
        private string GetText(TextBox tb)
        {
            string ph = tb.Tag as string ?? "";
            string val = tb.Text.Trim();
            return val == ph ? "" : val;
        }

        // ─────────────────────────────────────────────────────────
        //  SOLO NÚMEROS EN TxtTotDiscos
        // ─────────────────────────────────────────────────────────
        private void OnlyNumbers_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");
        }

        // ─────────────────────────────────────────────────────────
        //  COMBOS CON "Otro..."
        // ─────────────────────────────────────────────────────────
        private void CmbJuzgado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtJuzgadoOtro == null) return;
            if (CmbJuzgado.SelectedItem is ComboBoxItem item)
                TxtJuzgadoOtro.Visibility = item.Content.ToString() == "Otra..."
                    ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CmbTipoDisco_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtTipoDiscoOtro == null) return;
            if (CmbTipoDisco.SelectedItem is ComboBoxItem item)
                TxtTipoDiscoOtro.Visibility = item.Content.ToString() == "Otro..."
                    ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CmbTotDiscoAudiencia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtTotDiscoAudienciaOtro == null) return;
            if (CmbTotDiscoAudiencia.SelectedItem is ComboBoxItem item)
                TxtTotDiscoAudienciaOtro.Visibility = item.Content.ToString() == "Otro..."
                    ? Visibility.Visible : Visibility.Collapsed;
        }

        // ─────────────────────────────────────────────────────────
        //  AGREGAR JUEZ EXTRA
        // ─────────────────────────────────────────────────────────
        private void BtnAgregarJuez_Click(object sender, RoutedEventArgs e)
        {
            _juecesExtra++;

            var panel = new StackPanel { Margin = new Thickness(0, 0, 0, 8) };

            // Etiqueta
            var lbl = new TextBlock
            {
                Text = $"Juez {_juecesExtra + 1}",
                FontSize = 13,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(0x1A, 0x1A, 0x2E)),
                Margin = new Thickness(0, 0, 0, 6)
            };

            // Combo juez
            var combo = new ComboBox
            {
                Height = 42,
                FontSize = 13,
                Background = new SolidColorBrush(Color.FromRgb(0xFA, 0xFA, 0xFA)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0xE5, 0xE7, 0xEB)),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(12, 0, 12, 0)
            };

            combo.Items.Add(new ComboBoxItem { Content = "Seleccione un juez" });
            combo.Items.Add(new ComboBoxItem { Content = "Lic. García Ramírez" });
            combo.Items.Add(new ComboBoxItem { Content = "Lic. Torres Mendoza" });
            combo.Items.Add(new ComboBoxItem { Content = "Lic. Herrera López" });
            combo.Items.Add(new ComboBoxItem { Content = "Lic. Vargas Soto" });
            combo.Items.Add(new ComboBoxItem { Content = "Lic. Morales Díaz" });
            combo.Items.Add(new ComboBoxItem { Content = "Otro..." });
            combo.SelectedIndex = 0;

            // TextBox "Otro" oculto
            var tbOtro = new TextBox
            {
                Height = 42,
                FontSize = 13,
                Padding = new Thickness(12, 0, 12, 0),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0xE5, 0xE7, 0xEB)),
                BorderThickness = new Thickness(1),
                Background = new SolidColorBrush(Color.FromRgb(0xFA, 0xFA, 0xFA)),
                Margin = new Thickness(0, 6, 0, 0),
                Visibility = Visibility.Collapsed
            };

            combo.SelectionChanged += (s, ev) =>
            {
                if (combo.SelectedItem is ComboBoxItem ci)
                    tbOtro.Visibility = ci.Content.ToString() == "Otro..."
                        ? Visibility.Visible : Visibility.Collapsed;
            };

            // Botón quitar
            var btnQuitar = new Button
            {
                Content = "× Quitar",
                Height = 28,
                FontSize = 11,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 4, 0, 0),
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Foreground = new SolidColorBrush(Color.FromRgb(0xEF, 0x44, 0x44)),
                Cursor = Cursors.Hand
            };
            btnQuitar.Click += (s, ev) =>
            {
                PanelJuecesExtra.Children.Remove(panel);
                _juecesExtra--;
            };

            panel.Children.Add(lbl);
            panel.Children.Add(combo);
            panel.Children.Add(tbOtro);
            panel.Children.Add(btnQuitar);

            PanelJuecesExtra.Children.Add(panel);
        }

        // ─────────────────────────────────────────────────────────
        //  GUARDAR REGISTRO
        // ─────────────────────────────────────────────────────────
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(GetText(TxtId)))
            {
                MessageBox.Show("El campo ID es obligatorio.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtId.Focus();
                return;
            }

            if (DpFeAudiencia.SelectedDate == null)
            {
                MessageBox.Show("Seleccione la fecha de audiencia.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CmbJuzgado.SelectedIndex <= 0)
            {
                MessageBox.Show("Seleccione un juzgado.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ── Aquí se construye el modelo y se guarda en BD ──
            // var registro = new Audiencia { ... };
            // _repository.Save(registro);

            MessageBox.Show("Registro guardado correctamente.", "Éxito",
                MessageBoxButton.OK, MessageBoxImage.Information);

            // Limpiar formulario
            LimpiarFormulario();
        }

        // ─────────────────────────────────────────────────────────
        //  LIMPIAR FORMULARIO
        // ─────────────────────────────────────────────────────────
        private void LimpiarFormulario()
        {
            TxtId.Text = "";
            TxtTotDiscos.Text = "0";
            TxtNoCausa.Text = "";
            TxtNUC.Text = "";
            TxtNoCausaJuicio.Text = "";
            TxtTipoAudiencia.Text = "";
            TxtHoraAudiencia.Text = "";
            TxtHoraRecibo.Text = "";
            TxtHoraConclusion.Text = "";
            TxtImputado.Text = "";
            TxtDelito.Text = "";
            TxtAgraviado.Text = "";
            TxtDiferida.Text = "";
            TxtJuzgadoOtro.Visibility = Visibility.Collapsed;
            TxtTipoDiscoOtro.Visibility = Visibility.Collapsed;
            TxtTotDiscoAudienciaOtro.Visibility = Visibility.Collapsed;
            TxtJuez1Otro.Visibility = Visibility.Collapsed;
            PanelJuecesExtra.Children.Clear();
            _juecesExtra = 0;
            DpFeAudiencia.SelectedDate = null;
            DpFeRecibo.SelectedDate = null;
            CmbTipoCausa.SelectedIndex = 0;
            CmbJuzgado.SelectedIndex = 0;
            CmbSala.SelectedIndex = 0;
            CmbTipoDisco.SelectedIndex = 0;
            CmbTotDiscoAudiencia.SelectedIndex = 0;
            CmbJuez1.SelectedIndex = 0;
            CmbQuienRealiza.SelectedIndex = 0;

            // Re-aplicar placeholders
            AddPlaceholderBehavior(TxtId);
            AddPlaceholderBehavior(TxtHoraAudiencia, "HH:MM");
            AddPlaceholderBehavior(TxtHoraRecibo, "HH:MM");
            AddPlaceholderBehavior(TxtHoraConclusion, "HH:MM");
            AddPlaceholderBehavior(TxtNoCausa, "Ej: 123/2024");
            AddPlaceholderBehavior(TxtNUC, "Ej: 12-2024-00567");
            AddPlaceholderBehavior(TxtNoCausaJuicio, "Ej: 89/2024");
            AddPlaceholderBehavior(TxtTipoAudiencia, "Escriba el tipo de audiencia");
            AddPlaceholderBehavior(TxtImputado, "Nombre del imputado");
            AddPlaceholderBehavior(TxtDelito, "Tipo de delito");
            AddPlaceholderBehavior(TxtAgraviado, "Nombre del agraviado");
            AddPlaceholderBehavior(TxtDiferida, "Puede quedar vacío");
        }
    }
}

