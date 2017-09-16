using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ITST.ViewModels
{
    public class CreateRequistionViewModels
    {
        public string MachineName { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage = "ModelName Required")]
        public string ModelName { get; set; }

        [Required(ErrorMessage = "SerialNumber Required")]
        public string SerialNumber { get; set; }
        public string IPAddress { get; set; }
        public string FixAccess { get; set; }
        public string PRNumber { get; set; }
        public string DeviceName { get; set; }
        public bool IsAsset { get; set; }
    }
}