using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace CefDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            InitializeCefSharp();
        }

        /// <summary>
        /// 初始化CefSharp
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]  //禁止方法在JIT内联优化
        private void InitializeCefSharp()
        {
            var settings = new CefSettings();
            //设置CefSharp.BrowserSubprocess.exe地址
            settings.BrowserSubprocessPath = Path.Combine(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                Environment.Is64BitProcess ? @"runtimes\win-x64\native" : @"runtimes\win-x86\native",
                "CefSharp.BrowserSubprocess.exe"
            );
            settings.Locale = "zh-CN";
            settings.CachePath = Directory.GetCurrentDirectory() + @"\Cache";
            settings.MultiThreadedMessageLoop = true;
            settings.CefCommandLineArgs.Add("proxy-auto-detect", "0");
            settings.CefCommandLineArgs.Add("--disable-web-security", "");
            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            settings.CefCommandLineArgs.Add("disable-gpu-vsync");
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");

            Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
        }
        /// <summary>
        /// 程序集加载失败回调，添加对Any CPU地支持
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = Path.Combine(
                    AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                    Environment.Is64BitProcess ? @"runtimes\win-x64\lib" : @"runtimes\win-x86\lib",
                    assemblyName
                );
                return File.Exists(archSpecificPath)
                    ? Assembly.LoadFile(archSpecificPath)
                    : null;
            }

            return null;
        }
    }
}
