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
    }
}