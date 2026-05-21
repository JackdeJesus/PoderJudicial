using System.Collections.Generic;
using System.Data.OleDb;

namespace PoderJudicial.Data
{
    public class DelitoRepository : BaseRepository
    {
        public List<string> ObtenerDelitos()
        {
            List<string> lista = new List<string>();

            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                string query =
                    "SELECT Delito FROM [Audiencias 2026-2028]";

                using (OleDbCommand cmd =
                    new OleDbCommand(query, conn))
                {
                    using (OleDbDataReader reader =
                        cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string valor =
                                reader["Delito"]?.ToString();

                            lista.Add(valor);
                        }
                    }
                }
            }

            return LimpiarLista(lista);
        }
    }
}