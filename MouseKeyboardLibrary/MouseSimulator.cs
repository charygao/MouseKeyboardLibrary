using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MouseKeyboardLibrary
{
    /// <summary>
    /// 可以模拟的鼠标按键
    /// Mouse buttons that can be pressed
    /// </summary>
    public enum MouseButton
    {
        Left = 0x2,
        Right = 0x8,
        Middle = 0x20
    }

    /// <summary>
    /// 模拟鼠标动作
    /// Operations that simulate mouse events
    /// </summary>
    public static class MouseSimulator
    {
        #region Windows API Code 底层API接口

        /// <summary>
        /// 显示或隐藏光标
        /// </summary>
        /// <param name="show">确定内部的显示计数器是增加还是减少，如果bShow为TRUE，则显示计数器增加1，如果bShow为FALSE，则计数器减1。</param>
        /// <returns>返回值规定新的显示计数器。</returns>
        [DllImport("user32.dll")]
        private static extern int ShowCursor(bool show);

        /// <summary>
        /// 综合鼠标击键和鼠标动作
        /// </summary>
        /// <param name="flags">标志位集，指定点击按钮和鼠标动作的多种情况。</param>
        /// <param name="dX">鼠标沿x轴的绝对位置或者从上次鼠标事件产生以来移动的数量</param>
        /// <param name="dY">鼠标沿y轴的绝对位置或者从上次鼠标事件产生以来移动的数量</param>
        /// <param name="buttons">鼠标轮移动的数量</param>
        /// <param name="extraInfo">鼠标事件相关的附加32位值。应用程序调用函数GetMessageExtraInfo来获得此附加信息。</param>
        [DllImport("user32.dll")]
        private static extern void mouse_event(int flags, int dX, int dY, int buttons, int extraInfo);

        private const int MOUSEEVENTF_MOVE = 0x1;
        private const int MOUSEEVENTF_LEFTDOWN = 0x2;
        private const int MOUSEEVENTF_LEFTUP = 0x4;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x8;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        private const int MOUSEEVENTF_MIDDLEUP = 0x40;
        private const int MOUSEEVENTF_WHEEL = 0x800;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        #endregion Windows API Code

        #region Properties 属性

        /// <summary>
        /// 获取或设置xy坐标
        /// Gets or sets a structure that represents both X and Y mouse coordinates
        /// </summary>
        public static Point Position
        {
            get
            {
                return new Point(Cursor.Position.X, Cursor.Position.Y);
            }
            set
            {
                Cursor.Position = value;
            }
        }

        /// <summary>
        /// 获取或设置x坐标
        /// Gets or sets only the mouse's x coordinate
        /// </summary>
        public static int X
        {
            get
            {
                return Cursor.Position.X;
            }
            set
            {
                Cursor.Position = new Point(value, Y);
            }
        }

        /// <summary>
        /// 获取或设置y坐标
        /// Gets or sets only the mouse's y coordinate
        /// </summary>
        public static int Y
        {
            get
            {
                return Cursor.Position.Y;
            }
            set
            {
                Cursor.Position = new Point(X, value);
            }
        }

        #endregion Properties

        #region Methods 方法

        /// <summary>
        /// 按下一个鼠标按键动作
        /// Press a mouse buttons down
        /// </summary>
        /// <param name="button">按下什么按键</param>
        public static void MouseDown(MouseButton button)
        {
            mouse_event(((int)button), 0, 0, 0, 0);
        }

        /// <summary>
        /// 区分是那个按键按下，并按下
        /// Press a mouse buttons down
        /// </summary>
        /// <param name="buttons">按键</param>
        public static void MouseDown(MouseButtons buttons)
        {
            switch (buttons)
            {
                case MouseButtons.Left:
                    MouseDown(MouseButton.Left);
                    break;

                case MouseButtons.Middle:
                    MouseDown(MouseButton.Middle);
                    break;

                case MouseButtons.Right:
                    MouseDown(MouseButton.Right);
                    break;
            }
        }

        /// <summary>
        /// 释放一个按键动作
        /// Let a mouse buttons up
        /// </summary>
        /// <param name="button">释放什么按键</param>
        public static void MouseUp(MouseButton button)
        {
            mouse_event(((int)button) * 2, 0, 0, 0, 0);
        }

        /// <summary>
        /// 区分是那个按键释放，并释放
        /// Let a mouse buttons up
        /// </summary>
        /// <param name="buttons">按键</param>
        public static void MouseUp(MouseButtons buttons)
        {
            switch (buttons)
            {
                case MouseButtons.Left:
                    MouseUp(MouseButton.Left);
                    break;

                case MouseButtons.Middle:
                    MouseUp(MouseButton.Middle);
                    break;

                case MouseButtons.Right:
                    MouseUp(MouseButton.Right);
                    break;
            }
        }

        /// <summary>
        /// 单击按键=按下+释放
        /// Click a mouse buttons (down then up)
        /// </summary>
        /// <param name="button">按键</param>
        public static void Click(MouseButton button)
        {
            MouseDown(button);
            MouseUp(button);
        }

        /// <summary>
        /// 区分哪个按键单击，并单击
        /// Click a mouse buttons (down then up)
        /// </summary>
        /// <param name="buttons">按键</param>
        public static void Click(MouseButtons buttons)
        {
            switch (buttons)
            {
                case MouseButtons.Left:
                    Click(MouseButton.Left);
                    break;

                case MouseButtons.Middle:
                    Click(MouseButton.Middle);
                    break;

                case MouseButtons.Right:
                    Click(MouseButton.Right);
                    break;
            }
        }

        /// <summary>
        /// 双击按键=单击x2
        /// Double click a mouse buttons (down then up twice)
        /// </summary>
        /// <param name="button">按键</param>
        public static void DoubleClick(MouseButton button)
        {
            Click(button);
            Click(button);
        }

        /// <summary>
        /// 区分哪个按键双击，并双击
        /// Double click a mouse buttons (down then up twice)
        /// </summary>
        /// <param name="buttons">按键</param>
        public static void DoubleClick(MouseButtons buttons)
        {
            switch (buttons)
            {
                case MouseButtons.Left:
                    DoubleClick(MouseButton.Left);
                    break;

                case MouseButtons.Middle:
                    DoubleClick(MouseButton.Middle);
                    break;

                case MouseButtons.Right:
                    DoubleClick(MouseButton.Right);
                    break;
            }
        }

        /// <summary>
        /// 滚动鼠标滚轮
        /// Roll the mouse wheel. Delta of 120 wheels up once normally, -120 wheels down once normally
        /// </summary>
        /// <param name="delta">转角度数</param>
        public static void MouseWheel(int delta)
        {
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, delta, 0);
        }

        /// <summary>
        /// 显示隐藏的鼠标在当前应用程序
        /// Show a hidden current on currently application
        /// </summary>
        public static void Show()
        {
            ShowCursor(true);
        }

        /// <summary>
        /// 隐藏鼠标在当前应用程序
        /// Hide mouse cursor only on current application's forms
        /// </summary>
        public static void Hide()
        {
            ShowCursor(false);
        }

        #endregion Methods
    }
}