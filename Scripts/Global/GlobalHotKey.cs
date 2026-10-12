using System.Runtime.InteropServices;
using System;

namespace StepanPlayer.Global
{
    public static class GlobalHotKey
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
        [StructLayout(LayoutKind.Sequential)]
        private struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public int ptX;
            public int ptY;
        }

        private static uint WM_HOTKEY = 0x0312; // Código da mensagem de tecla de atalho
        private static IntPtr hWnd = IntPtr.Zero; // Handle da janela
        private static int keys = 1; // Contador de teclas registradas


        public static void LaunchKeysStepanPlayer()
        {
            RegisterKey(Virtual_Keys.F7);
            RegisterKey(Virtual_Keys.F8);
            RegisterKey(Virtual_Keys.F9);

            while(GetMessage(out MSG msg, IntPtr.Zero, 0, 0))
            {
                if (msg.message == WM_HOTKEY) // WM_HOTKEY
                {
                    int id = msg.wParam.ToInt32();
                    switch (id)
                    {
                        case 1:
                            TasksQueue.Queue.Enqueue(() => Program.GlobalHotKeyHandler(Virtual_Keys.F7));
                            break;
                        case 2:
                            TasksQueue.Queue.Enqueue(() => Program.GlobalHotKeyHandler(Virtual_Keys.F8));
                            break;
                        case 3:
                            TasksQueue.Queue.Enqueue(() => Program.GlobalHotKeyHandler(Virtual_Keys.F9));
                            break;
                    }
                }
            }
        }

        public static void RegisterKey(Virtual_Keys vk, Virtual_Keys fsModifiers = 0x000)
        {
            if (!RegisterHotKey(hWnd, keys, (uint)fsModifiers, (uint)vk))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
            keys++;
        }
        


        public static void UnregisterKeys()
        {
            for (int i = 0; i < keys; i++)
            {
                UnregisterHotKey(hWnd, i);
            }
        }
    }
}
