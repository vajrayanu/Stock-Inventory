using ITST.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ITST.DAL
{
    public class PlantRepository : IPlantRepository, IDisposable
    {
         private ITStockEntities1 context;

        public PlantRepository(ITStockEntities1 context)
        {
            this.context = context;
        }

        public IEnumerable<Plant> GetPlants()
        {
            return context.Plants.ToList();
        }

        public Plant GetPlantByID(int id)
        {
            return context.Plants.Find(id);
        }

        public void InsertPlant(Plant plant)
        {
            context.Plants.Add(plant);
        }

        public void DeletePlant(int plantID)
        {
            Plant plant = context.Plants.Find(plantID);
            context.Plants.Remove(plant);
        }

        public void UpdatePlant(Plant plant)
        {
            context.Entry(plant).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}