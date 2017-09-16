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

namespace ITST.Controllers
{
    public class BrandController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        // GET: /Brand/
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Index()
        {
            //return View(db.Brands.OrderBy(b=>b.BrandName).ToList());
            return View();
        }

        public JsonResult getBrand()
        {
            using (var ctx = new ITStockEntities1())
            {
                //Disable Proxy creation
                ctx.Configuration.ProxyCreationEnabled = false;
                var brand = ctx.Brands.OrderBy(p => p.BrandName).ToList();
                return Json(new { data = brand }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult FindDevice(string prefixText)
        {
            var modelname = from x in db.Devices
                            where x.SerialNumber.StartsWith(prefixText) || x.DeviceType.Type.StartsWith(prefixText) || x.Brand.BrandName.StartsWith(prefixText) || x.Model.ModelName.StartsWith(prefixText)
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

        // GET: /Brand/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brands.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        // GET: /Brand/Create
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Create()
        {
            Brand brand = new Brand();
            brand.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            brand.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            brand.DateCreate = DateTime.Now;
            brand.DateUpdate = DateTime.Now;
            return View(brand);
        }

        // POST: /Brand/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [AuthLog(Roles = "SuperUser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="BrandID,BrandName,Description,Remark,DateCreate,DateUpdate,CreateBy,UpdateBy")] Brand brand)
        {
            if (ModelState.IsValid && !db.Brands.Any(d => d.BrandName == brand.BrandName))
            {
                db.Brands.Add(brand);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("BrandName", "BrandName is Duplicate");


            return View(brand);
        }

        // GET: /Brand/Edit/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brands.Find(id);
            brand.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            brand.DateUpdate = DateTime.Now;
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        // POST: /Brand/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [AuthLog(Roles = "SuperUser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="BrandID,BrandName,Description,Remark,DateCreate,DateUpdate,CreateBy,UpdateBy")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                db.Entry(brand).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(brand);
        }

        // GET: /Brand/Delete/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brands.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        // POST: /Brand/Delete/5
        [AuthLog(Roles = "SuperUser")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Brand brand = db.Brands.Find(id);
            db.Brands.Remove(brand);
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
