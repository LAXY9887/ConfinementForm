using System;
using System.Windows.Forms;

namespace RigsterForm
{
    public partial class ReasomForm : Form
    {
        public string ReasonStr;

        public ReasomForm()
        {
            InitializeComponent();

            ReasonStr = "";
        }

        private void reason_confirmBtn_Click(object sender, EventArgs e)
        {
            ReasonStr = textBoxreason.Text;
            Close();
        }

        private void reason_cancelBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
