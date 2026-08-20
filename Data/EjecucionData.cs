using PoderJudicial.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace PoderJudicial.Data
{
    public class EjecucionData
    {
        public void Insertar(Ejecucion ejecucion)
        {
            using (OleDbConnection conn = Conexion.ObtenerConexion())
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
    ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?
)";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", ejecucion.Id);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.FechaAudiencia.HasValue
                            ? (object)ejecucion.FechaAudiencia.Value
                            : DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.TotalDiscos ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.Juez ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.ExpedienteNumero ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.Causa ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.TipoAudiencia ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.HoraTermino ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.Imputado ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.Delito ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.Victima ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.Sala ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.Observaciones ?? string.Empty);
cmd.ExecuteNonQuery();
                }
            }
        }

        public void Actualizar(Ejecucion ejecucion)
        {
            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                string query = @"
UPDATE Ejecucion SET
    FechaAudiencia = ?,
    TotalDiscos    = ?,
    Juez           = ?,
    Expediente     = ?,
    Causa          = ?,
    TipoAudiencia  = ?,
    HoraTermino    = ?,
    Imputado       = ?,
    Delito         = ?,
    Victima        = ?,
    Sala           = ?,
    Observaciones  = ?
WHERE Id = ?";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.FechaAudiencia.HasValue
                            ? (object)ejecucion.FechaAudiencia.Value
                            : DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.TotalDiscos ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.Juez ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.ExpedienteNumero ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.Causa ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.TipoAudiencia ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.HoraTermino ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.Imputado ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.Delito ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.Victima ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.Sala ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.Observaciones ?? string.Empty);
cmd.Parameters.AddWithValue(
                        "?",
                        ejecucion.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int ObtenerSiguienteId()
        {
            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                string query = "SELECT MAX(Id) FROM Ejecucion";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    object resultado = cmd.ExecuteScalar();

                    if (resultado == DBNull.Value || resultado == null)
                    {
                        return 1;
                    }

                    return Convert.ToInt32(resultado) + 1;
                }
            }
        }

        public Ejecucion ObtenerEjecucionPorId(int id)
        {
            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                string query =
                    "SELECT * FROM Ejecucion WHERE Id = ?";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", id);

                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Ejecucion
                            {
                                Id = Convert.ToInt32(reader["Id"]),

                                FechaAudiencia =
                                    DateTime.TryParse(
                                        reader["FechaAudiencia"]?.ToString(),
                                        out DateTime fecha)
                                            ? fecha
                                            : (DateTime?)null,

                                TotalDiscos =
                                    reader["TotalDiscos"]?.ToString(),

                                Juez =
                                    reader["Juez"]?.ToString(),

                                ExpedienteNumero =
                                    reader["Expediente"]?.ToString(),

                                Causa =
                                    reader["Causa"]?.ToString(),

                                TipoAudiencia =
                                    reader["TipoAudiencia"]?.ToString(),

                                HoraTermino =
                                    reader["HoraTermino"]?.ToString(),

                                Imputado =
                                    reader["Imputado"]?.ToString(),

                                Delito =
                                    reader["Delito"]?.ToString(),

                                Victima =
                                    reader["Victima"]?.ToString(),

                                Sala =
                                    reader["Sala"]?.ToString(),

                                Observaciones =
                                    reader["Observaciones"]?.ToString()
                            };
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Listado completo (con Id y TotalDiscos incluidos) usado por los
        /// indicadores "Total de registros" / "Total Discos Audiencia" en
        /// Consultar Registros. No confundir con ObtenerEjecuciones(),
        /// que solo trae Delito/TipoAudiencia para el autocompletado de
        /// Nuevo Registro y no debe tocarse.
        /// </summary>
        public List<Ejecucion> ObtenerTodas()
        {
            List<Ejecucion> lista = new();

            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                string query =
                    "SELECT Id, TotalDiscos FROM Ejecucion";

                using (OleDbCommand cmd =
                       new OleDbCommand(query, conn))
                using (OleDbDataReader reader =
                       cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Ejecucion
                        {
                            Id =
                                reader["Id"] != DBNull.Value
                                    ? Convert.ToInt32(reader["Id"])
                                    : 0,

                            TotalDiscos =
                                reader["TotalDiscos"]?.ToString()
                        });
                    }
                }
            }

            return lista;
        }

        public List<Ejecucion> ObtenerEjecuciones()
        {
            List<Ejecucion> lista = new();

            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                string query =
                    "SELECT * FROM Ejecucion";

                using (OleDbCommand cmd =
                       new OleDbCommand(query, conn))
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

            return lista;
        }
    }
}
