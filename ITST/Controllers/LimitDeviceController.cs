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
    public class LimitDeviceController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        // GET: /LimitDevice/
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Index()
        {
            return View(db.LimitDeviceQuantities.ToList());
        }

        // GET: /LimitDevice/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LimitDeviceQuantity limitdevicequantity = db.LimitDeviceQuantities.Find(id);
            if (limitdevicequantity == null)
            {
                return HttpNotFound();
            }
            return View(limitdevicequantity);
        }

        // GET: /LimitDevice/Create
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /LimitDevice/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Plant,Department,Location,Phase,Machine,MaxQuantity,DeviceType,CreateBy,UpdateBy,DateCreate,DateUpdate")] LimitDeviceQuantity limitdevicequantity)
        {
            if (db.LimitDeviceQuantities.Any(d => d.Machine == limitdevicequantity.Machine && d.DeviceType == limitdevicequantity.DeviceType))
            {
                ModelState.AddModelError("Machine", " ");
            }

            if (ModelState.IsValid)
            {
                limitdevicequantity.Plant = db.Machines.Where(m => m.MachineName == limitdevicequantity.Machine).Select(m => m.Plant.PlantName).DefaultIfEmpty().First();
                limitdevicequantity.Department = db.Machines.Where(m => m.MachineName == limitdevicequantity.Machine).Select(m => m.Department.DepartmentName).DefaultIfEmpty().First();
                limitdevicequantity.Location = db.Machines.Where(m => m.MachineName == limitdevicequantity.Machine).Select(m => m.Location.LocationName).DefaultIfEmpty().First();
                limitdevicequantity.Phase = db.Machines.Where(m => m.MachineName == limitdevicequantity.Machine).Select(m => m.Phase.PhaseName).DefaultIfEmpty().First();
                limitdevicequantity.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
                limitdevicequantity.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                limitdevicequantity.DateCreate = DateTime.Now;
                limitdevicequantity.DateUpdate = DateTime.Now;
                db.LimitDeviceQuantities.Add(limitdevicequantity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Duplicate = "Y";
            return View(limitdevicequantity);
        }

        // GET: /LimitDevice/Edit/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LimitDeviceQuantity limitdevicequantity = db.LimitDeviceQuantities.Find(id);
            if (limitdevicequantity == null)
            {
                return HttpNotFound();
            }
            return View(limitdevicequantity);
        }

        // POST: /LimitDevice/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Plant,Department,Location,Phase,Machine,MaxQuantity,DeviceType,CreateBy,UpdateBy,DateCreate,DateUpdate")] LimitDeviceQuantity limitdevicequantity)
        {
            if (ModelState.IsValid)
            {
                limitdevicequantity.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
                limitdevicequantity.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                limitdevicequantity.DateCreate = DateTime.Now;
                limitdevicequantity.DateUpdate = DateTime.Now;
                db.Entry(limitdevicequantity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(limitdevicequantity);
        }

        // GET: /LimitDevice/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LimitDeviceQuantity limitdevicequantity = db.LimitDeviceQuantities.Find(id);
            if (limitdevicequantity == null)
            {
                return HttpNotFound();
            }
            return View(limitdevicequantity);
        }

        // POST: /LimitDevice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LimitDeviceQuantity limitdevicequantity = db.LimitDeviceQuantities.Find(id);
            db.LimitDeviceQuantities.Remove(limitdevicequantity);
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
