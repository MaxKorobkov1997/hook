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
            // Твоя логика при нажатии Shift+Z
            this.WindowState = FormWindowState.Normal; // разворачиваем, если было свёрнуто
            this.Activate(); // возвращаем фокус
            MessageBox.Show("Сработала горячая клавиша Shift+Z!");
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool res = RegisterHotKey(this.Handle, HOTKEY_ID, MOD_SHIFT, (uint)Keys.Z);
            if (!res)
            {
                int error = Marshal.GetLastWin32Error();
                MessageBox.Show($"Не удалось зарегистрировать горячую клавишу. Ошибка Win32: {error}");
                return;
            }

            // Сворачиваем форму после успешной регистрации
            this.WindowState = FormWindowState.Minimized;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UnregisterHotKey(Handle, HOTKEY_ID);
            base.OnFormClosing(e);
        }
    }
}
