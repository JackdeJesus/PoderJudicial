using PoderJudicial.Data;
using PoderJudicial.Helpers;
using PoderJudicial.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;



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

            CargarIdVisual();

            // Cargar datos para listas autocompletar

            AudienciaRepository audienciaRepo = new AudienciaRepository();

            historialAudiencias =
    new AudienciaData()
    .ObtenerAudiencias();


            JuezRepository juezRepo = new JuezRepository();
            
            jueces = juezRepo.ObtenerJueces();

            //metodo hora real
            IniciarReloj();

            // Placeholder visual para los TextBox con Tag
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





        }



        private void CargarIdVisual()
        {
            try
            {
                int id =
                    new AudienciaData()
                    .ObtenerSiguienteIdVisual();

                TxtId.Text = id.ToString();
            }
            catch
            {
                TxtId.Text = "---";
            }
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
        private void TxtFechaAudiencia_TextChanged(object sender,TextChangedEventArgs e)
        {
            TextBox txt = (TextBox)sender;

            if (PlaceholderHelper.IsPlaceholder(txt))
                return;

            string textoOriginal = txt.Text;

            // Si contiene letras → salir
            if (textoOriginal.Any(char.IsLetter))
                return;

            // Solo números
            string numeros = new string(
                textoOriginal
                .Where(char.IsDigit)
                .ToArray());

            // SOLO cuando tenga 8 dígitos
            if (numeros.Length != 8)
                return;

            string fecha =
                numeros.Insert(4, "/")
                       .Insert(2, "/");

            txt.Text = fecha;

            txt.CaretIndex = txt.Text.Length;
        }

        private void TxtHoraAudiencia_TextChanged(object sender,TextChangedEventArgs e)
        {
            TextBox txt = (TextBox)sender;

            if (PlaceholderHelper.IsPlaceholder(txt))
                return;

            string textoOriginal = txt.Text;

            // Si contiene letras → salir
            if (textoOriginal.Any(char.IsLetter))
                return;

            // Solo números
            string numeros = new string(
                textoOriginal
                .Where(char.IsDigit)
                .ToArray());

            // SOLO cuando tenga 4 dígitos
            if (numeros.Length != 4)
                return;

            int horas =
                int.Parse(numeros.Substring(0, 2));

            int minutos =
                int.Parse(numeros.Substring(2, 2));

            // Validar
            if (horas < 0 || horas > 23)
                return;

            if (minutos < 0 || minutos > 59)
                return;

            string hora =
                $"{horas:D2}:{minutos:D2}";

            txt.Text = hora;

            txt.CaretIndex = txt.Text.Length;
        }

        private void TxtHoraConclusion_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Ignorar placeholder
            if (PlaceholderHelper.IsPlaceholder(TxtHoraConclusion))
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



        //--------------
        private List<Audiencia> historialAudiencias;

        // -─────────────────────────────────────────────
        //  METODOS PARA AGREGAR JUEZ Y + Juez
        // ─────────────────────────────────────────────

        private List<string> jueces;
        private void txtJuez_TextChanged(object sender,TextChangedEventArgs e)
        {
            TextBox txt = (TextBox)sender;

            string texto = ObtenerTexto(txt);

            // Ignorar placeholder o vacío
            if (string.IsNullOrWhiteSpace(texto))
                return;

            AutocompleteHelper
                .FiltrarDesdeSender(
                    sender,
                    jueces);
        }

        private void BtnAgregarJuez_Click(object sender, RoutedEventArgs e)
        {
            AgregarCampoJuez();
        }


        // ──────────────────────────────────────────
        //  METODO PARA AGREGAR DELITO Y + Delito
        // ──────────────────────────────────────────


        private void TxtDelito_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = (TextBox)sender;

            string texto = ObtenerTexto(txt);

            // Ignorar placeholder o vacío
            if (string.IsNullOrWhiteSpace(texto))
                return;

            string tipoCausa =
                ObtenerTipoCausaSeleccionado();

            List<string> delitosFiltrados =
                historialAudiencias
                .Where(x =>
                    x.TipoCausa == tipoCausa &&
                    !string.IsNullOrWhiteSpace(x.Delito))
                .Select(x => x.Delito)
                .Distinct()
                .ToList();

            AutocompleteHelper
                .FiltrarDesdeSender(
                    sender,
                    delitosFiltrados);
        }

        private void BtnAgregarDelito_Click(object sender,RoutedEventArgs e)
        {
            DynamicFieldFactory.CrearCampoAutocomplete(
                PanelDelitoExtra,
                "Tipo de delito",
                TxtDelito_TextChanged,
                TxtAutocomplete_PreviewKeyDown,
                lstAutocomplete_PreviewKeyDown,
                lstAutocomplete_MouseClick,
                (s, ev) =>
                {
                    Button btn = (Button)s;

                    Grid grid =
                        (Grid)btn.Parent;

                    PanelDelitoExtra
                        .Children
                        .Remove(grid);
                },
                (Style)FindResource("InputStyle"));
        }



        // ──────────────────────────────────────────
        //  METODO PARA AGREGAR AUDIENCIA y + Audiencia
        // ──────────────────────────────────────────


        private void TxtTipoAudiencia_TextChanged(object sender,TextChangedEventArgs e)
        {
            string tipoCausa =
                ObtenerTipoCausaSeleccionado();

            List<string> audienciasFiltradas =
                historialAudiencias
                .Where(x =>
                    x.TipoCausa == tipoCausa &&
                    !string.IsNullOrWhiteSpace(x.TipoAudiencia))
                .Select(x => x.TipoAudiencia)
                .Distinct()
                .ToList();

            AutocompleteHelper
                .FiltrarDesdeSender(
                    sender,
                    audienciasFiltradas);
        }

        private void BtnAgregarAudiencia_Click(object sender,RoutedEventArgs e)
        {
            DynamicFieldFactory.CrearCampoAutocomplete(
                PanelAudienciaExtra,
                "Tipo de Audiencia",
                TxtTipoAudiencia_TextChanged,
                TxtAutocomplete_PreviewKeyDown,
                lstAutocomplete_PreviewKeyDown,
                lstAutocomplete_MouseClick,
                (s, ev) =>
                {
                    Button btn = (Button)s;

                    Grid grid =
                        (Grid)btn.Parent;

                    PanelAudienciaExtra
                        .Children
                        .Remove(grid);
                },
                (Style)FindResource("InputStyle"));
        }


        // ────────────────────────────────────────
        //  GUARDAR
        // -───────────────────────────────────────
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario())
                return;

            // ── Recopilar valores ─────────────────────────────────────────────────
            string juzgado = ObtenerValorComboOtro(CmbJuzgado, TxtJuzgadoOtro);
            string totAud = ObtenerValorComboOtro(CmbTotDiscoAudiencia, TxtTotDiscoAudienciaOtro);
            string Causa = ObtenerValorCombo(CmbTipoCausa);
            string tipoDiscoTexto = ObtenerValorCombo(CmbTipoDisco);
            // Campos con paneles dinámicos — combina el TextBox principal
            // con los campos extra que el usuario haya agregado con "+"
            string juezFinal = ObtenerTextosPanelDinamico(PanelJuecesExtra, TxtJuez);
            string delitoFinal = ObtenerTextosPanelDinamico(PanelDelitoExtra, TxtDelito);
            string audienciaFinal = ObtenerTextosPanelDinamico(PanelAudienciaExtra, TxtTipoAudiencia);
            string cantidadDiscos = tipoDiscoTexto.Split(' ')[0];

            //PARSEAR FECHA Y HORA DE AUDIENCIA EN UN SOLO CAMPO PARA GUARDAR EN LA BD (si es posible)

            DateTime? fechaAudiencia = null;

            string fechaAudienciaTexto =
                $"{ObtenerTexto(TxtFechaAudiencia)} {ObtenerTexto(TxtHoraAudiencia)}";

            if (DateTime.TryParse(
                fechaAudienciaTexto,
                out DateTime fechaAud))
            {
                fechaAudiencia = fechaAud;
            }


            //--------
            DateTime? fechaRecibo = null;

            string fechaReciboTexto =
                $"{TxtFechaRecibo.Text} {TxtHoraRecibo.Text}";

            if (DateTime.TryParse(
                fechaReciboTexto,
                out DateTime fechaRec))
            {
                fechaRecibo = fechaRec;
            }

            //----------------
            DateTime? horaConclusion = null;

            if (DateTime.TryParse(
                ObtenerTexto(TxtHoraConclusion),
                out DateTime hora))
            {
                horaConclusion = hora;
            }

            //------------- 
            int? totalDiscos = null;

            if (int.TryParse(cantidadDiscos, out int discos))
            {
                totalDiscos = discos;
            }



            // ── Construir modelo ──────────────────────────────────────────────────
            var registro = new Audiencia
            {
                Id = int.Parse(TxtId.Text),
                NoCausa = ObtenerTexto(TxtNoCausa),
                NUC = ObtenerTexto(TxtNUC),
                FechaAudiencia = fechaAudiencia,
                FechaRecibo = fechaRecibo,
                TipoAudiencia = audienciaFinal,
                TipoCausa = Causa,         
                Juzgado = juzgado,
                Juez = juezFinal,
                Imputado = ObtenerTexto(TxtImputado),
                Delito = delitoFinal,
                Agraviado = ObtenerTexto(TxtAgraviado),
                Sala = ObtenerValorCombo(CmbSala),
                HoraConclusion = horaConclusion,
                NoCausaJuicio = ObtenerTexto(TxtNoCausaJuicio),
                Diferida = string.Empty,          // sin campo en el form aún
                QuienRealiza = SesionActual.Usuario,         // sin campo en el form aún
                TotDiscos = totalDiscos,
                TipoDisco = "Archivo",          
                TotDiscoAudiencia = totAud
            };

            // ── Insertar ──────────────────────────────────────────────────────────
            try
            {
                new AudienciaData().Insertar(registro);

                MessageBox.Show("Registro guardado correctamente.", "Éxito",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                LimpiarFormulario();
                CargarIdVisual();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar:\n{ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        /// <summary>
           /// Devuelve el Content del item seleccionado en un ComboBox simple
            /// (sin opción "Otro..." con TextBox).
        /// </summary>
          private string ObtenerValorCombo(ComboBox combo)
          {
               var item = combo.SelectedItem as ComboBoxItem;
               if (item == null) return string.Empty;

              string content = item.Content?.ToString() ?? string.Empty;

              // Si el item es un placeholder de selección, devolver vacío
              if (content.StartsWith("Seleccione")) return string.Empty;

              return content;
          }


        private string ObtenerTipoCausaSeleccionado()
        {
            var item =
                CmbTipoCausa.SelectedItem as ComboBoxItem;

            if (item == null)
                return string.Empty;

            return item.Content?.ToString() ?? "";
        }


        // ──────────────────────────────────────────
        //  HELPERS PRIVADOS
        // ──────────────────────────────────────────

        /// <summary>
        /// Devuelve string.Empty si el TextBox tiene placeholder o está vacío,
        /// de lo contrario devuelve el texto limpio.
        /// </summary>
        private string ObtenerTexto(TextBox txt)
        {
            if (txt == null)
                return string.Empty;

            string texto = txt.Text?.Trim() ?? "";

            // PlaceholderHelper
            if (PlaceholderHelper.IsPlaceholder(txt))
                return string.Empty;

            // Placeholders manuales/dinámicos
            string[] placeholders =
            {
        "Nombre del juez",
        "Escriba el nombre del juez",
        "Tipo de delito",
        "Tipo de Audiencia",
        "Escriba el tipo de audiencia",
        "Ej: 123/2024",
        "Ej: 89/2024",
        "Ej: 12-2024-00567",
        "hh:mm",
        "dd/MM/yyyy"
    };

            if (placeholders.Contains(texto))
                return string.Empty;

            return texto;
        }


        private bool ValidarFormulario()
        {
            string tipoCausa =
                ObtenerValorCombo(CmbTipoCausa);

            // ─────────────────────────────
            // ID
            // ─────────────────────────────
            if (ValidationHelper.CampoVacio(TxtId))
            {
                MessageBox.Show(
                    "El campo 'Id' es obligatorio.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            //______________
            //  NO CAUSA
            //--------------

            string noCausa = ObtenerTexto(TxtNoCausa);

            if (!ValidationHelper.NumerosYDiagonal(noCausa))
            {
                MessageBox.Show(
                    "El campo 'No. Causa' solo permite números y '/'.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }



            if (ValidationHelper.CampoVacio(TxtNoCausa))
            {
                MessageBox.Show(
                    "El campo 'No. Causa' es obligatorio.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }



            //-----------------------
            //  NUC
            //-----------------------

            if (ValidationHelper.CampoVacio(TxtNUC))
            {
                MessageBox.Show(
                    "El campo 'NUC' es obligatorio.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }






            // ─────────────────────────────
            // JUZGADO
            // ─────────────────────────────
            var juzgadoItem =
                CmbJuzgado.SelectedItem as ComboBoxItem;

            if (juzgadoItem == null ||
                juzgadoItem.Content.ToString() ==
                "Seleccione juzgado")
            {
                MessageBox.Show(
                    "El campo 'Juzgado' es obligatorio.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            // ─────────────────────────────
            // NO CAUSA
            // ─────────────────────────────
            if (!ValidationHelper.NumerosYDiagonal(
                ObtenerTexto(TxtNoCausa)))
            {
                MessageBox.Show(
                    "El campo 'No. Causa' solo permite números y '/'.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            // ─────────────────────────────
            // FECHA AUDIENCIA
            // ─────────────────────────────
            if (!ValidationHelper.FechaValida(
                ObtenerTexto(TxtFechaAudiencia)))
            {
                MessageBox.Show(
                    "La fecha de audiencia no es válida.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            // ─────────────────────────────
            // HORA AUDIENCIA
            // ─────────────────────────────
            if (!ValidationHelper.HoraValida(
                ObtenerTexto(TxtHoraAudiencia)))
            {
                MessageBox.Show(
                    "La hora de audiencia no es válida.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            // ─────────────────────────────
            // HORA CONCLUSIÓN
            // ─────────────────────────────
            if (!ValidationHelper.HoraValida(
                ObtenerTexto(TxtHoraConclusion)))
            {
                MessageBox.Show(
                    "La hora de conclusión no es válida.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            // ─────────────────────────────
            // JUEZ OBLIGATORIO
            // ─────────────────────────────
            string juezFinal =
                ObtenerTextosPanelDinamico(
                    PanelJuecesExtra,
                    TxtJuez);

            if (string.IsNullOrWhiteSpace(juezFinal))
            {
                MessageBox.Show(
                    "Debe capturar al menos un juez.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            // ─────────────────────────────
            // TIPO AUDIENCIA
            // ─────────────────────────────
            string audienciaFinal =
                ObtenerTextosPanelDinamico(
                    PanelAudienciaExtra,
                    TxtTipoAudiencia);

            if (string.IsNullOrWhiteSpace(audienciaFinal))
            {
                MessageBox.Show(
                    "El tipo de audiencia es obligatorio.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            // ─────────────────────────────
            // VALIDACIONES PARA C y JO
            // ─────────────────────────────
            if (tipoCausa != "CP")
            {
                if (string.IsNullOrWhiteSpace(
                    ObtenerTexto(TxtImputado)))
                {
                    MessageBox.Show(
                        "El imputado es obligatorio.",
                        "Validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return false;
                }

                string delitoFinal =
                    ObtenerTextosPanelDinamico(
                        PanelDelitoExtra,
                        TxtDelito);

                if (string.IsNullOrWhiteSpace(delitoFinal))
                {
                    MessageBox.Show(
                        "El delito es obligatorio.",
                        "Validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return false;
                }

                if (string.IsNullOrWhiteSpace(
                    ObtenerTexto(TxtAgraviado)))
                {
                    MessageBox.Show(
                        "El agraviado es obligatorio.",
                        "Validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return false;
                }
            }

            // ─────────────────────────────
            // NO CAUSA JUICIO SOLO JO
            // ─────────────────────────────
            if (tipoCausa == "JO")
            {
                if (string.IsNullOrWhiteSpace(
                    ObtenerTexto(TxtNoCausaJuicio)))
                {
                    MessageBox.Show(
                        "El No. Causa Juicio es obligatorio para JO.",
                        "Validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return false;
                }

                if (!ValidationHelper.NumerosYDiagonal(
                    ObtenerTexto(TxtNoCausaJuicio)))
                {
                    MessageBox.Show(
                        "No. Causa Juicio solo permite números y '/'.",
                        "Validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Combina el TextBox principal con los campos dinámicos del panel,
        /// separados por " / ". Ignora placeholders y vacíos.
        /// </summary>
        private string ObtenerTextosPanelDinamico(StackPanel panel, TextBox txtPrincipal)
        {
            var valores = new List<string>();

            // Campo principal
            string valorPrincipal = ObtenerTexto(txtPrincipal);
            if (!string.IsNullOrWhiteSpace(valorPrincipal))
                valores.Add(valorPrincipal);

            // Campos agregados dinámicamente con el botón "+"
            foreach (var child in panel.Children)
            {
                if (!(child is Grid grid)) continue;

                foreach (var gridChild in grid.Children)
                {
                    if (!(gridChild is StackPanel sp)) continue;

                    TextBox txt = sp.Children.OfType<TextBox>().FirstOrDefault();
                    if (txt == null) continue;

                    string valor = ObtenerTexto(txt);
                    if (!string.IsNullOrWhiteSpace(valor))
                        valores.Add(valor);
                }
            }

            return string.Join(" / ", valores);
        }

        /// <summary>
        /// Limpia el formulario y restaura todos los placeholders
        /// después de guardar exitosamente.
        /// </summary>
        private void LimpiarFormulario()
        {
            // Limpiar TextBox
            TxtId.Text = string.Empty;
            TxtNoCausa.Text = string.Empty;
            TxtNUC.Text = string.Empty;
            TxtFechaAudiencia.Text = string.Empty;
            TxtTipoAudiencia.Text = string.Empty;
            TxtImputado.Text = string.Empty;
            TxtDelito.Text = string.Empty;
            TxtAgraviado.Text = string.Empty;
            TxtHoraConclusion.Text = string.Empty;
            TxtNoCausaJuicio.Text = string.Empty;
            TxtJuez.Text = string.Empty;
            TxtHoraAudiencia.Text = string.Empty;
            TxtHoraRecibo.Text = string.Empty;
            TxtFechaRecibo.Text = string.Empty;

            // Limpiar ComboBox
            CmbJuzgado.SelectedIndex = 1;
            CmbTotDiscoAudiencia.SelectedIndex = 1;
            CmbSala.SelectedIndex = 1;  // ← agregar aquí
            CmbTipoCausa.SelectedIndex = 1;
            CmbTipoDisco.SelectedIndex = 0;


            // Limpiar paneles dinámicos
            PanelJuecesExtra.Children.Clear();
            PanelDelitoExtra.Children.Clear();
            PanelAudienciaExtra.Children.Clear();

            // Restaurar placeholders (mismo orden que el constructor)
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


        }

        


        private void lstAutocomplete_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ListBox lst = (ListBox)sender;

            AutocompleteHelper
                .ManejarTecladoListBox(lst, e);
        }
        private void lstAutocomplete_MouseClick(object sender,MouseButtonEventArgs e)
        {
            ListBox lst = (ListBox)sender;

            AutocompleteHelper
                .ManejarClickMouse(lst);
        }




        private void AgregarCampoJuez()
        {
            DynamicFieldFactory.CrearCampoAutocomplete(
                PanelJuecesExtra,
                "Escriba el nombre del juez",
                txtJuez_TextChanged,
                TxtAutocomplete_PreviewKeyDown,
                lstAutocomplete_PreviewKeyDown,
                lstAutocomplete_MouseClick,
                (s, ev) =>
                {
                    Button btn = (Button)s;

                    Grid grid = (Grid)btn.Parent;

                    PanelJuecesExtra
                        .Children
                        .Remove(grid);
                },
                (Style)FindResource("InputStyle"));
        }


        private void CmbTipoCausa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            if (CmbTipoCausa.SelectedItem is not ComboBoxItem item)
                return;

            string tipo = item.Content.ToString();

            // Reset base
            PanelNoCausaJuicio.Visibility = Visibility.Collapsed;

            TxtNoCausaJuicio.Text = "";

            // Limpiar jueces extra
            PanelJuecesExtra.Children.Clear();

            switch (tipo)
            {
                // ─────────────────────────────
                // C → solo 1 juez
                // ─────────────────────────────
                case "C":

                    BtnAgregarJuez.Visibility =
                        Visibility.Collapsed;

                    break;

                // ─────────────────────────────
                // CP → solo 1 juez
                // ─────────────────────────────
                case "CP":

                    BtnAgregarJuez.Visibility =
                        Visibility.Collapsed;

                    break;

                // ─────────────────────────────
                // JO → múltiples jueces
                // ─────────────────────────────
                case "JO":

                    PanelNoCausaJuicio.Visibility =
                        Visibility.Visible;

                    BtnAgregarJuez.Visibility =
                        Visibility.Visible;

                    // Agregar automáticamente
                    // un juez extra
                    if (PanelJuecesExtra.Children.Count == 0)
                    {
                        AgregarCampoJuez();
                    }

                    break;
            }
        }


        private void SoloNumeros_PreviewTextInput(
    object sender,
    TextCompositionEventArgs e)
        {
            e.Handled =
                !e.Text.All(char.IsDigit);
        }

        private void NoCausa_PreviewTextInput(
    object sender,
    TextCompositionEventArgs e)
        {
            char c = e.Text.FirstOrDefault();

            e.Handled =
                !(char.IsDigit(c) || c == '/');
        }




    }


}