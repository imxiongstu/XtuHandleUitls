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
        /// 获取在运行的所有窗口相关句柄信息
        /// </summary>
        /// <returns></returns>
        public static List<WindowInfo> GetAllWindowList()
        {
            List<WindowInfo> windowInfoList = new List<WindowInfo>();
            StringBuilder sb = new StringBuilder(4048);
            EnumWindows((IntPtr hwnd, int lParam) =>
            {
                WindowInfo windowInfo = new WindowInfo();

                windowInfo.Hwnd = hwnd;
                GetWindowText(hwnd, sb, sb.Capacity);
                windowInfo.WindowName = sb.ToString();
                GetClassName(hwnd, sb, sb.Capacity);
                windowInfo.ClassName = sb.ToString();

                windowInfoList.Add(windowInfo);
                return true;
            }, 0);
            return windowInfoList;
        }

        /// <summary>
        /// 获取该窗口的所有子窗口句柄信息
        /// </summary>
        /// <param name="hwnd">父句柄</param>
        /// <returns></returns>
        public static List<ChildWindowInfo> GetAllChildWindowList(this WindowInfo wi)
        {
            List<ChildWindowInfo> windowInfoList = new List<ChildWindowInfo>();
            StringBuilder sb = new StringBuilder(4048);

            EnumChildWindows(wi.Hwnd, (IntPtr childHwnd, int lParam) =>
            {
                ChildWindowInfo windowInfo = new ChildWindowInfo();

                windowInfo.Hwnd = childHwnd;
                GetWindowText(childHwnd, sb, sb.Capacity);
                windowInfo.WindowName = sb.ToString();
                GetClassName(childHwnd, sb, sb.Capacity);
                windowInfo.ClassName = sb.ToString();

                windowInfoList.Add(windowInfo);
                return true;
            }, 0);
            return windowInfoList;
        }

        /// <summary>
        /// 根据参数查找获取某个运行窗口相关信息(文本类型支持模糊寻找)
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="windowName"></param>
        /// <param name="className"></param>
        /// <returns>返回找到的第一个</returns>
        public static WindowInfo FindWindowInfo(IntPtr? hwnd, string windowName, string className)
        {
            List<WindowInfo> windowInfoList = GetAllWindowList();

            if (hwnd != null && hwnd != (IntPtr)0)
            {
                if (windowInfoList.FindIndex(o => o.Hwnd == hwnd) != -1)
                    return windowInfoList.Find(o => o.Hwnd == hwnd);
            }
            else if (windowName != null)
            {
                if (windowInfoList.FindIndex(o => o.WindowName.Contains(windowName)) != -1)
                    return windowInfoList.Find(o => o.WindowName.Contains(windowName));
            }
            else if (className != null)
            {
                if (windowInfoList.FindIndex(o => o.ClassName.Contains(className)) != -1)
                    return windowInfoList.Find(o => o.ClassName.Contains(className));
            }
            return new WindowInfo();
        }

        /// <summary>
        /// 找到该父窗口下面的某个子窗口的相关信息（文本类型支持模糊寻找）
        /// </summary>
        /// <param name="wi"></param>
        /// <returns></returns>
        public static ChildWindowInfo GetChildWindowInfo(this WindowInfo wi, IntPtr? hwnd, string windowName, string className)
        {
            List<ChildWindowInfo> windowInfoList = wi.GetAllChildWindowList();

            if (hwnd != null && hwnd != (IntPtr)0)
            {
                if (windowInfoList.FindIndex(o => o.Hwnd == hwnd) != -1)
                    return windowInfoList.Find(o => o.Hwnd == hwnd);
            }
            else if (windowName != null)
            {
                if (windowInfoList.FindIndex(o => o.WindowName.Contains(windowName)) != -1)
                    return windowInfoList.Find(o => o.WindowName.Contains(windowName));
            }
            else if (className != null)
            {
                if (windowInfoList.FindIndex(o => o.ClassName.Contains(className)) != -1)
                    return windowInfoList.Find(o => o.ClassName.Contains(className));
            }
            return new ChildWindowInfo();
        }

        /// <summary>
        /// 获取窗口标题
        /// </summary>
        /// <param name="wi"></param>
        public static string GetWindowText(this WindowInfo wi)
        {
            StringBuilder sb = new StringBuilder(4048);
            GetWindowText(wi.Hwnd, sb, sb.Capacity);
            return sb.ToString();
        }
        /// <summary>
        /// 设置窗口标题
        /// </summary>
        /// <param name="wi"></param>
        public static void SetWindowText(this WindowInfo wi, string text)
        {
            SetWindowText(wi.Hwnd, text);
        }

        /// <summary>
        /// 获取该子窗口的文本内容
        /// </summary>
        /// <param name="cwi"></param>
        public static string GetContent(this ChildWindowInfo cwi)
        {
            StringBuilder sb = new StringBuilder(4048);
            SendMessage(cwi.Hwnd, WM_GETTEXT, sb.Capacity, sb);
            return sb.ToString();
        }

        /// <summary>
        /// 设置该子窗口的文本内容
        /// </summary>
        /// <param name="cwi"></param>
        /// <param name="content"></param>
        public static void SetContent(this ChildWindowInfo cwi, string content)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(content);
            SendMessage(cwi.Hwnd, WM_SETTEXT, 0, sb);
        }

        /// <summary>
        /// 模拟鼠标点击此窗口
        /// </summary>
        /// <param name="cwi"></param>
        public static void Click(this ChildWindowInfo cwi)
        {
            SetForegroundWindow(cwi.Hwnd);
            SendMessage(cwi.Hwnd, WM_LBUTTONDOWN, 0, null);
            SendMessage(cwi.Hwnd, WM_LBUTTONUP, 0, null);
        }


        /// <summary>
        /// 回调函数
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate bool CallBack(IntPtr hwnd, int lParam);
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
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hwnd);

        /// <summary>
        /// 枚举窗口的子句柄
        /// </summary>
        /// <param name="hWndParent"></param>
        /// <param name="lpCallBack"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>

        [DllImport("user32.dll", EntryPoint = "EnumChildWindows")]
        public static extern int EnumChildWindows(IntPtr hWndParent, CallBack lCallBack, int lParam);

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

        /// <summary>
        /// 获取桌面句柄
        /// </summary>
        /// <returns></returns>

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();

        /// <summary>
        /// 枚举所有窗口
        /// </summary>
        /// <param name="lCallBack"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "EnumWindows")]
        public static extern bool EnumWindows(CallBack lCallBack, int lParam);

        /// <summary>
        /// 获取窗口类名
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="text"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetClassName")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder text, int maxLength);

    }
}
