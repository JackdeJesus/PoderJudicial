using System.Collections.Generic;
using System.Data.OleDb;

namespace PoderJudicial.Data
{
    public class AudienciaRepository : BaseRepository
    {
        public List<string> ObtenerTiposAudiencia()
        {
            List<string> lista = new List<string>();

            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                string query =
                    "SELECT TipoAudiencia FROM [Audiencias 2026-2028]";

                using (OleDbCommand cmd =
                    new OleDbCommand(query, conn))
                {
                    using (OleDbDataReader reader =
                        cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string valor =
                                reader["TipoAudiencia"]?.ToString();

                            lista.Add(valor);
                        }
                    }
                }
            }

            return LimpiarLista(lista);
        }
    }
}