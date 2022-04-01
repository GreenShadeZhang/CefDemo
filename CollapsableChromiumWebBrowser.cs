using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CefDemo
{
    class CollapsableChromiumWebBrowser:ChromiumWebBrowser
    {
        public CollapsableChromiumWebBrowser()
        {
            Loaded += BrowserLoaded;
        }

        private void BrowserLoaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }
            ApplyTemplate();
        }
    }
}
