using System;
using System.Collections.Generic;
using System.Linq;
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
        // ── Capa de datos 
        private readonly AudienciaData _data = new AudienciaData();
        private List<Audiencia> _todas = new();
        private bool _cargando = true;

        public ReportesView()
        {
            InitializeComponent();
            Loaded += ReportesView_Loaded;
        }

       
        //  CARGA INICIAL

        private void ReportesView_Loaded(object sender, RoutedEventArgs e) => CargarDatos();

        private void CargarDatos()
        {
            try
            {
                _cargando = true;
                _todas = _data.ObtenerAudiencias();
                LlenarComboAnios();
                LlenarComboJuzgados();
                LlenarComboSalas();
                _cargando = false;
                AplicarFiltros();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos:\n{ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        //  LLENADO DINÁMICO DE COMBOS
       
        private void LlenarComboAnios()
        {
            var anios = _todas
                .Where(a => a.FechaAudiencia.HasValue)
                .Select(a => a.FechaAudiencia!.Value.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            CmbAnio.Items.Clear();
            CmbAnio.Items.Add(new ComboBoxItem { Content = "Todos", IsSelected = true });
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
            CmbJuzgado.Items.Add(new ComboBoxItem { Content = "Todos", IsSelected = true });
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
            CmbSala.Items.Add(new ComboBoxItem { Content = "Todas", IsSelected = true });
            foreach (var s in salas)
                CmbSala.Items.Add(new ComboBoxItem { Content = s });
            CmbSala.SelectedIndex = 0;
        }

        
        //  FILTRADO
      
        private void Filtro_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (_cargando) return;
            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            string mes = (CmbMes.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Todos";
            string anio = (CmbAnio.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Todos";
            string juzgado = (CmbJuzgado.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Todos";
            string sala = (CmbSala.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Todas";

            int? mesNum = ObtenerNumeroMes(mes);
            int? anioNum = int.TryParse(anio, out int a) ? a : null;

            var filtradas = _todas.AsEnumerable();

            if (mesNum.HasValue)
                filtradas = filtradas.Where(x =>
                    x.FechaAudiencia.HasValue &&
                    x.FechaAudiencia.Value.Month == mesNum.Value);

            if (anioNum.HasValue)
                filtradas = filtradas.Where(x =>
                    x.FechaAudiencia.HasValue &&
                    x.FechaAudiencia.Value.Year == anioNum.Value);

            if (juzgado != "Todos")
                filtradas = filtradas.Where(x =>
                    string.Equals(x.Juzgado, juzgado, StringComparison.OrdinalIgnoreCase));

            if (sala != "Todas")
                filtradas = filtradas.Where(x =>
                    string.Equals(x.Sala, sala, StringComparison.OrdinalIgnoreCase));

            var resultado = filtradas.ToList();

            DgResultados.ItemsSource = resultado;
            TxtTotalRegistros.Text = resultado.Count.ToString();
            TxtTotalDiscos.Text = resultado.Sum(x => x.TotDiscos ?? 0).ToString();
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

        //  EXPORTAR EXCEL  —  .xlsx real con ClosedXML
        //  NuGet: Install-Package ClosedXML
        
        private void BtnExportarExcel_Click(object sender, RoutedEventArgs e)
        {
            var datos = DgResultados.ItemsSource as List<Audiencia>;
            if (datos == null || datos.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Info",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Guardar Excel",
                Filter = "Excel (*.xlsx)|*.xlsx",
                FileName = $"Reporte_Audiencias_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
            };
            if (dlg.ShowDialog() != true) return;

            try
            {
                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Audiencias");

                // ── Encabezados 
                string[] headers =
                {
                    "ID", "Fecha Audiencia", "Juzgado", "Juez", "No. Causa", "NUC",
                    "Tipo Audiencia", "Tipo Causa", "Sala", "Imputado", "Agraviado",
                    "Delito", "Fecha Recibo", "Hora Conclusión", "Tot. Discos",
                    "Tipo Disco", "Tot. Disco Audiencia", "No. Causa Juicio",
                    
                };

                for (int i = 0; i < headers.Length; i++)
                    ws.Cell(1, i + 1).Value = headers[i];

                var headerRange = ws.Range(1, 1, 1, headers.Length);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F7A5C");
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Medium;

                // ── Datos
              
                for (int row = 0; row < datos.Count; row++)
                {
                    var a = datos[row];
                    int r = row + 2;

                    ws.Cell(r, 1).Value = a.Id;
                    ws.Cell(r, 2).Value = a.FechaAudiencia?.ToString("dd/MM/yyyy") ?? "";
                    ws.Cell(r, 3).Value = a.Juzgado ?? "";
                    ws.Cell(r, 4).Value = a.Juez ?? "";
                    ws.Cell(r, 5).Value = a.NoCausa ?? "";
                    ws.Cell(r, 6).Value = a.NUC ?? "";
                    ws.Cell(r, 7).Value = a.TipoAudiencia ?? "";
                    ws.Cell(r, 8).Value = a.TipoCausa ?? "";
                    ws.Cell(r, 9).Value = a.Sala ?? "";
                    ws.Cell(r, 10).Value = a.Imputado ?? "";
                    ws.Cell(r, 11).Value = a.Agraviado ?? "";
                    ws.Cell(r, 12).Value = a.Delito ?? "";
                    ws.Cell(r, 13).Value = a.FechaRecibo?.ToString("dd/MM/yyyy") ?? "";
                    ws.Cell(r, 14).Value = a.HoraConclusion?.ToString("HH:mm") ?? "";
                    ws.Cell(r, 17).Value = a.TotDiscoAudiencia ?? "";
                    ws.Cell(r, 18).Value = a.NoCausaJuicio ?? "";
                    

                    if (row % 2 == 1)
                        ws.Range(r, 1, r, headers.Length)
                          .Style.Fill.BackgroundColor = XLColor.FromHtml("#F9FAFB");
                }

                // ── Bordes
                var tableRange = ws.Range(1, 1, datos.Count + 1, headers.Length);
                tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                tableRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#D1D5DB");
                tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                tableRange.Style.Border.InsideBorderColor = XLColor.FromHtml("#E5E7EB");

                ws.Style.Font.FontName = "Arial";
                ws.Style.Font.FontSize = 10;

                // ── Fila de totales 
                int totalRow = datos.Count + 2;
                ws.Cell(totalRow, 1).Value = $"Registros: {datos.Count}";
                ws.Cell(totalRow, 1).Style.Font.Bold = true;

                ws.Cell(totalRow, 14).Value = "TOTAL DISCOS:";
                ws.Cell(totalRow, 14).Style.Font.Bold = true;
                ws.Cell(totalRow, 14).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                ws.Cell(totalRow, 15).Value = datos.Sum(x => x.TotDiscos ?? 0);
                ws.Cell(totalRow, 15).Style.Font.Bold = true;
                ws.Cell(totalRow, 15).Style.Font.FontColor = XLColor.FromHtml("#1F7A5C");

                // ── Anchos automáticos
                ws.Columns().AdjustToContents();
                foreach (int col in new[] { 4, 10, 11, 12, 20 })
                    if (ws.Column(col).Width > 40) ws.Column(col).Width = 40;

                ws.SheetView.FreezeRows(1);

                wb.SaveAs(dlg.FileName);

                var res = MessageBox.Show(
                    $"Excel exportado exitosamente.\n{dlg.FileName}\n\n¿Deseas abrirlo ahora?",
                    "Éxito", MessageBoxButton.YesNo, MessageBoxImage.Information);

                if (res == MessageBoxResult.Yes)
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = dlg.FileName,
                        UseShellExecute = true
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar:\n{ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  EXPORTAR PDF  —  delega a Helpers/PdfExporter.cs
        // ═══════════════════════════════════════════════════════════════
        private void BtnExportarPdf_Click(object sender, RoutedEventArgs e)
        {
            var datos = DgResultados.ItemsSource as List<Audiencia>;
            PdfExporter.Exportar(datos);
        }
    }
}
