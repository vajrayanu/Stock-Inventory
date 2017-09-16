using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ITST.Models;

namespace ITST.Controllers
{
    public class LocationStockController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        // GET: /LocationStock/
        [Authorize]
        public ActionResult Index()
        {
            return View(db.LocationStocks.ToList());
        }

        // GET: /LocationStock/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocationStock locationstock = db.LocationStocks.Find(id);
            if (locationstock == null)
            {
                return HttpNotFound();
            }
            return View(locationstock);
        }

        // GET: /LocationStock/Create
        public ActionResult Create()
        {
            LocationStock locationstock = new LocationStock();
            locationstock.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            locationstock.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            locationstock.DateCreate = DateTime.Now;
            locationstock.DateUpdate = DateTime.Now;
            return View(locationstock);
        }

        // POST: /LocationStock/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LocationID,LocationName,CreateBy,UpdateBy,DateCreate,DateUpdate")] LocationStock locationstock)
        {
            if (ModelState.IsValid && !db.LocationStocks.Any(d => d.LocationName == locationstock.LocationName))
            {
                db.LocationStocks.Add(locationstock);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("LocationName", "LocationName is Duplicate");


            return View(locationstock);
        }

        // GET: /LocationStock/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocationStock locationstock = db.LocationStocks.Find(id);
            locationstock.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            locationstock.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            locationstock.DateCreate = DateTime.Now;
            locationstock.DateUpdate = DateTime.Now;
            if (locationstock == null)
            {
                return HttpNotFound();
            }
            return View(locationstock);
        }

        // POST: /LocationStock/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LocationID,LocationName,CreateBy,UpdateBy,DateCreate,DateUpdate")] LocationStock locationstock)
        {
            if (ModelState.IsValid)
            {
                db.Entry(locationstock).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(locationstock);
        }

        // GET: /LocationStock/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocationStock locationstock = db.LocationStocks.Find(id);
            if (locationstock == null)
            {
                return HttpNotFound();
            }
            return View(locationstock);
        }

        // POST: /LocationStock/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LocationStock locationstock = db.LocationStocks.Find(id);
            db.LocationStocks.Remove(locationstock);
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
