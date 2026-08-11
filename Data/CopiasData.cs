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
            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                string sql = "SELECT MAX(Id) FROM CopiasAudiencias";

                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    object resultado = cmd.ExecuteScalar();

                    if (resultado == null || resultado == DBNull.Value)
                    {
                        return 1;
                    }

                    return Convert.ToInt32(resultado) + 1;
                }
            }
        }

        /// <summary>
        /// Actualiza un registro existente.
        /// </summary>
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
                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.FeAudiencia.HasValue
                            ? (object)registro.FeAudiencia.Value
                            : DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.FeRecibo.HasValue
                            ? (object)registro.FeRecibo.Value
                            : DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.TotDiscosEntregados.HasValue
                            ? (object)registro.TotDiscosEntregados.Value
                            : DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.TipoDisco ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.NoCausa ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.NUC ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.TipoCausa ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.DiscosExternos ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.EtiquetasEntregadas ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.AQuienSeEntrega ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.Observaciones ?? string.Empty);

                    cmd.Parameters.AddWithValue("?", registro.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserta un nuevo registro.
        /// </summary>
        public void Insertar(RegistroCopia registro)
        {
            using (OleDbConnection conn = Conexion.ObtenerConexion())
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

                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("?", registro.Id);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.FeAudiencia ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.FeRecibo ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.TotDiscosEntregados ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.TipoDisco ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.NoCausa ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.NUC ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.TipoCausa ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.DiscosExternos?.ToString() ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.EtiquetasEntregadas?.ToString() ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.AQuienSeEntrega ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.Observaciones ?? string.Empty);

                    cmd.Parameters.AddWithValue(
                        "?",
                        registro.QuienRegistra ?? string.Empty);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Obtiene los valores distintos registrados en
        /// "A quien se entrega".
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
                        {
                            lista.Add(valor);
                        }
                    }
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene todos los registros necesarios para calcular
        /// el total de discos entregados.
        /// </summary>
        public List<RegistroCopia> ObtenerTodas()
        {
            var lista = new List<RegistroCopia>();

            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                string sql = @"
SELECT
    Id,
    TotDiscosEntregados
FROM CopiasAudiencias";

                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new RegistroCopia
                        {
                            Id = reader["Id"] != DBNull.Value
                                ? Convert.ToInt32(reader["Id"])
                                : 0,

                            TotDiscosEntregados =
                                int.TryParse(
                                    reader["TotDiscosEntregados"]?.ToString(),
                                    out int total)
                                    ? total
                                    : (int?)null
                        });
                    }
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene un registro completo de Registro de Copias por Id.
        /// </summary>
        public RegistroCopia ObtenerCopiaPorId(int id)
        {
            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                string sql =
                    "SELECT * FROM CopiasAudiencias WHERE Id = ?";

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
                                    DateTime.TryParse(
                                        reader["FeAudiencia"]?.ToString(),
                                        out DateTime fechaAudiencia)
                                        ? fechaAudiencia
                                        : (DateTime?)null,

                                FeRecibo =
                                    DateTime.TryParse(
                                        reader["FeRecibo"]?.ToString(),
                                        out DateTime fechaRecibo)
                                        ? fechaRecibo
                                        : (DateTime?)null,

                                TotDiscosEntregados =
                                    int.TryParse(
                                        reader["TotDiscosEntregados"]?.ToString(),
                                        out int totalDiscos)
                                        ? totalDiscos
                                        : (int?)null,

                                TipoDisco =
                                    reader["TipoDisco"]?.ToString()
                                    ?? string.Empty,

                                NoCausa =
                                    reader["NoCausa"]?.ToString()
                                    ?? string.Empty,

                                NUC =
                                    reader["NUC"]?.ToString()
                                    ?? string.Empty,

                                TipoCausa =
                                    reader["TipoCausa"]?.ToString()
                                    ?? string.Empty,

                                DiscosExternos =
                                    reader["DiscosExternos"]?.ToString()
                                    ?? string.Empty,

                                EtiquetasEntregadas =
                                    reader["Etiquetas entregadas"]?.ToString()
                                    ?? string.Empty,

                                AQuienSeEntrega =
                                    reader["A quien se entraga"]?.ToString()
                                    ?? string.Empty,

                                Observaciones =
                                    reader["Observaciones"]?.ToString()
                                    ?? string.Empty,

                                QuienRegistra =
                                    reader["Quien Realiza"]?.ToString()
                                    ?? string.Empty
                            };
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Obtiene todos los registros de CopiasAudiencias.
        /// Se utiliza para reportes y listados completos.
        /// </summary>
        public List<RegistroCopia> ObtenerCopias()
        {
            var lista = new List<RegistroCopia>();

            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                const string sql = @"
SELECT
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
FROM CopiasAudiencias
ORDER BY FeRecibo, Id";

                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var registro = new RegistroCopia
                        {
                            Id = Convert.ToInt32(reader["Id"]),

                            FeAudiencia = DateTime.TryParse(
                                reader["FeAudiencia"]?.ToString(),
                                out DateTime fechaAudiencia)
                                ? fechaAudiencia
                                : (DateTime?)null,

                            FeRecibo = DateTime.TryParse(
                                reader["FeRecibo"]?.ToString(),
                                out DateTime fechaRecibo)
                                ? fechaRecibo
                                : (DateTime?)null,

                            TotDiscosEntregados = int.TryParse(
                                reader["TotDiscosEntregados"]?.ToString(),
                                out int totalDiscos)
                                ? totalDiscos
                                : (int?)null,

                            TipoDisco =
                                reader["TipoDisco"]?.ToString()
                                ?? string.Empty,

                            NoCausa =
                                reader["NoCausa"]?.ToString()
                                ?? string.Empty,

                            NUC =
                                reader["NUC"]?.ToString()
                                ?? string.Empty,

                            TipoCausa =
                                reader["TipoCausa"]?.ToString()
                                ?? string.Empty,

                            DiscosExternos =
                                reader["DiscosExternos"]?.ToString()
                                ?? string.Empty,

                            EtiquetasEntregadas =
                                reader["Etiquetas entregadas"]?.ToString()
                                ?? string.Empty,

                            AQuienSeEntrega =
                                reader["A quien se entraga"]?.ToString()
                                ?? string.Empty,

                            Observaciones =
                                reader["Observaciones"]?.ToString()
                                ?? string.Empty,

                            QuienRegistra =
                                reader["Quien Realiza"]?.ToString()
                                ?? string.Empty
                        };

                        lista.Add(registro);
                    }
                }
            }

            return lista;
        }
















    }
}