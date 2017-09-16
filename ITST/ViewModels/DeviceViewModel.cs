using ITST.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ITST.ViewModels
{
    public partial class DeviceViewModel
    {
        public int DeviceID { get; set; }
        public Nullable<int> MachineID { get; set; }
        public Nullable<int> UMachineID { get; set; }
        public string DeviceName { get; set; }
        public string Description { get; set; }

        public string CauseRequistion { get; set; }

        public double PricePerItem { get; set; }

        [Required(ErrorMessage = "SerialNumber Required")]
        public string SerialNumber { get; set; }
        public string Specification { get; set; }
        public Nullable<int> DepartmentID { get; set; }
        public Nullable<int> PlantID { get; set; }
        public Nullable<int> LocationID { get; set; }
        public Nullable<int> DeviceTypeID { get; set; }
        public Nullable<int> BrandID { get; set; }
        public Nullable<int> StatusID { get; set; }
        public Nullable<int> ModelID { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy hh:mm:ss tt}")]
        public Nullable<System.DateTime> DateCreate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy hh:mm:ss tt}")]
        public Nullable<System.DateTime> DateUpdate { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }

        public string BillReceiptNo { get; set; }

        //[Required(ErrorMessage = "LocationStock Required")]
        public Nullable<int> LocationStockID { get; set; }
        public string LocationStockName { get; set; }

        [Required(ErrorMessage = "ModelName Required")]
        public string ModelName { get; set; }
        public string Type { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage = "MachineName Required")]
        public string MachineName { get; set; }

        public string BrandName { get; set; }
        public string StatusName { get; set; }
        public Nullable<System.DateTime> InstockDate { get; set; }
        public Nullable<int> PhaseID { get; set; }
        public string PhaseName { get; set; }
        public string PRNumber { get; set; }
        public string FixAccess { get; set; }
        public string IPAddress { get; set; }
        public Nullable<int> UserID { get; set; }
        public string PlantName { get; set; }
        public string DepartmentName { get; set; }
        public string LocationName { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual Department Department { get; set; }
        public virtual DeviceType DeviceType { get; set; }
        public virtual Location Location { get; set; }
        public virtual Machine Machine { get; set; }
        public virtual Model Model { get; set; }
        public virtual Plant Plant { get; set; }
        public virtual Status Status { get; set; }
        public virtual UserMachine UserMachine { get; set; }
        public virtual User User { get; set; }
    }

    public partial class DeviceViewModels
    {
        public int DeviceID { get; set; }
        public Nullable<int> MachineID { get; set; }
        public Nullable<int> UMachineID { get; set; }
        public string DeviceName { get; set; }
        public string Description { get; set; }
        public string PlantName { get; set; }
        public string DepartmentName { get; set; }
        public string LocationName { get; set; }

        public string CauseRequistion { get; set; }

        public double PricePerItem { get; set; }

        [Required(ErrorMessage = "SerialNumber Required")]
        public string SerialNumber { get; set; }
        public string Specification { get; set; }
        public Nullable<int> DepartmentID { get; set; }
        public Nullable<int> PlantID { get; set; }
        public Nullable<int> LocationID { get; set; }
        public Nullable<int> DeviceTypeID { get; set; }
        public Nullable<int> BrandID { get; set; }
        public Nullable<int> StatusID { get; set; }
        public Nullable<int> ModelID { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy hh:mm:ss tt}")]
        public Nullable<System.DateTime> DateCreate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy hh:mm:ss tt}")]
        public Nullable<System.DateTime> DateUpdate { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }

        public string BillReceiptNo { get; set; }

        //[Required(ErrorMessage = "LocationStock Required")]
        public Nullable<int> LocationStockID { get; set; }
        public string LocationStockName { get; set; }

        [Required(ErrorMessage = "ModelName Required")]
        public string ModelName { get; set; }
        public string Type { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage = "MachineName Required")]
        public string MachineName { get; set; }

        public string BrandName { get; set; }
        public string StatusName { get; set; }
        public Nullable<System.DateTime> InstockDate { get; set; }
        public Nullable<int> PhaseID { get; set; }
        public string PhaseName { get; set; }
        public string PRNumber { get; set; }
        public string FixAccess { get; set; }
        public string IPAddress { get; set; }
        public Nullable<int> UserID { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual Department Department { get; set; }
        public virtual DeviceType DeviceType { get; set; }
        public virtual Location Location { get; set; }
        public virtual Machine Machine { get; set; }
        public virtual Model Model { get; set; }
        public virtual Plant Plant { get; set; }
        public virtual Status Status { get; set; }
        public virtual UserMachine UserMachine { get; set; }
        public virtual User User { get; set; }
    }

    public class CartridgeRequisition
    {
        public int DeviceID { get; set; }

        [Required(ErrorMessage = "Plant Required")]
        public int PlantID { get; set; }

        [Required(ErrorMessage = "Location Required")]
        public int LocationID { get; set; }

        public int PhaseID { get; set; }
        public int ModelID { get; set; }
        public int MachineID { get; set; }
    }

    public class AccessoriesRequisition
    {
        public int DeviceID { get; set; }

        [Required(ErrorMessage = "Plant Required")]
        public int PlantID { get; set; }

        [Required(ErrorMessage = "Location Required")]
        public int LocationID { get; set; }

        public int PhaseID { get; set; }
    }

    public class MultipleCartridgeRequisition
    {
        public int DeviceID { get; set; }

        [Required(ErrorMessage = "Plant Required")]
        public int PlantID { get; set; }

        [Required(ErrorMessage = "Location Required")]
        public int LocationID { get; set; }

        [Required(ErrorMessage = "Phase Required")]
        public int PhaseID { get; set; }

        public int ModelID { get; set; }

        [Required(ErrorMessage = "Model Required")]
        public string ModelName { get; set; }

        public int MachineID { get; set; }

        [Required(ErrorMessage = "Quantity Required")]
        public int Quantity { get; set; }
    }

    public class MultipleAccessoriesRequisition
    {
        public int DeviceID { get; set; }

        public int PlantID { get; set; }

        public int LocationID { get; set; }

        public int PhaseID { get; set; }

        public int ModelID { get; set; }

        [Required(ErrorMessage = "Model Required")]
        public string ModelName { get; set; }

        public int MachineID { get; set; }

        public string MachineName { get; set; }

        [Required(ErrorMessage = "Quantity Required")]
        public int Quantity { get; set; }
    }

    public class setChangeLocationStockAccessories
    {
        [Required(ErrorMessage = "Model Required")]
        public string ModelName { get; set; }

        [Required(ErrorMessage = "Quantity Required")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "LocationStock Requied")]
        public string LocationStock { get; set; }

        [Required(ErrorMessage = "Previous LocationStock Requied")]
        public string PreviousLocationStock { get; set; }
    }

    public class setMultipleAccessoriesRequisition
    {

        public string PlantID { get; set; }
        public string LocationID { get; set; }
        public string PhaseID { get; set; }
        public string MachineName { get; set; }

        [Required(ErrorMessage="LocationStock Requied")]
        public string LocationStock { get; set; }

        [Required(ErrorMessage = "Quantity Required")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Model Required")]
        public string ModelName { get; set; }
    }

    public class TotalDeviceViewModel
    {
        public string IPAddress { get; set; }
        public string PlantName { get; set; }
        public string DepartmentName { get; set; }
        public string LocationName { get; set; }
        public string PhaseName { get; set; }
        public string SerialNumber { get; set; }
        public string LocationStockName { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> DateCreate { get; set; }
        public string Type { get; set; }
        public string ModelName { get; set; }
        public string StatusName { get; set; }
        public string MachineName { get; set; }
        public string UserName { get; set; }
        public Nullable<System.DateTime> DateUpdate { get; set; }
        public string BrandName { get; set; }
        public int DeviceID { get; set; }
    }

    public class ViewModelsDevices
    {
        public int DeviceID { get; set; }
        public string DeviceName { get; set; }
        public string Description { get; set; }
        public bool IsFix { get; set; }
        public string SerialNumber { get; set; }
        public string Specification { get; set; }
        public string Reason { get; set; }

        public string Model { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public string PRNumber { get; set; }
        public string FixAccess { get; set; }
        public string IPAddress { get; set; }
        public string MacAddress { get; set; }
        public string Asset { get; set; }
        public string Image { get; set; }

        public string Status { get; set; }


        public string UserName { get; set; }
        public string MachineName { get; set; }

        public string Plant { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public string Phase { get; set; }
        public string LocationStockName { get; set; }
        public int SerialID { get; set; }

        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> DateCreate { get; set; }
        public Nullable<System.DateTime> DateUpdate { get; set; }
        public Nullable<System.DateTime> InstockDate { get; set; }
    }

    public class ViewModelsDevice
    {
        public int DeviceID { get; set; }
        public string DeviceName { get; set; }
        public string Description { get; set; }
        public bool IsFix { get; set; }
        public string SerialNumber { get; set; }
        public string Specification { get; set; }

        public string Model { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public string PRNumber { get; set; }
        public string FixAccess { get; set; }
        public string IPAddress { get; set; }
        public string MacAddress { get; set; }
        public string Asset { get; set; }

        public string Status { get; set; }


        public string UserName { get; set; }
        public string MachineName { get; set; }

        public string Plant { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public string Phase { get; set; }
        public string LocationStockName { get; set; }
        public string Reason { get; set; }

        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> DateCreate { get; set; }
        public Nullable<System.DateTime> DateUpdate { get; set; }
        public Nullable<System.DateTime> InstockDate { get; set; }
    }

    public class ViewModelDevice
    {
        public int DeviceID { get; set; }
        public int SerialID { get; set; }
        public string DeviceName { get; set; }
        public string SerialNumber { get; set; }
        public string IPAddress { get; set; }
        public string MacAddress { get; set; }
        public string Asset { get; set; }
        public string FixAccess { get; set; }
        public string PRNumber { get; set; }
        public string Specification { get; set; }
        public string Image { get; set; } 
        public string Model { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public string Status { get; set; }
        public string UserName { get; set; }
        public string MachineName { get; set; }
        public string Plant { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public string Phase { get; set; }
        public string LocationStockName { get; set; }
        public string Reason { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> DateCreate { get; set; }
        public Nullable<System.DateTime> DateUpdate { get; set; }
    }


    public class PhoneListViewModel
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string Plant { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public Nullable<System.DateTime> DateCreate { get; set; }
        public Nullable<System.DateTime> DateUpdate { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public string Phase { get; set; }
        public string Phone { get; set; }
    }

    public class UserViewModel
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmployeeID { get; set; }
        public string Position { get; set; }
        public string Section { get; set; }
        public string Phone { get; set; }
        public string DepartmentName { get; set; }
        public string LocationName { get; set; }
        public string PlantName { get; set; }
        public string UserLogOn { get; set; }
        public string IPAddress { get; set; }
        public string MacAddress { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> DateCreate { get; set; }
        public Nullable<System.DateTime> DateUpdate { get; set; }
        public Nullable<int> PhaseID { get; set; }
        public string PhaseName { get; set; }
        public string FullName { get; set; }
        public string DeviceName { get; set; }
    }

    public class ViewModelsUser
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string EmployeeID { get; set; }
        public string Position { get; set; }
        public string Section { get; set; }
        public string Phone { get; set; }
        public string Id { get; set; }
        public string Role { get; set; }
    }

    public class MachineViewModel
    {
        public int MachineID { get; set; }
        public string MachineName { get; set; }
        public string Description { get; set; }
        public string Plant { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public Nullable<System.DateTime> DateCreate { get; set; }
        public Nullable<System.DateTime> DateUpdate { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public string Phase { get; set; }
        public string IPAddress { get; set; }
        public string PLCAddress { get; set; }
        public string MACAddress { get; set; }
    }

    public class ViewDeviceModels
    {
        public int ModelID { get; set; }
        public string ModelName { get; set; }
        public string DeviceType { get; set; }
        public string BrandName { get; set; }
        public string Specification { get; set; }
        public Nullable<bool> IsAccess { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> DateCreate { get; set; }
        public Nullable<System.DateTime> DateUpdate { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
    }

    public class ViewModelsChart
    {
        public string Dapartment { get; set; }
        public int UserQuantity { get; set; }
        public int DeviceQuantity { get; set; }
    }

    public class ViewModelsChartScanner
    {
        public string Model { get; set; }
        public int InStock { get; set; }
        public int ReqQTY { get; set; }
        public int Minimum { get; set; }
    }
}