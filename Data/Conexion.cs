using System;
using System.Data.OleDb;
using System.IO;

namespace PoderJudicial.Data
{
    public static class Conexion
    {
        public static readonly string RutaBD =
            @"\\ANTONIOS_LAPTOP\Prueba de Base General\Base 2024\BASE_2025.accdb";

        public static OleDbConnection ObtenerConexion()
        {
            string connectionString =
                $@"Provider=Microsoft.ACE.OLEDB.12.0;
                   Data Source={RutaBD};";

            return new OleDbConnection(connectionString);
        }
    }
} 


