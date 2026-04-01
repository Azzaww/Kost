using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kost_SiguraGura
{
    public static class ApiClient
    {
        // Container ini yang menyimpan 'izin' login dari server
        private static readonly CookieContainer cookieContainer = new CookieContainer();
        private static readonly HttpClientHandler handler = new HttpClientHandler() { CookieContainer = cookieContainer };

        // Gunakan satu Client ini untuk SEMUA Form dan UserControl
        public static readonly HttpClient Client = new HttpClient(handler);
    }
}
