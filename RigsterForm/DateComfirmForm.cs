using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RigsterForm
{
    public partial class DateComfirmForm : Form
    {
        public DatePicker datepicker;

        public string GetDate()
        {
            return $"{datepicker.YearCB.Text}年{datepicker.MonthCB.Text}月{datepicker.DayCB.Text}日";
        }

        public enum Options
        {
            Comfirm, Cancel, NOT_REMIT, NONE
        }

        public Options comfirm;

        public DateComfirmForm()
        {
            InitializeComponent();

            // 日期選擇器
            DateTime now = DateTime.Now;
            DateTime defaultDate = new DateTime(now.Year - 1911, now.Month,now.Day);
            datepicker = new DatePicker(comfirmYY_btn, comfirmMM_btn, comfirmDD_btn, defaultDate);

            // 選擇
            comfirm = Options.NONE;
        }

        private void comfirmDateOKBtn_Click(object sender, EventArgs e)
        {
            comfirm = Options.Comfirm;
            this.Close();
        }

        private void comfirmDateCancelBtn_Click(object sender, EventArgs e)
        {
            comfirm = Options.Cancel;
            this.Close();
        }

        private void NOTRemitBtn_Click(object sender, EventArgs e)
        {
            comfirm = Options.NOT_REMIT;
            this.Close();
        }
    }
}
