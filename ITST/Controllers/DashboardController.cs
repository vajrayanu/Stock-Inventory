using ITST.CustomFilters;
using ITST.Models;
using ITST.ViewModels;
using Microsoft.Reporting.Common;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ITST.Controllers
{
    public class DashboardController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        private ITStockEntities dtx = new ITStockEntities();
        //
        // GET: /Dashboard/

        [Authorize]
        public ActionResult Index(int? id)
        {
            var viewmodel = new DeviceRatioViewModels();
            viewmodel.Devicest = from device in db.Devices group device by device.DeviceType.DeviceTypeID  into dv let m = dv.FirstOrDefault() select m;
            if (id != null)
            {
                viewmodel.Devicend = from device in db.Devices where device.DeviceType.DeviceTypeID == id group device by device.Model.ModelID into dv let m = dv.FirstOrDefault() select m;
            }
            else if (id == null)
            {
                viewmodel.Devicend = from device in db.Devices where device.DeviceType.DeviceTypeID == 50 group device by device.Model.ModelID into dv let m = dv.FirstOrDefault() select m;
            }
            //DateTime date = DateTime.Today;
            //var Date = date.ToString("dd");
            //var Month = date.ToString("MM");
            //int Dt = Int32.Parse(Date);
            //int Mt = Int32.Parse(Month);
            //int start = Mt + 12;
            string dateTime = "01/01/2016 00:00:00.00";
            DateTime std = Convert.ToDateTime(dateTime);
            DateTime crd = DateTime.Now;
            var totalmonth = ((crd.Year - std.Year) * 12) + crd.Month;

            ViewBag.Use = db.Devices.Where(d => d.StatusID == 1).Count();
            ViewBag.InRepair = db.Devices.Where(d => d.StatusID == 2).Count();
            ViewBag.InStock = db.Devices.Where(d => d.StatusID == 3).Count();
            ViewBag.InSale = db.Devices.Where(d => d.StatusID == 4).Count();
            ViewBag.Spare = db.Devices.Where(d => d.StatusID == 5).Count();
            ViewBag.SentRepair = db.Devices.Where(d => d.StatusID == 6).Count();
            ViewBag.SentSale = db.Devices.Where(d => d.StatusID == 7).Count();
            ViewBag.WSentRepair = db.Devices.Where(d => d.StatusID == 8).Count();
            ViewBag.WSentSale = db.Devices.Where(d=>d.StatusID == 9).Count();

            ViewBag.TotalUser = db.Users.Count();
            ViewBag.TotalMachine = db.Machines.Count();
            ViewBag.TotalDevice = db.Devices.Count();
            ViewBag.TotalMonth = totalmonth;
            //ViewBag.Date = Dt;
            //ViewBag.Start = start;
            //ViewBag.Month = Mt;
            return View(viewmodel);
        }

        public ActionResult MainMenu()
        {
            ViewBag.Use = db.Devices.Where(d => d.StatusID == 1).Count();
            ViewBag.InRepair = db.Devices.Where(d => d.StatusID == 2).Count();
            ViewBag.InStock = db.Devices.Where(d => d.StatusID == 3).Count();
            ViewBag.InSale = db.Devices.Where(d => d.StatusID == 4).Count();
            ViewBag.Spare = db.Devices.Where(d => d.StatusID == 5).Count();
            ViewBag.Access = db.Devices.Where(d => d.Model.IsAccess == true).Count();
            ViewBag.SentRepair = db.Devices.Where(d => d.StatusID == 6).Count();
            ViewBag.SentSale = db.Devices.Where(d => d.StatusID == 7).Count();
            ViewBag.WSentRepair = db.Devices.Where(d => d.StatusID == 8).Count();
            ViewBag.WSentSale = db.Devices.Where(d => d.StatusID == 9).Count();
            ViewBag.UnknownStatus = db.Devices.Where(d => d.StatusID == 10).Count();
            ViewBag.Destroyed = db.Devices.Where(d => d.StatusID == 12).Count();
            ViewBag.TotalUser = db.Users.Count();
            ViewBag.TotalMachine = db.Machines.Count();
            ViewBag.TotalDevice = db.Devices.Count();
            return View();
        }

        public ActionResult TotalStaff()
        {
            //var staffs = db.AspNetUsers.OrderBy(a=>a.UserName);
            //return View(staffs.ToList());
            return View();
        }

        public JsonResult getTotalUserAdmin()
        {
            var userrole = dtx.AspNetUserRoles.DefaultIfEmpty().ToList();
            var netuser = db.AspNetUsers.Where(u=>u.UserName != "SAdmin").ToList();
            var role = db.AspNetRoles.DefaultIfEmpty().ToList();
            var user = db.Users.Where(u => u.EmployeeID != null && u.DepartmentID == 15).ToList();

            var useradmin = (from d in netuser
                             join i in user on d.UserName equals i.EmployeeID
                             into tempPets
                             from i in tempPets.DefaultIfEmpty()

                             join p in userrole on d.Id equals p.UserId
                             into tempPets2
                             from p in tempPets2.DefaultIfEmpty()

                             select new ViewModelsUser
                             {
                                 Id = d.Id,
                                 FirstName = i == null ? "No User Data" : i.FirstName,
                                 LastName = i == null ? "No User Data" : i.LastName,
                                 EmployeeID = d == null ? "No User Data" : d.UserName,
                                 Role = p.RoleId
                             }).ToList();
            return Json(new { data = useradmin }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewAllDevice()
        {
            #region countstock
            var totalDevice = db.Devices.Count();
            var totalmachine = db.Machines.Count();
            var totaluser = db.Users.Count();
            var totalInStock = db.Devices.Where(x => x.StatusID == 3).Count();
            var totalInUse = db.Devices.Where(x => x.StatusID == 1).Count();
            var totalInRepair = db.Devices.Where(x => x.StatusID == 2).Count();
            var totalSpare = db.Devices.Where(x => x.StatusID == 5).Count();
            var totalSale = db.Devices.Where(x => x.StatusID == 4).Count();
            var totalSentSale = db.Devices.Where(x => x.StatusID == 7).Count();
            var totalSentRepair = db.Devices.Where(x => x.StatusID == 6).Count();
            var totalWaitSentRepair = db.Devices.Where(x => x.StatusID == 8).Count();
            var totalWaitSentSale = db.Devices.Where(x => x.StatusID == 9).Count();


            #endregion

            #region countMachineCENTER
            var totalmachinehr = db.Machines.Where(m => m.DepartmentID == 14 && m.PlantID == 3).Count();
            var totalmachineit = db.Machines.Where(m => m.DepartmentID == 15 && m.PlantID == 3).Count();
            var totalmachinetechnology = db.Machines.Where(m => m.DepartmentID == 23 && m.PlantID == 3).Count();
            var totalmachineqa = db.Machines.Where(m => m.DepartmentID == 21 && m.PlantID == 3).Count();
            var totalmachineplantcontrol = db.Machines.Where(m => m.DepartmentID == 20 && m.PlantID == 3).Count();
            var totalmachineproduction = db.Machines.Where(m => m.DepartmentID == 26 && m.PlantID == 3).Count();
            var totalmachinese = db.Machines.Where(m => m.DepartmentID == 24 && m.PlantID == 3).Count();
            var totalmachineboi = db.Machines.Where(m => m.DepartmentID == 25 && m.PlantID == 3).Count();
            var totalmachinefa = db.Machines.Where(m => m.DepartmentID == 27 && m.PlantID == 3).Count();
            var totalmachinepurchasing = db.Machines.Where(m => m.DepartmentID == 28 && m.PlantID == 3).Count();
            var totalmachinetpmtps = db.Machines.Where(m => m.DepartmentID == 29 && m.PlantID == 3).Count();



            #endregion

            #region countUserCENTER
            var totaluserhr = db.Users.Where(m => m.DepartmentID == 14 && m.PlantID == 3).Count();
            var totaluserit = db.Users.Where(m => m.DepartmentID == 15 && m.PlantID == 3).Count();
            var totalusertechnology = db.Users.Where(m => m.DepartmentID == 23 && m.PlantID == 3).Count();
            var totaluserqa = db.Users.Where(m => m.DepartmentID == 21 && m.PlantID == 3).Count();
            var totaluserplantcontrol = db.Users.Where(m => m.DepartmentID == 20 && m.PlantID == 3).Count();
            var totaluserproduction = db.Users.Where(m => m.DepartmentID == 26 && m.PlantID == 3).Count();
            var totaluserse = db.Users.Where(m => m.DepartmentID == 24 && m.PlantID == 3).Count();
            var totaluserboi = db.Users.Where(m => m.DepartmentID == 25 && m.PlantID == 3).Count();
            var totaluserfa = db.Users.Where(m => m.DepartmentID == 27 && m.PlantID == 3).Count();
            var totaluserpurchasing = db.Users.Where(m => m.DepartmentID == 28 && m.PlantID == 3).Count();
            var totalusertpmtps = db.Users.Where(m => m.DepartmentID == 29 && m.PlantID == 3).Count();



            #endregion

            #region countMachineTBS
            var totalmachinebuildingTBS = db.Machines.Where(m => m.DepartmentID == 9 && m.PlantID == 1).Count();
            var totalmachineCuringTBS = db.Machines.Where(m => m.DepartmentID == 10 && m.PlantID == 1).Count();
            var totalmachineFinishingTBS = db.Machines.Where(m => m.DepartmentID == 13 && m.PlantID == 1).Count();
            var totalmachineElectricalTBS = db.Machines.Where(m => m.DepartmentID == 12 && m.PlantID == 1).Count();
            var totalmachineMixingTBS = db.Machines.Where(m => m.DepartmentID == 19 && m.PlantID == 1).Count();
            var totalmachineTechnologyTBS = db.Machines.Where(m => m.DepartmentID == 23 && m.PlantID == 1).Count();
            var totalmachineRawMaterialTBS = db.Machines.Where(m => m.DepartmentID == 22 && m.PlantID == 1).Count();
            var totalmachineDistributeTBS = db.Machines.Where(m => m.DepartmentID == 11 && m.PlantID == 1).Count();
            var totalmachineMaintenanceTBS = db.Machines.Where(m => m.DepartmentID == 17 && m.PlantID == 1).Count();
            var totalmachineMatPrepTBS = db.Machines.Where(m => m.DepartmentID == 18 && m.PlantID == 1).Count();
            var totalmachineQATBS = db.Machines.Where(m => m.DepartmentID == 21 && m.PlantID == 1).Count();
            var totalmachinePlantControlTBS = db.Machines.Where(m => m.DepartmentID == 20 && m.PlantID == 1).Count();
            var totalmachineProductionTBS = db.Machines.Where(m => m.DepartmentID == 26 && m.PlantID == 1).Count();
            var totalmachineITTBS = db.Machines.Where(m => m.DepartmentID == 15 && m.PlantID == 1).Count();
            var totalmachineHRTBS = db.Machines.Where(m => m.DepartmentID == 14 && m.PlantID == 1).Count();
            #endregion

            #region countUserTBS
            var totaluserbuildingTBS = db.Users.Where(m => m.DepartmentID == 9 && m.PlantID == 1).Count();
            var totaluserCuringTBS = db.Users.Where(m => m.DepartmentID == 10 && m.PlantID == 1).Count();
            var totaluserFinishingTBS = db.Users.Where(m => m.DepartmentID == 13 && m.PlantID == 1).Count();
            var totaluserElectricalTBS = db.Users.Where(m => m.DepartmentID == 12 && m.PlantID == 1).Count();
            var totaluserMixingTBS = db.Users.Where(m => m.DepartmentID == 19 && m.PlantID == 1).Count();
            var totaluserTechnologyTBS = db.Users.Where(m => m.DepartmentID == 23 && m.PlantID == 1).Count();
            var totaluserRawMaterialTBS = db.Users.Where(m => m.DepartmentID == 22 && m.PlantID == 1).Count();
            var totaluserDistributeTBS = db.Users.Where(m => m.DepartmentID == 11 && m.PlantID == 1).Count();
            var totaluserMaintenanceTBS = db.Users.Where(m => m.DepartmentID == 17 && m.PlantID == 1).Count();
            var totaluserMatPrepTBS = db.Users.Where(m => m.DepartmentID == 18 && m.PlantID == 1).Count();
            var totaluserQATBS = db.Users.Where(m => m.DepartmentID == 21 && m.PlantID == 1).Count();
            var totaluserPlantControlTBS = db.Users.Where(m => m.DepartmentID == 20 && m.PlantID == 1).Count();
            var totaluserProductionTBS = db.Users.Where(m => m.DepartmentID == 26 && m.PlantID == 1).Count();
            var totaluserITTBS = db.Users.Where(m => m.DepartmentID == 15 && m.PlantID == 1).Count();
            var totaluserHRTBS = db.Users.Where(m => m.DepartmentID == 14 && m.PlantID == 1).Count();
            #endregion

            #region countDeviceCenter
            var totalDeviceBuildingCenter = db.Devices.Where(m => m.DepartmentID == 9 && m.PlantID == 3).Count();
            var totalDeviceCuringCenter = db.Devices.Where(m => m.DepartmentID == 10 && m.PlantID == 3).Count();
            var totalDeviceDistributionCenter = db.Devices.Where(m => m.DepartmentID == 11 && m.PlantID == 3).Count();
            var totalDeviceElectricalCenter = db.Devices.Where(m => m.DepartmentID == 12 && m.PlantID == 3).Count();
            var totalDeviceFinishingCenter = db.Devices.Where(m => m.DepartmentID == 13 && m.PlantID == 3).Count();
            var totalDeviceHRCenter = db.Devices.Where(m => m.DepartmentID == 14 && m.PlantID == 3).Count();
            var totalDeviceITCenter = db.Devices.Where(m => m.DepartmentID == 15 && m.PlantID == 3).Count();
            var totalDeviceMaintenanceCenter = db.Devices.Where(m => m.DepartmentID == 17 && m.PlantID == 3).Count();
            var totalDeviceMatPrepCenter = db.Devices.Where(m => m.DepartmentID == 18 && m.PlantID == 3).Count();
            var totalDeviceMixingCenter = db.Devices.Where(m => m.DepartmentID == 19 && m.PlantID == 3).Count();
            var totalDevicePlantControlCenter = db.Devices.Where(m => m.DepartmentID == 20 && m.PlantID == 3).Count();
            var totalDeviceProductionCenter = db.Devices.Where(m => m.DepartmentID == 28 && m.PlantID == 3).Count();
            var totalDeviceQACenter = db.Devices.Where(m => m.DepartmentID == 21 && m.PlantID == 3).Count();
            var totalDeviceRawMatCenter = db.Devices.Where(m => m.DepartmentID == 22 && m.PlantID == 3).Count();
            var totalDeviceTechnologyCenter = db.Devices.Where(m => m.DepartmentID == 23 && m.PlantID == 3).Count();
            var totalDeviceSECenter = db.Devices.Where(m => m.DepartmentID == 24 && m.PlantID == 3).Count();
            var totalDeviceBOICenter = db.Devices.Where(m => m.DepartmentID == 25 && m.PlantID == 3).Count();
            var totalDeviceFACenter = db.Devices.Where(m => m.DepartmentID == 27 && m.PlantID == 3).Count();
            var totalDevicePurchasingCenter = db.Devices.Where(m => m.DepartmentID == 28 && m.PlantID == 3).Count();
            var totalDeviceTpmTpsCenter = db.Devices.Where(m => m.DepartmentID == 29 && m.PlantID == 3).Count();



            #endregion

            #region countMachinePCLT
            var totalmachinebuildingPCLT = db.Machines.Where(m => m.DepartmentID == 9 && m.PlantID == 2).Count();
            var totalmachineCuringPCLT = db.Machines.Where(m => m.DepartmentID == 10 && m.PlantID == 2).Count();
            var totalmachineFinishingPCLT = db.Machines.Where(m => m.DepartmentID == 13 && m.PlantID == 2).Count();
            var totalmachineElectricalPCLT = db.Machines.Where(m => m.DepartmentID == 12 && m.PlantID == 2).Count();
            var totalmachineMixingPCLT = db.Machines.Where(m => m.DepartmentID == 19 && m.PlantID == 2).Count();
            var totalmachineTechnologyPCLT = db.Machines.Where(m => m.DepartmentID == 23 && m.PlantID == 2).Count();
            var totalmachineRawMaterialPCLT = db.Machines.Where(m => m.DepartmentID == 22 && m.PlantID == 2).Count();
            var totalmachineDistributePCLT = db.Machines.Where(m => m.DepartmentID == 11 && m.PlantID == 2).Count();
            var totalmachineMaintenancePCLT = db.Machines.Where(m => m.DepartmentID == 17 && m.PlantID == 2).Count();
            var totalmachineMatPrepPCLT = db.Machines.Where(m => m.DepartmentID == 18 && m.PlantID == 2).Count();
            var totalmachineQAPCLT = db.Machines.Where(m => m.DepartmentID == 21 && m.PlantID == 2).Count();
            var totalmachinePlantControlPCLT = db.Machines.Where(m => m.DepartmentID == 20 && m.PlantID == 2).Count();
            var totalmachineProductionPCLT = db.Machines.Where(m => m.DepartmentID == 26 && m.PlantID == 2).Count();
            var totalmachineITPCLT = db.Machines.Where(m => m.DepartmentID == 15 && m.PlantID == 2).Count();
            var totalmachineHRPCLT = db.Machines.Where(m => m.DepartmentID == 14 && m.PlantID == 2).Count();
            #endregion

            #region countUserPCLT
            var totaluserbuildingPCLT = db.Users.Where(m => m.DepartmentID == 9 && m.PlantID == 2).Count();
            var totaluserCuringPCLT = db.Users.Where(m => m.DepartmentID == 10 && m.PlantID == 2).Count();
            var totaluserFinishingPCLT = db.Users.Where(m => m.DepartmentID == 13 && m.PlantID == 2).Count();
            var totaluserElectricalPCLT = db.Users.Where(m => m.DepartmentID == 12 && m.PlantID == 2).Count();
            var totaluserMixingPCLT = db.Users.Where(m => m.DepartmentID == 19 && m.PlantID == 2).Count();
            var totaluserTechnologyPCLT = db.Users.Where(m => m.DepartmentID == 23 && m.PlantID == 2).Count();
            var totaluserRawMaterialPCLT = db.Users.Where(m => m.DepartmentID == 22 && m.PlantID == 2).Count();
            var totaluserDistributePCLT = db.Users.Where(m => m.DepartmentID == 11 && m.PlantID == 2).Count();
            var totaluserMaintenancePCLT = db.Users.Where(m => m.DepartmentID == 17 && m.PlantID == 2).Count();
            var totaluserMatPrepPCLT = db.Users.Where(m => m.DepartmentID == 18 && m.PlantID == 2).Count();
            var totaluserQAPCLT = db.Users.Where(m => m.DepartmentID == 21 && m.PlantID == 2).Count();
            var totaluserPlantControlPCLT = db.Users.Where(m => m.DepartmentID == 20 && m.PlantID == 2).Count();
            var totaluserProductionPCLT = db.Users.Where(m => m.DepartmentID == 26 && m.PlantID == 2).Count();
            var totaluserITPCLT = db.Users.Where(m => m.DepartmentID == 15 && m.PlantID == 2).Count();
            var totaluserHRPCLT = db.Users.Where(m => m.DepartmentID == 14 && m.PlantID == 2).Count();
            #endregion

            #region countDeviceTBS
            var totalDeviceBuildingTBS = db.Devices.Where(m => m.DepartmentID == 9 && m.PlantID == 1).Count();
            var totalDeviceCuringTBS = db.Devices.Where(m => m.DepartmentID == 10 && m.PlantID == 1).Count();
            var totalDeviceDistributionTBS = db.Devices.Where(m => m.DepartmentID == 11 && m.PlantID == 1).Count();
            var totalDeviceElectricalTBS = db.Devices.Where(m => m.DepartmentID == 12 && m.PlantID == 1).Count();
            var totalDeviceFinishingTBS = db.Devices.Where(m => m.DepartmentID == 13 && m.PlantID == 1).Count();
            var totalDeviceHRTBS = db.Devices.Where(m => m.DepartmentID == 14 && m.PlantID == 1).Count();
            var totalDeviceITTBS = db.Devices.Where(m => m.DepartmentID == 15 && m.PlantID == 1).Count();
            var totalDeviceMaintenanceTBS = db.Devices.Where(m => m.DepartmentID == 17 && m.PlantID == 1).Count();
            var totalDeviceMatPrepTBS = db.Devices.Where(m => m.DepartmentID == 18 && m.PlantID == 1).Count();
            var totalDeviceMixingTBS = db.Devices.Where(m => m.DepartmentID == 19 && m.PlantID == 1).Count();
            var totalDevicePlantControlTBS = db.Devices.Where(m => m.DepartmentID == 20 && m.PlantID == 1).Count();
            var totalDeviceProductionTBS = db.Devices.Where(m => m.DepartmentID == 26 && m.PlantID == 1).Count();
            var totalDeviceQATBS = db.Devices.Where(m => m.DepartmentID == 21 && m.PlantID == 1).Count();
            var totalDeviceRawMatTBS = db.Devices.Where(m => m.DepartmentID == 22 && m.PlantID == 1).Count();
            var totalDeviceTechnologyTBS = db.Devices.Where(m => m.DepartmentID == 23 && m.PlantID == 1).Count();
            #endregion

            #region countDevicePCLT
            var totalDeviceBuildingPCLT = db.Devices.Where(m => m.DepartmentID == 9 && m.PlantID == 2).Count();
            var totalDeviceCuringPCLT = db.Devices.Where(m => m.DepartmentID == 10 && m.PlantID == 2).Count();
            var totalDeviceDistributionPCLT = db.Devices.Where(m => m.DepartmentID == 11 && m.PlantID == 2).Count();
            var totalDeviceElectricalPCLT = db.Devices.Where(m => m.DepartmentID == 12 && m.PlantID == 2).Count();
            var totalDeviceFinishingPCLT = db.Devices.Where(m => m.DepartmentID == 13 && m.PlantID == 2).Count();
            var totalDeviceHRPCLT = db.Devices.Where(m => m.DepartmentID == 14 && m.PlantID == 2).Count();
            var totalDeviceITPCLT = db.Devices.Where(m => m.DepartmentID == 15 && m.PlantID == 2).Count();
            var totalDeviceMaintenancePCLT = db.Devices.Where(m => m.DepartmentID == 17 && m.PlantID == 2).Count();
            var totalDeviceMatPrepPCLT = db.Devices.Where(m => m.DepartmentID == 18 && m.PlantID == 2).Count();
            var totalDeviceMixingPCLT = db.Devices.Where(m => m.DepartmentID == 19 && m.PlantID == 2).Count();
            var totalDevicePlantControlPCLT = db.Devices.Where(m => m.DepartmentID == 20 && m.PlantID == 2).Count();
            var totalDeviceProductionPCLT = db.Devices.Where(m => m.DepartmentID == 26 && m.PlantID == 2).Count();
            var totalDeviceQAPCLT = db.Devices.Where(m => m.DepartmentID == 21 && m.PlantID == 2).Count();
            var totalDeviceRawMatPCLT = db.Devices.Where(m => m.DepartmentID == 22 && m.PlantID == 2).Count();
            var totalDeviceTechnologyPCLT = db.Devices.Where(m => m.DepartmentID == 23 && m.PlantID == 2).Count();
            #endregion

            var summachinecenter = db.Machines.Where(m => m.PlantID == 3).Count();
            var sumusercenter = db.Users.Where(u => u.PlantID == 3).Count();
            var sumdevicecenter = db.Devices.Where(d => d.PlantID == 3).Count();

            var summachinetbs = db.Machines.Where(m => m.PlantID == 1).Count();
            var sumusertbs = db.Users.Where(u => u.PlantID == 1).Count();
            var sumdevicetbs = db.Devices.Where(d => d.PlantID == 1).Count();

            var summachinepclt = db.Machines.Where(m => m.PlantID == 2).Count();
            var sumuserpclt = db.Users.Where(u => u.PlantID == 2).Count();
            var sumdevicepclt = db.Devices.Where(d => d.PlantID == 2).Count();

            #region setObj
            DashboardViewModels dashboard = new DashboardViewModels();

            dashboard.SumMachineCenter = summachinecenter;
            dashboard.SumUserCenter = sumusercenter;
            dashboard.SumDeviceCenter = sumdevicecenter;

            dashboard.SumMachineTBS = summachinetbs;
            dashboard.SumUserTBS = sumusertbs;
            dashboard.SumDeviceTBS = sumdevicetbs;

            dashboard.SumMachinePCLT = summachinepclt;
            dashboard.SumUserPCLT = sumuserpclt;
            dashboard.SumDevicePCLT = sumdevicepclt;

            dashboard.TotalDevice = totalDevice;
            dashboard.TotalMachine = totalmachine;
            dashboard.TotalUser = totaluser;
            dashboard.TotalInStock = totalInStock;
            dashboard.TotalInUse = totalInUse;
            dashboard.TotalInRepair = totalInRepair;
            dashboard.TotalSpare = totalSpare;
            dashboard.TotalSale = totalSale;
            dashboard.TotalSentSale = totalSentSale;
            dashboard.TotalSentRepair = totalSentRepair;
            dashboard.TotalWaitSentRepair = totalWaitSentRepair;
            dashboard.TotalWaitSentSale = totalWaitSentSale;

            dashboard.TotalMachineHR = totalmachinehr;
            dashboard.TotalMachineIT = totalmachineit;
            dashboard.TotalMachineTechnology = totalmachinetechnology;
            dashboard.TotalMachineQA = totalmachineqa;
            dashboard.TotalMachinePlantControl = totalmachineplantcontrol;
            dashboard.TotalMachineProduction = totalmachineproduction;
            dashboard.TotalMachineSE = totalmachinese;
            dashboard.TotalMachineBOI = totalmachineboi;
            dashboard.TotalMachineFA = totalmachinefa;
            dashboard.TotalMachinePurchasing = totalmachinepurchasing;
            dashboard.TotalMachineTPMTPS = totalmachinetpmtps;

            dashboard.TotalUserHR = totaluserhr;
            dashboard.TotalUserIT = totaluserit;
            dashboard.TotalUserTechnology = totalusertechnology;
            dashboard.TotalUserQA = totaluserqa;
            dashboard.TotalUserPlantControl = totaluserplantcontrol;
            dashboard.TotalUserProduction = totaluserproduction;
            dashboard.TotalUserSE = totaluserse;
            dashboard.TotalUserBOI = totaluserboi;
            dashboard.TotalUserFA = totaluserfa;
            dashboard.TotalUserPurchasing = totaluserpurchasing;
            dashboard.TotalUserTPMTPS = totalusertpmtps;

            dashboard.TotalMachineBuildingTBS = totalmachinebuildingTBS;
            dashboard.TotalMachineCuringTBS = totalmachineCuringTBS;
            dashboard.TotalMachineFinishingTBS = totalmachineFinishingTBS;
            dashboard.TotalMachineElectricalTBS = totalmachineElectricalTBS;
            dashboard.TotalMachineMixingTBS = totalmachineMixingTBS;
            dashboard.TotalMachineTechnologyTBS = totalmachineTechnologyTBS;
            dashboard.TotalMachineRawMaterialTBS = totalmachineRawMaterialTBS;
            dashboard.TotalMachineDistributeTBS = totalmachineDistributeTBS;
            dashboard.TotalMachineMaintenanceTBS = totalmachineMaintenanceTBS;
            dashboard.TotalMachineMatPrepareTBS = totalmachineMatPrepTBS;
            dashboard.TotalMachineQATBS = totalmachineQATBS;
            dashboard.TotalMachinePlantControlTBS = totalmachinePlantControlTBS;
            dashboard.TotalMachineProductionTBS = totalmachineProductionTBS;
            dashboard.TotalMachineITTBS = totalmachineITTBS;
            dashboard.TotalMachineHRTBS = totalmachineHRTBS;

            dashboard.TotalUserBuildingTBS = totaluserbuildingTBS;
            dashboard.TotalUserCuringTBS = totaluserCuringTBS;
            dashboard.TotalUserFinishingTBS = totaluserFinishingTBS;
            dashboard.TotalUserElectricalTBS = totaluserElectricalTBS;
            dashboard.TotalUserMixingTBS = totaluserMixingTBS;
            dashboard.TotalUserTechnologyTBS = totaluserTechnologyTBS;
            dashboard.TotalUserRawMaterialTBS = totaluserRawMaterialTBS;
            dashboard.TotalUserDistributeTBS = totaluserDistributeTBS;
            dashboard.TotalUserMaintenanceTBS = totaluserMaintenanceTBS;
            dashboard.TotalUserMatPrepareTBS = totaluserMatPrepTBS;
            dashboard.TotalUserQATBS = totaluserQATBS;
            dashboard.TotalUserPlantControlTBS = totaluserPlantControlTBS;
            dashboard.TotalUserProductionTBS = totaluserProductionTBS;
            dashboard.TotalUserITTBS = totaluserITTBS;
            dashboard.TotalUserHRTBS = totaluserHRTBS;

            dashboard.TotalMachineBuildingPCLT = totalmachinebuildingPCLT;
            dashboard.TotalMachineCuringPCLT = totalmachineCuringPCLT;
            dashboard.TotalMachineFinishingPCLT = totalmachineFinishingPCLT;
            dashboard.TotalMachineElectricalPCLT = totalmachineElectricalPCLT;
            dashboard.TotalMachineMixingPCLT = totalmachineMixingPCLT;
            dashboard.TotalMachineTechnologyPCLT = totalmachineTechnologyPCLT;
            dashboard.TotalMachineRawMaterialPCLT = totalmachineRawMaterialPCLT;
            dashboard.TotalMachineDistributePCLT = totalmachineDistributePCLT;
            dashboard.TotalMachineMaintenancePCLT = totalmachineMaintenancePCLT;
            dashboard.TotalMachineMatPreparePCLT = totalmachineMatPrepPCLT;
            dashboard.TotalMachineQAPCLT = totalmachineQAPCLT;
            dashboard.TotalMachinePlantControlPCLT = totalmachinePlantControlPCLT;
            dashboard.TotalMachineProductionPCLT = totalmachineProductionPCLT;
            dashboard.TotalMachineHRPCLT = totalmachineHRPCLT;
            dashboard.TotalMachineITPCLT = totalmachineITPCLT;

            dashboard.TotalUserBuildingPCLT = totaluserbuildingPCLT;
            dashboard.TotalUserCuringPCLT = totaluserCuringPCLT;
            dashboard.TotalUserFinishingPCLT = totaluserFinishingPCLT;
            dashboard.TotalUserElectricalPCLT = totaluserElectricalPCLT;
            dashboard.TotalUserMixingPCLT = totaluserMixingPCLT;
            dashboard.TotalUserTechnologyPCLT = totaluserTechnologyPCLT;
            dashboard.TotalUserRawMaterialPCLT = totaluserRawMaterialPCLT;
            dashboard.TotalUserDistributePCLT = totaluserDistributePCLT;
            dashboard.TotalUserMaintenancePCLT = totaluserMaintenancePCLT;
            dashboard.TotalUserMatPreparePCLT = totaluserMatPrepPCLT;
            dashboard.TotalUserQAPCLT = totaluserQAPCLT;
            dashboard.TotalUserPlantControlPCLT = totaluserPlantControlPCLT;
            dashboard.TotalMachineProductionPCLT = totaluserProductionPCLT;
            dashboard.TotalUserHRPCLT = totaluserHRPCLT;
            dashboard.TotalUserITPCLT = totaluserITPCLT;

            dashboard.TotalDeviceBuildingCenter = totalDeviceBuildingCenter;
            dashboard.TotalDeviceCuringCenter = totalDeviceCuringCenter;
            dashboard.TotalDeviceDistributeCenter = totalDeviceDistributionCenter;
            dashboard.TotalDeviceElectricalCenter = totalDeviceElectricalCenter;
            dashboard.TotalDeviceFinishingCenter = totalDeviceFinishingCenter;
            dashboard.TotalDeviceHRCenter = totalDeviceHRCenter;
            dashboard.TotalDeviceITCenter = totalDeviceITCenter;
            dashboard.TotalDeviceMaintenanceCenter = totalDeviceMaintenanceCenter;
            dashboard.TotalDeviceMatPrepareCenter = totalDeviceMatPrepCenter;
            dashboard.TotalDeviceMixingCenter = totalDeviceMixingCenter;
            dashboard.TotalDevicePlantControlCenter = totalDevicePlantControlCenter;
            dashboard.TotalDeviceProductionCenter = totalDeviceProductionCenter;
            dashboard.TotalDeviceQACenter = totalDeviceQACenter;
            dashboard.TotalDeviceRawMaterialCenter = totalDeviceRawMatCenter;
            dashboard.TotalDeviceTechnologyCenter = totalDeviceTechnologyCenter;
            dashboard.TotalDeviceSECenter = totalDeviceSECenter;
            dashboard.TotalDeviceBOICenter = totalDeviceBOICenter;
            dashboard.TotalDeviceFACenter = totalDeviceFACenter;
            dashboard.TotalDevicePurchasingCenter = totalDevicePurchasingCenter;
            dashboard.TotalDeviceTPMTPSCenter = totalDeviceTpmTpsCenter;



            dashboard.TotalDeviceBuildingTBS = totalDeviceBuildingTBS;
            dashboard.TotalDeviceCuringTBS = totalDeviceCuringTBS;
            dashboard.TotalDeviceDistributeTBS = totalDeviceDistributionTBS;
            dashboard.TotalDeviceElectricalTBS = totalDeviceElectricalTBS;
            dashboard.TotalDeviceFinishingTBS = totalDeviceFinishingTBS;
            dashboard.TotalDeviceHRTBS = totalDeviceHRTBS;
            dashboard.TotalDeviceITTBS = totalDeviceITTBS;
            dashboard.TotalDeviceMaintenanceTBS = totalDeviceMaintenanceTBS;
            dashboard.TotalDeviceMatPrepareTBS = totalDeviceMatPrepTBS;
            dashboard.TotalDeviceMixingTBS = totalDeviceMixingTBS;
            dashboard.TotalDevicePlantControlTBS = totalDevicePlantControlTBS;
            dashboard.TotalDeviceProductionTBS = totalDeviceProductionTBS;
            dashboard.TotalDeviceQATBS = totalDeviceQATBS;
            dashboard.TotalDeviceRawMaterialTBS = totalDeviceRawMatTBS;
            dashboard.TotalDeviceTechnologyTBS = totalDeviceTechnologyTBS;

            dashboard.TotalDeviceBuildingPCLT = totalDeviceBuildingPCLT;
            dashboard.TotalDeviceCuringPCLT = totalDeviceCuringPCLT;
            dashboard.TotalDeviceDistributePCLT = totalDeviceDistributionPCLT;
            dashboard.TotalDeviceElectricalPCLT = totalDeviceElectricalPCLT;
            dashboard.TotalDeviceFinishingPCLT = totalDeviceFinishingPCLT;
            dashboard.TotalDeviceHRPCLT = totalDeviceHRPCLT;
            dashboard.TotalDeviceITPCLT = totalDeviceITPCLT;
            dashboard.TotalDeviceMaintenancePCLT = totalDeviceMaintenancePCLT;
            dashboard.TotalDeviceMatPreparePCLT = totalDeviceMatPrepPCLT;
            dashboard.TotalDeviceMixingPCLT = totalDeviceMixingPCLT;
            dashboard.TotalDevicePlantControlPCLT = totalDevicePlantControlPCLT;
            dashboard.TotalDeviceProductionPCLT = totalDeviceProductionPCLT;
            dashboard.TotalDeviceQAPCLT = totalDeviceQAPCLT;
            dashboard.TotalDeviceRawMaterialPCLT = totalDeviceRawMatPCLT;
            dashboard.TotalDeviceTechnologyPCLT = totalDeviceTechnologyPCLT;
            #endregion

            return View(dashboard);
        }

        public ActionResult Location(int? ID, int? plantID)
        {
            var machines = db.Machines.Where(m => m.DepartmentID == ID && m.PlantID == plantID).Include(m => m.Department).OrderBy(m => m.LocationID).Include(m => m.Location).Include(m => m.Phase).Include(m => m.Plant).OrderBy(m=>m.Phase.PhaseName);
            ViewBag.Department = db.Departments.Where(d => d.DepartmentID == ID).Select(d => d.DepartmentName).DefaultIfEmpty().First();
            return View(machines.ToList());
        }

        public ActionResult LocationUser(int? ID, int? plantID)
        {
            var users = db.Users.Where(u => u.DepartmentID == ID && u.PlantID == plantID).Include(u => u.Location).Include(u => u.Plant);
            ViewBag.Department = db.Departments.Where(d => d.DepartmentID == ID).Select(d => d.DepartmentName).DefaultIfEmpty().First();
            return View(users.ToList());
        }

        public ActionResult Device(int? ID, int? plantID)
        {
            var devices = db.Devices.Where(m => m.DepartmentID == ID && m.PlantID == plantID).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            ViewBag.Department = db.Departments.Where(d => d.DepartmentID == ID).Select(d => d.DepartmentName).DefaultIfEmpty().First();
            return View(devices.ToList());
        }

        public ActionResult Machine(int? ID)
        {
            var devices = db.Devices.Where(m => m.MachineID == ID && m.StatusID == 1).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            ViewBag.MachineName = db.Machines.Where(m => m.MachineID == ID).Select(m => m.MachineName).DefaultIfEmpty().First();
            return View(devices.ToList());
        }

        public ActionResult User(int? id)
        {
            var devices = db.Devices.Where(m => m.UserID == id && m.StatusID == 1 ).Include(m => m.Machine).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            ViewBag.UserName = db.Users.Where(u => u.UserID == id).Select(u => u.FullName).DefaultIfEmpty().First();
            return View(devices.ToList());
        }

        public ActionResult IPScannerByPlant()
        {
            var devices = from device in db.Devices where device.IPAddress != null && device.StatusID == 1 && device.DeviceTypeID == 58  group device by device.PlantID into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        public ActionResult IPScannerByDepartment(int?id)
        {
            var devices = from device in db.Devices where device.IPAddress != null && device.StatusID == 1 && device.PlantID == id && device.DeviceTypeID == 58 group device by device.DepartmentID into dv let m = dv.FirstOrDefault() select m;
            ViewBag.IPBuildingLT = 41;
            ViewBag.IPCuringLT = 21;
            ViewBag.IPRackingWHLT = 21;
            ViewBag.IPShippingWHLT = 51;
            ViewBag.IPQATLT = 11;
            return View(devices);
        }

        public ActionResult ListIPScanner()
        {
            //var devices = db.Devices.Where(d => d.StatusID == 1 && d.IPAddress != null && d.DeviceType.DeviceTypeID == 58 ).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
           // return View(devices.ToList());
            return View();
        }

        public ActionResult getListIPScanner()
        {
            var devices = (from d in db.Devices
                           join i in db.Departments on d.DepartmentID equals i.DepartmentID
                           into tempPets
                           from i in tempPets.DefaultIfEmpty()

                           join p in db.Plants on d.PlantID equals p.PlantID
                           into tempPets2
                           from p in tempPets2.DefaultIfEmpty()

                           join l in db.Locations on d.LocationID equals l.LocationID
                           into tempPets3
                           from l in tempPets3.DefaultIfEmpty()

                           join m in db.Machines on d.MachineID equals m.MachineID
                           into tempPets4
                           from m in tempPets4.DefaultIfEmpty()

                           join u in db.Users on d.UserID equals u.UserID
                           into tempPets5
                           from u in tempPets5.DefaultIfEmpty()

                           where d.StatusID == 1 && d.IPAddress != null && d.DeviceType.DeviceTypeID == 58

                           select new DeviceViewModels
                           {
                               SerialNumber = d.SerialNumber,
                               PlantName = p.PlantName,
                               DepartmentName = i.DepartmentName,
                               LocationName = l.LocationName,
                               LocationStockName = d.LocationStockName,
                               CreateBy = d.CreateBy,
                               UpdateBy = d.UpdateBy,
                               DateCreate = d.DateCreate,
                               DateUpdate = d.DateUpdate,
                               PhaseName = d.PhaseName,
                               BrandName = d.BrandName,
                               Type = d.Type,
                               ModelName = d.ModelName,
                               IPAddress = d.IPAddress,
                               StatusName = d.StatusName,
                               MachineName = m.MachineName,
                               UserName = u.FullName
                           }).ToList();
            return Json(new { data = devices }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllLogFile()
        {
            return View();
        }

        public JsonResult getAllLogFile()
        {
            using (var ctx = new ITStockEntities1())
            {
                ctx.Configuration.ProxyCreationEnabled = false;
                var logfile = (from d in ctx.RecordDevices
                             select new LogFileViewModels
                             {
                                 ActionBy = d.EditBy,
                                 ActionDate = d.EditDate,
                                 DeviceType = d.Type,
                                 Model = d.Model,
                                 Brand = d.Brand,
                                 SerialNumber = d.SerialNumber,
                                 Plant = d.Plant,
                                 Department = d.Department,
                                 Location = d.Location,
                                 Phase = d.Phase,
                                 Status = d.Description,
                                 LocationStock = d.LocationStock,
                                 Cause = d.Cause,
                                 Machine = d.Machine,
                                 DeviceName = d.DeviceName,
                                 UserName = d.UserName
                             }).Union
                            (from d in ctx.RecordInstocks
                             select new LogFileViewModels
                             {
                                 ActionBy = d.InstockBy,
                                 ActionDate = d.DateInstock,
                                 DeviceType = d.DeviceType,
                                 Model = d.Model,
                                 Brand = d.Brand,
                                 SerialNumber = d.SerialNumber,
                                 Plant = d.Plant,
                                 Department = d.Department,
                                 Location = d.Location,
                                 Phase = d.Phase,
                                 Status = d.Status,
                                 LocationStock = d.LocationStock,
                                 Cause = null,
                                 Machine = d.Machine,
                                 DeviceName = d.DeviceName,
                                 UserName = d.UserName
                             }).Union
                             (from d in ctx.RecordReinstocks
                              select new LogFileViewModels
                              {
                                  ActionBy = d.RequestBy,
                                  ActionDate = d.DateRequest,
                                  DeviceType = d.DeviceType,
                                  Model = d.Model,
                                  Brand = d.Brand,
                                  SerialNumber = d.SerialNumber,
                                  Plant = d.Plant,
                                  Department = d.Department,
                                  Location = d.Location,
                                  Phase = d.Phase,
                                  Status = d.Status,
                                  LocationStock = d.LocationStock,
                                  Cause = null,
                                  Machine = d.Machine,
                                  DeviceName = d.DeviceName,
                                  UserName = d.UserName
                              }).Union
                              (from d in ctx.RecordSales
                               select new LogFileViewModels
                               {
                                   ActionBy = d.RequestBy,
                                   ActionDate = d.DateRequest,
                                   DeviceType = d.DeviceType,
                                   Model = d.Model,
                                   Brand = d.Brand,
                                   SerialNumber = d.SerialNumber,
                                   Plant = d.Plant,
                                   Department = d.Department,
                                   Location = d.Location,
                                   Phase = d.Phase,
                                   Status = d.Status,
                                   LocationStock = d.LocationStock,
                                   Cause = d.Cause,
                                   Machine = d.Machine,
                                   DeviceName = d.DeviceName,
                                   UserName = d.UserName
                               }).Union
                               (from d in ctx.RecordSpares
                                select new LogFileViewModels
                                {
                                    ActionBy = d.RequestBy,
                                    ActionDate = d.DateRequest,
                                    DeviceType = d.DeviceType,
                                    Model = d.Model,
                                    Brand = d.Brand,
                                    SerialNumber = d.SerialNumber,
                                    Plant = d.Plant,
                                    Department = d.Department,
                                    Location = d.Location,
                                    Phase = d.Phase,
                                    Status = d.Status,
                                    LocationStock = d.LocationStock,
                                    Cause = null,
                                    Machine = d.Machine,
                                    DeviceName = d.DeviceName,
                                    UserName = d.UserName
                                }).Union
                             (from d in ctx.RecordInRepairs
                             select new LogFileViewModels
                             {
                                ActionBy = d.RequestBy,
                                ActionDate = d.DateRequest,
                                DeviceType = d.DeviceType,
                                Model = d.Model,
                                Brand = d.Brand,
                                SerialNumber = d.SerialNumber,
                                Plant = d.Plant,
                                Department = d.Department,
                                Location = d.Location,
                                Phase = d.Phase,
                                Status = d.Status,
                                LocationStock = d.LocationStock,
                                Cause = d.Cause,
                                Machine = d.Machine,
                                DeviceName = d.DeviceName,
                                UserName = d.UserName
                             }).Union
                           (from d in ctx.RecordRequisitions
                            select new LogFileViewModels
                            {
                                ActionBy = d.RequestBy,
                                ActionDate = d.DateRequisition,
                                DeviceType = d.DeviceType,
                                Model = d.Model,
                                Brand = d.Brand,
                                SerialNumber = d.SerialNumber,
                                Plant = d.Plant,
                                Department = d.Department,
                                Location = d.Location,
                                Phase = d.Phase,
                                Status = d.Status,
                                LocationStock = d.LocationStock,
                                Cause = d.Cause,
                                Machine = d.Machine,
                                DeviceName = d.DeviceName,
                                UserName = d.UserName
                            }).ToList().OrderBy(d => d.ActionDate).Reverse().Take(4000);
                return Json(new { data = logfile.OrderByDescending(d => d.ActionDate) }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ContentResult getChartData()
        {
            List<ViewModelsChart> chart = new List<ViewModelsChart>();

            var result = db.Departments.ToList();
            foreach (Department departments in result)
            {
                ViewModelsChart vmChart = new ViewModelsChart();
                vmChart.Dapartment = departments.DepartmentName;
                vmChart.DeviceQuantity = db.Devices.Where(d => d.Department.DepartmentName == vmChart.Dapartment).Count();
                vmChart.UserQuantity = db.Users.Where(u => u.Department.DepartmentName == vmChart.Dapartment).Count();
                chart.Add(vmChart);
            }
            return Content(JsonConvert.SerializeObject(chart), "application/json");
        }

        public ContentResult getChartScannerData()
        {
            List<ViewModelsChartScanner> chart = new List<ViewModelsChartScanner>();

            string dateTime = "01/01/2016 00:00:00.00";
            DateTime std = Convert.ToDateTime(dateTime);
            DateTime crd = DateTime.Now;
            var totalmonth = ((crd.Year - std.Year) * 12) + crd.Month;

            var result = db.Models.Where(m=>m.DeviceTypeID == 58 && m.BrandID != 93).ToList();
            foreach (Model Models in result)
            {
                ViewModelsChartScanner vmChart = new ViewModelsChartScanner();
                vmChart.Model = Models.ModelName;
                vmChart.InStock = db.Devices.Where(d => d.ModelName == vmChart.Model && d.StatusID == 3).Count();
                vmChart.ReqQTY = db.RecordRequisitions.Where(r => r.Model == Models.ModelName).Count();
                vmChart.Minimum = vmChart.ReqQTY / totalmonth;
                chart.Add(vmChart);
            }
            return Content(JsonConvert.SerializeObject(chart), "application/json");
        }

        public ContentResult getChartPanelPCData()
        {
            List<ViewModelsChartScanner> chart = new List<ViewModelsChartScanner>();

            string dateTime = "01/01/2016 00:00:00.00";
            DateTime std = Convert.ToDateTime(dateTime);
            DateTime crd = DateTime.Now;
            var totalmonth = ((crd.Year - std.Year) * 12) + crd.Month;

            var result = db.Models.Where(m => m.DeviceTypeID == 50).ToList();
            foreach (Model Models in result)
            {
                ViewModelsChartScanner vmChart = new ViewModelsChartScanner();
                vmChart.Model = Models.ModelName;
                vmChart.InStock = db.Devices.Where(d => d.ModelName == vmChart.Model && d.StatusID == 3).Count();
                vmChart.ReqQTY = db.RecordRequisitions.Where(r => r.Model == Models.ModelName).Count();
                vmChart.Minimum = vmChart.ReqQTY / totalmonth;
                chart.Add(vmChart);
            }
            return Content(JsonConvert.SerializeObject(chart), "application/json");
        }

        public ContentResult getChartPrinterData()
        {
            List<ViewModelsChartScanner> chart = new List<ViewModelsChartScanner>();

            string dateTime = "01/01/2016 00:00:00.00";
            DateTime std = Convert.ToDateTime(dateTime);
            DateTime crd = DateTime.Now;
            var totalmonth = ((crd.Year - std.Year) * 12) + crd.Month;

            var result = db.Models.Where(m => m.DeviceTypeID == 56 && m.BrandID != 47 && m.BrandID != 53).ToList();
            foreach (Model Models in result)
            {
                ViewModelsChartScanner vmChart = new ViewModelsChartScanner();
                vmChart.Model = Models.ModelName;
                vmChart.InStock = db.Devices.Where(d => d.ModelName == vmChart.Model && d.StatusID == 3).Count();
                vmChart.ReqQTY = db.RecordRequisitions.Where(r => r.Model == Models.ModelName).Count();
                vmChart.Minimum = vmChart.ReqQTY / totalmonth;
                chart.Add(vmChart);
            }
            return Content(JsonConvert.SerializeObject(chart), "application/json");
        }

        public ContentResult getChartPresetPCData()
        {
            List<ViewModelsChartScanner> chart = new List<ViewModelsChartScanner>();

            string dateTime = "01/01/2016 00:00:00.00";
            DateTime std = Convert.ToDateTime(dateTime);
            DateTime crd = DateTime.Now;
            var totalmonth = ((crd.Year - std.Year) * 12) + crd.Month;

            var result = db.Models.Where(m => m.DeviceTypeID == 54 && m.BrandID == 42).ToList();
            foreach (Model Models in result)
            {
                ViewModelsChartScanner vmChart = new ViewModelsChartScanner();
                vmChart.Model = Models.ModelName;
                vmChart.InStock = db.Devices.Where(d => d.ModelName == vmChart.Model && d.StatusID == 3).Count();
                vmChart.ReqQTY = db.RecordRequisitions.Where(r => r.Model == Models.ModelName).Count();
                vmChart.Minimum = vmChart.ReqQTY / totalmonth;
                chart.Add(vmChart);
            }
            return Content(JsonConvert.SerializeObject(chart), "application/json");
        }

        public ActionResult GenerateSerialNumber()
        {
            return View();
        }

        public ActionResult inventoryChart()
        {
            ViewBag.ScannerRepairQty = db.Devices.Where(d => d.DeviceTypeID == 58 && d.StatusID == 2).Count();
            ViewBag.PanelPCRepairQty = db.Devices.Where(d => d.DeviceTypeID == 50 && d.StatusID == 2).Count();
            ViewBag.PrinterRepairQty = db.Devices.Where(d => d.DeviceTypeID == 56 && d.StatusID == 2).Count();
            ViewBag.PresetPCRepairQty = db.Devices.Where(d => d.DeviceTypeID == 54 && d.StatusID == 2).Count();
            return View();
        }

        public ActionResult listTotalDevice()
        {
            return View();
        }

        public ActionResult listAllDevice()
        {
            var devices = db.Devices.Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            return View(devices.ToList());
        }

        public JsonResult GetDevice(string profissao)
        {
            using (var db = new ITStockEntities1())
            {
                db.Configuration.ProxyCreationEnabled = false;
                return Json(db.Devices
                .Where(x =>
                    x.Department.DepartmentName.Equals(profissao, StringComparison.InvariantCultureIgnoreCase)
                    ||
                    profissao.Equals("TODOS", StringComparison.InvariantCultureIgnoreCase)
                )
                .Select(x => new DeviceViewModels
                {
                    SerialNumber = x.SerialNumber,
                    IPAddress = x.IPAddress,
                    Type = x.Type,
                    ModelName = x.ModelName,
                    BrandName = x.BrandName,
                    CreateBy = x.CreateBy,
                    UpdateBy = x.UpdateBy,
                    LocationStockName = x.LocationStockName,
                    PlantName = x.Plant.PlantName,
                    DepartmentName = x.Department.DepartmentName,
                    LocationName = x.Location.LocationName,
                    PhaseName = x.PhaseName,
                    MachineName = x.Machine.MachineName,
                    UserName = x.User.FullName,
                    StatusName = x.Status.Status1
                }).ToList()
            );
            } 
          }

        public ActionResult DeviceInStock()
        {
            string param1 = this.Request.QueryString["model"];
            ViewBag.Model = param1;
            return View();
        }

        public JsonResult getDeviceByModel(string model)
        {
            var devices = (from d in db.Devices
                           join i in db.Departments on d.DepartmentID equals i.DepartmentID
                           into tempPets
                           from i in tempPets.DefaultIfEmpty()

                           join p in db.Plants on d.PlantID equals p.PlantID
                           into tempPets2
                           from p in tempPets2.DefaultIfEmpty()

                           join l in db.Locations on d.LocationID equals l.LocationID
                           into tempPets3
                           from l in tempPets3.DefaultIfEmpty()

                           join ph in db.Phases on d.PhaseID equals ph.PhaseID
                           into tempPets4
                           from ph in tempPets4.DefaultIfEmpty()

                           join mod in db.Models on d.ModelID equals mod.ModelID
                           into tempPets5
                           from mod in tempPets5.DefaultIfEmpty()

                           join t in db.DeviceTypes on d.DeviceTypeID equals t.DeviceTypeID
                           into tempPets6
                           from t in tempPets6.DefaultIfEmpty()

                           join b in db.Brands on d.BrandID equals b.BrandID
                           into tempPets7
                           from b in tempPets7.DefaultIfEmpty()

                           join s in db.Status on d.StatusID equals s.StatusID
                           into tempPets8
                           from s in tempPets8.DefaultIfEmpty()

                           join mc in db.Machines on d.MachineID equals mc.MachineID
                           into tempPets9
                           from mc in tempPets9.DefaultIfEmpty()

                           join us in db.Users on d.UserID equals us.UserID
                           into tempPets10
                           from us in tempPets10.DefaultIfEmpty()

                           join ls in db.LocationStocks on d.LocationStockID equals ls.LocationID
                           into tempPets11
                           from ls in tempPets11.DefaultIfEmpty()

                           where d.ModelName == model && d.StatusID == 3

                           select new ViewModelsDevices
                           {
                               DeviceID = d.DeviceID,
                               MachineName = mc.MachineName,
                               UserName = us.FullName,
                               FixAccess = d.FixAccess,
                               Asset = d.Description,
                               PRNumber = d.PRNumber,
                               MacAddress = d.MacAddress,
                               IPAddress = d.IPAddress,
                               SerialNumber = d.SerialNumber,
                               Specification = d.Specification,
                               Model = mod.ModelName,
                               Type = t.Type,
                               Brand = b.BrandName,
                               Status = s.Status1,
                               Plant = p.PlantName,
                               Department = i.DepartmentName,
                               Location = l.LocationName,
                               Phase = ph.PhaseName,
                               LocationStockName = ls.LocationName,
                               CreateBy = d.CreateBy,
                               UpdateBy = d.UpdateBy,
                               DateCreate = d.DateCreate,
                               DateUpdate = d.DateUpdate,
                           }).ToList();
            return Json(new { data = devices }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult IPScanner()
        {
            var devices = from device in db.Devices where device.IPAddress != null && device.StatusID == 1 group device by device.DeviceID into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        public ActionResult TotalDevice(string id)
        {
            var viewmodel = new DeviceRatioViewModels();
            if(id == null)
            {
                viewmodel.Devicend = from device in db.Devices group device by device.DeviceType.DeviceTypeID into dv let m = dv.FirstOrDefault() select m;
            }
            if (id == "Type")
            {
                viewmodel.Devicend = from device in db.Devices group device by device.DeviceType.DeviceTypeID into dv let m = dv.FirstOrDefault() select m;
            }
            else if (id == "Status")
            {
                viewmodel.Devicest = from device in db.Devices group device by device.Status.StatusID into dv let m = dv.FirstOrDefault() select m;
            }
            else if (id == "Department")
            {
                viewmodel.Devicerd = from device in db.Devices group device by device.Department.DepartmentID into dv let m = dv.FirstOrDefault() select m;
            }
            else if (id == "LStock")
            {
                viewmodel.Deviceth = from device in db.Devices group device by device.LocationStockName into dv let m = dv.FirstOrDefault() select m;
            }
            else if (id == "Spare")
            {
                viewmodel.Devicespr = from device in db.Devices  where device.StatusID == 5 group device by device.ModelID into dv let m = dv.FirstOrDefault() select m;
            }
            return View(viewmodel);
        }

        public ActionResult TotalMachine()
        {
            var machines = db.Machines.Include(m => m.Department).OrderBy(m => m.MachineName).Include(m => m.Location).Include(m => m.Phase).Include(m => m.Plant);
            return View(machines.ToList());
        }

        public ActionResult TotalUser()
        {
            var users = db.Users.Include(u => u.Department).Include(u => u.Location).Include(u => u.Plant);
            return View(users.ToList());
        }

        public ActionResult TotalDeviceUse()
        {
            return View();
        }

        public ActionResult TotalDeviceByType(int?id)
        {
            var devices = from device in db.Devices where device.DeviceType.DeviceTypeID == id group device by device.Model.ModelID into dv let m = dv.FirstOrDefault() select m;
            ViewBag.Type = db.DeviceTypes.Where(d => d.DeviceTypeID == id).Select(d => d.Type).DefaultIfEmpty().First();
            return View(devices);
        }

        public ActionResult TotalDeviceByModel(int? id, int? mid)
        {
            var devices = from device in db.Devices where device.DeviceType.DeviceTypeID == id && device.Model.ModelID == mid group device by device.DeviceID into dv let m = dv.FirstOrDefault() select m;
            ViewBag.Type = db.DeviceTypes.Where(d => d.DeviceTypeID == id).Select(d => d.Type).DefaultIfEmpty().First();
            return View(devices);
        }

        public ActionResult TotalDeviceByDepartment(int? id)
        {
            var devices = from device in db.Devices where device.StatusID == id group device by device.DepartmentID into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        public ActionResult TotalDeviceByLocationStock(int? id)
        {
            var devices = from device in db.Devices where device.StatusID == id group device by device.LocationStockID into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        public ActionResult TotalDeviceByStatus(int?id)
        {
            if(id == 1)
            {
                ViewBag.Status = "Use";
                ViewBag.ID = id;
                return View();
            }
            else if (id == 2)
            {
                ViewBag.Status = "In Repair";
                ViewBag.ID = id;
                return View();
            }
            else if (id == 3)
            {
                ViewBag.Status = "In Stock";
                ViewBag.ID = id;
                return View();
            }
            else if (id == 4)
            {
                ViewBag.Status = "In Sale";
                ViewBag.ID = id;
                return View();
            }
            else if (id == 5)
            {
                ViewBag.Status = "Spare";
                ViewBag.ID = id;
                return View();
            }
            else if (id == 6)
            {
                ViewBag.Status = "Sent Repair";
                ViewBag.ID = id;
                return View();
            }
            else if (id == 7)
            {
                ViewBag.Status = "Sent Sale";
                ViewBag.ID = id;
                return View();
            }
            else if (id == 8)
            {
                ViewBag.Status = "Wait Sent Repair";
                ViewBag.ID = id;
                return View();
            }
            else if (id == 9)
            {
                ViewBag.Status = "Wait Sent Sale";
                ViewBag.ID = id;
                return View();
            }
            return View();
        }

        public ActionResult FindByType(int? id, int? lid)
        {
            if (lid == null)
            {
                var devices = from device in db.Devices where device.StatusID == id group device by device.DeviceTypeID into dv let m = dv.FirstOrDefault() select m;
                ViewBag.sid = id;
                ViewBag.lid = lid;
                return View(devices);
            }
            else
            {
                var devices = from device in db.Devices where device.StatusID == id && device.LocationStockID == lid group device by device.DeviceTypeID into dv let m = dv.FirstOrDefault() select m;
                ViewBag.sid = id;
                ViewBag.lid = lid;
                return View(devices);
            }
        }

        public ActionResult InRepairByProduction()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InRepairByProduction([Bind(Include = "DeviceID,SerialNumber")] ProductionInRepair viewmodel)
        {
            if (ModelState.IsValid)
            {
                var serialnumber = viewmodel.SerialNumber;
                ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName");
                return RedirectToAction("setScannerInformRepair", "Dashboard", new { sr = serialnumber });
            }
            ModelState.AddModelError("SerialNumber", "Current status not ready to ReLocation");
            return View();
        }

        public ActionResult setScannerInformRepair(string sr)
        {
            if (sr == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int id = db.Devices.Where(d => d.SerialNumber == sr).Select(d => d.DeviceID).DefaultIfEmpty().First();
            if(id == 0)
            {
                return RedirectToAction("setNewScannerInformRepair", "Dashboard", new { sr = sr });
            }
            Device device = db.Devices.Find(id);
            ScannerInRepair sc = new ScannerInRepair();
            sc.DeviceID = device.DeviceID;
            sc.Brand = device.BrandName;
            sc.Model = device.ModelName;
            sc.Type = device.Type;
            sc.IPAddress = device.IPAddress;
            sc.SerialNumberR = device.SerialNumber;
            sc.Specification = device.Specification;
            sc.Status = device.StatusName;

            string CurPla = db.Devices.Where(d => d.DeviceID == id).Select(d => d.Plant.PlantName).DefaultIfEmpty().First();
            string CurDept = db.Devices.Where(d => d.DeviceID == id).Select(d => d.Department.DepartmentName).DefaultIfEmpty().First();
            string CurLoc = db.Devices.Where(d => d.DeviceID == id).Select(d => d.Location.LocationName).DefaultIfEmpty().First();
            string CurPhs = db.Devices.Where(d => d.DeviceID == id).Select(d => d.PhaseName).DefaultIfEmpty().First();

            if (string.IsNullOrEmpty(CurPla) && string.IsNullOrEmpty(CurDept) && string.IsNullOrEmpty(CurLoc) && string.IsNullOrEmpty(CurPhs))
            {
                sc.CurrentLocation = device.LocationStockName;
            }
            else
            {
                sc.CurrentLocation = CurPla + " ," + CurDept + " ," + CurLoc + " ," + CurPhs;
            }
            if (device == null)
            {
                return HttpNotFound();
            }
            else if (device.StatusID != 1 && device.StatusID != 3 && device.StatusID != 10)
            {
                ViewBag.FalseStatus = "อุปกรณ์หมายเลขซีเรียลนี้อยู่ในสถานะที่ไม่สามารถแจ้งซ่อมได้";
            }
            else if (device.DeviceTypeID != 58)
            {
                ViewBag.FalseType = "อุปกรณ์ชนิดนี้ไม่สามารถแจ้งซ่อมได้";
            }
            else if (device.StatusID == 3 || device.StatusID == 10)
            {
                ViewBag.NotUsed = "NotUsed";
                sc.FStatus = "1";
            }
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName");
            return View(sc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult setScannerInformRepair([Bind(Include = "DeviceID,SerialNumberR,CauseRepair,RequestBy,Model,IPAddress,CurrentLocation,Status,Specification,LocationStock,Machine,FStatus,FirstName,LastName")] ScannerInRepair viewmodel)
        {
            #region CheckData
            Device device = db.Devices.Find(viewmodel.DeviceID);
            viewmodel.Specification = device.Specification;

            if (string.IsNullOrEmpty(viewmodel.CauseRepair))
            {
                ModelState.AddModelError("CauseRepair", "กรอกอาการเสีย");
            }

            if (string.IsNullOrEmpty(viewmodel.FirstName))
            {
                ModelState.AddModelError("FirstName", "กรอกชื่อ");
            }

            if (string.IsNullOrEmpty(viewmodel.LastName))
            {
                ModelState.AddModelError("LastName", "กรอกนามสกุล");
            }

            if (string.IsNullOrEmpty(viewmodel.RequestBy))
            {
                ModelState.AddModelError("RequestBy", "กรอกรหัสพนักงาน");
            }

            if (!string.IsNullOrEmpty(viewmodel.RequestBy))
            {
                var rp = viewmodel.RequestBy.Substring(0, 2);
                if (!rp.Equals("rp"))
                {
                    ModelState.AddModelError("RequestBy", "กรอกรหัสพนักงาน");
                }
            }

            if (!string.IsNullOrEmpty(viewmodel.RequestBy))
            {
                var Name = viewmodel.RequestBy.Replace("\r", "").Replace("\n", "").Replace("\t", "").ToString();
                int MaxLength = Name.Length;
                var IsNum = viewmodel.RequestBy.Substring(2, MaxLength-2);
                int Num;
                if (!int.TryParse(IsNum, out Num))
                {
                    ModelState.AddModelError("RequestBy", "กรอกรหัสพนักงาน");
                }
            }

            //if (string.IsNullOrEmpty(viewmodel.LocationStock))
            //{
            //    ModelState.AddModelError("LocationStock", "เลือกสถานที่เก็บของ");
            //}

            if(device.StatusID == 3 || device.StatusID == 10)
            {
                if (string.IsNullOrEmpty(viewmodel.Machine))
                {
                    ModelState.AddModelError("Machine", "กรอกชื่อเครื่องจักร");
                }
            }
            #endregion

            if (ModelState.IsValid)
            {
                device.StatusID = 2;
                device.StatusName = "In Repair";

                if (!string.IsNullOrEmpty(viewmodel.Machine))
                {
                    device.MachineID = db.Machines.Where(m => m.MachineName == viewmodel.Machine).Select(m => m.MachineID).DefaultIfEmpty().First();
                    device.Reason = viewmodel.CauseRepair;
                    device.PlantID = db.Machines.Where(m => m.MachineID == device.MachineID).Select(m => m.PlantID).DefaultIfEmpty().First();
                    device.DepartmentID = db.Machines.Where(m => m.MachineID == device.MachineID).Select(m => m.DepartmentID).DefaultIfEmpty().First();
                    device.LocationID = db.Machines.Where(m => m.MachineID == device.MachineID).Select(m => m.LocationID).DefaultIfEmpty().First();
                    device.PhaseID = db.Machines.Where(m => m.MachineID == device.MachineID).Select(m => m.PhaseID).DefaultIfEmpty().First();
                    device.PhaseName = db.Machines.Where(m => m.MachineID == device.MachineID).Select(m => m.Phase.PhaseName).DefaultIfEmpty().First();
                }

                //device.LocationStockID = int.Parse(viewmodel.LocationStock);
                //int LocationStockID = int.Parse(viewmodel.LocationStock);
                //device.LocationStockName = db.LocationStocks.Where(l => l.LocationID == LocationStockID).Select(l => l.LocationName).DefaultIfEmpty().First();
                device.UpdateBy = viewmodel.RequestBy;
                device.DateUpdate = DateTime.Now;

                #region LogFile
                RecordInRepair recordinrepair = new RecordInRepair();
                recordinrepair.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recordinrepair.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                recordinrepair.RequestBy = viewmodel.RequestBy;
                recordinrepair.RequestFullName = viewmodel.FirstName+" "+viewmodel.LastName;
                recordinrepair.DateRequest = device.DateUpdate;
                recordinrepair.Cause = viewmodel.CauseRepair;
                recordinrepair.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recordinrepair.SerialNumber = db.Devices.Where(b => b.DeviceID == device.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                recordinrepair.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                recordinrepair.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                recordinrepair.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordinrepair.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                recordinrepair.LocationStock = db.LocationStocks.Where(b => b.LocationName == device.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordinrepair.Machine = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                recordinrepair.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First() +" By Production";
                recordinrepair.UserName = db.Users.Where(b => b.UserID == device.UserID).Select(b => b.FullName).DefaultIfEmpty().First();
                if (device.Description == "5k")
                {
                    recordinrepair.IsFixAsset = "Asset";
                }
#endregion

                db.Entry(device).State = EntityState.Modified;
                db.RecordInRepairs.Add(recordinrepair);
                db.SaveChanges();
                return RedirectToAction("LastUpdated", "Dashboard", new { id = device.DeviceID, uri = device.Uri });
            }
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName");
            return View(viewmodel);
        }

        public ActionResult setNewScannerInformRepair(string sr)
        {
            if (sr == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ScannerInRepair scanner = new ScannerInRepair();
            scanner.SerialNumberR = sr;
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName");
            return View(scanner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult setNewScannerInformRepair([Bind(Include = "DeviceID,SerialNumberR,CauseRepair,RequestBy,Model,IPAddress,CurrentLocation,Status,Specification,LocationStock,Machine,FirstName,LastName")] ScannerInRepair viewmodel)
        {
            #region CheckData
            var sr = viewmodel.SerialNumberR.Replace("\r", "").Replace("\n", "").Replace("\t", "").ToString();

            if(sr.Substring(0,2) == "SC")
            {
                if(sr.Length < 6)
                {
                    ModelState.AddModelError("SerialNumberR", "หมายเลขซีเรียลไม่ถูกต้อง");
                }
            }else if(sr.Length < 7)
            {
                ModelState.AddModelError("SerialNumberR", "หมายเลขซีเรียลไม่ถูกต้อง");
            }

            if (db.Devices.Any(d => d.SerialNumber.Trim() == viewmodel.SerialNumberR.Trim()))
            {
                ModelState.AddModelError("SerialNumberR", "หมายเลขซีเรียลซ้ำ");
            }

            if (db.Devices.Any(d => d.SerialNumber.Trim() == sr.Trim()))
            {
                ModelState.AddModelError("SerialNumberR", "หมายเลขซีเรียลซ้ำ");
            }

            if (string.IsNullOrEmpty(viewmodel.CauseRepair))
            {
                ModelState.AddModelError("CauseRepair", "กรอกอาการเสีย");
            }

            if (string.IsNullOrEmpty(viewmodel.RequestBy))
            {
                ModelState.AddModelError("RequestBy", "กรอกรหัสพนักงาน");
            }

            if (!string.IsNullOrEmpty(viewmodel.RequestBy))
            {
                var rp = viewmodel.RequestBy.Substring(0, 2);
                if (!rp.Equals("rp"))
                {
                    ModelState.AddModelError("RequestBy", "กรอกรหัสพนักงาน");
                }
            }

            if (!string.IsNullOrEmpty(viewmodel.RequestBy))
            {
                var Name = viewmodel.RequestBy.Replace("\r", "").Replace("\n", "").Replace("\t", "").ToString();
                int MaxLength = Name.Length;
                var IsNum = viewmodel.RequestBy.Substring(2, MaxLength - 2);
                int Num;
                if (!int.TryParse(IsNum, out Num))
                {
                    ModelState.AddModelError("RequestBy", "กรอกรหัสพนักงาน");
                }
            }

            //if (string.IsNullOrEmpty(viewmodel.LocationStock))
            //{
            //    ModelState.AddModelError("LocationStock", "เลือกสถานที่เก็บของ");
            //}

            if (string.IsNullOrEmpty(viewmodel.Model))
            {
                ModelState.AddModelError("Model", "กรอกชื่อโมเดล");
            }

            if (string.IsNullOrEmpty(viewmodel.Machine))
            {
                ModelState.AddModelError("Machine", "กรอกชื่อเครื่องจักร");
            }

            if (string.IsNullOrEmpty(viewmodel.FirstName))
            {
                ModelState.AddModelError("FirstName", "กรอกชื่อเครื่องจักร");
            }
            if (string.IsNullOrEmpty(viewmodel.LastName))
            {
                ModelState.AddModelError("LastName", "กรอกชื่อเครื่องจักร");
            }
            #endregion

            if (ModelState.IsValid)
            {
                Device device = new Device();
                device.SerialNumber = viewmodel.SerialNumberR;
                device.DateCreate = DateTime.Now;
                device.DateUpdate = DateTime.Now;
                device.CreateBy = viewmodel.RequestBy;
                device.UpdateBy = viewmodel.RequestBy;

                device.MachineID = db.Machines.Where(b => b.MachineName == viewmodel.Machine).Select(b => b.MachineID).DefaultIfEmpty().First();
                device.PlantID = db.Machines.Where(b => b.MachineName == viewmodel.Machine).Select(b => b.PlantID).DefaultIfEmpty().First();
                device.DepartmentID = db.Machines.Where(b => b.MachineName == viewmodel.Machine).Select(b => b.DepartmentID).DefaultIfEmpty().First();
                device.LocationID = db.Machines.Where(b => b.MachineName == viewmodel.Machine).Select(b => b.LocationID).DefaultIfEmpty().First();
                device.PhaseID = db.Machines.Where(b => b.MachineName == viewmodel.Machine).Select(b => b.PhaseID).DefaultIfEmpty().First();
                device.PhaseName = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();

                device.StatusID = 2;
                device.StatusName = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                device.LocationStockID = 1;
                device.LocationStockName = db.LocationStocks.Where(l => l.LocationID == device.LocationStockID).Select(l => l.LocationName).DefaultIfEmpty().First();
                device.InstockDate = DateTime.Now;
                device.ModelName = viewmodel.Model;
                device.ModelID = db.Models.Where(b => b.ModelName == viewmodel.Model).Select(b => b.ModelID).DefaultIfEmpty().First();
                device.BrandID = db.Models.Where(b => b.ModelName == viewmodel.Model).Select(b => b.BrandID).DefaultIfEmpty().First();
                device.Specification = db.Models.Where(b => b.ModelName == viewmodel.Model).Select(b => b.Specification).DefaultIfEmpty().First();
                device.DeviceTypeID = db.Models.Where(b => b.ModelName == viewmodel.Model).Select(b => b.DeviceTypeID).DefaultIfEmpty().First();
                device.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                device.BrandName = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                device.PhaseName = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();

                //if (viewmodel.IsAsset == true)
                //{
                //    device.Description = "5k";
                //}
                //device.FixAccess = deviceviewmodel.FixAccess;
                //device.PRNumber = deviceviewmodel.PRNumber;
                //device.UserName = deviceviewmodel.UserName;

                #region LogFile

                RecordInstock recordinstock = new RecordInstock();
                recordinstock.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recordinstock.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                recordinstock.InstockBy = viewmodel.RequestBy;
                recordinstock.DateInstock = DateTime.Now;
                recordinstock.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recordinstock.SerialNumber = viewmodel.SerialNumberR;
                recordinstock.Plant = "";
                recordinstock.Department = "";
                recordinstock.Location = "";
                recordinstock.Phase = "";
                recordinstock.LocationStock = device.LocationStockName;
                recordinstock.Machine = "";
                recordinstock.Status = "In Stock";
                //if (deviceviewmodel.IsAsset == true)
                //{
                //    recordinstock.IsFixAsset = "Asset";
                //}

                RecordInRepair recordinrepair = new RecordInRepair();
                recordinrepair.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recordinrepair.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                recordinrepair.RequestBy = viewmodel.RequestBy;
                recordinrepair.RequestFullName = viewmodel.FirstName+" "+viewmodel.LastName;
                recordinrepair.DateRequest = device.DateUpdate;
                recordinrepair.Cause = viewmodel.CauseRepair;
                recordinrepair.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recordinrepair.SerialNumber = viewmodel.SerialNumberR;
                recordinrepair.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                recordinrepair.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                recordinrepair.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordinrepair.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                recordinrepair.LocationStock = db.LocationStocks.Where(b => b.LocationName == device.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordinrepair.Machine = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                recordinrepair.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First() + " By Production";
                recordinrepair.UserName = db.Users.Where(b => b.UserID == device.UserID).Select(b => b.FullName).DefaultIfEmpty().First();
                #endregion

                db.Devices.Add(device);
                db.RecordInstocks.Add(recordinstock);
                db.RecordInRepairs.Add(recordinrepair);
                db.SaveChanges();
                return RedirectToAction("LastUpdated", "Dashboard", new { id = device.DeviceID });
            }
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName");
            return View(viewmodel);
        }


        public ActionResult LastUpdated(int? id)
        {
            var devices = db.Devices.Where(d => d.DeviceID == id).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine).OrderByDescending(d => d.DeviceID);
            return View(devices.ToList());
        }

        [Authorize]
        public ActionResult setReturnDevice(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecordInRepair inrepair = db.RecordInRepairs.Find(id);

            Device device = db.Devices.Where(d => d.SerialNumber == inrepair.SerialNumber).Single();
            device.UserName4 = id.ToString();
            return View(device);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult setReturnDevice([Bind(Include = "DeviceID,Type,SerialNumber,StatusName,UpdateBy,UserName4")] Device device)
        {
            if (!string.IsNullOrEmpty(device.DeviceID.ToString()))
            {
                Device dev = db.Devices.Find(device.DeviceID);
                dev.StatusID = 1;
                dev.StatusName = "Use";
                dev.DateUpdate = DateTime.Now;
                dev.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;

                int repairedID = int.Parse(device.UserName4);
                RecordInRepair inrepair = db.RecordInRepairs.Find(repairedID);
                inrepair.DateReturned = DateTime.Now;
                inrepair.ReturnedBy = System.Web.HttpContext.Current.User.Identity.Name;

                #region LogFile
                RecordRequisition recordRequisition = new RecordRequisition();
                recordRequisition.Brand = db.Brands.Where(b => b.BrandID == dev.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recordRequisition.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == dev.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                recordRequisition.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                recordRequisition.DateRequisition = dev.DateUpdate;
                recordRequisition.Cause = "Return to Use";
                recordRequisition.Model = db.Models.Where(b => b.ModelID == dev.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recordRequisition.SerialNumber = db.Devices.Where(b => b.DeviceID == dev.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                recordRequisition.Plant = db.Plants.Where(b => b.PlantID == dev.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                recordRequisition.Department = db.Departments.Where(b => b.DepartmentID == dev.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                recordRequisition.Location = db.Locations.Where(b => b.LocationID == dev.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordRequisition.Phase = db.Phases.Where(b => b.PhaseID == dev.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                recordRequisition.LocationStock = db.LocationStocks.Where(b => b.LocationName == dev.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordRequisition.Machine = db.Machines.Where(b => b.MachineID == dev.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                recordRequisition.Status = "Return to Use";
                recordRequisition.UserName = dev.UserName;
                recordRequisition.DeviceName = dev.DeviceName;
                if (dev.Description == "5k")
                {
                    recordRequisition.IsFixAsset = "Asset";
                }
                #endregion

                db.Entry(dev).State = EntityState.Modified;
                db.Entry(inrepair).State = EntityState.Modified;
                db.RecordRequisitions.Add(recordRequisition);
                db.SaveChanges();
                return RedirectToAction("InRepairByProductionReport", "Dashboard");
            }
            return View(device);
        }

        [Authorize]
        public ActionResult setReceivedDevice(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecordInRepair inrepair = db.RecordInRepairs.Find(id);

            ConfirmReceived received = new ConfirmReceived();
            received.DeviceID = inrepair.InRepairID;
            received.SerialNumber = inrepair.SerialNumber;
            received.ReceivedBy = System.Web.HttpContext.Current.User.Identity.Name;
            received.DateReceived = DateTime.Now;
            received.LocationStock = db.LocationStocks.Where(r => r.LocationName == inrepair.LocationStock).Select(r => r.LocationID).DefaultIfEmpty().First();
            if(inrepair.ReceivedBy != null)
            {
                received.IsReceived = true;
            }

            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName");
            return View(received);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult setReceivedDevice([Bind(Include = "DeviceID,LocationStock,IsReceived,ReceivedBy,DateReceived,SerialNumber")] ConfirmReceived viewmodel)
        {
            int ID = db.Devices.Where(d => d.SerialNumber == viewmodel.SerialNumber).Select(d => d.DeviceID).DefaultIfEmpty().First();

            Device device = db.Devices.Find(ID);

            if (ModelState.IsValid && viewmodel.IsReceived == true)
            {
                device.LocationStockID = viewmodel.LocationStock;
                device.LocationStockName = db.LocationStocks.Where(l => l.LocationID == viewmodel.LocationStock).Select(l => l.LocationName).DefaultIfEmpty().First();
                device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                device.DateUpdate = DateTime.Now;

                #region LogFile
                RecordInRepair recordinrepair = db.RecordInRepairs.Find(viewmodel.DeviceID);
                recordinrepair.ReceivedBy = System.Web.HttpContext.Current.User.Identity.Name;
                recordinrepair.LocationStock = device.LocationStockName;
                recordinrepair.DateReceived = DateTime.Now;
                #endregion

                db.Entry(device).State = EntityState.Modified;
                db.Entry(recordinrepair).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("InRepairByProductionReport", "Dashboard");
            }
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName");
            return View(viewmodel);
        }

        [Authorize]
        public ActionResult InStock()
        {
            var devices = from device in db.Devices where device.StatusID == 3 group device by device.DeviceType.Type into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        public ActionResult WaitSentSale()
        {
            var devices = db.Devices.Where(d=>d.StatusID == 9).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine).OrderByDescending(d=>d.DateUpdate);
            return View(devices.ToList());
        }

        public ActionResult WaitSentRepair()
        {
            var devices = from device in db.Devices where device.StatusID == 8 group device by device.DeviceID into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        public ActionResult InStockByBrand(int? id, int? sid)
        {
            var devices = from device in db.Devices where device.StatusID == sid && device.DeviceTypeID == id group device by device.BrandName into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        public ActionResult InStockByModel(int? id, int? sid, int? bid)
        {
            var devices = from device in db.Devices where device.StatusID == sid && device.DeviceTypeID == id && device.BrandID == bid group device by device.ModelName into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        public ActionResult InStockListItem(int? id, int? sid, int? bid, int? mid)
        {
            var devices = from device in db.Devices where device.StatusID == sid && device.DeviceTypeID == id && device.BrandID == bid &&  device.ModelID == mid group device by device.DeviceID into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        public ActionResult InStockByType(int?id, int?sid)
        {
            var devices = from device in db.Devices where device.StatusID == sid &&  device.DeviceTypeID == id group device by device.DeviceID into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        [Authorize]
        public ActionResult InUse()
        {
            var devices = from device in db.Devices where device.StatusID == 1 group device by device.DeviceType.Type into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        public ActionResult InUseByType(int? id, int? sid)
        {
            var devices = from device in db.Devices where device.StatusID == sid && device.DeviceTypeID == id group device by device.DeviceID into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        public ActionResult InUseByModel(int? id, int? sid)
        {
            var devices = from device in db.Devices where device.StatusID == sid && device.DeviceTypeID == id group device by device.ModelID into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        public ActionResult ListInUse(int? id, int? sid, int?mid)
        {
            var devices = from device in db.Devices where device.StatusID == sid && device.DeviceTypeID == id && device.ModelID == mid group device by device.DeviceID into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        public ActionResult ListSearchDevice(int?sid, int?lid, int?tid)
        {
            var devices = from device in db.Devices where device.StatusID == sid && device.LocationStockID == lid && device.DeviceTypeID == tid group device by device.ModelID into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        public ActionResult ListItemSearchDevice(int? sid, int? lid, int? tid, int? mid)
        {
            var devices = from device in db.Devices where device.StatusID == sid && device.LocationStockID == lid && device.DeviceTypeID == tid && device.ModelID == mid group device by device.DeviceID into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        public ActionResult RatioByType(int? id)
        {
            var devices = from device in db.Devices where device.DeviceTypeID == id group device by device.ModelName into dv let m = dv.FirstOrDefault() select m;
            DateTime date = DateTime.Today;
            var Date = date.ToString("dd");
            var Month = date.ToString("MM");
            int Dt = Int32.Parse(Date);
            int Mt = Int32.Parse(Month);

            ViewBag.Date = Dt;
            ViewBag.Month = Mt;
            return View(devices);
        }

        public ActionResult SearchDevice()
        {
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1");
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName");
            return View();
        }

        public ActionResult SearchDeviceBySerialNumber()
        {
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchDeviceBySerialNumber([Bind(Include = "DeviceID,SerialNumber")] Requistion requistion)
        {
            if (ModelState.IsValid && requistion.SerialNumber != null)
            {
                var serialnumber = requistion.SerialNumber;
                return RedirectToAction("FindBySerialNumber", "Dashboard", new { sr = serialnumber });
            }
            ModelState.AddModelError("SerialNumber", "Enter SerialNumber");
            return View();
        }

        [Authorize]
        public ActionResult FindBySerialNumber(string sr)
        {
            var devices = from device in db.Devices where device.SerialNumber.Contains(sr)  group device by device.DeviceID into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchDevice([Bind(Include = "StatusID,Status,LocationStockID")] FindDeviceViewModels finddeviceviewmodels)
        {
            if (string.IsNullOrEmpty(finddeviceviewmodels.StatusID.ToString()))
            {
                ModelState.AddModelError("StatusID", "StatusID is Required");
            }
            if (ModelState.IsValid)
            {
                return RedirectToAction("FindByType", "Dashboard", new { id = finddeviceviewmodels.StatusID, lid = finddeviceviewmodels.LocationStockID });
            }
            ModelState.AddModelError("SerialNumber", "Current status not ready to ReLocation");
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", finddeviceviewmodels.StatusID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", finddeviceviewmodels.LocationStockID);
            return View();
        }

        public ActionResult ExportPDF(int sid, int lid)
        {
            if (ModelState.IsValid)
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/Reports"), "InstockByDateReport.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return View("Index");
                }

                if (sid != 0)
                {
                    List<Device> cm = new List<Device>();

                    {
                        var query = from s in db.Devices
                                    orderby s.Type
                                    where s.StatusID == sid && s.LocationStockID == lid
                                    select s;
                        cm = query.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DataSet1", cm);
                    lr.DataSources.Add(rd);
                    string reportType = "Excel";
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                    "  <OutputFormat>" + "Excel" + "</OutputFormat>" +
                    "  <PageWidth>11.7in</PageWidth>" +
                    "  <PageHeight>16.5in</PageHeight>" +
                    "  <MarginTop>0.5in</MarginTop>" +
                    "  <MarginLeft>1in</MarginLeft>" +
                    "  <MarginRight>1in</MarginRight>" +
                    "  <MarginBottom>0.5in</MarginBottom>" +
                    "</DeviceInfo>";

                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = lr.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warnings);
                    return File(renderedBytes, mimeType);
                }
            }
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1");
            return View();
        }

        [HttpPost]
        public JsonResult FindDevice(string prefixText)
        {
            var modelname = from x in db.Devices
                            where x.SerialNumber.Contains(prefixText) || x.DeviceType.Type.Contains(prefixText) || x.Brand.BrandName.Contains(prefixText) || x.Model.ModelName.Contains(prefixText)
                            select new
                            {
                                value = x.DeviceType.Type + " " + x.Model.ModelName + " " + x.Brand.BrandName + " " + x.SerialNumber,
                                name = x.SerialNumber,
                                id = x.DeviceID
                            };
            var result = Json(modelname.Take(5).ToList());
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName");
            return result;
        }

        [Authorize]
        public ActionResult FindingDevice()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FindingDevice([Bind(Include = "DeviceID,SerialNumber")] Requistion requistion)
        {
            if (ModelState.IsValid && !db.Devices.Any(d => d.SerialNumber == requistion.SerialNumber && d.StatusID != 5 && d.StatusID != 3))
            {
                var serialnumber = requistion.SerialNumber;
                return RedirectToAction("EditLocationStock", "Dashboard", new { sr = serialnumber });
            }
            ModelState.AddModelError("SerialNumber", "Current status not ready to ReLocation");
            return View();
        }

        [Authorize]
        public ActionResult FHistoryDevice()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FHistoryDevice([Bind(Include = "DeviceID,SerialNumber")] Requistion requistion)
        {
            if (ModelState.IsValid)
            {
                var serialnumber = requistion.SerialNumber;
                return RedirectToAction("HistoryDevice", "Dashboard", new { sr = serialnumber });
            }
            ModelState.AddModelError("SerialNumber", "Current status not ready to ReLocation");
            return View();
        }

        [Authorize]
        public ActionResult EditLocationStock(string sr)
        {
            var devices = db.Devices.Where(d => d.SerialNumber == sr).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            return View(devices.ToList());
        }

        [Authorize]
        public ActionResult HistoryDevice(string sr)
        {
            CombineViewModels combineviewmodels = new CombineViewModels();
            combineviewmodels.RecordInstock = db.RecordInstocks.Where(d => d.SerialNumber == sr).ToList();
            combineviewmodels.RecordRequisition = db.RecordRequisitions.Where(d => d.SerialNumber == sr).ToList();
            combineviewmodels.RecordInRepair = db.RecordInRepairs.Where(d => d.SerialNumber == sr).ToList();
            combineviewmodels.RecordSale = db.RecordSales.Where(d => d.SerialNumber == sr).ToList();
            combineviewmodels.RecordSpare = db.RecordSpares.Where(d => d.SerialNumber == sr).ToList();
            combineviewmodels.RecordReinstock = db.RecordReinstocks.Where(d => d.SerialNumber == sr).ToList();
            return View(combineviewmodels);
        }

        [Authorize]
        public ActionResult SetLocationStock(int? id, string uri)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            device.DateUpdate = DateTime.Now;
            device.Uri = uri;
            if (device == null)
            {
                return HttpNotFound();
            }
            else if (device.StatusID != 5 && device.StatusID != 3)
            {
                return Content("Current status can't move location");
            }

            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetLocationStock([Bind(Include = "DeviceID,MachineID,UMachineID,DeviceName,Description,SerialNumber,Specification,DepartmentID,PlantID,LocationID,DeviceTypeID,BrandID,StatusID,ModelID,DateCreate,DateUpdate,CreateBy,UpdateBy,LocationStockID,LocationStockName,ModelName,Type,BrandName,StatusName,InstockDate,PhaseID,PhaseName,PRNumber,FixAccess,Uri,MacAddress,CauseRequistion")] Device device)
        {

            if (string.IsNullOrEmpty(device.CauseRequistion))
            {
                ModelState.AddModelError("CauseRequistion", "Cause is Required");
            }

            if (ModelState.IsValid)
            {
                device.ModelID = db.Models.Where(b => b.ModelName == device.ModelName).Select(b => b.ModelID).DefaultIfEmpty().First();
                device.StatusName = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();

                device.BrandID = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.BrandID).DefaultIfEmpty().First();
                device.Specification = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.Specification).DefaultIfEmpty().First();
                device.DeviceTypeID = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.DeviceTypeID).DefaultIfEmpty().First();

                device.Reason = device.CauseRequistion;
                device.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                device.BrandName = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                device.LocationStockName = db.LocationStocks.Where(b => b.LocationID == device.LocationStockID).Select(b => b.LocationName).DefaultIfEmpty().First();
                device.PhaseName = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();

                RecordDevice recorddevice = new RecordDevice();
                recorddevice.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recorddevice.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                recorddevice.EditBy = System.Web.HttpContext.Current.User.Identity.Name;
                recorddevice.EditDate = device.DateUpdate;
                recorddevice.Cause = device.CauseRequistion;
                recorddevice.Description = "Change LocationStock";
                recorddevice.Specification = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.Specification).DefaultIfEmpty().First();
                recorddevice.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recorddevice.SerialNumber = db.Devices.Where(b => b.DeviceID == device.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                recorddevice.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                recorddevice.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                recorddevice.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                recorddevice.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                recorddevice.LocationStock = db.LocationStocks.Where(b => b.LocationName == device.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                recorddevice.Machine = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                recorddevice.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();

                db.Entry(device).State = EntityState.Modified;
                db.RecordDevices.Add(recorddevice);
                db.SaveChanges();
                return RedirectToAction("LastSet", "Device", new { id = device.DeviceID, uri=device.Uri });
            }

            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        public ActionResult InRepairByProductionReport()
        {
            return View();
        }

        public ActionResult loadDataScannerInrepair()
        {
            using (var dc = new ITStockEntities1())
            {
                dc.Configuration.ProxyCreationEnabled = false;
                var data = dc.RecordInRepairs.Where(a => a.Status.Length > 22).OrderBy(a => a.DateRequest).ToList();
                return Json(new { data = data }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult getSerialNumberTemplated()
        {
            var templates = db.SerialNumberTemplates.ToList();
            return Json(new { data = templates }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getSerialNumberGenerated()
        {
            var generates = db.SerialNumberGenerates.ToList();
            List<ViewModelSerialNumberGenerate> options = new List<ViewModelSerialNumberGenerate>();
            foreach (var i in generates)
            {
                var c = db.Devices.Where(b => b.SerialNumber.Equals(i.SerialNumber)).Select(b => b.SerialNumber).DefaultIfEmpty().First();

                if (c == i.SerialNumber)
                {
                    options.Add(new ViewModelSerialNumberGenerate
                    {
                        GenerateID = i.GenerateID,
                        DeviceType = i.DeviceType,
                        CreateBy = i.CreateBy,
                        UpdateBy = i.UpdateBy,
                        SerialNumber = i.SerialNumber,
                        DateCreate = i.DateCreate,
                        DateUpdate = i.DateUpdate,
                        IsUse = "$",
                    });
                }
                else if(c != i.SerialNumber)
                {
                    options.Add(new ViewModelSerialNumberGenerate
                    {
                        GenerateID = i.GenerateID,
                        DeviceType = i.DeviceType,
                        CreateBy = i.CreateBy,
                        UpdateBy = i.UpdateBy,
                        SerialNumber = i.SerialNumber,
                        DateCreate = i.DateCreate,
                        DateUpdate = i.DateUpdate,
                        IsUse = "",
                    });
                }
            }
            return Json(new { data = options }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RecordDevice()
        {
            var recorddevice = db.RecordDevices.OrderByDescending(r=>r.EditDate);
            return View(recorddevice.ToList());
        }

        public ActionResult DeviceDetials()
        {
            DeviceDetialsViewModels devicedetialsviewModels = new DeviceDetialsViewModels();
            devicedetialsviewModels.Device = db.Devices.ToList();
            devicedetialsviewModels.DeviceType = db.DeviceTypes.OrderBy(d=>d.Type).ToList();
            return View(devicedetialsviewModels);
        }

        public static IEnumerable<SelectListItem> GetTerritoryList()
        {
            IList<SelectListItem> territory = new List<SelectListItem>
            {
                new SelectListItem() {Text="", Value=""},
                new SelectListItem() { Text="DeviceType", Value="DeviceType"},
                new SelectListItem() { Text="Model", Value="Model"},
                new SelectListItem() { Text="Brand", Value="Brand"},
            };
            return territory;
        }

        public static IEnumerable<SelectListItem> GetPhaseList()
        {
            IList<SelectListItem> phases = new List<SelectListItem>
            {
                new SelectListItem() {Text="", Value=""},
                new SelectListItem() { Text="Phase1", Value="1"},
                new SelectListItem() { Text="Phase2", Value="2"},
                new SelectListItem() { Text="Phase3", Value="3"},
                new SelectListItem() { Text="Phase4", Value="4"},
            };
            return phases;
        }

        public static IEnumerable<SelectListItem> GetListDeviceByStockLocation()
        {
            IList<SelectListItem> phases = new List<SelectListItem>
            {
                new SelectListItem() {Text="", Value=""},
                new SelectListItem() { Text="In Repair", Value="2"},
                new SelectListItem() { Text="In Stock", Value="3"},
                new SelectListItem() { Text="Spare", Value="5"},
            };
            return phases;
        }
    }
}