using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Globalization;

namespace PoderJudicial.Views
{
    public partial class NuevoRegistro : Page
    {

        private DispatcherTimer timer;
        private bool actualizandoFecha = false;
        private bool actualizandoHora = false;
        // ──────────────────────────────────────────
        //  CONSTRUCTOR
        // ──────────────────────────────────────────
        public NuevoRegistro()
        {
            InitializeComponent();

            IniciarReloj();



            // Placeholder visual para los TextBox con Tag
            AddPlaceholderBehavior(TxtId);
            AddPlaceholderBehavior(TxtNoCausa, "Ej: 123/2024");
            AddPlaceholderBehavior(TxtNUC, "Ej: 12-2024-00567");
            AddPlaceholderBehavior(TxtNoCausaJuicio, "Ej: 89/2024");
            AddPlaceholderBehavior(TxtTipoAudiencia, "Escriba el tipo de audiencia");
            AddPlaceholderBehavior(TxtImputado, "Nombre del imputado");
            AddPlaceholderBehavior(TxtDelito, "Tipo de delito");
            AddPlaceholderBehavior(TxtAgraviado, "Nombre del agraviado");
            AddPlaceholderBehavior(TxtFechaAudiencia, "dd/mm/yyyy");
            AddPlaceholderBehavior(TxtHoraAudiencia, "hh:mm");
            AddPlaceholderBehavior(TxtHoraConclusion, "hh:mm");




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


            TxtFechaRecibo.Text = DateTime.Now.ToString("dd/MM/yyyy");

            TxtHoraRecibo.Text = DateTime.Now.ToString("hh:mm tt");
        }


        //─────────────────────────────────────────────
        // Metodos para autocompletar hora y fecha
        //─────────────────────────────────────────────
        private void TxtFechaAudiencia_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Ignorar placeholder
            if (TxtFechaAudiencia.Foreground.ToString() == "#FF9CA3AF")
                return;

            if (actualizandoFecha) return;

            actualizandoFecha = true;

            string texto = TxtFechaAudiencia.Text.Replace("/", "");

            // Solo números
            texto = new string(texto.Where(char.IsDigit).ToArray());

            // Máximo 8 dígitos
            if (texto.Length > 8)
                texto = texto.Substring(0, 8);

            // Insertar /
            if (texto.Length >= 5)
                texto = texto.Insert(4, "/").Insert(2, "/");
            else if (texto.Length >= 3)
                texto = texto.Insert(2, "/");

            TxtFechaAudiencia.Text = texto;
            TxtFechaAudiencia.SelectionStart = TxtFechaAudiencia.Text.Length;

            actualizandoFecha = false;
        }

        private void TxtHoraAudiencia_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Ignorar placeholder
            if (TxtHoraAudiencia.Foreground.ToString() == "#FF9CA3AF")
                return;

            if (actualizandoHora) return;

            actualizandoHora = true;

            string texto = TxtHoraAudiencia.Text.Replace(":", "");

            // Solo números
            texto = new string(texto.Where(char.IsDigit).ToArray());

            // Máximo 4 dígitos
            if (texto.Length > 4)
                texto = texto.Substring(0, 4);

            // Insertar :
            if (texto.Length >= 3)
                texto = texto.Insert(2, ":");

            TxtHoraAudiencia.Text = texto;
            TxtHoraAudiencia.SelectionStart = TxtHoraAudiencia.Text.Length;

