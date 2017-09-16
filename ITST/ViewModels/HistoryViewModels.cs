using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITST.ViewModels
{
    public class HistoryViewModels
    {
        public int DeviceID { get; set; }
        public string Description { get; set; }
        public string By { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Cause { get; set; }
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public string Specification { get; set; }
        public string Plant { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public string Phase { get; set; }
        public string Machine { get; set; }
        public string Status { get; set; }
        public string LocationStock { get; set; }
    }

    public class HistoryViewModelsList
    {
        public int DeviceID { get; set; }
        public string Description { get; set; }
        public string By { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Cause { get; set; }
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public string Specification { get; set; }
        public string Plant { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public string Phase { get; set; }
        public string Machine { get; set; }
        public string Status { get; set; }
        public string LocationStock { get; set; }
    }

    public class LogFileViewModels
    {
        public string ActionBy { get; set; }
        public Nullable<System.DateTime> ActionDate { get; set; }
        public string Cause { get; set; }
        public string DeviceType { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public string SerialNumber { get; set; }
        public string PRNumber { get; set; }
        public string FixAccess { get; set; }
        public string Status { get; set; }
        public string Plant { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public string Phase { get; set; }
        public string LocationStock { get; set; }
        public string Machine { get; set; }
        public string UserName { get; set; }
        public string DeviceName { get; set; }
    }
}
