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
    public class UserController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        // GET: /User/
        [Authorize]
        public ActionResult Index()
        {
            //var users = db.Users.Include(u => u.Department).Include(u => u.Location).Include(u => u.Plant);
            //return View(users.OrderBy(u=>u.FirstName).ToList());
            return View();
        }

        public JsonResult getUserList()
        {
            var users = db.Users.Include(u => u.Department).Include(u => u.Location).Include(u => u.Plant).OrderBy(u => u.FirstName).ToList();
            return Json(new { data = users }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExtensionPhoneList()
        {
            return View();
        }

        public JsonResult getPhoneList()
        {
            {
                var users = (from d in db.Users
                             join i in db.Departments on d.DepartmentID equals i.DepartmentID
                             into tempPets
                             from i in tempPets.DefaultIfEmpty()

                             join p in db.Plants on d.PlantID equals p.PlantID
                             into tempPets2
                             from p in tempPets2.DefaultIfEmpty()

                             join l in db.Locations on d.LocationID equals l.LocationID
                             into tempPets3
                             from l in tempPets3.DefaultIfEmpty()

                             join u in db.Users on d.UserID equals u.UserID
                             into tempPets5
                             from u in tempPets5.DefaultIfEmpty()

                             where d.Phone != null

                             select new PhoneListViewModel
                             {
                                 UserID = d.UserID,
                                 FirstName = d.FirstName,
                                 LastName = d.LastName,
                                 NickName = d.NickName,
                                 Plant = p.PlantName,
                                 Department = i.DepartmentName,
                                 Location = l.LocationName,
                                 CreateBy = d.CreateBy,
                                 UpdateBy = d.UpdateBy,
                                 DateCreate = d.DateCreate,
                                 DateUpdate = d.DateUpdate,
                                 Phase = d.PhaseName,
                                 Phone = d.Phone,
                             }).OrderBy(u => u.Phone).ToList();
                return Json(new { data = users.OrderBy(u => u.Phone) }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult getAllUser()
        {
            var users = (from d in db.Users
                           join i in db.Departments on d.DepartmentID equals i.DepartmentID
                           into tempPets
                           from i in tempPets.DefaultIfEmpty()

                           join p in db.Plants on d.PlantID equals p.PlantID
                           into tempPets2
                           from p in tempPets2.DefaultIfEmpty()

                           join l in db.Locations on d.LocationID equals l.LocationID
                           into tempPets3
                           from l in tempPets3.DefaultIfEmpty()

                           join u in db.Users on d.UserID equals u.UserID
                           into tempPets5
                           from u in tempPets5.DefaultIfEmpty()

                            select new UserViewModel
                           {
                               UserID = d.UserID,
                               UserLogOn = d.UserLogOn,
                               FirstName = d.FirstName,
                               LastName = d.LastName,
                               EmployeeID = d.EmployeeID,
                               PlantName = p.PlantName,
                               DepartmentName = i.DepartmentName,
                               LocationName = l.LocationName,
                               CreateBy = d.CreateBy,
                               UpdateBy = d.UpdateBy,
                               DateCreate = d.DateCreate,
                               DateUpdate = d.DateUpdate,
                               PhaseName = d.PhaseName,
                               IPAddress = d.IPAddress,
                               FullName = u.FullName,
                               Phone = d.Phone,
                               DeviceName = d.DeviceName
                           }).OrderBy(u=>u.FirstName).ToList();
            return Json(new { data = users.OrderBy(u=>u.FirstName) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserByPlant()
        {
            var users = from user in db.Users group user by user.Plant.PlantName into dv let m = dv.FirstOrDefault() select m;
            return View(users.ToList());
        }

        public ActionResult UserByDepartment(int? id)
        {
            var users = from user in db.Users where user.PlantID == id group user by user.Department.DepartmentName into dv let m = dv.FirstOrDefault() select m;
            return View(users.ToList());
        }

        public ActionResult ListUser(int? pid, int? did)
        {
            var users = from user in db.Users where user.PlantID == pid && user.DepartmentID == did group user by user.UserID into dv let m = dv.FirstOrDefault() select m;
            return View(users.ToList());
        }

        public ActionResult setNickName(int id)
        {
            User user = db.Users.Find(id);
            ViewModelsUser vmuser = new ViewModelsUser();
            vmuser.UserID = id;
            vmuser.FirstName = user.FirstName;
            vmuser.LastName = user.LastName;
            vmuser.NickName = user.NickName;
            vmuser.Phone = user.Phone;
            return View(vmuser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult setNickName([Bind(Include = "UserID,FirstName,LastName,NickName,EmployeeID,Position,Section,Phone")] ViewModelsUser viewmodel)
        {
            if (ModelState.IsValid)
            {
                User user = db.Users.Find(viewmodel.UserID);
                user.NickName = viewmodel.NickName;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ExtensionPhoneList");
            }
            return View(viewmodel);
        }

        [Authorize]
        public ActionResult setPhoneNumber(int id)
        {
            User user = db.Users.Find(id);
            ViewModelsUser vmuser = new ViewModelsUser();
            vmuser.UserID = id;
            vmuser.FirstName = user.FirstName;
            vmuser.LastName = user.LastName;
            vmuser.NickName = user.NickName;
            vmuser.Phone = user.Phone;
            return View(vmuser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult setPhoneNumber([Bind(Include = "UserID,FirstName,LastName,NickName,EmployeeID,Position,Section,Phone")] ViewModelsUser viewmodel)
        {
            if (ModelState.IsValid)
            {
                User user = db.Users.Find(viewmodel.UserID);
                user.Phone = viewmodel.Phone;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ExtensionPhoneList");
            }
            return View(viewmodel);
        }

        // GET: /User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [Authorize]
        public ActionResult Sheet()
        {
            var users = from user in db.Users group user by user.PlantID into dv let m = dv.FirstOrDefault() select m;
            return View(users.ToList());
        }

        public ActionResult UserSheetByDepartment(int? id)
        {
            var users = from user in db.Users where user.PlantID == id group user by user.DepartmentID into dv let m = dv.FirstOrDefault() select m;
            return View(users.ToList());
        }

        public ActionResult CreateUserSheet(int? id, int? did)
        {
            var users = from user in db.Users where user.PlantID == id && user.DepartmentID == did group user by user.UserID into dv let m = dv.FirstOrDefault() select m;
            return View(users.ToList());
        }

        [Authorize]
        public ActionResult CreateSheet(int? id)
        {
            #region SetDevice
            UserSheet usersheet = new UserSheet();

            var totalmonitor = db.Devices.Where(d => d.UserID == id && d.DeviceType.DeviceTypeID == 43 && d.Status.StatusID == 1).Count();
            var totalhdd = db.Devices.Where(d => d.UserID == id && d.DeviceType.DeviceTypeID == 13 && d.Status.StatusID == 1).Count();

            usersheet.PCBoardBrand = db.Devices.Where(d => d.UserID == id && (d.DeviceTypeID == 49 || d.DeviceTypeID == 54 || d.DeviceTypeID == 66) && d.StatusID == 1).Select(d => d.Brand.BrandName).DefaultIfEmpty("").First();
            usersheet.PCBoardSerial = db.Devices.Where(d => d.UserID == id && (d.DeviceTypeID == 49 || d.DeviceTypeID == 54 || d.DeviceTypeID == 66) && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();
            usersheet.PCBoardModel = db.Devices.Where(d => d.UserID == id && (d.DeviceTypeID == 49 || d.DeviceTypeID == 54 || d.DeviceTypeID == 66) && d.StatusID == 1).Select(d => d.Model.ModelName).DefaultIfEmpty("").First();

            //if(usersheet.PCBoardBrand.Equals(null))
            //{
            //    usersheet.PCBoardBrand = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 54 && d.StatusID == 1).Select(d => d.Brand.BrandName).DefaultIfEmpty("").First();
            //    usersheet.PCBoardSerial = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 54 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();
            //    usersheet.PCBoardModel = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 54 && d.StatusID == 1).Select(d => d.Model.ModelName).DefaultIfEmpty("").First();
            //}

            usersheet.MemoryBrand = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 59 && d.StatusID == 1).Select(d => d.Brand.BrandName).DefaultIfEmpty("").First();
            usersheet.MemorySerial = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 59 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();
            usersheet.MemoryModel = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 59 && d.StatusID == 1).Select(d => d.Model.ModelName).DefaultIfEmpty("").First();

            usersheet.LANCardBrand = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 40 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            usersheet.LANCardModel = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 40 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            usersheet.LANCardSerial = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 40 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            usersheet.VideoCardBrand = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 65 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            usersheet.VideoCardModel = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 65 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            usersheet.VideoCardSerial = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 65 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            usersheet.MouseBrand = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 44 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            usersheet.MouseModel = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 44 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            usersheet.MouseSerial = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 44 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            usersheet.KeyboardBrand = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 18 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            usersheet.KeyboardModel = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 18 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            usersheet.KeyboardSerial = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 18 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            usersheet.DVDBrand = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 32 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            usersheet.DVDModel = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 32 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            usersheet.DVDSerial = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 32 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            usersheet.HDD1Brand = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 13 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            usersheet.HDD1Model = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 13 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            usersheet.HDD1Serial = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 13 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            if(totalhdd >= 2)
            {
                usersheet.HDD2Brand = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 13 && d.StatusID == 1).OrderBy(d => d.DeviceID).Skip(totalhdd - 1).Select(d => d.BrandName).DefaultIfEmpty().First();
                usersheet.HDD2Model = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 13 && d.StatusID == 1).OrderBy(d => d.DeviceID).Skip(totalhdd - 1).Select(d => d.ModelName).DefaultIfEmpty().First();
                usersheet.HDD2Serial = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 13 && d.StatusID == 1).OrderBy(d => d.DeviceID).Skip(totalhdd - 1).Select(d => d.SerialNumber).DefaultIfEmpty().First();
            }
            

            usersheet.PrinterBrand = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 56 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            usersheet.PrinterModel = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 56 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            usersheet.PrinterSerial = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 56 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            usersheet.MonitorBrand = db.Devices.OrderBy(d => d.DeviceID).Where(d => d.UserID == id && d.DeviceTypeID == 43 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            usersheet.MonitorModel = db.Devices.OrderBy(d => d.DeviceID).Where(d => d.UserID == id && d.DeviceTypeID == 43 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            usersheet.MonitorSerial = db.Devices.OrderBy(d => d.DeviceID).Where(d => d.UserID == id && d.DeviceTypeID == 43 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            if(totalmonitor >= 2)
            {
                usersheet.Monitor2Brand = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 43 && d.StatusID == 1).OrderBy(d => d.DeviceID).Skip(totalmonitor - 1).Select(d => d.BrandName).DefaultIfEmpty().First();
                usersheet.Monitor2Model = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 43 && d.StatusID == 1).OrderBy(d => d.DeviceID).Skip(totalmonitor - 1).Select(d => d.ModelName).DefaultIfEmpty().First();
                usersheet.Monitor2Serial = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 43 && d.StatusID == 1).OrderBy(d => d.DeviceID).Skip(totalmonitor - 1).Select(d => d.SerialNumber).DefaultIfEmpty().First();
            }

            usersheet.ScannerBrand = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 58 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            usersheet.ScannerModel = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 58 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            usersheet.ScannerSerial = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 58 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            usersheet.UPSBrand = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 64 && d.StatusID == 1).Select(d => d.BrandName).DefaultIfEmpty("").First();
            usersheet.UPSModel = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 64 && d.StatusID == 1).Select(d => d.ModelName).DefaultIfEmpty("").First();
            usersheet.UPSSerial = db.Devices.Where(d => d.UserID == id && d.DeviceTypeID == 64 && d.StatusID == 1).Select(d => d.SerialNumber).DefaultIfEmpty("").First();

            usersheet.Plant = db.Users.Where(d => d.UserID == id).Select(d => d.Plant.PlantName).DefaultIfEmpty("").First();
            usersheet.Department = db.Users.Where(d => d.UserID == id).Select(d => d.Department.DepartmentName).DefaultIfEmpty("").First();
            usersheet.Location = db.Users.Where(d => d.UserID == id).Select(d => d.Location.LocationName).DefaultIfEmpty("").First();
            usersheet.Phase = db.Users.Where(d => d.UserID == id).Select(d => d.PhaseName).DefaultIfEmpty("").First();
            usersheet.UserName = db.Users.Where(d => d.UserID == id).Select(d => d.FullName).DefaultIfEmpty("").First();
            usersheet.DeviceName = db.Users.Where(d => d.UserID == id).Select(d => d.DeviceName).DefaultIfEmpty("").First();

            usersheet.Zip7Ver = db.UserSheets.Where(u => u.UserName == usersheet.UserName).Select(u => u.Zip7Ver).DefaultIfEmpty().First();
            usersheet.AcrobatVer = db.UserSheets.Where(u => u.UserName == usersheet.UserName).Select(u => u.AcrobatVer).DefaultIfEmpty().First();
            usersheet.AutoCADVer = db.UserSheets.Where(u => u.UserName == usersheet.UserName).Select(u => u.AutoCADVer).DefaultIfEmpty().First();
            usersheet.CureGraphVer = db.UserSheets.Where(u => u.UserName == usersheet.UserName).Select(u => u.CureGraphVer).DefaultIfEmpty().First();
            usersheet.CutePDFVer = db.UserSheets.Where(u => u.UserName == usersheet.UserName).Select(u => u.CutePDFVer).DefaultIfEmpty().First();
            usersheet.HealthBookVer = db.UserSheets.Where(u => u.UserName == usersheet.UserName).Select(u => u.HealthBookVer).DefaultIfEmpty().First();
            usersheet.HRMSystemVer = db.UserSheets.Where(u => u.UserName == usersheet.UserName).Select(u => u.HRMSystemVer).DefaultIfEmpty().First();
            usersheet.IllustratorVer = db.UserSheets.Where(u => u.UserName == usersheet.UserName).Select(u => u.IllustratorVer).DefaultIfEmpty().First();
            usersheet.JP1Ver = db.UserSheets.Where(u => u.UserName == usersheet.UserName).Select(u => u.JP1Ver).DefaultIfEmpty().First();
            usersheet.LexitronDictVer = db.UserSheets.Where(u => u.UserName == usersheet.UserName).Select(u => u.LexitronDictVer).DefaultIfEmpty().First();
            usersheet.MiniTabVer = db.UserSheets.Where(u => u.UserName == usersheet.UserName).Select(u => u.MiniTabVer).DefaultIfEmpty().First();
            usersheet.MSOfficeVer = db.UserSheets.Where(u => u.UserName == usersheet.UserName).Select(u => u.MSOfficeVer).DefaultIfEmpty().First();
            usersheet.PhotoShopVer = db.UserSheets.Where(u => u.UserName == usersheet.UserName).Select(u => u.PhotoShopVer).DefaultIfEmpty().First();
            usersheet.SAPVer = db.UserSheets.Where(u => u.UserName == usersheet.UserName).Select(u => u.SAPVer).DefaultIfEmpty().First();
            usersheet.SaveToPDFVer = db.UserSheets.Where(u => u.UserName == usersheet.UserName).Select(u => u.SaveToPDFVer).DefaultIfEmpty().First();
            usersheet.SeedWincsVer = db.UserSheets.Where(u => u.UserName == usersheet.UserName).Select(u => u.SeedWincsVer).DefaultIfEmpty().First();
            usersheet.SolidEdgeVer = db.UserSheets.Where(u => u.UserName == usersheet.UserName).Select(u => u.SolidEdgeVer).DefaultIfEmpty().First();
            usersheet.TrendMicroVer = db.UserSheets.Where(u => u.UserName == usersheet.UserName).Select(u => u.TrendMicroVer).DefaultIfEmpty().First();
            usersheet.WindowsVer = db.UserSheets.Where(u => u.UserName == usersheet.UserName).Select(u => u.WindowsVer).DefaultIfEmpty().First();
            usersheet.ZimbraVer = db.UserSheets.Where(u => u.UserName == usersheet.UserName).Select(u => u.ZimbraVer).DefaultIfEmpty().First();



            usersheet.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            usersheet.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            usersheet.DateCreate = DateTime.Now;
            usersheet.DateUpdate = DateTime.Now;
            #endregion

            ViewBag.DeviceID = id;
            ViewBag.PlantID = db.Users.Where(d => d.UserID == id).Select(d => d.PlantID).DefaultIfEmpty().First();
            ViewBag.DepartmentID = db.Users.Where(d => d.UserID == id).Select(d => d.DepartmentID).DefaultIfEmpty().First();
            ViewBag.LocationID = db.Users.Where(d => d.UserID == id).Select(d => d.LocationID).DefaultIfEmpty().First();

            return View(usersheet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSheet([Bind(Include = "SheetID,PCBoardBrand,PCBoardModel,PCBoardSerial,PCBoardRemark,ProcessorBrand,ProcessorModel,ProcessorSerial,ProcessorRemark,MemoryBrand,MemoryModel,MemorySerial,MemoryRemark,VideoCardBrand,VideoCardModel,VideoCardSerial,VideoCardRemark,LANCardBrand,LANCardModel,LANCardSerial,LANCardRemark,MouseBrand,MouseModel,MouseSerial,MouseRemark,MonitorBrand,MonitorModel,MonitorSerial,MonitorRemark,Monitor2Brand,Monitor2Model,Monitor2Serial,Monitor2Remark,KeyboardBrand,KeyboardModel,KeyboardSerial,KeyboardRemark,ScannerBrand,ScannerModel,ScannerSerial,ScannerRemark,DVDBrand,DVDModel,DVDSerial,DVDRemark,HDD1Brand,HDD1Model,HDD1Serial,HDD1Remark,HDD2Brand,HDD2Model,HDD2Serial,HDD2Remark,PrinterBrand,PrinterModel,PrinterSerial,PrinterRemark,UPSBrand,UPSModel,UPSSerial,UPSRemark,Zip7Ver,Zip7Remark,AcrobatVer,AcrobatRemark,AutoCADVer,AutoCADRemark,CureGraphVer,CureGraphRemark,CutePDFVer,CutePDFRemark,HealthBookVer,HealthBookRemark,HRMSystemVer,HRMSystemRemark,IllustratorVer,IllustratorRemark,JP1Ver,JP1Remark,LexitronDictVer,LexitronDictRemark,MiniTabVer,MiniTabRemark,MSOfficeVer,MSOfficeRemark,PhotoShopVer,PhotoShopRemark,SAPVer,SAPRemark,SaveToPDFVer,SaveToPDFRemark,SeedWincsVer,SeedWincsRemark,SolidEdgeVer,SolidEdgeRemark,TrendMicroVer,TrendMicroRemark,WindowsVer,WindowsRemark,ZimbraVer,ZimbraRemark,OtherProgram,OtherProgramVer,OtherProgramRemark,OtherProgram1,OtherProgram1Ver,OtherProgram1Remark,OtherProgram2,OtherProgram2Ver,OtherProgram2Remark,UserID,UserName,PlantID,DepartmentID,LocationID,PhaseID,Plant,Department,Location,Phase,FixAccess,CreateBy,UpdateBy,DateCreate,DateUpdate,DeviceName")] UserSheet usersheet)
        {
            if (ModelState.IsValid)
            {
                usersheet.PlantID = db.Users.Where(m => m.Plant.PlantName == usersheet.Plant).Select(m => m.PlantID).DefaultIfEmpty().First();
                usersheet.DepartmentID = db.Users.Where(m => m.Department.DepartmentName == usersheet.Department).Select(m => m.DepartmentID).DefaultIfEmpty().First();
                usersheet.LocationID = db.Users.Where(m => m.Location.LocationName == usersheet.Location).Select(m => m.LocationID).DefaultIfEmpty().First();
                usersheet.PhaseID = db.Users.Where(m => m.PhaseName == usersheet.Phase).Select(m => m.PhaseID).DefaultIfEmpty().First();
                usersheet.UserID = db.Users.Where(m => m.FullName == usersheet.UserName).Select(m => m.UserID).DefaultIfEmpty().First();
                usersheet.DeviceName = usersheet.DeviceName;

                db.UserSheets.Add(usersheet);
                db.SaveChanges();
                return RedirectToAction("CreateUserSheet", "User", new { id= usersheet.PlantID, did = usersheet.DepartmentID});
            }

            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentID", "DepartmentName", usersheet.DepartmentID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(d => d.LocationName), "LocationID", "LocationName", usersheet.LocationID);
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", usersheet.PhaseID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(d => d.PlantName), "PlantID", "PlantName", usersheet.PlantID);
            return View(usersheet);
        }

        // GET: /User/Create
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Create()
        {
            User user = new User();
            user.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            user.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            user.DateCreate = DateTime.Now;
            user.DateUpdate = DateTime.Now;
            user.PreviousUri = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();

            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName");
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d=>d.DepartmentName), "DepartmentID", "DepartmentName");
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(l=>l.LocationName), "LocationID", "LocationName");
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(p=>p.PlantName), "PlantID", "PlantName");
            return View(user);
        }

        // POST: /User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,FirstName,LastName,EmployeeID,Position,Section,Phone,DepartmentID,LocationID,PlantID,UserLogOn,IPAddress,MacAddress,CreateBy,UpdateBy,DateCreate,DateUpdate,PhaseID,PhaseName,DeviceName,PreviousUri")] User user)
        {
            if (ModelState.IsValid && db.Users.Any(d => d.UserLogOn == user.UserLogOn))
            {
                ModelState.AddModelError("UserLogOn", "UserLogOn is Duplicate");
            }

            if (ModelState.IsValid && !db.Users.Any(u=>u.IPAddress == user.IPAddress && u.IPAddress != "DHCP"))
            {
                user.FullName = user.FirstName + " " + user.LastName;
                user.PhaseName = db.Phases.Where(b => b.PhaseID == user.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                db.Users.Add(user);
                db.SaveChanges();
                //return RedirectToAction("Index");
                return Redirect(user.PreviousUri);
            }
            ModelState.AddModelError("IPAddress", "IPAddress is Duplicate");

            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", user.PhaseID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d=>d.DepartmentName), "DepartmentID", "DepartmentName", user.DepartmentID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(l=>l.LocationName), "LocationID", "LocationName", user.LocationID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(p=>p.PlantName), "PlantID", "PlantName", user.PlantID);
            return View(user);
        }

        // GET: /User/Edit/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            user.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            user.PreviousUri = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
            user.DateUpdate = DateTime.Now;
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", user.PhaseID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d=>d.DepartmentName), "DepartmentID", "DepartmentName", user.DepartmentID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(l=>l.LocationName), "LocationID", "LocationName", user.LocationID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(p=>p.PlantName), "PlantID", "PlantName", user.PlantID);
            return View(user);
        }

        // POST: /User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,FirstName,LastName,EmployeeID,Position,Section,Phone,DepartmentID,LocationID,PlantID,UserLogOn,IPAddress,MacAddress,CreateBy,UpdateBy,DateCreate,DateUpdate,PhaseID,PhaseName,DeviceName,NickName,PreviousUri")] User user)
        {
            if (ModelState.IsValid)
            {
                user.FullName = user.FirstName + " " + user.LastName;
                user.PhaseName = db.Phases.Where(b => b.PhaseID == user.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Index");
                return Redirect(user.PreviousUri);
            }
            ViewBag.PhaseID = new SelectList(db.Phases.OrderBy(d => d.PhaseName), "PhaseID", "PhaseName", user.PhaseID);
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d=>d.DepartmentName), "DepartmentID", "DepartmentName", user.DepartmentID);
            ViewBag.LocationID = new SelectList(db.Locations.OrderBy(l=>l.LocationName), "LocationID", "LocationName", user.LocationID);
            ViewBag.PlantID = new SelectList(db.Plants.OrderBy(p=>p.PlantName), "PlantID", "PlantName", user.PlantID);
            return View(user);
        }

        // GET: /User/Delete/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.PreviousUrl = System.Web.HttpContext.Current.Request.UrlReferrer;
            ViewBag.UserID = id;
            ViewBag.CountDevice = db.Devices.Where(d => d.UserID == id).Count();
            return View(user);
        }

        // POST: /User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string uri)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            //return RedirectToAction("Index");
            return Redirect(uri);
        }

        public JsonResult getUserDevice(int id)
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

                           where d.UserID == id

                           select new ViewModelsDevices
                           {
                               DeviceID = d.DeviceID,
                               MachineName = mc.MachineName,
                               UserName = us.FullName,
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
