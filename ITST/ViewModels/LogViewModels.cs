using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ITST.ViewModels
{
    public class LogViewModels
    {
        [Required(ErrorMessage = "StartDate is Required")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy hh:mm:ss tt}")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "EndDate is Required")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy hh:mm:ss tt}")]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Format is Required")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy hh:mm:ss tt}")]
        public String Format { get; set; }
        [Required(ErrorMessage = "Status is Required")]
        public int StatusID { get; set; }
        //[Required(ErrorMessage = "Status is Required")]
        public string DeviceTypeID { get; set; }
    }
}