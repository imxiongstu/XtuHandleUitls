using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XtuHandleUitls
{
    public static class XtuHandle
    {
        public const int WM_SETTEXT = 0x000C;//设置文本
        public const int WM_LBUTTONDOWN = 0x0201;//按钮按下
        public const int WM_LBUTTONUP = 0x0202;//按钮抬起
        public const int WM_CLOSE = 0x0010;//关闭
        public const int WM_GETTEXT = 0x000D;//获取文本
        public const int WM_GETTEXTLENGTH = 0x000E;//获取文本长度




        /// <summary>
        /// 查找窗口句柄
        /// </summary>
        /// <param name="lpClassName">窗口类名</param>
        /// <param name="lpWindowName">窗口标题</param>
        /// <returns>句柄值</returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// 查找子窗口句柄
        /// </summary>
        /// <param name="hwndParent">父窗口句柄</param>
        /// <param name="hwndChildAfter">子窗口句柄</param>
        /// <param name="lpClassName">子窗口类名</param>
        /// <param name="lpWindowName">子窗口标题</param>
        /// <returns>句柄值</returns>
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);

        /// <summary>
        /// 设置窗口标题
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="text">内容</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowText")]
        public static extern int SetWindowText(IntPtr hwnd, string text);

        /// <summary>
        /// 获取窗口标题
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="text">获取到的内容</param>
        /// <param name="maxLength">获取到的最大长度</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetWindowText")]
        public static extern int GetWindowText(IntPtr hwnd, StringBuilder text, int maxLength);

        /// <summary>
        /// 将窗口置顶
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <returns></returns>
        [DllImport("user32.DLL", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hwnd);

        /// <summary>
        /// 枚举窗口的子句柄
        /// </summary>
        /// <param name="hWndParent"></param>
        /// <param name="lpCallBack"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>

        [DllImport("user32.dll", EntryPoint = "EnumChildWindows")]
        public static extern int EnumChildWindows(IntPtr hWndParent, CallBack lpCallBack, int lParam);
        public delegate bool CallBack(int hwnd, int lParam);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="msg">消息类型（WS_XXX）</param>
        /// <param name="wParam">消息类型指定参数</param>
        /// <param name="lParam">消息类型指定参数</param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, int msg, int wParam, StringBuilder lParam);

    }
}
