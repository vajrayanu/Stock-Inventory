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
    
    public partial class LimitDeviceQuantity
    {
        public int ID { get; set; }
        public string Plant { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public string Phase { get; set; }

        [Required(ErrorMessage = "Machine Required")]
        public string Machine { get; set; }

        [Required(ErrorMessage = "MaxQuantity Required")]
        public Nullable<int> MaxQuantity { get; set; }

        [Required(ErrorMessage = "DeviceType Required")]
        public string DeviceType { get; set; }

        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> DateCreate { get; set; }
        public Nullable<System.DateTime> DateUpdate { get; set; }
    }
}