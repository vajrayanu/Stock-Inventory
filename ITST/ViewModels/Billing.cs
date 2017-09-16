using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ITST.ViewModels
{
    public class Billing
    {
        private static int ID = 0;
        private int myId = 0;

        public int BillReceiptID { get; set; }

        [Required]
        public string Type { get; set; }

        public string BillReceiptNo { get; set; }

        [Required]
        public string CompanyName { get; set; }
        public Nullable<int> IsPrint { get; set; }
        public int number
        {
            get { return myId; }
        }
        public Nullable<System.DateTime> DateCreate { get; set; }
        public Nullable<System.DateTime> DateUpdate { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }

        public Billing()
        {
            ID++;
            this.myId = ID;
        }
    }
}