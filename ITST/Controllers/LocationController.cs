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
    public class LocationController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        // GET: /Location/
        [Authorize]
        public ActionResult Index()
        {
            return View(db.Locations.ToList());
        }

        // GET: /Location/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // GET: /Location/Create
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Create()
        {
            Location location = new Location();
            location.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            location.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            location.DateCreate = DateTime.Now;
            location.DateUpdate = DateTime.Now;
            return View(location);
        }

        // POST: /Location/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="LocationID,LocationName,Description,DateCreate,DateUpdate,CreateBy,UpdateBy")] Location location)
        {
            if (ModelState.IsValid && !db.Locations.Any(d => d.LocationName == location.LocationName))
            {
                db.Locations.Add(location);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("LocationName", "LocationName is Duplicate");


            return View(location);
        }

        // GET: /Location/Edit/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            location.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            location.DateUpdate = DateTime.Now;
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // POST: /Location/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="LocationID,LocationName,Description,DateCreate,DateUpdate,CreateBy,UpdateBy")] Location location)
        {
            if (ModelState.IsValid)
            {
                db.Entry(location).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(location);
        }

        // GET: /Location/Delete/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // POST: /Location/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Location location = db.Locations.Find(id);
            db.Locations.Remove(location);
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
