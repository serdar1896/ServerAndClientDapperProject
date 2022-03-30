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
    public class ProductImages : BaseEntity,IAuditEntity
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public string ImageName { get; set; }
        [Required]
        public string ContentType { get; set; }
        [Required]
        public byte[] Content { get; set; }
        public int? OrderNumber { get; set; }

        public bool? Deleted { get; set; }
        public int? CreateUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
