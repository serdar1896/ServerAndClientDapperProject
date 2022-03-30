using CoreProject.Entities.Infrastructure;
using System.Collections.Generic;

namespace CoreProject.Entities.Models
{
    public class Address: BaseEntity
    {
        public int CustomerId { get; set; }
        public int CountiesId { get; set; }
        public string Detail { get; set; }
        public string PK { get; set; }
        public string Type { get; set; }
       
    }
}
