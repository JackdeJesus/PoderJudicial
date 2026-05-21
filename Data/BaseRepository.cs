using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PoderJudicial.Data
{
    public class BaseRepository
    {
        protected List<string> LimpiarLista(IEnumerable<string> datos)
        {
            return datos
                // Separar registros compuestos
                .SelectMany(SepararRegistros)

                // Limpiar texto
                .Select(LimpiarTexto)

                // Quitar vacíos
                .Where(x => !string.IsNullOrWhiteSpace(x))

                // Quitar duplicados ignorando mayúsculas
                .Distinct(StringComparer.OrdinalIgnoreCase)

                // Ordenar
                .OrderBy(x => x)

                .ToList();
        }

        private IEnumerable<string> SepararRegistros(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return Enumerable.Empty<string>();

            // Separar por:
            // y
            // ,
            // ;
            // /
            string[] separadores =
            {
                " y ",
                ",",
                ";",
                "/"
            };

            return texto.Split(
                separadores,
                StringSplitOptions.RemoveEmptyEntries
            );
        }

        private string LimpiarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            texto = texto.Trim();

            // Espacios dobles
            texto = Regex.Replace(texto, @"\s+", " ");

            // Quitar puntos dobles
            texto = texto.Replace("..", ".");

            // Normalizar LIC
            texto = Regex.Replace(
                texto,
                @"^lic\.?\s*",
                "Lic. ",
                RegexOptions.IgnoreCase
            );

            // Primera letra mayúscula
            texto = Capitalizar(texto);

            return texto;
        }

        private string Capitalizar(string texto)
        {
            return System.Globalization.CultureInfo
                .CurrentCulture
                .TextInfo
                .ToTitleCase(texto.ToLower());
        }
    }
}