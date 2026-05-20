using System.Collections.Generic;

namespace PoderJudicial.Data
{
    public class AudienciaRepository
    {
        public List<string> ObtenerTiposAudiencia()
        {
            // temporal
            return new List<string>()
            {
                "Audiencia intermedia",
                "Audiencia Inicial de Formulación de Imputación",
                "Ratificacion de Medida de Proteccion-Concentradas",
                "Audiencia de Solicitud de Orden de Cateo",
                "Control Judicial"
            };
        }
    }
}