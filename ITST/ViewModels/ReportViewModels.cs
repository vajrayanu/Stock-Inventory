using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ITST.ViewModels
{
    public class ReportViewModels
    {
        public Nullable<int> DeviceTypeID { get; set; }

        [Required(ErrorMessage = "Status is Required")]
        public int StatusID { get; set; }

        [Required(ErrorMessage = "Type is Required")]
        public string Type { get; set; }
    }

    public class ReportCriteriaViewModels
    {
        [Required]
        public string StatusID { get; set; }

        public string DeviceTypeID { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public string PRNumber { get; set; }
        public string FixAccess { get; set; }
        public string LocationStock { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
    }
}