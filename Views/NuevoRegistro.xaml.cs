using System;
using System.Windows;
using System.Windows.Controls;

namespace PoderJudicial.Views
{
    public partial class NuevoRegistro : Page
    {
        // ──────────────────────────────────────────
        //  CONSTRUCTOR
        // ──────────────────────────────────────────
        public NuevoRegistro()
        {
            InitializeComponent();
        }

        // ══════════════════════════════════════════
        //  HELPER: poblar cualquier ComboBox de horas
        //  08:00 → 20:00, minuto a minuto (721 opciones)
        // ══════════════════════════════════════════
        private void HoraCombo_Loaded(object sender, RoutedEventArgs e)
        {
            var combo = (ComboBox)sender;
            if (combo.Items.Count > 0) return; // ya poblado

            combo.Items.Add(new ComboBoxItem { Content = "-- Hora --" });

            for (int h = 8; h <= 20; h++)
            {
                int minLimit = (h == 20) ? 0 : 59; // 20:00 es el tope
                for (int m = 0; m <= minLimit; m++)
                {
                    combo.Items.Add(new ComboBoxItem
                    {
                        Content = $"{h:D2}:{m:D2}"
                    });
                }
            }

            combo.SelectedIndex = 0;
        }

        // ══════════════════════════════════════════
        //  TOTAL DISCOS  (★ nuevo comportamiento)
        // ══════════════════════════════════════════
        private void CmbTotDiscos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtTotDiscosOtro == null) return;

