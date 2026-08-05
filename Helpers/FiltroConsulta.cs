using System;

namespace PoderJudicial.Helpers
{
    /// <summary>
    /// Criterios de búsqueda avanzada para "Consultar Registros". Todas las
    /// propiedades son opcionales (null/vacío = "no filtrar por esto").
    /// Es una sola clase para las 3 tablas porque el modelo de datos que
    /// consume (Audiencia) ya llega unificado desde
    /// AudienciaData.MapearDesdeReader — un campo que no exista en la tabla
    /// actual simplemente viene vacío y ese filtro no aporta, sin necesidad
    /// de que este archivo sepa en qué tabla está parado.
    /// </summary>
    public class FiltroConsulta
    {
        public string NUC { get; set; }
        public string NoCausa { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string TipoCausa { get; set; }
        public string Juzgado { get; set; }
        public string Sala { get; set; }
        public string Imputado { get; set; }
        public string Delito { get; set; }
        public string Juez { get; set; }
        public string Expediente { get; set; }
        public string AQuienEntrega { get; set; }

        /// <summary>True si el usuario dejó algún criterio con valor.</summary>
        public bool TieneAlgunCriterio =>
            !string.IsNullOrWhiteSpace(NUC) ||
            !string.IsNullOrWhiteSpace(NoCausa) ||
            FechaDesde.HasValue ||
            FechaHasta.HasValue ||
            !string.IsNullOrWhiteSpace(TipoCausa) ||
            !string.IsNullOrWhiteSpace(Juzgado) ||
            !string.IsNullOrWhiteSpace(Sala) ||
            !string.IsNullOrWhiteSpace(Imputado) ||
            !string.IsNullOrWhiteSpace(Delito) ||
            !string.IsNullOrWhiteSpace(Juez) ||
            !string.IsNullOrWhiteSpace(Expediente) ||
            !string.IsNullOrWhiteSpace(AQuienEntrega);

        public void Limpiar()
        {
            NUC = NoCausa = TipoCausa = Juzgado = Sala =
                Imputado = Delito = Juez = Expediente = AQuienEntrega = null;
            FechaDesde = FechaHasta = null;
        }
    }
}