using ITST.CustomFilters;
using ITST.Models;
using ITST.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ITST.Controllers
{
    public class SettingController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();
        //
        // GET: /Setting/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Machine()
        {
            var machines = from machine in db.Machines group machine by machine.Plant.PlantName into dv let m = dv.FirstOrDefault() select m;
            return View(machines);
        }

        public ActionResult MachineByDepartment(int?id)
        {
            var machines = from machine in db.Machines where machine.PlantID == id group machine by machine.Department.DepartmentName into dv let m = dv.FirstOrDefault() select m;
            return View(machines);
        }

        public ActionResult MachineByLocation(int? pid, int? did)
        {
            var machines = from machine in db.Machines where machine.PlantID == pid  && machine.DepartmentID == did group machine by machine.Location.LocationName into dv let m = dv.FirstOrDefault() select m;
            return View(machines);
        }

        public ActionResult MachineByPhase(int? pid, int? did, int? lid)
        {
            var machines = from machine in db.Machines where machine.PlantID == pid && machine.DepartmentID == did && machine.LocationID == lid group machine by machine.Phase.PhaseName into dv let m = dv.FirstOrDefault() select m;
            return View(machines);
        }

        public ActionResult MachineList(int? pid, int? did, int? lid, int? phid)
        {
            var machines = from machine in db.Machines where machine.PlantID == pid && machine.DepartmentID == did && machine.LocationID == lid && machine.PhaseID == phid group machine by machine.MachineID into dv let m = dv.FirstOrDefault() select m;
            return View(machines);
        }
	}
}