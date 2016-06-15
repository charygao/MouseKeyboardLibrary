using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MouseKeyboardLibrary
{
    /// <summary>
    /// 多数程序使用的标准键盘快捷键
    /// Standard Keyboard Shortcuts used by most applications
    /// </summary>
    public enum StandardShortcut
    {
        Copy,
        Cut,
        Paste,
        SelectAll,
        Save,
        Open,
        New,
        Close,
        Print
    }

    /// <summary>
    /// 模拟按键press动作
    /// Simulate keyboard key presses
    /// </summary>
    public static class KeyboardSimulator
    {
        #region Windows API Code 底层API接口

        private const int KEYEVENTF_EXTENDEDKEY = 0x1;
        private const int KEYEVENTF_KEYUP = 0x2;
        /// <summary>
        /// 触发一个按键事件，也就是说会产生一个WM_KEYDOWN或WM_KEYUP消息
        /// </summary>
        /// <param name="key">按键的虚拟键值</param>
        /// <param name="scan">扫描码，一般不用设置，用0代替就行</param>
        /// <param name="flags">选项标志，如果为keydown则置0即可，如果为keyup则设成"KEYEVENTF_KEYUP"</param>
        /// <param name="extraInfo">一般也是置0即可</param>
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte key, byte scan, int flags, int extraInfo);

        #endregion Windows API Code

        #region Methods
        /// <summary>
        /// 模拟按下key键
        /// </summary>
        /// <param name="key">要模拟的KEY</param>
        public static void KeyDown(Keys key)
        {
            keybd_event(ParseKey(key), 0, 0, 0);
        }
        /// <summary>
        /// 模拟抬起key键
        /// </summary>
        /// <param name="key">要模拟的KEY</param>
        public static void KeyUp(Keys key)
        {
            keybd_event(ParseKey(key), 0, KEYEVENTF_KEYUP, 0);
        }
        /// <summary>
        /// 模拟press key键=按下，抬起
        /// </summary>
        /// <param name="key">要模拟的KEY</param>
        public static void KeyPress(Keys key)
        {
            KeyDown(key);
            KeyUp(key);
        }
        /// <summary>
        /// 模拟系统常用快捷键
        /// </summary>
        /// <param name="shortcut">快捷键操作</param>
        public static void SimulateStandardShortcut(StandardShortcut shortcut)
        {
            switch (shortcut)
            {
                case StandardShortcut.Copy://=Ctrl+C
                    KeyDown(Keys.Control);
                    KeyPress(Keys.C);
                    KeyUp(Keys.Control);
                    break;

                case StandardShortcut.Cut://=Ctrl+X
                    KeyDown(Keys.Control);
                    KeyPress(Keys.X);
                    KeyUp(Keys.Control);
                    break;

                case StandardShortcut.Paste://=Ctrl+V
                    KeyDown(Keys.Control);
                    KeyPress(Keys.V);
                    KeyUp(Keys.Control);
                    break;

                case StandardShortcut.SelectAll://=Ctrl+A
                    KeyDown(Keys.Control);
                    KeyPress(Keys.A);
                    KeyUp(Keys.Control);
                    break;

                case StandardShortcut.Save://=Ctrl+S
                    KeyDown(Keys.Control);
                    KeyPress(Keys.S);
                    KeyUp(Keys.Control);
                    break;

                case StandardShortcut.Open://=Ctrl+O
                    KeyDown(Keys.Control);
                    KeyPress(Keys.O);
                    KeyUp(Keys.Control);
                    break;

                case StandardShortcut.New://=Ctrl+N
                    KeyDown(Keys.Control);
                    KeyPress(Keys.N);
                    KeyUp(Keys.Control);
                    break;

                case StandardShortcut.Close://=Alt+F4
                    KeyDown(Keys.Alt);
                    KeyPress(Keys.F4);
                    KeyUp(Keys.Alt);
                    break;

                case StandardShortcut.Print://=Ctrl+P
                    KeyDown(Keys.Control);
                    KeyPress(Keys.P);
                    KeyUp(Keys.Control);
                    break;
            }
        }
        /// <summary>
        /// 功能键特殊对待
        /// </summary>
        /// <param name="key">按键key</param>
        /// <returns>按键的返回值</returns>
        private static byte ParseKey(Keys key)
        {
            // Alt, Shift, 和Ctrl 功能键，要特殊对待
            // Alt, Shift, and Control need to be changed for API function to work with them
            switch (key)
            {
                case Keys.Alt:
                    return (byte)18;

                case Keys.Control:
                    return (byte)17;

                case Keys.Shift:
                    return (byte)16;

                default:
                    return (byte)key;
            }
        }

        #endregion Methods
    }
}

