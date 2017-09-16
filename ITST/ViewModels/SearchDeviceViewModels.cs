using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITST.ViewModels
{
    public partial class SearchDeviceViewModels
    {
        public string DeviceType { get; set; }
        public string Status { get; set; }
        public string Department { get; set; }
        public int TotalDevice { get; set; }
        public int ITTotalUse { get; set; }
        public int ITTotalSpare { get; set; }
        public int ITTotalInRepair { get; set; }
        public int ITTotalInStock { get; set; }
    }
}