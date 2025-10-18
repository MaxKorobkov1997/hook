using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hook
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int showCmds);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public const int MOD_ALT = 0x1;
        public const int MOD_CONTROL = 0x2;
        public const int MOD_SHIFT = 0x4;
        public const int MOD_WIN = 0x8;
        public const int WM_HOTKEY = 0x312;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                ShowWindow(Handle, 1);
                UnregisterHotKey(Handle, 1);
                m.Result = (IntPtr)0;
                return;
            }
            base.WndProc(ref m);
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool res = RegisterHotKey(this.Handle, 1, MOD_SHIFT, (uint)Keys.Z);
            if (res == false) MessageBox.Show("as");
            ShowWindow(Handle, 0);
        }
    }
}
