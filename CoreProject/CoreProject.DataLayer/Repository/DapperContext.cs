using CoreProject.DataLayer.Infrastructure;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CoreProject.DataLayer.Repository
{
    public class DapperContext : IDapperContext
    {
        private readonly string connectionString;
        public DapperContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private IDbConnection connection;
        public IDbConnection Connection
        {
            get
            {
                if (connection == null)
                {
                    connection = new SqlConnection(connectionString);
                }

                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return connection;
            }

        }
        public string ConnectionString
        {
            get { return connectionString; }
        }

        public int TimeOut { get; set; }

        public IDbTransaction Transaction { get; set; }

        public void Dispose()
        {
            Transaction?.Dispose();

            if (connection != null && connection.State != ConnectionState.Closed)
                connection.Close();
        }
       

    }
}
