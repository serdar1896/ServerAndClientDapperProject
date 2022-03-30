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
    public class ProductComment : BaseEntity, IAuditEntity
    {
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public string Comment { get; set; }
        public bool Status { get; set; }
        public bool? Deleted { get; set; }

        public int? CreateUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}