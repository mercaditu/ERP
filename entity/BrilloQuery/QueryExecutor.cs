using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.BrilloQuery
{
    public static class QueryExecutor
    {
        public static DataTable DT(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                MySqlConnection sqlConn = new MySqlConnection(CurrentSession.ConnectionString);
                sqlConn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, sqlConn);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
                sqlConn.Close();
            }
            catch
            {
                //MessageBox.Show("Unable to Connect to Database. Please Check your credentials.");
            }
            return dt;
        }
    }
}
