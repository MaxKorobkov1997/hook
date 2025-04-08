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
            textBox1.Text += Hook.s;
            Hook.s = "";
        }

        static class Hook
        {
            private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
            private static IntPtr hookHandle = IntPtr.Zero;
            private static HookProc hookProcedure;

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern bool UnhookWindowsHookEx(IntPtr hhk);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

            private const int WH_KEYBOARD_LL = 13;
            private const int WM_KEYDOWN = 0x0100; //на любую клавищу
            private const int WM_SYSKEYDOWN = 0x0104; //при зажатой alt (вывод с alt)
            private const int WM_ALT = 0x0105; //при нажатой (вывод без alt)

            public static void StartHook()
            {
                hookProcedure = HookCallback;
                using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    hookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, hookProcedure, IntPtr.Zero, 0);
                }
            }

            public static void StopHook()
            {
                UnhookWindowsHookEx(hookHandle);
            }
            public static string s;

            private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
            {
                if (nCode >= 0 && wParam == (IntPtr)WM_SYSKEYDOWN)
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    s += (Keys)vkCode;
                }
                return CallNextHookEx(hookHandle, nCode, wParam, lParam);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hook.s = "";
        }
    }
}
