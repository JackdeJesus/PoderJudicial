using System;
using System.Data;
using System.Data.OleDb;
using System.Reflection;
using System.IO;


namespace PoderJudicial.Data
{
    public class DashboardData
    {
        public int ObtenerTotalAudienciasMes()
        {
            int total = 0;

            DateTime inicioMes =
                new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    1);

            DateTime inicioSiguienteMes =
                inicioMes.AddMonths(1);

            using (OleDbConnection conn =
                Conexion.ObtenerConexion())
            {
                conn.Open();

                DataTable schema =
                    conn.GetSchema("Tables");

                foreach (DataRow row in schema.Rows)
                {
                    string nombreTabla =
                        row["TABLE_NAME"].ToString();

                    if (nombreTabla.StartsWith("MSys"))
                        continue;

                    if (!nombreTabla.StartsWith(
                            "Audiencias ",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    try
                    {
                        string query = $@"
                            SELECT COUNT(*)
                            FROM [{nombreTabla}]
                            WHERE FeAudiencia >= ?
                            AND FeAudiencia < ?";

                        using (OleDbCommand cmd =
                            new OleDbCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue(
                                "?",
                                inicioMes);

                            cmd.Parameters.AddWithValue(
                                "?",
                                inicioSiguienteMes);

                            object resultado =
                                cmd.ExecuteScalar();

                            if (resultado != null &&
                                resultado != DBNull.Value)
                            {
                                total +=
                                    Convert.ToInt32(resultado);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return total;
        }



        public int ObtenerTotalEjecucionesMes()
        {
            int total = 0;

            DateTime inicioMes =
                new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    1);

            DateTime inicioSiguienteMes =
                inicioMes.AddMonths(1);

            using (OleDbConnection conn =
                Conexion.ObtenerConexion())
            {
                conn.Open();

                string query = @"
            SELECT COUNT(*)
            FROM Ejecucion
            WHERE FechaAudiencia >= ?
            AND FechaAudiencia < ?";

                using (OleDbCommand cmd =
                    new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "?",
                        inicioMes);

                    cmd.Parameters.AddWithValue(
                        "?",
                        inicioSiguienteMes);

                     object resultado =
                        cmd.ExecuteScalar();

                    if (resultado != null &&
                        resultado != DBNull.Value)
                    {
                        total = Convert.ToInt32(resultado);
                    }
                }
            }

            return total;
        }



        public int ObtenerTotalCopiasMes()
        {
            int total = 0;

            DateTime inicioMes =
                new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    1);

            DateTime inicioSiguienteMes =
                inicioMes.AddMonths(1);

            using (OleDbConnection conn =
                Conexion.ObtenerConexion())
            {
                conn.Open();

                string query = @"
            SELECT SUM(Val(TotDiscosEntregados))
            FROM CopiasAudiencias
            WHERE FeRecibo >= ?
            AND FeRecibo < ?";

                using (OleDbCommand cmd =
                    new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "?",
                        inicioMes);

                    cmd.Parameters.AddWithValue(
                        "?",
                        inicioSiguienteMes);

                    object resultado =
                        cmd.ExecuteScalar();

                    if (resultado != null &&
                        resultado != DBNull.Value)
                    {
                        total = Convert.ToInt32(resultado);
                    }
                }
            }

            return total;
        }


        public int ObtenerAudienciasHoy()
        {
            int total = 0;

            DateTime inicioDia = DateTime.Today;
            DateTime inicioSiguienteDia = inicioDia.AddDays(1);

            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                DataTable schema = conn.GetSchema("Tables");

                foreach (DataRow row in schema.Rows)
                {
                    string nombreTabla = row["TABLE_NAME"].ToString();

                    if (nombreTabla.StartsWith("MSys"))
                        continue;

                    if (!nombreTabla.StartsWith(
                            "Audiencias ",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    try
                    {
                        string query = @"
                    SELECT COUNT(*)
                    FROM [" + nombreTabla + @"]
                    WHERE FeAudiencia >= ?
                    AND FeAudiencia < ?";

                        using (OleDbCommand cmd = new OleDbCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("?", inicioDia);
                            cmd.Parameters.AddWithValue("?", inicioSiguienteDia);

                            object resultado = cmd.ExecuteScalar();

                            if (resultado != null &&
                                resultado != DBNull.Value)
                            {
                                total += Convert.ToInt32(resultado);
                            }
                        }
                    }
                    catch
                    {
                        // Ignorar tablas que no correspondan
                    }
                }
            }

            return total;
        }


        public string ObtenerVersionSistema()
        {
            Version version =
                Assembly.GetExecutingAssembly().GetName().Version;

            return $"v{version.Major}.{version.Minor}.{version.Build}";
        }

        public string ObtenerEstadoSistema()
        {
            try
            {
                using (var cn = Conexion.ObtenerConexion())
                {
                    cn.Open();
                }

                return "Operativo";
            }
            catch
            {
                return "Sin conexión";
            }
        }


        public string ObtenerNombreBaseDatos()
        {
            return Path.GetFileName(Conexion.RutaBD);
        }


    }
}