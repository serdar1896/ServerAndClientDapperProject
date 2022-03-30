using CoreProject.DataLayer.Helpers;
using CoreProject.DataLayer.Helpers.Enum;
using CoreProject.DataLayer.Infrastructure;
using CoreProject.DataLayer.LogService;
using CoreProject.Entities.Infrastructure;
using CoreProject.Entities.Models;
using CoreProject.Entities.VMModels;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CoreProject.DataLayer.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private readonly string _tableName;
        public IUnitOfWork _unitOfWork { get; private set; }

        readonly Commander _nlog;
        readonly OrmHelper OrmHelper;
        readonly OrmSqlEnum OrmSqlEnum;

        public BaseRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            _tableName = typeof(T).Name;
            OrmHelper = new OrmHelper(typeof(T));
            OrmSqlEnum = new OrmSqlEnum();
            _nlog = new Commander(_unitOfWork.Context.ConnectionString);
        }

        public Task<IEnumerable<T>> GetAll()
        {
            return _unitOfWork.QueryAsync<T>(OrmSqlEnum.GetAllSQL<T>());
        }

        public async Task<T> GetById(int id)
        {
            return await _unitOfWork.QueryFirstOrDefaultAsync<T>(OrmSqlEnum.GetByIdSQL<T>(), new Dictionary<string, object> { { "Id", id } });            
        }

        public async Task<IEnumerable<T>> GetByParam(object param)      
        {
            return await _unitOfWork.GetByParamAsync<T>(param); 
        }

        public async Task<int> Insert(T entity)
        {   
            var insertQuery = OrmHelper.GenerateInsertQuery();

            return await _unitOfWork.QueryIntegerAsync(insertQuery, entity);   
        }


        public async Task<int> InsertRange(IEnumerable<T> list)
        {
            var query = OrmHelper.GenerateInsertQuery();
            
            return await _unitOfWork.ExecuteAsync(query, (dynamic)list);
               
        }

        public async Task<int> Update(T entity)
        {           
                var updateQuery = OrmHelper.GenerateUpdateQuery();

                return await _unitOfWork.ExecuteAsync(updateQuery, entity);   
        }


        public async Task<int> DeleteRow(int id)
        {
            return await _unitOfWork.ExecuteAsync(OrmSqlEnum.DeleteSQL<T>(), new Dictionary<string, object> { { "Id", id } });
        }



        #region Joinlemek için kullanılır Generic hale getir
        //JOIN için yada ortak bir model tanımlayıp istedigimizi cekebilirz uymayan icin asla modelde ki ad verilir yada aşağıdaki metottaki gibi mapleme yapılır.

        //    public async Task<IEnumerable<ProdAndCat>> GetForModel()
        //    {
        //        string sqlQuery = @"SELECT p.*, c.parentId ,c.name as categoryname
        //from [products] p
        //inner join categories c on p.CategoryId = c.Id ";


        //        return await _unitOfWork.QueryAsync<ProdAndCat>(sqlQuery);
        //    }

        //    public async Task<List<Products>> GetProductsOneToMany()
        //    {
        //        string sqlQuery = @"SELECT * from [products] p
        //        inner join categories c on p.CategoryId = c.Id ";


        //        var res = await _unitOfWork.Context.Connection.QueryAsync<Products, Categories, Products>(sqlQuery,
        //            (product, category) =>
        //            {
        //                Products prod = product;
        //                prod.CategoryId = category.Id;

        //                return prod;

        //            }, splitOn: "Id");

        //        return res.ToList();
        //    }
        #endregion

    }
}
