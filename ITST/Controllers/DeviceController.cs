using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ITST.Models;
using ITST.ViewModels;
using ITST.CustomFilters;
using PagedList;
using Newtonsoft.Json;

namespace ITST.Controllers
{
    public class DeviceController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        // GET: /Device/
        [Authorize]
        public ActionResult Index()
        {
            var devices = db.Devices.Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            return View(devices.ToList());
        }

        public ActionResult unknownDeviceStatus()
        {
            return View();
        }

        public ActionResult LastCreate(string uri)
        {
            var devices = db.Devices.Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine).OrderByDescending(d=>d.DeviceID).Take(1);
            ViewBag.uri = uri;
            return View(devices.ToList());
        }

        public ActionResult LastSet(int?id, string uri)
        {
            var devices = db.Devices.Where(d=>d.DeviceID == id).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine).OrderByDescending(d => d.DeviceID);
            ViewBag.uri = uri;
            return View(devices.ToList());
        }

        public ActionResult InStock()
        {
            var devices = db.Devices.Where(d => d.StatusID == 3).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            return View(devices.ToList());
        }

        public ActionResult listAllDevice()
        {
            return View();
        }

        public JsonResult getAllITDevice()
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

        public ActionResult InUse()
        {
            var devices = db.Devices.Where(d => d.StatusID == 1).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            return View(devices.ToList());
        }

        public ActionResult SendRepair()
        {
            var recordreq = from s in db.Devices.Where(d => d.StatusID == 6).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine)
                            select s;
            return View(recordreq);
        }

        public ActionResult Destroyed()
        {
            var recordreq = from s in db.Devices.Where(d => d.StatusID == 12).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine)
                            select s;
            return View(recordreq);
        }

        [Authorize]
        public ActionResult AccessoriesParts(int? id)
        {
            var viewmodel = new DeviceRatioViewModels();
            viewmodel.AccessType = from access in db.Models where access.IsAccess == true group access by access.DeviceType.DeviceTypeID into dv let m = dv.FirstOrDefault() select m;
            if (id != null && id != 0)
            {
                viewmodel.Accessories = from access in db.Models join device in db.Devices on access.ModelID equals device.ModelID where access.IsAccess == true && device.DeviceType.DeviceTypeID == id group access by access.ModelID into dv let m = dv.FirstOrDefault() select m;
            }
            else if (id == null || id == 0)
            {
                viewmodel.Accessories = from access in db.Models where access.IsAccess == true group access by access.ModelID into dv let m = dv.FirstOrDefault() select m;
            }
            ViewBag.TotalCartridge = db.Devices.Where(d => d.Model.IsAccess == true).Count();
            return View(viewmodel);
        }

        [Authorize]
        public ActionResult CartridgePrinter(int? id)
        {
            var viewmodel = new DeviceRatioViewModels();
            viewmodel.Devicest = from device in db.Devices where device.Machine.Description == "Cart" group device by device.Machine.MachineName into dv let m = dv.FirstOrDefault() select m;
            viewmodel.Devicerd = from device in db.Devices where device.DeviceType.DeviceTypeID == 81 group device by device.Model.ModelID into dv let m = dv.FirstOrDefault() select m;
            if (id != null && id != 0)
            {
                viewmodel.CartridgeModel = from model in db.Models join device in db.Devices on model.ModelID equals device.ModelID where model.DeviceType.DeviceTypeID == 81 && device.MachineID == id group model by model.ModelID into dv let m = dv.FirstOrDefault() select m;
            }
            else if (id == null || id == 0)
            {
                viewmodel.CartridgeModel = from model in db.Models where model.DeviceType.DeviceTypeID == 81 group model by model.ModelID into dv let m = dv.FirstOrDefault() select m;
            }
            DateTime date = DateTime.Today;
            var Date = date.ToString("dd");
            var Month = date.ToString("MM");
            int Dt = Int32.Parse(Date);
            int Mt = Int32.Parse(Month);
            int start = Mt + 5;

            ViewBag.Date = Dt;
            ViewBag.Month = Mt;
            ViewBag.Start = start;
            ViewBag.TotalCartridge = db.Devices.Where(d => d.DeviceType.DeviceTypeID == 81).Count();
            return View(viewmodel);
        }

        [Authorize]
        public ActionResult listCartridge()
        {
            var devices = db.Devices.Where(d => d.DeviceType.DeviceTypeID == 81).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            return View(devices.ToList());
        }

        [Authorize]
        public ActionResult listAccessories()
        {
            var devices = db.Devices.Where(d => d.Model.IsAccess == true).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            return View(devices.ToList());
        }

        public ActionResult listCartridgeByModel(int? modelID, int? machineID)
        {
            var devices = db.Devices.Where(d => d.DeviceType.DeviceTypeID == 81 && d.ModelID == modelID && d.MachineID == machineID).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            return View(devices.ToList());
        }

        [Authorize]
        public ActionResult CreateAccessories()
        {
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName");
            ViewBag.Machine = new SelectList(db.Machines.Where(m => m.Description == "Cart").OrderBy(d => d.MachineName), "MachineID", "MachineName");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAccessories([Bind(Include = "ModelName,Quantity,LocationStock")] CreateAccessoriesViewModels viewmodels)
        {
            if (ModelState.IsValid)
            {
                for (int i = 1; i <= viewmodels.Quantity; i++)
                {
                    int lastID = db.Devices.Max(x => x.DeviceID);
                    int srID = lastID+1;
                    Device device = new Device();
                    device.StatusID = 3;
                    device.SerialNumber = "No SerialNumber" + " " + srID;
                    device.ModelID = db.Models.Where(b => b.ModelName == viewmodels.ModelName).Select(b => b.ModelID).DefaultIfEmpty().First();
                    device.StatusName = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                    device.BrandID = db.Models.Where(b => b.ModelName == viewmodels.ModelName).Select(b => b.BrandID).DefaultIfEmpty().First();
                    device.Specification = db.Models.Where(b => b.ModelName == viewmodels.ModelName).Select(b => b.Specification).DefaultIfEmpty().First();
                    device.DeviceTypeID = db.Models.Where(b => b.ModelName == viewmodels.ModelName).Select(b => b.DeviceTypeID).DefaultIfEmpty().First();
                    device.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                    device.BrandName = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                    device.LocationStockID = viewmodels.LocationStock;
                    //device.MachineID = viewmodels.MachineName;
                    device.LocationStockName = db.LocationStocks.Where(b => b.LocationID == device.LocationStockID).Select(b => b.LocationName).DefaultIfEmpty().First();
                    device.ModelName = viewmodels.ModelName;
                    device.IPAddress = null;
                    device.PRNumber = null;
                    device.FixAccess = null;
                    device.DateCreate = DateTime.Now;
                    device.DateUpdate = DateTime.Now;
                    device.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
                    device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                    device.InstockDate = DateTime.Today;

                    RecordInstock recordinstock = new RecordInstock();
                    recordinstock.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                    recordinstock.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                    recordinstock.InstockBy = System.Web.HttpContext.Current.User.Identity.Name;
                    recordinstock.DateInstock = device.DateCreate;
                    recordinstock.Model = device.ModelName;
                    recordinstock.SerialNumber = device.SerialNumber;
                    recordinstock.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                    recordinstock.LocationStock = device.LocationStockName;
                    db.Devices.Add(device);
                    db.RecordInstocks.Add(recordinstock);
                    db.SaveChanges();
                }
                return RedirectToAction("recentCreate", "Device", new { id = viewmodels.Quantity });
            }
            return View(viewmodels);
        }

        [Authorize]
        public ActionResult CreateCartridge()
        {
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName");
            ViewBag.Machine = new SelectList(db.Machines.Where(m => m.Description == "Cart").OrderBy(d => d.MachineName), "MachineID", "MachineName");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCartridge([Bind(Include = "ModelName,MachineName,Quantity")] CreateCartridgeViewModels viewmodels)
        {
            if (ModelState.IsValid)
            {
                for (int i = 1; i <= viewmodels.Quantity; i++)
                {
                    Device device = new Device();
                    device.StatusID = 3;
                    device.SerialNumber = "CT";
                    device.ModelID = db.Models.Where(b => b.ModelName == viewmodels.ModelName).Select(b => b.ModelID).DefaultIfEmpty().First();
                    device.StatusName = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                    device.BrandID = db.Models.Where(b => b.ModelName == viewmodels.ModelName).Select(b => b.BrandID).DefaultIfEmpty().First();
                    device.Specification = db.Models.Where(b => b.ModelName == viewmodels.ModelName).Select(b => b.Specification).DefaultIfEmpty().First();
                    device.DeviceTypeID = db.Models.Where(b => b.ModelName == viewmodels.ModelName).Select(b => b.DeviceTypeID).DefaultIfEmpty().First();
                    device.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                    device.BrandName = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                    device.LocationStockID = 1;
                    device.MachineID = viewmodels.MachineName;
                    device.LocationStockName = db.LocationStocks.Where(b => b.LocationID == device.LocationStockID).Select(b => b.LocationName).DefaultIfEmpty().First();
                    device.ModelName = viewmodels.ModelName;
                    device.IPAddress = null;
                    device.PRNumber = null;
                    device.FixAccess = null;
                    device.DateCreate = DateTime.Now;
                    device.DateUpdate = DateTime.Now;
                    device.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
                    device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                    device.InstockDate = DateTime.Today;

                    RecordInstock recordinstock = new RecordInstock();
                    recordinstock.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                    recordinstock.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                    recordinstock.InstockBy = System.Web.HttpContext.Current.User.Identity.Name;
                    recordinstock.DateInstock = device.DateCreate;
                    recordinstock.Model = device.ModelName;
                    recordinstock.SerialNumber = device.SerialNumber;
                    recordinstock.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                    recordinstock.LocationStock = device.LocationStockName;
                    db.Devices.Add(device);
                    db.RecordInstocks.Add(recordinstock);
                    db.SaveChanges();
                }
                return RedirectToAction("recentCreate", "Device", new { id = viewmodels.Quantity });
            }
            return View(viewmodels);
        }

        [Authorize]
        public ActionResult setMultipleAccessoriesRequisition()
        {
            setMultipleAccessoriesRequisition viewmodel = new setMultipleAccessoriesRequisition();
            viewmodel.Quantity = 1;
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseName", "PhaseName");
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationName", "LocationName");
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantName", "PlantName");
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationID), "LocationName", "LocationName");
            return View(viewmodel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult setMultipleAccessoriesRequisition([Bind(Include = "PlantID,LocationID,PhaseID,ModelName,MachineName,Quantity,LocationStock")] setMultipleAccessoriesRequisition viewmodel)
        {
            var totalCartridge = db.Devices.Where(d => d.ModelName == viewmodel.ModelName && d.LocationStockName == viewmodel.LocationStock && d.StatusID == 3).Count();
            var type = db.Models.Where(m => m.ModelName == viewmodel.ModelName).Select(m => m.DeviceType.Type).DefaultIfEmpty().First();
            var typeid = db.Models.Where(m => m.ModelName == viewmodel.ModelName).Select(m => m.DeviceType.DeviceTypeID).DefaultIfEmpty().First();
            var typesub = type.Substring(0,3);

            var Mid = db.Machines.Where(b => b.MachineName == viewmodel.MachineName).Select(b => b.MachineID).DefaultIfEmpty().First();
            var Mqty = db.Devices.Where(d => d.DeviceType.Type == type && d.MachineID == Mid && d.StatusID == 1).Count();

            if (viewmodel.Quantity > totalCartridge)
            {
                ModelState.AddModelError("Quantity", "Quantity is more than stock quantity");
            }

            if (typesub == "HDD" || typesub == "SSD")
            {
                if (string.IsNullOrEmpty(viewmodel.MachineName))
                {
                    ModelState.AddModelError("MachineName", "MachineName Required");
                }

                int? LimitQuantity;
                int CurrentQuantity = db.Devices.Where(d => d.Machine.MachineName == viewmodel.MachineName && d.DeviceType.Type == type && d.StatusID == 1).Count();
                int LimitID = db.LimitDeviceQuantities.Where(m => m.Machine == viewmodel.MachineName && m.DeviceType == type).Select(m => m.ID).DefaultIfEmpty().First();

                if (LimitID != 0)
                {
                    LimitQuantity = db.LimitDeviceQuantities.Where(m => m.ID == LimitID).Select(m => m.MaxQuantity).DefaultIfEmpty().First();
                    if (CurrentQuantity >= LimitQuantity)
                    {
                        ModelState.AddModelError("MachineName", "Limit Device");
                    }
                    if (viewmodel.Quantity > LimitQuantity)
                    {
                        ModelState.AddModelError("MachineName", "Limit Device");
                    }
                }
                else if (LimitID == 0)
                {
                    if (Mqty >= 1)
                    {
                        ModelState.AddModelError("MachineName", "Limit Device");
                    }
                }

                if (ModelState.IsValid)
                {
                    for (int k = 1; k <= viewmodel.Quantity; k++)
                    {
                        var device = db.Devices.Where(u => u.ModelName == viewmodel.ModelName && u.LocationStockName == viewmodel.LocationStock && u.StatusID ==3).FirstOrDefault();
                        device.MachineID = Mid;
                        device.StatusID = 1;
                        device.StatusName = "Use";
                        device.DateUpdate = DateTime.Now;
                        device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;

                        RecordRequisition recordRequisition = new RecordRequisition();
                        recordRequisition.Brand = db.Models.Where(b => b.ModelName == viewmodel.ModelName).Select(b => b.Brand.BrandName).DefaultIfEmpty().First();
                        recordRequisition.DeviceType = "Accessories " + db.Models.Where(b => b.ModelName == viewmodel.ModelName).Select(b => b.DeviceType.Type).DefaultIfEmpty().First();
                        recordRequisition.Model = db.Models.Where(b => b.ModelName == viewmodel.ModelName).Select(b => b.ModelName).DefaultIfEmpty().First();
                        recordRequisition.SerialNumber = device.SerialNumber;
                        recordRequisition.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                        recordRequisition.DateRequisition = DateTime.Now;
                        recordRequisition.Cause = "เบิกไปใช้งาน";
                        recordRequisition.Plant = db.Machines.Where(b => b.MachineName == viewmodel.MachineName).Select(b => b.Plant.PlantName).DefaultIfEmpty().First();
                        recordRequisition.Location = db.Machines.Where(b => b.MachineName == viewmodel.MachineName).Select(b => b.Location.LocationName).DefaultIfEmpty().First();
                        recordRequisition.Phase = db.Machines.Where(b => b.MachineName == viewmodel.MachineName).Select(b => b.Phase.PhaseName).DefaultIfEmpty().First();
                        recordRequisition.Machine = viewmodel.MachineName;
                        recordRequisition.LocationStock = device.LocationStockName;
                        recordRequisition.Status = "Use";
                        db.RecordRequisitions.Add(recordRequisition);
                        db.Entry(device).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return RedirectToAction("AccessoriesParts", "Device");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(viewmodel.PlantID))
                {
                    ModelState.AddModelError("PlantID", "Plant Required");
                }
                if (string.IsNullOrEmpty(viewmodel.LocationID))
                {
                    ModelState.AddModelError("LocationID", "Location Required");
                }
                if (string.IsNullOrEmpty(viewmodel.PhaseID))
                {
                    ModelState.AddModelError("PhaseID", "Phase Required");
                }

                if (ModelState.IsValid)
                {
                    int plantID = db.Plants.Where(p => p.PlantName == viewmodel.PlantID).Select(p => p.PlantID).DefaultIfEmpty().First();
                    int locationID = db.Locations.Where(p => p.LocationName == viewmodel.LocationID).Select(p => p.LocationID).DefaultIfEmpty().First();
                    int phaseID = db.Phases.Where(p => p.PhaseName == viewmodel.PhaseID).Select(p => p.PhaseID).DefaultIfEmpty().First();
                    for (int k = 1; k <= viewmodel.Quantity; k++)
                    {
                        var device = db.Devices.Where(u => u.ModelName == viewmodel.ModelName && u.LocationStockName == viewmodel.LocationStock && u.StatusID == 3).FirstOrDefault();

                        RecordRequisition recordRequisition = new RecordRequisition();
                        recordRequisition.Brand = db.Models.Where(b => b.ModelName == viewmodel.ModelName).Select(b => b.Brand.BrandName).DefaultIfEmpty().First();
                        recordRequisition.DeviceType = "Accessories " + db.Models.Where(b => b.ModelName == viewmodel.ModelName).Select(b => b.DeviceType.Type).DefaultIfEmpty().First();
                        recordRequisition.Model = db.Models.Where(b => b.ModelName == viewmodel.ModelName).Select(b => b.ModelName).DefaultIfEmpty().First();
                        recordRequisition.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                        recordRequisition.DateRequisition = DateTime.Now;
                        recordRequisition.Cause = "เบิกไปใช้งาน";
                        recordRequisition.Plant = db.Plants.Where(b => b.PlantID == plantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                        recordRequisition.Location = db.Locations.Where(b => b.LocationID == locationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                        recordRequisition.Phase = db.Phases.Where(b => b.PhaseID == phaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                        recordRequisition.LocationStock = "PBX Room";
                        recordRequisition.Status = "Use";
                        db.RecordRequisitions.Add(recordRequisition);
                        db.Devices.Remove(device);
                        db.SaveChanges();
                    }
                    return RedirectToAction("AccessoriesParts", "Device");
                }
            }
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationID), "LocationName", "LocationName", viewmodel.LocationID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", viewmodel.PhaseID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", viewmodel.LocationID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", viewmodel.PlantID);
            return View(viewmodel);
        }

        [Authorize]
        public ActionResult setChangeLocationStockAccessoriesParts()
        {
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationID), "LocationName", "LocationName");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult setChangeLocationStockAccessoriesParts([Bind(Include = "ModelName,LocationStock,Quantity,PreviousLocationStock")] setChangeLocationStockAccessories viewmodel)
        {
            //var totalCartridge = db.Devices.Where(d => d.ModelName == viewmodel.ModelName && d.StatusID == 3).Count();
            var totalPrevious = db.Devices.Where(d => d.ModelName == viewmodel.ModelName && d.StatusID == 3 && d.LocationStockName == viewmodel.PreviousLocationStock).Count();

            //if (viewmodel.Quantity > totalCartridge)
            //{
            //    ModelState.AddModelError("Quantity", "Quantity is more than stock quantity");
            //}

            if (viewmodel.Quantity > totalPrevious)
            {
                ModelState.AddModelError("Quantity", "Quantity is more than Stock quantity");
            }

            if (ModelState.IsValid)
            {
                for (int k = 1; k <= viewmodel.Quantity; k++)
                {
                    var device = db.Devices.Where(u => u.ModelName == viewmodel.ModelName && u.StatusID == 3 && u.LocationStockName == viewmodel.PreviousLocationStock).FirstOrDefault();

                    device.LocationStockID = db.LocationStocks.Where(b => b.LocationName == viewmodel.LocationStock).Select(b => b.LocationID).DefaultIfEmpty().First();
                    device.LocationStockName = viewmodel.LocationStock;
                    device.DateUpdate = DateTime.Now;
                    device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                    RecordDevice recordDevice = new RecordDevice();
                    recordDevice.Brand = device.BrandName;
                    recordDevice.Type = device.DeviceType.Type;
                    recordDevice.Model = device.ModelName;
                    recordDevice.EditBy = System.Web.HttpContext.Current.User.Identity.Name;
                    recordDevice.EditDate = DateTime.Now;
                    recordDevice.LocationStock = device.LocationStockName;
                    recordDevice.Status = device.StatusName;
                    recordDevice.Description = "Change LocationStock";
                    db.Entry(device).State = EntityState.Modified;
                    db.RecordDevices.Add(recordDevice);
                    db.SaveChanges();
                }
                return RedirectToAction("AccessoriesParts", "Device");
            }
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationID), "LocationName", "LocationName", viewmodel.LocationStock);
            return View(viewmodel);
        }

        [Authorize]
        public ActionResult setMultipleCartridegeRequisition()
        {
            MultipleCartridgeRequisition viewmodel = new MultipleCartridgeRequisition();
            viewmodel.Quantity = 1;
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName");
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName");
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName");
            return View(viewmodel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult setMultipleCartridegeRequisition([Bind(Include = "DeviceID,PlantID,LocationID,PhaseID,ModelID,ModelName,MachineID,Quantity")] MultipleCartridgeRequisition viewmodel)
        {
            var totalCartridge = db.Devices.Where(d => d.ModelName == viewmodel.ModelName).Count();

            if (viewmodel.Quantity > totalCartridge)
            {
                ModelState.AddModelError("Quantity", "Quantity is more than stock quantity");
            }

            if (ModelState.IsValid)
            {
                for (int k = 1; k <= viewmodel.Quantity; k++)
                {
                    var device = db.Devices.Where(u => u.ModelName == viewmodel.ModelName).FirstOrDefault();

                    RecordRequisition recordRequisition = new RecordRequisition();
                    recordRequisition.Brand = db.Models.Where(b => b.ModelName == viewmodel.ModelName).Select(b => b.Brand.BrandName).DefaultIfEmpty().First();
                    recordRequisition.DeviceType = db.Models.Where(b => b.ModelName == viewmodel.ModelName).Select(b => b.DeviceType.Type).DefaultIfEmpty().First();
                    recordRequisition.Model = db.Models.Where(b => b.ModelName == viewmodel.ModelName).Select(b => b.ModelName).DefaultIfEmpty().First();
                    recordRequisition.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                    recordRequisition.DateRequisition = DateTime.Now;
                    recordRequisition.Cause = "เบิกไปใช้งาน";
                    recordRequisition.Plant = db.Plants.Where(b => b.PlantID == viewmodel.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                    recordRequisition.Location = db.Locations.Where(b => b.LocationID == viewmodel.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                    recordRequisition.Phase = db.Phases.Where(b => b.PhaseID == viewmodel.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                    recordRequisition.LocationStock = "PBX Room";
                    recordRequisition.Status = "Use";
                    db.RecordRequisitions.Add(recordRequisition);
                    db.Devices.Remove(device);
                    db.SaveChanges();
                }
                return RedirectToAction("CartridgePrinter", "Device");
            }
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", viewmodel.PhaseID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", viewmodel.LocationID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", viewmodel.PlantID);
            return View(viewmodel);
        }


        [Authorize]
        public ActionResult setCartridegeRequisition(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CartridgeRequisition viewmodel = new CartridgeRequisition();
            viewmodel.DeviceID = id;
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName");
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName");
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName");
            return View(viewmodel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult setCartridegeRequisition([Bind(Include = "DeviceID,PlantID,LocationID,PhaseID,ModelID,MachineID")] CartridgeRequisition viewmodel)
        {
            if (ModelState.IsValid)
            {
                Device device = db.Devices.Find(viewmodel.DeviceID);

                var modelID = db.Devices.Where(d => d.DeviceID == viewmodel.DeviceID).Select(d => d.ModelID).DefaultIfEmpty().First();
                var machineID = db.Devices.Where(d => d.DeviceID == viewmodel.DeviceID).Select(d => d.MachineID).DefaultIfEmpty().First();
                RecordRequisition recordRequisition = new RecordRequisition();
                recordRequisition.Brand = db.Devices.Where(b => b.DeviceID == viewmodel.DeviceID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recordRequisition.DeviceType = db.Devices.Where(b => b.DeviceID == viewmodel.DeviceID).Select(b => b.Type).DefaultIfEmpty().First();
                recordRequisition.Model = db.Devices.Where(b => b.DeviceID == viewmodel.DeviceID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recordRequisition.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                recordRequisition.DateRequisition = DateTime.Now;
                recordRequisition.Cause = "เบิกไปใช้งาน";
                recordRequisition.Plant = db.Plants.Where(b => b.PlantID == viewmodel.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                recordRequisition.Location = db.Locations.Where(b => b.LocationID == viewmodel.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordRequisition.Phase = db.Phases.Where(b => b.PhaseID == viewmodel.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                recordRequisition.LocationStock = "PBX Room";
                recordRequisition.Status = "Use";

                db.Devices.Remove(device);
                db.RecordRequisitions.Add(recordRequisition);
                db.SaveChanges();
                return RedirectToAction("listCartridgeByModel", "Device", new { modelID = modelID, machineID = machineID });
            }
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", viewmodel.PhaseID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", viewmodel.LocationID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", viewmodel.PlantID);
            return View(viewmodel);
        }

        [Authorize]
        public ActionResult setAccessoriesRequisition(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessoriesRequisition viewmodel = new AccessoriesRequisition();
            viewmodel.DeviceID = id;
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName");
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName");
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName");
            return View(viewmodel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult setAccessoriesRequisition([Bind(Include = "DeviceID,PlantID,LocationID,PhaseID")] AccessoriesRequisition viewmodel)
        {
            if (ModelState.IsValid)
            {
                Device device = db.Devices.Find(viewmodel.DeviceID);

                RecordRequisition recordRequisition = new RecordRequisition();
                recordRequisition.Brand = db.Devices.Where(b => b.DeviceID == viewmodel.DeviceID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recordRequisition.DeviceType = "Accessories "+ db.Devices.Where(b => b.DeviceID == viewmodel.DeviceID).Select(b => b.Type).DefaultIfEmpty().First();
                recordRequisition.Model = db.Devices.Where(b => b.DeviceID == viewmodel.DeviceID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recordRequisition.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                recordRequisition.DateRequisition = DateTime.Now;
                recordRequisition.Cause = "เบิกไปใช้งาน";
                recordRequisition.Plant = db.Plants.Where(b => b.PlantID == viewmodel.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                recordRequisition.Location = db.Locations.Where(b => b.LocationID == viewmodel.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordRequisition.Phase = db.Phases.Where(b => b.PhaseID == viewmodel.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                recordRequisition.LocationStock = "PBX Room";
                recordRequisition.Status = "Use";

                db.Devices.Remove(device);
                db.RecordRequisitions.Add(recordRequisition);
                db.SaveChanges();
                return RedirectToAction("listAccessories", "Device");
            }
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", viewmodel.PhaseID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", viewmodel.LocationID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", viewmodel.PlantID);
            return View(viewmodel);
        }

        [HttpPost]
        public JsonResult findCartridgeModel(string prefixText)
        {
            var modelname = from x in db.Models
                            where x.ModelName.Contains(prefixText) && x.DeviceTypeID == 81 || x.Brand.BrandName.Contains(prefixText) && x.DeviceTypeID == 81 || x.DeviceType.Type.Contains(prefixText) && x.DeviceTypeID == 81
                            select new
                            {
                                value = x.ModelName + ", " + x.DeviceType.Type + ", " + x.Brand.BrandName + ", " + " Spec: " + x.Specification.Substring(0, 100),
                                name = x.ModelName,
                                id = x.ModelID
                            };
            var result = Json(modelname.Take(10).ToList());
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName");
            return result;
        }

        [HttpPost]
        public JsonResult findAccessoriesModel(string prefixText)
        {
            var modelname = from x in db.Models
                            where x.ModelName.Contains(prefixText) && x.IsAccess == true || x.Brand.BrandName.Contains(prefixText) && x.IsAccess == true || x.DeviceType.Type.Contains(prefixText) && x.IsAccess == true
                            select new
                            {
                                value = x.ModelName + ", " + x.DeviceType.Type + ", " + x.Brand.BrandName + ", " + " Spec: " + x.Specification.Substring(0, 100),
                                name = x.ModelName,
                                id = x.ModelID
                            };
            var result = Json(modelname.Take(10).ToList());
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName");
            return result;
        }

        [Authorize]
        public ActionResult BulkData()
        {
            // This is only for show by default one row for insert data to the database
            List<Device> ci = new List<Device> { new Device { DeviceID = 0, IsAsset=false, ModelName = "", SerialNumber = "", IPAddress = "", FixAccess = "", PRNumber = "", LocationStockName = "" } };
            return View(ci);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BulkData(List<Device> ci)
        {
            if (ModelState.IsValid)
            {
                using (ITStockEntities1 dc = new ITStockEntities1())
                {
                    var allrec = ci.Count();
                    var grouprec = ci.GroupBy(c => c.SerialNumber).Count();
                    if (allrec == grouprec)
                    {
                        foreach (var i in ci)
                        {
                            var sr = i.SerialNumber.Replace("\r", "").Replace("\n", "").Replace("\t", "").ToString();
                            //var txt = i.DeviceName.Replace("\r", "").Replace("\n", "").Replace("\t", "").ToString();
                            //var deviceTypeID = db.Devices.Where(d => d.Model.ModelName == i.ModelName).Select(d => d.DeviceTypeID).DefaultIfEmpty().First();


                            if (db.Devices.Any(d => d.IPAddress.Trim() == i.IPAddress.Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim() && d.IPAddress.Trim() != null && d.IPAddress.Trim() != "DHCP"))
                                {
                                    ModelState.AddModelError("IPAddress", "IPAddress is Duplicated");
                                    ViewBag.WIPAddress = "IPAddress is Duplicated or Empty";
                                    ViewBag.IPAddress = i.IPAddress;
                                    ModelState.Clear();
                                    return View(ci);
                                }

                            else if (db.Devices.Any(d => d.SerialNumber.Trim() == sr.Trim()))
                            {
                                ModelState.AddModelError("SerialNumber", "SerialNumber is Duplicated");
                                ViewBag.WSerialNumber = "SerialNumber is Duplicated";
                                ViewBag.SerialNumber = i.SerialNumber;
                                ModelState.Clear();
                                return View(ci);
                            }
                                else if(!db.LocationStocks.Any(l=>l.LocationName == i.LocationStockName))
                            {
                                ModelState.AddModelError("LocationStockName", "LocationStockName is false");
                                ViewBag.WSerialNumber = "LocationStockName is false";
                                ViewBag.SerialNumber = i.LocationStockName;
                                ModelState.Clear();
                                return View(ci);
                            }
                                else if(!db.Models.Any(m=>m.ModelName == i.ModelName))
                            {
                                ModelState.AddModelError("ModelName", "ModelName is false");
                                ViewBag.WSerialNumber = "ModelName is false";
                                ViewBag.SerialNumber = i.ModelName;
                                ModelState.Clear();
                                return View(ci);
                            }
                            else if (string.IsNullOrEmpty(i.PRNumber) && i.IsNotPR == false)
                            {
                                ModelState.AddModelError("PRNumber", "PRNumber is Required");
                                ViewBag.DeviceName = "PRNumber is Required";
                                ModelState.Clear();
                                return View(ci);
                            }

                            //else if (db.Devices.Any(d => d.DeviceName.Trim() == dvn.Trim()))
                            //{
                            //    ModelState.AddModelError("DeviceName", "DeviceName is Duplicated");
                            //    ViewBag.WSerialNumber = "DeviceName is Duplicated";
                            //    ViewBag.DeviceNameDup = i.DeviceName;
                            //    ModelState.Clear();
                            //    return View(ci);
                            //}
                            else
                            {
                                i.StatusID = 3;
                                i.ModelID = db.Models.Where(b => b.ModelName == i.ModelName).Select(b => b.ModelID).DefaultIfEmpty().First();
                                if (i.IsAsset == true)
                                {
                                    i.Description = "5k";
                                }
                                i.Specification = db.Models.Where(b => b.ModelName == i.ModelName).Select(b => b.Specification).DefaultIfEmpty().First();
                                i.StatusName = db.Status.Where(b => b.StatusID == i.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                                i.BrandID = db.Models.Where(b => b.ModelName == i.ModelName).Select(b => b.BrandID).DefaultIfEmpty().First();
                                i.DeviceTypeID = db.Models.Where(b => b.ModelName == i.ModelName).Select(b => b.DeviceTypeID).DefaultIfEmpty().First();
                                i.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == i.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                                i.BrandName = db.Brands.Where(b => b.BrandID == i.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                                i.LocationStockID = db.LocationStocks.Where(b => b.LocationName == i.LocationStockName).Select(b => b.LocationID).DefaultIfEmpty().First();
                                i.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
                                i.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                                i.InstockDate = DateTime.Now;
                                i.DateCreate = DateTime.Now;
                                i.DateUpdate = DateTime.Now;

                                RecordInstock recordinstock = new RecordInstock();
                                recordinstock.Brand = db.Brands.Where(b => b.BrandID == i.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                                recordinstock.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == i.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                                recordinstock.InstockBy = System.Web.HttpContext.Current.User.Identity.Name;
                                recordinstock.DateInstock = i.DateCreate;
                                recordinstock.Model = db.Models.Where(b => b.ModelID == i.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                                recordinstock.SerialNumber = i.SerialNumber;
                                recordinstock.LocationStock = db.LocationStocks.Where(b => b.LocationName == i.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                                recordinstock.Status = db.Status.Where(b => b.StatusID == i.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                                if (i.Description == "5k")
                                {
                                    recordinstock.IsFixAsset = "Asset";
                                }
                                dc.Devices.Add(i);
                                dc.RecordInstocks.Add(recordinstock);
                            }
                        }
                        dc.SaveChanges();
                        ViewBag.Message = "Data successfully saved!";
                        ModelState.Clear();
                        ci = new List<Device> { new Device { DeviceID = 0, ModelName = "", SerialNumber = "", IPAddress = "", FixAccess = "", PRNumber = "", LocationStockName = "" } };
                        return View(ci);
                    }
                    else
                    {
                        ViewBag.DupMessage = "Duplicated SerialNumber in the list";
                        return View(ci);
                    }
                }
            }
            return View(ci);
        }

        [Authorize]
        public ActionResult moveDevice(int? id, string uri)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            ChangeViewModels dviewmodel = new ChangeViewModels();
            dviewmodel.deviceID = device.DeviceID;
            dviewmodel.Uri = uri;
            if (device == null)
            {
                return HttpNotFound();
            }else if(device.StatusID != 1)
            {
                return Content("Current status can't move");
            }
            ViewBag.URI = uri;
            return View(dviewmodel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult moveDevice([Bind(Include = "deviceID,keyword,Uri")] ChangeViewModels viewmodels)
        {
            if (ModelState.IsValid && viewmodels.keyword != null)
            {
                Device device = db.Devices.Find(viewmodels.deviceID);
                int newUID = db.Users.Where(u => u.FullName == viewmodels.keyword).Select(u => u.UserID).DefaultIfEmpty().First();
                int newMID = db.Machines.Where(m => m.MachineName == viewmodels.keyword).Select(u => u.MachineID).DefaultIfEmpty().First();
                var previousUser = db.Devices.Where(u => u.DeviceID == viewmodels.deviceID).Select(u => u.User.FullName).DefaultIfEmpty().First();
                var previousMachine = db.Devices.Where(u => u.DeviceID == viewmodels.deviceID).Select(u => u.Machine.MachineName).DefaultIfEmpty().First();


                if (newUID != 0)
                {
                    device.MachineID = null;
                    device.UserID = newUID;
                    if (previousUser != null)
                    {
                        device.Reason = "Moved device from " + previousUser;
                    }
                    else if (previousMachine != null)
                    {
                        device.Reason = "Moved device from " + previousMachine;
                    }
                    device.PlantID = db.Users.Where(d => d.UserID == newUID).Select(d => d.PlantID).DefaultIfEmpty().First();
                    device.DepartmentID = db.Users.Where(d => d.UserID == newUID).Select(d => d.DepartmentID).DefaultIfEmpty().First();
                    device.LocationID = db.Users.Where(d => d.UserID == newUID).Select(d => d.LocationID).DefaultIfEmpty().First();
                    device.PhaseID = db.Users.Where(d => d.UserID == newUID).Select(d => d.PhaseID).DefaultIfEmpty().First();
                    device.PhaseName = db.Users.Where(d => d.UserID == newUID).Select(d => d.PhaseName).DefaultIfEmpty().First();
                    device.DateUpdate = DateTime.Now;
                    device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                    db.Entry(device).State = EntityState.Modified;

                    RecordDevice recorddevice = new RecordDevice();
                    recorddevice.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                    recorddevice.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                    recorddevice.EditBy = System.Web.HttpContext.Current.User.Identity.Name;
                    recorddevice.EditDate = device.DateUpdate;
                    if (previousUser != null)
                    {
                        recorddevice.Description = "Moved device from " + previousUser;
                        recorddevice.Cause = "Moved device from " + previousUser;
                    }
                    else
                    {
                        recorddevice.Description = "Moved device from " + previousMachine;
                        recorddevice.Cause = "Moved device from " + previousMachine;
                    }
                    recorddevice.Specification = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.Specification).DefaultIfEmpty().First();
                    recorddevice.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                    recorddevice.SerialNumber = db.Devices.Where(b => b.DeviceID == device.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                    recorddevice.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                    recorddevice.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                    recorddevice.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                    recorddevice.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                    recorddevice.LocationStock = db.LocationStocks.Where(b => b.LocationName == device.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                    recorddevice.Machine = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                    recorddevice.UserName = db.Users.Where(b => b.UserID == device.UserID).Select(b => b.FullName).DefaultIfEmpty().First();
                    recorddevice.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                    db.RecordDevices.Add(recorddevice);
                    db.SaveChanges();
                    return RedirectToAction("LastCreate", new { uri = viewmodels.Uri});
                }
                else if (newMID != 0)
                {
                    device.UserID = null;
                    device.MachineID = newMID;
                    if (previousUser != null)
                    {
                        device.Reason = "Moved device from " + previousUser;
                    }
                    else if (previousMachine != null)
                    {
                        device.Reason = "Moved device from " + previousMachine;
                    }
                    device.PlantID = db.Machines.Where(d => d.MachineID == newMID).Select(d => d.PlantID).DefaultIfEmpty().First();
                    device.DepartmentID = db.Machines.Where(d => d.MachineID == newMID).Select(d => d.DepartmentID).DefaultIfEmpty().First();
                    device.LocationID = db.Machines.Where(d => d.MachineID == newMID).Select(d => d.LocationID).DefaultIfEmpty().First();
                    device.PhaseID = db.Machines.Where(d => d.MachineID == newMID).Select(d => d.PhaseID).DefaultIfEmpty().First();
                    device.PhaseName = db.Machines.Where(d => d.MachineID == newMID).Select(d => d.Phase.PhaseName).DefaultIfEmpty().First();
                    device.DateUpdate = DateTime.Now;
                    device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                    db.Entry(device).State = EntityState.Modified;

                    RecordDevice recorddevice = new RecordDevice();
                    recorddevice.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                    recorddevice.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                    recorddevice.EditBy = System.Web.HttpContext.Current.User.Identity.Name;
                    recorddevice.EditDate = device.DateUpdate;
                    if (previousUser != null)
                    {
                        recorddevice.Description = "Moved device from " + previousUser;
                        recorddevice.Cause = "Moved device from " + previousUser;
                    }
                    else
                    {
                        recorddevice.Description = "Moved device from " + previousMachine;
                        recorddevice.Cause = "Moved device from " + previousMachine;
                    }
                    recorddevice.Specification = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.Specification).DefaultIfEmpty().First();
                    recorddevice.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                    recorddevice.SerialNumber = db.Devices.Where(b => b.DeviceID == device.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                    recorddevice.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                    recorddevice.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                    recorddevice.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                    recorddevice.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                    recorddevice.LocationStock = db.LocationStocks.Where(b => b.LocationName == device.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                    recorddevice.Machine = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                    recorddevice.UserName = db.Users.Where(b => b.UserID == device.UserID).Select(b => b.FullName).DefaultIfEmpty().First();
                    recorddevice.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                    db.RecordDevices.Add(recorddevice);
                    db.SaveChanges();
                    return RedirectToAction("LastCreate", new { uri = viewmodels.Uri });
                }
            }
            ModelState.AddModelError("SerialNumber", "Please Enter Text");
            return View();
        }

        [Authorize]
        public ActionResult setDevice(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.DeviceID = id;
            ViewBag.SerialNumber = db.Devices.Where(b => b.DeviceID == id).Select(b => b.SerialNumber).DefaultIfEmpty().First();
            ViewBag.Type = db.Devices.Where(b => b.DeviceID == id).Select(b => b.Type).DefaultIfEmpty().First();
            ViewBag.Model = db.Devices.Where(b => b.DeviceID == id).Select(b => b.ModelName).DefaultIfEmpty().First();
            ViewBag.Brand = db.Devices.Where(b => b.DeviceID == id).Select(b => b.BrandName).DefaultIfEmpty().First();
            ViewBag.Specification = db.Devices.Where(b => b.DeviceID == id).Select(b => b.Specification).DefaultIfEmpty().First();
            ViewBag.Status = db.Devices.Where(b => b.DeviceID == id).Select(b => b.StatusName).DefaultIfEmpty().First();
            ViewBag.PreviousUrl = System.Web.HttpContext.Current.Request.UrlReferrer;
            return View();
        }

        [Authorize]
        public ActionResult inRepaired()
        {
            var recordreq = from s in db.Devices.Where(d => d.StatusID == 2).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine)
                            select s;
            ViewBag.BillReceiptID = new SelectList(db.BillReceipts.Where(m => m.IsPrint == 0).OrderBy(m => m.BillReceiptID), "BillReceiptID", "BillReceiptNo");
            return View(recordreq);
        }

        public ActionResult InRepair(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var recordreq = from s in db.Devices.Where(d => d.StatusID == 2).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine)
                            select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                recordreq = recordreq.Where(s => s.SerialNumber.Contains(searchString)
                                       || s.ModelName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    recordreq = recordreq.OrderByDescending(s => s.SerialNumber);
                    break;
                case "Date":
                    recordreq = recordreq.OrderBy(s => s.ModelName);
                    break;
                case "date_desc":
                    recordreq = recordreq.OrderByDescending(s => s.ModelName);
                    break;
                default:  // Name ascending 
                    recordreq = recordreq.OrderBy(s => s.SerialNumber);
                    break;
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            ViewBag.BillReceiptID = new SelectList(db.BillReceipts.Where(m=>m.IsPrint == 0 && m.Type == "Repair").OrderBy(m => m.BillReceiptID), "BillReceiptID", "BillReceiptNo");

            return View(recordreq.OrderByDescending(d=>d.DateUpdate).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult SparePart(int pid, int did, int mid)
        {
            var devices = db.Devices.Where(d => d.StatusID == 5 && d.PlantID == pid && d.DepartmentID == did && d.MachineID == mid).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            //ViewBag.PreviousUrl = System.Web.HttpContext.Current.Request.UrlReferrer;
            ViewBag.PlantID = pid;
            ViewBag.DepartmentID = did;
            ViewBag.MachineID = mid;
            return View(devices.ToList());
        }

        public ActionResult SparePartByPlant()
        {
            var plant = db.Plants.Where(p=>p.PlantID == 1 || p.PlantID == 2).ToList();
            return View(plant);
        }

        public ActionResult SparePartByDepartment(int id)
        {
            var department = from device in db.Devices where device.StatusID == 5 && device.PlantID == id group device by device.DepartmentID into dv let m = dv.FirstOrDefault() select m;
            return View(department);
        }

        public ActionResult SparePartByMachine(int did, int pid)
        {
            var machine = from device in db.Devices where device.StatusID == 5 && device.PlantID == pid && device.DepartmentID == did group device by device.MachineID into dv let m = dv.FirstOrDefault() select m;
            return View(machine);
        }

        [Authorize]
        public ActionResult SentRepair()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SentRepair([Bind(Include = "DeviceID,SerialNumber")] Requistion requistion)
        {
            if (ModelState.IsValid && !db.Devices.Any(d => d.SerialNumber == requistion.SerialNumber && d.StatusID != 2))
            {
                var serialnumber = requistion.SerialNumber;
                return RedirectToAction("SentRepairReq", "Device", new { sr = serialnumber });
            }
            ModelState.AddModelError("SerialNumber", "Current status not ready to sent repair");
            return View();
        }

        [Authorize]
        public ActionResult InstockAfterRepair()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InstockAfterRepair([Bind(Include = "DeviceID,SerialNumber")] Requistion requistion)
        {
            if (ModelState.IsValid && !db.Devices.Any(d => d.SerialNumber == requistion.SerialNumber && d.StatusID != 2 && d.StatusID != 6))
            {
                var serialnumber = requistion.SerialNumber;
                return RedirectToAction("InstockAfterRepairReq", "Device", new { sr = serialnumber });
            }
            ModelState.AddModelError("SerialNumber", "Current status not ready to ReInstock");
            return View();
        }

        [Authorize]
        public ActionResult InstockAfterRepairReq(string sr)
        {
            var devices = db.Devices.Include(d => d.Brand).Where(d => d.SerialNumber == sr).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            return View(devices.ToList());
        }

        [Authorize]
        public ActionResult SetInstockAfterRepair(int? id, string uri)
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
            else if(device.StatusID != 2 && device.StatusID != 6)
            {
                return Content("Current status can't set for re-instock");
            }
            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetInstockAfterRepair([Bind(Include = "DeviceID,UserID,MachineID,UMachineID,DeviceName,Description,SerialNumber,Specification,DepartmentID,PlantID,LocationID,DeviceTypeID,BrandID,StatusID,ModelID,DateCreate,DateUpdate,CreateBy,UpdateBy,LocationStockID,LocationStockName,ModelName,Type,BrandName,StatusName,CauseRequistion,InstockDate,PhaseID,PhaseName,MachineName,PRNumber,FixAccess,UserName,IsAsset,Uri,MacAddress")] Device device)
        {
            if (string.IsNullOrEmpty(device.LocationStockID.ToString()))
            {
                ModelState.AddModelError("LocationStockID", "LocationStock is Required");
            }

            if (ModelState.IsValid)
            {
                device.StatusID = 3;
                device.Reason = "Re-Instock After Repaired";
                device.StatusName = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                device.MachineID = null;
                device.PlantID = null;
                device.DepartmentID = null;
                device.LocationID = null;
                device.PhaseID = null;
                device.UserID = null;
                device.UserName = null;
                device.PhaseName = null;
                device.LocationStockName = db.LocationStocks.Where(b => b.LocationID == device.LocationStockID).Select(b => b.LocationName).DefaultIfEmpty().First();
                device.BrandName = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                device.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();

                RecordReinstock recordreinstock = new RecordReinstock();
                recordreinstock.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recordreinstock.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                recordreinstock.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                recordreinstock.DateRequest = device.DateUpdate;
                recordreinstock.Cause = device.CauseRequistion;
                recordreinstock.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recordreinstock.SerialNumber = db.Devices.Where(b => b.DeviceID == device.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                recordreinstock.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                recordreinstock.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                recordreinstock.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordreinstock.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                recordreinstock.LocationStock = db.LocationStocks.Where(b => b.LocationName == device.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordreinstock.Machine = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                recordreinstock.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                recordreinstock.UserName = db.Users.Where(b => b.UserID == device.UserID).Select(b => b.FullName).DefaultIfEmpty().First();
                if(device.Description == "5k")
                {
                    recordreinstock.IsFixAsset = "Asset";
                }
                
                db.Entry(device).State = EntityState.Modified;
                db.RecordReinstocks.Add(recordreinstock);
                db.SaveChanges();
                return RedirectToAction("LastSet", "Device", new { id = device.DeviceID, uri = device.Uri });
            }

            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        [Authorize]
        public ActionResult setToInstock(int? id, string uri)
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
            else if (device.StatusID != 1 && device.StatusID != 10)
            {
                return Content("Current status can't back to in-stock");
            }
            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult setToInstock([Bind(Include = "DeviceID,UserID,MachineID,UMachineID,DeviceName,Description,SerialNumber,Specification,DepartmentID,PlantID,LocationID,DeviceTypeID,BrandID,StatusID,ModelID,DateCreate,DateUpdate,CreateBy,UpdateBy,LocationStockID,LocationStockName,ModelName,Type,BrandName,StatusName,CauseRequistion,InstockDate,PhaseID,PhaseName,MachineName,PRNumber,FixAccess,UserName,Uri,IPAddress,MacAddress")] Device device)
        {
            if (string.IsNullOrEmpty(device.LocationStockID.ToString()))
            {
                ModelState.AddModelError("LocationStockID", "LocationStock is Required");
            }

            if (device.StatusID != 1 && device.StatusID != 10)
            {
                ModelState.AddModelError("StatusID", "StatusID Can't Set To Instock");
                ViewBag.DStatusID = "Not";
            }

            if (ModelState.IsValid)
            {
                device.StatusID = 3;
                device.Reason = "Re-Instock";
                device.StatusName = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                device.MachineID = null;
                device.PlantID = null;
                device.IPAddress = null;
                device.DepartmentID = null;
                device.LocationID = null;
                device.PhaseID = null;
                device.UserID = null;
                device.UserName = null;
                device.PhaseName = null;
                device.LocationStockName = db.LocationStocks.Where(b => b.LocationID == device.LocationStockID).Select(b => b.LocationName).DefaultIfEmpty().First();
                device.BrandName = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                device.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();

                RecordReinstock recordreinstock = new RecordReinstock();
                recordreinstock.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recordreinstock.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                recordreinstock.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                recordreinstock.DateRequest = device.DateUpdate;
                recordreinstock.Cause = device.CauseRequistion;
                recordreinstock.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recordreinstock.SerialNumber = db.Devices.Where(b => b.DeviceID == device.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                recordreinstock.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                recordreinstock.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                recordreinstock.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordreinstock.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                recordreinstock.LocationStock = db.LocationStocks.Where(b => b.LocationName == device.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordreinstock.Machine = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                recordreinstock.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                recordreinstock.UserName = db.Users.Where(b => b.UserID == device.UserID).Select(b => b.FullName).DefaultIfEmpty().First();
                db.Entry(device).State = EntityState.Modified;
                db.RecordReinstocks.Add(recordreinstock);
                db.SaveChanges();
                return RedirectToAction("LastSet", "Device", new { id = device.DeviceID, uri = device.Uri });
            }

            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        [Authorize]
        public ActionResult Spare()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Spare([Bind(Include = "DeviceID,SerialNumber")] Requistion requistion)
        {
            if (ModelState.IsValid && !db.Devices.Any(d => d.SerialNumber == requistion.SerialNumber && d.StatusID != 3))
            {
                var serialnumber = requistion.SerialNumber;
                return RedirectToAction("SpareReq", "Device", new { sr = serialnumber });
            }
            ModelState.AddModelError("SerialNumber", "Current status not ready to spare");
            return View();
        }

        [Authorize]
        public ActionResult inSale()
        {
            var recordreq = from s in db.Devices.Where(d => d.StatusID == 4).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine).OrderByDescending(d=>d.DateUpdate)
                            select s;
            ViewBag.BillReceiptID = new SelectList(db.BillReceipts.Where(m => m.IsPrint == 0 && m.Type == "Sale" || m.Type == "Destroy").OrderBy(m => m.BillReceiptID), "BillReceiptID", "BillReceiptNo");
            return View(recordreq);
        }

        public ActionResult Sale(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var recordreq = from s in db.Devices.Where(d => d.StatusID == 4).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine)
                            select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                recordreq = recordreq.Where(s => s.SerialNumber.Contains(searchString)
                                       || s.ModelName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    recordreq = recordreq.OrderByDescending(s => s.SerialNumber);
                    break;
                case "Date":
                    recordreq = recordreq.OrderBy(s => s.ModelName);
                    break;
                case "date_desc":
                    recordreq = recordreq.OrderByDescending(s => s.ModelName);
                    break;
                default:  // Name ascending 
                    recordreq = recordreq.OrderBy(s => s.SerialNumber);
                    break;
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            ViewBag.BillReceiptID = new SelectList(db.BillReceipts.Where(m => m.IsPrint == 0 && m.Type == "Sale").OrderBy(m => m.BillReceiptID), "BillReceiptID", "BillReceiptNo");
            return View(recordreq.OrderByDescending(d=>d.DateUpdate).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult SentSale()
        {
            var devices = db.Devices.Where(d => d.StatusID == 7).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine).OrderByDescending(d=>d.DateUpdate);
            return View(devices.ToList());
        }

        public ActionResult FindingDevice()
        {
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1");
            return View();
        }

        [HttpPost]
        public JsonResult FindLocationStock(string prefixText)
        {
            var lname = from x in db.LocationStocks
                        where x.LocationName.Contains(prefixText)
                        select new
                        {
                            value = x.LocationName,
                            name = x.LocationName,
                            id = x.LocationID
                        };
            var result = Json(lname.ToList());
            return result;
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FindingDevice([Bind(Include = "DeviceID,SerialNumber")] Requistion requistion)
        {
            if (ModelState.IsValid && requistion.SerialNumber != null)
            {
                var serialnumber = requistion.SerialNumber;
                return RedirectToAction("FindAllDevice", "Device", new { sr = serialnumber });
            }
            ModelState.AddModelError("SerialNumber", "Please Enter Text");
            return View();
        }

        [Authorize]
        public ActionResult Requisition()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Requisition([Bind(Include = "DeviceID,SerialNumber")] Requistion requistion)
        {
            if (ModelState.IsValid && !db.Devices.Any(d => d.SerialNumber == requistion.SerialNumber && d.StatusID != 3 && d.StatusID != 5))
            {
                var serialnumber = requistion.SerialNumber;
                return RedirectToAction("Req", "Device", new { sr = serialnumber });
            }
            ModelState.AddModelError("SerialNumber", "Current status not ready to Requisition");
            return View();
        }

        [Authorize]
        public ActionResult SentRepairReq(string sr)
        {
            var devices = db.Devices.Include(d => d.Brand).Where(d => d.SerialNumber == sr).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            return View(devices.ToList());
        }

        [Authorize]
        public ActionResult Req(string sr)
        {
            var devices = db.Devices.Include(d => d.Brand).Where(d => d.SerialNumber == sr).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            return View(devices.ToList());
        }

        [Authorize]
        public ActionResult FindAllDevice(string sr)
        {
            var MachineID = db.Machines.Where(m => m.MachineName == sr).Select(m => m.MachineID).FirstOrDefault();
            var UserID = db.Users.Where(m => m.FullName == sr || m.DeviceName == sr).Select(m => m.UserID).FirstOrDefault();
            if(!string.IsNullOrEmpty(MachineID.ToString()) && MachineID != 0)
            {
                ViewBag.MachineName = sr;
                ViewBag.Plant = db.Machines.Where(m => m.MachineID == MachineID).Select(m => m.Plant.PlantName).FirstOrDefault();
                ViewBag.Department = db.Machines.Where(m => m.MachineID == MachineID).Select(m => m.Department.DepartmentName).FirstOrDefault();
                ViewBag.Location = db.Machines.Where(m => m.MachineID == MachineID).Select(m => m.Location.LocationName).FirstOrDefault();
                ViewBag.Phase = db.Machines.Where(m => m.MachineID == MachineID).Select(m => m.Phase.PhaseName).FirstOrDefault();
                ViewBag.IPAddress = db.Machines.Where(m => m.MachineID == MachineID).Select(m => m.IPAddress).FirstOrDefault();
                ViewBag.MACAddress = db.Machines.Where(m => m.MachineID == MachineID).Select(m => m.MACAddress).FirstOrDefault();
                ViewBag.PLCAddress = db.Machines.Where(m => m.MachineID == MachineID).Select(m => m.PLCAddress).FirstOrDefault();
            }
            else if (!string.IsNullOrEmpty(UserID.ToString()) && UserID != 0)
            {
                ViewBag.UserName = db.Users.Where(m => m.UserID == UserID).Select(m => m.FullName).FirstOrDefault();
                ViewBag.ComputerName = db.Users.Where(m => m.UserID == UserID).Select(m => m.DeviceName).FirstOrDefault();
                ViewBag.Plant = db.Users.Where(m => m.UserID == UserID).Select(m => m.Plant.PlantName).FirstOrDefault();
                ViewBag.Department = db.Users.Where(m => m.UserID == UserID).Select(m => m.Department.DepartmentName).FirstOrDefault();
                ViewBag.Location = db.Users.Where(m => m.UserID == UserID).Select(m => m.Location.LocationName).FirstOrDefault();
                ViewBag.Phase = db.Users.Where(m => m.UserID == UserID).Select(m => m.PhaseName).FirstOrDefault();
                ViewBag.IPAddress = db.Users.Where(m => m.UserID == UserID).Select(m => m.IPAddress).FirstOrDefault();
            }
            ViewBag.SR = sr;
            return View();
        }

        //[HttpPost]
        public ActionResult setReasonDevice()
        {
            var q1 = from x in db.RecordRequisitions
                     group x by x.SerialNumber
                     into groups
                     select groups.OrderByDescending(q => q.RequisitionID).FirstOrDefault();

            var query = q1.ToList();

            var device = db.Devices.Where(d=>d.StatusID == 1 && d.DepartmentID == 29).ToList();

            foreach (var i in query)
            {
                foreach(var j in device)
                {
                    if(i.SerialNumber == j.SerialNumber)
                    {
                        j.Reason = i.Cause;
                        db.Entry(j).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("Index","Home");
        }

        public ActionResult SetPRNumber()
        {
            //var q1 = from x in db.RecordRequisitions
            //         group x by x.SerialNumber
            //             into groups
            //             select groups.OrderByDescending(q => q.RequisitionID).FirstOrDefault();

            //var query = q1.ToList();

            var device = db.Devices.ToList();

                foreach (var j in device)
                {
                    if (j.PRNumber == "500015694")
                    {
                        j.PRNumber = "PR"+j.PRNumber;
                        db.Entry(j).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public JsonResult getSearchResult(string sr)
        {
            var devices = (from d in db.Devices
                           where d.SerialNumber.Equals(sr) ||
                           d.DeviceName.Equals(sr) && d.StatusID == 1 ||
                           d.User.DeviceName.Equals(sr) && d.StatusID == 1 ||
                           d.Model.ModelName.Equals(sr) ||
                           d.Brand.BrandName.Equals(sr) ||
                           d.DeviceType.Type.Equals(sr) ||
                           d.Machine.MachineName.Equals(sr) && d.StatusID == 1 ||
                           d.User.FullName.Equals(sr) && d.StatusID == 1

                           select new ViewModelsDevices
                           {
                               DeviceID = d.DeviceID,
                               SerialID = d.DeviceID,
                               DeviceName = d.User.DeviceName,
                               MachineName = d.Machine.MachineName,
                               UserName = d.User.FullName,
                               IPAddress = d.IPAddress,
                               MacAddress = d.MacAddress,
                               Asset = d.Description,
                               FixAccess = d.FixAccess,
                               PRNumber = d.PRNumber,
                               SerialNumber = d.SerialNumber,
                               Specification = d.Specification,
                               Model = d.Model.ModelName,
                               Image = d.Model.ImagePath,
                               Type = d.DeviceType.Type,
                               Brand = d.Brand.BrandName,
                               Status = d.Status.Status1,
                               Plant = d.Plant.PlantName,
                               Department = d.Department.DepartmentName,
                               Location = d.Location.LocationName,
                               Phase = d.PhaseName,
                               LocationStockName = d.LocationStockName,
                               CreateBy = d.CreateBy,
                               UpdateBy = d.UpdateBy,
                               DateCreate = d.DateCreate,
                               DateUpdate = d.DateUpdate,
                           }).ToList();

            var q1 = from x in db.RecordRequisitions
                     group x by x.SerialNumber
                         into groups
                         select groups.OrderByDescending(q => q.RequisitionID).FirstOrDefault();

            var q2 = from n in db.RecordInRepairs
                     group n by n.SerialNumber
                         into groups
                         select groups.OrderByDescending(q => q.InRepairID).FirstOrDefault();

            var q3 = from n in db.RecordInstocks
                     group n by n.SerialNumber
                         into groups
                         select groups.OrderByDescending(q => q.DateInstock).FirstOrDefault();

            var q4 = from n in db.RecordSales
                     group n by n.SerialNumber
                         into groups
                         select groups.OrderByDescending(q => q.DateRequest).FirstOrDefault();

            var q5 = from n in db.RecordSpares
                     group n by n.SerialNumber
                         into groups
                         select groups.OrderByDescending(q => q.DateRequest).FirstOrDefault();

            var q6 = from n in db.RecordReinstocks
                     group n by n.SerialNumber
                         into groups
                         select groups.OrderByDescending(q => q.DateRequest).FirstOrDefault();

            var q7 = from n in db.RecordDevices
                     group n by n.SerialNumber
                         into groups
                         select groups.OrderByDescending(q => q.EditDate).FirstOrDefault();

            var qcombine = (from d in q1
                            join x in db.Devices on d.SerialNumber equals x.SerialNumber
                            select new ViewModelDevice
                            {
                                DeviceID = x.DeviceID,
                                SerialID = x.DeviceID,
                                DeviceName = x.User.DeviceName,
                                MachineName = x.Machine.MachineName,
                                UserName = x.User.FullName,
                                IPAddress = x.IPAddress,
                                MacAddress = x.MacAddress,
                                Asset = x.Description,
                                FixAccess = x.FixAccess,
                                PRNumber = x.PRNumber,
                                SerialNumber = d.SerialNumber,
                                Specification = x.Specification,
                                Model = d.Model,
                                Image = x.Model.ImagePath,
                                Type = d.DeviceType,
                                Brand = d.Brand,
                                Status = x.Status.Status1,
                                Plant = x.Plant.PlantName,
                                Department = x.Department.DepartmentName,
                                Location = x.Location.LocationName,
                                Phase = x.PhaseName,
                                LocationStockName = d.LocationStock,
                                Reason = d.Cause,
                                CreateBy = x.CreateBy,
                                UpdateBy = x.UpdateBy,
                                DateCreate = x.DateCreate,
                                DateUpdate = d.DateRequisition
                            }).Union
                           (from d in q2
                            join x in db.Devices on d.SerialNumber equals x.SerialNumber
                            select new ViewModelDevice
                            {
                                DeviceID = x.DeviceID,
                                SerialID = x.DeviceID,
                                DeviceName = x.User.DeviceName,
                                MachineName = x.Machine.MachineName,
                                UserName = x.User.FullName,
                                IPAddress = x.IPAddress,
                                MacAddress = x.MacAddress,
                                Asset = x.Description,
                                FixAccess = x.FixAccess,
                                PRNumber = x.PRNumber,
                                SerialNumber = d.SerialNumber,
                                Specification = x.Specification,
                                Model = d.Model,
                                Image = x.Model.ImagePath,
                                Type = d.DeviceType,
                                Brand = d.Brand,
                                Status = x.Status.Status1,
                                Plant = x.Plant.PlantName,
                                Department = x.Department.DepartmentName,
                                Location = x.Location.LocationName,
                                Phase = x.PhaseName,
                                LocationStockName = d.LocationStock,
                                Reason = d.Cause,
                                CreateBy = x.CreateBy,
                                UpdateBy = x.UpdateBy,
                                DateCreate = x.DateCreate,
                                DateUpdate = d.DateRequest
                            }).Union
                            (from d in q3
                             join x in db.Devices on d.SerialNumber equals x.SerialNumber
                             select new ViewModelDevice
                             {
                                 DeviceID = x.DeviceID,
                                 SerialID = x.DeviceID,
                                 DeviceName = x.User.DeviceName,
                                 MachineName = x.Machine.MachineName,
                                 UserName = x.User.FullName,
                                 IPAddress = x.IPAddress,
                                 MacAddress = x.MacAddress,
                                 Asset = x.Description,
                                 FixAccess = x.FixAccess,
                                 PRNumber = x.PRNumber,
                                 SerialNumber = d.SerialNumber,
                                 Specification = x.Specification,
                                 Model = d.Model,
                                 Image = x.Model.ImagePath,
                                 Type = d.DeviceType,
                                 Brand = d.Brand,
                                 Status = x.Status.Status1,
                                 Plant = x.Plant.PlantName,
                                 Department = x.Department.DepartmentName,
                                 Location = x.Location.LocationName,
                                 Phase = x.PhaseName,
                                 LocationStockName = d.LocationStock,
                                 Reason = null,
                                 CreateBy = x.CreateBy,
                                 UpdateBy = x.UpdateBy,
                                 DateCreate = x.DateCreate,
                                 DateUpdate = d.DateInstock
                             }).Union
                            (from d in q4
                             join x in db.Devices on d.SerialNumber equals x.SerialNumber
                             select new ViewModelDevice
                             {
                                 DeviceID = x.DeviceID,
                                 SerialID = x.DeviceID,
                                 DeviceName = x.User.DeviceName,
                                 MachineName = x.Machine.MachineName,
                                 UserName = x.User.FullName,
                                 IPAddress = x.IPAddress,
                                 MacAddress = x.MacAddress,
                                 Asset = x.Description,
                                 FixAccess = x.FixAccess,
                                 PRNumber = x.PRNumber,
                                 SerialNumber = d.SerialNumber,
                                 Specification = x.Specification,
                                 Model = d.Model,
                                 Image = x.Model.ImagePath,
                                 Type = d.DeviceType,
                                 Brand = d.Brand,
                                 Status = x.Status.Status1,
                                 Plant = x.Plant.PlantName,
                                 Department = x.Department.DepartmentName,
                                 Location = x.Location.LocationName,
                                 Phase = x.PhaseName,
                                 LocationStockName = d.LocationStock,
                                 Reason = d.Cause,
                                 CreateBy = x.CreateBy,
                                 UpdateBy = x.UpdateBy,
                                 DateCreate = x.DateCreate,
                                 DateUpdate = d.DateRequest
                             }).Union
                            (from d in q5
                             join x in db.Devices on d.SerialNumber equals x.SerialNumber
                             where d.Cause != null
                             select new ViewModelDevice
                             {
                                 DeviceID = x.DeviceID,
                                 SerialID = x.DeviceID,
                                 DeviceName = x.User.DeviceName,
                                 MachineName = x.Machine.MachineName,
                                 UserName = x.User.FullName,
                                 IPAddress = x.IPAddress,
                                 MacAddress = x.MacAddress,
                                 Asset = x.Description,
                                 FixAccess = x.FixAccess,
                                 PRNumber = x.PRNumber,
                                 SerialNumber = d.SerialNumber,
                                 Specification = x.Specification,
                                 Model = d.Model,
                                 Image = x.Model.ImagePath,
                                 Type = d.DeviceType,
                                 Brand = d.Brand,
                                 Status = x.Status.Status1,
                                 Plant = x.Plant.PlantName,
                                 Department = x.Department.DepartmentName,
                                 Location = x.Location.LocationName,
                                 Phase = x.PhaseName,
                                 LocationStockName = d.LocationStock,
                                 Reason = d.Cause,
                                 CreateBy = x.CreateBy,
                                 UpdateBy = x.UpdateBy,
                                 DateCreate = x.DateCreate,
                                 DateUpdate = d.DateRequest
                             }).Union
                            (from d in q6
                             join x in db.Devices on d.SerialNumber equals x.SerialNumber
                             //where d.Cause != null
                             select new ViewModelDevice
                             {
                                 DeviceID = x.DeviceID,
                                 SerialID = x.DeviceID,
                                 DeviceName = x.User.DeviceName,
                                 MachineName = x.Machine.MachineName,
                                 UserName = x.User.FullName,
                                 IPAddress = x.IPAddress,
                                 MacAddress = x.MacAddress,
                                 Asset = x.Description,
                                 FixAccess = x.FixAccess,
                                 PRNumber = x.PRNumber,
                                 SerialNumber = d.SerialNumber,
                                 Specification = x.Specification,
                                 Model = d.Model,
                                 Image = x.Model.ImagePath,
                                 Type = d.DeviceType,
                                 Brand = d.Brand,
                                 Status = x.Status.Status1,
                                 Plant = x.Plant.PlantName,
                                 Department = x.Department.DepartmentName,
                                 Location = x.Location.LocationName,
                                 Phase = x.PhaseName,
                                 LocationStockName = d.LocationStock,
                                 Reason = "Re-instock",
                                 CreateBy = x.CreateBy,
                                 UpdateBy = x.UpdateBy,
                                 DateCreate = x.DateCreate,
                                 DateUpdate = d.DateRequest
                             }).Union
                            (from d in q7
                             join x in db.Devices on d.SerialNumber equals x.SerialNumber
                             where d.Cause != null
                             select new ViewModelDevice
                             {
                                 DeviceID = x.DeviceID,
                                 SerialID = x.DeviceID,
                                 DeviceName = x.User.DeviceName,
                                 MachineName = x.Machine.MachineName,
                                 UserName = x.User.FullName,
                                 IPAddress = x.IPAddress,
                                 MacAddress = x.MacAddress,
                                 Asset = x.Description,
                                 FixAccess = x.FixAccess,
                                 PRNumber = x.PRNumber,
                                 SerialNumber = d.SerialNumber,
                                 Specification = x.Specification,
                                 Model = d.Model,
                                 Image = x.Model.ImagePath,
                                 Type = d.Type,
                                 Brand = d.Brand,
                                 Status = x.Status.Status1,
                                 Plant = x.Plant.PlantName,
                                 Department = x.Department.DepartmentName,
                                 Location = x.Location.LocationName,
                                 Phase = x.PhaseName,
                                 LocationStockName = d.LocationStock,
                                 Reason = d.Cause,
                                 CreateBy = x.CreateBy,
                                 UpdateBy = x.UpdateBy,
                                 DateCreate = x.DateCreate,
                                 DateUpdate = d.EditDate
                             }).ToList();

            var qgroup = from x in qcombine
                         group x by x.SerialNumber into groups
                         select groups.OrderByDescending(x => x.DateUpdate).FirstOrDefault();

            var result = from d in devices
                         join x in qgroup on d.SerialNumber equals x.SerialNumber
                         select x;

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getFindAllDevice(string sr)
        {
            var devices = (from d in db.Devices
                           where d.SerialNumber.Equals(sr) ||
                           d.DeviceName.Equals(sr) && d.StatusID == 1 ||
                           d.User.DeviceName.Equals(sr) && d.StatusID == 1 ||
                           d.Model.ModelName.Equals(sr) ||
                           d.Brand.BrandName.Equals(sr) ||
                           d.DeviceType.Type.Equals(sr) ||
                           d.Machine.MachineName.Equals(sr) && d.StatusID == 1 ||
                           d.User.FullName.Equals(sr) && d.StatusID == 1

                           select new ViewModelsDevices
                           {
                               DeviceID = d.DeviceID,
                               SerialID = d.DeviceID,
                               DeviceName = d.User.DeviceName,
                               MachineName = d.Machine.MachineName,
                               UserName = d.User.FullName,
                               IPAddress = d.IPAddress,
                               MacAddress = d.MacAddress,
                               Asset = d.Description,
                               FixAccess = d.FixAccess,
                               PRNumber = d.PRNumber,
                               SerialNumber = d.SerialNumber,
                               Specification = d.Specification,
                               Model = d.Model.ModelName,
                               Image = d.Model.ImagePath,
                               Type = d.DeviceType.Type,
                               Brand = d.Brand.BrandName,
                               Status = d.Status.Status1,
                               Plant = d.Plant.PlantName,
                               Department = d.Department.DepartmentName,
                               Location = d.Location.LocationName,
                               Phase = d.PhaseName,
                               LocationStockName = d.LocationStockName,
                               CreateBy = d.CreateBy,
                               UpdateBy = d.UpdateBy,
                               DateCreate = d.DateCreate,
                               DateUpdate = d.DateUpdate,
                               Reason = d.Reason,
                           }).ToList();
            return Json(new { data = devices }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult getAllDevice()
        {
            var devices = (from d in db.Devices
                           select new ViewModelsDevices
                           {
                               DeviceID = d.DeviceID,
                               MachineName = d.Machine.MachineName,
                               UserName = d.User.FullName,
                               FixAccess = d.FixAccess,
                               Asset = d.Description,
                               PRNumber = d.PRNumber,
                               MacAddress = d.MacAddress,
                               IPAddress = d.IPAddress,
                               SerialNumber = d.SerialNumber,
                               Specification = d.Specification,
                               Model = d.Model.ModelName,
                               Image = d.Model.ImagePath,
                               Type = d.DeviceType.Type,
                               Brand = d.Brand.BrandName,
                               Status = d.Status.Status1,
                               Plant = d.Plant.PlantName,
                               Department = d.Department.DepartmentName,
                               Location = d.Location.LocationName,
                               Phase = d.PhaseName,
                               LocationStockName = d.LocationStockName,
                               CreateBy = d.CreateBy,
                               UpdateBy = d.UpdateBy,
                               DateCreate = d.DateCreate,
                               DateUpdate = d.DateUpdate,
                           }).ToList();
            return Json(new { data = devices }, JsonRequestBehavior.AllowGet);
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

        public JsonResult getUnkownDevice()
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

                           where d.StatusID == 10

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


        [Authorize]
        public ActionResult ListFindAllDevice(int? id)
        {
            var devices = from device in db.Devices where device.ModelID == id group device by device.DeviceID into dv let m = dv.FirstOrDefault() select m;
            return View(devices);
        }

        [Authorize]
        public ActionResult FindD(string sr)
        {
            var devices = db.Devices.Where(d => d.SerialNumber == sr || d.ModelName == sr || d.BrandName == sr || d.DeviceType.Type == sr).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            return View(devices.ToList());
        }

        [Authorize]
        public ActionResult Repair()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Repair([Bind(Include = "DeviceID,SerialNumber")] Requistion requistion)
        {
            if (ModelState.IsValid && !db.Devices.Any(d => d.SerialNumber == requistion.SerialNumber && d.StatusID != 1 && d.StatusID != 3))
            {
                var serialnumber = requistion.SerialNumber;
                return RedirectToAction("RepairReq", "Device", new { sr = serialnumber });
            }
            ModelState.AddModelError("SerialNumber", "Current status not ready to repair");
            return View();
        }

        public ActionResult recentCreate(int id)
        {
            var devices = db.Devices.Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine).OrderByDescending(d => d.DeviceID).Take(id);
            return View(devices.ToList());
        }

        [Authorize]
        public ActionResult RepairReq(string sr)
        {
            var devices = db.Devices.Include(d => d.Brand).Where(d => d.SerialNumber == sr).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            return View(devices.ToList());
        }

        [Authorize]
        public ActionResult SellReq(string sr)
        {
            var devices = db.Devices.Include(d => d.Brand).Where(d => d.SerialNumber == sr).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            return View(devices.ToList());
        }

        [Authorize]
        public ActionResult SpareReq(string sr)
        {
            var devices = db.Devices.Include(d => d.Brand).Where(d => d.SerialNumber == sr).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine);
            return View(devices.ToList());
        }

        [Authorize]
        public ActionResult Sell()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sell([Bind(Include = "DeviceID,SerialNumber")] Requistion requistion)
        {
            if (ModelState.IsValid && !db.Devices.Any(d => d.SerialNumber == requistion.SerialNumber && d.StatusID != 2 && d.StatusID != 3))
            {
                var serialnumber = requistion.SerialNumber;
                return RedirectToAction("SellReq", "Device", new { sr = serialnumber });
            }
            ModelState.AddModelError("SerialNumber", "Current status not ready to sale");
            return View();
        }

        // GET: /Device/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            return View(device);
        }

        // GET: /Device/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName");
            ViewBag.Uri = System.Web.HttpContext.Current.Request.UrlReferrer;
            return View();
        }

        // POST: /Device/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SerialNumber,ModelName,LocationStock,IPAddress,FixAccess,PRNumber,IsAsset,IsNoPRNumber,MacAddress,DeviceName")] CreateDeviceViewModels viewmodels, string Uri)
        {
            var sr = viewmodels.SerialNumber.Replace("\r", "").Replace("\n", "").Replace("\t", "").ToString();
            //var deviceTypeID = db.Devices.Where(d => d.Model.ModelName == viewmodels.ModelName).Select(d => d.DeviceTypeID).DefaultIfEmpty().First();

            if (viewmodels.IPAddress != null)
            {
                var ip = viewmodels.IPAddress.Replace("\r", "").Replace("\n", "").Replace("\t", "").ToString();
                if (db.Devices.Any(d => d.IPAddress.Trim() == ip.Trim() && d.IPAddress != null && d.IPAddress.Trim() != "DHCP"))
                {
                    ModelState.AddModelError("IPAddress", "IPAddress is Duplicated");
                }
            }

            if (db.Devices.Any(d => d.SerialNumber.Trim() == viewmodels.SerialNumber.Trim()))
            {
                ModelState.AddModelError("SerialNumber", "SerialNumber is Duplicated");
            }

            if (db.Devices.Any(d => d.SerialNumber.Trim() == sr.Trim()))
            {
                ModelState.AddModelError("SerialNumber", "SerialNumber is Duplicated");
            }

            if (string.IsNullOrEmpty(viewmodels.PRNumber) && viewmodels.IsNoPRNumber == false)
            {
                ModelState.AddModelError("PRNumber", "PRNumber is Required");
            }

            if (ModelState.IsValid)
            {
                Device device = new Device();
                device.StatusID = 3;
                device.DeviceName = viewmodels.DeviceName;
                device.MacAddress = viewmodels.MacAddress;
                device.SerialNumber = viewmodels.SerialNumber;
                device.ModelID = db.Models.Where(b => b.ModelName == viewmodels.ModelName).Select(b => b.ModelID).DefaultIfEmpty().First();
                device.StatusName = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                device.BrandID = db.Models.Where(b => b.ModelName == viewmodels.ModelName).Select(b => b.BrandID).DefaultIfEmpty().First();
                device.Specification = db.Models.Where(b => b.ModelName == viewmodels.ModelName).Select(b => b.Specification).DefaultIfEmpty().First();
                device.DeviceTypeID = db.Models.Where(b => b.ModelName == viewmodels.ModelName).Select(b => b.DeviceTypeID).DefaultIfEmpty().First();
                device.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                device.BrandName = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                device.LocationStockID = viewmodels.LocationStock;
                device.LocationStockName = db.LocationStocks.Where(b => b.LocationID == device.LocationStockID).Select(b => b.LocationName).DefaultIfEmpty().First();
                device.ModelName = viewmodels.ModelName;
                device.IPAddress = viewmodels.IPAddress;
                device.PRNumber = viewmodels.PRNumber;
                device.FixAccess = viewmodels.FixAccess;
                device.DateCreate = DateTime.Now;
                device.DateUpdate = DateTime.Now;
                device.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
                device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                device.InstockDate = DateTime.Today;
                if (viewmodels.IsAsset == true)
                {
                    device.Description = "5k";
                }

                RecordInstock recordinstock = new RecordInstock();
                recordinstock.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recordinstock.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                recordinstock.InstockBy = System.Web.HttpContext.Current.User.Identity.Name;
                recordinstock.DateInstock = device.DateCreate;
                recordinstock.Model = device.ModelName;
                recordinstock.SerialNumber = device.SerialNumber;
                recordinstock.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                recordinstock.LocationStock = device.LocationStockName;
                if (viewmodels.IsAsset == true)
                {
                    recordinstock.IsFixAsset = "Asset";
                }

                db.Devices.Add(device);
                db.RecordInstocks.Add(recordinstock);
                db.SaveChanges();
                return RedirectToAction("LastCreate", new { uri = Uri});
            }
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", viewmodels.LocationStock);
            return View(viewmodels);
        }

        [Authorize]
        public ActionResult CreateAndRequisition()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAndRequisition([Bind(Include = "MachineName,UserName,ModelName,SerialNumber,IPAddress,FixAccess,PRNumber,DeviceName,IsAsset")] CreateRequistionViewModels deviceviewmodel)
        {
            try
            {
                if ((!string.IsNullOrEmpty(deviceviewmodel.MachineName)) && (!string.IsNullOrEmpty(deviceviewmodel.UserName)))
                {
                    ModelState.AddModelError("UserName", "Input only one textbox");
                    ModelState.AddModelError("MachineName", "Input only one textbox");
                }
                if ((string.IsNullOrEmpty(deviceviewmodel.MachineName)) && (string.IsNullOrEmpty(deviceviewmodel.UserName)))
                {
                    ModelState.AddModelError("UserName", "Input only one textbox");
                    ModelState.AddModelError("MachineName", "Input only one textbox");
                }

                var sr = deviceviewmodel.SerialNumber.Replace("\r", "").Replace("\n", "").Replace("\t", "").ToString();

                if (deviceviewmodel.IPAddress != null)
                {
                    var ip = deviceviewmodel.IPAddress.Replace("\r", "").Replace("\n", "").Replace("\t", "").ToString();
                    if (db.Devices.Any(d => d.IPAddress.Trim() == ip.Trim() && d.IPAddress != null && d.IPAddress.Trim() != "DHCP"))
                    {
                        ModelState.AddModelError("IPAddress", "IPAddress is Duplicated");
                    }
                }

                if (db.Devices.Any(d => d.SerialNumber.Trim() == deviceviewmodel.SerialNumber.Trim()))
                {
                    ModelState.AddModelError("SerialNumber", "SerialNumber is Duplicated");
                }

                if (db.Devices.Any(d => d.SerialNumber.Trim() == sr.Trim()))
                {
                    ModelState.AddModelError("SerialNumber", "SerialNumber is Duplicated");
                }

                if (ModelState.IsValid)
                {
                    Device device = new Device();
                    device.DeviceName = deviceviewmodel.DeviceName;
                    device.SerialNumber = deviceviewmodel.SerialNumber;
                    device.IPAddress = deviceviewmodel.IPAddress;
                    device.DateCreate = DateTime.Now;
                    device.DateUpdate = DateTime.Now;
                    device.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
                    device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;

                    if (deviceviewmodel.MachineName == null)
                    {
                        device.UserID = db.Users.Where(b => b.FullName == deviceviewmodel.UserName).Select(b => b.UserID).DefaultIfEmpty().First();
                        device.PlantID = db.Users.Where(b => b.FullName == deviceviewmodel.UserName).Select(b => b.PlantID).DefaultIfEmpty().First();
                        device.DepartmentID = db.Users.Where(b => b.FullName == deviceviewmodel.UserName).Select(b => b.DepartmentID).DefaultIfEmpty().First();
                        device.LocationID = db.Users.Where(b => b.FullName == deviceviewmodel.UserName).Select(b => b.LocationID).DefaultIfEmpty().First();
                        device.PhaseID = db.Users.Where(b => b.FullName == deviceviewmodel.UserName).Select(b => b.PhaseID).DefaultIfEmpty().First();
                        device.PhaseName = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                    }
                    else if (deviceviewmodel.UserName == null)
                    {
                        device.MachineID = db.Machines.Where(b => b.MachineName == deviceviewmodel.MachineName).Select(b => b.MachineID).DefaultIfEmpty().First();
                        device.PlantID = db.Machines.Where(b => b.MachineName == deviceviewmodel.MachineName).Select(b => b.PlantID).DefaultIfEmpty().First();
                        device.DepartmentID = db.Machines.Where(b => b.MachineName == deviceviewmodel.MachineName).Select(b => b.DepartmentID).DefaultIfEmpty().First();
                        device.LocationID = db.Machines.Where(b => b.MachineName == deviceviewmodel.MachineName).Select(b => b.LocationID).DefaultIfEmpty().First();
                        device.PhaseID = db.Machines.Where(b => b.MachineName == deviceviewmodel.MachineName).Select(b => b.PhaseID).DefaultIfEmpty().First();
                        device.PhaseName = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                    }

                    device.StatusID = 1;
                    device.LocationStockName = "PBX Room";
                    device.LocationStockID = 1;
                    device.InstockDate = DateTime.Today;
                    device.InstockDate = DateTime.Now;
                    device.ModelName = deviceviewmodel.ModelName;
                    device.MachineName = deviceviewmodel.MachineName;

                    if(deviceviewmodel.IsAsset == true)
                    {
                        device.Description = "5k";
                    }

                    device.ModelID = db.Models.Where(b => b.ModelName == deviceviewmodel.ModelName).Select(b => b.ModelID).DefaultIfEmpty().First();
                    device.StatusName = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                    device.BrandID = db.Models.Where(b => b.ModelName == deviceviewmodel.ModelName).Select(b => b.BrandID).DefaultIfEmpty().First();
                    device.Specification = db.Models.Where(b => b.ModelName == deviceviewmodel.ModelName).Select(b => b.Specification).DefaultIfEmpty().First();
                    device.DeviceTypeID = db.Models.Where(b => b.ModelName == deviceviewmodel.ModelName).Select(b => b.DeviceTypeID).DefaultIfEmpty().First();
                    device.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                    device.BrandName = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                    device.PhaseName = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                    device.FixAccess = deviceviewmodel.FixAccess;
                    device.PRNumber = deviceviewmodel.PRNumber;
                    device.UserName = deviceviewmodel.UserName;

                    RecordInstock recordinstock = new RecordInstock();
                    recordinstock.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                    recordinstock.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                    recordinstock.InstockBy = System.Web.HttpContext.Current.User.Identity.Name;
                    recordinstock.DateInstock = device.DateCreate;
                    recordinstock.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                    recordinstock.SerialNumber = deviceviewmodel.SerialNumber;
                    recordinstock.Plant = "";
                    recordinstock.Department = "";
                    recordinstock.Location = "";
                    recordinstock.Phase = "";
                    recordinstock.LocationStock = "PBX Room";
                    recordinstock.Machine = "";
                    recordinstock.Status = "In Stock";
                    if (deviceviewmodel.IsAsset == true)
                    {
                        recordinstock.IsFixAsset = "Asset";
                    }

                    RecordRequisition recordRequisition = new RecordRequisition();
                    recordRequisition.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                    recordRequisition.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                    recordRequisition.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                    recordRequisition.DateRequisition = device.DateUpdate;
                    recordRequisition.Cause = "เบิกไปใช้งาน";
                    recordRequisition.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                    recordRequisition.SerialNumber = deviceviewmodel.SerialNumber;
                    recordRequisition.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                    recordRequisition.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                    recordRequisition.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                    recordRequisition.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                    recordRequisition.LocationStock = "PBX Room";
                    recordRequisition.Machine = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                    recordRequisition.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                    recordRequisition.UserName = deviceviewmodel.UserName;
                    recordRequisition.DeviceName = deviceviewmodel.DeviceName;
                    if (deviceviewmodel.IsAsset == true)
                    {
                        recordRequisition.IsFixAsset = "Asset";
                    }

                    db.Devices.Add(device);
                    db.RecordInstocks.Add(recordinstock);
                    db.RecordRequisitions.Add(recordRequisition);
                    db.SaveChanges();
                    return RedirectToAction("LastCreate");
                }
                return View(deviceviewmodel);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [AuthLog(Roles = "SuperUser")]
        // GET: /Device/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            device.PreSerialNumber = db.Devices.Where(d => d.DeviceID == id).Select(d => d.SerialNumber).DefaultIfEmpty().First();
            device.PreIPAddress = db.Devices.Where(d => d.DeviceID == id).Select(d => d.IPAddress).DefaultIfEmpty().First();
            device.DateUpdate = DateTime.Now;
            device.Uri = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
            if (device.Description == "5k")
            {
                device.IsAsset = true;
            }
            if (device == null)
            {
                return HttpNotFound();
            }

            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName",device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName",device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type",device.DeviceTypeID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName",device.LocationID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationName", "LocationName", device.LocationStockID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName",device.MachineID);
            ViewBag.UserID = new SelectList(db.Users.OrderBy(d => d.FullName), "UserID", "FullName", device.UserID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName",device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName",device.PlantID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1",device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            ViewBag.PreviousUrl = System.Web.HttpContext.Current.Request.UrlReferrer;
            return View(device);
        }

        // POST: /Device/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [AuthLog(Roles = "SuperUser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DeviceID,UserID,MachineID,UMachineID,DeviceName,Description,SerialNumber,Specification,DepartmentID,PlantID,LocationID,DeviceTypeID,BrandID,StatusID,ModelID,DateCreate,DateUpdate,CreateBy,UpdateBy,LocationStockID,LocationStockName,ModelName,Type,BrandName,StatusName,InstockDate,PhaseID,PhaseName,PRNumber,FixAccess,IPAddress,IsAsset,PreSerialNumber,PreIPAddress,Uri,MacAddress")] Device device)
        {
            var sr = device.SerialNumber.Replace("\r", "").Replace("\n", "").Replace("\t", "").ToString();
            var prevsr = device.PreSerialNumber.Replace("\r", "").Replace("\n", "").Replace("\t", "").ToString();

            if (device.IPAddress != null && device.PreIPAddress != null)
            {
                var ip = device.IPAddress.Replace("\r", "").Replace("\n", "").Replace("\t", "").ToString();
                var previp = device.PreIPAddress.Replace("\r", "").Replace("\n", "").Replace("\t", "").ToString();

                var ip1 = ip.Trim();
                var previp1 = previp.Trim();

                if (previp.Trim() != ip.Trim())
                {
                    if (db.Devices.Any(d => d.IPAddress.Trim() == ip.Trim() && d.IPAddress.Trim() != null && ip.Trim() != "DHCP"))
                    {
                        ModelState.AddModelError("IPAddress", "IPAddress is Duplicated");
                    }
                    device.IPAddress = ip1;
                }
            }
            else if (device.IPAddress != null && device.PreIPAddress == null)
            {
                var ip = device.IPAddress.Replace("\r", "").Replace("\n", "").Replace("\t", "").ToString();
                var previp = " ";

                if (previp.Trim() != ip.Trim())
                {
                    if (db.Devices.Any(d => d.IPAddress.Trim() == ip.Trim() && d.IPAddress.Trim() != null && ip.Trim() != "DHCP"))
                    {
                        ModelState.AddModelError("IPAddress", "IPAddress is Duplicated");
                    }
                    device.IPAddress = ip.Trim();
                }
            }

            if (sr.Trim() != prevsr.Trim())
            {
                if (db.Devices.Any(d => d.SerialNumber.Trim() == sr.Trim()))
                {
                    ModelState.AddModelError("SerialNumber", "SerialNumber is Duplicated");
                }
                device.SerialNumber = sr;
            }

            if (ModelState.IsValid)
            {
                var exStatusName = device.StatusName;
                var exStatusID = db.Status.Where(s => s.Status1 == exStatusName).Select(s => s.StatusID).DefaultIfEmpty().First();
                if (User.Identity.Name == "rp15082")
                {
                    device.StatusName = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                }
                else if (User.Identity.Name != "rp15082")
                {
                    device.StatusName = exStatusName;
                    device.StatusID = exStatusID;
                }

                device.ModelID = db.Models.Where(b => b.ModelName == device.ModelName).Select(b => b.ModelID).DefaultIfEmpty().First();
                //device.StatusName = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                device.BrandID = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.BrandID).DefaultIfEmpty().First();
                device.Specification = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.Specification).DefaultIfEmpty().First();
                device.DeviceTypeID = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.DeviceTypeID).DefaultIfEmpty().First();
                device.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                device.BrandName = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                device.LocationStockID = db.LocationStocks.Where(b => b.LocationName == device.LocationStockName).Select(b => b.LocationID).DefaultIfEmpty().First();
                device.PhaseName = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                if (device.IsAsset == true)
                {
                    device.Description = "5k";
                }

                RecordDevice recorddevice = new RecordDevice();
                recorddevice.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recorddevice.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                recorddevice.EditBy = System.Web.HttpContext.Current.User.Identity.Name;
                recorddevice.EditDate = device.DateUpdate;
                recorddevice.Description = "Edit Device";
                recorddevice.Specification = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.Specification).DefaultIfEmpty().First();
                recorddevice.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recorddevice.SerialNumber = db.Devices.Where(b => b.DeviceID == device.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                recorddevice.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                recorddevice.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                recorddevice.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                recorddevice.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                recorddevice.LocationStock = db.LocationStocks.Where(b => b.LocationName == device.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                recorddevice.Machine = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                recorddevice.UserName = db.Users.Where(b => b.UserID == device.UserID).Select(b => b.FullName).DefaultIfEmpty().First();
                recorddevice.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();

                db.Entry(device).State = EntityState.Modified;
                db.RecordDevices.Add(recorddevice);
                db.SaveChanges();
                //return RedirectToAction("Index");
                return RedirectToAction("LastSet", "Device", new { id = device.DeviceID, uri = device.Uri });
            }

            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationName", "LocationName", device.LocationStockID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.UserID = new SelectList(db.Users.OrderBy(d => d.FullName), "UserID", "FullName", device.UserID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        public ActionResult SetOverRequistion(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            device.DateUpdate = DateTime.Now;
            if (device == null)
            {
                return HttpNotFound();
            }
            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
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
        public ActionResult SetOverRequistion([Bind(Include = "DeviceID,MachineID,UserID,UMachineID,DeviceName,Description,SerialNumber,Specification,DepartmentID,PlantID,LocationID,DeviceTypeID,BrandID,StatusID,ModelID,DateCreate,DateUpdate,CreateBy,UpdateBy,LocationStockID,LocationStockName,ModelName,Type,BrandName,StatusName,CauseRequistion,InstockDate,PhaseID,PhaseName,MachineName,PRNumber,FixAccess,UserName,DeviceName,IPAddress")] Device device)
        {
            var currentID = db.Devices.Where(b => b.SerialNumber == device.SerialNumber).Select(b => b.DeviceID).DefaultIfEmpty().First();

            if (db.Devices.Any(d => d.IPAddress == device.IPAddress && d.IPAddress != null && device.IPAddress != "DHCP" && currentID == device.DeviceID && d.StatusID == 1))
            {
                ModelState.AddModelError("IPAddress", "IPAddress is Duplicated");
            }

            if ((!string.IsNullOrEmpty(device.MachineName)) && (!string.IsNullOrEmpty(device.UserName)))
            {
                ModelState.AddModelError("UserName", "Input only one textbox");
                ModelState.AddModelError("MachineName", "Input only one textbox");
            }
            if ((string.IsNullOrEmpty(device.MachineName)) && (string.IsNullOrEmpty(device.UserName)))
            {
                ModelState.AddModelError("UserName", "Input only one textbox");
                ModelState.AddModelError("MachineName", "Input only one textbox");
            }
            if (string.IsNullOrEmpty(device.CauseRequistion))
            {
                ModelState.AddModelError("CauseRequistion", "CauseRequistion is Required");
            }

            if (ModelState.IsValid)
            {
                device.StatusID = 1;
                device.StatusName = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();

                if (string.IsNullOrEmpty(device.MachineName))
                {
                    device.UserID = db.Users.Where(b => b.FullName == device.UserName).Select(b => b.UserID).DefaultIfEmpty().First();
                    device.PlantID = db.Users.Where(b => b.FullName == device.UserName).Select(b => b.PlantID).DefaultIfEmpty().First();
                    device.DepartmentID = db.Users.Where(b => b.FullName == device.UserName).Select(b => b.DepartmentID).DefaultIfEmpty().First();
                    device.LocationID = db.Users.Where(b => b.FullName == device.UserName).Select(b => b.LocationID).DefaultIfEmpty().First();
                    device.PhaseID = db.Users.Where(b => b.FullName == device.UserName).Select(b => b.PhaseID).DefaultIfEmpty().First();
                    device.PhaseName = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                }
                else if (string.IsNullOrEmpty(device.UserName))
                {
                    device.MachineID = db.Machines.Where(b => b.MachineName == device.MachineName).Select(b => b.MachineID).DefaultIfEmpty().First();
                    device.PlantID = db.Machines.Where(b => b.MachineName == device.MachineName).Select(b => b.PlantID).DefaultIfEmpty().First();
                    device.DepartmentID = db.Machines.Where(b => b.MachineName == device.MachineName).Select(b => b.DepartmentID).DefaultIfEmpty().First();
                    device.LocationID = db.Machines.Where(b => b.MachineName == device.MachineName).Select(b => b.LocationID).DefaultIfEmpty().First();
                    device.PhaseID = db.Machines.Where(b => b.MachineName == device.MachineName).Select(b => b.PhaseID).DefaultIfEmpty().First();
                    device.PhaseName = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                }

                RecordRequisition recordRequisition = new RecordRequisition();
                recordRequisition.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recordRequisition.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                recordRequisition.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                recordRequisition.DateRequisition = device.DateUpdate;
                recordRequisition.Cause = device.CauseRequistion;
                recordRequisition.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recordRequisition.SerialNumber = db.Devices.Where(b => b.DeviceID == device.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                recordRequisition.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                recordRequisition.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                recordRequisition.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordRequisition.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                recordRequisition.LocationStock = db.LocationStocks.Where(b => b.LocationName == device.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordRequisition.Machine = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                recordRequisition.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                recordRequisition.UserName = device.UserName;
                recordRequisition.DeviceName = device.DeviceName;

                if(device.Description == "5k")
                {
                    recordRequisition.IsFixAsset = "Asset";
                }

                db.Entry(device).State = EntityState.Modified;
                db.RecordRequisitions.Add(recordRequisition);
                db.SaveChanges();
                return RedirectToAction("LastSet", "Device", new { id = device.DeviceID });
            }

            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        // GET: /Device/SetRequistion/5
        [Authorize]
        public ActionResult SetRequistion(int? id, string uri)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            var model = device.ModelID;
            var type = device.DeviceTypeID;
            device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            device.DateUpdate = DateTime.Now;
            device.Uri = uri;
            if (device == null)
            {
                return HttpNotFound();
            }
            else if (device.StatusID != 3 && device.StatusID != 5 && device.StatusID != 10)
            {
                return Content("Current status can't requisition");
            }
            ViewBag.type = type;
            ViewBag.Model = model;
            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            ViewBag.URI = uri;
            return View(device);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetRequistion([Bind(Include = "DeviceID,MachineID,UserID,UMachineID,DeviceName,Description,SerialNumber,Specification,DepartmentID,PlantID,LocationID,DeviceTypeID,BrandID,StatusID,ModelID,DateCreate,DateUpdate,CreateBy,UpdateBy,LocationStockID,LocationStockName,ModelName,Type,BrandName,StatusName,CauseRequistion,InstockDate,PhaseID,PhaseName,MachineName,PRNumber,FixAccess,UserName,UserName2,UserName3,UserName4,DeviceName,IPAddress,IsAsset,Confirm,Uri,MacAddress,PhoneNumber1,PhoneNumber2,PhoneNumber3,PhoneNumber4,Reason")] Device device, string URI)
        {
            #region initialVariables
            var currentID = db.Devices.Where(b => b.SerialNumber == device.SerialNumber).Select(b => b.DeviceID).DefaultIfEmpty().First();
            var Mid = db.Machines.Where(b => b.MachineName == device.MachineName).Select(b => b.MachineID).DefaultIfEmpty().First();
            var Mqty = db.Devices.Where(d => d.DeviceTypeID == device.DeviceTypeID && d.MachineID == Mid && d.StatusID == 1).Count();
            var MPlant = db.Machines.Where(m => m.MachineID == Mid).Select(m => m.Plant.PlantName).DefaultIfEmpty().First();
            var subMachineName = "";
            if(device.MachineName != null)
            {
                subMachineName = device.MachineName.Substring(0, 3);
            }else if(device.MachineName == null)
            {
                subMachineName = "";
            }

            var Uid = db.Users.Where(b => b.FullName == device.UserName).Select(b => b.UserID).DefaultIfEmpty().First();
            var Uqty = db.Devices.Where(d => d.DeviceTypeID == device.DeviceTypeID && d.UserID == Uid && d.StatusID == 1).Count();
            var UPlant = db.Users.Where(u => u.UserID == Uid).Select(u => u.Plant.PlantName).DefaultIfEmpty().First();

            #endregion

            #region checkDuplicatedIPAddress
            if (db.Devices.Any(d => d.IPAddress == device.IPAddress && d.IPAddress != null  && device.IPAddress != "DHCP" && currentID == device.DeviceID && d.StatusID == 1))
                {
                    ModelState.AddModelError("IPAddress", "IPAddress is Duplicated");
                }
            #endregion

            #region checkValidationInput
            if ((!string.IsNullOrEmpty(device.MachineName)) && (!string.IsNullOrEmpty(device.UserName)))
            {
                ModelState.AddModelError("UserName", "Input only one textbox");
                ModelState.AddModelError("MachineName", "Input only one textbox");
                ViewBag.Validation = "Active";
            }
            if ((string.IsNullOrEmpty(device.MachineName)) && (string.IsNullOrEmpty(device.UserName)))
            {
                ModelState.AddModelError("UserName", "Input only one textbox");
                ModelState.AddModelError("MachineName", "Input only one textbox");
                ViewBag.Validation = "Active";
            }
            if (string.IsNullOrEmpty(device.CauseRequistion))
            {
                ModelState.AddModelError("CauseRequistion", "CauseRequistion is Required");
                ViewBag.Validation = "Active";
            }
            //if ((device.DeviceTypeID == 51 || device.DeviceTypeID == 83) && (string.IsNullOrEmpty(device.PhoneNumber1)))
            //{
            //    ModelState.AddModelError("PhoneNumber1", "PhoneNumber is Required");
            //}
            #endregion

            int? LimitQuantity;
            int CurrentQuantity = db.Devices.Where(d => d.Machine.MachineName == device.MachineName && d.DeviceType.Type == device.Type && d.StatusID == 1).Count();
            int LimitID = db.LimitDeviceQuantities.Where(m => m.Machine == device.MachineName && m.DeviceType == device.Type).Select(m => m.ID).DefaultIfEmpty().First();

            if (LimitID != 0)
            {
                LimitQuantity = db.LimitDeviceQuantities.Where(m => m.ID == LimitID).Select(m => m.MaxQuantity).DefaultIfEmpty().First();
                if (CurrentQuantity >= LimitQuantity)
                {
                    ModelState.AddModelError("MachineName", "Limit Device");
                }
            }
            else if (LimitID == 0)
            {
                if(Mqty >= 1)
                {
                    ModelState.AddModelError("MachineName", "Limit Device");
                }
            }

            #region confirmDuplicateRequisitioDeviceType
            if ((Uqty <= 1) && (Uqty != 0) && device.Confirm == false)
            {
                ViewBag.UserQTY = Uqty;
                ViewBag.MaxQTY = Mqty;
                ViewBag.UserName = device.UserName;
                ViewBag.MachineName = device.MachineName;
                ViewBag.DType = device.Type;
                ModelState.AddModelError("Confirm", "Check box to Confirm requisition");
            }
            #endregion

            #region checkDuplicatePhoneNumber
            if (db.Users.Any(d => d.Phone == device.PhoneNumber1 && d.Phone != null && device.PhoneNumber1 != null))
            {
                ModelState.AddModelError("PhoneNumber1", "PhoneNumber is Duplicated");
            }

            if (db.Users.Any(d => d.Phone == device.PhoneNumber2 && d.Phone != null && device.PhoneNumber2 != null))
            {
                ModelState.AddModelError("PhoneNumber2", "PhoneNumber is Duplicated");
            }

            if (db.Users.Any(d => d.Phone == device.PhoneNumber3 && d.Phone != null && device.PhoneNumber3 != null))
            {
                ModelState.AddModelError("PhoneNumber3", "PhoneNumber is Duplicated");
            }
            if (db.Users.Any(d => d.Phone == device.PhoneNumber4 && d.Phone != null && device.PhoneNumber4 != null))
            {
                ModelState.AddModelError("PhoneNumber4", "PhoneNumber is Duplicated");
            }
            #endregion

            #region LimitPhoneNumber
            var list = new List<string> { device.PhoneNumber1, device.PhoneNumber2, device.PhoneNumber3, device.PhoneNumber4 };
            var q = list.Where(x => x != null).GroupBy(x => x).Count();

            if (q > 2)
            {
                ViewBag.PhoneNumberExceed = "PhoneNumber is Exceed";
                ModelState.AddModelError(" ", "PhoneNumber is Exceed");
            }
            #endregion

            if (ModelState.IsValid)
            {
                #region setValues
                device.StatusID = 1;
                device.StatusName = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();

                if (string.IsNullOrEmpty(device.MachineName))
                {
                    #region setPhoneNumber
                    var userID1 = db.Users.Where(b => b.FullName == device.UserName).Select(b => b.UserID).DefaultIfEmpty().First();
                    var userID2 = db.Users.Where(b => b.FullName == device.UserName2).Select(b => b.UserID).DefaultIfEmpty().First();
                    var userID3 = db.Users.Where(b => b.FullName == device.UserName3).Select(b => b.UserID).DefaultIfEmpty().First();
                    var userID4 = db.Users.Where(b => b.FullName == device.UserName4).Select(b => b.UserID).DefaultIfEmpty().First();

                    if (device.DeviceTypeID == 83 || device.DeviceTypeID == 51)
                    {
                        if(userID1 != 0)
                        {
                            User user = db.Users.Find(userID1);
                            user.Phone = device.PhoneNumber1;
                            db.Entry(user).State = EntityState.Modified;
                            db.SaveChanges();
                            if (userID2 != 0)
                            {
                                User user2 = db.Users.Find(userID2);
                                user2.Phone = device.PhoneNumber2;
                                db.Entry(user2).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            if (userID3 != 0)
                            {
                                User user3 = db.Users.Find(userID3);
                                user3.Phone = device.PhoneNumber3;
                                db.Entry(user3).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            if (userID4 != 0)
                            {
                                User user4 = db.Users.Find(userID4);
                                user4.Phone = device.PhoneNumber4;
                                db.Entry(user4).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                    #endregion

                    device.Reason = device.CauseRequistion;
                    device.UserID = db.Users.Where(b => b.FullName == device.UserName).Select(b => b.UserID).DefaultIfEmpty().First();
                    device.PlantID = db.Users.Where(b => b.FullName == device.UserName).Select(b => b.PlantID).DefaultIfEmpty().First();
                    device.DepartmentID = db.Users.Where(b => b.FullName == device.UserName).Select(b => b.DepartmentID).DefaultIfEmpty().First();
                    device.LocationID = db.Users.Where(b => b.FullName == device.UserName).Select(b => b.LocationID).DefaultIfEmpty().First();
                    device.PhaseID = db.Users.Where(b => b.FullName == device.UserName).Select(b => b.PhaseID).DefaultIfEmpty().First();
                    device.PhaseName = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                }
                else if (string.IsNullOrEmpty(device.UserName))
                {
                    device.Reason = device.CauseRequistion;
                    device.MachineID = db.Machines.Where(b => b.MachineName == device.MachineName).Select(b => b.MachineID).DefaultIfEmpty().First();
                    device.PlantID = db.Machines.Where(b => b.MachineName == device.MachineName).Select(b => b.PlantID).DefaultIfEmpty().First();
                    device.DepartmentID = db.Machines.Where(b => b.MachineName == device.MachineName).Select(b => b.DepartmentID).DefaultIfEmpty().First();
                    device.LocationID = db.Machines.Where(b => b.MachineName == device.MachineName).Select(b => b.LocationID).DefaultIfEmpty().First();
                    device.PhaseID = db.Machines.Where(b => b.MachineName == device.MachineName).Select(b => b.PhaseID).DefaultIfEmpty().First();
                    device.PhaseName = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                }
                #endregion
                #region LogFile
                RecordRequisition recordRequisition = new RecordRequisition();
                recordRequisition.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recordRequisition.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                recordRequisition.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                recordRequisition.DateRequisition = device.DateUpdate;
                recordRequisition.Cause = device.CauseRequistion;
                recordRequisition.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recordRequisition.SerialNumber = db.Devices.Where(b => b.DeviceID == device.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                recordRequisition.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                recordRequisition.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                recordRequisition.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordRequisition.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                recordRequisition.LocationStock = db.LocationStocks.Where(b => b.LocationName == device.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordRequisition.Machine = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                recordRequisition.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                recordRequisition.UserName = device.UserName;
                recordRequisition.DeviceName = device.DeviceName;
                if(device.Description == "5k")
                {
                    recordRequisition.IsFixAsset = "Asset";
                }

                db.Entry(device).State = EntityState.Modified;
                db.RecordRequisitions.Add(recordRequisition);
                db.SaveChanges();
                #endregion
                return RedirectToAction("LastSet", "Device", new { id = device.DeviceID, uri = device.Uri });
            }

            #region ViewBage
            ViewBag.Type = device.DeviceTypeID;
            ViewBag.Model = device.ModelID;
            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            #endregion
            return View(device);
        }

        [Authorize]
        public ActionResult WarningRequistion(int? id, string Mqty, string Uqty, string MachineName, string UserName, string Type )
        {
            var devices = db.Devices.Where(d => d.DeviceID == id).Include(d => d.Brand).Include(d => d.Department).Include(d => d.DeviceType).Include(d => d.Location).Include(d => d.Machine).Include(d => d.Model).Include(d => d.Plant).Include(d => d.Status).Include(d => d.UserMachine).OrderByDescending(d => d.DeviceID);
            ViewBag.Mqty = Mqty;
            ViewBag.Uqty = Uqty;
            ViewBag.MachineName = MachineName;
            ViewBag.UserName = UserName;
            ViewBag.Type = Type;
            ViewBag.DeviceID = id;
            return View(devices.ToList());
        }

        [Authorize]
        public ActionResult SetRepair(int? id, string uri)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            device.DateUpdate = DateTime.Now;
            device.Uri = uri;
            string CurPla = db.Devices.Where(d => d.DeviceID == id).Select(d => d.Plant.PlantName).DefaultIfEmpty().First();
            string CurDept = db.Devices.Where(d => d.DeviceID == id).Select(d => d.Department.DepartmentName).DefaultIfEmpty().First();
            string CurLoc = db.Devices.Where(d => d.DeviceID == id).Select(d => d.Location.LocationName).DefaultIfEmpty().First();
            string CurPhs = db.Devices.Where(d => d.DeviceID == id).Select(d => d.PhaseName).DefaultIfEmpty().First();

            if (string.IsNullOrEmpty(CurPla) && string.IsNullOrEmpty(CurDept) && string.IsNullOrEmpty(CurLoc) && string.IsNullOrEmpty(CurPhs))
            {
                device.CurrentLocation = device.LocationStockName;
            }
            else
            {
                device.CurrentLocation = CurPla+" ," + CurDept+ " ,"+ CurLoc+ " ," + CurPhs;
            }
            if (device == null)
            {
                return HttpNotFound();
            }
            else if (device.StatusID != 1 && device.StatusID != 3 && device.StatusID != 10)
            {
                return Content("Current status can't set for in repair");
            }
            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            ViewBag.URI = uri;
            return View(device);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetRepair([Bind(Include = "DeviceID,UserID,MachineID,UMachineID,DeviceName,Description,SerialNumber,Specification,DepartmentID,PlantID,LocationID,DeviceTypeID,BrandID,StatusID,ModelID,DateCreate,DateUpdate,CreateBy,UpdateBy,LocationStockID,LocationStockName,ModelName,Type,BrandName,StatusName,CauseRequistion,InstockDate,PhaseID,PhaseName,MachineName,PRNumber,FixAccess,UserName,CurrentLocation,IPAddress,IsAsset,Uri,MacAddress,Confirm")] Device device, string URI)
        {
            int MachineID = db.Machines.Where(m => m.MachineName == device.MachineName).Select(m => m.MachineID).DefaultIfEmpty().First();

            if (string.IsNullOrEmpty(device.CauseRequistion))
            {
                ModelState.AddModelError("CauseRequistion", "CauseRepair is Required");
            }
            if (string.IsNullOrEmpty(device.LocationStockID.ToString()))
            {
                ModelState.AddModelError("LocationStockID", "LocationStockID is Required");
            }
            if (device.StatusID == 10 && MachineID == 0)
            {
                if (string.IsNullOrEmpty(device.MachineName) || MachineID == 0)
                {
                    ModelState.AddModelError("MachineName", "MachineName is Required");
                }
            }

            if (device.StatusID == 3 && device.Confirm == false && MachineID == 0)
            {
                if (string.IsNullOrEmpty(device.MachineName) && device.Confirm == false || MachineID == 0)
                {
                    ModelState.AddModelError("MachineName", "MachineName is Required");
                }
            }

            if (ModelState.IsValid)
            {
                device.StatusID = 2;
                device.StatusName = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                device.PhaseName = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                device.BrandName = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                device.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                device.IPAddress = null;
                device.LocationStockName = db.LocationStocks.Where(b => b.LocationID == device.LocationStockID).Select(b => b.LocationName).DefaultIfEmpty().First();
                device.Reason = device.CauseRequistion;

                if (MachineID != 0)
                {
                    device.MachineID = MachineID;
                    device.PlantID = db.Machines.Where(m => m.MachineID == MachineID).Select(m => m.PlantID).DefaultIfEmpty().First();
                    device.DepartmentID = db.Machines.Where(m => m.MachineID == MachineID).Select(m => m.DepartmentID).DefaultIfEmpty().First();
                    device.LocationID = db.Machines.Where(m => m.MachineID == MachineID).Select(m => m.LocationID).DefaultIfEmpty().First();
                    device.PhaseID = db.Machines.Where(m => m.MachineID == MachineID).Select(m => m.PhaseID).DefaultIfEmpty().First();
                    device.PhaseName = db.Phases.Where(p => p.PhaseID == device.PhaseID).Select(m => m.PhaseName).DefaultIfEmpty().First();
                }


                RecordInRepair recordinrepair = new RecordInRepair();
                recordinrepair.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recordinrepair.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                recordinrepair.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                recordinrepair.DateRequest = device.DateUpdate;
                recordinrepair.Cause = device.CauseRequistion;
                recordinrepair.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recordinrepair.SerialNumber = db.Devices.Where(b => b.DeviceID == device.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                recordinrepair.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                recordinrepair.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                recordinrepair.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordinrepair.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                recordinrepair.LocationStock = db.LocationStocks.Where(b => b.LocationName == device.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordinrepair.Machine = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                recordinrepair.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                recordinrepair.UserName = db.Users.Where(b => b.UserID == device.UserID).Select(b => b.FullName).DefaultIfEmpty().First();
                if(device.Description == "5k")
                {
                    recordinrepair.IsFixAsset = "Asset";
                }
                
                db.Entry(device).State = EntityState.Modified;
                db.RecordInRepairs.Add(recordinrepair);
                db.SaveChanges();
                return RedirectToAction("LastSet", "Device", new { id = device.DeviceID, uri = device.Uri});
            }

            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            ViewBag.MachineName = MachineID;
            return View(device);
        }

        [Authorize]
        public ActionResult SetRepairPrice(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            device.DateUpdate = DateTime.Now;
            if (device == null)
            {
                return HttpNotFound();
            }
            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        [Authorize]
        public ActionResult SetWaitSentSale(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            device.DateUpdate = DateTime.Now;
            if (device == null)
            {
                return HttpNotFound();
            }
            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetWaitSentSale([Bind(Include = "DeviceID,MachineID,UMachineID,DeviceName,Description,SerialNumber,Specification,DepartmentID,PlantID,LocationID,DeviceTypeID,BrandID,StatusID,ModelID,DateCreate,DateUpdate,CreateBy,UpdateBy,LocationStockID,LocationStockName,ModelName,Type,BrandName,StatusName,CauseRequistion,InstockDate,PhaseID,PhaseName,MachineName,PRNumber,FixAccess")] Device device)
        {
            if (ModelState.IsValid)
            {
                device.StatusID = 9;
                device.StatusName = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                device.BrandName = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                device.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();

                RecordSale recordsale = new RecordSale();
                recordsale.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recordsale.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                recordsale.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                recordsale.DateRequest = device.DateUpdate;
                recordsale.Cause = "Wait Sent Sale";
                recordsale.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recordsale.SerialNumber = db.Devices.Where(b => b.DeviceID == device.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                recordsale.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                recordsale.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                recordsale.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordsale.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                recordsale.LocationStock = db.LocationStocks.Where(b => b.LocationName == device.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordsale.Machine = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                recordsale.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                db.Entry(device).State = EntityState.Modified;
                db.RecordSales.Add(recordsale);
                db.SaveChanges();
                return RedirectToAction("LastSet", "Device", new { id = device.DeviceID });
            }

            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        [Authorize]
        public ActionResult setUnknownStatus(int? id, string uri)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            var model = device.ModelID;
            var type = device.DeviceTypeID;
            device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            device.DateUpdate = DateTime.Now;
            device.Uri = uri;
            if (device == null)
            {
                return HttpNotFound();
            }
            ViewBag.type = type;
            ViewBag.Model = model;
            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            ViewBag.URI = uri;
            return View(device);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult setUnknownStatus([Bind(Include = "DeviceID,MachineID,UserID,UMachineID,DeviceName,Description,SerialNumber,Specification,DepartmentID,PlantID,LocationID,DeviceTypeID,BrandID,StatusID,ModelID,DateCreate,DateUpdate,CreateBy,UpdateBy,LocationStockID,LocationStockName,ModelName,Type,BrandName,StatusName,CauseRequistion,InstockDate,PhaseID,PhaseName,MachineName,PRNumber,FixAccess,UserName,UserName2,UserName3,UserName4,DeviceName,IPAddress,IsAsset,Confirm,Uri,MacAddress,PhoneNumber1,PhoneNumber2,PhoneNumber3,PhoneNumber4")] Device device, string URI)
        {
            if (ModelState.IsValid)
            {
                device.StatusID = 10;
                device.StatusName = "Unknown";
                #region LogFile
                RecordDevice recordDevice = new RecordDevice();
                recordDevice.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recordDevice.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                recordDevice.EditBy = System.Web.HttpContext.Current.User.Identity.Name;
                recordDevice.EditDate = device.DateUpdate;
                recordDevice.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recordDevice.SerialNumber = db.Devices.Where(b => b.DeviceID == device.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                recordDevice.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                recordDevice.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                recordDevice.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordDevice.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                recordDevice.LocationStock = db.LocationStocks.Where(b => b.LocationName == device.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordDevice.Machine = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                recordDevice.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                recordDevice.Description = "Set Unknown";
                recordDevice.UserName = device.UserName;
                recordDevice.DeviceName = device.DeviceName;

                db.Entry(device).State = EntityState.Modified;
                db.RecordDevices.Add(recordDevice);
                db.SaveChanges();
                #endregion
                return RedirectToAction("LastSet", "Device", new { id = device.DeviceID, uri = device.Uri });
            }
            #region ViewBage
            ViewBag.Type = device.DeviceTypeID;
            ViewBag.Model = device.ModelID;
            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            #endregion
            return View(device);
        }

        [Authorize]
        public ActionResult SetWaitSentRepair(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            device.DateUpdate = DateTime.Now;
            if (device == null)
            {
                return HttpNotFound();
            }
            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetWaitSentRepair([Bind(Include = "DeviceID,UserID,MachineID,UMachineID,DeviceName,Description,SerialNumber,Specification,DepartmentID,PlantID,LocationID,DeviceTypeID,BrandID,StatusID,ModelID,DateCreate,DateUpdate,CreateBy,UpdateBy,LocationStockID,LocationStockName,ModelName,Type,BrandName,StatusName,CauseRequistion,InstockDate,PhaseID,PhaseName,MachineName,PRNumber,FixAccess,UserName")] Device device)
        {
            if (ModelState.IsValid)
            {
                device.StatusID = 8;
                device.StatusName = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                device.PhaseName = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                device.BrandName = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                device.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();

                RecordInRepair recordinrepair = new RecordInRepair();
                recordinrepair.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recordinrepair.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                recordinrepair.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                recordinrepair.DateRequest = device.DateUpdate;
                recordinrepair.Cause = "Wait Sent Repair";
                recordinrepair.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recordinrepair.SerialNumber = db.Devices.Where(b => b.DeviceID == device.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                recordinrepair.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                recordinrepair.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                recordinrepair.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordinrepair.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                recordinrepair.LocationStock = db.LocationStocks.Where(b => b.LocationName == device.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordinrepair.Machine = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                recordinrepair.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                recordinrepair.UserName = db.Users.Where(b => b.UserID == device.UserID).Select(b => b.FullName).DefaultIfEmpty().First();

                db.Entry(device).State = EntityState.Modified;
                db.RecordInRepairs.Add(recordinrepair);
                db.SaveChanges();
                return RedirectToAction("LastSet", "Device", new { id = device.DeviceID });
            }

            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        [Authorize]
        public ActionResult SetReceiptList(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            device.DateUpdate = DateTime.Now;
            if (device == null)
            {
                return HttpNotFound();
            }
            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetReceiptList([Bind(Include = "DeviceID,UserID,MachineID,UMachineID,DeviceName,Description,SerialNumber,Specification,DepartmentID,PlantID,LocationID,DeviceTypeID,BrandID,StatusID,ModelID,DateCreate,DateUpdate,CreateBy,UpdateBy,LocationStockID,LocationStockName,ModelName,Type,BrandName,StatusName,CauseRequistion,InstockDate,PhaseID,PhaseName,MachineName,PRNumber,FixAccess,UserName,BillReceiptNo")] Device device)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(device.BillReceiptNo))
            {
                device.StatusID = 8;
                device.StatusName = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                device.PhaseName = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                device.BrandName = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                device.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                RecordInRepair recordinrepair = new RecordInRepair();
                recordinrepair.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recordinrepair.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                recordinrepair.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                recordinrepair.DateRequest = device.DateUpdate;
                recordinrepair.Cause = "Wait Sent Repair";
                recordinrepair.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recordinrepair.SerialNumber = db.Devices.Where(b => b.DeviceID == device.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                recordinrepair.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                recordinrepair.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                recordinrepair.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordinrepair.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                recordinrepair.LocationStock = db.LocationStocks.Where(b => b.LocationName == device.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordinrepair.Machine = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                recordinrepair.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                recordinrepair.UserName = db.Users.Where(b => b.UserID == device.UserID).Select(b => b.FullName).DefaultIfEmpty().First();
                BillReceiptList receiptlist = new BillReceiptList();
                receiptlist.BillReceiptNo = device.BillReceiptNo;
                receiptlist.BillReceiptType = db.BillReceipts.Where(b => b.BillReceiptNo == device.BillReceiptNo).Select(b => b.Type).DefaultIfEmpty().First();
                receiptlist.Brand = device.BrandName;
                receiptlist.CompanyName = db.BillReceipts.Where(b => b.BillReceiptNo == device.BillReceiptNo).Select(b => b.CompanyName).DefaultIfEmpty().First();
                receiptlist.SerialNumber = db.Devices.Where(b => b.DeviceID == device.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                receiptlist.Cause = db.RecordInRepairs.Where(b => b.SerialNumber == device.SerialNumber).Select(b => b.Cause).DefaultIfEmpty().First();
                receiptlist.Model = device.ModelName;
                receiptlist.Type = device.Type;
                receiptlist.DateCreate = DateTime.Now;
                receiptlist.DateUpdate = DateTime.Now;
                receiptlist.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
                receiptlist.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                receiptlist.InRepairDate = device.DateCreate;
                receiptlist.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                receiptlist.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                receiptlist.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                receiptlist.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                receiptlist.MachineName = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                receiptlist.UserName = db.Users.Where(b => b.UserID == device.UserID).Select(b => b.FullName).DefaultIfEmpty().First();

                db.Entry(device).State = EntityState.Modified;
                db.RecordInRepairs.Add(recordinrepair);
                db.BillReceiptLists.Add(receiptlist);
                db.SaveChanges();
                //return RedirectToAction("LastSet", "Device", new { id = device.DeviceID });
                return RedirectToAction("InRepair", "Device");
            }
            ModelState.AddModelError("BillReceiptNo", "BillReceiptNo is Required");

            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        [Authorize]
        public ActionResult SetSentRepair(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            device.DateUpdate = DateTime.Now;
            if (device == null)
            {
                return HttpNotFound();
            }
            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetSentRepair([Bind(Include = "DeviceID,UserID,MachineID,UMachineID,DeviceName,Description,SerialNumber,Specification,DepartmentID,PlantID,LocationID,DeviceTypeID,BrandID,StatusID,ModelID,DateCreate,DateUpdate,CreateBy,UpdateBy,LocationStockID,LocationStockName,ModelName,Type,BrandName,StatusName,CauseRequistion,InstockDate,PhaseID,PhaseName,MachineName,PRNumber,FixAccess,UserName,IsAsset")] Device device)
        {
            if (ModelState.IsValid)
            {
                device.StatusID = 6;
                device.StatusName = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                device.PhaseName = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                device.BrandName = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                device.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                RecordInRepair recordinrepair = new RecordInRepair();
                recordinrepair.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recordinrepair.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                recordinrepair.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                recordinrepair.DateRequest = device.DateUpdate;
                recordinrepair.Cause = "Sent Repair";
                recordinrepair.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recordinrepair.SerialNumber = db.Devices.Where(b => b.DeviceID == device.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                recordinrepair.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                recordinrepair.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                recordinrepair.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordinrepair.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                recordinrepair.LocationStock = db.LocationStocks.Where(b => b.LocationName == device.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordinrepair.Machine = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                recordinrepair.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                recordinrepair.UserName = db.Users.Where(b => b.UserID == device.UserID).Select(b => b.FullName).DefaultIfEmpty().First();
                if (device.IsAsset == true)
                {
                    recordinrepair.IsFixAsset = "Asset";
                }
                
                db.Entry(device).State = EntityState.Modified;
                db.RecordInRepairs.Add(recordinrepair);
                db.SaveChanges();
                return RedirectToAction("LastSet", "Device", new { id = device.DeviceID });
            }

            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        [Authorize]
        public ActionResult SetSell(int? id, string uri)
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
            } else if(device.StatusID != 2 && device.StatusID != 3)
            {
                return Content("Current status can't set for in sale");
            }
            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetSell([Bind(Include = "DeviceID,MachineID,UMachineID,DeviceName,Description,SerialNumber,Specification,DepartmentID,PlantID,LocationID,DeviceTypeID,BrandID,StatusID,ModelID,DateCreate,DateUpdate,CreateBy,UpdateBy,LocationStockID,LocationStockName,ModelName,Type,BrandName,StatusName,CauseRequistion,InstockDate,PhaseID,PhaseName,MachineName,PRNumber,FixAccess,Uri,MacAddress")] Device device)
        {
            if (string.IsNullOrEmpty(device.CauseRequistion))
            {
                ModelState.AddModelError("CauseRequistion", "CauseSell is Required");
            }

            if (ModelState.IsValid)
            {
                device.StatusID = 4;
                device.Reason = device.CauseRequistion;
                device.StatusName = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                device.BrandName = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                device.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                RecordSale recordsale = new RecordSale();
                recordsale.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recordsale.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                recordsale.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                recordsale.DateRequest = device.DateUpdate;
                recordsale.Cause = device.CauseRequistion;
                recordsale.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recordsale.SerialNumber = db.Devices.Where(b => b.DeviceID == device.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                recordsale.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                recordsale.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                recordsale.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordsale.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                recordsale.LocationStock = db.LocationStocks.Where(b => b.LocationName == device.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordsale.Machine = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                recordsale.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                if(device.Description == "5k")
                {
                    recordsale.IsFixAsset = "Asset";
                }
                db.Entry(device).State = EntityState.Modified;
                db.RecordSales.Add(recordsale);
                db.SaveChanges();
                return RedirectToAction("LastSet", "Device", new { id = device.DeviceID, uri = device.Uri });
            }

            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        [Authorize]
        public ActionResult SetSentSell(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            device.DateUpdate = DateTime.Now;
            if (device == null)
            {
                return HttpNotFound();
            }
            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetSentSell([Bind(Include = "DeviceID,MachineID,UMachineID,DeviceName,Description,SerialNumber,Specification,DepartmentID,PlantID,LocationID,DeviceTypeID,BrandID,StatusID,ModelID,DateCreate,DateUpdate,CreateBy,UpdateBy,LocationStockID,LocationStockName,ModelName,Type,BrandName,StatusName,CauseRequistion,InstockDate,PhaseID,PhaseName,MachineName,PRNumber,FixAccess")] Device device)
        {
            if (string.IsNullOrEmpty(device.CauseRequistion))
            {
                ModelState.AddModelError("CauseRequistion", "CauseSell is Required");
            }

            if (ModelState.IsValid)
            {
                device.StatusID = 7;
                device.StatusName = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                device.BrandName = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                device.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();

                RecordSale recordsale = new RecordSale();
                recordsale.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recordsale.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                recordsale.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                recordsale.DateRequest = device.DateUpdate;
                recordsale.Cause = device.CauseRequistion;
                recordsale.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recordsale.SerialNumber = db.Devices.Where(b => b.DeviceID == device.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                recordsale.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                recordsale.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                recordsale.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordsale.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                recordsale.LocationStock = db.LocationStocks.Where(b => b.LocationName == device.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordsale.Machine = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                recordsale.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                if (device.IsAsset == true)
                {
                    recordsale.IsFixAsset = "Asset";
                }
                
                db.Entry(device).State = EntityState.Modified;
                db.RecordSales.Add(recordsale);
                db.SaveChanges();
                return RedirectToAction("LastSet", "Device", new { id = device.DeviceID });
            }

            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        [Authorize]
        public ActionResult SetSpare(int? id, string uri)
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
            else if(device.StatusID != 3)
            {
                return Content("Current status can't set for spare");
            }
            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName", device.MachineID);
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName", device.ModelID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", device.PlantID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.StatusID = new SelectList(db.Status.OrderBy(d => d.Status1), "StatusID", "Status1", device.StatusID);
            ViewBag.UMachineID = new SelectList(db.UserMachines, "UMachineID", "ComputerName", device.UMachineID);
            return View(device);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetSpare([Bind(Include = "DeviceID,MachineID,UMachineID,DeviceName,Description,SerialNumber,Specification,DepartmentID,PlantID,LocationID,DeviceTypeID,BrandID,StatusID,ModelID,DateCreate,DateUpdate,CreateBy,UpdateBy,LocationStockID,LocationStockName,ModelName,Type,BrandName,StatusName,CauseRequistion,InstockDate,PhaseID,PhaseName,MachineName,PRNumber,FixAccess,IsAsset,Uri,MacAddress")] Device device)
        {
            if (string.IsNullOrEmpty(device.MachineName))
            {
                ModelState.AddModelError("MachineName", "MachineName is Required");
            }

            if (ModelState.IsValid)
            {
                device.StatusID = 5;
                device.StatusName = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                device.MachineID = db.Machines.Where(b => b.MachineName == device.MachineName).Select(b => b.MachineID).DefaultIfEmpty().First();
                device.PlantID = db.Machines.Where(b => b.MachineName == device.MachineName).Select(b => b.PlantID).DefaultIfEmpty().First();
                device.DepartmentID = db.Machines.Where(b => b.MachineName == device.MachineName).Select(b => b.DepartmentID).DefaultIfEmpty().First();
                device.LocationID = db.Machines.Where(b => b.MachineName == device.MachineName).Select(b => b.LocationID).DefaultIfEmpty().First();
                device.PhaseID = db.Machines.Where(b => b.MachineName == device.MachineName).Select(b => b.PhaseID).DefaultIfEmpty().First();
                device.PhaseName = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                device.BrandName = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                device.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();

                RecordSpare recordspare = new RecordSpare();
                recordspare.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                recordspare.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                recordspare.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                recordspare.DateRequest = device.DateUpdate;
                recordspare.Cause = device.CauseRequistion;
                recordspare.Model = db.Models.Where(b => b.ModelID == device.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                recordspare.SerialNumber = db.Devices.Where(b => b.DeviceID == device.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                recordspare.Plant = db.Plants.Where(b => b.PlantID == device.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                recordspare.Department = db.Departments.Where(b => b.DepartmentID == device.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                recordspare.Location = db.Locations.Where(b => b.LocationID == device.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordspare.Phase = db.Phases.Where(b => b.PhaseID == device.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                recordspare.LocationStock = db.LocationStocks.Where(b => b.LocationName == device.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                recordspare.Machine = db.Machines.Where(b => b.MachineID == device.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                recordspare.Status = db.Status.Where(b => b.StatusID == device.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                if(device.Description == "5k")
                {
                    recordspare.IsFixAsset = "Asset";
                }
                
                db.Entry(device).State = EntityState.Modified;
                db.RecordSpares.Add(recordspare);
                db.SaveChanges();
                return RedirectToAction("LastSet", "Device", new { id = device.DeviceID, uri=device.Uri });
            }

            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(d => d.BrandName), "BrandID", "BrandName", device.BrandID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", device.DepartmentID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d => d.Type), "DeviceTypeID", "Type", device.DeviceTypeID);
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationStockID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", device.PhaseID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", device.LocationID);
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
        public ActionResult setNewLocationStock(string[] ids, int? LocationStockID, string CauseRequistion, string sid, string lid, string tid, string mid )
        {
            if (string.IsNullOrEmpty(LocationStockID.ToString()))
            {
                ModelState.AddModelError("LocationStockID", "LocationStockID is Required");
            }

            if (string.IsNullOrEmpty(CauseRequistion))
            {
                ModelState.AddModelError("CauseRequistion", "Cause is Required");
            }

            var LocationStockName = db.LocationStocks.Where(b => b.LocationID == LocationStockID).Select(b => b.LocationName).DefaultIfEmpty().First();

            if (ModelState.IsValid && !string.IsNullOrEmpty(LocationStockID.ToString()))
            {
                int[] id = null;
                if (ids != null)
                {
                    id = new int[ids.Length];
                    int j = 0;
                    foreach (string i in ids)
                    {
                        int.TryParse(i, out id[j++]);
                    }
                }

                if (id != null && id.Length > 0)
                {
                    {
                        var allSelected = db.Devices.Where(a => id.Contains(a.DeviceID)).ToList();

                        foreach (var i in allSelected)
                        {
                            var c = db.Devices.Where(b => b.DeviceID.Equals(i.DeviceID)).FirstOrDefault();
                            {
                                {
                                    c.LocationStockName = LocationStockName;
                                    c.LocationStockID = LocationStockID;
                                    c.Reason = CauseRequistion;
                                    c.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                                    c.DateUpdate = DateTime.Now;
                                    RecordDevice recorddevice = new RecordDevice();
                                    recorddevice.Brand = c.BrandName;
                                    recorddevice.Type = c.Type;
                                    recorddevice.EditBy = System.Web.HttpContext.Current.User.Identity.Name;
                                    recorddevice.EditDate = DateTime.Now;
                                    recorddevice.Cause = CauseRequistion;
                                    recorddevice.Description = "Change LocationStock";
                                    recorddevice.Specification = c.Specification;
                                    recorddevice.Model = c.ModelName;
                                    recorddevice.SerialNumber = c.SerialNumber;
                                    recorddevice.Plant = db.Plants.Where(t => t.PlantID == c.PlantID).Select(t => t.PlantName).DefaultIfEmpty().First();
                                    recorddevice.Department = db.Departments.Where(t => t.DepartmentID == c.DepartmentID).Select(t => t.DepartmentName).DefaultIfEmpty().First();
                                    recorddevice.Location = db.Locations.Where(t => t.LocationID == c.LocationID).Select(t => t.LocationName).DefaultIfEmpty().First();
                                    recorddevice.Phase = c.PhaseName;
                                    recorddevice.LocationStock = LocationStockName;
                                    recorddevice.Machine = db.Machines.Where(t => t.MachineID == c.MachineID).Select(t => t.MachineName).DefaultIfEmpty().First();
                                    recorddevice.Status = db.Status.Where(t => t.StatusID == c.StatusID).Select(t => t.Status1).DefaultIfEmpty().First();
                                    db.Entry(c).State = EntityState.Modified;
                                    db.RecordDevices.Add(recorddevice);
                                }
                            }
                        }
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("filterByItem", "PrintLog", new { sid = sid, lid = lid, tid = tid, mid = mid });
            }
            ViewBag.Required = "Active";
            return RedirectToAction("filterByItem", "PrintLog", new { sid = sid, lid = lid, tid = tid, mid = mid, req=mid });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WaitSentRepairSelected(string[] ids, int? BillReceiptID)
        {
            if (string.IsNullOrEmpty(BillReceiptID.ToString()))
            {
                ModelState.AddModelError("BillReceiptID", "BillReceiptID is Required");
            }

            var billReceiptNo = db.BillReceipts.Where(b => b.BillReceiptID == BillReceiptID).Select(b => b.BillReceiptNo).DefaultIfEmpty().First();
            string billType = billReceiptNo.Substring(0, 1);

            if (ModelState.IsValid && !string.IsNullOrEmpty(BillReceiptID.ToString()))
            {
                int[] id = null;
                if (ids != null)
                {
                    id = new int[ids.Length];
                    int j = 0;
                    foreach (string i in ids)
                    {
                        int.TryParse(i, out id[j++]);
                    }
                }

                if (id != null && id.Length > 0)
                {
                    {
                        var allSelected = db.Devices.Where(a => id.Contains(a.DeviceID)).ToList();
                        var repairitem = db.RecordInRepairs.OrderByDescending(i=>i.DateRequest).ToList();

                        foreach (var i in allSelected)
                        {
                            var c = db.Devices.Where(b => b.DeviceID.Equals(i.DeviceID)).FirstOrDefault();
                            {
                                if(billType == "D")
                                {
                                    c.StatusID = 12;
                                    c.StatusName = "Destroyed";
                                    c.DateUpdate = DateTime.Now;
                                    BillReceiptList billreceipt = new BillReceiptList();
                                    billreceipt.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
                                    billreceipt.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                                    billreceipt.DateCreate = DateTime.Now;
                                    billreceipt.DateUpdate = DateTime.Now;
                                    billreceipt.DeviceID = c.DeviceID;
                                    billreceipt.SerialNumber = c.SerialNumber;
                                    billreceipt.Type = db.DeviceTypes.Where(t => t.DeviceTypeID == c.DeviceTypeID).Select(t => t.Type).DefaultIfEmpty().First();
                                    billreceipt.Model = db.Models.Where(m => m.ModelID == c.ModelID).Select(t => t.ModelName).DefaultIfEmpty().First();
                                    billreceipt.Brand = db.Brands.Where(t => t.BrandID == c.BrandID).Select(t => t.BrandName).DefaultIfEmpty().First();
                                    billreceipt.Plant = db.Plants.Where(t => t.PlantID == c.PlantID).Select(t => t.PlantName).DefaultIfEmpty().First();
                                    billreceipt.Department = db.Departments.Where(t => t.DepartmentID == c.DepartmentID).Select(t => t.DepartmentName).DefaultIfEmpty().First();
                                    billreceipt.Location = db.Locations.Where(t => t.LocationID == c.LocationID).Select(t => t.LocationName).DefaultIfEmpty().First();
                                    billreceipt.Phase = db.Phases.Where(t => t.PhaseID == c.PhaseID).Select(t => t.PhaseName).DefaultIfEmpty().First();
                                    billreceipt.MachineName = db.Machines.Where(t => t.MachineID == c.MachineID).Select(t => t.MachineName).DefaultIfEmpty().First();
                                    billreceipt.UserName = db.Users.Where(t => t.UserID == c.UserID).Select(t => t.FullName).DefaultIfEmpty().First();
                                    billreceipt.InRepairDate = repairitem.Where(r => r.SerialNumber == c.SerialNumber && r.Status == "In Repair").Select(r => r.DateRequest).DefaultIfEmpty().First();
                                    billreceipt.BillReceiptType = db.BillReceipts.Where(b => b.BillReceiptID == BillReceiptID).Select(b => b.Type).DefaultIfEmpty().First();
                                    billreceipt.CompanyName = db.BillReceipts.Where(b => b.BillReceiptID == BillReceiptID).Select(b => b.CompanyName).DefaultIfEmpty().First();
                                    billreceipt.Cause = "Destroyed";
                                    billreceipt.BillReceiptNo = db.BillReceipts.Where(b => b.BillReceiptID == BillReceiptID).Select(b => b.BillReceiptNo).DefaultIfEmpty().First();
                                    db.Entry(c).State = EntityState.Modified;
                                    db.BillReceiptLists.Add(billreceipt);
                                }
                                else if (billType == "R")
                                {
                                    c.StatusID = 8;
                                    c.StatusName = "Wait Sent Repair";
                                    c.DateUpdate = DateTime.Now;
                                    BillReceiptList billreceiptlist = new BillReceiptList();
                                    billreceiptlist.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
                                    billreceiptlist.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                                    billreceiptlist.DateCreate = DateTime.Now;
                                    billreceiptlist.DateUpdate = DateTime.Now;
                                    billreceiptlist.DeviceID = c.DeviceID;
                                    billreceiptlist.SerialNumber = c.SerialNumber;
                                    billreceiptlist.Type = db.DeviceTypes.Where(t => t.DeviceTypeID == c.DeviceTypeID).Select(t => t.Type).DefaultIfEmpty().First();
                                    billreceiptlist.Model = db.Models.Where(m => m.ModelID == c.ModelID).Select(t => t.ModelName).DefaultIfEmpty().First();
                                    billreceiptlist.Brand = db.Brands.Where(t => t.BrandID == c.BrandID).Select(t => t.BrandName).DefaultIfEmpty().First();
                                    billreceiptlist.Plant = db.Plants.Where(t => t.PlantID == c.PlantID).Select(t => t.PlantName).DefaultIfEmpty().First();
                                    billreceiptlist.Department = db.Departments.Where(t => t.DepartmentID == c.DepartmentID).Select(t => t.DepartmentName).DefaultIfEmpty().First();
                                    billreceiptlist.Location = db.Locations.Where(t => t.LocationID == c.LocationID).Select(t => t.LocationName).DefaultIfEmpty().First();
                                    billreceiptlist.Phase = db.Phases.Where(t => t.PhaseID == c.PhaseID).Select(t => t.PhaseName).DefaultIfEmpty().First();
                                    billreceiptlist.MachineName = db.Machines.Where(t => t.MachineID == c.MachineID).Select(t => t.MachineName).DefaultIfEmpty().First();
                                    billreceiptlist.UserName = db.Users.Where(t => t.UserID == c.UserID).Select(t => t.FullName).DefaultIfEmpty().First();
                                    //billreceiptlist.InRepairDate = db.RecordInRepairs.Where(r => r.SerialNumber == c.SerialNumber).OrderByDescending(r=>r.DateRequest).Select(r => r.DateRequest).DefaultIfEmpty().First();
                                    billreceiptlist.InRepairDate = repairitem.Where(r => r.SerialNumber == c.SerialNumber && r.Status == "In Repair").Select(r => r.DateRequest).DefaultIfEmpty().First();
                                    billreceiptlist.BillReceiptType = db.BillReceipts.Where(b => b.BillReceiptID == BillReceiptID).Select(b => b.Type).DefaultIfEmpty().First();
                                    billreceiptlist.CompanyName = db.BillReceipts.Where(b => b.BillReceiptID == BillReceiptID).Select(b => b.CompanyName).DefaultIfEmpty().First();
                                    //billreceiptlist.Cause = db.RecordInRepairs.Where(r => r.SerialNumber == c.SerialNumber).Select(r => r.Cause).DefaultIfEmpty().First();
                                    billreceiptlist.Cause = repairitem.Where(r => r.SerialNumber == c.SerialNumber && r.Status == "In Repair").Select(r => r.Cause).DefaultIfEmpty().First();
                                    billreceiptlist.BillReceiptNo = db.BillReceipts.Where(b => b.BillReceiptID == BillReceiptID).Select(b => b.BillReceiptNo).DefaultIfEmpty().First();
                                    db.Entry(c).State = EntityState.Modified;
                                    db.BillReceiptLists.Add(billreceiptlist);
                                }
                            }
                        }
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("inRepaired");
            }
            return RedirectToAction("inRepaired");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WaitSentSaleSelected(string[] ids, int? BillReceiptID)
        {
            if (string.IsNullOrEmpty(BillReceiptID.ToString()))
            {
                ModelState.AddModelError("BillReceiptID", "BillReceiptID is Required");
            }

            var billReceiptNo = db.BillReceipts.Where(b => b.BillReceiptID == BillReceiptID).Select(b => b.BillReceiptNo).DefaultIfEmpty().First();
            string billType = billReceiptNo.Substring(0, 1);

            if (ModelState.IsValid && !string.IsNullOrEmpty(BillReceiptID.ToString()))
            {
                int[] id = null;
                if (ids != null)
                {
                    id = new int[ids.Length];
                    int j = 0;
                    foreach (string i in ids)
                    {
                        int.TryParse(i, out id[j++]);
                    }
                }

                if (id != null && id.Length > 0)
                {
                    {
                        var allSelected = db.Devices.Where(a => id.Contains(a.DeviceID)).ToList();

                        foreach (var i in allSelected)
                        {
                            var c = db.Devices.Where(b => b.DeviceID.Equals(i.DeviceID)).FirstOrDefault();
                            {
                                if (billType == "D")
                                {
                                    c.StatusID = 12;
                                    c.StatusName = "Destroyed";
                                    c.DateUpdate = DateTime.Now;
                                    BillReceiptList billreceiptlist = new BillReceiptList();
                                    billreceiptlist.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
                                    billreceiptlist.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                                    billreceiptlist.DateCreate = DateTime.Now;
                                    billreceiptlist.DateUpdate = DateTime.Now;
                                    billreceiptlist.DeviceID = c.DeviceID;
                                    billreceiptlist.SerialNumber = c.SerialNumber;
                                    billreceiptlist.FixAccess = c.FixAccess;
                                    billreceiptlist.Type = db.DeviceTypes.Where(t => t.DeviceTypeID == c.DeviceTypeID).Select(t => t.Type).DefaultIfEmpty().First();
                                    billreceiptlist.Model = db.Models.Where(m => m.ModelID == c.ModelID).Select(t => t.ModelName).DefaultIfEmpty().First();
                                    billreceiptlist.Brand = db.Brands.Where(t => t.BrandID == c.BrandID).Select(t => t.BrandName).DefaultIfEmpty().First();
                                    billreceiptlist.Plant = db.Plants.Where(t => t.PlantID == c.PlantID).Select(t => t.PlantName).DefaultIfEmpty().First();
                                    billreceiptlist.Department = db.Departments.Where(t => t.DepartmentID == c.DepartmentID).Select(t => t.DepartmentName).DefaultIfEmpty().First();
                                    billreceiptlist.Location = db.Locations.Where(t => t.LocationID == c.LocationID).Select(t => t.LocationName).DefaultIfEmpty().First();
                                    billreceiptlist.Phase = db.Phases.Where(t => t.PhaseID == c.PhaseID).Select(t => t.PhaseName).DefaultIfEmpty().First();
                                    billreceiptlist.MachineName = db.Machines.Where(t => t.MachineID == c.MachineID).Select(t => t.MachineName).DefaultIfEmpty().First();
                                    billreceiptlist.UserName = db.Users.Where(t => t.UserID == c.UserID).Select(t => t.FullName).DefaultIfEmpty().First();
                                    billreceiptlist.InRepairDate = db.RecordInRepairs.Where(r => r.SerialNumber == c.SerialNumber).Select(r => r.DateRequest).DefaultIfEmpty().First();
                                    billreceiptlist.BillReceiptType = db.BillReceipts.Where(b => b.BillReceiptID == BillReceiptID).Select(b => b.Type).DefaultIfEmpty().First();
                                    billreceiptlist.CompanyName = db.BillReceipts.Where(b => b.BillReceiptID == BillReceiptID).Select(b => b.CompanyName).DefaultIfEmpty().First();
                                    billreceiptlist.Cause = "Destroyed";
                                    billreceiptlist.BillReceiptNo = db.BillReceipts.Where(b => b.BillReceiptID == BillReceiptID).Select(b => b.BillReceiptNo).DefaultIfEmpty().First();
                                    billreceiptlist.Unit = 1;
                                    db.Entry(c).State = EntityState.Modified;
                                    db.BillReceiptLists.Add(billreceiptlist);
                                }
                                else if (billType == "S")
                                {
                                    c.StatusID = 9;
                                    c.StatusName = "Wait Sent Sale";
                                    c.DateUpdate = DateTime.Now;
                                    BillReceiptList billreceiptlist = new BillReceiptList();
                                    billreceiptlist.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
                                    billreceiptlist.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                                    billreceiptlist.DateCreate = DateTime.Now;
                                    billreceiptlist.DateUpdate = DateTime.Now;
                                    billreceiptlist.DeviceID = c.DeviceID;
                                    billreceiptlist.SerialNumber = c.SerialNumber;
                                    billreceiptlist.FixAccess = c.FixAccess;
                                    billreceiptlist.Type = db.DeviceTypes.Where(t => t.DeviceTypeID == c.DeviceTypeID).Select(t => t.Type).DefaultIfEmpty().First();
                                    billreceiptlist.Model = db.Models.Where(m => m.ModelID == c.ModelID).Select(t => t.ModelName).DefaultIfEmpty().First();
                                    billreceiptlist.Brand = db.Brands.Where(t => t.BrandID == c.BrandID).Select(t => t.BrandName).DefaultIfEmpty().First();
                                    billreceiptlist.Plant = db.Plants.Where(t => t.PlantID == c.PlantID).Select(t => t.PlantName).DefaultIfEmpty().First();
                                    billreceiptlist.Department = db.Departments.Where(t => t.DepartmentID == c.DepartmentID).Select(t => t.DepartmentName).DefaultIfEmpty().First();
                                    billreceiptlist.Location = db.Locations.Where(t => t.LocationID == c.LocationID).Select(t => t.LocationName).DefaultIfEmpty().First();
                                    billreceiptlist.Phase = db.Phases.Where(t => t.PhaseID == c.PhaseID).Select(t => t.PhaseName).DefaultIfEmpty().First();
                                    billreceiptlist.MachineName = db.Machines.Where(t => t.MachineID == c.MachineID).Select(t => t.MachineName).DefaultIfEmpty().First();
                                    billreceiptlist.UserName = db.Users.Where(t => t.UserID == c.UserID).Select(t => t.FullName).DefaultIfEmpty().First();
                                    billreceiptlist.InRepairDate = db.RecordInRepairs.Where(r => r.SerialNumber == c.SerialNumber).Select(r => r.DateRequest).DefaultIfEmpty().First();
                                    billreceiptlist.BillReceiptType = db.BillReceipts.Where(b => b.BillReceiptID == BillReceiptID).Select(b => b.Type).DefaultIfEmpty().First();
                                    billreceiptlist.CompanyName = db.BillReceipts.Where(b => b.BillReceiptID == BillReceiptID).Select(b => b.CompanyName).DefaultIfEmpty().First();
                                    billreceiptlist.Cause = db.RecordInRepairs.Where(r => r.SerialNumber == c.SerialNumber).Select(r => r.Cause).DefaultIfEmpty().First();
                                    billreceiptlist.BillReceiptNo = db.BillReceipts.Where(b => b.BillReceiptID == BillReceiptID).Select(b => b.BillReceiptNo).DefaultIfEmpty().First();
                                    billreceiptlist.Unit = 1;
                                    db.Entry(c).State = EntityState.Modified;
                                    db.BillReceiptLists.Add(billreceiptlist);
                                }
                            }
                        }
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("inSale");
            }
            return RedirectToAction("inSale");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult setRequisitionSpareSelected(string[] ids, string MachineName, int pid, int did, int mid)
        {
            if (string.IsNullOrEmpty(MachineName))
            {
                ModelState.AddModelError("BillReceiptID", "BillReceiptID is Required");
            }
            int MachineID = db.Machines.Where(m => m.MachineName == MachineName).Select(m => m.MachineID).DefaultIfEmpty().First();

            if (ModelState.IsValid && !string.IsNullOrEmpty(MachineName))
            {
                int[] id = null;
                if (ids != null)
                {
                    id = new int[ids.Length];
                    int j = 0;
                    foreach (string i in ids)
                    {
                        int.TryParse(i, out id[j++]);
                    }
                }

                if (id != null && id.Length > 0)
                {
                    {
                        var allSelected = db.Devices.Where(a => id.Contains(a.DeviceID)).ToList();

                        foreach (var i in allSelected)
                        {
                            var c = db.Devices.Where(b => b.DeviceID.Equals(i.DeviceID)).FirstOrDefault();
                            {
                                c.StatusID = 1;
                                c.StatusName = "Use";
                                c.MachineID = MachineID;
                                c.PlantID = db.Machines.Where(m => m.MachineID == MachineID).Select(m => m.PlantID).DefaultIfEmpty().First();
                                c.DepartmentID = db.Machines.Where(m => m.MachineID == MachineID).Select(m => m.DepartmentID).DefaultIfEmpty().First();
                                c.LocationID = db.Machines.Where(m => m.MachineID == MachineID).Select(m => m.LocationID).DefaultIfEmpty().First();
                                c.PhaseID = db.Machines.Where(m => m.MachineID == MachineID).Select(m => m.PhaseID).DefaultIfEmpty().First();
                                c.PhaseName = db.Phases.Where(m => m.PhaseID == c.PhaseID).Select(m => m.PhaseName).DefaultIfEmpty().First();
                                c.DateUpdate = DateTime.Now;
                                c.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;

                                RecordRequisition recordRequisition = new RecordRequisition();
                                recordRequisition.Brand = c.BrandName;
                                recordRequisition.DeviceType = c.Type;
                                recordRequisition.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                                recordRequisition.DateRequisition = c.DateUpdate;
                                recordRequisition.Cause = "Requisition SparePart";
                                recordRequisition.Model = c.ModelName;
                                recordRequisition.SerialNumber = c.SerialNumber;
                                recordRequisition.Plant = c.Plant.PlantName;
                                recordRequisition.Department = c.Department.DepartmentName;
                                recordRequisition.Location = c.Location.LocationName;
                                recordRequisition.Phase = c.PhaseName;
                                recordRequisition.LocationStock = c.LocationStockName;
                                recordRequisition.Machine = MachineName;
                                recordRequisition.Status = c.StatusName;
                                recordRequisition.UserName = c.UserName;
                                recordRequisition.DeviceName = c.DeviceName;
                                if (c.Description == "5k")
                                {
                                    recordRequisition.IsFixAsset = "Asset";
                                }
                                db.Entry(c).State = EntityState.Modified;
                                db.RecordRequisitions.Add(recordRequisition);
                            }
                        }
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("SparePart", new { pid = pid, did = did, mid = mid });
            }
            return RedirectToAction("SparePart", new { pid = pid, did = did, mid = mid });
        }

        // GET: /Device/Delete/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            ViewBag.PreviousUrl = System.Web.HttpContext.Current.Request.UrlReferrer;
            return View(device);
        }

        [HttpPost]
        public JsonResult FindModelName(string prefixText)
       {
            var modelname = from x in db.Models
                            where x.ModelName.Contains(prefixText) || x.Brand.BrandName.Contains(prefixText) || x.DeviceType.Type.Contains(prefixText)
                            select new
                            {
                                value = x.ModelName+", "+x.DeviceType.Type+", "+x.Brand.BrandName+ ", "+" Spec: "+ x.Specification.Substring(0,100),
                                name = x.ModelName,
                                id = x.ModelID
                            };
            var result = Json(modelname.Take(10).ToList());
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName");
            return result;
        }

        [HttpPost]
        public JsonResult FindModelScanner(string prefixText)
        {
            var modelname = from x in db.Models
                            where x.ModelName.Contains(prefixText) && x.DeviceTypeID == 58 || x.Brand.BrandName.Contains(prefixText) && x.DeviceTypeID == 58 || x.DeviceType.Type.Contains(prefixText) && x.DeviceTypeID == 58
                            select new
                            {
                                value = x.ModelName + ", " + x.DeviceType.Type + ", " + x.Brand.BrandName,
                                name = x.ModelName,
                                id = x.ModelID
                            };
            var result = Json(modelname.Take(10).ToList());
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName");
            return result;
        }

        public JsonResult SelectUsers(string id)
        {
            var modelname = from x in db.Models
                            where x.ModelName == id
                            select new
                            {
                                value = x.Specification,
                                name = x.Specification,
                                id = x.ModelID
                            };
            var result = Json(modelname.Take(5).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindMachineName(string prefixText)
        {
            var machinename = from x in db.Machines
                            where x.MachineName.StartsWith(prefixText)
                            select new
                            {
                                value = x.MachineName+" - "+x.Plant.PlantName+", "+x.Department.DepartmentName+", "+x.Location.LocationName+","+x.Phase.PhaseName,
                                name = x.MachineName,
                                id = x.MachineID
                            };
            var result = Json(machinename.ToList());
            ViewBag.MachineID = new SelectList(db.Machines.OrderBy(d => d.MachineName), "MachineID", "MachineName");
            return result;
        }

        [HttpPost]
        public JsonResult FindDeviceType(string prefixText)
        {
            var devicetype = from x in db.DeviceTypes
                              where x.Type.StartsWith(prefixText)
                              select new
                              {
                                  value = x.Type ,
                                  name = x.Type,
                                  id = x.DeviceTypeID
                              };
            var result = Json(devicetype.ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindUserName(string prefixText)
        {
            var username = from x in db.Users
                              where x.FirstName.Contains(prefixText) || x.LastName.Contains(prefixText) || x.DeviceName.Contains(prefixText) || x.Plant.PlantName.Contains(prefixText) || x.Department.DepartmentName.Contains(prefixText) || x.Location.LocationName.Contains(prefixText)
                              select new
                              {
                                  value = x.DeviceName+" "+x.FirstName + " " + x.LastName + " " + x.Plant.PlantName + " " + x.Department.DepartmentName + " " + x.Location.LocationName,
                                  name = x.FirstName + " " + x.LastName,
                                  id = x.UserID
                              };
            var result = Json(username.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindUserDetails(string id)
        {
            UsersViewModels model = new UsersViewModels();
            model.ComputerName = db.Users.Where(u => u.FullName == id).Select(u => u.DeviceName).DefaultIfEmpty().First();
            model.IPAddress = db.Users.Where(u => u.FullName == id).Select(u => u.IPAddress).DefaultIfEmpty().First();
            return Json(model);
        }

        [HttpPost]
        public JsonResult BillReceiptFind(string prefixText)
        {
            var modelname = from x in db.BillReceipts
                            where (x.BillReceiptNo.StartsWith(prefixText) || x.Type.StartsWith(prefixText)) && (x.IsPrint != 1)
                            select new
                            {
                                value = x.Type + " " + x.BillReceiptNo + " " + x.CompanyName,
                                name = x.BillReceiptNo,
                                id = x.BillReceiptID
                            };
            var result = Json(modelname.Take(5).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult RequistionFind(string prefixText)
        {
            var modelname = from x in db.Devices
                            where (x.SerialNumber.StartsWith(prefixText) || x.DeviceType.Type.StartsWith(prefixText) || x.Brand.BrandName.StartsWith(prefixText) || x.Model.ModelName.StartsWith(prefixText)) && (x.StatusID == 3)
                            select new
                            {
                                value = x.DeviceType.Type+" "+x.Model.ModelName+" "+x.Brand.BrandName+" "+x.SerialNumber,
                                name = x.SerialNumber,
                                id = x.DeviceID
                            };
            var result = Json(modelname.Take(10).ToList());
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName");
            return result;
        }

        [HttpPost]
        public JsonResult RepairFind(string prefixText)
        {
            var modelname = from x in db.Devices
                            where x.SerialNumber.StartsWith(prefixText) || x.DeviceType.Type.StartsWith(prefixText) || x.Brand.BrandName.StartsWith(prefixText)
                            select new
                            {
                                value = x.DeviceType.Type + " " + x.Brand.BrandName + " " + x.SerialNumber,
                                name = x.SerialNumber,
                                id = x.DeviceID
                            };
            var result = Json(modelname.Take(10).ToList());
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName");
            return result;
        }

        [HttpPost]
        public JsonResult SellFind(string prefixText)
        {
            var modelname = from x in db.Devices
                            where x.SerialNumber.StartsWith(prefixText) || x.DeviceType.Type.StartsWith(prefixText) || x.Brand.BrandName.StartsWith(prefixText)
                            select new
                            {
                                value = x.DeviceType.Type + " " + x.Brand.BrandName + " " + x.SerialNumber,
                                name = x.SerialNumber,
                                id = x.DeviceID
                            };
            var result = Json(modelname.Take(10).ToList());
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName");
            return result;
        }

        [HttpPost]
        public JsonResult FindDevice(string prefixText)
        {
            var modelname = (from x in db.Devices
                             where x.SerialNumber.Contains(prefixText)
                             select new
                             {
                                 value = x.SerialNumber,
                                 name = x.SerialNumber,
                                 id = x.DeviceID
                             }).Union
                            (from x in db.Machines
                             where x.MachineName.Contains(prefixText)
                             select new
                             {
                                 value = x.MachineName,
                                 name = x.MachineName,
                                 id = x.MachineID
                             }).Union
                            (from x in db.Users
                             where x.FullName.Contains(prefixText)
                             select new
                             {
                                 value = x.FullName,
                                 name = x.FullName,
                                 id = x.UserID
                             }).Union
                            (from x in db.Brands
                             where x.BrandName.Contains(prefixText)
                             select new
                             {
                                 value = x.BrandName,
                                 name = x.BrandName,
                                 id = x.BrandID
                             }).Union
                            (from x in db.DeviceTypes
                             where x.Type.Contains(prefixText)
                             select new
                             {
                                 value = x.Type,
                                 name = x.Type,
                                 id = x.DeviceTypeID
                             }).Union
                            (from x in db.Models
                             where x.ModelName.Contains(prefixText)
                             select new
                             {
                                 value = x.ModelName,
                                 name = x.ModelName,
                                 id = x.ModelID
                             }).Union
                            (from x in db.Users
                             where x.DeviceName.Contains(prefixText)
                             select new
                             {
                                 value = x.DeviceName,
                                 name = x.DeviceName,
                                 id = x.UserID
                             }).Union
                             (from x in db.Devices
                              where x.DeviceName.Contains(prefixText)
                              select new
                              {
                                  value = x.DeviceName,
                                  name = x.DeviceName,
                                  id = x.DeviceID
                              });
            var result = Json(modelname.ToList());
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName");
            return result;
        }

        [HttpPost]
        public JsonResult findMachine(string prefixText)
        {
            var modelname =
                            (from x in db.Machines
                             where x.MachineName.Contains(prefixText) || x.Description.Contains(prefixText)
                             select new
                             {
                                 value = x.MachineName + " " + x.Description,
                                 name = x.MachineName,
                                 id = x.MachineID
                             }).Union
                            (from x in db.Users
                             where x.FullName.Contains(prefixText) || x.DeviceName.Contains(prefixText)
                             select new
                             {
                                 value = x.FullName + " " + x.DeviceName,
                                 name = x.FullName,
                                 id = x.UserID
                             });
            var result = Json(modelname.ToList());
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName");
            return result;
        }

        [HttpPost]
        public JsonResult FindMachineDetails(string id)
        {
            UsersViewModels model = new UsersViewModels();
            model.ComputerName = db.Machines.Where(u => u.MachineName == id).Select(u => u.MachineName).DefaultIfEmpty().First();
            model.IPAddress = db.Machines.Where(u => u.MachineName == id).Select(u => u.IPAddress).DefaultIfEmpty().First();
            return Json(model);
        }

        [HttpPost]
        public JsonResult FindScanner(string prefixText)
        {
            var modelname = (from x in db.Devices
                             where x.SerialNumber.Contains(prefixText) && x.DeviceType.DeviceTypeID == 58
                             select new
                             {
                                 value = x.SerialNumber + ", " + x.Brand.BrandName + ", " + x.ModelName,
                                 name = x.SerialNumber,
                                 id = x.DeviceID
                             });
            var result = Json(modelname.ToList());
            return result;
        }

        // POST: /Device/Delete/5
        [AuthLog(Roles = "SuperUser")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string uri)
        {
            Device device = db.Devices.Find(id);

            RecordDevice recorddevice = new RecordDevice();
            recorddevice.Brand = db.Brands.Where(b => b.BrandID == device.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
            recorddevice.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == device.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
            recorddevice.EditBy = System.Web.HttpContext.Current.User.Identity.Name;
            recorddevice.EditDate = DateTime.Now;
            recorddevice.Description = "Delete Device";
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

            db.Devices.Remove(device);
            db.RecordDevices.Add(recorddevice);
            db.SaveChanges();
            //return RedirectToAction("Index");
            return Redirect(uri);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
