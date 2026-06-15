namespace PoderJudicial.Models
{
    public class RegistroCopia
    {
        public int Id { get; set; }
        public DateTime? FeAudiencia { get; set; }
        public DateTime? FeRecibo { get; set; }
        public int? TotDiscosEntregados { get; set; }
        public string TipoDisco { get; set; } = string.Empty;
        public string NoCausa { get; set; } = string.Empty;
        public string NUC { get; set; } = string.Empty;
        public string TipoCausa { get; set; } = string.Empty;
        public int? DiscosExternos { get; set; }
        public int? EtiquetasEntregadas { get; set; }
        public string AQuienSeEntrega { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public string QuienRegistra { get; set; } = string.Empty;
    }
}