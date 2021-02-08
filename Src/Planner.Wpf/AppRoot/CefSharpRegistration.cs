using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using CefSharp;
using CefSharp.Wpf;

namespace Planner.Wpf.AppRoot
{
    public static class CefSharpRegistration
    {
        public static void Initialize()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            LoadApp();
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void LoadApp()
        {
            var settings = new CefSettings();

            // Set BrowserSubProcessPath based on app bitness at runtime
            settings.BrowserSubprocessPath = Path.Combine(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase??"",
                Environment.Is64BitProcess ? "x64" : "x86",
                "CefSharp.BrowserSubprocess.exe");

            // Make sure you set performDependencyCheck false
            Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
            
        }

// Will attempt to load missing assembly from either x86 or x64 subdir
        private static Assembly? Resolver(object? sender, ResolveEventArgs args)
        {
            if (args.Name?.StartsWith("CefSharp") ?? false)
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = Path.Combine(
                    AppDomain.CurrentDomain.SetupInformation.ApplicationBase ??"",
                    Environment.Is64BitProcess ? "x64" : "x86",
                    assemblyName);

                return File.Exists(archSpecificPath)
                    ? Assembly.LoadFile(archSpecificPath)
                    : null;
            }

            return null;
        }
    }
}