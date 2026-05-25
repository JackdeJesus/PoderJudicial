using System.Collections.Generic;
using System.Data.OleDb;
using PoderJudicial.Models;
using PoderJudicial.Views;


namespace PoderJudicial.Data
{
    public class AudienciaData
    {
        // ──────────────────────────────────────────
        //  Mapeo centralizado — un solo lugar para
        //  leer columnas del reader al modelo
        // ──────────────────────────────────────────
        private static Audiencia MapearDesdeReader(OleDbDataReader reader)
        {
            return new Audiencia
            {
                Id = reader["Id"] != DBNull.Value
                    ? Convert.ToInt32(reader["Id"])
                    : 0,

                FechaAudiencia = reader["FeAudiencia"] != DBNull.Value
                    ? Convert.ToDateTime(reader["FeAudiencia"])
                    : null,

                FechaRecibo = reader["FeRecibo"] != DBNull.Value
                    ? Convert.ToDateTime(reader["FeRecibo"])
                    : null,

                TotDiscos = reader["TotDiscos"] != DBNull.Value
                    ? Convert.ToInt32(reader["TotDiscos"])
                    : null,

                TipoDisco = reader["TipoDisco"]?.ToString(),

                Juzgado = reader["Juzgado"]?.ToString(),

                TotDiscoAudiencia = reader["TotDiscoAudiencia"]?.ToString(),

                Juez = reader["Juez"]?.ToString(),

                NoCausa = reader["NoCausa"]?.ToString(),

                NUC = reader["NUC"]?.ToString(),

                TipoCausa = reader["TipoCausa"]?.ToString(),

                TipoAudiencia = reader["TipoAudiencia"]?.ToString(),

                HoraConclusion = reader["Hora conclusion"] != DBNull.Value
                    ? Convert.ToDateTime(reader["Hora conclusion"])
                    : null,

                Imputado = reader["Imputado"]?.ToString(),

                Delito = reader["Delito"]?.ToString(),

                Agraviado = reader["Agraviado"]?.ToString(),

                Sala = reader["Sala"]?.ToString(),

                NoCausaJuicio = reader["NoCausaJuicio"]?.ToString(),

                Diferida = reader["Diferida"]?.ToString(),

                QuienRealiza = reader["Quien Realiza"]?.ToString()
            };
        }

        // ──────────────────────────────────────────
        //  Parámetros centralizados — mismo orden
        //  que el INSERT y el UPDATE
        // ──────────────────────────────────────────
        private static void AgregarParametros(OleDbCommand cmd, Audiencia a)
        {
            // CRÍTICO en OleDb: el orden debe coincidir
            // exactamente con el orden del SQL
            cmd.Parameters.AddWithValue("@Id", a.Id);
            cmd.Parameters.AddWithValue("@FeAudiencia",a.FechaAudiencia.HasValue ? a.FechaAudiencia.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@FeRecibo",a.FechaRecibo.HasValue ? a.FechaRecibo.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@TotDiscos", a.TotDiscos.HasValue ? a.TotDiscos.Value: DBNull.Value);
            cmd.Parameters.AddWithValue("@TipoDisco", a.TipoDisco ?? string.Empty);
            cmd.Parameters.AddWithValue("@Juzgado", a.Juzgado ?? string.Empty);
            cmd.Parameters.AddWithValue("@TotDiscoAudiencia", a.TotDiscoAudiencia ?? string.Empty);
            cmd.Parameters.AddWithValue("@Juez", a.Juez ?? string.Empty);
            cmd.Parameters.AddWithValue("@NoCausa", a.NoCausa ?? string.Empty);
            cmd.Parameters.AddWithValue("@NUC", a.NUC ?? string.Empty);
            cmd.Parameters.AddWithValue("@TipoCausa", a.TipoCausa ?? string.Empty);
            cmd.Parameters.AddWithValue("@TipoAudiencia", a.TipoAudiencia ?? string.Empty);
            cmd.Parameters.AddWithValue("@HoraConclusion",a.HoraConclusion.HasValue? a.HoraConclusion.Value:DBNull.Value);
            cmd.Parameters.AddWithValue("@Imputado", a.Imputado ?? string.Empty);
            cmd.Parameters.AddWithValue("@Delito", a.Delito ?? string.Empty);
            cmd.Parameters.AddWithValue("@Agraviado", a.Agraviado ?? string.Empty);
            cmd.Parameters.AddWithValue("@Sala", a.Sala ?? string.Empty);
            cmd.Parameters.AddWithValue("@NoCausaJuicio", a.NoCausaJuicio ?? string.Empty);
            cmd.Parameters.AddWithValue("@Diferida", a.Diferida ?? string.Empty);
            cmd.Parameters.AddWithValue("@QuienRealiza", a.QuienRealiza ?? string.Empty);
        }

        // ──────────────────────────────────────────
        //  OBTENER TODOS
        // ──────────────────────────────────────────
        public List<Audiencia> ObtenerAudiencias()
        {
            List<Audiencia> lista = new List<Audiencia>();

            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();
                string query = $"SELECT * FROM [{TableDetector.TablaActual}]";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (string.IsNullOrWhiteSpace(reader["NoCausa"].ToString()) &&
                            string.IsNullOrWhiteSpace(reader["NUC"].ToString()))
                            continue;

                        lista.Add(MapearDesdeReader(reader));
                    }
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
                string query = $"SELECT * FROM [{TableDetector.TablaActual}] WHERE NoCausa = ?";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", noCausa);

                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return MapearDesdeReader(reader);
                    }
                }
            }

            return null;
        }

        // ──────────────────────────────────────────
        //  INSERT
        // ──────────────────────────────────────────
        public void Insertar(Audiencia a)
        {
            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();
                string tabla = TableDetector.TablaActual;

                string sql = $@"
                    INSERT INTO [{tabla}] (Id,
                        FeAudiencia, FeRecibo,
                        TotDiscos, TipoDisco,
                        Juzgado, TotDiscoAudiencia,
                        Juez, NoCausa, NUC,
                        TipoCausa, TipoAudiencia,
                        [Hora conclusion],
                        Imputado, Delito, Agraviado,
                        Sala, NoCausaJuicio,
                        Diferida, [Quien Realiza]
                    ) VALUES (
                       ?, ?, ?,
                        ?, ?,
                        ?, ?,
                        ?, ?, ?,
                        ?, ?,
                        ?,
                        ?, ?, ?,
                        ?, ?,
                        ?, ?
                    )";

                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    AgregarParametros(cmd, a);
                    cmd.ExecuteNonQuery();
                }
            }

            TableDetector.InvalidarCache();
        }

        public int ObtenerSiguienteIdVisual()
        {
            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                string tabla = TableDetector.TablaActual;

                string sql = $"SELECT MAX(Id) FROM [{tabla}]";

                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    object resultado = cmd.ExecuteScalar();

                    if (resultado == DBNull.Value || resultado == null)
                        return 1;

                    return Convert.ToInt32(resultado) + 1;
                }
            }
        }


    }

}