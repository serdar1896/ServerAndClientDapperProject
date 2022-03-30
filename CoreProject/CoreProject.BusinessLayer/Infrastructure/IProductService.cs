using CoreProject.DataLayer.Infrastructure;
using CoreProject.Entities.Models;
using CoreProject.Entities.VMModels;
using System.Threading.Tasks;

namespace CoreProject.BusinessLayer.Infrastructure
{
    public interface IProductService:IBaseService<Products>
    {
        Task<ServiceResponse<Products>> GetShowOnPageProducts();

        Task<string> GetByIdForJsonResult(int id);

        Task<ServiceResponse<VMProducts>> GetAllAsyncForVM();
    }
}
