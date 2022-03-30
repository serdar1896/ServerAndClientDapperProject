using AutoMapper;
using CoreProject.BusinessLayer.Infrastructure;
using CoreProject.Entities.Models;
using CoreProject.Entities.VMModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreProject.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private ICustomerService customerService;

        public UserController(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        [HttpGet("GetAll")]
        public async Task<ServiceResponse<Customers>> Get()
        {
            return await customerService.GetAllAsync();
        }

        [HttpGet("GetById/{id}")]
        public async Task<ServiceResponse<Customers>> Get(int? id)
        {
            return await customerService.GetByIdAsync(id.Value);
        }
        [HttpGet("GetUser")]
        public async Task<ServiceResponse<Customers>> GetUser(string email, string password)
        {
            return await customerService.GetUser(email,password); 
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<ServiceResponse<Customers>> Delete(int? id)
        {
            return await customerService.DeleteRowAsync(id.Value);
        }


        [HttpPost("InsertUser")]
        public async Task<ServiceResponse<Customers>> InsertUser(Customers customer)
        {
            return await customerService.AddUser(customer);
        }

        [HttpPut("UpdateUser")]
        public async Task<ServiceResponse<Customers>> Update(Customers customer)
        {
            return await customerService.UpdateUser(customer);
        }


    }
}
