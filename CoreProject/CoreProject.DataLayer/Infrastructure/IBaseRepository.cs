using CoreProject.Entities;
using CoreProject.Entities.Infrastructure;
using CoreProject.Entities.Models;
using CoreProject.Entities.VMModels;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CoreProject.DataLayer.Infrastructure
{
    public interface IBaseRepository<T> where T: BaseEntity
    {
        Task<IEnumerable<T>> GetAll();

        Task<T> GetById(int id);

        Task<IEnumerable<T>> GetByParam(object param);

        Task<int> Update(T entity);

        Task<int> Insert(T entity);

        Task<int> InsertRange(IEnumerable<T> list);

        Task<int> DeleteRow(int id);


        #region Joinlemek için kullanılır Generic hale getir


        //Task<List<Products>> GetProductsOneToMany();
        //Task<IEnumerable<ProdAndCat>> GetForModel();
        #endregion

    }
}
