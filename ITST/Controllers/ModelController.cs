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
    public class ModelController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        // GET: /Model/
        [Authorize]
        public ActionResult Index()
        {
            //var models = db.Models.OrderBy(m=>m.ModelName).Include(m => m.Brand).Include(m => m.DeviceType);
            //return View(models.ToList());
            return View();
        }

        public JsonResult getModels()
        {
            using (var ctx = new ITStockEntities1())
            {
                var models = (from d in db.Models
                               join i in db.Brands on d.BrandID equals i.BrandID
                               into tempPets
                               from i in tempPets.DefaultIfEmpty()

                               join p in db.DeviceTypes on d.DeviceTypeID equals p.DeviceTypeID
                               into tempPets2
                               from p in tempPets2.DefaultIfEmpty()

                               select new ViewDeviceModels
                               {
                                   ModelID = d.ModelID,
                                   CreateBy = d.CreateBy,
                                   UpdateBy = d.UpdateBy,
                                   DateCreate = d.DateCreate,
                                   DateUpdate = d.DateUpdate,
                                   Specification = d.Specification,
                                   IsAccess = d.IsAccess,
                                   Description = d.Description,
                                   BrandName = i.BrandName,
                                   DeviceType = p.Type,
                                   ModelName = d.ModelName,
                               }).ToList();
                return Json(new { data = models.OrderBy(p=>p.ModelName) }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: /Model/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Model model = db.Models.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: /Model/Create
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Create()
        {
            Model model = new Model();
            model.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.DateCreate = DateTime.Now;
            model.DateUpdate = DateTime.Now;

            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(b=>b.BrandName), "BrandID", "BrandName");
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d=>d.Type), "DeviceTypeID", "Type");
            return View(model);
        }

        // POST: /Model/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [AuthLog(Roles = "SuperUser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ModelID,ModelName,DeviceTypeID,BrandID,Specification,DateCreate,DateUpdate,CreateBy,UpdateBy,IsAccess,Description")] Model model, HttpPostedFileBase file)
        {
            if (string.IsNullOrEmpty(model.Specification))
            {
                ModelState.AddModelError("Specification", "Specification is Required");
            }

            if (ModelState.IsValid && !db.Models.Any(d => d.ModelName == model.ModelName))
            {
                if (file != null)
                {
                    file.SaveAs(HttpContext.Server.MapPath("~/Content/Images/") + file.FileName);
                    model.ImagePath = file.FileName;
                }
                db.Models.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("ModelName", "ModelName is Duplicate");

            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName", model.BrandID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes, "DeviceTypeID", "Type", model.DeviceTypeID);
            return View(model);
        }

        // GET: /Model/Edit/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Model model = db.Models.Find(id);
            model.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.DateUpdate = DateTime.Now;
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(b=>b.BrandName), "BrandID", "BrandName", model.BrandID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes.OrderBy(d=>d.Type), "DeviceTypeID", "Type", model.DeviceTypeID);
            return View(model);
        }

        // POST: /Model/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [AuthLog(Roles = "SuperUser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ModelID,ModelName,DeviceTypeID,BrandID,Specification,ImagePath,DateCreate,DateUpdate,CreateBy,UpdateBy,IsAccess,Description")] Model model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    file.SaveAs(HttpContext.Server.MapPath("~/Content/Images/") + file.FileName);
                    model.ImagePath = file.FileName;
                }

                db.Entry(model).State = EntityState.Modified;
                var device = db.Devices.Where(d => d.ModelID == model.ModelID).ToList();
                foreach (var i in device)
                {
                    i.BrandID = model.BrandID;
                    i.DeviceTypeID = model.DeviceTypeID;
                    i.BrandName = db.Brands.Where(b => b.BrandID == model.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                    i.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == model.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                    i.ModelName = model.ModelName;
                    i.Specification = model.Specification;
                    //i.DateUpdate = DateTime.Now;

                    //RecordDevice recorddevice = new RecordDevice();
                    //recorddevice.Brand = db.Brands.Where(b => b.BrandID == i.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                    //recorddevice.Type = db.DeviceTypes.Where(b => b.DeviceTypeID == i.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                    //recorddevice.EditBy = System.Web.HttpContext.Current.User.Identity.Name;
                    //recorddevice.EditDate = DateTime.Now;
                    //recorddevice.Description = "Edit Device";
                    //recorddevice.Specification = db.Models.Where(b => b.ModelID == i.ModelID).Select(b => b.Specification).DefaultIfEmpty().First();
                    //recorddevice.Model = db.Models.Where(b => b.ModelID == i.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                    //recorddevice.SerialNumber = db.Devices.Where(b => b.DeviceID == i.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                    //recorddevice.Plant = db.Plants.Where(b => b.PlantID == i.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                    //recorddevice.Department = db.Departments.Where(b => b.DepartmentID == i.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                    //recorddevice.Location = db.Locations.Where(b => b.LocationID == i.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                    //recorddevice.Phase = db.Phases.Where(b => b.PhaseID == i.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                    //recorddevice.LocationStock = db.LocationStocks.Where(b => b.LocationName == i.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                    //recorddevice.Machine = db.Machines.Where(b => b.MachineID == i.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                    //recorddevice.UserName = db.Users.Where(b => b.UserID == i.UserID).Select(b => b.FullName).DefaultIfEmpty().First();
                    //recorddevice.Status = db.Status.Where(b => b.StatusID == i.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();

                    db.Entry(i).State = EntityState.Modified;
                    //db.RecordDevices.Add(recorddevice);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName", model.BrandID);
            ViewBag.DeviceTypeID = new SelectList(db.DeviceTypes, "DeviceTypeID", "Type", model.DeviceTypeID);
            return View(model);
        }

        // GET: /Model/Delete/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Model model = db.Models.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /Model/Delete/5
        [AuthLog(Roles = "SuperUser")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Model model = db.Models.Find(id);
            db.Models.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
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
