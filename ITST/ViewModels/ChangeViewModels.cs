using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ITST.ViewModels
{
    public class ChangeViewModels
    {
        public int deviceID { get; set; }

        public string Uri { get; set; }

        [Required(ErrorMessage = "keyword Required")]
        public string keyword { get; set; }
    }

    public class UsersViewModels
    {
        public string ComputerName { get; set; }
        public string IPAddress { get; set; }
    }
}