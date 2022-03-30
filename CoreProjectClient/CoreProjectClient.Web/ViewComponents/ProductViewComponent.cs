using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreProjectClient.Data.Models.ViewModel;

namespace CoreProjectClient.Web.ViewComponents
{
    public class ProductViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            List<Product> productList = new List<Product> {
            new Product{
                 Name="dfr",
                 SalePrice=435,
                 CategoryId=2
            },
            new Product{
               Name="",
               Description="rergreg",
               SK="4545"
            }
        };

            return View(productList);
        }
    }
}
