using ITST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITST.Controllers
{
    public class RequistionLogController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        //
        // GET: /RequistionLog/
        public ActionResult Index()
        {
            var recordrequistions = db.RecordRequisitions.OrderByDescending(r=>r.DateRequisition);
            return View(recordrequistions.ToList());
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