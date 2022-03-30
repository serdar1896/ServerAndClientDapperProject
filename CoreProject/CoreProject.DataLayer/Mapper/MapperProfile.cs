using AutoMapper;
using CoreProject.Entities.Models;
using CoreProject.Entities.VMModels;

namespace CoreProject.DataLayer.Mapper
{
    public class MapperProfile: Profile
    {
        public MapperProfile()
        {
            CreateMap<Products, VMProducts>();
            CreateMap<VMProducts, Products>();

            CreateMap<ServiceResponse<Products>, ServiceResponse<VMProducts>>();
            CreateMap<ServiceResponse<VMProducts>, ServiceResponse<Products>>();

            CreateMap<ServiceResponse<Customers>, ServiceResponse<VMCustomers>>();
            CreateMap<ServiceResponse<VMCustomers>, ServiceResponse<Customers>>();

            //CreateMap<ProductsDto, Products>().ForMember(des => des.Id, opt => opt.Ignore())
            //  .ForMember(des => des.SK, opt => opt.MapFrom(src => src.SK));

        }
    }
}
