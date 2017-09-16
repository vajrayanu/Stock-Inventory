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
    public class PhaseController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        // GET: /Phase/
        [Authorize]
        public ActionResult Index()
        {
            return View(db.Phases.ToList());
        }

        // GET: /Phase/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phase phase = db.Phases.Find(id);
            if (phase == null)
            {
                return HttpNotFound();
            }
            return View(phase);
        }

        // GET: /Phase/Create
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Create()
        {
            Phase phase = new Phase();
            phase.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            phase.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            phase.DateCreate = DateTime.Now;
            phase.DateUpdate = DateTime.Now;
            return View(phase);
        }

        // POST: /Phase/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="PhaseID,PhaseName,Description,DateCreate,DateUpdate,CreateBy,UpdateBy")] Phase phase)
        {
            if (ModelState.IsValid && !db.Phases.Any(d => d.PhaseName == phase.PhaseName))
            {
                db.Phases.Add(phase);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("PhaseName", "PhaseName is Duplicate");

            return View(phase);
        }

        // GET: /Phase/Edit/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phase phase = db.Phases.Find(id);
            phase.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            phase.DateUpdate = DateTime.Now;
            if (phase == null)
            {
                return HttpNotFound();
            }
            return View(phase);
        }

        // POST: /Phase/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="PhaseID,PhaseName,Description,DateCreate,DateUpdate,CreateBy,UpdateBy")] Phase phase)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phase).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(phase);
        }

        // GET: /Phase/Delete/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phase phase = db.Phases.Find(id);
            if (phase == null)
            {
                return HttpNotFound();
            }
            return View(phase);
        }

        // POST: /Phase/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Phase phase = db.Phases.Find(id);
            db.Phases.Remove(phase);
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
