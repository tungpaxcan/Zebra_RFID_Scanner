using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Zebra_RFID_Scanner.Models;

namespace Zebra_RFID_Scanner.Controllers
{
    public class Data
    {
        public int eventNum { get; set; }
        public string format { get; set; }
        public string idHex { get; set; }
        public string userDefined { get; set; }
    }

    public class Root
    {
        public Data data { get; set; }
        public DateTime timestamp { get; set; }
        public string type { get; set; }
    }

}