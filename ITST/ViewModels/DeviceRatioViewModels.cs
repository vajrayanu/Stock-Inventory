using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITST.Models;
using System.ComponentModel.DataAnnotations;

namespace ITST.ViewModels
{
    public class DeviceRatioViewModels
    {
        public IEnumerable<Device> Devicest { get; set; }
        public IEnumerable<Device> Devicend { get; set; }
        public IEnumerable<Device> Devicerd { get; set; }
        public IEnumerable<Device> Deviceth { get; set; }
        public IEnumerable<Device> Devicespr { get; set; }
        public IEnumerable<Model> CartridgeModel { get; set; }
        public IEnumerable<Model> Accessories { get; set; }
        public IEnumerable<Model> AccessType { get; set; }
    }

    public class IndexSearch
    {
        [Required(ErrorMessage = "keyword Required")]
        public string SerialNumber { get; set; }
    }
}