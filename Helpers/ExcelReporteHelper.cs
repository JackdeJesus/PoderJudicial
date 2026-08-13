using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using PoderJudicial.Models;

namespace PoderJudicial.Helpers
{
    public static class ExcelReporteHelper
    {
        public static void ExportarAudiencias(IReadOnlyList<Audiencia> datos, string rutaArchivo)
        {
            if (datos == null) throw new ArgumentNullException(nameof(datos));
            if (string.IsNullOrWhiteSpace(rutaArchivo)) throw new ArgumentException("La ruta del archivo es obligatoria.", nameof(rutaArchivo));

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Audiencias");

            string[] headers =
            {
                "Fecha Audiencia", "Tot. Discos", "Juzgado", "Juez", "No. Causa", "NUC",
                "Tipo Causa", "Tipo Audiencia", "Hora Conclusión", "Imputado", "Delito",
                "Agraviado", "Sala", "No. Causa Juicio"
            };

            for (int i = 0; i < headers.Length; i++)
                ws.Cell(1, i + 1).Value = headers[i];

            var headerRange = ws.Range(1, 1, 1, headers.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F7A5C");
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Medium;

            for (int row = 0; row < datos.Count; row++)
            {
                var aud = datos[row];
                int r = row + 2;

                ws.Cell(r, 1).Value = aud.FechaAudiencia?.ToString("dd/MM/yyyy HH:mm") ?? "";
                if (aud.TotDiscos.HasValue)
                    ws.Cell(r, 2).Value = aud.TotDiscos.Value;
                else
                    ws.Cell(r, 2).Value = "";

                ws.Cell(r, 3).Value = aud.Juzgado ?? "";
                ws.Cell(r, 4).Value = aud.Juez ?? "";
                ws.Cell(r, 5).Value = aud.NoCausa ?? "";
                ws.Cell(r, 6).Value = aud.NUC ?? "";
                ws.Cell(r, 7).Value = aud.TipoCausa ?? "";
                ws.Cell(r, 8).Value = aud.TipoAudiencia ?? "";
                ws.Cell(r, 9).Value = aud.HoraConclusion?.ToString("HH:mm") ?? "";
                ws.Cell(r, 10).Value = aud.Imputado ?? "";
                ws.Cell(r, 11).Value = aud.Delito ?? "";
                ws.Cell(r, 12).Value = aud.Agraviado ?? "";
                ws.Cell(r, 13).Value = aud.Sala ?? "";
                ws.Cell(r, 14).Value = aud.NoCausaJuicio ?? "";

                if (row % 2 == 1)
                    ws.Range(r, 1, r, headers.Length).Style.Fill.BackgroundColor = XLColor.FromHtml("#F9FAFB");
            }

            var tableRange = ws.Range(1, 1, datos.Count + 1, headers.Length);
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#D1D5DB");
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorderColor = XLColor.FromHtml("#E5E7EB");

            ws.Style.Font.FontName = "Arial";
            ws.Style.Font.FontSize = 10;

            int totalRow = datos.Count + 2;
            ws.Cell(totalRow, 1).Value = $"Registros: {datos.Count}";
            ws.Cell(totalRow, 1).Style.Font.Bold = true;
            ws.Cell(totalRow, 14).Value = "TOTAL DISCOS:";
            ws.Cell(totalRow, 14).Style.Font.Bold = true;
            ws.Cell(totalRow, 14).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            ws.Cell(totalRow, 15).Value = datos.Sum(x =>
            {
                if (string.IsNullOrWhiteSpace(x.TotDiscoAudiencia)) return 0;
                string numeros = new string(x.TotDiscoAudiencia.Where(char.IsDigit).ToArray());
                return int.TryParse(numeros, out int valor) ? valor : 0;
            });

            ws.Cell(totalRow, 15).Style.Font.Bold = true;
            ws.Cell(totalRow, 15).Style.Font.FontColor = XLColor.FromHtml("#1F7A5C");
            ws.Columns().AdjustToContents();

            foreach (int col in new[] { 4, 10, 11, 12, 20 })
            {
                if (ws.Column(col).Width > 40)
                    ws.Column(col).Width = 40;
            }

            ws.SheetView.FreezeRows(1);
            wb.SaveAs(rutaArchivo);
        }
    }
}
