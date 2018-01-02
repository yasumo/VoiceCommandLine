using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowHandleUtils
{
    
    public class Window
    {
        public string ClassName;
        public string Title;
        public IntPtr hWnd;
        public int Style;
    }

    public static class Operation
    {

        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;
        public const int MK_LBUTTON = 0x0001;
        public const int MK_LCLICED = 245;
        public const int WM_SETTEXT = 0x000C;
        public const int WM_GETTEXT = 0x000D;
        public const uint WM_NULL = 0;
        public static int GWL_STYLE = -16;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, uint ptr, StringBuilder lParam);
 
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hWnd, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        // ウィンドウハンドルを渡すと、ウィンドウテキスト（ラベルなど）、クラス、スタイルを取得してWindowsクラスに格納して返す
        public static Window GetWindow(IntPtr hWnd)
        {
            int textLen = GetWindowTextLength(hWnd);
            string windowText = null;
            if (0 < textLen)
            {
                //ウィンドウのタイトルを取得する
                StringBuilder windowTextBuffer = new StringBuilder(textLen + 1);
                GetWindowText(hWnd, windowTextBuffer, windowTextBuffer.Capacity);
                windowText = windowTextBuffer.ToString();
            }

            //ウィンドウのクラス名を取得する
            StringBuilder classNameBuffer = new StringBuilder(256);
            GetClassName(hWnd, classNameBuffer, classNameBuffer.Capacity);

            // スタイルを取得する
            int style = GetWindowLong(hWnd, GWL_STYLE);
            return new Window() { hWnd = hWnd, Title = windowText, ClassName = classNameBuffer.ToString(), Style = style };
        }



        // 全てのウィンドウハンドルを列挙し、最初に見つかったtitleを返す。
        public static Window FindTargetButton(Window top, string findTitle, string className,int num)
        {
            var all = GetAllChildWindows(top, new List<Window>());
            var tmpWindowArray = all.Where(x => x.Title == findTitle && x.ClassName == className);
            if (tmpWindowArray.Count() <= num)
            {
                return null;
            }
            return tmpWindowArray.Skip(num).First();

        }

        // 指定したウィンドウの全ての子孫ウィンドウを取得し、リストに追加する
        public static List<Window> GetAllChildWindows(Window parent, List<Window> dest)
        {
            dest.Add(parent);
            EnumChildWindows(parent.hWnd).ToList().ForEach(x => GetAllChildWindows(x, dest));
            return dest;
        }

        // 与えた親ウィンドウの直下にある子ウィンドウを列挙する（孫ウィンドウは見つけてくれない）
        public static IEnumerable<Window> EnumChildWindows(IntPtr hParentWindow)
        {
            IntPtr hWnd = IntPtr.Zero;
            while ((hWnd = FindWindowEx(hParentWindow, hWnd, null, null)) != IntPtr.Zero) { yield return GetWindow(hWnd); }
        }






    }


}
