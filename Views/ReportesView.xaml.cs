using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ClosedXML.Excel;
using PoderJudicial.Data;
using PoderJudicial.Helpers;
using PoderJudicial.Models;

namespace PoderJudicial.Views
{
    public partial class ReportesView : Page
    {
        private readonly AudienciaData _data = new AudienciaData();
        private readonly CopiasData _copiasData = new CopiasData();

        private List<Audiencia> _todas = new();
        private List<Audiencia> _resultadosFiltrados = new();

        private List<RegistroCopia> _todasCopias = new();
        private List<RegistroCopia> _copiasFiltradas = new();

        private bool _cargando = true;

        // Catálogos separados:
        // - Entrega solo contiene nombres capturados en "Entregó".
        // - Recibe solo contiene nombres capturados en "Recibió".
        private readonly ObservableCollection<string> _catalogoEntregan = new();
        private readonly ObservableCollection<string> _catalogoReciben = new();

        // Personas seleccionadas para cada informe.
        private readonly ObservableCollection<string> _recibieronSimples = new();
        private readonly ObservableCollection<string> _recibieronAutenticas = new();

        private DateTime FechaInforme => DateTime.Today;

        public ReportesView()
        {
            InitializeComponent();

            LstRecibieronSimples.ItemsSource = _recibieronSimples;
            LstRecibieronAutenticas.ItemsSource = _recibieronAutenticas;

            CmbEntregoSimples.ItemsSource = _catalogoEntregan;
            CmbEntregoAutenticas.ItemsSource = _catalogoEntregan;

            CmbRecibioSimples.ItemsSource = _catalogoReciben;
            CmbRecibioAutenticas.ItemsSource = _catalogoReciben;

            Loaded += ReportesView_Loaded;
        }

        // ═══════════════════════════════════════════════════════════════
        // CARGA INICIAL
        // ═══════════════════════════════════════════════════════════════

        private void ReportesView_Loaded(object sender, RoutedEventArgs e)
        {
            RutasInformes.CrearEstructura();

            CargarCatalogosPersonas();
            ActualizarFechaInformeUI();
            CargarDatos();
            ActualizarEstadoBotones();
        }

        private void ActualizarFechaInformeUI()
        {
            int anioActual = FechaInforme.Year;

            TxtFechaInformeCopias.Text =
                $"Informe del {FechaInforme:dd/MM/yyyy}";

            TxtTituloInformeAnual.Text =
                $"2. Agregar al informe anual {anioActual}";

            TxtEncabezadoEstadoAnual.Text =
                $"Estado del informe anual {anioActual}";
        }

