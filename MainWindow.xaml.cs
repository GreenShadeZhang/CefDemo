using CefSharp;
using System;
using System.IO;
using System.Speech.Recognition;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CefDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CollapsableChromiumWebBrowser MyBrowser = null;
        private SpeechRecognitionEngine recognizer;
        private bool isFullscreen;
        public MainWindow()
        {
            InitializeComponent();
            //Browser.Load("http://ppos.top:8080"); 通过Xaml加载控件ChromiumWebBrowser比较耗性能

            InitWebBrowser();
            InitializeSpeech();
        }

        private void InitWebBrowser()
        {
            string pagepath = string.Format(@"{0}vue-dist\index.html", AppDomain.CurrentDomain.BaseDirectory);
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

            MyBrowser.JavascriptObjectRepository.Register("speechBridge", new SpeechBridge(this), options: BindingOptions.DefaultBinder);

        }

        private void InitializeSpeech()
        {
            recognizer = new SpeechRecognitionEngine();
            recognizer.LoadGrammar(new DictationGrammar());
            recognizer.SpeechRecognized += (s, e) =>
            {
                string text = e.Result.Text;
                MyBrowser.ExecuteScriptAsync($"window.updateRecognizedText('{text}')");
            };
            recognizer.SetInputToDefaultAudioDevice();
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

        private void RecieveCookie(object obj)
        {
            string cookies = (string)obj;
            //Console.WriteLine("cookies:" + cookies);
            System.Diagnostics.Debug.WriteLine("cookies:" + cookies);
            return;
        }

        public void StartRecognition()
        {
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        public void StopRecognition()
        {
            recognizer.RecognizeAsyncStop();
            string text = "我是测试文本";
            MyBrowser.ExecuteScriptAsync($"window.updateRecognizedText('{text}')");
        }

        private void CustomTitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 允许拖动窗口
            this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // 关闭窗口
            this.Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            // 最小化窗口
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            // 切换窗口最大化/还原
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            // 切换全屏/退出全屏
            ToggleFullscreen();
        }

        private void ToolsButton_Click(object sender, RoutedEventArgs e)
        {
            // 显示工具菜单或执行其他操作
            MyBrowser.ShowDevTools();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && isFullscreen)
            {
                ToggleFullscreen();
            }
        }

        private void ToggleFullscreen()
        {
            if (isFullscreen)
            {
                this.WindowState = WindowState.Normal;
                this.WindowStyle = WindowStyle.None;
                this.ResizeMode = ResizeMode.CanResize;
                this.Topmost = false;
                isFullscreen = false;
                CustomTitleBar.Visibility = Visibility.Visible;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
                this.ResizeMode = ResizeMode.NoResize;
                this.Topmost = true;
                isFullscreen = true;
                CustomTitleBar.Visibility = Visibility.Collapsed;
            }
        }
    }

    public class SpeechBridge
    {
        private readonly MainWindow _window;
        public SpeechBridge(MainWindow window) => _window = window;

        public void StartRecording() => _window.StartRecognition();
        public void StopRecording() => _window.StopRecognition();
    }
}