            actualizandoHora = false;
        }

        private void TxtHoraConclusion_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Ignorar placeholder
            if (TxtHoraConclusion.Foreground.ToString() == "#FF9CA3AF")
                return;

            if (actualizandoHora)
                return;

            string textoOriginal = TxtHoraConclusion.Text;

            // Si ya contiene letras (AM/PM o edición manual), NO reformatear
            if (textoOriginal.Any(char.IsLetter))
                return;

            // Obtener solo números
            string numeros = new string(
                textoOriginal
                .Where(char.IsDigit)
                .ToArray());

            // SOLO convertir cuando haya EXACTAMENTE 4 dígitos
            if (numeros.Length != 4)
                return;

            actualizandoHora = true;

            int horas = int.Parse(numeros.Substring(0, 2));
            int minutos = int.Parse(numeros.Substring(2, 2));

            // Validar
            if (horas >= 0 && horas <= 23 &&
                minutos >= 0 && minutos <= 59)
            {
                DateTime hora = new DateTime(
                    1, 1, 1,
                    horas,
                    minutos,
                    0);

                TxtHoraConclusion.Text =
                    hora.ToString("hh:mm tt");

                TxtHoraConclusion.SelectionStart =
                    TxtHoraConclusion.Text.Length;
            }

            actualizandoHora = false;
        }

        // -─────────────────────────────────────────────
        //  JUZGADO
        //-─────────────────────────────────────────────
        private void CmbJuzgado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtJuzgadoOtro == null) return;

            var item = CmbJuzgado.SelectedItem as ComboBoxItem;
            bool esOtra = item?.Content?.ToString() == "Otra...";
            TxtJuzgadoOtro.Visibility = esOtra ? Visibility.Visible : Visibility.Collapsed;
        }

        // -─────────────────────────────────────────────
        //  TOTAL DISCO AUDIENCIA
        // -─────────────────────────────────────────────
        private void CmbTotDiscoAudiencia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtTotDiscoAudienciaOtro == null) return;

            var item = CmbTotDiscoAudiencia.SelectedItem as ComboBoxItem;
            bool esOtro = item?.Content?.ToString() == "Otro...";
            TxtTotDiscoAudienciaOtro.Visibility = esOtro ? Visibility.Visible : Visibility.Collapsed;
        }

        // ──────────────────────────────────────────
        // Cambios en el codigo para seleccion
        // ──────────────────────────────────────────
        private void FiltrarAutocomplete(TextBox txt, ListBox lst, List<string> origen)
        {
            string texto = txt.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(texto))
            {
                lst.Visibility = Visibility.Collapsed;
                return;
            }

            var resultados = origen
                .Where(x => x.ToLower().Contains(texto))
                .OrderBy(x => x.Length)
                .ToList();

            lst.ItemsSource = resultados;

            // ← ESTA LÍNEA ES LA CLAVE
            lst.SelectedIndex = -1;

            lst.Visibility = resultados.Any()
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void SeleccionarItem(ListBox lst)
        {
            if (lst.SelectedItem == null)
                return;

            StackPanel stack = (StackPanel)lst.Parent;

            TextBox txt = stack.Children
                               .OfType<TextBox>()
                               .First();

            txt.Text = lst.SelectedItem.ToString();

            txt.CaretIndex = txt.Text.Length;

            lst.Visibility = Visibility.Collapsed;

            txt.Focus();
        }

        private void TxtAutocomplete_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = (TextBox)sender;

            StackPanel stack = (StackPanel)txt.Parent;

            ListBox lst = stack.Children
                               .OfType<ListBox>()
                               .First();

            if (e.Key == Key.Down)
            {
                // Si no hay selección todavía
                if (lst.SelectedIndex == -1)
                {
                    lst.Focus();

                    lst.SelectedIndex = 0;
                }

                e.Handled = true;
            }

            if (e.Key == Key.Escape)
            {
                lst.Visibility = Visibility.Collapsed;

                e.Handled = true;
            }
        }

        private void lstAutocomplete_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ListBox lst = (ListBox)sender;

            // ENTER
            if (e.Key == Key.Enter)
            {
                SeleccionarItem(lst);

                e.Handled = true;
            }

            // ESC
            if (e.Key == Key.Escape)
            {
                lst.Visibility = Visibility.Collapsed;

                StackPanel stack = (StackPanel)lst.Parent;

                TextBox txt = stack.Children
                                   .OfType<TextBox>()
                                   .First();

                txt.Focus();

                e.Handled = true;
            }

            // ← ESTA PARTE ES LA NUEVA
            if (e.Key == Key.Back)
            {
                StackPanel stack = (StackPanel)lst.Parent;

                TextBox txt = stack.Children
                                   .OfType<TextBox>()
                                   .First();

                // regresar foco al textbox
                txt.Focus();

                // borrar último caracter
                if (txt.Text.Length > 0)
                {
                    txt.Text = txt.Text.Substring(0, txt.Text.Length - 1);

                    txt.CaretIndex = txt.Text.Length;
                }

                e.Handled = true;
            }
        }

        // -─────────────────────────────────────────────
        //  METODOS PARA AGREGAR JUEZ Y + Juez
        // ─────────────────────────────────────────────

        List<string> jueces = new List<string>()
         {
                  "Lic. García Ramírez",
                                "Lic. Torres Mendoza",
                               "Lic. Herrera López",
                                "Lic. Vargas Soto",
                                "Lic. Morales Díaz"

          };

        private void txtJuez_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = (TextBox)sender;

            StackPanel stack = (StackPanel)txt.Parent;

            ListBox lst = stack.Children
                               .OfType<ListBox>()
                               .First();

            FiltrarAutocomplete(txt, lst, jueces);
        }

        private void BtnAgregarJuez_Click(object sender, RoutedEventArgs e)
        {
            // Contenedor horizontal
            Grid grid = new Grid
            {
                Margin = new Thickness(0, 0, 0, 10)
            };

            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(42) });

            // StackPanel interno
            StackPanel stack = new StackPanel();

            // TextBox
            TextBox txtNuevoJuez = new TextBox
            {
                Height = 42,
                FontSize = 16,
                Tag = "Escriba el nombre del juez",
                Style = (Style)FindResource("InputStyle")
            };

            // Eventos
            txtNuevoJuez.TextChanged += txtJuez_TextChanged;
            txtNuevoJuez.PreviewKeyDown += TxtAutocomplete_PreviewKeyDown;

            // ListBox
            ListBox lstNuevaSugerencia = new ListBox
            {
                Height = 120,
                Visibility = Visibility.Collapsed
            };

            
            lstNuevaSugerencia.PreviewKeyDown += lstAutocomplete_PreviewKeyDown;

            // Agregar al stack
            stack.Children.Add(txtNuevoJuez);
            stack.Children.Add(lstNuevaSugerencia);

            Grid.SetColumn(stack, 0);

            // BOTÓN X
            Button btnEliminar = new Button
            {
                Content = "X",
                Width = 42,
                Height = 42,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Background = Brushes.IndianRed,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand
            };

            btnEliminar.Click += (s, ev) =>
            {
                PanelJuecesExtra.Children.Remove(grid);
            };

            Grid.SetColumn(btnEliminar, 2);

            // Agregar al grid
            grid.Children.Add(stack);
            grid.Children.Add(btnEliminar);

            // Agregar al panel principal
            PanelJuecesExtra.Children.Add(grid);

            // FOCUS AUTOMÁTICO
            txtNuevoJuez.Focus();

            // Cursor al final
            txtNuevoJuez.CaretIndex = txtNuevoJuez.Text.Length;
        }


        // ──────────────────────────────────────────
        //  METODO PARA AGREGAR DELITO Y + Delito
        // ──────────────────────────────────────────
        private List<string> delitos = new List<string>()
                {
                "Robo",
                "Fraude",
                "Homicidio",
                "Violencia Familiar",
                "Narcomenudeo",
                "Lesiones",
                "Abuso de confianza",
                "Daño en propiedad ajena"
                };

        private void TxtDelito_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = (TextBox)sender;

            StackPanel stack = (StackPanel)txt.Parent;

            ListBox lst = stack.Children
                               .OfType<ListBox>()
                               .First();

            FiltrarAutocomplete(txt, lst, delitos);
        }

        private void BtnAgregarDelito_Click(object sender, RoutedEventArgs e)
        {
            // Grid principal
            Grid grid = new Grid
            {
                Margin = new Thickness(0, 0, 0, 10)
            };

            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(42) });

            // Stack interno
            StackPanel stack = new StackPanel();

            // TextBox
            TextBox txtNuevoDelito = new TextBox
            {
                Height = 42,
                FontSize = 16,
                Tag = "Tipo de delito",
                Style = (Style)FindResource("InputStyle")
            };

            txtNuevoDelito.TextChanged += TxtDelito_TextChanged;
            txtNuevoDelito.PreviewKeyDown += TxtAutocomplete_PreviewKeyDown;

            // ListBox
            ListBox lstNuevoDelito = new ListBox
            {
                Height = 120,
                Visibility = Visibility.Collapsed
            };

            
            lstNuevoDelito.PreviewKeyDown += lstAutocomplete_PreviewKeyDown;

            // Agregar
            stack.Children.Add(txtNuevoDelito);
            stack.Children.Add(lstNuevoDelito);

            Grid.SetColumn(stack, 0);

            // Botón X
            Button btnEliminar = new Button
            {
                Content = "X",
                Width = 42,
                Height = 42,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Background = Brushes.IndianRed,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand
            };

            btnEliminar.Click += (s, ev) =>
            {
                PanelDelitoExtra.Children.Remove(grid);
            };

            Grid.SetColumn(btnEliminar, 2);

            // Agregar al grid
            grid.Children.Add(stack);
            grid.Children.Add(btnEliminar);

            // Agregar al panel principal
            PanelDelitoExtra.Children.Add(grid);

            // Focus automático
            txtNuevoDelito.Focus();
        }

       

        // ──────────────────────────────────────────
        //  METODO PARA AGREGAR AUDIENCIA y + Audiencia
        // ──────────────────────────────────────────
        private List<string> audiencia = new List<string>()
                {
                "Audiencia intermedia",
                "Audiencia Inicial de Formulación de Imputación",
                "Ratificacion de Medida de Proteccion-Concentradas",
                "Audiencia de Solicitud de Orden de Cateo",
                "Control Judicial",
                };

        private void TxtTipoAudiencia_TextChanged(object sender,TextChangedEventArgs e)
        {
            TextBox txt = (TextBox)sender;

            StackPanel stack = (StackPanel)txt.Parent;

            ListBox lst = stack.Children
                               .OfType<ListBox>()
                               .First();

            FiltrarAutocomplete(txt, lst, audiencia);
        }

        private void BtnAgregarAudiencia_Click(object sender, RoutedEventArgs e)
        {
            // Grid principal
            Grid grid = new Grid
            {
                Margin = new Thickness(0, 0, 0, 10)
            };

            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(42) });

            // Stack interno
            StackPanel stack = new StackPanel();

            // TextBox
            TextBox TxtTipoAudiencia = new TextBox
            {
                Height = 42,
                FontSize = 16,
                Tag = "Tipo de Audiencia",
                Style = (Style)FindResource("InputStyle")
            };

            TxtTipoAudiencia.TextChanged += TxtTipoAudiencia_TextChanged;
            TxtTipoAudiencia.PreviewKeyDown += TxtAutocomplete_PreviewKeyDown;


            ListBox lstAudiencia = new ListBox
            {
                Height = 120,
                Visibility = Visibility.Collapsed
            };

            lstAudiencia.PreviewKeyDown += lstAutocomplete_PreviewKeyDown;

            // Agregar
            stack.Children.Add(TxtTipoAudiencia);
            stack.Children.Add(lstAudiencia);

            Grid.SetColumn(stack, 0);

            // Botón X
            Button btnEliminar = new Button
            {
                Content = "X",
                Width = 42,
                Height = 42,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Background = Brushes.IndianRed,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand
            };

            btnEliminar.Click += (s, ev) =>
            {
                PanelAudienciaExtra.Children.Remove(grid);
            };

            lstAudiencia.MouseDoubleClick += (s, ev) =>
            {
                SeleccionarItem(lstAudiencia);
            };

            Grid.SetColumn(btnEliminar, 2);

            // Agregar al grid
            grid.Children.Add(stack);
            grid.Children.Add(btnEliminar);

            // Agregar al panel principal
            PanelAudienciaExtra.Children.Add(grid);

            // Focus automático
            TxtTipoAudiencia.Focus();
        }



        // ────────────────────────────────────────
        //  GUARDAR
        // -───────────────────────────────────────
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // ── Validaciones básicas ──
            if (string.IsNullOrWhiteSpace(TxtId.Text))
            {
                MessageBox.Show("El campo 'Id' es obligatorio.", "Validación",
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





            string juzgado = ObtenerValorComboOtro(CmbJuzgado, TxtJuzgadoOtro);
            string totAud = ObtenerValorComboOtro(CmbTotDiscoAudiencia, TxtTotDiscoAudienciaOtro);

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