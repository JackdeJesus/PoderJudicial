using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoderJudicial.Data
{
    class Conexion
    {
        

        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\jackd\OneDrive\Documents\p.accdb;";

        public OleDbConnection GetConnection()
        {
            return new OleDbConnection(connectionString);
        }

        public bool ProbarConexion()
        {
            using (OleDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    
}
}
