using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Zebra_RFID_Scanner
{
    public class Encode
    {
        public const string Mac = "74710984af7613edd1e783e52149c636";//6
        public static string ToMD5(string str)
        {
            string result = "";
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            buffer = md5.ComputeHash(buffer);
            for (int i = 0; i < buffer.Length; i++)
            {
                result += buffer[i].ToString("x2");
            }
            return result;
        }
        public static string EPC(string str)
        {
            Random res = new Random();
            String strs = "abcdefghijklmnopqrstuvwxyz";
            int size = 2;
            String ran = "";
            for (int i = 0; i < size; i++)
            {
                int x = res.Next(26);
                ran = ran + strs[x];
            }
            int strlength = str.Length;
            int startno = strlength - 6;
            int startba = strlength - 11;
            string idepcsno = "";
            string idepcsba = "";
            string idepcs = "";
            if (strlength > 11)
            {
                idepcsno = str.Substring(startno, 6);
                idepcsba = str.Substring(startba, 3);
                idepcs = idepcsba + idepcsno + ran;
                byte[] bytes = Encoding.Default.GetBytes(idepcs);
                string idepc = BitConverter.ToString(bytes);
                idepc = idepc.Replace("-", "");
                return idepc + "EE";
            }
            else
            {
                idepcsno = str;
                byte[] bytes = Encoding.Default.GetBytes(idepcsno);
                string idepc = BitConverter.ToString(bytes);
                idepc = idepc.Replace("-", "");
                var idpeclength = idepc.Length;
                if (idpeclength < 22)
                {
                    var add = 22 - idpeclength;
                    for (int i = 0; i < add; i++)
                    {
                        idepc += "0";
                    }
                }
                return idepc + "EE";
            }
        }

    }
}