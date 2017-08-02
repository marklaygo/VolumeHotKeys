using System;
using System.Drawing;
using System.Windows.Forms;

namespace VolumeHotKeys
{
    public partial class YesNoDialogBox : Form
    {
        #region Fields

        ListViewItem _listViewItem;

        #endregion

        #region Constrcutor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="listViewItem"></param>
        public YesNoDialogBox(ListViewItem listViewItem)
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
        private void YesNoDialogBox_Load(object sender, EventArgs e)
        {
            lblTitle.Text = $"{_listViewItem.Text}? Current value: ";
            lblValue.Text = _listViewItem.SubItems[1].Text;
            lblValue.Location = new Point(lblTitle.Width + 5, lblValue.Location.Y);

            Width = lblTitle.Width + lblValue.Width + 32;
            CenterToParent();

            if (_listViewItem.SubItems[1].Text == "Yes")
            {
                ActiveControl = btnYes;
            }
            else
            {
                ActiveControl = btnNo;
            }
        }

        /// <summary>
        /// Yes dialog result
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnYes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        /// <summary>
        /// No dialog result
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }

        #endregion
    }
}
