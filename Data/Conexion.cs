using System;
using System.Data.OleDb;
using System.IO;

namespace PoderJudicial.Data
{

    //cambios en esta parte
    public static class Conexion
    {
        public static OleDbConnection ObtenerConexion()
        {
            string rutaBD =
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Database",
                    "p.accdb");

            string connectionString =
                $@"Provider=Microsoft.ACE.OLEDB.12.0;
                   Data Source={rutaBD};";

            return new OleDbConnection(connectionString);
        }
    }
}