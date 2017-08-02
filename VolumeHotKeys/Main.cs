using System;
using System.Windows.Forms;
using VolumeHotKeys.Library;

namespace VolumeHotKeys
{
    public partial class Main : Form
    {
        #region Fields

        private Settings _settings;

        private bool _completelyExit;
        private bool _loaded;

        private const int WM_SYSCOMMAND = 0x0112;
        private const int WM_HOTKEY = 0x0312;
        private const int SC_MINIMIZE = 0xf020;

        #endregion

        #region Constructor

        public Main()
        {
            InitializeComponent();
        }

        #endregion

        #region Form events

        /// <summary>
        /// Override WindProc to listen for minimize and override it's default function
        /// Override WindProc to listen for hotkey
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SYSCOMMAND)
            {
                if (m.WParam.ToInt32() == SC_MINIMIZE)
                {
                    m.Result = IntPtr.Zero;
                    HideApplication();
                }
            }
            else if (m.Msg == WM_HOTKEY)
            {
                switch (m.WParam.ToInt32())
                {
                    case 0:
                        NativeMethods.VolumeUp(Handle);
                        break;
                    case 1:
                        NativeMethods.VolumeDown(Handle);
                        break;
                    case 2:
                        NativeMethods.VolumeMute(Handle);
                        break;
                }
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Override the OnHandleCreated event to register the global hotkey
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            if (_loaded)
            {
                RegisterAllHotkeys();
            }

            base.OnHandleCreated(e);
        }

        /// <summary>
        /// Form load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Load(object sender, EventArgs e)
        {
            // load settings
            try
            {
                _settings = SettingsManager.Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // ui stuffs
            if(_settings.HideUIOnAppStart == "Yes")
            {
                HideApplication();
            }
            else
            {
                NativeMethods.HideFocusState(Handle);
            }

            // Initialize ListViewItems
            InitializeListViewItems(_settings);

            // register
            RegisterAllHotkeys();
            Helper.RegisterAtStartUp(_settings.StartAtWindowsStartUp == "Yes" ? true : false);

            _loaded = true;
        }

        /// <summary>
        /// Form closing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool ignoreCompletelyExit = false;
            if (lvSettings.Items[4].SubItems[1].Text == "No")
            {
                ignoreCompletelyExit = true;
            }

            if (!_completelyExit && !ignoreCompletelyExit)
            {
                e.Cancel = true;
                HideApplication();
                return;
            }
        }

