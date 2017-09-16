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
    public class PhoneController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        // GET: /Phone/
        public ActionResult Index()
        {
            return View(db.IntExtPhones.ToList());
        }

        public ActionResult IntExtPhone()
        {
            return View();
        }

        public JsonResult getInternalPhonelist()
        {
            var phone = db.IntExtPhones.Where(p => p.Type == "I").ToList();
            return Json(new { data = phone }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getEmergencyPhonelist()
        {
            var phone = db.IntExtPhones.Where(p => p.Type == "E").ToList();
            return Json(new { data = phone }, JsonRequestBehavior.AllowGet);
        }

        // GET: /Phone/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IntExtPhone intextphone = db.IntExtPhones.Find(id);
            if (intextphone == null)
            {
                return HttpNotFound();
            }
            return View(intextphone);
        }

        // GET: /Phone/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Phone/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,LocationName,PhoneNumber,Type")] IntExtPhone intextphone)
        {
            if (ModelState.IsValid)
            {
                db.IntExtPhones.Add(intextphone);
                db.SaveChanges();
                return RedirectToAction("IntExtPhone");
            }

            return View(intextphone);
        }

        // GET: /Phone/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IntExtPhone intextphone = db.IntExtPhones.Find(id);
            if (intextphone == null)
            {
                return HttpNotFound();
            }
            return View(intextphone);
        }

        // POST: /Phone/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,LocationName,PhoneNumber,Type")] IntExtPhone intextphone)
        {
            if (ModelState.IsValid)
            {
                db.Entry(intextphone).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IntExtPhone");
            }
            return View(intextphone);
        }

        // GET: /Phone/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IntExtPhone intextphone = db.IntExtPhones.Find(id);
            if (intextphone == null)
            {
                return HttpNotFound();
            }
            return View(intextphone);
        }

        // POST: /Phone/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IntExtPhone intextphone = db.IntExtPhones.Find(id);
            db.IntExtPhones.Remove(intextphone);
            db.SaveChanges();
            return RedirectToAction("IntExtPhone");
        }

        public static IEnumerable<SelectListItem> getPhoneType()
        {
            IList<SelectListItem> types = new List<SelectListItem>
            {
                new SelectListItem() {Text="", Value=""},
                new SelectListItem() { Text="Internal", Value="I"},
                new SelectListItem() { Text="Emergency", Value="E"},
            };
            return types;
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
