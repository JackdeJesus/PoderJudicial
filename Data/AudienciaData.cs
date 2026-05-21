using System.Collections.Generic;
using System.Data.OleDb;
using PoderJudicial.Models;
using PoderJudicial.Views;


namespace PoderJudicial.Data
{
    public class AudienciaData
    {
        // Cambiar la declaración para usar el método estático directamente
        public List<Audiencia> ObtenerAudiencias()
        {
            List<Audiencia> lista = new List<Audiencia>();

            using (OleDbConnection conn = Conexion.ObtenerConexion()) // Usar el método estático
            {
                conn.Open();
                string query = "SELECT * FROM [Audiencias 2026-2028]";
                OleDbCommand cmd = new OleDbCommand(query, conn);
                OleDbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    // Ignorar filas vacías
                    if (string.IsNullOrWhiteSpace(reader["NoCausa"].ToString()) &&
                        string.IsNullOrWhiteSpace(reader["NUC"].ToString()))
                    {
                        continue;
                    }

                    lista.Add(new Audiencia
                    {
                        NoCausa = reader["NoCausa"].ToString(),
                        NUC = reader["NUC"].ToString(),
                        FechaAudiencia = reader["FeAudiencia"].ToString(),
                        TipoAudiencia = reader["TipoAudiencia"].ToString(),
                        Juzgado = reader["Juzgado"].ToString(),
                        Imputado = reader["Imputado"].ToString(),
                        Sala = reader["Sala"].ToString()
                    });
                }
            }

            return lista;
        }

        // ──────────────────────────────────────────
        //  OBTENER UNO POR NoCausa
        // ──────────────────────────────────────────
        public Audiencia ObtenerAudienciaPorNoCausa(string noCausa)
        {
            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();
                string query = "SELECT * FROM [Audiencias 2026-2028] WHERE NoCausa = ?";
                OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("?", noCausa);

                OleDbDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Audiencia
                    {
                        NoCausa = reader["NoCausa"].ToString(),
                        NUC = reader["NUC"].ToString(),
                        FechaAudiencia = reader["FeAudiencia"].ToString(),
                        FechaRecibo = reader["FeRecibo"].ToString(),
                        TipoAudiencia = reader["TipoAudiencia"].ToString(),
                        TipoCausa = reader["TipoCausa"].ToString(),
                        Juzgado = reader["Juzgado"].ToString(),
                        Juez = reader["Juez"].ToString(),
                        Imputado = reader["Imputado"].ToString(),
                        Delito = reader["Delito"].ToString(),
                        Agraviado = reader["Agraviado"].ToString(),
                        Sala = reader["Sala"].ToString(),
                        HoraConclusion = reader["Hora conclusion"].ToString(),
                        QuienRealiza = reader["Quien Realiza"].ToString(),

                        NoCausaJuicio = reader["NoCausaJuicio"].ToString(),
                        TotDiscos = reader["TotDiscos"].ToString(),
                        TotDiscoAudiencia = reader["TotDiscoAudiencia"].ToString(),
                        TipoDisco = reader["TipoDisco"].ToString(),
                    };
                }

                return null;
            }
        }

    }
}