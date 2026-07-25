using PoderJudicial.Models;
using System.Data.OleDb;

namespace PoderJudicial.Data
{
    public class EjecucionData
    {
        public void Insertar(Ejecucion ejecucion)
        {
            using (OleDbConnection conn =
                Conexion.ObtenerConexion())
            {
                conn.Open();

                string query = @"
INSERT INTO Ejecucion
(
    Id,
    FechaAudiencia,
    TotalDiscos,
    Juez,
    Expediente,
    Causa,
    TipoAudiencia,
    HoraTermino,
    Imputado,
    Delito,
    Victima,
    Sala,
    Observaciones
)
VALUES
(
    ?, ?, ?, ?, ?, ?, ?, ?,?, ?, ?, ?, ?
)";

                using (OleDbCommand cmd =
                    new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "?", ejecucion.Id);

                    cmd.Parameters.AddWithValue(
                        "?", ejecucion.FechaAudiencia);

                    cmd.Parameters.AddWithValue(
                        "?", ejecucion.TotalDiscos);

                    cmd.Parameters.AddWithValue(
                        "?", ejecucion.Juez);

                    cmd.Parameters.AddWithValue(
                        "?", ejecucion.ExpedienteNumero);

                    cmd.Parameters.AddWithValue(
                        "?", ejecucion.Causa);

                    cmd.Parameters.AddWithValue(
                        "?", ejecucion.TipoAudiencia);

                    cmd.Parameters.AddWithValue(
                        "?", ejecucion.HoraTermino);

                    cmd.Parameters.AddWithValue(
                        "?", ejecucion.Imputado);

                    cmd.Parameters.AddWithValue(
                        "?", ejecucion.Delito);

                    cmd.Parameters.AddWithValue(
                        "?", ejecucion.Victima);

                    cmd.Parameters.AddWithValue(
                        "?", ejecucion.Sala);



                    cmd.Parameters.AddWithValue(
                        "?", ejecucion.Observaciones);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        public int ObtenerSiguienteId()
        {
            using (OleDbConnection conn =
                Conexion.ObtenerConexion())
            {
                conn.Open();

                string query =
                    "SELECT MAX(Id) FROM Ejecucion";

                using (OleDbCommand cmd =
                    new OleDbCommand(query, conn))
                {
                    object resultado =
                        cmd.ExecuteScalar();

                    if (resultado == DBNull.Value ||
                        resultado == null)
                    {
                        return 1;
                    }

                    return Convert.ToInt32(resultado) + 1;
                }
            }
        }



        public Ejecucion ObtenerEjecucionPorId(int id)
        {
            using (OleDbConnection conn =
                Conexion.ObtenerConexion())
            {
                conn.Open();

                string query =
                    "SELECT * FROM Ejecucion WHERE Id = ?";

                using (OleDbCommand cmd =
                    new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", id);

                    using (OleDbDataReader reader =
                        cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Ejecucion
                            {
                                Id = Convert.ToInt32(reader["Id"]),

                                FechaAudiencia =
                                    DateTime.TryParse(reader["FechaAudiencia"]?.ToString(), out DateTime fecha)
                                        ? fecha : (DateTime?)null,

                                TotalDiscos = reader["TotalDiscos"]?.ToString(),
                                Juez = reader["Juez"]?.ToString(),
                                ExpedienteNumero = reader["Expediente"]?.ToString(),
                                Causa = reader["Causa"]?.ToString(),
                                TipoAudiencia = reader["TipoAudiencia"]?.ToString(),
                                HoraTermino = reader["HoraTermino"]?.ToString(),
                                Imputado = reader["Imputado"]?.ToString(),
                                Delito = reader["Delito"]?.ToString(),
                                Victima = reader["Victima"]?.ToString(),
                                Sala = reader["Sala"]?.ToString(),
                                Observaciones = reader["Observaciones"]?.ToString()
                            };
                        }
                    }
                }
            }

            return null;
        }

        public List<Ejecucion> ObtenerEjecuciones()
        {
            List<Ejecucion> lista = new();

            using (OleDbConnection conn =
                Conexion.ObtenerConexion())
            {
                conn.Open();

                string query =
                    "SELECT * FROM Ejecucion";

                using (OleDbCommand cmd =
                    new OleDbCommand(query, conn))
                {
                    using (OleDbDataReader reader =
                        cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Ejecucion
                            {
                                Delito =
                                    reader["Delito"]?.ToString(),

                                TipoAudiencia =
                                    reader["TipoAudiencia"]?.ToString()
                            });
                        }
                    }
                }
            }

            return lista;
        }


    }
}