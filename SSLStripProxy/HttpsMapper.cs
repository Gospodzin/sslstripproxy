using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SSLStripProxy
{
    public class HttpsMapper
    {
        private static HashSet<String> https = new HashSet<string>();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void AddHttps(string uriStr)
        {
            try
            {
                Uri uri = new Uri(uriStr);
                https.Add(uri.Host + uri.AbsolutePath);
            }
            catch (Exception e)
            {
                Logger.Log(e.Message + " : " + uriStr);
            }
        }

        public static bool IsHttps(string uriStr)
        {
            Uri uri = new Uri(uriStr);
            string uriScendent = uri.Host + uri.AbsolutePath;
            return https.Contains(uriScendent);
        }
    }
}
