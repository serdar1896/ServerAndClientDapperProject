using CoreProject.DataLayer.Infrastructure;
using CoreProject.DataLayer.LogService;
using CoreProject.Entities;
using CoreProject.Entities.Infrastructure;
using CoreProject.Entities.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CoreProject.DataLayer.Repository
{   
    public class UnitOfWork : IUnitOfWork
    {
        readonly Commander _nlog;

        private bool _disposed;
        private readonly IConfiguration configuration;
        public IDapperContext Context { get; set; }

        public Dictionary<Type, dynamic> repositories = new Dictionary<Type, dynamic>();

        public UnitOfWork(IConfiguration configuration)
        {
            this.configuration = configuration;

            Context = new DapperContext(this.configuration.GetConnectionString("Baglanti")) { TimeOut=120 };
            _nlog = new Commander(Context.ConnectionString);

        }
        public void BeginTransaction()
        {
            Context.Transaction = Context.Connection.BeginTransaction();
        }


        public void Commit()
        {
            if (Context.Transaction != null)
            {
                try
                {
                    Context.Transaction.Commit();

                }
                catch (Exception ex)
                {
                    Context.Transaction.Rollback();
                    throw ex;
                }
                finally
                {
                    Context.Transaction.Dispose();
                }
            }
        }

        public void Rollback()
        {
            if (Context.Transaction == null) return;
            try
            {
                Context.Transaction.Rollback();

            }
            catch (Exception ex)
            {
                //logger.LogTrace(exception: ex);
                Context.Transaction.Rollback();
                throw ex;
            }
            finally
            {
                Context.Transaction.Dispose();
            }
        }
        public IBaseRepository<T> GetRepository<T>() where T : BaseEntity, new()
        {
            if (repositories.ContainsKey(typeof(T)))
            {
                return repositories[typeof(T)] as IBaseRepository<T>;
            }
            IBaseRepository<T> repository = new BaseRepository<T>(this);
            repositories.Add(typeof(T), repository);

            return repository;

            //var repositoryType = typeof(BaseRepository<>);
            //repositories.Add(typeof(T), Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), this.Context));
            //return repositories[typeof(T)];
        }

      
        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        private void dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (Context != null)
                    {
                        Context.Dispose();
                    }
                }
                _disposed = true;
            }
        }
        //public Task<IEnumerable<T>> GetListParam<T>(string whereSql=null ,object param = null) where T:class
        //{
        //    return  Context.Connection.GetListAsync<T>(whereSql, param);

        //}

        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, Dictionary<string, object> param = null, CommandType? commandType = null) where T : class
        {
            return Context.Connection.QueryFirstOrDefaultAsync<T>(sql,param, Context.Transaction, Context.TimeOut, commandType);
        }
        
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, Dictionary<string, object> param =null, CommandType? commandType=null) where T : class
        {
            return Context.Connection.QueryAsync<T>(sql, param, Context.Transaction, Context.TimeOut, commandType);
        }
        public async Task<int> QueryIntegerAsync(string sql,  object param = null, CommandType? commandType = null) 
        {
            return await Context.Connection.QueryFirstOrDefaultAsync<int>(sql, param, Context.Transaction, Context.TimeOut, commandType);
        }
        public Task QueryMultipleAsync(string sql, Dictionary<string, object> param = null, CommandType? commandType = null)
        {

            return Context.Connection.QueryMultipleAsync(sql, param, Context.Transaction, Context.TimeOut, commandType);
        }
        public Task<int> ExecuteAsync(string sql,  object param = null, CommandType? commandType = null)
        {
            return Context.Connection.ExecuteAsync(sql,param, Context.Transaction, Context.TimeOut, commandType);
        }

        public Task<T> ExecuteScalarAsync<T>(string sql, Dictionary<string, object> param = null, CommandType? commandType = null) where T : class
        {

                return Context.Connection.ExecuteScalarAsync<T>(sql, param,Context.Transaction,Context.TimeOut,commandType);
        }
     

        public async Task<IEnumerable<T>> GetByParamAsync<T>(object where = null, object order = null) where T : class  
        {
           return await Context.Connection.GetByAsync<T>(where, order, Context.Transaction, Context.TimeOut);
        }

       
    }
}
