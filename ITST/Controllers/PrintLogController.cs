using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Microsoft.Reporting.WebForms;
using ITST.Models;
using ITST.ViewModels;
using System.Globalization;
using System.Data.Entity.Core.Objects;

namespace ITST.Controllers
{
    [Authorize]
    public class PrintLogController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        //
        // GET: /PrintLog/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AssetRequisitionReport()
        {
            return View();
        }

        public JsonResult getAssetRequisition()
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

                           where d.StatusID == 1 && d.Description == "5k"

                           select new TotalDeviceViewModel
                           {
                               DeviceID = d.DeviceID,
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

        public ActionResult byCriteria()
        {
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1");
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "Type", "Type");
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelName", "ModelName");
            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandName", "BrandName");
            ViewBag.LocationID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationName", "LocationName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult byCriteria([Bind(Include = "StatusID,DeviceTypeID,Model,Brand,LocationStock,PRNumber,FixAccess,StartDate,EndDate")] ReportCriteriaViewModels reportviewmodels)
        {
            if (ModelState.IsValid)
            {
                RedirectToAction("getResultCriteria");
            }

            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1");
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "Type", "Type");
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelName", "ModelName");
            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandName", "BrandName");
            ViewBag.LocationID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationName", "LocationName");
            return View(reportviewmodels);
        }

