using PoderJudicial.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

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

            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                foreach (string nombreTabla in ObtenerTablasAudiencias(conn))
                {
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
                            cmd.Parameters.AddWithValue("?", inicioMes);
                            cmd.Parameters.AddWithValue("?", inicioSiguienteMes);

                            object resultado = cmd.ExecuteScalar();

                            if (resultado != null &&
                                resultado != DBNull.Value)
                            {
                                total += Convert.ToInt32(resultado);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.Message,
                            nombreTabla);
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

               

                foreach (string nombreTabla in ObtenerTablasAudiencias(conn))
                {
                    
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
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.Message,
                            nombreTabla);
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


        public List<ActividadReciente> ObtenerActividadesRecientes()
        {
            List<ActividadReciente> actividades = new List<ActividadReciente>();

            actividades.AddRange(ObtenerActividadesAudiencias());

            actividades.AddRange(ObtenerActividadesCopias());

            actividades.AddRange(ObtenerActividadesEjecuciones());

            return actividades
                .OrderByDescending(x => x.FechaHora)
                .Take(5)
                .ToList();
        }

        private List<ActividadReciente> ObtenerActividadesAudiencias()
        {
            List<ActividadReciente> lista = new List<ActividadReciente>();

            using (OleDbConnection conn = Conexion.ObtenerConexion())
            {
                conn.Open();

                foreach (string nombreTabla in ObtenerTablasAudiencias(conn))
                {
                    try
                    {
                        string query = $@"
                    SELECT TOP 10
                        FeRecibo,
                        NUC,
                        NoCausa,
                        [Quien Realiza]
                    FROM [{nombreTabla}]
                    WHERE FeRecibo IS NOT NULL
                    ORDER BY FeRecibo DESC";

                        using (OleDbCommand cmd = new OleDbCommand(query, conn))
                        using (OleDbDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                lista.Add(new ActividadReciente
                                {
                                    FechaHora = Convert.ToDateTime(dr["FeRecibo"]),
                                    Icono = "⚖",
                                    TipoActividad = "Registro de audiencia",
                                    Descripcion =
                                        $"NUC: {dr["NUC"]} | Causa: {dr["NoCausa"]}",
                                    Usuario = dr["Quien Realiza"].ToString()
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.Message,
                            nombreTabla);
                    }
                }
            }

            return lista;
        }

        private List<ActividadReciente> ObtenerActividadesCopias()
        {
            return new List<ActividadReciente>();
        }

        private List<ActividadReciente> ObtenerActividadesEjecuciones()
        {
            return new List<ActividadReciente>();
        }


        private List<string> ObtenerTablasAudiencias(OleDbConnection conn)
        {
            List<string> tablas = new List<string>();

            DataTable schema = conn.GetSchema("Tables");

            foreach (DataRow row in schema.Rows)
            {
                string nombreTabla = row["TABLE_NAME"].ToString();

                if (nombreTabla.StartsWith("MSys"))
                    continue;

                if (nombreTabla.StartsWith(
                    "Audiencias ",
                    StringComparison.OrdinalIgnoreCase))
                {
                    tablas.Add(nombreTabla);
                }
            }

            return tablas;
        }


    }
}