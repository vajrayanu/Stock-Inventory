using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ITST.ViewModels
{
    public partial class Requistion
    {
        public int DeviceID { get; set; }
        public string SerialNumber { get; set; }
        public string CauseRequistion { get; set; }
        public int MachineID { get; set; }
        public int PlantID { get; set; }
        public int DepartmentID { get; set; }
        public int Location { get; set; }
        public int Phase { get; set; }
        public string Remark { get; set; }
        public string RequestBy { get; set; }
        public int StatusID { get; set; }
        public DateTime DateRequistion { get; set; }
    }

    public class ProductionInRepair
    {
        [Required(ErrorMessage = "กรุณากรอกหมายเลขซีเรียล")]
        public string SerialNumber { get; set; }
    }

    public class ScannerInRepair
    {
        public int DeviceID { get; set; }
        public string SerialNumberR { get; set; }
        public string CauseRepair { get; set; }
        public string Uri { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public string Type { get; set; }
        public string CurrentLocation { get; set; }
        public string LocationStock { get; set; }
        public string Status { get; set; }
        public string Specification { get; set; }
        public string IPAddress { get; set; }
        public string FStatus { get; set; }
        public string Machine { get; set; }
        [StringLength(9, ErrorMessage = "จำกัด 9 ตัวอักษร")]
        public string RequestBy { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class ConfirmReceived
    {
        public int DeviceID { get; set; }
        public int LocationStock { get; set; }
        public string SerialNumber { get; set; }
        public Nullable<bool> IsReceived { get; set; }
        public string ReceivedBy { get; set; }
        public Nullable<System.DateTime> DateReceived { get; set; }
    }
}