            var item = CmbTotDiscos.SelectedItem as ComboBoxItem;
            bool esOtro = item?.Content?.ToString() == "Otro...";
            TxtTotDiscosOtro.Visibility = esOtro ? Visibility.Visible : Visibility.Collapsed;
        }

        // ══════════════════════════════════════════
        //  TIPO DISCO
        // ══════════════════════════════════════════
        private void CmbTipoDisco_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtTipoDiscoOtro == null) return;

            var item = CmbTipoDisco.SelectedItem as ComboBoxItem;
            bool esOtro = item?.Content?.ToString() == "Otro...";
            TxtTipoDiscoOtro.Visibility = esOtro ? Visibility.Visible : Visibility.Collapsed;
        }

        // ══════════════════════════════════════════
        //  JUZGADO
        // ══════════════════════════════════════════
        private void CmbJuzgado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtJuzgadoOtro == null) return;

            var item = CmbJuzgado.SelectedItem as ComboBoxItem;
            bool esOtra = item?.Content?.ToString() == "Otra...";
            TxtJuzgadoOtro.Visibility = esOtra ? Visibility.Visible : Visibility.Collapsed;
        }

        // ══════════════════════════════════════════
        //  TOTAL DISCO AUDIENCIA
        // ══════════════════════════════════════════
        private void CmbTotDiscoAudiencia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtTotDiscoAudienciaOtro == null) return;

            var item = CmbTotDiscoAudiencia.SelectedItem as ComboBoxItem;
            bool esOtro = item?.Content?.ToString() == "Otro...";
            TxtTotDiscoAudienciaOtro.Visibility = esOtro ? Visibility.Visible : Visibility.Collapsed;
        }

        // ══════════════════════════════════════════
        //  JUEZ PRINCIPAL — muestra TextBox si elige "Otro..."
        // ══════════════════════════════════════════
        private void CmbJuez1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtJuez1Otro == null) return;

            var item = CmbJuez1.SelectedItem as ComboBoxItem;
            bool esOtro = item?.Content?.ToString() == "Otro...";
            TxtJuez1Otro.Visibility = esOtro ? Visibility.Visible : Visibility.Collapsed;
        }

        // ══════════════════════════════════════════
        //  AGREGAR JUEZ EXTRA (+)
        //  Cada fila tiene su propio ComboBox y,
        //  si se elige "Otro...", su propio TextBox.
        // ══════════════════════════════════════════
        private void BtnAgregarJuez_Click(object sender, RoutedEventArgs e)
        {
            int numero = PanelJuecesExtra.Children.Count + 2; // juez 2, 3, …

            // ── Contenedor de la fila ──
            var fila = new StackPanel { Margin = new Thickness(0, 6, 0, 0) };

            // ── ComboBox del juez extra ──
            var combo = new ComboBox { Style = (Style)FindResource("ComboStyle") };
            foreach (string nombre in new[]
            {
                "Seleccione un juez",
                "Lic. García Ramírez",
                "Lic. Torres Mendoza",
                "Lic. Herrera López",
                "Lic. Vargas Soto",
                "Lic. Morales Díaz",
                "Otro..."
            })
                combo.Items.Add(new ComboBoxItem { Content = nombre });

            combo.SelectedIndex = 0;

            // ── TextBox "Otro" para este juez extra ──
            var txtOtro = new TextBox
            {
                Style = (Style)FindResource("InputStyle"),
                Margin = new Thickness(0, 6, 0, 0),
                Visibility = Visibility.Collapsed,
                Tag = $"Nombre del juez {numero}"
            };

            // Mostrar/ocultar TextBox al cambiar selección
            combo.SelectionChanged += (s, args) =>
            {
                var sel = combo.SelectedItem as ComboBoxItem;
                txtOtro.Visibility = sel?.Content?.ToString() == "Otro..."
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            };

            fila.Children.Add(combo);
            fila.Children.Add(txtOtro);
            PanelJuecesExtra.Children.Add(fila);
        }

        // ══════════════════════════════════════════
        //  GUARDAR
        // ══════════════════════════════════════════
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // ── Validaciones básicas ──
            if (string.IsNullOrWhiteSpace(TxtId.Text))
            {
                MessageBox.Show("El campo 'Id' es obligatorio.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DpFeAudiencia.SelectedDate == null ||
                (CmbHoraAudiencia.SelectedItem as ComboBoxItem)?.Content?.ToString() == "-- Hora --")
            {
                MessageBox.Show("Seleccione la fecha y hora de audiencia.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var juzgadoItem = CmbJuzgado.SelectedItem as ComboBoxItem;
            if (juzgadoItem == null || juzgadoItem.Content.ToString() == "Seleccione juzgado")
            {
                MessageBox.Show("El campo 'Juzgado' es obligatorio.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ── Recopilar valores ──
            string horaAudiencia = (CmbHoraAudiencia.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string horaRecibo = (CmbHoraRecibo.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string horaConclusion = (CmbHoraConclusion.SelectedItem as ComboBoxItem)?.Content?.ToString();

            string totDiscos = ObtenerValorComboOtro(CmbTotDiscos, TxtTotDiscosOtro);
            string tipoDisco = ObtenerValorComboOtro(CmbTipoDisco, TxtTipoDiscoOtro);
            string juzgado = ObtenerValorComboOtro(CmbJuzgado, TxtJuzgadoOtro);
            string totAud = ObtenerValorComboOtro(CmbTotDiscoAudiencia, TxtTotDiscoAudienciaOtro);
            string juez1 = ObtenerValorComboOtro(CmbJuez1, TxtJuez1Otro);

            // TODO: guardar en base de datos o llamar al servicio correspondiente

            MessageBox.Show("Registro guardado correctamente.", "Éxito",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ──────────────────────────────────────────
        //  Utilidad: retorna el texto del TextBox
        //  cuando se eligió "Otro...", de lo contrario
        //  retorna el Content del ComboBoxItem.
        // ──────────────────────────────────────────
        private static string ObtenerValorComboOtro(ComboBox combo, TextBox txtOtro)
        {
            var item = combo.SelectedItem as ComboBoxItem;
            if (item == null) return string.Empty;

            string content = item.Content?.ToString() ?? string.Empty;
            if (content == "Otro..." || content == "Otra...")
                return txtOtro.Text.Trim();

            return content;
        }
    }
}