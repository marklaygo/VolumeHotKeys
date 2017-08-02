using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows.Forms;

namespace VolumeHotKeys.Library
{
    static class Helper
    {
        /// <summary>
        /// Parse string hotkey to KeyModifiers and Virtual Key
        /// 
        /// c# 7.0 tuples is better, but is not available on .net 4.5
        /// without nuget package
        /// 
        /// NOTE: Update to System.ValueTuple
        /// </summary>
        /// <param name="hotkey">String key to parse</param>
        /// <returns></returns>
        public static Tuple<NativeMethods.KeyModifiers, Keys> ParseHotkey(string hotkey)
        {
            NativeMethods.KeyModifiers modifier = NativeMethods.KeyModifiers.None;
            Keys key = Keys.None;

            var stringKeys = hotkey.Split(new string[] { " + " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var stringKey in stringKeys)
            {
                if (stringKey == Keys.Control.ToString() || stringKey == "Ctrl")
                {
                    modifier |= NativeMethods.KeyModifiers.Control;
                }
                else if (stringKey == Keys.Shift.ToString())
                {
                    modifier |= NativeMethods.KeyModifiers.Shift;
                }
                else if (stringKey == Keys.Alt.ToString())
                {
                    modifier |= NativeMethods.KeyModifiers.Alt;
                }
                else if (stringKey == Keys.LWin.ToString())
                {
                    modifier |= NativeMethods.KeyModifiers.Win;
                }
            }

            key = (Keys)new KeysConverter().ConvertFromString(stringKeys.Last());

            return Tuple.Create(modifier, key);
        }

        /// <summary>
        /// Register application on startup
        /// </summary>
        /// <param name="register">Register</param>
        public static void RegisterAtStartUp(bool register)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (register)
            {
                rk.SetValue(Application.ProductName, Application.ExecutablePath.ToString());
            }
            else
            {
                rk.DeleteValue(Application.ProductName, false);
            }
        }
    }
}
