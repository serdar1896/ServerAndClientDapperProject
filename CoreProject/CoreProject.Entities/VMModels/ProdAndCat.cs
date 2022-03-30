using CoreProject.Entities.Infrastructure;
using CoreProject.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreProject.Entities.VMModels
{
   public class ProdAndCat:Products
    {
        public int ParentId { get; set; }
        public string CategoryName { get; set; }

    }
}
