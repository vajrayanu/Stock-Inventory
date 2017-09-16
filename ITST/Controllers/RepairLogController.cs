using ITST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITST.Controllers
{
    public class RepairLogController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();
        //
        // GET: /RepairLog/
        public ActionResult Index()
        {
            var recordrepaire = db.RecordInRepairs.OrderByDescending(r=>r.DateRequest);
            return View(recordrepaire.ToList());
        }
	}
}