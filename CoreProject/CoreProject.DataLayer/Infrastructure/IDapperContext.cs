using System;
using System.Data;
using System.Data.SqlClient;

namespace CoreProject.DataLayer.Infrastructure
{
    public interface IDapperContext : IDisposable
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; set; }
        string ConnectionString { get; }
        int TimeOut { get; set; }
    }
}
