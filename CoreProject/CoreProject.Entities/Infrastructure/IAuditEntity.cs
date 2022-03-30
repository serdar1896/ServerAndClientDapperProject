using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreProject.Entities.Infrastructure
{
     public interface IAuditEntity
    {
        bool? Deleted { get; set; }

        int? CreateUserId { get; set; }
        DateTime? CreatedDate { get; set; }
        int? UpdatedUserId{ get; set; }
        DateTime? UpdatedDate { get; set; }
    }
}
