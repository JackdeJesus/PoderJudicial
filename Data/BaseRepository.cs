using System;
using System.Collections.Generic;
using System.Data.OleDb;
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

        /// <summary>
        /// Consulta una columna en múltiples tablas y devuelve valores únicos.
        /// Omite tablas vacías automáticamente para no romper el autocompletado.
        /// </summary>
        protected List<string> ConsultarColumnaHistorica(string columna)
        {
            List<string> acumulado = new List<string>();

            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                foreach (string tabla in TableDetector.TodasLasTablas)
                {
                    try
                    {
                        // Antes se hacía un SELECT COUNT(*) previo para
                        // saltar tablas vacías, duplicando el roundtrip a
                        // Access por cada tabla histórica. El SELECT de
                        // abajo ya distingue "tabla vacía" de "tabla con
                        // datos" con el propio reader (si no hay filas, el
                        // while simplemente no itera), así que el COUNT
                        // previo era innecesario — se quita para reducir a
                        // la mitad las consultas de este método.
                        string sql =
                            $"SELECT [{columna}] FROM [{tabla}] " +
                            $"WHERE [{columna}] IS NOT NULL AND [{columna}] <> ''";

                        using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                acumulado.Add(reader[0]?.ToString());
                        }
                    }
                    catch (OleDbException)
                    {
                        // Si la tabla no tiene esa columna (esquema distinto),
                        // se omite silenciosamente
                    }
                }
            }

            return LimpiarLista(acumulado);
        }

    }
}