        public JsonResult getResultCriteria(string type, string status, string location, string model, string brand, string prnumber, string fixaccess, string te, string de)
        {
            #region getRepaired
            if (!String.IsNullOrEmpty(status) && status == "2" && String.IsNullOrEmpty(de) && String.IsNullOrEmpty(te))
            {
                var repaired = (from rep in db.RecordInRepairs
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where 
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location)
                                select new LogFileViewModels
                                {
                                    ActionBy = rep.RequestBy,
                                    ActionDate = rep.DateRequest,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = rep.Cause,
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            else if (!String.IsNullOrEmpty(status) && status == "2" && !String.IsNullOrEmpty(de) && String.IsNullOrEmpty(te))
            {
                DateTime start = DateTime.Parse(de);

                var repaired = (from rep in db.RecordInRepairs
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location) &&
                                 (de == "" ? true : EntityFunctions.TruncateTime(rep.DateRequest) == EntityFunctions.TruncateTime(start))

                                select new LogFileViewModels
                                {
                                    ActionBy = rep.RequestBy,
                                    ActionDate = rep.DateRequest,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = rep.Cause,
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            else if (!String.IsNullOrEmpty(status) && status == "2" && !String.IsNullOrEmpty(de) && !String.IsNullOrEmpty(te))
            {
                DateTime start = DateTime.Parse(de);
                DateTime end = DateTime.Parse(te);

                var repaired = (from rep in db.RecordInRepairs
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location) &&
                                 (de == "" ? true : EntityFunctions.TruncateTime(rep.DateRequest) >= EntityFunctions.TruncateTime(start) && EntityFunctions.TruncateTime(rep.DateRequest) <= EntityFunctions.TruncateTime(end))

                                select new LogFileViewModels
                                {
                                    ActionBy = rep.RequestBy,
                                    ActionDate = rep.DateRequest,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = rep.Cause,
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region getInStock
            if (!String.IsNullOrEmpty(status) && status == "3" && String.IsNullOrEmpty(de) && String.IsNullOrEmpty(te))
            {
                var repaired = (from rep in db.RecordInstocks
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location)
                                select new LogFileViewModels
                                {
                                    ActionBy = rep.InstockBy,
                                    ActionDate = rep.DateInstock,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = "",
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).Union(from rep in db.RecordReinstocks
                                         join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                         where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location)
                                         select new LogFileViewModels
                                         {
                                             ActionBy = rep.RequestBy,
                                             ActionDate = rep.DateRequest,
                                             DeviceType = rep.DeviceType,
                                             Model = rep.Model,
                                             Brand = rep.Brand,
                                             SerialNumber = rep.SerialNumber,
                                             PRNumber = d.PRNumber,
                                             FixAccess = d.FixAccess,
                                             Plant = rep.Plant,
                                             Department = rep.Department,
                                             Location = rep.Location,
                                             Phase = rep.Phase,
                                             Status = rep.Status,
                                             LocationStock = rep.LocationStock,
                                             Cause = rep.Cause,
                                             Machine = rep.Machine,
                                             DeviceName = rep.DeviceName,
                                             UserName = rep.UserName
                                         }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            else if (!String.IsNullOrEmpty(status) && status == "3" && !String.IsNullOrEmpty(de) && String.IsNullOrEmpty(te))
            {
                DateTime start = DateTime.Parse(de);

                var repaired = (from rep in db.RecordInstocks
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location) &&
                                 (de == "" ? true : EntityFunctions.TruncateTime(rep.DateInstock) == EntityFunctions.TruncateTime(start))

                                select new LogFileViewModels
                                {
                                    ActionBy = rep.InstockBy,
                                    ActionDate = rep.DateInstock,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = "",
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).Union(from rep in db.RecordReinstocks
                                         join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                         where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location) &&
                                 (de == "" ? true : EntityFunctions.TruncateTime(rep.DateRequest) == EntityFunctions.TruncateTime(start))
                                         select new LogFileViewModels
                                         {
                                             ActionBy = rep.RequestBy,
                                             ActionDate = rep.DateRequest,
                                             DeviceType = rep.DeviceType,
                                             Model = rep.Model,
                                             Brand = rep.Brand,
                                             SerialNumber = rep.SerialNumber,
                                             PRNumber = d.PRNumber,
                                             FixAccess = d.FixAccess,
                                             Plant = rep.Plant,
                                             Department = rep.Department,
                                             Location = rep.Location,
                                             Phase = rep.Phase,
                                             Status = rep.Status,
                                             LocationStock = rep.LocationStock,
                                             Cause = rep.Cause,
                                             Machine = rep.Machine,
                                             DeviceName = rep.DeviceName,
                                             UserName = rep.UserName
                                         }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            else if (!String.IsNullOrEmpty(status) && status == "3" && !String.IsNullOrEmpty(de) && !String.IsNullOrEmpty(te))
            {
                DateTime start = DateTime.Parse(de);
                DateTime end = DateTime.Parse(te);

                var repaired = (from rep in db.RecordInstocks
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location) &&
                                 (de == "" ? true : EntityFunctions.TruncateTime(rep.DateInstock) >= EntityFunctions.TruncateTime(start) && EntityFunctions.TruncateTime(rep.DateInstock) <= EntityFunctions.TruncateTime(end))

                                select new LogFileViewModels
                                {
                                    ActionBy = rep.InstockBy,
                                    ActionDate = rep.DateInstock,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = "",
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).Union(from rep in db.RecordReinstocks
                                         join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                         where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location) &&
                                 (de == "" ? true : EntityFunctions.TruncateTime(rep.DateRequest) >= EntityFunctions.TruncateTime(start) && EntityFunctions.TruncateTime(rep.DateRequest) <= EntityFunctions.TruncateTime(end))
                                         select new LogFileViewModels
                                         {
                                             ActionBy = rep.RequestBy,
                                             ActionDate = rep.DateRequest,
                                             DeviceType = rep.DeviceType,
                                             Model = rep.Model,
                                             Brand = rep.Brand,
                                             SerialNumber = rep.SerialNumber,
                                             PRNumber = d.PRNumber,
                                             FixAccess = d.FixAccess,
                                             Plant = rep.Plant,
                                             Department = rep.Department,
                                             Location = rep.Location,
                                             Phase = rep.Phase,
                                             Status = rep.Status,
                                             LocationStock = rep.LocationStock,
                                             Cause = rep.Cause,
                                             Machine = rep.Machine,
                                             DeviceName = rep.DeviceName,
                                             UserName = rep.UserName
                                         }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region getInUsed
            if (!String.IsNullOrEmpty(status) && status == "1" && String.IsNullOrEmpty(de) && String.IsNullOrEmpty(te))
            {
                var repaired = (from rep in db.RecordRequisitions
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location)
                                select new LogFileViewModels
                                {
                                    ActionBy = rep.RequestBy,
                                    ActionDate = rep.DateRequisition,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = rep.Cause,
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            else if (!String.IsNullOrEmpty(status) && status == "1" && !String.IsNullOrEmpty(de) && String.IsNullOrEmpty(te))
            {
                DateTime start = DateTime.Parse(de);

                var repaired = (from rep in db.RecordRequisitions
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location) &&
                                 (de == "" ? true : EntityFunctions.TruncateTime(rep.DateRequisition) == EntityFunctions.TruncateTime(start))

                                select new LogFileViewModels
                                {
                                    ActionBy = rep.RequestBy,
                                    ActionDate = rep.DateRequisition,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = rep.Cause,
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            else if (!String.IsNullOrEmpty(status) && status == "1" && !String.IsNullOrEmpty(de) && !String.IsNullOrEmpty(te))
            {
                DateTime start = DateTime.Parse(de);
                DateTime end = DateTime.Parse(te);

                var repaired = (from rep in db.RecordRequisitions
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location) &&
                                 (de == "" ? true : EntityFunctions.TruncateTime(rep.DateRequisition) >= EntityFunctions.TruncateTime(start) && EntityFunctions.TruncateTime(rep.DateRequisition) <= EntityFunctions.TruncateTime(end))

                                select new LogFileViewModels
                                {
                                    ActionBy = rep.RequestBy,
                                    ActionDate = rep.DateRequisition,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = rep.Cause,
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region getSale
            if (!String.IsNullOrEmpty(status) && status == "4" && String.IsNullOrEmpty(de) && String.IsNullOrEmpty(te))
            {
                var repaired = (from rep in db.RecordSales
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location)
                                select new LogFileViewModels
                                {
                                    ActionBy = rep.RequestBy,
                                    ActionDate = rep.DateRequest,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = rep.Cause,
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            else if (!String.IsNullOrEmpty(status) && status == "4" && !String.IsNullOrEmpty(de) && String.IsNullOrEmpty(te))
            {
                DateTime start = DateTime.Parse(de);

                var repaired = (from rep in db.RecordSales
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location) &&
                                 (de == "" ? true : EntityFunctions.TruncateTime(rep.DateRequest) == EntityFunctions.TruncateTime(start))

                                select new LogFileViewModels
                                {
                                    ActionBy = rep.RequestBy,
                                    ActionDate = rep.DateRequest,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = rep.Cause,
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            else if (!String.IsNullOrEmpty(status) && status == "4" && !String.IsNullOrEmpty(de) && !String.IsNullOrEmpty(te))
            {
                DateTime start = DateTime.Parse(de);
                DateTime end = DateTime.Parse(te);

                var repaired = (from rep in db.RecordSales
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location) &&
                                 (de == "" ? true : EntityFunctions.TruncateTime(rep.DateRequest) >= EntityFunctions.TruncateTime(start) && EntityFunctions.TruncateTime(rep.DateRequest) <= EntityFunctions.TruncateTime(end))

                                select new LogFileViewModels
                                {
                                    ActionBy = rep.RequestBy,
                                    ActionDate = rep.DateRequest,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = rep.Cause,
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region getSentRepaired
            if (!String.IsNullOrEmpty(status) && status == "6" && String.IsNullOrEmpty(de) && String.IsNullOrEmpty(te))
            {
                var repaired = (from rep in db.RecordInRepairs.Where(r=>r.Status == "Sent Repair")
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location)
                                select new LogFileViewModels
                                {
                                    ActionBy = rep.RequestBy,
                                    ActionDate = rep.DateRequest,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = rep.Cause,
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            else if (!String.IsNullOrEmpty(status) && status == "6" && !String.IsNullOrEmpty(de) && String.IsNullOrEmpty(te))
            {
                DateTime start = DateTime.Parse(de);

                var repaired = (from rep in db.RecordInRepairs.Where(r => r.Status == "Sent Repair")
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location) &&
                                 (de == "" ? true : EntityFunctions.TruncateTime(rep.DateRequest) == EntityFunctions.TruncateTime(start))

                                select new LogFileViewModels
                                {
                                    ActionBy = rep.RequestBy,
                                    ActionDate = rep.DateRequest,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = rep.Cause,
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            else if (!String.IsNullOrEmpty(status) && status == "6" && !String.IsNullOrEmpty(de) && !String.IsNullOrEmpty(te))
            {
                DateTime start = DateTime.Parse(de);
                DateTime end = DateTime.Parse(te);

                var repaired = (from rep in db.RecordInRepairs.Where(r => r.Status == "Sent Repair")
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location) &&
                                 (de == "" ? true : EntityFunctions.TruncateTime(rep.DateRequest) >= EntityFunctions.TruncateTime(start) && EntityFunctions.TruncateTime(rep.DateRequest) <= EntityFunctions.TruncateTime(end))

                                select new LogFileViewModels
                                {
                                    ActionBy = rep.RequestBy,
                                    ActionDate = rep.DateRequest,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = rep.Cause,
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region getSentSale
            if (!String.IsNullOrEmpty(status) && status == "7" && String.IsNullOrEmpty(de) && String.IsNullOrEmpty(te))
            {
                var repaired = (from rep in db.RecordSales.Where(r=>r.Status == "Sent Sale")
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location)
                                select new LogFileViewModels
                                {
                                    ActionBy = rep.RequestBy,
                                    ActionDate = rep.DateRequest,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = rep.Cause,
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            else if (!String.IsNullOrEmpty(status) && status == "7" && !String.IsNullOrEmpty(de) && String.IsNullOrEmpty(te))
            {
                DateTime start = DateTime.Parse(de);

                var repaired = (from rep in db.RecordSales.Where(r => r.Status == "Sent Sale")
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location) &&
                                 (de == "" ? true : EntityFunctions.TruncateTime(rep.DateRequest) == EntityFunctions.TruncateTime(start))

                                select new LogFileViewModels
                                {
                                    ActionBy = rep.RequestBy,
                                    ActionDate = rep.DateRequest,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = rep.Cause,
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            else if (!String.IsNullOrEmpty(status) && status == "7" && !String.IsNullOrEmpty(de) && !String.IsNullOrEmpty(te))
            {
                DateTime start = DateTime.Parse(de);
                DateTime end = DateTime.Parse(te);

                var repaired = (from rep in db.RecordSales.Where(r => r.Status == "Sent Sale")
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location) &&
                                 (de == "" ? true : EntityFunctions.TruncateTime(rep.DateRequest) >= EntityFunctions.TruncateTime(start) && EntityFunctions.TruncateTime(rep.DateRequest) <= EntityFunctions.TruncateTime(end))

                                select new LogFileViewModels
                                {
                                    ActionBy = rep.RequestBy,
                                    ActionDate = rep.DateRequest,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = rep.Cause,
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region getUnknown
            if (!String.IsNullOrEmpty(status) && status == "10" && String.IsNullOrEmpty(de) && String.IsNullOrEmpty(te))
            {
                var repaired = (from rep in db.RecordDevices.Where(r => r.Status == "Unknown")
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.Type == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location)
                                select new LogFileViewModels
                                {
                                    ActionBy = rep.EditBy,
                                    ActionDate = rep.EditDate,
                                    DeviceType = rep.Type,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = "",
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            else if (!String.IsNullOrEmpty(status) && status == "10" && !String.IsNullOrEmpty(de) && String.IsNullOrEmpty(te))
            {
                DateTime start = DateTime.Parse(de);

                var repaired = (from rep in db.RecordDevices.Where(r => r.Status == "Unknown")
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.Type == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location) &&
                                 (de == "" ? true : EntityFunctions.TruncateTime(rep.EditDate) == EntityFunctions.TruncateTime(start))

                                select new LogFileViewModels
                                {
                                    ActionBy = rep.EditBy,
                                    ActionDate = rep.EditDate,
                                    DeviceType = rep.Type,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = "",
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            else if (!String.IsNullOrEmpty(status) && status == "10" && !String.IsNullOrEmpty(de) && !String.IsNullOrEmpty(te))
            {
                DateTime start = DateTime.Parse(de);
                DateTime end = DateTime.Parse(te);

                var repaired = (from rep in db.RecordDevices.Where(r => r.Status == "Unknown")
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.Type == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location) &&
                                 (de == "" ? true : EntityFunctions.TruncateTime(rep.EditDate) >= EntityFunctions.TruncateTime(start) && EntityFunctions.TruncateTime(rep.EditDate) <= EntityFunctions.TruncateTime(end))

                                select new LogFileViewModels
                                {
                                    ActionBy = rep.EditBy,
                                    ActionDate = rep.EditDate,
                                    DeviceType = rep.Type,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = "",
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region getSpare
            if (!String.IsNullOrEmpty(status) && status == "5" && String.IsNullOrEmpty(de) && String.IsNullOrEmpty(te))
            {
                var repaired = (from rep in db.RecordSpares
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location)
                                select new LogFileViewModels
                                {
                                    ActionBy = rep.RequestBy,
                                    ActionDate = rep.DateRequest,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = "",
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            else if (!String.IsNullOrEmpty(status) && status == "5" && !String.IsNullOrEmpty(de) && String.IsNullOrEmpty(te))
            {
                DateTime start = DateTime.Parse(de);

                var repaired = (from rep in db.RecordSpares
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location) &&
                                 (de == "" ? true : EntityFunctions.TruncateTime(rep.DateRequest) == EntityFunctions.TruncateTime(start))

                                select new LogFileViewModels
                                {
                                    ActionBy = rep.RequestBy,
                                    ActionDate = rep.DateRequest,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = "",
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            else if (!String.IsNullOrEmpty(status) && status == "5" && !String.IsNullOrEmpty(de) && !String.IsNullOrEmpty(te))
            {
                DateTime start = DateTime.Parse(de);
                DateTime end = DateTime.Parse(te);

                var repaired = (from rep in db.RecordSpares
                                join d in db.Devices on rep.SerialNumber equals d.SerialNumber
                                where
                                 (model == "" ? true : rep.Model == model) &&
                                 (type == "" ? true : rep.DeviceType == type) &&
                                 (brand == "" ? true : rep.Brand == brand) &&
                                 (prnumber == "No" || prnumber == "" ? d.PRNumber == null : d.PRNumber != null) &&
                                 (fixaccess == "No" || fixaccess == "" ? d.FixAccess == null : d.FixAccess != null) &&
                                 (location == "" ? true : rep.LocationStock == location) &&
                                 (de == "" ? true : EntityFunctions.TruncateTime(rep.DateRequest) >= EntityFunctions.TruncateTime(start) && EntityFunctions.TruncateTime(rep.DateRequest) <= EntityFunctions.TruncateTime(end))

                                select new LogFileViewModels
                                {
                                    ActionBy = rep.RequestBy,
                                    ActionDate = rep.DateRequest,
                                    DeviceType = rep.DeviceType,
                                    Model = rep.Model,
                                    Brand = rep.Brand,
                                    SerialNumber = rep.SerialNumber,
                                    PRNumber = d.PRNumber,
                                    FixAccess = d.FixAccess,
                                    Plant = rep.Plant,
                                    Department = rep.Department,
                                    Location = rep.Location,
                                    Phase = rep.Phase,
                                    Status = rep.Status,
                                    LocationStock = rep.LocationStock,
                                    Cause = "",
                                    Machine = rep.Machine,
                                    DeviceName = rep.DeviceName,
                                    UserName = rep.UserName
                                }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region getAllStatus
            if (!String.IsNullOrEmpty(status) && status == "11" && String.IsNullOrEmpty(de) && String.IsNullOrEmpty(te))
            {
                var repaired = (from d in db.RecordDevices
                                //join x in db.Devices on d.SerialNumber equals x.SerialNumber
                                where
                                 (model == "" ? true : d.Model == model) &&
                                 (type == "" ? true : d.Type == type) &&
                                 (brand == "" ? true : d.Brand == brand) &&
                                 //(prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                                 //(fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                                 (location == "" ? true : d.LocationStock == location)

                                select new LogFileViewModels
                                {
                                    ActionBy = d.EditBy,
                                    ActionDate = d.EditDate,
                                    DeviceType = d.Type,
                                    Model = d.Model,
                                    Brand = d.Brand,
                                    SerialNumber = d.SerialNumber,
                                    PRNumber = "",
                                    FixAccess = "",
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
                            (from d in db.RecordInstocks
                             //join x in db.Devices on d.SerialNumber equals x.SerialNumber
                             where
                            (model == "" ? true : d.Model == model) &&
                            (type == "" ? true : d.DeviceType == type) &&
                            (brand == "" ? true : d.Brand == brand) &&
                            //(prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                            //(fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                            (location == "" ? true : d.LocationStock == location)

                             select new LogFileViewModels
                             {
                                 ActionBy = d.InstockBy,
                                 ActionDate = d.DateInstock,
                                 DeviceType = d.DeviceType,
                                 Model = d.Model,
                                 Brand = d.Brand,
                                 SerialNumber = d.SerialNumber,
                                 PRNumber = "",
                                 FixAccess = "",
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
                             (from d in db.RecordReinstocks
                              //join x in db.Devices on d.SerialNumber equals x.SerialNumber
                              where
                              (model == "" ? true : d.Model == model) &&
                              (type == "" ? true : d.DeviceType == type) &&
                              (brand == "" ? true : d.Brand == brand) &&
                              //(prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                              //(fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                              (location == "" ? true : d.LocationStock == location)

                              select new LogFileViewModels
                              {
                                  ActionBy = d.RequestBy,
                                  ActionDate = d.DateRequest,
                                  DeviceType = d.DeviceType,
                                  Model = d.Model,
                                  Brand = d.Brand,
                                  SerialNumber = d.SerialNumber,
                                  PRNumber = "",
                                  FixAccess = "",
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
                              (from d in db.RecordSales
                               //join x in db.Devices on d.SerialNumber equals x.SerialNumber
                               where
                                (model == "" ? true : d.Model == model) &&
                                (type == "" ? true : d.DeviceType == type) &&
                                (brand == "" ? true : d.Brand == brand) &&
                                //(prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                                //(fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                                (location == "" ? true : d.LocationStock == location)
                               select new LogFileViewModels
                               {
                                   ActionBy = d.RequestBy,
                                   ActionDate = d.DateRequest,
                                   DeviceType = d.DeviceType,
                                   Model = d.Model,
                                   Brand = d.Brand,
                                   SerialNumber = d.SerialNumber,
                                   PRNumber = "",
                                   FixAccess = "",
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
                               (from d in db.RecordSpares
                                join x in db.Devices on d.SerialNumber equals x.SerialNumber
                                where
                                (model == "" ? true : d.Model == model) &&
                                (type == "" ? true : d.DeviceType == type) &&
                                (brand == "" ? true : d.Brand == brand) &&
                                (prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                                (fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                                (location == "" ? true : d.LocationStock == location)

                                select new LogFileViewModels
                                {
                                    ActionBy = d.RequestBy,
                                    ActionDate = d.DateRequest,
                                    DeviceType = d.DeviceType,
                                    Model = d.Model,
                                    Brand = d.Brand,
                                    SerialNumber = d.SerialNumber,
                                    PRNumber = x.PRNumber,
                                    FixAccess = x.FixAccess,
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
                             (from d in db.RecordInRepairs
                              join x in db.Devices on d.SerialNumber equals x.SerialNumber
                              where
                            (model == "" ? true : d.Model == model) &&
                            (type == "" ? true : d.DeviceType == type) &&
                            (brand == "" ? true : d.Brand == brand) &&
                            //(prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                            //(fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                            (location == "" ? true : d.LocationStock == location)

                              select new LogFileViewModels
                              {
                                  ActionBy = d.RequestBy,
                                  ActionDate = d.DateRequest,
                                  DeviceType = d.DeviceType,
                                  Model = d.Model,
                                  Brand = d.Brand,
                                  SerialNumber = d.SerialNumber,
                                  PRNumber = "",
                                  FixAccess = "",
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
                           (from d in db.RecordRequisitions
                            //join x in db.Devices on d.SerialNumber equals x.SerialNumber
                            where
                            (model == "" ? true : d.Model == model) &&
                            (type == "" ? true : d.DeviceType == type) &&
                            (brand == "" ? true : d.Brand == brand) &&
                            //(prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                            //(fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                            (location == "" ? true : d.LocationStock == location)

                            select new LogFileViewModels
                            {
                                ActionBy = d.RequestBy,
                                ActionDate = d.DateRequisition,
                                DeviceType = d.DeviceType,
                                Model = d.Model,
                                Brand = d.Brand,
                                SerialNumber = d.SerialNumber,
                                PRNumber = "",
                                FixAccess = "",
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
                            }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            else if (!String.IsNullOrEmpty(status) && status == "11" && !String.IsNullOrEmpty(de) && String.IsNullOrEmpty(te))
            {
                DateTime start = DateTime.Parse(de);

                var repaired = (from d in db.RecordDevices
                                //join x in db.Devices on d.SerialNumber equals x.SerialNumber
                                where
                                 (model == "" ? true : d.Model == model) &&
                                 (type == "" ? true : d.Type == type) &&
                                 (brand == "" ? true : d.Brand == brand) &&
                                 (location == "" ? true : d.LocationStock == location) &&
                                 //(prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                                 //(fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                                 (de == "" ? true : EntityFunctions.TruncateTime(d.EditDate) == EntityFunctions.TruncateTime(start))
                                select new LogFileViewModels
                                {
                                    ActionBy = d.EditBy,
                                    ActionDate = d.EditDate,
                                    DeviceType = d.Type,
                                    Model = d.Model,
                                    Brand = d.Brand,
                                    SerialNumber = d.SerialNumber,
                                    PRNumber = "",
                                    FixAccess = "",
                                    Plant = d.Plant,
                                    Department = d.Department,
                                    Location = d.Location,
                                    Phase = d.Phase,
                                    Status = d.Description,
                                    LocationStock = d.LocationStock,
                                    Cause = null,
                                    Machine = d.Machine,
                                    DeviceName = d.DeviceName,
                                    UserName = d.UserName
                                }).Union
                            (from d in db.RecordInstocks
                             //join x in db.Devices on d.SerialNumber equals x.SerialNumber
                             where
                            (model == "" ? true : d.Model == model) &&
                            (type == "" ? true : d.DeviceType == type) &&
                            (brand == "" ? true : d.Brand == brand) &&
                            //(prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                            //(fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                            (location == "" ? true : d.LocationStock == location) &&
                            (de == "" ? true : EntityFunctions.TruncateTime(d.DateInstock) == EntityFunctions.TruncateTime(start))
                             select new LogFileViewModels
                             {
                                 ActionBy = d.InstockBy,
                                 ActionDate = d.DateInstock,
                                 DeviceType = d.DeviceType,
                                 Model = d.Model,
                                 Brand = d.Brand,
                                 SerialNumber = d.SerialNumber,
                                 PRNumber = "",
                                 FixAccess = "",
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
                             (from d in db.RecordReinstocks
                              //join x in db.Devices on d.SerialNumber equals x.SerialNumber
                              where
                              (model == "" ? true : d.Model == model) &&
                              (type == "" ? true : d.DeviceType == type) &&
                              (brand == "" ? true : d.Brand == brand) &&
                              //(prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                              //(fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                              (location == "" ? true : d.LocationStock == location) &&
                              (de == "" ? true : EntityFunctions.TruncateTime(d.DateRequest) == EntityFunctions.TruncateTime(start))

                              select new LogFileViewModels
                              {
                                  ActionBy = d.RequestBy,
                                  ActionDate = d.DateRequest,
                                  DeviceType = d.DeviceType,
                                  Model = d.Model,
                                  Brand = d.Brand,
                                  SerialNumber = d.SerialNumber,
                                  PRNumber = "",
                                  FixAccess = "",
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
                              (from d in db.RecordSales
                               //join x in db.Devices on d.SerialNumber equals x.SerialNumber
                               where
                                (model == "" ? true : d.Model == model) &&
                                (type == "" ? true : d.DeviceType == type) &&
                                (brand == "" ? true : d.Brand == brand) &&
                                //(prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                                //(fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                                (location == "" ? true : d.LocationStock == location) &&
                                (de == "" ? true : EntityFunctions.TruncateTime(d.DateRequest) == EntityFunctions.TruncateTime(start))

                               select new LogFileViewModels
                               {
                                   ActionBy = d.RequestBy,
                                   ActionDate = d.DateRequest,
                                   DeviceType = d.DeviceType,
                                   Model = d.Model,
                                   Brand = d.Brand,
                                   SerialNumber = d.SerialNumber,
                                   PRNumber = "",
                                   FixAccess = "",
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
                               (from d in db.RecordSpares
                                join x in db.Devices on d.SerialNumber equals x.SerialNumber
                                where
                                (model == "" ? true : d.Model == model) &&
                                (type == "" ? true : d.DeviceType == type) &&
                                (brand == "" ? true : d.Brand == brand) &&
                                (prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                                (fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                                (location == "" ? true : d.LocationStock == location)&&
                                (de == "" ? true : EntityFunctions.TruncateTime(d.DateRequest) == EntityFunctions.TruncateTime(start))

                                select new LogFileViewModels
                                {
                                    ActionBy = d.RequestBy,
                                    ActionDate = d.DateRequest,
                                    DeviceType = d.DeviceType,
                                    Model = d.Model,
                                    Brand = d.Brand,
                                    SerialNumber = d.SerialNumber,
                                    PRNumber = x.PRNumber,
                                    FixAccess = x.FixAccess,
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
                             (from d in db.RecordInRepairs
                              //join x in db.Devices on d.SerialNumber equals x.SerialNumber
                              where
                            (model == "" ? true : d.Model == model) &&
                            (type == "" ? true : d.DeviceType == type) &&
                            (brand == "" ? true : d.Brand == brand) &&
                            //(prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                            //(fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                            (location == "" ? true : d.LocationStock == location) &&
                            (de == "" ? true : EntityFunctions.TruncateTime(d.DateRequest) == EntityFunctions.TruncateTime(start))

                              select new LogFileViewModels
                              {
                                  ActionBy = d.RequestBy,
                                  ActionDate = d.DateRequest,
                                  DeviceType = d.DeviceType,
                                  Model = d.Model,
                                  Brand = d.Brand,
                                  SerialNumber = d.SerialNumber,
                                  PRNumber = "",
                                  FixAccess = "",
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
                           (from d in db.RecordRequisitions
                            //join x in db.Devices on d.SerialNumber equals x.SerialNumber
                            where
                            (model == "" ? true : d.Model == model) &&
                            (type == "" ? true : d.DeviceType == type) &&
                            (brand == "" ? true : d.Brand == brand) &&
                            //(prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                            //(fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                            (location == "" ? true : d.LocationStock == location) &&
                            (de == "" ? true : EntityFunctions.TruncateTime(d.DateRequisition) == EntityFunctions.TruncateTime(start))
                            select new LogFileViewModels
                            {
                                ActionBy = d.RequestBy,
                                ActionDate = d.DateRequisition,
                                DeviceType = d.DeviceType,
                                Model = d.Model,
                                Brand = d.Brand,
                                SerialNumber = d.SerialNumber,
                                PRNumber = "",
                                FixAccess = "",
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
                            }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            else if (!String.IsNullOrEmpty(status) && status == "11" && !String.IsNullOrEmpty(de) && !String.IsNullOrEmpty(te))
            {
                DateTime start = DateTime.Parse(de);
                DateTime end = DateTime.Parse(te);

                var repaired = (from d in db.RecordDevices
                //join x in db.Devices on d.SerialNumber equals x.SerialNumber
                                where
                                 (model == "" ? true : d.Model == model) &&
                                 (type == "" ? true : d.Type == type) &&
                                 (brand == "" ? true : d.Brand == brand) &&
                                 //(prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                                 //(fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                                 (location == "" ? true : d.LocationStock == location) &&
                                 (de == "" ? true : EntityFunctions.TruncateTime(d.EditDate) >= EntityFunctions.TruncateTime(start) && EntityFunctions.TruncateTime(d.EditDate) <= EntityFunctions.TruncateTime(end))

                                select new LogFileViewModels
                                {
                                    ActionBy = d.EditBy,
                                    ActionDate = d.EditDate,
                                    DeviceType = d.Type,
                                    Model = d.Model,
                                    Brand = d.Brand,
                                    SerialNumber = d.SerialNumber,
                                    PRNumber = "",
                                    FixAccess = "",
                                    Plant = d.Plant,
                                    Department = d.Department,
                                    Location = d.Location,
                                    Phase = d.Phase,
                                    Status = d.Description,
                                    LocationStock = d.LocationStock,
                                    Cause = null,
                                    Machine = d.Machine,
                                    DeviceName = d.DeviceName,
                                    UserName = d.UserName
                                }).Union
                            (from d in db.RecordInstocks
                             //join x in db.Devices on d.SerialNumber equals x.SerialNumber
                             where
                            (model == "" ? true : d.Model == model) &&
                            (type == "" ? true : d.DeviceType == type) &&
                            (brand == "" ? true : d.Brand == brand) &&
                            //(prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                            //(fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                            (location == "" ? true : d.LocationStock == location) &&
                            (de == "" ? true : EntityFunctions.TruncateTime(d.DateInstock) >= EntityFunctions.TruncateTime(start) && EntityFunctions.TruncateTime(d.DateInstock) <= EntityFunctions.TruncateTime(end))

                             select new LogFileViewModels
                             {
                                 ActionBy = d.InstockBy,
                                 ActionDate = d.DateInstock,
                                 DeviceType = d.DeviceType,
                                 Model = d.Model,
                                 Brand = d.Brand,
                                 SerialNumber = d.SerialNumber,
                                 PRNumber = "",
                                 FixAccess = "",
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
                             (from d in db.RecordReinstocks
                              //join x in db.Devices on d.SerialNumber equals x.SerialNumber
                              where
                              (model == "" ? true : d.Model == model) &&
                              (type == "" ? true : d.DeviceType == type) &&
                              (brand == "" ? true : d.Brand == brand) &&
                              //(prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                              //(fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                              (location == "" ? true : d.LocationStock == location) &&
                              (de == "" ? true : EntityFunctions.TruncateTime(d.DateRequest) >= EntityFunctions.TruncateTime(start) && EntityFunctions.TruncateTime(d.DateRequest) <= EntityFunctions.TruncateTime(end))

                              select new LogFileViewModels
                              {
                                  ActionBy = d.RequestBy,
                                  ActionDate = d.DateRequest,
                                  DeviceType = d.DeviceType,
                                  Model = d.Model,
                                  Brand = d.Brand,
                                  SerialNumber = d.SerialNumber,
                                  PRNumber = "",
                                  FixAccess = "",
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
                              (from d in db.RecordSales
                               //join x in db.Devices on d.SerialNumber equals x.SerialNumber
                               where
                                (model == "" ? true : d.Model == model) &&
                                (type == "" ? true : d.DeviceType == type) &&
                                (brand == "" ? true : d.Brand == brand) &&
                                //(prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                                //(fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                                (location == "" ? true : d.LocationStock == location) &&
                                (de == "" ? true : EntityFunctions.TruncateTime(d.DateRequest) >= EntityFunctions.TruncateTime(start) && EntityFunctions.TruncateTime(d.DateRequest) <= EntityFunctions.TruncateTime(end))

                               select new LogFileViewModels
                               {
                                   ActionBy = d.RequestBy,
                                   ActionDate = d.DateRequest,
                                   DeviceType = d.DeviceType,
                                   Model = d.Model,
                                   Brand = d.Brand,
                                   SerialNumber = d.SerialNumber,
                                   PRNumber = "",
                                   FixAccess = "",
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
                               (from d in db.RecordSpares
                                join x in db.Devices on d.SerialNumber equals x.SerialNumber
                                where
                                (model == "" ? true : d.Model == model) &&
                                (type == "" ? true : d.DeviceType == type) &&
                                (brand == "" ? true : d.Brand == brand) &&
                                (prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                                (fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                                (location == "" ? true : d.LocationStock == location) &&
                                (de == "" ? true : EntityFunctions.TruncateTime(d.DateRequest) >= EntityFunctions.TruncateTime(start) && EntityFunctions.TruncateTime(d.DateRequest) <= EntityFunctions.TruncateTime(end))

                                select new LogFileViewModels
                                {
                                    ActionBy = d.RequestBy,
                                    ActionDate = d.DateRequest,
                                    DeviceType = d.DeviceType,
                                    Model = d.Model,
                                    Brand = d.Brand,
                                    SerialNumber = d.SerialNumber,
                                    PRNumber = x.PRNumber,
                                    FixAccess = x.FixAccess,
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
                             (from d in db.RecordInRepairs
                              ///join x in db.Devices on d.SerialNumber equals x.SerialNumber
                              where
                            (model == "" ? true : d.Model == model) &&
                            (type == "" ? true : d.DeviceType == type) &&
                            (brand == "" ? true : d.Brand == brand) &&
                            //(prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                            //(fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                            (location == "" ? true : d.LocationStock == location) &&
                            (de == "" ? true : EntityFunctions.TruncateTime(d.DateRequest) >= EntityFunctions.TruncateTime(start) && EntityFunctions.TruncateTime(d.DateRequest) <= EntityFunctions.TruncateTime(end))

                              select new LogFileViewModels
                              {
                                  ActionBy = d.RequestBy,
                                  ActionDate = d.DateRequest,
                                  DeviceType = d.DeviceType,
                                  Model = d.Model,
                                  Brand = d.Brand,
                                  SerialNumber = d.SerialNumber,
                                  PRNumber = "",
                                  FixAccess = "",
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
                           (from d in db.RecordRequisitions
                            //join x in db.Devices on d.SerialNumber equals x.SerialNumber
                            where
                            (model == "" ? true : d.Model == model) &&
                            (type == "" ? true : d.DeviceType == type) &&
                            (brand == "" ? true : d.Brand == brand) &&
                            //(prnumber == "No" || prnumber == "" ? x.PRNumber == null : x.PRNumber != null) &&
                            //(fixaccess == "No" || fixaccess == "" ? x.FixAccess == null : x.FixAccess != null) &&
                            (location == "" ? true : d.LocationStock == location) &&
                            (de == "" ? true : EntityFunctions.TruncateTime(d.DateRequisition) >= EntityFunctions.TruncateTime(start) && EntityFunctions.TruncateTime(d.DateRequisition) <= EntityFunctions.TruncateTime(end))
                            select new LogFileViewModels
                            {
                                ActionBy = d.RequestBy,
                                ActionDate = d.DateRequisition,
                                DeviceType = d.DeviceType,
                                Model = d.Model,
                                Brand = d.Brand,
                                SerialNumber = d.SerialNumber,
                                PRNumber = "",
                                FixAccess = "",
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
                            }).ToList().OrderBy(d => d.ActionDate);
                return Json(new { data = repaired }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            var data = (from device in db.Devices where device.Status.StatusID == 0 group device by device.DeviceID into dv let m = dv.FirstOrDefault() select m).OrderBy(p => p.DeviceID).ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult byDate()
        {
            //ReportViewModels reportviewmodels = new ReportViewModels();
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "Type", "Type");
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult byDate([Bind(Include = "StartDate,EndDate,Format,StatusID,DeviceTypeID")] LogViewModels logViewModel)
        {
            if (ModelState.IsValid)
            {
                if (logViewModel.StatusID == 1)
                {
                    return RedirectToAction("exportDataInUseByDate", "PrintLog", logViewModel);
                }
                else if (logViewModel.StatusID == 2)
                {
                    return RedirectToAction("exportDataInRepairByDate", "PrintLog", logViewModel);
                }
                else if(logViewModel.StatusID == 4)
                {
                    return RedirectToAction("exportDataInSaleByDate", "PrintLog", logViewModel);
                }
                else if (logViewModel.StatusID == 5)
                {
                    return RedirectToAction("exportDataSpareByDate", "PrintLog", logViewModel);
                }
                else if(logViewModel.StatusID == 6)
                {
                    return RedirectToAction("exportDataSentRepairByDate", "PrintLog", logViewModel);
                }
                else if (logViewModel.StatusID == 7)
                {
                    return RedirectToAction("exportDataSentSaleByDate", "PrintLog", logViewModel);
                }
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/Reports"), "RptInstock.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return View("Index");
                }
#region Status3
                if (logViewModel.StatusID == 3 && logViewModel.DeviceTypeID != null)
                {
                    List<RecordInstock> cm = new List<RecordInstock>();

                    if (logViewModel.EndDate > logViewModel.StartDate)
                    {
                        {
                            var query = from s in db.RecordInstocks
                                        orderby s.DateInstock
                                        where (EntityFunctions.TruncateTime(s.DateInstock) >= EntityFunctions.TruncateTime(logViewModel.StartDate) && s.DeviceType == logViewModel.DeviceTypeID && EntityFunctions.TruncateTime(s.DateInstock) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.DeviceType == logViewModel.DeviceTypeID)
                                        select s;
                            cm = query.ToList();
                        }
                    }
                    else
                    {
                        var query = from s in db.RecordInstocks
                                    orderby s.DateInstock
                                    where (EntityFunctions.TruncateTime(s.DateInstock) == EntityFunctions.TruncateTime(logViewModel.StartDate) && s.DeviceType == logViewModel.DeviceTypeID && EntityFunctions.TruncateTime(s.DateInstock) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.DeviceType == logViewModel.DeviceTypeID)
                                    select s;
                        cm = query.ToList();
                    }


                    ReportDataSource rd = new ReportDataSource("DataSet", cm);
                    lr.DataSources.Add(rd);
                    string reportType = logViewModel.Format;
                    string mimeType;
                    string encoding;
                    string fileNameExtension;



                    string deviceInfo =

                    "<DeviceInfo>" +
                    "  <OutputFormat>" + logViewModel.Format + "</OutputFormat>" +
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
#endregion
                else if(logViewModel.StatusID == 3 && logViewModel.DeviceTypeID == null)
                {
                    List<RecordInstock> cm = new List<RecordInstock>();

                    if (logViewModel.EndDate > logViewModel.StartDate)
                    {
                        {
                            var query = from s in db.RecordInstocks
                                        orderby s.DateInstock
                                        where (EntityFunctions.TruncateTime(s.DateInstock) >= EntityFunctions.TruncateTime(logViewModel.StartDate) && EntityFunctions.TruncateTime(s.DateInstock) <= EntityFunctions.TruncateTime(logViewModel.EndDate))
                                        select s;
                            cm = query.ToList();
                        }
                    }
                    else
                    {
                        var query = from s in db.RecordInstocks
                                    orderby s.DateInstock
                                    where (EntityFunctions.TruncateTime(s.DateInstock) == EntityFunctions.TruncateTime(logViewModel.StartDate) && EntityFunctions.TruncateTime(s.DateInstock) <= EntityFunctions.TruncateTime(logViewModel.EndDate))
                                    select s;
                        cm = query.ToList();
                    }


                    ReportDataSource rd = new ReportDataSource("DataSet", cm);
                    lr.DataSources.Add(rd);
                    string reportType = logViewModel.Format;
                    string mimeType;
                    string encoding;
                    string fileNameExtension;



                    string deviceInfo =

                    "<DeviceInfo>" +
                    "  <OutputFormat>" + logViewModel.Format + "</OutputFormat>" +
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
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "Type", "Type");
            return View(logViewModel);
        }

        public ActionResult ReportCartridgePrinter()
        {
            return View();
        }

        public ActionResult CartridgeRequisitionReport()
        {
            return View();
        }

        public ActionResult AccessoriesRequisitionReport()
        {
            return View();
        }

        public ActionResult loadDataCartridgeRequisition()
        {
            using (var dc = new ITStockEntities1())
            {
                dc.Configuration.ProxyCreationEnabled = false;
                var data = dc.RecordRequisitions.Where(a => a.DeviceType == "Cartridge Printer").OrderBy(a => a.DateRequisition).ToList();
                return Json(new { data = data }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult loadDataAccessoriesRequisition()
        {
            using (var dc = new ITStockEntities1())
            {
                dc.Configuration.ProxyCreationEnabled = false;
                var data = dc.RecordRequisitions.Where(a => a.DeviceType.Substring(0,11) == "Accessories").OrderBy(a => a.DateRequisition).ToList();
                return Json(new { data = data }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult loadDataScannerInrepair()
        {
            using (var dc = new ITStockEntities1())
            {
                dc.Configuration.ProxyCreationEnabled = false;
                var data = dc.RecordRequisitions.Where(a => a.Status.Substring(14, 1) == "P").OrderBy(a => a.DateRequisition).ToList();
                return Json(new { data = data }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult requisitionAssetReport()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult requisitionAssetReport([Bind(Include = "StartDate,EndDate")] AssetReport viewmodels)
        {
            if (ModelState.IsValid)
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/Reports"), "AssetReport.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return View("Index");
                }
                    List<RecordRequisition> cm = new List<RecordRequisition>();

                    if (viewmodels.EndDate > viewmodels.StartDate)
                    {
                        {
                            var query = from s in db.RecordRequisitions
                                        orderby s.RequisitionID
                                        where (EntityFunctions.TruncateTime(s.DateRequisition) >= EntityFunctions.TruncateTime(viewmodels.StartDate) && s.IsFixAsset == "Asset" && EntityFunctions.TruncateTime(s.DateRequisition) <= EntityFunctions.TruncateTime(viewmodels.EndDate) && s.IsFixAsset == "Asset")
                                        select s;
                            cm = query.ToList();
                        }
                    }
                    else
                    {
                        var query = from s in db.RecordRequisitions
                                    orderby s.RequisitionID
                                    where (EntityFunctions.TruncateTime(s.DateRequisition) == EntityFunctions.TruncateTime(viewmodels.StartDate) && s.IsFixAsset == "Asset" && EntityFunctions.TruncateTime(s.DateRequisition) <= EntityFunctions.TruncateTime(viewmodels.EndDate) && s.IsFixAsset == "Asset")
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
            return View(viewmodels);
        }

        public ActionResult byLocationStock()
        {
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1");
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult byLocationStock([Bind(Include = "StatusID,Status,LocationStockID")] FindDeviceViewModels finddeviceviewmodels)
        {
            if (string.IsNullOrEmpty(finddeviceviewmodels.StatusID.ToString()))
            {
                ModelState.AddModelError("StatusID", "StatusID is Required");
            }
            if (ModelState.IsValid)
            {
                return RedirectToAction("filterByType", "PrintLog", new { id = finddeviceviewmodels.StatusID, lid = finddeviceviewmodels.LocationStockID });
            }
            ModelState.AddModelError("SerialNumber", "Current status not ready to ReLocation");
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", finddeviceviewmodels.StatusID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", finddeviceviewmodels.LocationStockID);
            return View();
        }

        public ActionResult filterByType(int? id, int? lid)
        {
            if (lid == null)
            {
                var devices = from device in db.Devices where device.StatusID == id group device by device.DeviceTypeID into dv let m = dv.FirstOrDefault() select m;
                return View(devices);
            }
            else
            {
                var devices = from device in db.Devices where device.StatusID == id && device.LocationStockID == lid group device by device.DeviceTypeID into dv let m = dv.FirstOrDefault() select m;
                return View(devices);
            }
        }

        public ActionResult filterByModel(int? sid, int? lid, int? tid)
        {
            var devices = from device in db.Devices where device.StatusID == sid && device.LocationStockID == lid && device.DeviceTypeID == tid group device by device.ModelID into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        public ActionResult filterByItem(int? sid, int? lid, int? tid, int? mid, int? req)
        {
            var devices = from device in db.Devices where device.StatusID == sid && device.LocationStockID == lid && device.DeviceTypeID == tid && device.ModelID == mid group device by device.DeviceID into dv let m = dv.FirstOrDefault() select m;
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName");
            ViewBag.Sid = sid;
            ViewBag.Lid = lid;
            ViewBag.Tid = tid;
            ViewBag.Mid = mid;
            if(req != null)
            {
                ViewBag.Required = "Active";
            }
            return View(devices);
        }

        public ActionResult criteriaByModel(ReportViewModels reportviewmodels)
        {
            var devices = from device in db.Devices where device.StatusID == reportviewmodels.StatusID group device by device.ModelID into dv let m = dv.FirstOrDefault() select m;
            ViewBag.StatusID = reportviewmodels.StatusID;
            return View(devices);
        }

        public ActionResult criteriaByBrand(ReportViewModels reportviewmodels)
        {
            var devices = from device in db.Devices where device.StatusID == reportviewmodels.StatusID group device by device.BrandID into dv let m = dv.FirstOrDefault() select m;
            ViewBag.StatusID = reportviewmodels.StatusID;
            return View(devices);
        }

        public ActionResult criteriaByLocation(ReportViewModels reportviewmodels)
        {
            ViewBag.StatusID = reportviewmodels.StatusID;
            return View(db.Plants.ToList());
        }

        public ActionResult exportDataInUseByDate([Bind(Include = "StartDate,EndDate,Format,StatusID,DeviceTypeID")] LogViewModels logViewModel)
        {
            if (ModelState.IsValid)
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/Reports"), "RptInUse.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return View("Index");
                }

                if(logViewModel.DeviceTypeID == null)
                {
                    List<RecordRequisition> cm = new List<RecordRequisition>();

                    if (logViewModel.EndDate > logViewModel.StartDate)
                    {
                        {
                            var query = from s in db.RecordRequisitions
                                        orderby s.RequisitionID
                                        where (EntityFunctions.TruncateTime(s.DateRequisition) >= EntityFunctions.TruncateTime(logViewModel.StartDate) && EntityFunctions.TruncateTime(s.DateRequisition) <= EntityFunctions.TruncateTime(logViewModel.EndDate))
                                        select s;
                            cm = query.ToList();
                        }
                    }
                    else
                    {
                        var query = from s in db.RecordRequisitions
                                    orderby s.RequisitionID
                                    where (EntityFunctions.TruncateTime(s.DateRequisition) == EntityFunctions.TruncateTime(logViewModel.StartDate) && EntityFunctions.TruncateTime(s.DateRequisition) <= EntityFunctions.TruncateTime(logViewModel.EndDate))
                                    select s;
                        cm = query.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DataSet", cm);
                    lr.DataSources.Add(rd);
                    string reportType = logViewModel.Format;
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                    "  <OutputFormat>" + logViewModel.Format + "</OutputFormat>" +
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
                else if (logViewModel.DeviceTypeID != null)
                {
                    List<RecordRequisition> cm = new List<RecordRequisition>();

                    if (logViewModel.EndDate > logViewModel.StartDate)
                    {
                        {
                            var query = from s in db.RecordRequisitions
                                        orderby s.RequisitionID
                                        where (EntityFunctions.TruncateTime(s.DateRequisition) >= EntityFunctions.TruncateTime(logViewModel.StartDate) && s.DeviceType == logViewModel.DeviceTypeID && EntityFunctions.TruncateTime(s.DateRequisition) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.DeviceType == logViewModel.DeviceTypeID)
                                        select s;
                            cm = query.ToList();
                        }
                    }
                    else
                    {
                        var query = from s in db.RecordRequisitions
                                    orderby s.RequisitionID
                                    where (EntityFunctions.TruncateTime(s.DateRequisition) == EntityFunctions.TruncateTime(logViewModel.StartDate) && s.DeviceType == logViewModel.DeviceTypeID && EntityFunctions.TruncateTime(s.DateRequisition) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.DeviceType == logViewModel.DeviceTypeID)
                                    select s;
                        cm = query.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DataSet", cm);
                    lr.DataSources.Add(rd);
                    string reportType = logViewModel.Format;
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                    "  <OutputFormat>" + logViewModel.Format + "</OutputFormat>" +
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
            return View(logViewModel);
        }

        public ActionResult exportDataInRepairByDate([Bind(Include = "StartDate,EndDate,Format,StatusID,DeviceTypeID")] LogViewModels logViewModel)
        {
            if (ModelState.IsValid)
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/Reports"), "RptInRepair.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return View("Index");
                }
                if (logViewModel.DeviceTypeID == null)
                {
                    List<RecordInRepair> cm = new List<RecordInRepair>();

                    if (logViewModel.EndDate > logViewModel.StartDate)
                    {
                        {
                            var query = from s in db.RecordInRepairs
                                        orderby s.InRepairID
                                        where (EntityFunctions.TruncateTime(s.DateRequest) >= EntityFunctions.TruncateTime(logViewModel.StartDate) && s.Status == "In Repair" && EntityFunctions.TruncateTime(s.DateRequest) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.Status == "In Repair")
                                        select s;
                            cm = query.ToList();
                        }
                    }
                    else
                    {
                        var query = from s in db.RecordInRepairs
                                    orderby s.InRepairID
                                    where (EntityFunctions.TruncateTime(s.DateRequest) == EntityFunctions.TruncateTime(logViewModel.StartDate) && s.Status == "In Repair" && EntityFunctions.TruncateTime(s.DateRequest) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.Status == "In Repair")
                                    select s;
                        cm = query.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DataSet", cm);
                    lr.DataSources.Add(rd);
                    string reportType = logViewModel.Format;
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                    "  <OutputFormat>" + logViewModel.Format + "</OutputFormat>" +
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
                else if (logViewModel.DeviceTypeID != null)
                {
                    List<RecordInRepair> cm = new List<RecordInRepair>();

                    if (logViewModel.EndDate > logViewModel.StartDate)
                    {
                        {
                            var query = from s in db.RecordInRepairs
                                        orderby s.InRepairID
                                        where (EntityFunctions.TruncateTime(s.DateRequest) >= EntityFunctions.TruncateTime(logViewModel.StartDate) && s.Status == "In Repair" && s.DeviceType == logViewModel.DeviceTypeID && EntityFunctions.TruncateTime(s.DateRequest) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.Status == "In Repair" && s.DeviceType == logViewModel.DeviceTypeID)
                                        select s;
                            cm = query.ToList();
                        }
                    }
                    else
                    {
                        var query = from s in db.RecordInRepairs
                                    orderby s.InRepairID
                                    where (EntityFunctions.TruncateTime(s.DateRequest) == EntityFunctions.TruncateTime(logViewModel.StartDate) && s.Status == "In Repair" && s.DeviceType == logViewModel.DeviceTypeID && EntityFunctions.TruncateTime(s.DateRequest) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.Status == "In Repair" && s.DeviceType == logViewModel.DeviceTypeID)
                                    select s;
                        cm = query.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DataSet", cm);
                    lr.DataSources.Add(rd);
                    string reportType = logViewModel.Format;
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                    "  <OutputFormat>" + logViewModel.Format + "</OutputFormat>" +
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
            return View(logViewModel);
        }

        public ActionResult exportDataInSaleByDate([Bind(Include = "StartDate,EndDate,Format,StatusID,DeviceTypeID")] LogViewModels logViewModel)
        {
            if (ModelState.IsValid)
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/Reports"), "RptInSale.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return View("Index");
                }
                if (logViewModel.DeviceTypeID == null)
                {
                    List<RecordSale> cm = new List<RecordSale>();

                    if (logViewModel.EndDate > logViewModel.StartDate)
                    {
                        {
                            var query = from s in db.RecordSales
                                        orderby s.SaleID
                                        where (EntityFunctions.TruncateTime(s.DateRequest) >= EntityFunctions.TruncateTime(logViewModel.StartDate) && s.Status == "In Sale" && EntityFunctions.TruncateTime(s.DateRequest) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.Status == "In Sale")
                                        select s;
                            cm = query.ToList();
                        }
                    }
                    else
                    {
                        var query = from s in db.RecordSales
                                    orderby s.SaleID
                                    where (EntityFunctions.TruncateTime(s.DateRequest) == EntityFunctions.TruncateTime(logViewModel.StartDate) && s.Status == "In Sale" && EntityFunctions.TruncateTime(s.DateRequest) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.Status == "In Sale")
                                    select s;
                        cm = query.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DataSet", cm);
                    lr.DataSources.Add(rd);
                    string reportType = logViewModel.Format;
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                    "  <OutputFormat>" + logViewModel.Format + "</OutputFormat>" +
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
                else if (logViewModel.DeviceTypeID != null)
                {
                    List<RecordSale> cm = new List<RecordSale>();

                    if (logViewModel.EndDate > logViewModel.StartDate)
                    {
                        {
                            var query = from s in db.RecordSales
                                        orderby s.SaleID
                                        where (EntityFunctions.TruncateTime(s.DateRequest) >= EntityFunctions.TruncateTime(logViewModel.StartDate) && s.Status == "In Sale" && s.DeviceType == logViewModel.DeviceTypeID && EntityFunctions.TruncateTime(s.DateRequest) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.Status == "In Sale" && s.DeviceType == logViewModel.DeviceTypeID)
                                        select s;
                            cm = query.ToList();
                        }
                    }
                    else
                    {
                        var query = from s in db.RecordSales
                                    orderby s.SaleID
                                    where (EntityFunctions.TruncateTime(s.DateRequest) == EntityFunctions.TruncateTime(logViewModel.StartDate) && s.Status == "In Sale" && s.DeviceType == logViewModel.DeviceTypeID && EntityFunctions.TruncateTime(s.DateRequest) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.Status == "In Sale" && s.DeviceType == logViewModel.DeviceTypeID)
                                    select s;
                        cm = query.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DataSet", cm);
                    lr.DataSources.Add(rd);
                    string reportType = logViewModel.Format;
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                    "  <OutputFormat>" + logViewModel.Format + "</OutputFormat>" +
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
            return View(logViewModel);
        }

        public ActionResult exportDataSpareByDate([Bind(Include = "StartDate,EndDate,Format,StatusID,DeviceTypeID")] LogViewModels logViewModel)
        {
            if (ModelState.IsValid)
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/Reports"), "RptSpare.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return View("Index");
                }
                if (logViewModel.DeviceTypeID == null)
                {
                    List<RecordSpare> cm = new List<RecordSpare>();

                    if (logViewModel.EndDate > logViewModel.StartDate)
                    {
                        {
                            var query = from s in db.RecordSpares
                                        orderby s.SpareID
                                        where (EntityFunctions.TruncateTime(s.DateRequest) >= EntityFunctions.TruncateTime(logViewModel.StartDate) && EntityFunctions.TruncateTime(s.DateRequest) <= EntityFunctions.TruncateTime(logViewModel.EndDate))
                                        select s;
                            cm = query.ToList();
                        }
                    }
                    else
                    {
                        var query = from s in db.RecordSpares
                                    orderby s.SpareID
                                    where (EntityFunctions.TruncateTime(s.DateRequest) == EntityFunctions.TruncateTime(logViewModel.StartDate) && EntityFunctions.TruncateTime(s.DateRequest) <= EntityFunctions.TruncateTime(logViewModel.EndDate))
                                    select s;
                        cm = query.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DataSet", cm);
                    lr.DataSources.Add(rd);
                    string reportType = logViewModel.Format;
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                    "  <OutputFormat>" + logViewModel.Format + "</OutputFormat>" +
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
                else if (logViewModel.DeviceTypeID != null)
                {
                    List<RecordSpare> cm = new List<RecordSpare>();

                    if (logViewModel.EndDate > logViewModel.StartDate)
                    {
                        {
                            var query = from s in db.RecordSpares
                                        orderby s.SpareID
                                        where (EntityFunctions.TruncateTime(s.DateRequest) >= EntityFunctions.TruncateTime(logViewModel.StartDate) && s.DeviceType == logViewModel.DeviceTypeID && EntityFunctions.TruncateTime(s.DateRequest) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.DeviceType == logViewModel.DeviceTypeID)
                                        select s;
                            cm = query.ToList();
                        }
                    }
                    else
                    {
                        var query = from s in db.RecordSpares
                                    orderby s.SpareID
                                    where (EntityFunctions.TruncateTime(s.DateRequest) == EntityFunctions.TruncateTime(logViewModel.StartDate) && s.DeviceType == logViewModel.DeviceTypeID && EntityFunctions.TruncateTime(s.DateRequest) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.DeviceType == logViewModel.DeviceTypeID)
                                    select s;
                        cm = query.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DataSet", cm);
                    lr.DataSources.Add(rd);
                    string reportType = logViewModel.Format;
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                    "  <OutputFormat>" + logViewModel.Format + "</OutputFormat>" +
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
            return View(logViewModel);
        }

        public ActionResult exportDataSentRepairByDate([Bind(Include = "StartDate,EndDate,Format,StatusID,DeviceTypeID")] LogViewModels logViewModel)
        {
            if (ModelState.IsValid)
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/Reports"), "RptInRepair.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return View("Index");
                }
                if (logViewModel.DeviceTypeID == null)
                {
                    List<RecordInRepair> cm = new List<RecordInRepair>();

                    if (logViewModel.EndDate > logViewModel.StartDate)
                    {
                        {
                            var query = from s in db.RecordInRepairs
                                        orderby s.InRepairID
                                        where (EntityFunctions.TruncateTime(s.DateRequest) >= EntityFunctions.TruncateTime(logViewModel.StartDate) && s.Status == "Sent Repair" && EntityFunctions.TruncateTime(s.DateRequest) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.Status == "Sent Repair")
                                        select s;
                            cm = query.ToList();
                        }
                    }
                    else
                    {
                        var query = from s in db.RecordInRepairs
                                    orderby s.InRepairID
                                    where (EntityFunctions.TruncateTime(s.DateRequest) == EntityFunctions.TruncateTime(logViewModel.StartDate) && s.Status == "Sent Repair" && EntityFunctions.TruncateTime(s.DateRequest) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.Status == "Sent Repair")
                                    select s;
                        cm = query.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DataSet", cm);
                    lr.DataSources.Add(rd);
                    string reportType = logViewModel.Format;
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                    "  <OutputFormat>" + logViewModel.Format + "</OutputFormat>" +
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
                else if (logViewModel.DeviceTypeID != null)
                {
                    List<RecordInRepair> cm = new List<RecordInRepair>();

                    if (logViewModel.EndDate > logViewModel.StartDate)
                    {
                        {
                            var query = from s in db.RecordInRepairs
                                        orderby s.InRepairID
                                        where (EntityFunctions.TruncateTime(s.DateRequest) >= EntityFunctions.TruncateTime(logViewModel.StartDate) && s.Status == "Sent Repair" && s.DeviceType == logViewModel.DeviceTypeID && EntityFunctions.TruncateTime(s.DateRequest) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.Status == "Sent Repair" && s.DeviceType == logViewModel.DeviceTypeID)
                                        select s;
                            cm = query.ToList();
                        }
                    }
                    else
                    {
                        var query = from s in db.RecordInRepairs
                                    orderby s.InRepairID
                                    where (EntityFunctions.TruncateTime(s.DateRequest) == EntityFunctions.TruncateTime(logViewModel.StartDate) && s.Status == "Sent Repair" && s.DeviceType == logViewModel.DeviceTypeID && EntityFunctions.TruncateTime(s.DateRequest) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.Status == "Sent Repair" && s.DeviceType == logViewModel.DeviceTypeID)
                                    select s;
                        cm = query.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DataSet", cm);
                    lr.DataSources.Add(rd);
                    string reportType = logViewModel.Format;
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                    "  <OutputFormat>" + logViewModel.Format + "</OutputFormat>" +
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
            return View(logViewModel);
        }

        public ActionResult exportDataSentSaleByDate([Bind(Include = "StartDate,EndDate,Format,StatusID,DeviceTypeID")] LogViewModels logViewModel)
        {
            if (ModelState.IsValid)
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/Reports"), "RptInSale.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return View("Index");
                }
                if (logViewModel.DeviceTypeID == null)
                {
                    List<RecordSale> cm = new List<RecordSale>();

                    if (logViewModel.EndDate > logViewModel.StartDate)
                    {
                        {
                            var query = from s in db.RecordSales
                                        orderby s.SaleID
                                        where (EntityFunctions.TruncateTime(s.DateRequest) >= EntityFunctions.TruncateTime(logViewModel.StartDate) && s.Status == "Sent Sale" && EntityFunctions.TruncateTime(s.DateRequest) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.Status == "Sent Sale")
                                        select s;
                            cm = query.ToList();
                        }
                    }
                    else
                    {
                        var query = from s in db.RecordSales
                                    orderby s.SaleID
                                    where (EntityFunctions.TruncateTime(s.DateRequest) == EntityFunctions.TruncateTime(logViewModel.StartDate) && s.Status == "Sent Sale" && EntityFunctions.TruncateTime(s.DateRequest) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.Status == "Sent Sale")
                                    select s;
                        cm = query.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DataSet", cm);
                    lr.DataSources.Add(rd);
                    string reportType = logViewModel.Format;
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                    "  <OutputFormat>" + logViewModel.Format + "</OutputFormat>" +
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
                else if (logViewModel.DeviceTypeID != null)
                {
                    List<RecordSale> cm = new List<RecordSale>();

                    if (logViewModel.EndDate > logViewModel.StartDate)
                    {
                        {
                            var query = from s in db.RecordSales
                                        orderby s.SaleID
                                        where (EntityFunctions.TruncateTime(s.DateRequest) >= EntityFunctions.TruncateTime(logViewModel.StartDate) && s.Status == "Sent Sale" && s.DeviceType == logViewModel.DeviceTypeID && EntityFunctions.TruncateTime(s.DateRequest) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.Status == "Sent Sale" && s.DeviceType == logViewModel.DeviceTypeID)
                                        select s;
                            cm = query.ToList();
                        }
                    }
                    else
                    {
                        var query = from s in db.RecordSales
                                    orderby s.SaleID
                                    where (EntityFunctions.TruncateTime(s.DateRequest) == EntityFunctions.TruncateTime(logViewModel.StartDate) && s.Status == "Sent Sale" && s.DeviceType == logViewModel.DeviceTypeID && EntityFunctions.TruncateTime(s.DateRequest) <= EntityFunctions.TruncateTime(logViewModel.EndDate) && s.Status == "Sent Sale" && s.DeviceType == logViewModel.DeviceTypeID)
                                    select s;
                        cm = query.ToList();
                    }

                    ReportDataSource rd = new ReportDataSource("DataSet", cm);
                    lr.DataSources.Add(rd);
                    string reportType = logViewModel.Format;
                    string mimeType;
                    string encoding;
                    string fileNameExtension;

                    string deviceInfo =

                    "<DeviceInfo>" +
                    "  <OutputFormat>" + logViewModel.Format + "</OutputFormat>" +
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
            return View(logViewModel);
        }

        public ActionResult InstockPrintByBrand()
        {
            var devices = from device in db.Devices where device.StatusID == 3 group device by device.BrandName into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        public ActionResult InstockPrintByLocation()
        {
            return View(db.Plants.ToList());
        }

        public ActionResult InUsePrintByLocation()
        {
            return View(db.Plants.ToList());
        }

        public ActionResult InRepairPrintByLocation()
        {
            return View(db.Plants.ToList());
        }

        public ActionResult listDataByDepartment(int? id,int? sid)
        {
            var devices = from device in db.Devices where device.StatusID == sid && device.PlantID == id group device by device.Department.DepartmentName into dv let m = dv.FirstOrDefault() select m;
            ViewBag.StatusID = sid;
            return View(devices);
        }

        public ActionResult listDataByLocation(int? did, int? pid, int? sid)
        {
            var devices = from device in db.Devices where device.DepartmentID == did && device.StatusID == 1 && device.PlantID == pid group device by device.Location.LocationID into dv let m = dv.FirstOrDefault() select m;
            ViewBag.StatusID = sid;
            return View(devices);
        }

        public ActionResult listDataByPhase(int? did, int? pid, int? lid, int? sid)
        {
            var devices = from device in db.Devices where device.LocationID == lid && device.DepartmentID == did && device.StatusID == sid && device.PlantID == pid group device by device.PhaseID into dv let m = dv.FirstOrDefault() select m;
            ViewBag.StatusID = sid;
            return View(devices);
        }

        public ActionResult listDataByMachine(int? did, int? pid, int? lid, int? phid, int? sid)
        {
            var devices = from device in db.Devices where device.LocationID == lid && device.DepartmentID == did && device.StatusID == sid && device.PlantID == pid && device.PhaseID == phid group device by device.DeviceID into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        public ActionResult listDataByCriteriaModel(int?id, int? sid)
        {
            var devices = db.Devices.Where(d=>d.ModelID == id && d.StatusID == sid).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            return View(devices.ToList());
        }

        public ActionResult listDataByCriteriaBrand(int? id, int? sid)
        {
            var devices = db.Devices.Where(d => d.BrandID == id && d.StatusID == sid).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            return View(devices.ToList());
        }

        public ActionResult ExportData(int? pid, int?did, int? lid, int?phid, int?mid, int?id, int? sid)
        {
            if (ModelState.IsValid)
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/Reports"), "InstockByLocationReport.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return View("Index");
                }

                if(pid == null && did == null && lid == null && phid == null && mid == null && id != null)
                {
                    List<Device> cm = new List<Device>();
                cm = db.Devices.Where(d => d.DeviceID == id && d.StatusID == sid).ToList();

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
                else if (pid == null && did == null && lid == null && phid == null && mid != null && id == null)
                {
                    List<Device> cm = new List<Device>();
                    cm = db.Devices.Where(d => d.MachineID == mid && d.StatusID == sid).ToList();

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
                else if (pid != null && did != null && phid != null && lid != null && mid == null && id == null)
                {
                    List<Device> cm = new List<Device>();
                    cm = db.Devices.Where(d => d.PhaseID == phid && d.PlantID == pid && d.DepartmentID == did && d.LocationID == lid && d.StatusID == sid).ToList();

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
                else if(pid != null && did != null && phid == null && mid == null && id == null && lid != null)
                {
                    List<Device> cm = new List<Device>();
                    cm = db.Devices.Where(d => d.PlantID == pid && d.DepartmentID == did && d.LocationID == lid && d.StatusID == sid).ToList();

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
                else if (pid != null && did != null && lid == null && phid == null && mid == null && id == null)
                {
                    List<Device> cm = new List<Device>();
                    cm = db.Devices.Where(d => d.PlantID == pid && d.DepartmentID == did && d.StatusID == sid).ToList();

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
                else if (pid != null && did == null && lid == null && phid == null && mid == null && id == null)
                {
                    List<Device> cm = new List<Device>();
                    cm = db.Devices.Where(d => d.PlantID == pid && d.StatusID == sid).ToList();

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
            //return View(id);
                }
            return null;
        }

        public ActionResult exportDataByLocationStock(int? tid, int? lid, int? sid, int? mid)
        {
            if (ModelState.IsValid)
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/Reports"), "InstockByLocationReport.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return View("Index");
                }

                if (mid == null)
                {
                    List<Device> cm = new List<Device>();
                    cm = db.Devices.Where(d => d.DeviceTypeID == tid && d.StatusID == sid && d.LocationStockID == lid).ToList();

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
                else if(mid != null)
                {
                    List<Device> cm = new List<Device>();
                    cm = db.Devices.Where(d => d.DeviceTypeID == tid && d.StatusID == sid && d.LocationStockID == lid && d.ModelID == mid).ToList();

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
            return null;
        }

        public ActionResult criteriaWithDeviceType(ReportViewModels reportviewmodels)
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

                List<Device> cm = new List<Device>();
                cm = db.Devices.Where(d => d.StatusID == reportviewmodels.StatusID && d.DeviceTypeID == reportviewmodels.DeviceTypeID).ToList();

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
            return View(reportviewmodels);
        }

        public ActionResult InRepairByProductionReport()
        {
            return View();
        }


        public ActionResult printListDataByCriteriaBrand(int? id, int? sid)
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

                List<Device> cm = new List<Device>();
                cm = db.Devices.Where(d => d.BrandID == id && d.StatusID == sid).ToList();

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
            return View(id);
        }

        public ActionResult printListDataByCriteriaModel(int?id, int? sid)
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

                List<Device> cm = new List<Device>();
                cm = db.Devices.Where(d => d.ModelID == id && d.StatusID == sid).ToList();

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
            return View(id);
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

        public static IEnumerable<SelectListItem> getDropDownlistTrueFalse()
        {
            IList<SelectListItem> phases = new List<SelectListItem>
            {
                new SelectListItem() {Text="", Value=""},
                new SelectListItem() { Text="Yes", Value="Yes"},
            };
            return phases;
        }

        public static IEnumerable<SelectListItem> GetReportType()
        {
            IList<SelectListItem> types = new List<SelectListItem>
            {
                new SelectListItem() { Text="Model", Value="1"},
                new SelectListItem() { Text="Brand", Value="2"},
                new SelectListItem() { Text="Location", Value="3"},
            };
            return types;
        }

        public static IEnumerable<SelectListItem> GetFileType()
        {
            IList<SelectListItem> ftypes = new List<SelectListItem>
            {
                new SelectListItem() { Text=" ", Value=" "},
                new SelectListItem() { Text="Pdf", Value="Pdf"},
                new SelectListItem() { Text="Excel", Value="Excel"},
                new SelectListItem() { Text="Word", Value="Word"},
            };
            return ftypes;
        }
	}
}