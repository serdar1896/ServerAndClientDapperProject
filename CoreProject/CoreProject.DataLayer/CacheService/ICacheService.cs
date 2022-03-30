using CoreProject.Entities.VMModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreProject.DataLayer.CacheService
{
    public interface ICacheService
    {
        Task<ServiceResponse<T>> GetAsync<T>(string key);
        Task AddAsync(string key, object data);
        Task RemoveAsync(string key);
        void Clear();
        Task<bool> AnyAsync(string key);
    }
}
