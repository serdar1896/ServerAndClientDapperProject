using CoreProject.Entities.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreProject.Entities.Models
{
    public class Categories : BaseEntity, IAuditEntity
    {

        [Required(ErrorMessage = "{0} Bos gecilmez")]
        [DisplayName("Kategori Adi")]
        public string Name { get; set; }
        public int? ParentId { get; set; }

        public bool? Deleted { get; set; }
        public int? CreateUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }


}
}
