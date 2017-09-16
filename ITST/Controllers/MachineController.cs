using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ITST.Models;
using ITST.CustomFilters;
using ITST.ViewModels;

namespace ITST.Controllers
{
    public class MachineController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        // GET: /Machine/
        [Authorize]
        public ActionResult Index()
        {
            //var machines = db.Machines.Include(m => m.Department).OrderBy(m=>m.MachineName).Include(m => m.Location).Include(m => m.Phase).Include(m => m.Plant);
            //return View(machines.ToList());
            return View();
        }

        public JsonResult getMachineList()
        {
            var users = (from d in db.Machines
                         join i in db.Departments on d.DepartmentID equals i.DepartmentID
                         into tempPets
                         from i in tempPets.DefaultIfEmpty()

                         join p in db.Plants on d.PlantID equals p.PlantID
                         into tempPets2
                         from p in tempPets2.DefaultIfEmpty()

                         join l in db.Locations on d.LocationID equals l.LocationID
                         into tempPets3
                         from l in tempPets3.DefaultIfEmpty()

                         join u in db.Phases on d.PhaseID equals u.PhaseID
                         into tempPets5
                         from u in tempPets5.DefaultIfEmpty()

                         select new MachineViewModel
                         {
                             MachineID = d.MachineID,
                             MachineName = d.MachineName,
                             IPAddress = d.IPAddress,
                             MACAddress = d.MACAddress,
                             PLCAddress = d.PLCAddress,
                             Plant = p.PlantName,
                             Department = i.DepartmentName,
                             Location = l.LocationName,
                             Phase = u.PhaseName,
                             CreateBy = d.CreateBy,
                             UpdateBy = d.UpdateBy,
                             DateCreate = d.DateCreate,
                             DateUpdate = d.DateUpdate,
                             Description = d.Description
                         }).OrderBy(u => u.MachineName).ToList();
            return Json(new { data = users.OrderBy(u => u.MachineName) }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult CreateSheet(int? id)
        {
            #region SetDevice
            PMSheet pmsheet = new PMSheet();

            var totalhdd = db.Devices.Where(d => d.MachineID == id && d.DeviceType.DeviceTypeID == 13 && d.Status.StatusID == 1).Count();
            var createName = System.Web.HttpContext.Current.User.Identity.Name;
            var updateName = System.Web.HttpContext.Current.User.Identity.Name;

            pmsheet.PCBoardBrand = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 50 && d.StatusID == 1).Select(d => d.Brand.BrandName).DefaultIfEmpty().First();
            pmsheet.PCBoardSerial = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 50 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty().First();
            pmsheet.PCBoardModel = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 50 && d.StatusID == 1).Select(d => d.Model.ModelName).DefaultIfEmpty().First();

            if (pmsheet.PCBoardBrand == null)
            {
                pmsheet.PCBoardBrand = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 54 && d.StatusID == 1).Select(d => d.Brand.BrandName).DefaultIfEmpty().First();
                pmsheet.PCBoardSerial = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 54 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty().First();
                pmsheet.PCBoardModel = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 54 && d.StatusID == 1).Select(d => d.Model.ModelName).DefaultIfEmpty().First();
            }

            pmsheet.MemoryBrand = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 59 && d.StatusID == 1).Select(d => d.Brand.BrandName).DefaultIfEmpty("").First();
            pmsheet.MemorySerial = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 59 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();
            pmsheet.MemoryModel = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 59 && d.StatusID == 1).Select(d => d.Model.ModelName).DefaultIfEmpty("").First();

            pmsheet.LANCardBrand = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 40 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            pmsheet.LANCardModel = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 40 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            pmsheet.LANCardSerial = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 40 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            pmsheet.VideoCardBrand = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 65 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            pmsheet.VideoCardModel = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 65 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            pmsheet.VideoCardSerial = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 65 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            pmsheet.MouseBrand = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 44 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            pmsheet.MouseModel = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 44 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            pmsheet.MouseSerial = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 44 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            pmsheet.KeyboardBrand = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 18 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            pmsheet.KeyboardModel = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 18 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            pmsheet.KeyboardSerial = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 18 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            pmsheet.DVDBrand = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 32 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            pmsheet.DVDModel = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 32 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            pmsheet.DVDSerial = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 32 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            pmsheet.HDD1Brand = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 13 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            pmsheet.HDD1Model = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 13 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            pmsheet.HDD1Serial = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 13 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            if(totalhdd >= 2)
            {
                pmsheet.HDD2Brand = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 13 && d.StatusID == 1).OrderBy(d => d.DeviceID).Skip(totalhdd - 1).Select(d => d.BrandName).DefaultIfEmpty().First();
                pmsheet.HDD2Model = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 13 && d.StatusID == 1).OrderBy(d => d.DeviceID).Skip(totalhdd - 1).Select(d => d.ModelName).DefaultIfEmpty().First();
                pmsheet.HDD2Serial = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 13 && d.StatusID == 1).OrderBy(d => d.DeviceID).Skip(totalhdd - 1).Select(d => d.SerialNumber).DefaultIfEmpty().First();
            }

            pmsheet.PrinterBrand = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 56 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            pmsheet.PrinterModel = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 56 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            pmsheet.PrinterSerial = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 56 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            pmsheet.MonitorBrand = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 43 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            pmsheet.MonitorModel = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 43 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            pmsheet.MonitorSerial = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 43 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            pmsheet.ScannerBrand = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 58 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            pmsheet.ScannerModel = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 58 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            pmsheet.ScannerSerial = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 58 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            pmsheet.UPSBrand = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 64 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            pmsheet.UPSModel = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 64 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            pmsheet.UPSSerial = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 64 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            pmsheet.BluetoothBrand = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 7 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            pmsheet.BluetoothModel = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 7 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            pmsheet.BluetoothSerial = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 7 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            pmsheet.HUBBrand = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 15 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            pmsheet.HUBModel = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 15 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            pmsheet.HUBSerial = db.Devices.Where(d => d.MachineID == id && d.DeviceTypeID == 15 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            pmsheet.PlantName = db.Machines.Where(d => d.MachineID == id ).Select(d => d.Plant.PlantName).DefaultIfEmpty("").First();
            pmsheet.DepartmentName = db.Machines.Where(d => d.MachineID == id ).Select(d => d.Department.DepartmentName).DefaultIfEmpty("").First();
            pmsheet.LocationName = db.Machines.Where(d => d.MachineID == id ).Select(d => d.Location.LocationName).DefaultIfEmpty("").First();
            pmsheet.PhaseName = db.Machines.Where(d => d.MachineID == id).Select(d => d.Phase.PhaseName).DefaultIfEmpty("").First();
            pmsheet.MachineName = db.Machines.Where(d => d.MachineID == id ).Select(d => d.MachineName).DefaultIfEmpty("").First();


            pmsheet.CreateBy = db.Users.Where(u => u.EmployeeID == createName).Select(u => u.FullName).DefaultIfEmpty().First();
            pmsheet.UpdateBy = db.Users.Where(u => u.EmployeeID == updateName).Select(u => u.FullName).DefaultIfEmpty().First();
            pmsheet.DateCreate = DateTime.Now;
            pmsheet.DateUpdate = DateTime.Now;
            #endregion

            ViewBag.DeviceID = id;
            ViewBag.PlantID = db.Machines.Where(d => d.MachineID == id).Select(d => d.PlantID).DefaultIfEmpty().First();
            ViewBag.DepartmentID = db.Machines.Where(d => d.MachineID == id).Select(d => d.DepartmentID).DefaultIfEmpty().First();
            ViewBag.LocationID = db.Machines.Where(d => d.MachineID == id).Select(d => d.LocationID).DefaultIfEmpty().First();

            return View(pmsheet);
        }

        // POST: /Machine/CreateSheet
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSheet([Bind(Include = "SheetID,PCBoardBrand,PCBoardModel,PCBoardSerial,PCBoardRemark,CreateBy,UpdateBy,DateCreate,DateUpdate,FixAccess,ProcessorBrand,ProcessorModel,ProcessorSerial,ProcessorRemark,PlantID,DepartmentID,LocationID,PhaseID,MachineID,PlantName,DepartmentName,LocationName,PhaseName,MachineName,MemoryBrand,MemoryModel,MemorySerial,MemoryRemark,VideoCardBrand,VideoCardModel,VideoCardSerial,VideoCardRemark,LANCardBrand,LANCardSerial,LANCardModel,LANCardRemark,MouseBrand,MouseModel,MouseSerial,MouseRemark,MonitorBrand,MonitorModel,MonitorSerial,MonitorRemark,KeyboardBrand,KeyboardModel,KeyboardSerial,KeyboardRemark,ScannerBrand,ScannerModel,ScannerSerial,ScannerRemark,DVDBrand,DVDModel,DVDSerial,DVDRemark,HDD1Brand,HDD1Model,HDD1Serial,HDD1Remark,HDD2Brand,HDD2Model,HDD2Serial,HDD2Remark,PrinterBrand,PrinterModel,PrinterSerial,PrinterRemark,UPSBrand,UPSModel,UPSSerial,UPSRemark,PCICardBrand,PCICardModel,PCICardSerial,PCIRemark,BluetoothBrand,BluetoothModel,BluetoothSerial,BluetoothRemark,HUBBrand,HUBModel,HUBSerial,HUBRemark,OtherHardwareName1,OtherHardwareBrand,OtherHardwareModel,OtherHardwareSerial,OtherHardwareRemark,OtherHardwareName2,OtherHardware1Brand,OtherHardware1Model,OtherHardware1Serial,OtherHardware1Remark,OtherHardwareName3,OtherHardware2Brand,OtherHardware2Model,OtherHardware2Serial,OtherHardware2Remark,FixAccess,CreateBy,UpdateBy,DateCreate,DateUpdate,PlantID,DepartmentID,LocationID,PhaseID,MachineID,PlantName,DepartmentName,LocationName,PhaseName,MachineName,Zip7Version,Zip7Remark,AcrobatVersion,AcrobatRemark,CureGraphVersion,CureGraphRemark,JP1Version,JP1Remark,MSOfficeVersion,MSOfficeRemark,OracleVersion,OracleRemark,RaidVersion,RaidRemark,SeedWincsVersion,SeedWincsRemark,SharedCPCVersion,SharedCPCRemark,SharedDPanelPCVersion,SharedDPanelPCRemark,TightVNCVersion,TightVNCRemark,TrendMicroVersion,TrendMicroRemark,WindowsVersion,WindowsRemark,OtherSoftware,OtherSoftwareRemark")] PMSheet pmsheet)
        {
            if (ModelState.IsValid)
            {
                pmsheet.PlantID = db.Machines.Where(m => m.Plant.PlantName == pmsheet.PlantName).Select(m => m.PlantID).DefaultIfEmpty().First();
                pmsheet.DepartmentID = db.Machines.Where(m => m.Department.DepartmentName == pmsheet.DepartmentName).Select(m => m.DepartmentID).DefaultIfEmpty().First();
                pmsheet.LocationID = db.Machines.Where(m => m.Location.LocationName == pmsheet.LocationName).Select(m => m.LocationID).DefaultIfEmpty().First();
                pmsheet.PhaseID = db.Machines.Where(m => m.Phase.PhaseName == pmsheet.PhaseName).Select(m => m.PhaseID).DefaultIfEmpty().First();
                pmsheet.MachineID = db.Machines.Where(m => m.MachineName == pmsheet.MachineName).Select(m => m.MachineID).DefaultIfEmpty().First();

                db.PMSheets.Add(pmsheet);
                db.SaveChanges();
                return RedirectToAction("Index","MachineSheet");
            }

            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", pmsheet.DepartmentID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", pmsheet.LocationID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", pmsheet.PhaseID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", pmsheet.PlantID);
            return View(pmsheet);
        }

        [Authorize]
        public ActionResult Sheet()
        {
            var machines = from machine in db.Machines group machine by machine.PlantID into dv let m = dv.FirstOrDefault() select m;
            return View(machines.ToList());
        }

        public ActionResult MachineSheetByDepartment(int?id)
        {
            var machines = from machine in db.Machines where machine.PlantID == id group machine by machine.DepartmentID into dv let m = dv.FirstOrDefault() select m;
            return View(machines.ToList());
        }

        public ActionResult CreateMachineSheet(int? id, int? did)
        {
            var machines = from machine in db.Machines where machine.PlantID == id  && machine.DepartmentID == did group machine by machine.MachineID into dv let m = dv.FirstOrDefault() select m;
            return View(machines.ToList());
        }

        public ActionResult listCartridgePrinter()
        {
            var machines = db.Machines.Where(m => m.Description == "Cart").Include(m => m.Department).OrderBy(m => m.MachineName).Include(m => m.Location).Include(m => m.Phase).Include(m => m.Plant);
            return View(machines.ToList());
        }

        [Authorize]
        public ActionResult CreateCartridgePrinter()
        {
            return View();
        }

        // POST: /Machine/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCartridgePrinter([Bind(Include = "MachineName")] CreateCartridgePrinterViewModels viewmodel)
        {


            if (ModelState.IsValid && db.Machines.Any(d => d.MachineName == viewmodel.MachineName))
            {
                ModelState.AddModelError("MachineName", "MachineName is Duplicate");
            }

            if (ModelState.IsValid)
            {
                Machine machine = new Machine();
                machine.MachineName = viewmodel.MachineName;
                machine.Description = "Cart";
                machine.PlantID = 1;
                machine.DepartmentID = 34;
                machine.LocationID = 66;
                machine.PhaseID = 6;
                machine.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
                machine.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                machine.DateCreate = DateTime.Now;
                machine.DateUpdate = DateTime.Now;
                db.Machines.Add(machine);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(viewmodel);
        }

        // GET: /Machine/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Machine machine = db.Machines.Find(id);
            if (machine == null)
            {
                return HttpNotFound();
            }
            return View(machine);
        }

        // GET: /Machine/Create
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Create()
        {
            Machine machine = new Machine();
            machine.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            machine.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            machine.PLCAddress = " ";
            machine.MACAddress = " ";
            machine.DateCreate = DateTime.Now;
            machine.DateUpdate = DateTime.Now;

            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d=>d.DepartmentName), "DepartmentID", "DepartmentName");
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d=>d.LocationName), "LocationID", "LocationName");
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d=>d.PhaseName), "PhaseID", "PhaseName");
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d=>d.PlantName), "PlantID", "PlantName");
            return View(machine);
        }

        // POST: /Machine/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="MachineID,MachineName,Description,PlantID,DepartmentID,LocationID,DateCreate,DateUpdate,CreateBy,UpdateBy,PhaseID,IPAddress,PLCAddress,MACAddress")] Machine machine)
        {
            if (string.IsNullOrEmpty(machine.MachineName))
            {
                ModelState.AddModelError("MachineName", "MachineName is Required");
            }

            if (ModelState.IsValid && db.Machines.Any(d => d.MachineName == machine.MachineName))
            {
                ModelState.AddModelError("MachineName", "MachineName is Duplicate");
            }

            if (ModelState.IsValid && db.Machines.Any(d => d.IPAddress == machine.IPAddress && d.IPAddress != "DHCP"))
            {
                ModelState.AddModelError("IPAddress", "IPAddress is Duplicate");
            }

            if (ModelState.IsValid && db.Machines.Any(d => d.PLCAddress == machine.PLCAddress && machine.PLCAddress != " "))
            {
                ModelState.AddModelError("PLCAddress", "PLCAddress is Duplicate");
            }

            if (ModelState.IsValid && db.Machines.Any(d => d.MACAddress == machine.MACAddress && machine.MACAddress != " "))
            {
                ModelState.AddModelError("MACAddress", "MACAddress is Duplicate");
            }

            if (string.IsNullOrEmpty(machine.PlantID.ToString()))
            {
                ModelState.AddModelError("PlantID", "PlantID is Required");
            }
            if (string.IsNullOrEmpty(machine.DepartmentID.ToString()))
            {
                ModelState.AddModelError("DepartmentID", "DepartmentID is Required");
            }
            if (string.IsNullOrEmpty(machine.LocationID.ToString()))
            {
                ModelState.AddModelError("LocationID", "LocationID is Required");
            }
            if (string.IsNullOrEmpty(machine.PhaseID.ToString()))
            {
                ModelState.AddModelError("PhaseID", "PhaseID is Required");
            }

            if (ModelState.IsValid)
            {
                db.Machines.Add(machine);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName",machine.DepartmentID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName",machine.LocationID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName",machine.PhaseID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName",machine.PlantID);
            return View(machine);
        }

        // GET: /Machine/Edit/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Machine machine = db.Machines.Find(id);
            machine.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            machine.DateUpdate = DateTime.Now;
            machine.FormerMachineName = machine.MachineName;
            if (machine == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", machine.DepartmentID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", machine.LocationID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", machine.PhaseID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", machine.PlantID);
            return View(machine);
        }

        // POST: /Machine/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MachineID,MachineName,Description,PlantID,DepartmentID,LocationID,DateCreate,DateUpdate,CreateBy,UpdateBy,PhaseID,IPAddress,PLCAddress,MACAddress,FormerMachineName")] Machine machine)
        {
            if (ModelState.IsValid)
            {
                var device = db.Devices.Where(d => d.MachineID == machine.MachineID).ToList();
                var limitDevice = db.LimitDeviceQuantities.Where(l => l.Machine == machine.FormerMachineName).ToList();
                foreach (var i in device)
                {
                    i.PlantID = machine.PlantID;
                    i.DepartmentID = machine.DepartmentID;
                    i.LocationID = machine.LocationID;
                    i.PhaseID = machine.PhaseID;
                    i.PhaseName = db.Phases.Where(p => p.PhaseID == machine.PhaseID).Select(p => p.PhaseName).DefaultIfEmpty().First();
                    i.DateUpdate = DateTime.Now;
                    db.Entry(i).State = EntityState.Modified;
                }
                foreach(var j in limitDevice)
                {
                    j.Machine = machine.MachineName;
                    db.Entry(j).State = EntityState.Modified;
                }
                db.Entry(machine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", machine.DepartmentID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", machine.LocationID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", machine.PhaseID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", machine.PlantID);
            return View(machine);
        }

        // GET: /Machine/Delete/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Machine machine = db.Machines.Find(id);
            if (machine == null)
            {
                return HttpNotFound();
            }
            return View(machine);
        }

        // POST: /Machine/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Machine machine = db.Machines.Find(id);
            db.Machines.Remove(machine);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public static IEnumerable<SelectListItem> GetOtherHardwareList()
        {
            IList<SelectListItem> otherhardware = new List<SelectListItem>
            {
                new SelectListItem() {Text="", Value=""},
                new SelectListItem() { Text="Scanner(Green)", Value="Scanner(Green)"},
                new SelectListItem() { Text="Charger(Green)", Value="Charger(Green)"},
                new SelectListItem() { Text="Charger(TBM)", Value="Charger(TBM)"},
                new SelectListItem() { Text="Monitor", Value="Monitor"},
            };
            return otherhardware;
        }

        public static IEnumerable<SelectListItem> GetMSOfficeVersionList()
        {
            IList<SelectListItem> msofficeversion = new List<SelectListItem>
            {
                new SelectListItem() {Text="", Value=""},
                new SelectListItem() { Text="2003", Value="2003"},
                new SelectListItem() { Text="2007", Value="2007"},
                new SelectListItem() { Text="2010", Value="2010"},
                new SelectListItem() { Text="2013", Value="2013"},
                new SelectListItem() { Text="2016", Value="2016"},
            };
            return msofficeversion;
        }

        public static IEnumerable<SelectListItem> GetWindowsVersionList()
        {
            IList<SelectListItem> winsversion = new List<SelectListItem>
            {
                new SelectListItem() {Text="", Value=""},
                new SelectListItem() { Text="Windows XP", Value="Windows XP"},
                new SelectListItem() { Text="Windows 7", Value="Windows 7"},
                new SelectListItem() { Text="Windows 8", Value="Windows 8"},
                new SelectListItem() { Text="Windows 8.1", Value="Windows 8.1"},
                new SelectListItem() { Text="Windows 10", Value="Windows 10"},
            };
            return winsversion;
        }

        [HttpPost]
        public JsonResult FindMouseSerialNumber(string prefixText, int? dvid, int? pid, int? did, int? lid)
        {
            var brandname = from x in db.Devices
                            where x.SerialNumber.Contains(prefixText) && x.DeviceTypeID == 44 && x.PlantID == pid && x.DepartmentID == did && x.LocationID == lid && x.StatusID == 1
                            select new
                            {
                                value = x.SerialNumber,
                                name = x.SerialNumber,
                                id = x.BrandID
                            };
            var result = Json(brandname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindMonitorSerialNumber(string prefixText, int? dvid, int? pid, int? did, int? lid)
        {
            var brandname = from x in db.Devices
                            where x.SerialNumber.Contains(prefixText) && x.DeviceTypeID == 43 && x.PlantID == pid && x.DepartmentID == did && x.LocationID == lid && x.StatusID == 1
                            select new
                            {
                                value = x.SerialNumber,
                                name = x.SerialNumber,
                                id = x.BrandID
                            };
            var result = Json(brandname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindKeyboardSerialNumber(string prefixText, int? dvid, int? pid, int? did, int? lid)
        {
            var brandname = from x in db.Devices
                            where x.SerialNumber.Contains(prefixText) && x.DeviceTypeID == 18 && x.PlantID == pid && x.DepartmentID == did && x.LocationID == lid && x.StatusID == 1
                            select new
                            {
                                value = x.SerialNumber,
                                name = x.SerialNumber,
                                id = x.BrandID
                            };
            var result = Json(brandname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindPCBBrand(string prefixText)
        {
            var brandname = from x in db.Brands
                              where x.BrandName.Contains(prefixText)
                              select new
                              {
                                  value = x.BrandName,
                                  name = x.BrandName,
                                  id = x.BrandID
                              };
            var result = Json(brandname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindPrinterBrand(string prefixText)
        {
            var brandname = from x in db.Brands
                            where x.BrandName.Contains(prefixText) 
                            select new
                            {
                                value = x.BrandName,
                                name = x.BrandName,
                                id = x.BrandID
                            };
            var result = Json(brandname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindMouseBrand(string prefixText)
        {
            var brandname = from x in db.Brands
                            where x.BrandName.Contains(prefixText)
                            select new
                            {
                                value = x.BrandName,
                                name = x.BrandName,
                                id = x.BrandID
                            };
            var result = Json(brandname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindMonitorBrand(string prefixText)
        {
            var brandname = from x in db.Brands
                            where x.BrandName.Contains(prefixText)
                            select new
                            {
                                value = x.BrandName,
                                name = x.BrandName,
                                id = x.BrandID
                            };
            var result = Json(brandname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindKeyboardBrand(string prefixText)
        {
            var brandname = from x in db.Brands
                            where x.BrandName.Contains(prefixText)
                            select new
                            {
                                value = x.BrandName,
                                name = x.BrandName,
                                id = x.BrandID
                            };
            var result = Json(brandname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindScannerBrand(string prefixText)
        {
            var brandname = from x in db.Brands
                            where x.BrandName.Contains(prefixText)
                            select new
                            {
                                value = x.BrandName,
                                name = x.BrandName,
                                id = x.BrandID
                            };
            var result = Json(brandname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindPCBModel(string prefixText)
        {
            var modelname = from x in db.Models
                            where x.ModelName.Contains(prefixText)
                            select new
                            {
                                value = x.ModelName,
                                name = x.ModelName,
                                id = x.ModelID
                            };
            var result = Json(modelname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindLANCardSerialNumber(string prefixText, int?dvid, int?pid, int?did, int?lid)
        {
            var brandname = from x in db.Devices
                            where x.SerialNumber.Contains(prefixText) && x.DeviceTypeID == 40 && x.PlantID == pid && x.DepartmentID == did && x.LocationID == lid && x.StatusID == 1
                            select new
                            {
                                value = x.SerialNumber,
                                name = x.SerialNumber,
                                id = x.BrandID
                            };
            var result = Json(brandname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindOtherHWSerialNumber(string prefixText, int? dvid, int? pid, int? did, int? lid)
        {
            var brandname = from x in db.Devices
                            where x.SerialNumber.Contains(prefixText) && x.DeviceTypeID == 58 || x.DeviceTypeID == 11 && x.PlantID == pid && x.DepartmentID == did && x.LocationID == lid && x.StatusID == 1
                            select new
                            {
                                value = x.SerialNumber,
                                name = x.SerialNumber,
                                id = x.BrandID
                            };
            var result = Json(brandname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindScannerSerialNumber(string prefixText, int? dvid, int? pid, int? did, int? lid)
        {
            var brandname = from x in db.Devices
                            where x.SerialNumber.Contains(prefixText) && x.DeviceTypeID == 58 && x.PlantID == pid && x.DepartmentID == did && x.LocationID == lid && x.StatusID == 1
                            select new
                            {
                                value = x.SerialNumber,
                                name = x.SerialNumber,
                                id = x.BrandID
                            };
            var result = Json(brandname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindDVDSerialNumber(string prefixText, int? dvid, int? pid, int? did, int? lid)
        {
            var brandname = from x in db.Devices
                            where x.SerialNumber.Contains(prefixText) && x.DeviceTypeID == 32 && x.PlantID == pid && x.DepartmentID == did && x.LocationID == lid && x.StatusID == 1
                            select new
                            {
                                value = x.SerialNumber,
                                name = x.SerialNumber,
                                id = x.BrandID
                            };
            var result = Json(brandname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindVDOCardSerialNumber(string prefixText, int? dvid, int? pid, int? did, int? lid)
        {
            var brandname = from x in db.Devices
                            where x.SerialNumber.Contains(prefixText) && x.DeviceTypeID == 65 && x.PlantID == pid && x.DepartmentID == did && x.LocationID == lid && x.StatusID == 1
                            select new
                            {
                                value = x.SerialNumber,
                                name = x.SerialNumber,
                                id = x.BrandID
                            };
            var result = Json(brandname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindPCBSerialNumber(string prefixText, int? dvid, int? pid, int? did, int? lid)
        {
            var brandname = from x in db.Devices
                            where x.SerialNumber.Contains(prefixText) && x.DeviceTypeID == 50 && x.PlantID == pid && x.DepartmentID == did && x.LocationID == lid && x.StatusID == 1
                            select new
                            {
                                value = x.SerialNumber,
                                name = x.SerialNumber,
                                id = x.BrandID
                            };
            var result = Json(brandname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindHDDSerialNumber(string prefixText, int? dvid, int? pid, int? did, int? lid)
        {
            var brandname = from x in db.Devices
                            where x.SerialNumber.Contains(prefixText) && x.DeviceTypeID == 13 && x.PlantID == pid && x.DepartmentID == did && x.LocationID == lid && x.StatusID == 1
                            select new
                            {
                                value = x.SerialNumber,
                                name = x.SerialNumber,
                                id = x.BrandID
                            };
            var result = Json(brandname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindUPSSerialNumber(string prefixText, int? dvid, int? pid, int? did, int? lid)
        {
            var brandname = from x in db.Devices
                            where x.SerialNumber.Contains(prefixText) && x.DeviceTypeID == 64 && x.PlantID == pid && x.DepartmentID == did && x.LocationID == lid && x.StatusID == 1
                            select new
                            {
                                value = x.SerialNumber,
                                name = x.SerialNumber,
                                id = x.BrandID
                            };
            var result = Json(brandname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindBluetoothSerialNumber(string prefixText, int? dvid, int? pid, int? did, int? lid)
        {
            var brandname = from x in db.Devices
                            where x.SerialNumber.Contains(prefixText) && x.DeviceTypeID == 7 && x.PlantID == pid && x.DepartmentID == did && x.LocationID == lid && x.StatusID == 1
                            select new
                            {
                                value = x.SerialNumber,
                                name = x.SerialNumber,
                                id = x.BrandID
                            };
            var result = Json(brandname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindHUBSerialNumber(string prefixText, int? dvid, int? pid, int? did, int? lid)
        {
            var brandname = from x in db.Devices
                            where x.SerialNumber.Contains(prefixText) && x.DeviceTypeID == 15 && x.PlantID == pid && x.DepartmentID == did && x.LocationID == lid && x.StatusID == 1
                            select new
                            {
                                value = x.SerialNumber,
                                name = x.SerialNumber,
                                id = x.BrandID
                            };
            var result = Json(brandname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindPrinterSerialNumber(string prefixText, int? dvid, int? pid, int? did, int? lid)
        {
            var brandname = from x in db.Devices
                            where (x.SerialNumber.Contains(prefixText) && x.DeviceTypeID == 55 && x.PlantID == pid && x.DepartmentID == did && x.LocationID == lid && x.StatusID == 1) || (x.SerialNumber.Contains(prefixText) && x.DeviceTypeID == 56 && x.PlantID == pid && x.DepartmentID == did && x.LocationID == lid && x.StatusID == 1) || (x.SerialNumber.Contains(prefixText) && x.DeviceTypeID == 57 && x.PlantID == pid && x.DepartmentID == did && x.LocationID == lid && x.StatusID == 1)
                            select new
                            {
                                value = x.SerialNumber,
                                name = x.SerialNumber,
                                id = x.BrandID
                            };
            var result = Json(brandname.Take(10).ToList());
            return result;
        }

        [HttpPost]
        public JsonResult FindMemorySerialNumber(string prefixText, int? dvid, int? pid, int? did, int? lid)
        {
            var brandname = from x in db.Devices
                            where x.SerialNumber.Contains(prefixText) && x.DeviceTypeID == 59 && x.PlantID == pid && x.DepartmentID == did && x.LocationID == lid && x.StatusID == 1
                            select new
                            {
                                value = x.SerialNumber,
                                name = x.SerialNumber,
                                id = x.BrandID
                            };
            var result = Json(brandname.Take(10).ToList());
            return result;
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
