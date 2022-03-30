using AutoMapper;
using CoreProject.BusinessLayer.Infrastructure;
using CoreProject.BusinessLayer.Services;
using CoreProject.DataLayer.Infrastructure;
using CoreProject.Entities.Models;
using CoreProject.Entities.VMModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoreProject.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService productService;

        public ProductController(  IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet("GetAll")]
        public async Task<ServiceResponse<VMProducts>> GetAll()
        {
            return await productService.GetAllAsyncForVM();
        }

        [HttpGet("GetById/{id}")]
        public async Task<ServiceResponse<Products>> GetById(int? id)
        {
            return await productService.GetByIdAsync(id.Value);
        }
       

        [HttpPost("InsertProduct")]
        public async Task<ServiceResponse<Products>> InsertProduct(Products product)
        {
            return await productService.InsertAsync(product);
        }

        [HttpPost("InsertRangeProduct")]
        public async Task<ServiceResponse<Products>> InsertRangeProduct(IEnumerable<Products> products)
        {
            return await productService.InsertRangeAsync(products);
        }

        [HttpPut("UpdateProduct")]
        public async Task<ServiceResponse<Products>> UpdateProduct(Products product)
        {
            return await productService.UpdateAsync(product);
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<ServiceResponse<Products>> DeleteProduct(int? id)
        {
            return await productService.DeleteRowAsync(id.Value);
        }

    }
}

