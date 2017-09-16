using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ITST.ViewModels
{
    public class SummaryRpt
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Department is Required")]
        public String DepartmentID { get; set; }

    }
}