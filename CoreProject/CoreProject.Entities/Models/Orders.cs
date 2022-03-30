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
    public class Orders : BaseEntity, IAuditEntity
    {
        public DateTime OrderDate { get; set; }
        public float TotalPrice { get; set; }
        public bool InBasket { get; set; }
        public bool Status { get; set; }
        public int CargoId { get; set; }

        //Kargo.adres.adresid oIarak cekiceksin ve uye ID        
        public bool? Deleted { get; set; }

        public int? CreateUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
