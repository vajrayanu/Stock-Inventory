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
using ITST.DAL;

namespace ITST.Controllers
{
    public class PlantController : Controller
    {
        private IPlantRepository plantRepository;
        private ITStockEntities1 db = new ITStockEntities1();

        public PlantController()
        {
            this.plantRepository = new PlantRepository(new ITStockEntities1());
        }

        public PlantController(IPlantRepository plantRepository)
        {
            this.plantRepository = plantRepository;
        }

        // GET: /Plant/
        [Authorize]
        public ActionResult Index()
        {
            return View(plantRepository.GetPlants());
        }

        // GET: /Plant/Details/5
        public ActionResult Details(int id)
        {
            Plant plant = plantRepository.GetPlantByID(id);
            return View(plant);
        }

        // GET: /Plant/Create
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Create()
        {
            Plant plant = new Plant();
            plant.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            plant.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            plant.DateUpdate = DateTime.Now;
            plant.DateCreate = DateTime.Now;
            return View(plant);
        }

        // POST: /Plant/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="PlantID,PlantName,Description,DateCreate,DateUpdate,CreateBy,UpdateBy")] Plant plant)
        {
            if (ModelState.IsValid && !db.Plants.Any(p => p.PlantName == plant.PlantName))
            {
                plantRepository.InsertPlant(plant);
                plantRepository.Save();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("PlantName", "PlantName is Duplicate");

            return View(plant);
        }

        // GET: /Plant/Edit/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Edit(int id)
        {
            Plant plant = plantRepository.GetPlantByID(id);
            return View(plant);
        }

        // POST: /Plant/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="PlantID,PlantName,Description,DateCreate,DateUpdate,CreateBy,UpdateBy")] Plant plant)
        {
            if (ModelState.IsValid && !db.Plants.Any(p => p.PlantName == plant.PlantName))
            {
                plantRepository.UpdatePlant(plant);
                plantRepository.Save();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("PlantName", "PlantName is Duplicated");
            return View(plant);
        }

        // GET: /Plant/Delete/5
        [AuthLog(Roles = "SuperUser")]
        public ActionResult Delete(bool? saveChangesError = false, int id = 0)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Plant plant = plantRepository.GetPlantByID(id);
            if (plant == null)
            {
                return HttpNotFound();
            }
            return View(plant);
        }

        // POST: /Plant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Plant plant = plantRepository.GetPlantByID(id);
            plantRepository.DeletePlant(id);
            plantRepository.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                plantRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
