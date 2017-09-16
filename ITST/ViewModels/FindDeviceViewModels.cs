using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITST.ViewModels
{
    public class FindDeviceViewModels
    {
        public int StatusID { get; set; }
        public int LocationStockID { get; set; }
        public string Status { get; set; }
        //public string Territory { get; set; }
    }

    public class AssetReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}