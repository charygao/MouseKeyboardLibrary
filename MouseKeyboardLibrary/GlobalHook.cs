using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MouseKeyboardLibrary
{
    /// <summary>
    /// Abstract base class for Mouse and Keyboard hooks
    /// 鼠标和键盘钩子的抽象类
    /// </summary>
    public abstract class GlobalHook
    {
        #region Windows API Code 底层API部分，调入非托管代码

        [StructLayout(LayoutKind.Sequential)]
        protected class Point
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected class MouseHookStruct
        {
            public Point pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected class MouseLlHookStruct
        {
            public Point pt;
            public int mouseData;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected class KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        /// <summary>
        /// 安装钩子
        /// 应用程序可以在上面设置子程以监视指定窗口的某种消息，而且所监视的窗口可以是其他进程所创建的。当消息到达后，在目标窗口处理函数之前处理它。
        /// </summary>
        /// <param name="idHook">钩子类型,全局钩子和局部钩子</param>
        /// <param name="lpfn">回调函数地址,当抓取到消息时，操作系统将自动调用该函数处理消息。</param>
        /// <param name="hMod">实例句柄,对于线程序钩子，参数传NULL；对于系统钩子：参数为钩子DLL的句柄。</param>
        /// <param name="dwThreadId">线程ID,对于全局钩子，该参数为NULL</param>
        /// <returns>若此函数执行成功,则返回值就是该挂钩处理过程的句柄;若此函数执行失败,则返回值为NULL(0).若想获得更多错误信息,请调用GetLastError函数.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        protected static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);
        /// <summary>
        /// 卸载钩子,使用完钩子后一定要卸载钩子，否则可能会导致BUG，甚至导致死机。
        /// </summary>
        /// <param name="idHook">要卸载的钩子句柄</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        protected static extern int UnhookWindowsHookEx(int idHook);
        /// <summary>
        /// 继续下一个钩子
        /// </summary>
        /// <param name="idHook">钩子类型,全局钩子和局部钩子</param>
        /// <param name="nCode">指定是否需要处理该消息</param>
        /// <param name="wParam">附加消息wParam</param>
        /// <param name="lParam">附加消息lParam</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        protected static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);
        /// <summary>
        /// 将虚拟键码转化为相应的字符或字符串
        /// </summary>
        /// <param name="uVirtKey">指定要翻译的虚拟键码</param>
        /// <param name="uScanCode">定义被翻译键的硬件扫描码。若该键处于up状态，则该值的最高位被设置。</param>
        /// <param name="lpbKeyState">指向包含当前键盘状态的一个256字节数组。数组的每个成员包含一个键的状态。若某字节的最高位被设置，则该键处于down状态。若最低位被设置，则表明该键被触发。在此函数中，仅有capslock键的触发位是相关的。NumloCk和scroll loCk键的触发状态将被忽略。</param>
        /// <param name="lpwTransKey">指向接受翻译所得字符或字符串的缓冲区。</param>
        /// <param name="fuState">定义一个菜单是否处于激活状态。若一菜单是活动的，则该参数为1，否则为0。</param>
        /// <returns>若定义的键为死键，则返回值为负值。否则，O：对于当前键盘状态，所定义的虚拟键没有翻译。1：一个字符被拷贝到缓冲区。  2：两个字符被拷贝到缓冲区。当一个存储在键盘布局中的死键（重音或双音字符）无法与所定义的虚拟键形成一个单字符时，通常会返回该值。  </returns>
        [DllImport("user32")]
        protected static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);
        /// <summary>
        /// 检取所有虚拟键的当前状态。
        /// </summary>
        /// <param name="pbKeyState">指向一个256字节的数组，数组用于接收每个虚拟键的状态。</param>
        /// <returns>若函数调用成功，则返回非0值。若函数调用不成功，则返回值为0。</returns>
        [DllImport("user32")]
        protected static extern int GetKeyboardState(byte[] pbKeyState);
        /// <summary>
        /// 检取指定虚拟键的状态。该状态指定此键是UP状态，DOWN状态，还是被触发的（开关每次按下此键时进行切换）。
        /// </summary>
        /// <param name="vKey">定义一虚拟键</param>
        /// <returns>大于 0 没按下,小于0被按下</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        protected static extern short GetKeyState(int vKey);
        /// <summary>
        /// 钩子处理程序
        /// </summary>
        /// <param name="nCode">指定是否需要处理该消息</param>
        /// <param name="wParam">附加消息wParam</param>
        /// <param name="lParam">附加消息lParam</param>
        /// <returns></returns>
        protected delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        protected const int WH_MOUSE_LL = 14;
        protected const int WH_KEYBOARD_LL = 13;

        protected const int WH_MOUSE = 7;
        protected const int WH_KEYBOARD = 2;
        protected const int WM_MOUSEMOVE = 0x200;
        protected const int WM_LBUTTONDOWN = 0x201;
        protected const int WM_RBUTTONDOWN = 0x204;
        protected const int WM_MBUTTONDOWN = 0x207;
        protected const int WM_LBUTTONUP = 0x202;
        protected const int WM_RBUTTONUP = 0x205;
        protected const int WM_MBUTTONUP = 0x208;
        protected const int WM_LBUTTONDBLCLK = 0x203;
        protected const int WM_RBUTTONDBLCLK = 0x206;
        protected const int WM_MBUTTONDBLCLK = 0x209;
        protected const int WM_MOUSEWHEEL = 0x020A;
        protected const int WM_KEYDOWN = 0x100;
        protected const int WM_KEYUP = 0x101;
        protected const int WM_SYSKEYDOWN = 0x104;
        protected const int WM_SYSKEYUP = 0x105;

        protected const byte VK_SHIFT = 0x10;
        protected const byte VK_CAPITAL = 0x14;
        protected const byte VK_NUMLOCK = 0x90;

        protected const byte VK_LSHIFT = 0xA0;
        protected const byte VK_RSHIFT = 0xA1;
        protected const byte VK_LCONTROL = 0xA2;
        protected const byte VK_RCONTROL = 0x3;
        protected const byte VK_LALT = 0xA4;
        protected const byte VK_RALT = 0xA5;

        protected const byte LLKHF_ALTDOWN = 0x20;

        #endregion Windows API Code

        #region Private Variables 私有变量

        protected int _hookType;
        protected int _handleToHook;
        protected bool _isStarted;
        protected HookProc _hookCallback;

        #endregion Private Variables 私有变量

        #region Properties 属性

        /// <summary>
        /// 获取是否已经开始-只读
        /// </summary>
        public bool IsStarted
        {
            get
            {
                return _isStarted;
            }
        }

        #endregion Properties

        #region Constructor 构造函数

        public GlobalHook()
        {
            Application.ApplicationExit += Application_ApplicationExit;//退出程序时，如果在运行，则停止
        }

        #endregion Constructor

        #region Methods 方法
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            if (!_isStarted &&
                _hookType != 0)
            {   //确保引用都有委托，否则，GC可能会回收引用，然后抛出异常
                // Make sure we keep a reference to this delegate!
                // If not, GC randomly collects it, and a NullReference exception is thrown
                _hookCallback = HookCallbackProcedure;

                _handleToHook = SetWindowsHookEx(
                    _hookType,
                    _hookCallback,
                    Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),
                    0);
                //我们是否成功的启动了钩子？
                // Were we able to sucessfully start hook?
                if (_handleToHook != 0)
                {
                    _isStarted = true;
                }
            }
        }
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (_isStarted)
            {
                UnhookWindowsHookEx(_handleToHook);//卸载钩子

                _isStarted = false;
            }
        }
        /// <summary>
        /// 被子类重写以实现各种Hooks
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        protected virtual int HookCallbackProcedure(int nCode, Int32 wParam, IntPtr lParam)
        {
            // 被子类重写以实现各种Hooks
            return 0;
        }
        /// <summary>
        /// 程序退出时要停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (_isStarted)
            {
                Stop();
            }
        }

        #endregion Methods
    }
}