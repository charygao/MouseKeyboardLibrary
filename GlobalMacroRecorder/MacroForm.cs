using MouseKeyboardLibrary;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace GlobalMacroRecorder
{
    public partial class MacroForm : Form
    {
        private List<MacroEvent> events = new List<MacroEvent>();
        private int lastTimeRecorded = 0;

        private MouseHook mouseHook = new MouseHook();
        private KeyboardHook keyboardHook = new KeyboardHook();

        public MacroForm()
        {
            InitializeComponent();

            mouseHook.MouseMove += new MouseEventHandler(mouseHook_MouseMove);
            mouseHook.MouseDown += new MouseEventHandler(mouseHook_MouseDown);
            mouseHook.MouseUp += new MouseEventHandler(mouseHook_MouseUp);

            keyboardHook.KeyDown += new KeyEventHandler(keyboardHook_KeyDown);
            keyboardHook.KeyUp += new KeyEventHandler(keyboardHook_KeyUp);
        }

        private void mouseHook_MouseMove(object sender, MouseEventArgs e)
        {
            events.Add(
                new MacroEvent(
                    MacroEventType.MouseMove,
                    e,
                    Environment.TickCount - lastTimeRecorded
                ));

            lastTimeRecorded = Environment.TickCount;
        }

        private void mouseHook_MouseDown(object sender, MouseEventArgs e)
        {
            events.Add(
                new MacroEvent(
                    MacroEventType.MouseDown,
                    e,
                    Environment.TickCount - lastTimeRecorded
                ));

            lastTimeRecorded = Environment.TickCount;
        }

        private void mouseHook_MouseUp(object sender, MouseEventArgs e)
        {
            events.Add(
                new MacroEvent(
                    MacroEventType.MouseUp,
                    e,
                    Environment.TickCount - lastTimeRecorded
                ));

            lastTimeRecorded = Environment.TickCount;
        }

        private void keyboardHook_KeyDown(object sender, KeyEventArgs e)
        {
            events.Add(
                new MacroEvent(
                    MacroEventType.KeyDown,
                    e,
                    Environment.TickCount - lastTimeRecorded
                ));

            lastTimeRecorded = Environment.TickCount;
        }

        private void keyboardHook_KeyUp(object sender, KeyEventArgs e)
        {
            events.Add(
                new MacroEvent(
                    MacroEventType.KeyUp,
                    e,
                    Environment.TickCount - lastTimeRecorded
                ));

            lastTimeRecorded = Environment.TickCount;
        }

        private void recordStartButton_Click(object sender, EventArgs e)
        {
            events.Clear();
            lastTimeRecorded = Environment.TickCount;

            keyboardHook.Start();
            mouseHook.Start();
        }

        private void recordStopButton_Click(object sender, EventArgs e)
        {
            keyboardHook.Stop();
            mouseHook.Stop();
        }

        private void playBackMacroButton_Click(object sender, EventArgs e)
        {
            foreach (MacroEvent macroEvent in events)
            {
                Thread.Sleep(macroEvent.TimeSinceLastEvent);

                switch (macroEvent.MacroEventType)
                {
                    case MacroEventType.MouseMove:
                        {
                            MouseEventArgs mouseArgs = (MouseEventArgs)macroEvent.EventArgs;

                            MouseSimulator.X = mouseArgs.X;
                            MouseSimulator.Y = mouseArgs.Y;
                        }
                        break;

                    case MacroEventType.MouseDown:
                        {
                            MouseEventArgs mouseArgs = (MouseEventArgs)macroEvent.EventArgs;

                            MouseSimulator.MouseDown(mouseArgs.Button);
                        }
                        break;

                    case MacroEventType.MouseUp:
                        {
                            MouseEventArgs mouseArgs = (MouseEventArgs)macroEvent.EventArgs;

                            MouseSimulator.MouseUp(mouseArgs.Button);
                        }
                        break;

                    case MacroEventType.KeyDown:
                        {
                            KeyEventArgs keyArgs = (KeyEventArgs)macroEvent.EventArgs;

                            KeyboardSimulator.KeyDown(keyArgs.KeyCode);
                        }
                        break;

                    case MacroEventType.KeyUp:
                        {
                            KeyEventArgs keyArgs = (KeyEventArgs)macroEvent.EventArgs;

                            KeyboardSimulator.KeyUp(keyArgs.KeyCode);
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        private void MacroForm_Load(object sender, EventArgs e)
        {
        }
    }
}