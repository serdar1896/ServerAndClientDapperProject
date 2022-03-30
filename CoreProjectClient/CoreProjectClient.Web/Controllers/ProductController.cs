using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreProjectClient.Data.Models;
using CoreProjectClient.Data.Models.ViewModel;
using CoreProjectClient.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CoreProjectClient.Web.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        ProductService _productService;
        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        public async Task<ActionResult> Index()
        {
            
            var list= await _productService.GetProduct();
            
            return View();
            
        }

    }
}