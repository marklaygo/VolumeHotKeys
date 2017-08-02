using System;
using System.Runtime.InteropServices;

namespace VolumeHotKeys.Library
{
    static class NativeMethods
    {
        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x0319;
        private const int WM_CHANGEUISTATE = 0x127;
        private const int UIS_SET = 1;
        private const int UISF_HIDEFOCUS = 0x1;

        /// <summary>
        /// Register global hotkey
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        /// <param name="id">hotkey id</param>
        /// <param name="fsModifiers">Key modifiers Alt, Control, Shift, Windows</param>
        /// <param name="vk">Virtual key</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        /// <summary>
        /// Unregister global hotkey
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        /// <param name="id">Hotkey id</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        /// <param name="msg">The message to be sent. System-Defined Messages</param>
        /// <param name="wParam">Additional message-specific information</param>
        /// <param name="lParam">Additional message-specific information</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Increase the volume
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        public static void VolumeUp(IntPtr hWnd)
        {
            SendMessageW(hWnd, WM_APPCOMMAND, hWnd, (IntPtr)APPCOMMAND_VOLUME_UP);
        }

        /// <summary>
        /// Decrease the volume
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        public static void VolumeDown(IntPtr hWnd)
        {
            SendMessageW(hWnd, WM_APPCOMMAND, hWnd, (IntPtr)APPCOMMAND_VOLUME_DOWN);
        }

        /// <summary>
        /// Mute the volume
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        public static void VolumeMute(IntPtr hWnd)
        {
            SendMessageW(hWnd, WM_APPCOMMAND, hWnd, (IntPtr)APPCOMMAND_VOLUME_MUTE);
        }

        /// <summary>
        /// Remove the dotted border on focus
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        public static void HideFocusState(IntPtr hWnd)
        {
            SendMessageW(hWnd, WM_CHANGEUISTATE, (IntPtr)MakeLong(UIS_SET, UISF_HIDEFOCUS), (IntPtr)0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wLow"></param>
        /// <param name="wHigh"></param>
        /// <returns></returns>
        private static int MakeLong(int wLow, int wHigh)
        {
            int low = (int)IntLoWord(wLow);
            short high = IntLoWord(wHigh);
            int product = 0x10000 * (int)high;
            int mkLong = (int)(low | product);

            return mkLong;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private static short IntLoWord(int word)
        {
            return (short)(word & short.MaxValue);
        }

        /// <summary>
        /// Key modifiers
        /// </summary>
        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            Win = 8
        }
    }
}
