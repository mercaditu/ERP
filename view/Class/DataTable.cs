using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Cognitivo.Class
{
    public static class Generate
    {
        public static DataTable DataTable(string SQL)
        {
            DataTable dt = new DataTable();
            try
            {
                MySqlConnection sqlConn = new MySqlConnection(Properties.Settings.Default.MySQLconnString);
                sqlConn.Open();
                MySqlCommand cmd = new MySqlCommand(SQL, sqlConn);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
               
                da.Fill(dt);
                sqlConn.Close();
            }
            catch (Exception ex) { throw ex; }
            return dt;
        }
    }
}