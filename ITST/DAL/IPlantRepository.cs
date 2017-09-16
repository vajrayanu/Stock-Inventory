using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITST.Models;

namespace ITST.DAL
{
    public interface IPlantRepository : IDisposable
    {
        IEnumerable<Plant> GetPlants();
        Plant GetPlantByID(int studentId);
        void InsertPlant(Plant plant);
        void DeletePlant(int plantID);
        void UpdatePlant(Plant plant);
        void Save();
    }
}