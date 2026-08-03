using PoderJudicial.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace PoderJudicial.Data
{
    public class CopiasData
    {
        public int ObtenerSiguienteIdVisual()
        {
            using (OleDbConnection conn =
                Conexion.ObtenerConexion())
            {
                conn.Open();

                string sql =
                    "SELECT MAX(Id) FROM CopiasAudiencias";

                using (OleDbCommand cmd =
                    new OleDbCommand(sql, conn))
                {
                    object resultado =
                        cmd.ExecuteScalar();

                    if (resultado == null ||
                        resultado == DBNull.Value)
                    {
                        return 1;
                    }

                    return Convert.ToInt32(resultado) + 1;
                }
            }
        }

        /// <summary>Actualiza un registro existente (modo edición).</summary>
        public void Actualizar(RegistroCopia registro)
        {
            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                string sql = @"
UPDATE CopiasAudiencias SET
    FeAudiencia            = ?,
    FeRecibo               = ?,
    TotDiscosEntregados    = ?,
    TipoDisco              = ?,
    NoCausa                = ?,
    NUC                    = ?,
    TipoCausa              = ?,
    DiscosExternos         = ?,
    [Etiquetas entregadas] = ?,
    [A quien se entraga]   = ?,
    Observaciones          = ?
WHERE Id = ?";

                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("?", registro.FeAudiencia.HasValue ? (object)registro.FeAudiencia.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("?", registro.FeRecibo.HasValue ? (object)registro.FeRecibo.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("?", registro.TotDiscosEntregados.HasValue ? (object)registro.TotDiscosEntregados.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("?", registro.TipoDisco ?? string.Empty);
                    cmd.Parameters.AddWithValue("?", registro.NoCausa ?? string.Empty);
                    cmd.Parameters.AddWithValue("?", registro.NUC ?? string.Empty);
                    cmd.Parameters.AddWithValue("?", registro.TipoCausa ?? string.Empty);
                    cmd.Parameters.AddWithValue("?", registro.DiscosExternos ?? string.Empty);
                    cmd.Parameters.AddWithValue("?", registro.EtiquetasEntregadas ?? string.Empty);
                    cmd.Parameters.AddWithValue("?", registro.AQuienSeEntrega ?? string.Empty);
                    cmd.Parameters.AddWithValue("?", registro.Observaciones ?? string.Empty);
                    cmd.Parameters.AddWithValue("?", registro.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Insertar(RegistroCopia registro)
        {
            using (OleDbConnection conn =
                Conexion.ObtenerConexion())
            {
                conn.Open();

                string sql = @"
INSERT INTO CopiasAudiencias
(
    Id,
    FeAudiencia,
    FeRecibo,
    TotDiscosEntregados,
    TipoDisco,
    NoCausa,
    NUC,
    TipoCausa,
    DiscosExternos,
    [Etiquetas entregadas],
    [A quien se entraga],
    Observaciones,
    [Quien Realiza]
)
VALUES
(
    ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?
)";

                using (OleDbCommand cmd =
                    new OleDbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("?", registro.Id);

                    cmd.Parameters.AddWithValue("?",
                        registro.FeAudiencia ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("?",
                        registro.FeRecibo ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("?",
                        registro.TotDiscosEntregados ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("?", registro.TipoDisco);
                    cmd.Parameters.AddWithValue("?", registro.NoCausa);
                    cmd.Parameters.AddWithValue("?", registro.NUC);
                    cmd.Parameters.AddWithValue("?", registro.TipoCausa);

                    cmd.Parameters.AddWithValue("?",
                        registro.DiscosExternos?.ToString() ?? "");

                    cmd.Parameters.AddWithValue("?",
                        registro.EtiquetasEntregadas?.ToString() ?? "");

                    cmd.Parameters.AddWithValue("?",
                        registro.AQuienSeEntrega);

                    cmd.Parameters.AddWithValue("?",
                        registro.Observaciones);

                    cmd.Parameters.AddWithValue("?",
                        registro.QuienRegistra);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Valores distintos ya capturados en la columna "A quien se entrega"
        /// (CopiasAudiencias), usados como fuente del autocompletado del
        /// campo del mismo nombre — misma infraestructura que Jueces/Delito.
        /// </summary>
        public List<string> ObtenerValoresAQuienSeEntrega()
        {
            var lista = new List<string>();

            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                string sql = @"
SELECT DISTINCT [A quien se entraga]
FROM CopiasAudiencias
WHERE [A quien se entraga] IS NOT NULL";

                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string valor = reader[0]?.ToString();
                        if (!string.IsNullOrWhiteSpace(valor))
                            lista.Add(valor);
                    }
                }
            }

            return lista;
        }
        /// <summary>
        /// Obtiene un registro completo de "Registro de Copias" por Id,
        /// usado por "Ver Detalle" en Consulta de Registros.
        /// </summary>
        public RegistroCopia ObtenerCopiaPorId(int id)
        {
            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                string sql = "SELECT * FROM CopiasAudiencias WHERE Id = ?";

                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("?", id);

                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new RegistroCopia
                            {
                                Id = Convert.ToInt32(reader["Id"]),

                                FeAudiencia =
                                    DateTime.TryParse(reader["FeAudiencia"]?.ToString(), out DateTime feA)
                                        ? feA : (DateTime?)null,

                                FeRecibo =
                                    DateTime.TryParse(reader["FeRecibo"]?.ToString(), out DateTime feR)
                                        ? feR : (DateTime?)null,

                                TotDiscosEntregados =
                                    int.TryParse(reader["TotDiscosEntregados"]?.ToString(), out int tot)
                                        ? tot : (int?)null,

                                TipoDisco = reader["TipoDisco"]?.ToString() ?? "",
                                NoCausa = reader["NoCausa"]?.ToString() ?? "",
                                NUC = reader["NUC"]?.ToString() ?? "",
                                TipoCausa = reader["TipoCausa"]?.ToString() ?? "",
                                DiscosExternos = reader["DiscosExternos"]?.ToString() ?? "",
                                EtiquetasEntregadas = reader["Etiquetas entregadas"]?.ToString() ?? "",
                                AQuienSeEntrega = reader["A quien se entraga"]?.ToString() ?? "",
                                Observaciones = reader["Observaciones"]?.ToString() ?? "",
                                QuienRegistra = reader["Quien Realiza"]?.ToString() ?? ""
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}