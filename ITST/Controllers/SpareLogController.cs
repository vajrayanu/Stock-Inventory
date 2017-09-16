using ITST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITST.Controllers
{
    public class SpareLogController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();
        //
        // GET: /SpareLog/
        public ActionResult Index()
        {
            var recordspare = db.RecordSpares.OrderByDescending(r=>r.DateRequest);
            return View(recordspare.ToList());
        }
	}
}