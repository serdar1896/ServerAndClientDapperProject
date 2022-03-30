using CoreProject.Entities.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreProject.Entities.VMModels
{
    public class VMCustomers: BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SurName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Tc { get; set; }
        public DateTime? RegistirationDate { get; set; }
        public int RoleId { get; set; }
        public bool Status { get; set; }

    }
}
