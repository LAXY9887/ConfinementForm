using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RigsterForm
{
    public partial class SensorForm : Form
    {
        // 選項
        public enum ButtonID
        {
            Approved, Denied, Cancel, NULL
        }

        // 審核結果
        public const string STR_APPROVED = "通過";
        public const string STR_DISAPPROVED = "不通過";
        public const string STR_NOT_SENSRO = "未審核";
        public Dictionary<ButtonID, string> SensorResMatrix = new Dictionary<ButtonID, string>()
        {
            { ButtonID.Approved, STR_APPROVED}, 
            { ButtonID.Denied, STR_DISAPPROVED}, 
            {ButtonID.NULL, STR_NOT_SENSRO}
        };
        

        // 點擊目標
        public ButtonID clickTarget;

        public SensorForm()
        {
            InitializeComponent();

            // 初始化選項
            clickTarget = ButtonID.NULL;
        }

        private void ApproveBtn_Click(object sender, EventArgs e)
        {
            clickTarget = ButtonID.Approved;
            this.Close();
        }

        private void DeniedBtn_Click(object sender, EventArgs e)
        {
            clickTarget = ButtonID.Denied;
            this.Close();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            clickTarget = ButtonID.Cancel;
            this.Close();
        }

        private void UnSensorBtn_Click(object sender, EventArgs e)
        {
            clickTarget = ButtonID.NULL;
            this.Close();
        }
    }
}
