using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace hook
{
    public partial class Form1 : Form
    {
        private const int MOD_SHIFT = 0x4;
        private const int WM_HOTKEY = 0x312;
        private const int HOTKEY_ID = 1; // уникальный ID для этой горячей клавиши

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                // Обрабатываем горячую клавишу
                OnHotKeyPressed();
                // Если нужна одноразовая клавиша — раскомментируй строку ниже:
                // UnregisterHotKey(Handle, HOTKEY_ID);
                m.Result = IntPtr.Zero;
                return;
            }
            base.WndProc(ref m);
        }

        private void OnHotKeyPressed()
        {
            if (!this.Visible || this.WindowState == FormWindowState.Minimized)
            {
                // Окно было скрыто или свёрнуто — показываем
                this.Show();
                this.Visible = true;
                this.ShowInTaskbar = true;
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
            else
            {
                // Окно видимо — вывести на передний план
                this.BringToFront();
                this.Activate();
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RegisterHotKey(this.Handle, HOTKEY_ID, MOD_SHIFT, (int)Keys.Z);
        }
    }
}
