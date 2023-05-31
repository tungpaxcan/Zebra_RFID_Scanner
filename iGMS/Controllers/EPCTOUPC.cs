using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zebra_RFID_Scanner.Controllers
{
    public class EPCTOUPC
    {
        public class EpctoUpc
        {
            public string EPC { get; set; }
            public string UPC { get; set; }
            public string Status { get; set; }
            public string Qty { get; set; }
        }
    }
}