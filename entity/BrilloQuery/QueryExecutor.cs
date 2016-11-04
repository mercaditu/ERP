using MySql.Data.MySqlClient;
using System.Data;

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
                throw;
                //MessageBox.Show("Unable to Connect to Database. Please Check your credentials.");
            }
            return dt;
        }
    }
}
