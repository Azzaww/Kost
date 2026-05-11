using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace Kost_SiguraGura
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Setup assembly resolver untuk help load DLL dependencies
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string assemblyName = new AssemblyName(args.Name).Name;
                string assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyName + ".dll");

                if (File.Exists(assemblyPath))
                {
                    return Assembly.LoadFrom(assemblyPath);
                }

                return null;
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ✅ NEW: Initialize API connection dengan smart failover
            // Cek koneksi ke Production, jika gagal fallback ke Localhost
            InitializeApiConnection();

            Application.Run(new Form1());
        }

        /// <summary>
        /// ✅ NEW: Initialize API connection on startup
        /// Handles server connectivity check and automatic failover
        /// </summary>
        private static async void InitializeApiConnection()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🔄 Initializing API connection...");
                await ApiClient.InitializeConnection();
                System.Diagnostics.Debug.WriteLine($"✅ API connection initialized. Active URL: {ApiClient.ActiveBaseUrl}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error during API initialization: {ex.Message}");
                // Tetap lanjutkan aplikasi meskipun ada error, akan retry di login
            }
        }
    }
}
