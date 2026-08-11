using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PoderJudicial.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PoderJudicial.Helpers
{
    public static class WordExporter
    {
        private const string CargoEntrega =
            "Jefe de Unidad de Informática del Juzgado en Materia Penal " +
            "Acusatorio y Oral para el Circuito de Pachuca de Soto, Hidalgo.";

        private const string CargoRecibeAutenticas =
            "Jefe de Unidad de Causa del Juzgado en Materia Penal " +
            "Acusatorio y Oral para el Circuito de Pachuca de Soto, Hidalgo.";

        private const string CargoRecibeSimples =
            "Encargado de Atención Ciudadana del Juzgado en Materia Penal " +
            "Acusatorio y Oral para el Circuito de Pachuca de Soto, Hidalgo.";

        public static void GenerarInformeCopias(
            List<RegistroCopia> datos,
            string tipoTitulo,
            string tipoDvd,
            string entrego,
            IEnumerable<string> recibieron,
            string rutaArchivo,
            DateTime fechaInforme)
        {
            if (datos == null || datos.Count == 0)
            {
                throw new ArgumentException(
                    "No existen registros para generar el informe.",
                    nameof(datos));
            }

            if (string.IsNullOrWhiteSpace(entrego))
            {
                throw new ArgumentException(
                    "Debe indicar quién realizó la entrega.",
                    nameof(entrego));
            }

            List<string> listaRecibieron =
                (recibieron ?? Enumerable.Empty<string>())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

            if (listaRecibieron.Count == 0)
            {
                throw new ArgumentException(
                    "Debe indicar al menos una persona que recibió.",
                    nameof(recibieron));
            }

            string? carpeta =
                Path.GetDirectoryName(rutaArchivo);

            if (!string.IsNullOrWhiteSpace(carpeta))
                Directory.CreateDirectory(carpeta);

            if (File.Exists(rutaArchivo))
                File.Delete(rutaArchivo);

            using WordprocessingDocument documento =
                WordprocessingDocument.Create(
                    rutaArchivo,
                    WordprocessingDocumentType.Document);

            MainDocumentPart partePrincipal =
                documento.AddMainDocumentPart();

            partePrincipal.Document =
                new Document(new Body());

            Body cuerpo =
                partePrincipal.Document.Body!;

            AgregarSeccion(
                cuerpo,
                datos,
                tipoTitulo,
                tipoDvd,
                entrego.Trim(),
                listaRecibieron,
                fechaInforme);

            partePrincipal.Document.Save();
        }

        private static void AgregarSeccion(
            Body cuerpo,
            List<RegistroCopia> datos,
            string tipoTitulo,
            string tipoDvd,
            string entrego,
            IReadOnlyCollection<string> recibieron,
            DateTime fechaInforme)
        {
            CultureInfo cultura =
                new CultureInfo("es-MX");

            int total =
                datos.Sum(
                    x => x.TotDiscosEntregados ?? 0);

            AgregarParrafo(
                cuerpo,
                $"Entrega de {tipoDvd} de {tipoTitulo}",
                true,
                24,
                JustificationValues.Center);

            AgregarParrafo(
                cuerpo,
                "Unidad de Informática",
                true,
                24,
                JustificationValues.Center);

            AgregarParrafo(
                cuerpo,
                $"Pachuca de Soto, Hgo., " +
                $"{fechaInforme.ToString("d 'de' MMMM 'de' yyyy", cultura)}",
                true,
                20,
                JustificationValues.Right);

            string introduccion =
                $"Se hace entrega de {total} {tipoDvd} ({total} {tipoTitulo}),";

            string continuacion =
                " que contiene la videograbación de audiencias del Juzgado en Materia " +
                "Penal Acusatorio y Oral para el Circuito de Pachuca de Soto, Hidalgo, " +
                "debidamente revisado y etiquetado, para los efectos legales que correspondan " +
                "a solicitud del Jefe de Unidad de Causa o las Partes.";

            AgregarParrafoMixto(
                cuerpo,
                introduccion,
                continuacion,
                18,
                JustificationValues.Both);

            AgregarParrafo(
                cuerpo,
                "Información General de la Video Audiencia:",
                true,
                18,
                JustificationValues.Left);

            Table tabla =
                CrearTablaPrincipal();

            string[] encabezados =
            {
                "Causa",
                "NUC",
                "Fecha",
                "Tipo de Copia",
                "Solicita",
                "Discos",
                "Observaciones"
            };

            tabla.Append(
                new TableRow(
                    encabezados
                        .Select(x =>
                            (OpenXmlElement)CrearCeldaEncabezado(x))
                        .ToArray()));

            foreach (RegistroCopia copia in datos)
            {
                tabla.Append(
                    new TableRow(
                        CrearCeldaDato(copia.NoCausa),
                        CrearCeldaDato(copia.NUC),
                        CrearCeldaDato(
                            copia.FeAudiencia?
                                .ToString(
                                    "dddd, d 'de' MMMM 'de' yyyy",
                                    cultura)
                            ?? string.Empty),
                        CrearCeldaDato(copia.TipoDisco),
                        CrearCeldaDato(copia.AQuienSeEntrega),
                        CrearCeldaDato(
                            (copia.TotDiscosEntregados ?? 0)
                                .ToString()),
                        CrearCeldaDato(copia.Observaciones)));
            }

            tabla.Append(
                new TableRow(
                    CrearCeldaTotal(""),
                    CrearCeldaTotal(""),
                    CrearCeldaTotal(""),
                    CrearCeldaTotal(""),
                    CrearCeldaTotal("TOTAL"),
                    CrearCeldaTotal(total.ToString()),
                    CrearCeldaTotal("")));

            cuerpo.Append(tabla);

            AgregarEspacio(
                cuerpo,
                12);

            string cargoRecibe =
                ObtenerCargoRecibe(tipoTitulo);

            AgregarTablaEntregaRecibe(
                cuerpo,
                entrego,
                recibieron,
                cargoRecibe);
        }

        private static Table CrearTablaPrincipal()
        {
            var propiedades =
                new TableProperties(
                    new TableWidth
                    {
                        Type = TableWidthUnitValues.Dxa,
                        Width = "11066"
                    },
                    new TableJustification
                    {
                        Val = TableRowAlignmentValues.Center
                    },
                    new TableLayout
                    {
                        Type = TableLayoutValues.Autofit
                    },
                    new TableBorders(
                        new TopBorder
                        {
                            Val = BorderValues.Single,
                            Size = 6
                        },
                        new BottomBorder
                        {
                            Val = BorderValues.Single,
                            Size = 6
                        },
                        new LeftBorder
                        {
                            Val = BorderValues.Single,
                            Size = 6
                        },
                        new RightBorder
                        {
                            Val = BorderValues.Single,
                            Size = 6
                        },
                        new InsideHorizontalBorder
                        {
                            Val = BorderValues.Single,
                            Size = 4
                        },
                        new InsideVerticalBorder
                        {
                            Val = BorderValues.Single,
                            Size = 4
                        }));

            return new Table(propiedades);
        }

        private static TableCell CrearCeldaEncabezado(
            string texto)
        {
            var runProps =
                CrearPropiedadesTexto(
                    20,
                    true);

            var parrafo =
                CrearParrafoTabla(
                    texto,
                    runProps);

            return new TableCell(
                new TableCellProperties(
                    new Shading
                    {
                        Val = ShadingPatternValues.Clear,
                        Color = "auto",
                        Fill = "C0C0C0"
                    },
                    new TableCellVerticalAlignment
                    {
                        Val = TableVerticalAlignmentValues.Center
                    }),
                parrafo);
        }

        private static TableCell CrearCeldaDato(
            string texto)
        {
            var runProps =
                CrearPropiedadesTexto(
                    12,
                    false);

            var parrafo =
                CrearParrafoTabla(
                    texto,
                    runProps);

            return new TableCell(
                new TableCellProperties(
                    new TableCellVerticalAlignment
                    {
                        Val = TableVerticalAlignmentValues.Center
                    }),
                parrafo);
        }

        private static TableCell CrearCeldaTotal(
            string texto)
        {
            var runProps =
                CrearPropiedadesTexto(
                    18,
                    true);

            var parrafo =
                CrearParrafoTabla(
                    texto,
                    runProps);

            // Sin sombreado.
            return new TableCell(
                new TableCellProperties(
                    new TableCellVerticalAlignment
                    {
                        Val = TableVerticalAlignmentValues.Center
                    }),
                parrafo);
        }

        private static Paragraph CrearParrafoTabla(
            string texto,
            RunProperties propiedadesTexto)
        {
            return new Paragraph(
                new ParagraphProperties(
                    new Justification
                    {
                        Val = JustificationValues.Center
                    },
                    new SpacingBetweenLines
                    {
                        Before = "0",
                        After = "0"
                    }),
                new Run(
                    propiedadesTexto,
                    new Text(texto ?? string.Empty)
                    {
                        Space = SpaceProcessingModeValues.Preserve
                    }));
        }

        private static RunProperties CrearPropiedadesTexto(
            int tamanoOpenXml,
            bool negrita)
        {
            var propiedades =
                new RunProperties(
                    new RunFonts
                    {
                        Ascii = "Calibri",
                        HighAnsi = "Calibri",
                        EastAsia = "Calibri"
                    },
                    new FontSize
                    {
                        Val = tamanoOpenXml.ToString()
                    },
                    new FontSizeComplexScript
                    {
                        Val = tamanoOpenXml.ToString()
                    });

            if (negrita)
                propiedades.Append(new Bold());

            return propiedades;
        }

        private static void AgregarTablaEntregaRecibe(
            Body cuerpo,
            string entrego,
            IReadOnlyCollection<string> recibieron,
            string cargoRecibe)
        {
            var tabla =
                new Table(
                    new TableProperties(
                        new TableWidth
                        {
                            Type = TableWidthUnitValues.Pct,
                            Width = "5000"
                        },
                        new TableJustification
                        {
                            Val = TableRowAlignmentValues.Center
                        },
                        new TableLayout
                        {
                            Type = TableLayoutValues.Fixed
                        },
                        new TableBorders(
                            new TopBorder
                            {
                                Val = BorderValues.Nil
                            },
                            new BottomBorder
                            {
                                Val = BorderValues.Nil
                            },
                            new LeftBorder
                            {
                                Val = BorderValues.Nil
                            },
                            new RightBorder
                            {
                                Val = BorderValues.Nil
                            },
                            new InsideHorizontalBorder
                            {
                                Val = BorderValues.Nil
                            },
                            new InsideVerticalBorder
                            {
                                Val = BorderValues.Nil
                            })));

            var fila =
                new TableRow();

            fila.Append(
                CrearCeldaFirma(
                    "Entrega",
                    new[] { entrego },
                    CargoEntrega),
                CrearCeldaFirma(
                    "Recibe",
                    recibieron,
                    cargoRecibe));

            tabla.Append(fila);
            cuerpo.Append(tabla);
        }

        private static TableCell CrearCeldaFirma(
            string titulo,
            IEnumerable<string> nombres,
            string cargo)
        {
            var celda =
                new TableCell(
                    new TableCellProperties(
                        new TableCellWidth
                        {
                            Type = TableWidthUnitValues.Pct,
                            Width = "2500"
                        },
                        new TableCellVerticalAlignment
                        {
                            Val = TableVerticalAlignmentValues.Top
                        },
                        new TableCellBorders(
                            new TopBorder
                            {
                                Val = BorderValues.Nil
                            },
                            new BottomBorder
                            {
                                Val = BorderValues.Nil
                            },
                            new LeftBorder
                            {
                                Val = BorderValues.Nil
                            },
                            new RightBorder
                            {
                                Val = BorderValues.Nil
                            },
                            new InsideHorizontalBorder
                            {
                                Val = BorderValues.Nil
                            },
                            new InsideVerticalBorder
                            {
                                Val = BorderValues.Nil
                            })));

            celda.Append(
                CrearParrafoFirma(
                    titulo,
                    true,
                    16));

            foreach (string nombre in
                     (nombres ?? Enumerable.Empty<string>())
                         .Where(x => !string.IsNullOrWhiteSpace(x))
                         .Select(x => x.Trim())
                         .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                celda.Append(
                    CrearParrafoFirma(
                        nombre,
                        false,
                        16));
            }

            celda.Append(
                CrearParrafoFirma(
                    cargo,
                    false,
                    16));

            return celda;
        }

        private static Paragraph CrearParrafoFirma(
            string texto,
            bool negrita,
            int tamanoOpenXml)
        {
            RunProperties props =
                CrearPropiedadesTexto(
                    tamanoOpenXml,
                    negrita);

            return new Paragraph(
                new ParagraphProperties(
                    new Justification
                    {
                        Val = JustificationValues.Center
                    },
                    new SpacingBetweenLines
                    {
                        Before = "0",
                        After = "0"
                    }),
                new Run(
                    props,
                    new Text(texto ?? string.Empty)
                    {
                        Space = SpaceProcessingModeValues.Preserve
                    }));
        }

        private static string ObtenerCargoRecibe(
            string tipoTitulo)
        {
            string normalizado =
                NormalizarTexto(tipoTitulo);

            return normalizado.Contains("AUT")
                ? CargoRecibeAutenticas
                : CargoRecibeSimples;
        }

        private static string NormalizarTexto(
            string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return string.Empty;

            string texto =
                valor
                    .Trim()
                    .ToUpperInvariant()
                    .Normalize(
                        System.Text.NormalizationForm.FormD);

            IEnumerable<char> caracteres =
                texto.Where(c =>
                    CharUnicodeInfo.GetUnicodeCategory(c)
                    != UnicodeCategory.NonSpacingMark);

            return new string(caracteres.ToArray())
                .Normalize(
                    System.Text.NormalizationForm.FormC);
        }

        private static void AgregarEspacio(
            Body cuerpo,
            int tamanoOpenXml)
        {
            cuerpo.Append(
                new Paragraph(
                    new Run(
                        new RunProperties(
                            new FontSize
                            {
                                Val = tamanoOpenXml.ToString()
                            }),
                        new Text(" "))));
        }

        private static void AgregarParrafo(
            Body cuerpo,
            string texto,
            bool negrita,
            int tamanoOpenXml,
            JustificationValues alineacion)
        {
            RunProperties props =
                CrearPropiedadesTexto(
                    tamanoOpenXml,
                    negrita);

            cuerpo.Append(
                new Paragraph(
                    new ParagraphProperties(
                        new Justification
                        {
                            Val = alineacion
                        },
                        new SpacingBetweenLines
                        {
                            Before = "0",
                            After = "0"
                        }),
                    new Run(
                        props,
                        new Text(texto ?? string.Empty)
                        {
                            Space = SpaceProcessingModeValues.Preserve
                        })));
        }

        private static void AgregarParrafoMixto(
            Body cuerpo,
            string textoNegrita,
            string textoNormal,
            int tamanoOpenXml,
            JustificationValues alineacion)
        {
            RunProperties propsNegrita =
                CrearPropiedadesTexto(
                    tamanoOpenXml,
                    true);

            RunProperties propsNormal =
                CrearPropiedadesTexto(
                    tamanoOpenXml,
                    false);

            cuerpo.Append(
                new Paragraph(
                    new ParagraphProperties(
                        new Justification
                        {
                            Val = alineacion
                        },
                        new SpacingBetweenLines
                        {
                            Before = "0",
                            After = "0"
                        }),
                    new Run(
                        propsNegrita,
                        new Text(textoNegrita ?? string.Empty)
                        {
                            Space = SpaceProcessingModeValues.Preserve
                        }),
                    new Run(
                        propsNormal,
                        new Text(textoNormal ?? string.Empty)
                        {
                            Space = SpaceProcessingModeValues.Preserve
                        })));
        }
    }
}
