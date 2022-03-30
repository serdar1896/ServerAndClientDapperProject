using CoreProject.Entities.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreProject.Entities.Models
{
    public class Stocks : BaseEntity,IAuditEntity
    {
        public int ProductId { get; set; }
        public string Quantity { get; set; }
        public string WarehouseAddress { get; set; }
        public int Type { get; set; }
        public bool? Deleted { get; set; }

        public int? CreateUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
