using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITST.Models;

namespace ITST.ViewModels
{
    public class CombineViewModels
    {
        public IEnumerable<RecordInstock> RecordInstock { get; set; }
        public IEnumerable<RecordRequisition> RecordRequisition { get; set; }
        public IEnumerable<RecordInRepair> RecordInRepair { get; set; }
        public IEnumerable<RecordSpare> RecordSpare { get; set; }
        public IEnumerable<RecordSale> RecordSale { get; set; }
        public IEnumerable<RecordReinstock> RecordReinstock { get; set; }
    }
}