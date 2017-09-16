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
    public class DepartmentController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        // GET: /Department/
        [Authorize]
        public ActionResult Index()
        {
            return View(db.Departments.ToList());
        }

        // GET: /Department/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: /Department/Create
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Create()
        {
            Department department = new Department();
            department.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            department.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            department.DateCreate = DateTime.Now;
            department.DateUpdate = DateTime.Now;
            return View(department);
        }

        // POST: /Department/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="DepartmentID,DepartmentName,Description,DateCreate,DateUpdate,CreateBy,UpdateBy")] Department department)
        {
            if (ModelState.IsValid && !db.Departments.Any(d => d.DepartmentName == department.DepartmentName))
            {
                db.Departments.Add(department);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("DepartmentName", "DepartmentName is Duplicate");

            return View(department);
        }

        // GET: /Department/Edit/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            department.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            department.DateUpdate = DateTime.Now;
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: /Department/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="DepartmentID,DepartmentName,Description,DateCreate,DateUpdate,CreateBy,UpdateBy")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(department);
        }

        // GET: /Department/Delete/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: /Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Department department = db.Departments.Find(id);
            db.Departments.Remove(department);
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
