using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MouseKeyboardLibrary
{
    /// <summary>
    /// 捕获全局鼠标事件
    /// Captures global mouse events
    /// </summary>
    public class MouseHook : GlobalHook
    {
        #region MouseEventType Enum
        /// <summary>
        /// 鼠标事件类型
        /// </summary>
        private enum MouseEventType
        {
            None,
            MouseDown,
            MouseUp,
            DoubleClick,
            MouseWheel,
            MouseMove
        }

        #endregion MouseEventType Enum

        #region Events 事件

        public event MouseEventHandler MouseDown;

        public event MouseEventHandler MouseUp;

        public event MouseEventHandler MouseMove;

        public event MouseEventHandler MouseWheel;

        public event EventHandler Click;

        public event EventHandler DoubleClick;

        #endregion Events

        #region Constructor 构造函数

        public MouseHook()
        {
            _hookType = WH_MOUSE_LL;
        }

        #endregion Constructor

        #region Methods 方法
        /// <summary>
        /// 复写基类方法，钩子回调处理程序
        /// </summary>
        /// <param name="nCode">是否处理该事件</param>
        /// <param name="wParam">附加消息wParam</param>
        /// <param name="lParam">附加消息lParam</param>
        /// <returns>1：处理了事件</returns>
        protected override int HookCallbackProcedure(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode > -1 && (MouseDown != null || MouseUp != null || MouseMove != null))
            {
                MouseLlHookStruct mouseHookStruct = (MouseLlHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseLlHookStruct));

                MouseButtons button = GetButton(wParam);
                MouseEventType eventType = GetEventType(wParam);

                MouseEventArgs e = new MouseEventArgs(button,(eventType == MouseEventType.DoubleClick ? 2 : 1),mouseHookStruct.pt.x,mouseHookStruct.pt.y,(eventType == MouseEventType.MouseWheel ? (short)((mouseHookStruct.mouseData >> 16) & 0xffff) : 0));

                // 阻止多右键点击
                // Prevent multiple Right Click events (this probably happens for popup menus)
                if (button == MouseButtons.Right && mouseHookStruct.flags != 0){eventType = MouseEventType.None;}

                switch (eventType)
                {
                    case MouseEventType.MouseDown:
                        if (MouseDown != null){MouseDown(this, e);}
                        break;

                    case MouseEventType.MouseUp:
                        if (Click != null){Click(this, new EventArgs());}
                        if (MouseUp != null){MouseUp(this, e);}
                        break;

                    case MouseEventType.DoubleClick:
                        if (DoubleClick != null){DoubleClick(this, new EventArgs());}
                        break;

                    case MouseEventType.MouseWheel:
                        if (MouseWheel != null){MouseWheel(this, e);}
                        break;

                    case MouseEventType.MouseMove:
                        if (MouseMove != null){MouseMove(this, e);}
                        break;
                }
            }

            return CallNextHookEx(_handleToHook, nCode, wParam, lParam);
        }
        /// <summary>
        /// 获取按钮
        /// </summary>
        /// <param name="wParam"></param>
        /// <returns></returns>
        private MouseButtons GetButton(Int32 wParam)
        {
            switch (wParam)
            {
                case WM_LBUTTONDOWN:
                case WM_LBUTTONUP:
                case WM_LBUTTONDBLCLK:
                    return MouseButtons.Left;

                case WM_RBUTTONDOWN:
                case WM_RBUTTONUP:
                case WM_RBUTTONDBLCLK:
                    return MouseButtons.Right;

                case WM_MBUTTONDOWN:
                case WM_MBUTTONUP:
                case WM_MBUTTONDBLCLK:
                    return MouseButtons.Middle;

                default:
                    return MouseButtons.None;
            }
        }
        /// <summary>
        /// 获取事件类型
        /// </summary>
        /// <param name="wParam"></param>
        /// <returns></returns>
        private MouseEventType GetEventType(Int32 wParam)
        {
            switch (wParam)
            {
                case WM_LBUTTONDOWN:
                case WM_RBUTTONDOWN:
                case WM_MBUTTONDOWN:
                    return MouseEventType.MouseDown;

                case WM_LBUTTONUP:
                case WM_RBUTTONUP:
                case WM_MBUTTONUP:
                    return MouseEventType.MouseUp;

                case WM_LBUTTONDBLCLK:
                case WM_RBUTTONDBLCLK:
                case WM_MBUTTONDBLCLK:
                    return MouseEventType.DoubleClick;

                case WM_MOUSEWHEEL:
                    return MouseEventType.MouseWheel;

                case WM_MOUSEMOVE:
                    return MouseEventType.MouseMove;

                default:
                    return MouseEventType.None;
            }
        }

        #endregion Methods
    }
}