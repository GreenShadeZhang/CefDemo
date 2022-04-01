using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefDemo
{
    class CookieVisitor : ICookieVisitor
    {
        private string Cookies = null;
        public event Action<object> Action;
        public void Dispose()
        {
            if (Action != null)
            {
                Action(Cookies);
            }
            return;
        }

        public bool Visit(Cookie cookie, int count, int total, ref bool deleteCookie)
        {
            if (count == 0)
            {
                Cookies = null;
            }
            Cookies += cookie.Name + "=" + cookie.Value + ";";
            deleteCookie = false;
            return true;
        }
    }
}
