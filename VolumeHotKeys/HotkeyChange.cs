using System;
using System.Drawing;
using System.Windows.Forms;

namespace VolumeHotKeys
{
    public partial class HotkeyChange : Form
    {
        #region Fields

        private ListViewItem _listViewItem;
        private bool _isValidKey;
        private string _keyCombination;

        public string KeyCombination
        {
            get
            {
                return _keyCombination;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="listView">ListView to use</param>
        public HotkeyChange(ListViewItem listViewItem)
        {
            InitializeComponent();
            _listViewItem = listViewItem;
        }

        #endregion

        #region Form events

        /// <summary>
        /// On form load, set the value of labels and set the new width of the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HotkeyChange_Load(object sender, EventArgs e)
        {
            lblTitle.Text = "Press the new key combination for ";
            lblValue.Text = _listViewItem.Text;
            lblValue.Location = new Point(lblTitle.Width + 5, lblValue.Location.Y);

            Width = lblTitle.Width + lblValue.Width + 32;
            CenterToParent();

            ActiveControl = lblTitle;

            if (_listViewItem.SubItems[1].Text == "None")
            {
                btnUnset.Enabled = false;
            }
        }

        /// <summary>
        /// Unset dialog result
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUnset_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Ignore; // no appropriate dialog result
        }

        /// <summary>
        /// Cancel dialog result
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Form key down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HotkeyChange_KeyDown(object sender, KeyEventArgs e)
        {
            // convert to array of Ctrl | Shift | Alt
            var keys = new KeysConverter().ConvertToString(e.KeyData)
                .Replace("ControlKey", "")
                .Replace("ShiftKey", "")
                .Replace("Menu", "")
                .Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);

            // count the number of modifier/s
            var keyModifiersCount = e.Modifiers.ToString()
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Length;

            // check if key pressed have modifier and key
            if (keyModifiersCount > 0 && keys.Length > keyModifiersCount)
            {
                _isValidKey = true;
                _keyCombination = string.Join(" + ", keys);
            }
            else
            {
                _isValidKey = false;
                _keyCombination = string.Empty;
            }
        }

        /// <summary>
        /// Form key up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HotkeyChange_KeyUp(object sender, KeyEventArgs e)
        {
            if (_isValidKey)
            {
                DialogResult = DialogResult.OK;
            }
        }

        #endregion
    }
}
