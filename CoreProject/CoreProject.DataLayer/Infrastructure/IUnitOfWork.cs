using CoreProject.Entities;
using CoreProject.Entities.Infrastructure;
using CoreProject.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CoreProject.DataLayer.Infrastructure
{
    public interface IUnitOfWork:IDisposable
    {
        void BeginTransaction();
        void Commit();
        void Rollback();
        IDapperContext Context { get; }
        IBaseRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new();
        //Task<T> GetbyParam<T>(object param = null) where T : class;
        //Task<IEnumerable<T>> GetListParam<T>(string whereSql = null, object param = null) where T : class;      
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, Dictionary<string, object> param = null, CommandType? commandType = null) where T : class;
        Task<IEnumerable<T>> QueryAsync<T>(string sql, Dictionary<string, object> param = null, CommandType? commandType = null) where T : class;
        Task QueryMultipleAsync(string sql, Dictionary<string, object> param = null, CommandType? commandType = null);

        Task<int> ExecuteAsync(string sql,  object param = null, CommandType? commandType = null);
        Task<T> ExecuteScalarAsync<T>(string sql, Dictionary<string, object> param = null, CommandType? commandType = null) where T : class;
        Task<int> QueryIntegerAsync(string sql, object param = null, CommandType? commandType = null);


        Task<IEnumerable<T>> GetByParamAsync<T>(object where = null, object order = null) where T : class;  



    }
}
