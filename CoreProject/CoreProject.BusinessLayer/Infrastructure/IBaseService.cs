using CoreProject.DataLayer.Infrastructure;
using CoreProject.Entities.Infrastructure;
using CoreProject.Entities.VMModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreProject.BusinessLayer.Infrastructure
{
    public interface IBaseService<T> where T : BaseEntity
    {
        Task<ServiceResponse<T>> GetAllAsync();

        Task<ServiceResponse<T>> DeleteRowAsync(int id);

        Task<ServiceResponse<T>> GetByIdAsync(int id);

        Task<ServiceResponse<T>> GetByParamAsync(object param);

        Task<ServiceResponse<T>> InsertRangeAsync(IEnumerable<T> list);

        Task<ServiceResponse<T>> UpdateAsync(T entity);

        Task<ServiceResponse<T>> InsertAsync(T entity);
    }
}
