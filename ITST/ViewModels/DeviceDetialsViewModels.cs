using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITST.Models;
using ITST.ViewModels;

namespace ITST.ViewModels
{
    public class DeviceDetialsViewModels
    {
        public IEnumerable<Device> Device { get; set; }
        public IEnumerable<DeviceType> DeviceType { get; set; }
    }
}