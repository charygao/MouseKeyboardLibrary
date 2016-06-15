using MouseKeyboardLibrary;
using System;
using System.Windows.Forms;

namespace SampleApplication
{
    public partial class HookTestForm : Form
    {
        private MouseHook _mouseHook = new MouseHook();
        private KeyboardHook _keyboardHook = new KeyboardHook();

        public HookTestForm()
        {
            InitializeComponent();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            _mouseHook.MouseMove += new MouseEventHandler(mouseHook_MouseMove);
            _mouseHook.MouseDown += new MouseEventHandler(mouseHook_MouseDown);
            _mouseHook.MouseUp += new MouseEventHandler(mouseHook_MouseUp);
            _mouseHook.MouseWheel += new MouseEventHandler(mouseHook_MouseWheel);

            _keyboardHook.KeyDown += new KeyEventHandler(keyboardHook_KeyDown);
            _keyboardHook.KeyUp += new KeyEventHandler(keyboardHook_KeyUp);
            _keyboardHook.KeyPress += new KeyPressEventHandler(keyboardHook_KeyPress);

            _mouseHook.Start();
            _keyboardHook.Start();

            SetXYLabel(MouseSimulator.X, MouseSimulator.Y);
        }

        private void keyboardHook_KeyPress(object sender, KeyPressEventArgs e)
        {
            AddKeyboardEvent("KeyPress","",e.KeyChar.ToString(),"","","");
        }

        private void keyboardHook_KeyUp(object sender, KeyEventArgs e)
        {
            AddKeyboardEvent("KeyUp",e.KeyCode.ToString(),"",e.Shift.ToString(),e.Alt.ToString(),e.Control.ToString());
        }

        private void keyboardHook_KeyDown(object sender, KeyEventArgs e)
        {
            AddKeyboardEvent("KeyDown",e.KeyCode.ToString(),"",e.Shift.ToString(),e.Alt.ToString(),e.Control.ToString());
        }

        private void mouseHook_MouseWheel(object sender, MouseEventArgs e)
        {
            AddMouseEvent("MouseWheel","","","",e.Delta.ToString());
        }

        private void mouseHook_MouseUp(object sender, MouseEventArgs e)
        {
            AddMouseEvent("MouseUp",e.Button.ToString(),e.X.ToString(),e.Y.ToString(),"");
        }

        private void mouseHook_MouseDown(object sender, MouseEventArgs e)
        {
            AddMouseEvent("MouseDown",e.Button.ToString(),e.X.ToString(),e.Y.ToString(),"");
        }

        private void mouseHook_MouseMove(object sender, MouseEventArgs e)
        {
            SetXYLabel(e.X, e.Y);
        }

        private void SetXYLabel(int x, int y)
        {
            curXYLabel.Text = String.Format("Current Mouse Point: X={0}, y={1}", x, y);
        }

        private void AddMouseEvent(string eventType, string button, string x, string y, string delta)
        {
            listView1.Items.Insert(0,new ListViewItem(new string[]{eventType,button,x,y,delta}));
        }

        private void AddKeyboardEvent(string eventType, string keyCode, string keyChar, string shift, string alt, string control)
        {
            listView2.Items.Insert(0,new ListViewItem(new string[]{eventType,keyCode,keyChar,shift,alt,control}));
        }

        private void TestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Not necessary anymore, will stop when application exits

            //mouseHook.Stop();
            //keyboardHook.Stop();
        }
    }
}