        /// <summary>
        /// Show the application when the open menu is click on tray icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowApplication();
        }

        /// <summary>
        /// Exit the application when the exit menu is click on tray icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _completelyExit = true;
            Application.Exit();
        }

        /// <summary>
        /// Show applcation on mouse double click on tray icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowApplication();
        }

        /// <summary>
        /// Show dialog box on item mouse double click
        /// and process the result
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSettings_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int selectedIndex = lvSettings.SelectedIndices[0];

            // volume up || volume down || volume mute
            if (selectedIndex == 0 || selectedIndex == 1 || selectedIndex == 2)
            {
                using (HotkeyChange hc = new HotkeyChange(lvSettings.Items[selectedIndex]))
                {
                    DialogResult dr = hc.ShowDialog(this);
                    if(dr == DialogResult.OK)
                    {
                        lvSettings.Items[selectedIndex].SubItems[1].Text = hc.KeyCombination;
                    }
                    else if(dr == DialogResult.Ignore) // no appropriate dialog result
                    {
                        lvSettings.Items[selectedIndex].SubItems[1].Text = "None";
                    }
                }
            }

            // hide ui || system start up || exit to tray
            if (selectedIndex == 3 || selectedIndex == 4 || selectedIndex == 5)
            {
                using (YesNoDialogBox ynd = new YesNoDialogBox(lvSettings.Items[selectedIndex]))
                {
                    DialogResult dr = ynd.ShowDialog(this);
                    if(dr == DialogResult.Yes)
                    {
                        lvSettings.Items[selectedIndex].SubItems[1].Text = "Yes";
                    }
                    else if(dr == DialogResult.No)
                    {
                        lvSettings.Items[selectedIndex].SubItems[1].Text = "No";
                    }
                }
            }

            // clear selected indices
            lvSettings.SelectedIndices.Clear();

            // process the hotkey / value change
            ProcessHotkeyValueChange(selectedIndex, lvSettings.Items[selectedIndex].SubItems[1].Text);
        }

        /// <summary>
        /// Prevent column resize
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSettings_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = lvSettings.Columns[e.ColumnIndex].Width;
        }

        /// <summary>
        /// Prevent list view navigation using keyboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSettings_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Show the application
        /// </summary>
        private void ShowApplication()
        {
            ShowInTaskbar = true;
            Show();
            NativeMethods.HideFocusState(Handle);

            bool topMost = TopMost;
            TopMost = true;
            TopMost = topMost;
        }

        /// <summary>
        /// Hide the application
        /// </summary>
        private void HideApplication()
        {
            Hide();
            ShowInTaskbar = false;
        }

        /// <summary>
        /// Initialize ListViewItems
        /// </summary>
        private void InitializeListViewItems(Settings settings)
        {
            ListViewItem lvi = new ListViewItem { Text = "Volume Up" };
            lvi.SubItems.Add(settings.VolumeUp);
            lvSettings.Items.Add(lvi);

            lvi = new ListViewItem { Text = "Volume Down" };
            lvi.SubItems.Add(settings.VolumeDown);
            lvSettings.Items.Add(lvi);

            lvi = new ListViewItem { Text = "Volume Mute" };
            lvi.SubItems.Add(settings.VolumeMute);
            lvSettings.Items.Add(lvi);

            lvi = new ListViewItem { Text = "Hide UI on app start" };
            lvi.SubItems.Add(settings.HideUIOnAppStart);
            lvSettings.Items.Add(lvi);

            lvi = new ListViewItem { Text = "Start at windows startup" };
            lvi.SubItems.Add(settings.StartAtWindowsStartUp);
            lvSettings.Items.Add(lvi);

            lvi = new ListViewItem { Text = "Exit to tray" };
            lvi.SubItems.Add(settings.ExitToTray);
            lvSettings.Items.Add(lvi);
        }

        /// <summary>
        /// Register all hotkeys
        /// </summary>
        private void RegisterAllHotkeys()
        {
            for (int i = 0; i < 3; i++)
            {
                var lvSubItem = lvSettings.Items[i].SubItems[1].Text;
                if (lvSubItem != "None")
                {
                    var hotkey = Helper.ParseHotkey(lvSubItem);
                    int mods = (int)hotkey.Item1;
                    int key = (int)hotkey.Item2;

                    if (!NativeMethods.RegisterHotKey(Handle, i, mods, key))
                    {
                        MessageBox.Show($"Failed to register \"{lvSubItem}\" hotkey");
                    }
                }
            }
        }

        /// <summary>
        /// Process hotkey / value change
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        private void ProcessHotkeyValueChange(int index, string value)
        {
            // Note: this switch can be simplified but im trying new stuff in c# 7.0
            switch(index)
            {
                case 0 when (value != "None"):
                    RegisterHotkey();
                    _settings.VolumeUp = value;
                    break;

                case 0 when (value == "None"):
                    UnRegisterHotkey();
                    _settings.VolumeUp = value;
                    break;

                case 1 when (value != "None"):
                    RegisterHotkey();
                    _settings.VolumeDown = value;
                    break;

                case 1 when (value == "None"):
                    UnRegisterHotkey();
                    _settings.VolumeDown = value;
                    break;

                case 2 when (value != "None"):
                    RegisterHotkey();
                    _settings.VolumeMute = value;
                    break; 

                case 2 when (value == "None"):
                    UnRegisterHotkey();
                    _settings.VolumeMute = value;
                    break;

                case 3:
                    _settings.HideUIOnAppStart = value;
                    break;

                case 4:
                    Helper.RegisterAtStartUp(value == "Yes" ? true : false);
                    _settings.StartAtWindowsStartUp = value;
                    break;

                case 5:
                    _settings.ExitToTray = value;
                    break;
            }

            try
            {
                SettingsManager.Save(_settings);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            // Local functions
            void RegisterHotkey()
            {
                var hotkey = Helper.ParseHotkey(value);
                int mods = (int)hotkey.Item1;
                int key = (int)hotkey.Item2;

                NativeMethods.UnregisterHotKey(Handle, index);
                if(!NativeMethods.RegisterHotKey(Handle, index, mods, key))
                {
                    MessageBox.Show("Failed to register this hotkey");
                    lvSettings.Items[index].SubItems[1].Text = "None";
                }
            }

            void UnRegisterHotkey()
            {
                if(!NativeMethods.UnregisterHotKey(Handle, index))
                {
                    MessageBox.Show("Failed to unregister this hotkey");
                    lvSettings.Items[index].SubItems[1].Text = "None";
                }
            }
        }

        #endregion
    }
}
