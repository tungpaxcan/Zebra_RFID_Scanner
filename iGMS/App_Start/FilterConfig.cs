using System.Web;
using System.Web.Mvc;

namespace Zebra_RFID_Scanner
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
