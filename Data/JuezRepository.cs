using System.Collections.Generic;
using System.Data.OleDb;

namespace PoderJudicial.Data
{
    public class JuezRepository : BaseRepository
    {
        public List<string> ObtenerJueces()
        {
            List<string> lista = new List<string>();

            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                string query =
                    "SELECT Juez FROM [Audiencias 2026-2028]";

                using (OleDbCommand cmd =
                    new OleDbCommand(query, conn))
                {
                    using (OleDbDataReader reader =
                        cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string valor =
                                reader["Juez"]?.ToString();

                            lista.Add(valor);
                        }
                    }
                }
            }

            return LimpiarLista(lista);
        }
    }
}