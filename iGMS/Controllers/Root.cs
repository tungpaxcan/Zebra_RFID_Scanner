using System.Collections.Generic;

namespace CollTex.Controllers
{
    public class Data
    {
        public string USER { get; set; }
        public int antenna { get; set; }
        public int eventNum { get; set; }
        public string format { get; set; }
        public string idHex { get; set; }
        public int peakRssi { get; set; }
        public int reads { get; set; }
    }

    public class Root
    {
        public Data data { get; set; }
        public string timestamp { get; set; }
        public string type { get; set; }
    }




}