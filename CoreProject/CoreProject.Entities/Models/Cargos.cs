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
    public class Cargos : BaseEntity
    {

        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public int CustomerAddressId { get; set; }
       
    }
}
