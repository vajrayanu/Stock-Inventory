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
    public class SerialNumberGenerateController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        // GET: /SerialNumberGenerate/
        public ActionResult Index()
        {
            return View(db.SerialNumberGenerates.ToList());
        }

        // GET: /SerialNumberGenerate/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SerialNumberGenerate serialnumbergenerate = db.SerialNumberGenerates.Find(id);
            if (serialnumbergenerate == null)
            {
                return HttpNotFound();
            }
            return View(serialnumbergenerate);
        }

        // GET: /SerialNumberGenerate/Create
        [Authorize]
        public ActionResult Create()
        {
            SerialNumberGenerate serialnumbergenerate = new SerialNumberGenerate();
            serialnumbergenerate.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            serialnumbergenerate.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            serialnumbergenerate.DateCreate = DateTime.Now;
            serialnumbergenerate.DateUpdate = DateTime.Now;
            ViewBag.DeviceType = new SelectList(db.SerialNumberTemplates.OrderBy(s => s.DeviceType), "DeviceType", "DeviceType", serialnumbergenerate.DeviceType);
            return View(serialnumbergenerate);
        }

        // POST: /SerialNumberGenerate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="GenerateID,SerialNumber,DeviceType,CreateBy,UpdateBy,DateCreate,DateUpdate")] SerialNumberGenerate serialnumbergenerate)
        {
            var number = "";
            var srnb = db.SerialNumberGenerates.Where(g => g.DeviceType == serialnumbergenerate.DeviceType).Select(g => g.SerialNumber).DefaultIfEmpty().First();
            if (srnb == null)
            {
                number = null;
            }
            else if (srnb != null)
            {
                number = db.SerialNumberGenerates.Where(g => g.DeviceType == serialnumbergenerate.DeviceType).OrderByDescending(g => g.GenerateID).First().SerialNumber;
            }
            if (string.IsNullOrEmpty(number))
            {
                number = "0000";
            }

            if (ModelState.IsValid)
            {
                var template = db.SerialNumberTemplates.Where(t => t.DeviceType == serialnumbergenerate.DeviceType).Select(t => t.TemplateName).DefaultIfEmpty().First();
                string substr = number.Substring(number.Length - 4);
                int x = int.Parse(substr);
                int val = x + 1;
                string sr = val.ToString().PadLeft(4, '0');
                serialnumbergenerate.SerialNumber = template + sr;
                db.SerialNumberGenerates.Add(serialnumbergenerate);
                db.SaveChanges();
                return RedirectToAction("GenerateSerialNumber","Dashboard");
            }
            ViewBag.DeviceType = new SelectList(db.SerialNumberTemplates.OrderBy(s => s.DeviceType), "DeviceType", "DeviceType", serialnumbergenerate.DeviceType);
            return View(serialnumbergenerate);
        }

        [Authorize]
        public ActionResult CreateNewDevice(int? id)
        {
            var serialnumber = db.SerialNumberGenerates.Where(s => s.GenerateID == id).Select(s => s.SerialNumber).DefaultIfEmpty().First();
            CreateDeviceViewModels device = new CreateDeviceViewModels();
            device.SerialNumber = serialnumber;
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName");
            ViewBag.Uri = System.Web.HttpContext.Current.Request.UrlReferrer;
            return View(device);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewDevice([Bind(Include = "SerialNumber,ModelName,LocationStock,IPAddress,FixAccess,PRNumber,IsAsset,MacAddress")] CreateDeviceViewModels viewmodels, string Uri)
        {
            var sr = viewmodels.SerialNumber.Replace("\r", "").Replace("\n", "").Replace("\t", "").ToString();

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

            if (ModelState.IsValid)
            {
                Device device = new Device();
                device.StatusID = 3;
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
                return RedirectToAction("LastCreate","Device", new { uri = Uri });
            }
            ViewBag.LocationStockID = new SelectList(db.LocationStocks.OrderBy(d => d.LocationName), "LocationID", "LocationName", viewmodels.LocationStock);
            return View(viewmodels);
        }

        // GET: /SerialNumberGenerate/Edit/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SerialNumberGenerate serialnumbergenerate = db.SerialNumberGenerates.Find(id);
            if (serialnumbergenerate == null)
            {
                return HttpNotFound();
            }
            return View(serialnumbergenerate);
        }

        // POST: /SerialNumberGenerate/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="GenerateID,SerialNumber,DeviceType,CreateBy,UpdateBy,DateCreate,DateUpdate")] SerialNumberGenerate serialnumbergenerate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(serialnumbergenerate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("GenerateSerialNumber", "Dashboard");
            }
            return View(serialnumbergenerate);
        }

        // GET: /SerialNumberGenerate/Delete/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SerialNumberGenerate serialnumbergenerate = db.SerialNumberGenerates.Find(id);
            if (serialnumbergenerate == null)
            {
                return HttpNotFound();
            }
            return View(serialnumbergenerate);
        }

        // POST: /SerialNumberGenerate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SerialNumberGenerate serialnumbergenerate = db.SerialNumberGenerates.Find(id);
            db.SerialNumberGenerates.Remove(serialnumbergenerate);
            db.SaveChanges();
            return RedirectToAction("GenerateSerialNumber", "Dashboard");
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
