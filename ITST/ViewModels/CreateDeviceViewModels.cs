using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ITST.ViewModels
{
    public class CreateDeviceViewModels
    {
        [Required(ErrorMessage = "SerialNumber Required")]
        public string SerialNumber { get; set; }

        [Required(ErrorMessage = "ModelName Required")]
        public string ModelName { get; set; }

        [Required(ErrorMessage = "LocationStock Required")]
        public int LocationStock { get; set; }

        public string DeviceName { get; set; }
        public string IPAddress { get; set; }
        public string MacAddress { get; set; }
        public string FixAccess { get; set; }
        public string PRNumber { get; set; }
        public bool IsAsset { get; set; }
        public bool IsNoPRNumber { get; set; }
    }

    public class CreateCartridgeViewModels
    {
        [Required(ErrorMessage = "PrinterModel Required")]
        public int MachineName { get; set; }

        [Required(ErrorMessage = "ModelName Required")]
        public string ModelName { get; set; }

        [Required(ErrorMessage = "Quantity Required")]
        public int Quantity { get; set; }
    }

    public class CreateAccessoriesViewModels
    {
        [Required(ErrorMessage = "ModelName Required")]
        public string ModelName { get; set; }

        [Required(ErrorMessage = "Quantity Required")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "LocationStock Required")]
        public int LocationStock { get; set; }
    }

    public class CreateCartridgePrinterViewModels
    {
        [Required(ErrorMessage = "Name Required")]
        public string MachineName { get; set; }
    }
}