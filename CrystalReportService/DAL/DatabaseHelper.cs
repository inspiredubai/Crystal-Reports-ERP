using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace CrystalReportService.DAL
{
    public class DatabaseHelper : IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection _connection;

        public DatabaseHelper()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            _connection = new SqlConnection(_connectionString);
        }

        /// <summary>
        /// Executes a query and returns the result as a DataTable.
        /// </summary>
        public DataTable ExecuteQuery(string query, CommandType cmdType, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();

            using (SqlCommand cmd = new SqlCommand(query, _connection))
            {
                cmd.CommandType = cmdType;

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        /// <summary>
        /// Executes a non-query command (INSERT, UPDATE, DELETE) and returns the number of affected rows.
        /// </summary>
        public int ExecuteNonQuery(string query, CommandType cmdType, SqlParameter[] parameters = null)
        {
            int affectedRows = 0;

            using (SqlCommand cmd = new SqlCommand(query, _connection))
            {
                cmd.CommandType = cmdType;

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                _connection.Open();
                affectedRows = cmd.ExecuteNonQuery();
                _connection.Close();
            }

            return affectedRows;
        }

        /// <summary>
        /// Executes a scalar command and returns the result.
        /// </summary>
        public object ExecuteScalar(string query, CommandType cmdType, SqlParameter[] parameters = null)
        {
            object result = null;

            using (SqlCommand cmd = new SqlCommand(query, _connection))
            {
                cmd.CommandType = cmdType;

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                _connection.Open();
                result = cmd.ExecuteScalar();
                _connection.Close();
            }

            return result;
        }

        /// <summary>
        /// Dispose pattern to clean up the SqlConnection.
        /// </summary>
        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}