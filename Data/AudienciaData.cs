using System.Collections.Generic;
using System.Data.OleDb;
using PoderJudicial.Models;
using PoderJudicial.Views;


namespace PoderJudicial.Data
{
    public class AudienciaData
    {
        private Conexion _conexion = new Conexion();

        public List<Audiencia> ObtenerAudiencias()
        {
            List<Audiencia> lista = new List<Audiencia>();

            using (OleDbConnection conn = _conexion.GetConnection())
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
    }
}
