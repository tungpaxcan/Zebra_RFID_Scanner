using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zebra_RFID_Scanner.Controllers
{
    public class Ctn
    {
        public class Carton
        {
            public string CartonTo { get; set; }
            public string UPC { get; set; }
            public string UPCQty { get; set; }
            public bool status { get; set; }
            public string QtyScan { get; set; }
        }
    }
}