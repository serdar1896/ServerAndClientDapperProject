using AutoMapper;
using CoreProject.BusinessLayer.Infrastructure;
using CoreProject.DataLayer.Infrastructure;
using CoreProject.DataLayer.LogService;
using CoreProject.DataLayer.Repository;
using CoreProject.Entities.Models;
using CoreProject.Entities.VMModels;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoreProject.BusinessLayer.Services
{
    public class ProductService : BaseService<Products> ,IProductService
    {
        private readonly IMapper _mapper;

        public ProductService(IMapper mapper, Commander log, IBaseRepository<Products> repository) :base(log, repository)
        {
            _mapper = mapper;
        }

        public async Task<ServiceResponse<Products>> GetShowOnPageProducts()
        {
            ServiceResponse<Products> prod;
        
                var prodlist = await GetAllAsync();
                var t = prodlist.List.FirstOrDefault(x => x.ShowOnHomePage == true);
                prod = await GetByParamAsync(new { Id = t.Id, Status = true });
            
            return prod;
        }
        public async Task<string> GetByIdForJsonResult(int id)
        {
            var product = await GetByIdAsync(id);
            var vmProduct = _mapper.Map<ServiceResponse<VMProducts>>(product);
            #region ForJonvertJson
            var serializerOptions = new JsonSerializerOptions();
            serializerOptions.Converters.Add(new JsonStringEnumConverter());
            var json = JsonSerializer.Serialize(vmProduct, serializerOptions);
            #endregion
            return json;
        }

        public async Task<ServiceResponse<VMProducts>> GetAllAsyncForVM()
        {
            var products = await GetAllAsync();       
            var vmProducts = _mapper.Map<ServiceResponse<VMProducts>>(products);
            return vmProducts;
        }
    }
}
