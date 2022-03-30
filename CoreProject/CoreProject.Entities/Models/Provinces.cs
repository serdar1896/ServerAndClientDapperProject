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
    public class Provinces : BaseEntity
    {
        public string Name { get; set; }
        public int CountryId { get; set; }
       
    }
}
