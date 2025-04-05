using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hook
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Hook.StartHook();
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox1.Text = Hook.s;
        }

        static class Hook
        {
            [DllImport("user32.dll")]
            private static extern IntPtr SetWindowsHookEx(int idHook, LowLevel_KeyboardProc Ipfn, IntPtr hMod, uint dwThreadId);
            //IntPtr- целочисленный указатель на функцию, которая будет обработывать пользовательские хуки
            [DllImport("user32.dll")]
            private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool UnhookWindowsHookEx(IntPtr hhk);

            private static IntPtr _hookId = IntPtr.Zero;
            private delegate IntPtr LowLevel_KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
            private static LowLevel_KeyboardProc _proc = HookCallback;
            private const int WH_KEYBOARD_LL = 13;
            private const int WH_KEYDOWN = 0x0100;
            private const int WH_ALT = 0x0105;

            [DllImport("Kernel32.dll")]
            private static extern IntPtr GetModuleHandle(string lpModuleName);

            public static string s;

            private static IntPtr SetHook(LowLevel_KeyboardProc proc)
            {
                Process pr = Process.GetCurrentProcess();
                ProcessModule prM = pr.MainModule;
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(prM.ModuleName), 0);
            }
            private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
            {
                if (nCode >= 0 && wParam == (IntPtr)WH_ALT)
                {
                    int vkCode = Marshal.ReadInt32(lParam);


                    s += (Keys)vkCode;


                }


                return CallNextHookEx(_hookId, nCode, wParam, lParam);
            }
            public static void StartHook()
            {
                _hookId = SetHook(_proc);
            }
            public static void StopHook()
            {
                UnhookWindowsHookEx(_hookId);
            }
        }
    }
}
