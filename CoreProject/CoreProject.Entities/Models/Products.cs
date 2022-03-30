using CoreProject.Entities.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreProject.Entities.Models
{
    public class Products:BaseEntity, IAuditEntity
    {
          
        [Required(ErrorMessage = "{0} Bos gecilmez")]
        [DisplayName("Urun Adi")]
        public string Name { get; set; }
        [Required(ErrorMessage = "{0} Bos gecilmez")]
        public string SK { get; set; }
        public float PurchasePrice { get; set; }
        public float SalePrice { get; set; }
        public bool ShowOnHomePage{get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public bool? Deleted { get; set; }

        #region IAudit Implementation
        public int? CreateUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }

        #endregion
    }
}
