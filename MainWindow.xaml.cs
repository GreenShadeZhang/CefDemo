using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CefDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CollapsableChromiumWebBrowser MyBrowser = null;
        public MainWindow()
        {
            InitializeComponent();
            //Browser.Load("http://ppos.top:8080"); 通过Xaml加载控件ChromiumWebBrowser比较耗性能

            InitWebBrowser();
        }

        private void InitWebBrowser()
        {
            string pagepath = string.Format(@"{0}html\index.html", AppDomain.CurrentDomain.BaseDirectory);
            if (!File.Exists(pagepath))
            {
                MessageBox.Show("HTML不存在:" + pagepath);
                return;
            }
            MyBrowser = new CollapsableChromiumWebBrowser();
            MyBrowser.MenuHandler = new MenuHandler();
            MyBrowser.LifeSpanHandler = new LifeSpanHandler();
            MyBrowser.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f3f3f3"));


            ctrlBrowserGrid.Children.Add(MyBrowser);
            MyBrowser.FrameLoadEnd += Browser_FrameLoadEnd;
            //MyBrowser.Address = "https://bing.com";    //"http://ppos.top:8080";
            MyBrowser.Address = pagepath;

            //注册JS调用的方法
            MyBrowser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
            MyBrowser.JavascriptObjectRepository.Register(
                "callbackObj",
                new CallbackObjectForJs(),
                options: BindingOptions.DefaultBinder
            );
        }

        private async void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            CookieVisitor visitor = new CookieVisitor();
            string html = await MyBrowser.GetSourceAsync();
            //Console.WriteLine("html:" + html);
            System.Diagnostics.Debug.WriteLine("html:" + html);
            visitor.Action += RecieveCookie;
            Cef.GetGlobalCookieManager().VisitAllCookies(visitor);
        }

        private async void RecieveCookie(object obj)
        {
            string cookies = (string)obj;
            //Console.WriteLine("cookies:" + cookies);
            System.Diagnostics.Debug.WriteLine("cookies:" + cookies);
            return;
        }

        private void Btn_clicked(object sender, RoutedEventArgs e)
        {
            //调用JS方法
            MyBrowser.ExecuteScriptAsync("alert_msg('WPF呼叫HTML')");
        }
    }

    internal class CallbackObjectForJs
    {
        public void showMessage(string msg)
        {
            MessageBox.Show(msg);
        }
    }
}
