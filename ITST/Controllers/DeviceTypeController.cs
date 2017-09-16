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
    public class DeviceTypeController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        // GET: /DeviceType/
        [Authorize]
        public ActionResult Index()
        {
            //return View(db.DeviceTypes.ToList());
            return View();
        }

        public JsonResult getDeviceType()
        {
            using (var ctx = new ITStockEntities1())
            {
                //Disable Proxy creation
                ctx.Configuration.ProxyCreationEnabled = false;
                var type = ctx.DeviceTypes.OrderBy(p => p.Type).ToList();
                return Json(new { data = type }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: /DeviceType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviceType devicetype = db.DeviceTypes.Find(id);
            if (devicetype == null)
            {
                return HttpNotFound();
            }
            return View(devicetype);
        }

        // GET: /DeviceType/Create
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Create()
        {
            DeviceType devicetype = new DeviceType();
            devicetype.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            devicetype.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            devicetype.DateCreate = DateTime.Now;
            devicetype.DateUpdate = DateTime.Now;
            return View(devicetype);
        }

        // POST: /DeviceType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="DeviceTypeID,Type,Description,Remark,DateCreate,DateUpdate,CreateBy,UpdateBy")] DeviceType devicetype)
        {
            if (ModelState.IsValid && !db.DeviceTypes.Any(d => d.Type == devicetype.Type))
            {
                db.DeviceTypes.Add(devicetype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("Type", "Type is Duplicate");

            return View(devicetype);
        }

        // GET: /DeviceType/Edit/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviceType devicetype = db.DeviceTypes.Find(id);
            devicetype.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            devicetype.DateUpdate = DateTime.Now;
            if (devicetype == null)
            {
                return HttpNotFound();
            }
            return View(devicetype);
        }

        // POST: /DeviceType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="DeviceTypeID,Type,Description,Remark,DateCreate,DateUpdate,CreateBy,UpdateBy")] DeviceType devicetype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(devicetype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(devicetype);
        }

        // GET: /DeviceType/Delete/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviceType devicetype = db.DeviceTypes.Find(id);
            if (devicetype == null)
            {
                return HttpNotFound();
            }
            return View(devicetype);
        }

        // POST: /DeviceType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DeviceType devicetype = db.DeviceTypes.Find(id);
            db.DeviceTypes.Remove(devicetype);
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
