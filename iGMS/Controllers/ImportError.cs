using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;

namespace CollTex.Controllers
{
    public class ImportError
    {
        ResourceManager rm = new ResourceManager("CollTex.Resources.Resource", typeof(Resources.Resource).Assembly);
        private string errorMessage = "";
        private int line = 0;

        public ImportError(string errorMessage, int line)
        {
            ErrorMessage = errorMessage;
            Line = line;
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }
        public int Line
        {
            get { return line; }
            set { line = value; }
        }
        public override string ToString()
        {
            return  errorMessage +" "+ rm.GetString("atline").ToString() +" "+ line;
        }
    }
}