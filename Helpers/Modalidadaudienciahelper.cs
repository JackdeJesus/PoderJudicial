using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoderJudicial.Helpers
{
    /// <summary>
    /// Encapsula el criterio para registrar si una audiencia se realizó por
    /// videoconferencia dentro de columnas existentes (Quien Realiza / Observaciones),
    /// sin requerir cambios de esquema en la base de datos.
    /// </summary>
    public static class ModalidadAudienciaHelper
    {
        private const string Etiqueta = "Videoconferencia";
        private const string Separador = " / ";

        /// <summary>
        /// Construye el texto a guardar en "Quien Realiza" u "Observaciones".
        /// Ej: "Antonio" (presencial) o "Antonio / Videoconferencia" (videoconferencia).
        /// </summary>
        public static string ConstruirRegistro(string usuario, bool esVideoconferencia)
        {
            usuario = usuario?.Trim() ?? string.Empty;

            return esVideoconferencia
                ? $"{usuario}{Separador}{Etiqueta}"
                : usuario;
        }

        /// <summary>
        /// Interpreta un texto ya guardado (de "Quien Realiza" u "Observaciones")
        /// para saber si corresponde a una audiencia por videoconferencia.
        /// Útil al cargar un registro para editarlo o visualizarlo.
        /// </summary>
        public static bool EsVideoconferencia(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return false;
            return texto.IndexOf(Etiqueta, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Devuelve únicamente el nombre de usuario, sin la etiqueta de modalidad,
        /// por si se necesita mostrar/editar el nombre de forma aislada.
        /// </summary>
        public static string ExtraerUsuario(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;
            int idx = texto.IndexOf(Separador, StringComparison.OrdinalIgnoreCase);
            return idx >= 0 ? texto.Substring(0, idx).Trim() : texto.Trim();
        }
    }
}