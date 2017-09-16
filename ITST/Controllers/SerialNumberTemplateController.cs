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
    public class SerialNumberTemplateController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        // GET: /SerialNumberTemplate/
        public ActionResult Index()
        {
            return View(db.SerialNumberTemplates.ToList());
        }

        // GET: /SerialNumberTemplate/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SerialNumberTemplate serialnumbertemplate = db.SerialNumberTemplates.Find(id);
            if (serialnumbertemplate == null)
            {
                return HttpNotFound();
            }
            return View(serialnumbertemplate);
        }

        // GET: /SerialNumberTemplate/Create
        [Authorize]
        public ActionResult Create()
        {
            SerialNumberTemplate serialnumbertemplate = new SerialNumberTemplate();
            serialnumbertemplate.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            serialnumbertemplate.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            serialnumbertemplate.DateCreate = DateTime.Now;
            serialnumbertemplate.DateUpdate = DateTime.Now;
            ViewBag.DeviceType = new SelectList(db.DeviceTypes.OrderBy(s => s.Type), "Type", "Type", serialnumbertemplate.DeviceType);
            return View(serialnumbertemplate);
        }

        // POST: /SerialNumberTemplate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="TemplateID,TemplateName,DeviceType,CreateBy,UpdateBy,DateCreate,DateUpdate")] SerialNumberTemplate serialnumbertemplate)
        {
            var type = db.SerialNumberTemplates.Where(s => s.DeviceType == serialnumbertemplate.DeviceType).Select(s => s.DeviceType).DefaultIfEmpty().First();
            var name = db.SerialNumberTemplates.Where(s => s.TemplateName == serialnumbertemplate.TemplateName).Select(s => s.TemplateName).DefaultIfEmpty().First();

            if (!string.IsNullOrEmpty(type))
            {
                ModelState.AddModelError("DeviceType", "DeviceType is Duplicated");
            }

            if (!string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError("TemplateName", "TemplateName is Duplicated");
            }

            if (ModelState.IsValid)
            {
                db.SerialNumberTemplates.Add(serialnumbertemplate);
                db.SaveChanges();
                return RedirectToAction("GenerateSerialNumber", "Dashboard");
            }
            ViewBag.DeviceType = new SelectList(db.DeviceTypes.OrderBy(s => s.Type), "Type", "Type", serialnumbertemplate.DeviceType);

            return View(serialnumbertemplate);
        }

        // GET: /SerialNumberTemplate/Edit/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SerialNumberTemplate serialnumbertemplate = db.SerialNumberTemplates.Find(id);
            if (serialnumbertemplate == null)
            {
                return HttpNotFound();
            }
            return View(serialnumbertemplate);
        }

        // POST: /SerialNumberTemplate/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="TemplateID,TemplateName,DeviceType,CreateBy,UpdateBy,DateCreate,DateUpdate")] SerialNumberTemplate serialnumbertemplate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(serialnumbertemplate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("GenerateSerialNumber", "Dashboard");
            }
            return View(serialnumbertemplate);
        }

        // GET: /SerialNumberTemplate/Delete/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SerialNumberTemplate serialnumbertemplate = db.SerialNumberTemplates.Find(id);
            if (serialnumbertemplate == null)
            {
                return HttpNotFound();
            }
            return View(serialnumbertemplate);
        }

        // POST: /SerialNumberTemplate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SerialNumberTemplate serialnumbertemplate = db.SerialNumberTemplates.Find(id);
            db.SerialNumberTemplates.Remove(serialnumbertemplate);
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
