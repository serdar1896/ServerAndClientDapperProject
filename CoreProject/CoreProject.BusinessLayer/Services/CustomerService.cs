using CoreProject.BusinessLayer.Infrastructure;
using CoreProject.DataLayer.CacheService;
using CoreProject.DataLayer.Infrastructure;
using CoreProject.DataLayer.LogService;
using CoreProject.Entities.Models;
using CoreProject.Entities.VMModels;
using System.Threading.Tasks;

namespace CoreProject.BusinessLayer.Services
{
    public class CustomerService : BaseService<Customers>, ICustomerService
    {
        private readonly ICacheService _redisService;
        public CustomerService(

            ICacheService redisService,
            Commander log, 
            IBaseRepository<Customers> repository

            ): base(log, repository) 
        {
            _redisService = redisService;

        }

        public async Task<ServiceResponse<Customers>> GetUserByRoleId(int id)
        {
            return await GetByParamAsync(new { RoleId = id, Status = true });
        }

        public async Task<ServiceResponse<Customers>> GetUser(string email, string password)
        {
            var cache = await _redisService.GetAsync<Customers>("user");
            if (cache==null)
            {
                return await GetByParamAsync(new { Email = email, Password = password, Status = true });
            }
            return cache;
        }
        public async Task<ServiceResponse<Customers>> AddUser(Customers user)
        {
            var response = await InsertAsync(user);
            if (! await _redisService.AnyAsync("user"))
            {
                await _redisService.AddAsync("user", user);
            }
            return response;
        }
        public async Task<ServiceResponse<Customers>> UpdateUser(Customers user)
        {
            var response = await UpdateAsync(user);
            await _redisService.RemoveAsync("user");
            return response;
        }
    }
}