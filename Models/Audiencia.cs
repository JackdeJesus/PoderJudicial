using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoderJudicial.Models
{
   public  class Audiencia
    {
        public int Id { get; set; }
        public string NoCausa { get; set; }
        public string NUC { get; set; }
        public string FechaAudiencia { get; set; }
        public string FechaRecibo { get; set; }
        public string TipoAudiencia { get; set; }
        public string TipoCausa { get; set; }
        public string Juzgado { get; set; }
        public string Juez { get; set; }
        public string Imputado { get; set; }
        public string Delito { get; set; }
        public string Agraviado { get; set; }
        public string Sala { get; set; }
        public string HoraConclusion { get; set; }
        public string QuienRealiza { get; set; }
        public string Diferida { get; set; }
        public string NoCausaJuicio { get; set; }
        public int TotDiscos { get; set; }
        public string TipoDisco { get; set; }
        public int TotDiscoAudiencia { get; set; }
    }
}
