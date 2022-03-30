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
   public class OrderDetails : BaseEntity
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public float UnitPrice{ get; set; }
        public float Discount { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int PaymentOption { get; set; }

      
    }
}