        private void CargarCatalogosPersonas()
        {
            try
            {
                CatalogoPersonasData catalogo =
                    PersonaCatalogoService.Cargar();

                _catalogoEntregan.Clear();
                _catalogoReciben.Clear();

                foreach (string nombre in catalogo.Entregan
                             .Where(x => !string.IsNullOrWhiteSpace(x))
                             .Distinct(StringComparer.OrdinalIgnoreCase)
                             .OrderBy(x => x))
                {
                    _catalogoEntregan.Add(nombre);
                }

                foreach (string nombre in catalogo.Reciben
                             .Where(x => !string.IsNullOrWhiteSpace(x))
                             .Distinct(StringComparer.OrdinalIgnoreCase)
                             .OrderBy(x => x))
                {
                    _catalogoReciben.Add(nombre);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible cargar el catálogo de personas.\n\n{ex.Message}",
                    "Aviso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void GuardarCatalogosPersonas()
        {
            PersonaCatalogoService.Guardar(
                _catalogoEntregan,
                _catalogoReciben);
        }

        private void CargarDatos()
        {
            try
            {
                _cargando = true;

                _todas = _data.ObtenerAudiencias();
                _todasCopias = _copiasData.ObtenerCopias();

                LlenarComboAnios();
                LlenarComboJuzgados();
                LlenarComboSalas();

                _cargando = false;

                AplicarFiltros();
                AplicarFiltrosCopias();
            }
            catch (Exception ex)
            {
                _cargando = false;

                MessageBox.Show(
                    $"Error al cargar datos:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // LLENADO DE COMBOS DE AUDIENCIAS
        // ═══════════════════════════════════════════════════════════════

        private void LlenarComboAnios()
        {
            var anios = _todas
                .Where(a => a.FechaAudiencia.HasValue)
                .Select(a => a.FechaAudiencia!.Value.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            CmbAnio.Items.Clear();
            CmbAnio.Items.Add(new ComboBoxItem { Content = "Todos" });

            foreach (var anio in anios)
                CmbAnio.Items.Add(new ComboBoxItem { Content = anio.ToString() });

            CmbAnio.SelectedIndex = 0;
        }

        private void LlenarComboJuzgados()
        {
            var juzgados = _todas
                .Select(a => a.Juzgado)
                .Where(j => !string.IsNullOrWhiteSpace(j))
                .Distinct()
                .OrderBy(j => j)
                .ToList();

            CmbJuzgado.Items.Clear();
            CmbJuzgado.Items.Add(new ComboBoxItem { Content = "Todos" });

            foreach (var j in juzgados)
                CmbJuzgado.Items.Add(new ComboBoxItem { Content = j });

            CmbJuzgado.SelectedIndex = 0;
        }

        private void LlenarComboSalas()
        {
            var salas = _todas
                .Select(a => a.Sala)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            CmbSala.Items.Clear();
            CmbSala.Items.Add(new ComboBoxItem { Content = "Todas" });

            foreach (var s in salas)
                CmbSala.Items.Add(new ComboBoxItem { Content = s });

            CmbSala.SelectedIndex = 0;
        }

        // ═══════════════════════════════════════════════════════════════
        // FILTROS DE AUDIENCIAS
        // ═══════════════════════════════════════════════════════════════

        private void Filtro_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (_cargando)
                return;

            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            string mes =
                (CmbMes.SelectedItem as ComboBoxItem)?.Content?.ToString()
                ?? "Todos";

            string anio =
                (CmbAnio.SelectedItem as ComboBoxItem)?.Content?.ToString()
                ?? "Todos";

            string juzgado =
                (CmbJuzgado.SelectedItem as ComboBoxItem)?.Content?.ToString()
                ?? "Todos";

            string sala =
                (CmbSala.SelectedItem as ComboBoxItem)?.Content?.ToString()
                ?? "Todas";

            string tipoCausa =
                (CmbTipoCausa.SelectedItem as ComboBoxItem)?.Content?.ToString()
                ?? "Todos";

            int? mesNum = ObtenerNumeroMes(mes);
            int? anioNum = int.TryParse(anio, out int a) ? a : null;

            var filtradas = _todas.AsEnumerable();

            if (mesNum.HasValue)
            {
                filtradas = filtradas.Where(x =>
                    x.FechaAudiencia.HasValue &&
                    x.FechaAudiencia.Value.Month == mesNum.Value);
            }

            if (anioNum.HasValue)
            {
                filtradas = filtradas.Where(x =>
                    x.FechaAudiencia.HasValue &&
                    x.FechaAudiencia.Value.Year == anioNum.Value);
            }

            if (juzgado != "Todos")
            {
                filtradas = filtradas.Where(x =>
                    string.Equals(
                        x.Juzgado,
                        juzgado,
                        StringComparison.OrdinalIgnoreCase));
            }

            if (sala != "Todas")
            {
                filtradas = filtradas.Where(x =>
                    string.Equals(
                        x.Sala,
                        sala,
                        StringComparison.OrdinalIgnoreCase));
            }

            if (tipoCausa != "Todos")
            {
                filtradas = filtradas.Where(x =>
                {
                    if (string.IsNullOrWhiteSpace(x.TipoCausa))
                        return false;

                    string valorDato =
                        x.TipoCausa.Replace(" ", "").Trim().ToUpperInvariant();

                    string valorFiltro =
                        tipoCausa.Replace(" ", "").Trim().ToUpperInvariant();

                    return valorDato == valorFiltro;
                });
            }

            List<Audiencia> resultado = filtradas.ToList();
            _resultadosFiltrados = resultado;

            TxtTotalRegistros.Text = resultado.Count.ToString();

            TxtTotalDiscos.Text = resultado.Sum(x =>
            {
                if (string.IsNullOrWhiteSpace(x.TotDiscoAudiencia))
                    return 0;

                string numeros =
                    new string(
                        x.TotDiscoAudiencia
                            .Where(char.IsDigit)
                            .ToArray());

                return int.TryParse(numeros, out int valor)
                    ? valor
                    : 0;
            }).ToString();

            int copiasSimples = resultado.Count(x =>
                !string.IsNullOrWhiteSpace(x.TipoDisco) &&
                NormalizarTexto(x.TipoDisco).Contains("SIMP"));

            int copiasAutenticas = resultado.Count(x =>
                !string.IsNullOrWhiteSpace(x.TipoDisco) &&
                NormalizarTexto(x.TipoDisco).Contains("AUT"));

            TxtCopiasSimples.Text = copiasSimples.ToString();
            TxtCopiasAutenticas.Text = copiasAutenticas.ToString();
        }

        private void AplicarFiltrosCopias()
        {
            _copiasFiltradas = _todasCopias
                .Where(c =>
                    c.FeRecibo.HasValue &&
                    c.FeRecibo.Value.Date == FechaInforme.Date)
                .OrderBy(c => c.FeRecibo)
                .ThenBy(c => c.Id)
                .ToList();
        }

        private static int? ObtenerNumeroMes(string nombre) => nombre switch
        {
            "Enero" => 1,
            "Febrero" => 2,
            "Marzo" => 3,
            "Abril" => 4,
            "Mayo" => 5,
            "Junio" => 6,
            "Julio" => 7,
            "Agosto" => 8,
            "Septiembre" => 9,
            "Octubre" => 10,
            "Noviembre" => 11,
            "Diciembre" => 12,
            _ => null
        };

        // ═══════════════════════════════════════════════════════════════
        // EXPORTAR EXCEL
        // ═══════════════════════════════════════════════════════════════

        private void BtnExportarExcel_Click(object sender, RoutedEventArgs e)
        {
            var datos = _resultadosFiltrados;

            if (datos == null || datos.Count == 0)
            {
                MessageBox.Show(
                    "No hay datos para exportar.",
                    "Info",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Guardar Excel",
                Filter = "Excel (*.xlsx)|*.xlsx",
                FileName = $"Reporte_Audiencias_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
            };

            if (dlg.ShowDialog() != true)
                return;

            try
            {
                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Audiencias");

                string[] headers =
                {
                    "Fecha Audiencia",
                    "Tot. Discos",
                    "Juzgado",
                    "Juez",
                    "No. Causa",
                    "NUC",
                    "Tipo Causa",
                    "Tipo Audiencia",
                    "Hora Conclusión",
                    "Imputado",
                    "Delito",
                    "Agraviado",
                    "Sala",
                    "No. Causa Juicio"
                };

                for (int i = 0; i < headers.Length; i++)
                    ws.Cell(1, i + 1).Value = headers[i];

                var headerRange = ws.Range(1, 1, 1, headers.Length);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F7A5C");
                headerRange.Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                headerRange.Style.Border.BottomBorder =
                    XLBorderStyleValues.Medium;

                for (int row = 0; row < datos.Count; row++)
                {
                    var aud = datos[row];
                    int r = row + 2;

                    ws.Cell(r, 1).Value =
                        aud.FechaAudiencia?.ToString("dd/MM/yyyy HH:mm") ?? "";

                    ws.Cell(r, 2).Value =
                        aud.TotDiscos.HasValue ? aud.TotDiscos.Value : "";

                    ws.Cell(r, 3).Value = aud.Juzgado ?? "";
                    ws.Cell(r, 4).Value = aud.Juez ?? "";
                    ws.Cell(r, 5).Value = aud.NoCausa ?? "";
                    ws.Cell(r, 6).Value = aud.NUC ?? "";
                    ws.Cell(r, 7).Value = aud.TipoCausa ?? "";
                    ws.Cell(r, 8).Value = aud.TipoAudiencia ?? "";

                    ws.Cell(r, 9).Value =
                        aud.HoraConclusion?.ToString("HH:mm") ?? "";

                    ws.Cell(r, 10).Value = aud.Imputado ?? "";
                    ws.Cell(r, 11).Value = aud.Delito ?? "";
                    ws.Cell(r, 12).Value = aud.Agraviado ?? "";
                    ws.Cell(r, 13).Value = aud.Sala ?? "";
                    ws.Cell(r, 14).Value = aud.NoCausaJuicio ?? "";

                    if (row % 2 == 1)
                    {
                        ws.Range(r, 1, r, headers.Length)
                            .Style.Fill.BackgroundColor =
                            XLColor.FromHtml("#F9FAFB");
                    }
                }

                var tableRange =
                    ws.Range(1, 1, datos.Count + 1, headers.Length);

                tableRange.Style.Border.OutsideBorder =
                    XLBorderStyleValues.Thin;

                tableRange.Style.Border.OutsideBorderColor =
                    XLColor.FromHtml("#D1D5DB");

                tableRange.Style.Border.InsideBorder =
                    XLBorderStyleValues.Thin;

                tableRange.Style.Border.InsideBorderColor =
                    XLColor.FromHtml("#E5E7EB");

                ws.Style.Font.FontName = "Arial";
                ws.Style.Font.FontSize = 10;

                int totalRow = datos.Count + 2;

                ws.Cell(totalRow, 1).Value =
                    $"Registros: {datos.Count}";

                ws.Cell(totalRow, 1).Style.Font.Bold = true;

                ws.Cell(totalRow, 14).Value =
                    "TOTAL DISCOS:";

                ws.Cell(totalRow, 14).Style.Font.Bold = true;

                ws.Cell(totalRow, 14)
                    .Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Right;

                ws.Cell(totalRow, 15).Value = datos.Sum(x =>
                {
                    if (string.IsNullOrWhiteSpace(x.TotDiscoAudiencia))
                        return 0;

                    string numeros =
                        new string(
                            x.TotDiscoAudiencia
                                .Where(char.IsDigit)
                                .ToArray());

                    return int.TryParse(numeros, out int valor)
                        ? valor
                        : 0;
                });

                ws.Cell(totalRow, 15).Style.Font.Bold = true;

                ws.Cell(totalRow, 15)
                    .Style.Font.FontColor =
                    XLColor.FromHtml("#1F7A5C");

                ws.Columns().AdjustToContents();

                foreach (int col in new[] { 4, 10, 11, 12, 20 })
                {
                    if (ws.Column(col).Width > 40)
                        ws.Column(col).Width = 40;
                }

                ws.SheetView.FreezeRows(1);
                wb.SaveAs(dlg.FileName);

                var res = MessageBox.Show(
                    $"Excel exportado exitosamente.\n{dlg.FileName}\n\n¿Deseas abrirlo ahora?",
                    "Éxito",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                if (res == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(
                        new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = dlg.FileName,
                            UseShellExecute = true
                        });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al exportar:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // EXPORTAR PDF
        // ═══════════════════════════════════════════════════════════════

        private void BtnExportarPdf_Click(object sender, RoutedEventArgs e)
        {
            var datos = _resultadosFiltrados;
            PdfExporter.Exportar(datos);
        }

        // ═══════════════════════════════════════════════════════════════
        // PERSONAS QUE RECIBIERON
        // ═══════════════════════════════════════════════════════════════

        private void BtnAgregarRecibioSimples_Click(
            object sender,
            RoutedEventArgs e)
        {
            AgregarPersonaRecibida(
                CmbRecibioSimples,
                _recibieronSimples);
        }

        private void BtnAgregarRecibioAutenticas_Click(
            object sender,
            RoutedEventArgs e)
        {
            AgregarPersonaRecibida(
                CmbRecibioAutenticas,
                _recibieronAutenticas);
        }

        private void AgregarPersonaRecibida(
            ComboBox combo,
            ObservableCollection<string> destino)
        {
            string nombre = combo.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show(
                    "Escribe o selecciona el nombre de la persona que recibió.",
                    "Información requerida",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                combo.Focus();
                return;
            }

            bool yaExiste = destino.Any(persona =>
                string.Equals(
                    persona,
                    nombre,
                    StringComparison.OrdinalIgnoreCase));

            if (yaExiste)
            {
                MessageBox.Show(
                    "Esta persona ya se encuentra en la lista.",
                    "Registro duplicado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                combo.Focus();
                return;
            }

            destino.Add(nombre);

            // IMPORTANTE:
            // Los nombres capturados en Recibió solamente van al catálogo Reciben.
            AgregarAlCatalogoReciben(nombre);

            LimpiarCombo(combo);
            combo.Focus();

            GuardarCatalogosPersonas();
        }

        private void BtnQuitarRecibioSimples_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is Button boton &&
                boton.CommandParameter is string nombre)
            {
                _recibieronSimples.Remove(nombre);
            }
        }

        private void BtnQuitarRecibioAutenticas_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is Button boton &&
                boton.CommandParameter is string nombre)
            {
                _recibieronAutenticas.Remove(nombre);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // CATÁLOGOS SEPARADOS
        // ═══════════════════════════════════════════════════════════════

        private void AgregarAlCatalogoEntregan(string nombre)
        {
            nombre = nombre?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(nombre))
                return;

            bool yaExiste = _catalogoEntregan.Any(persona =>
                string.Equals(
                    persona,
                    nombre,
                    StringComparison.OrdinalIgnoreCase));

            if (!yaExiste)
                _catalogoEntregan.Add(nombre);
        }

        private void AgregarAlCatalogoReciben(string nombre)
        {
            nombre = nombre?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(nombre))
                return;

            bool yaExiste = _catalogoReciben.Any(persona =>
                string.Equals(
                    persona,
                    nombre,
                    StringComparison.OrdinalIgnoreCase));

            if (!yaExiste)
                _catalogoReciben.Add(nombre);
        }

        // ═══════════════════════════════════════════════════════════════
        // GENERAR INFORME DE COPIAS SIMPLES
        // ═══════════════════════════════════════════════════════════════

        private void BtnGenerarCopiasSimples_Click(
            object sender,
            RoutedEventArgs e)
        {
            ActualizarFechaInformeUI();
            AplicarFiltrosCopias();

            string entrego =
                CmbEntregoSimples.Text?.Trim() ?? string.Empty;

            if (!ValidarDatosEntrega(
                    entrego,
                    _recibieronSimples,
                    "copias simples"))
            {
                return;
            }

            List<RegistroCopia> copiasSimples =
                ObtenerCopiasSimples();

            if (copiasSimples.Count == 0)
            {
                MessageBox.Show(
                    $"No se encontraron copias simples para el día " +
                    $"{FechaInforme:dd/MM/yyyy}.\n\n" +
                    $"Copias cargadas del día: {_copiasFiltradas.Count}",
                    "Sin registros",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            try
            {
                RutasInformes.CrearEstructura();

                string ruta =
                    RutasInformes.ObtenerRutaSimples(FechaInforme);

                WordExporter.GenerarInformeCopias(
                    copiasSimples,
                    "Copias Simples",
                    "DVD-R",
                    entrego,
                    _recibieronSimples,
                    ruta,
                    FechaInforme);

                if (!File.Exists(ruta))
                {
                    MessageBox.Show(
                        $"El proceso terminó, pero el archivo no fue encontrado.\n\n" +
                        $"Ruta esperada:\n{ruta}",
                        "Archivo no encontrado",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                // Solo el nombre capturado en Entregó se guarda en el catálogo Entregan.
                AgregarAlCatalogoEntregan(entrego);

                // Las personas que recibieron solo permanecen en el catálogo Reciben.
                foreach (string persona in _recibieronSimples)
                    AgregarAlCatalogoReciben(persona);

                GuardarCatalogosPersonas();

                TxtEstadoSimples.Text =
                    $"Estado: Generado a las {DateTime.Now:hh:mm tt}";

                ActualizarEstadoBotones();

                // Limpiar solamente los campos del informe que se acaba de generar.
                LimpiarCamposSimples();

                AbrirArchivoWord(ruta);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ocurrió un error al generar el informe de copias simples.\n\n" +
                    $"{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // GENERAR INFORME DE COPIAS AUTÉNTICAS
        // ═══════════════════════════════════════════════════════════════

        private void BtnGenerarCopiasAutenticas_Click(
            object sender,
            RoutedEventArgs e)
        {
            ActualizarFechaInformeUI();
            AplicarFiltrosCopias();

            string entrego =
                CmbEntregoAutenticas.Text?.Trim() ?? string.Empty;

            if (!ValidarDatosEntrega(
                    entrego,
                    _recibieronAutenticas,
                    "copias auténticas"))
            {
                return;
            }

            List<RegistroCopia> copiasAutenticas =
                ObtenerCopiasAutenticas();

            if (copiasAutenticas.Count == 0)
            {
                MessageBox.Show(
                    $"No se encontraron copias auténticas para el día " +
                    $"{FechaInforme:dd/MM/yyyy}.\n\n" +
                    $"Copias cargadas del día: {_copiasFiltradas.Count}",
                    "Sin registros",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            try
            {
                RutasInformes.CrearEstructura();

                string ruta =
                    RutasInformes.ObtenerRutaAutenticas(FechaInforme);

                WordExporter.GenerarInformeCopias(
                    copiasAutenticas,
                    "Copias Auténticas",
                    "DVD's",
                    entrego,
                    _recibieronAutenticas,
                    ruta,
                    FechaInforme);

                if (!File.Exists(ruta))
                {
                    MessageBox.Show(
                        $"El proceso terminó, pero el archivo no fue encontrado.\n\n" +
                        $"Ruta esperada:\n{ruta}",
                        "Archivo no encontrado",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                AgregarAlCatalogoEntregan(entrego);

                foreach (string persona in _recibieronAutenticas)
                    AgregarAlCatalogoReciben(persona);

                GuardarCatalogosPersonas();

                TxtEstadoAutenticas.Text =
                    $"Estado: Generado a las {DateTime.Now:hh:mm tt}";

                ActualizarEstadoBotones();

                LimpiarCamposAutenticas();

                AbrirArchivoWord(ruta);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ocurrió un error al generar el informe de copias auténticas.\n\n" +
                    $"{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // LIMPIEZA DE CAMPOS DESPUÉS DE GENERAR
        // ═══════════════════════════════════════════════════════════════

        private void LimpiarCamposSimples()
        {
            LimpiarCombo(CmbEntregoSimples);
            LimpiarCombo(CmbRecibioSimples);
            _recibieronSimples.Clear();
        }

        private void LimpiarCamposAutenticas()
        {
            LimpiarCombo(CmbEntregoAutenticas);
            LimpiarCombo(CmbRecibioAutenticas);
            _recibieronAutenticas.Clear();
        }

        private static void LimpiarCombo(ComboBox combo)
        {
            combo.SelectedItem = null;
            combo.SelectedIndex = -1;
            combo.Text = string.Empty;
        }

        private static void AbrirArchivoWord(string ruta)
        {
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo
                {
                    FileName = ruta,
                    UseShellExecute = true
                });
        }

        // ═══════════════════════════════════════════════════════════════
        // VALIDACIONES
        // ═══════════════════════════════════════════════════════════════

        private static bool ValidarDatosEntrega(
            string entrego,
            ObservableCollection<string> recibieron,
            string tipoInforme)
        {
            if (string.IsNullOrWhiteSpace(entrego))
            {
                MessageBox.Show(
                    $"Indica quién entregó el informe de {tipoInforme}.",
                    "Información requerida",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            if (recibieron.Count == 0)
            {
                MessageBox.Show(
                    $"Agrega al menos una persona que recibió el informe de {tipoInforme}.",
                    "Información requerida",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            return true;
        }

        // ═══════════════════════════════════════════════════════════════
        // SEPARAR COPIAS POR TIPO
        // ═══════════════════════════════════════════════════════════════

        private List<RegistroCopia> ObtenerCopiasSimples()
        {
            return _copiasFiltradas
                .Where(EsCopiaSimple)
                .ToList();
        }

        private List<RegistroCopia> ObtenerCopiasAutenticas()
        {
            return _copiasFiltradas
                .Where(EsCopiaAutentica)
                .ToList();
        }

        private static bool EsCopiaSimple(RegistroCopia copia)
        {
            string tipo =
                NormalizarTexto(copia?.TipoDisco ?? string.Empty);

            return tipo.Contains("SIMP");
        }

        private static bool EsCopiaAutentica(RegistroCopia copia)
        {
            string tipo =
                NormalizarTexto(copia?.TipoDisco ?? string.Empty);

            return tipo.Contains("AUT");
        }

        private static string NormalizarTexto(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return string.Empty;

            string texto = valor
                .Trim()
                .ToUpperInvariant()
                .Normalize(NormalizationForm.FormD);

            var caracteres = texto.Where(c =>
                CharUnicodeInfo.GetUnicodeCategory(c) !=
                UnicodeCategory.NonSpacingMark);

            return new string(caracteres.ToArray())
                .Replace(" ", string.Empty)
                .Normalize(NormalizationForm.FormC);
        }

        // ═══════════════════════════════════════════════════════════════
        // ESTADOS
        // ═══════════════════════════════════════════════════════════════

        private void ActualizarEstadoBotones()
        {
            string rutaSimples =
                RutasInformes.ObtenerRutaSimples(FechaInforme);

            string rutaAutenticas =
                RutasInformes.ObtenerRutaAutenticas(FechaInforme);

            string rutaConsolidado =
                RutasInformes.ObtenerRutaConsolidado(FechaInforme);

            bool existeSimples = File.Exists(rutaSimples);
            bool existeAutenticas = File.Exists(rutaAutenticas);
            bool existeConsolidado = File.Exists(rutaConsolidado);

            if (existeSimples)
            {
                DateTime modificacion =
                    File.GetLastWriteTime(rutaSimples);

                TxtEstadoSimples.Text =
                    $"Estado: Generado a las {modificacion:hh:mm tt}";
            }
            else
            {
                TxtEstadoSimples.Text =
                    "Estado: No generado";
            }

            if (existeAutenticas)
            {
                DateTime modificacion =
                    File.GetLastWriteTime(rutaAutenticas);

                TxtEstadoAutenticas.Text =
                    $"Estado: Generado a las {modificacion:hh:mm tt}";
            }
            else
            {
                TxtEstadoAutenticas.Text =
                    "Estado: No generado";
            }

            BtnConsolidarInformeDiario.IsEnabled =
                existeSimples &&
                existeAutenticas &&
                !existeConsolidado;

            BtnAgregarInformeAnual.IsEnabled =
                existeConsolidado;

            if (existeConsolidado)
            {
                TxtEstadoConsolidado.Text =
                    "Estado: Informe diario consolidado";
            }
            else if (existeSimples && existeAutenticas)
            {
                TxtEstadoConsolidado.Text =
                    "Estado: Listo para consolidar";
            }
            else
            {
                TxtEstadoConsolidado.Text =
                    "Pendiente de ambos informes";
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // CONSOLIDACIÓN 
        // ═══════════════════════════════════════════════════════════════

        private void BtnAgregarInformeAnual_Click(
    object sender,
    RoutedEventArgs e)
        {
            try
            {
                string rutaConsolidado =
                    RutasInformes.ObtenerRutaConsolidado(
                        FechaInforme);

                if (!File.Exists(rutaConsolidado))
                {
                    MessageBox.Show(
                        "Primero debes consolidar los informes de copias simples y auténticas.",
                        "Consolidación requerida",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    ActualizarEstadoBotones();
                    return;
                }

                bool yaExiste =
                    InformeCopiasService
                        .EstaAgregadoAlAnual(
                            FechaInforme);

                MessageBoxResult confirmar =
                    MessageBox.Show(
                        yaExiste
                            ? $"El informe del {FechaInforme:dd/MM/yyyy} ya existe en Informes_{FechaInforme.Year}.docx.\n\n" +
                              "Se reemplazará únicamente la versión de este día por la versión más reciente.\n\n" +
                              "¿Deseas continuar?"
                            : $"El informe del {FechaInforme:dd/MM/yyyy} se agregará a Informes_{FechaInforme.Year}.docx.\n\n" +
                              "¿Deseas continuar?",
                        yaExiste
                            ? "Actualizar informe anual"
                            : "Agregar al informe anual",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                if (confirmar != MessageBoxResult.Yes)
                    return;

                string rutaAnual =
                    InformeCopiasService
                        .AgregarOActualizarInformeAnual(
                            FechaInforme);

                ActualizarEstadoBotones();

                int totalInformes =
                    InformeCopiasService
                        .ContarInformesEnAnual(
                            FechaInforme.Year);

                MessageBox.Show(
                    yaExiste
                        ? $"El informe del {FechaInforme:dd/MM/yyyy} fue actualizado correctamente.\n\n" +
                          $"Informes registrados en {FechaInforme.Year}: {totalInformes}."
                        : $"El informe del {FechaInforme:dd/MM/yyyy} fue agregado correctamente.\n\n" +
                          $"Informes registrados en {FechaInforme.Year}: {totalInformes}.",
                    "Informe anual actualizado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                AbrirArchivo(
                    rutaAnual);
            }
            catch (IOException ex)
            {
                MessageBox.Show(
                    $"No fue posible actualizar Informes_{FechaInforme.Year}.docx.\n\n" +
                    "Si el documento anual está abierto en Word, ciérralo e inténtalo nuevamente.\n\n" +
                    ex.Message,
                    "Informe anual en uso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ocurrió un error al actualizar el informe anual.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }



        private void BtnConsolidarInformeDiario_Click(
    object sender,
    RoutedEventArgs e)
        {
            try
            {
                string rutaSimples =
                    RutasInformes.ObtenerRutaSimples(
                        DateTime.Today);

                string rutaAutenticas =
                    RutasInformes.ObtenerRutaAutenticas(
                        DateTime.Today);

                if (!File.Exists(rutaSimples) ||
                    !File.Exists(rutaAutenticas))
                {
                    MessageBox.Show(
                        "Para consolidar primero deben existir los dos informes del día.",
                        "No se puede consolidar",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    return;
                }

                string rutaConsolidado =
                    InformeCopiasService.ConsolidarInformeDelDia(
                        DateTime.Today);

                ActualizarEstadoBotones();

                MessageBoxResult respuesta =
                    MessageBox.Show(
                        "Los informes se consolidaron correctamente.\n\n" +
                        "¿Deseas abrir el documento consolidado?",
                        "Consolidación",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);

                if (respuesta == MessageBoxResult.Yes)
                {
                    AbrirArchivo(rutaConsolidado);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al consolidar:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private static void AbrirArchivo(string ruta)
        {
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo
                {
                    FileName = ruta,
                    UseShellExecute = true
                });
        }









    }
}
