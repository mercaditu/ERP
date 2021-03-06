﻿using MySql.Data.MySqlClient;
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
            }
            return dt;
        }

        public static object Scalar(string sql)
        {
            object scalar;
            try
            {
                MySqlConnection sqlConn = new MySqlConnection(CurrentSession.ConnectionString);
                sqlConn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, sqlConn);
                scalar = cmd.ExecuteScalar();
                sqlConn.Close();
            }
            catch
            {
                throw;
            }

            return scalar;
        }
    }
}