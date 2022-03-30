using CoreProject.DataLayer.Infrastructure;
using CoreProject.Entities.Models;
using CoreProject.Entities.VMModels;
using System.Threading.Tasks;

namespace CoreProject.BusinessLayer.Infrastructure
{
    public interface ICustomerService:IBaseService<Customers>
    {
        Task<ServiceResponse<Customers>> GetUserByRoleId(int id);
        Task<ServiceResponse<Customers>> GetUser(string email, string password);
        Task<ServiceResponse<Customers>> AddUser(Customers user);

        Task<ServiceResponse<Customers>> UpdateUser(Customers user);
    }
}
