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
    
    public partial class RecordInRepair
    {
        public int InRepairID { get; set; }
        public string RequestBy { get; set; }
        public Nullable<System.DateTime> DateRequest { get; set; }
        public string Cause { get; set; }
        public string DeviceType { get; set; }
        public string Brand { get; set; }
        public string SerialNumber { get; set; }
        public string Status { get; set; }
        public string Department { get; set; }
        public string Plant { get; set; }
        public string Location { get; set; }
        public string Phase { get; set; }
        public string LocationStock { get; set; }
        public string Machine { get; set; }
        public string Model { get; set; }
        public string UserName { get; set; }
        public string DeviceName { get; set; }
        public string IsFixAsset { get; set; }
        public string ReceivedBy { get; set; }
        public string RequestFullName { get; set; }
        public Nullable<System.DateTime> DateReceived { get; set; }
        public string ReturnedBy { get; set; }
        public Nullable<System.DateTime> DateReturned { get; set; }
    }
}