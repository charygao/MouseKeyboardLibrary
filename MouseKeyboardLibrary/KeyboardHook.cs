using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MouseKeyboardLibrary
{
    /// <summary>
    /// 捕获全局的键盘事件
    /// Captures global keyboard events
    /// </summary>
    public class KeyboardHook : GlobalHook
    {
        #region Events 事件

        public event KeyEventHandler KeyDown;

        public event KeyEventHandler KeyUp;

        public event KeyPressEventHandler KeyPress;

        #endregion Events

        #region Constructor 构造函数

        public KeyboardHook()
        {
            _hookType = WH_KEYBOARD_LL;//钩子类型=键盘
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
            bool isHandled = false;

            if (nCode > -1 && (KeyDown != null || KeyUp != null || KeyPress != null))
            {
                KeyboardHookStruct keyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));

                // Is Control being held down? 是否按下了Ctrl键（不分左右）
                bool control = ((GetKeyState(VK_LCONTROL) & 0x80) != 0) || ((GetKeyState(VK_RCONTROL) & 0x80) != 0);

                // Is Shift being held down? 是否按下了Shift键（不分左右）
                bool shift = ((GetKeyState(VK_LSHIFT) & 0x80) != 0) || ((GetKeyState(VK_RSHIFT) & 0x80) != 0);

                // Is Alt being held down? 是否按下了Alt键（不分左右）
                bool alt = ((GetKeyState(VK_LALT) & 0x80) != 0) || ((GetKeyState(VK_RALT) & 0x80) != 0);

                // Is CapsLock on? 是否开启了大写锁定
                bool capslock = (GetKeyState(VK_CAPITAL) != 0);

                // Create event using keycode and control/shift/alt values found above 使用Keycode+功能键创建事件
                KeyEventArgs e = new KeyEventArgs((Keys)(keyboardHookStruct.vkCode | (control ? (int)Keys.Control : 0) | (shift ? (int)Keys.Shift : 0) | (alt ? (int)Keys.Alt : 0)));

                // Handle KeyDown and KeyUp events 键盘按下弹起事件句柄
                switch (wParam)
                {
                    case WM_KEYDOWN:
                    case WM_SYSKEYDOWN:
                        if (KeyDown != null)
                        {
                            KeyDown(this, e);
                            isHandled = isHandled || e.Handled;
                        }
                        break;

                    case WM_KEYUP:
                    case WM_SYSKEYUP:
                        if (KeyUp != null)
                        {
                            KeyUp(this, e);
                            isHandled = isHandled || e.Handled;
                        }
                        break;
                }

                // Handle KeyPress event 按键事件句柄
                if (wParam == WM_KEYDOWN && !isHandled && !e.SuppressKeyPress && KeyPress != null)
                {
                    byte[] keyState = new byte[256];//所有按键状态
                    byte[] inBuffer = new byte[2];
                    GetKeyboardState(keyState);

                    if (ToAscii(keyboardHookStruct.vkCode, keyboardHookStruct.scanCode, keyState, inBuffer, keyboardHookStruct.flags) == 1)
                    {
                        char key = (char)inBuffer[0];
                        if ((capslock ^ shift) && Char.IsLetter(key))//如果开了大写键盘，或者按下了Shift键，则字母按键转大写
                        { key = Char.ToUpper(key); }
                        KeyPressEventArgs e2 = new KeyPressEventArgs(key);
                        KeyPress(this, e2);
                        isHandled = isHandled || e.Handled;
                    }
                }
            }

            if (isHandled)//处理了该事件返回1
            {
                return 1;
            }
            else
            {
                return CallNextHookEx(_handleToHook, nCode, wParam, lParam);
            }
        }

        #endregion Methods
    }
}