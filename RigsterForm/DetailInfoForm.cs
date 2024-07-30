using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RigsterForm
{
    public partial class DetailInfoForm : Form
    {
        public DetailInfoForm()
        {
            InitializeComponent();
        }

        private void closeDetailBtn_Click(object sender, EventArgs e)
        {
            // 關閉該視窗
            this.Close();
        }

        public RichTextBox getDetailBox() 
        {
            return this.detailInfoBOX;
        }
    }
}
