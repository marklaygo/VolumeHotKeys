using System;
using System.Threading;
using System.Windows.Forms;

namespace VolumeHotKeys
{
    static class Program
    {
        /// <summary>
        /// https://stackoverflow.com/a/522874
        /// </summary>
        static Mutex mutex = new Mutex(true, "{A50722A7-EE17-44F3-A024-35996C92B38D}");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main());
            }
        }
    }
}
