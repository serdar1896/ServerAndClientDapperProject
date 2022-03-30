using CoreProject.Entities.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoreProject.Entities.VMModels
{
    public class VMProducts:BaseEntity
    {
        [Required]    
        public string Name { get; set; }
        public string SK { get; set; }
        public float PurchasePrice { get; set; }
        public float SalePrice { get; set; }
        public bool ShowOnHomePage { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
}
