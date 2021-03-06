//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ITST.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class Plant
    {
        public Plant()
        {
            this.Devices = new HashSet<Device>();
            this.Machines = new HashSet<Machine>();
            this.Users = new HashSet<User>();
            this.UserMachines = new HashSet<UserMachine>();
        }
    
        public int PlantID { get; set; }

        [Required(ErrorMessage = "PlantName Required")]
        public string PlantName { get; set; }
        public string Description { get; set; }
        public System.DateTime DateCreate { get; set; }
        public System.DateTime DateUpdate { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
    
        public virtual ICollection<Device> Devices { get; set; }
        public virtual ICollection<Machine> Machines { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<UserMachine> UserMachines { get; set; }
    }
}
