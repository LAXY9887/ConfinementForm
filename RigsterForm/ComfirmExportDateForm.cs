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
    public partial class ComfirmExportDateForm : Form
    {
        public const string ALL_RANGE = "All_range";
        public const string FILTER_DATE = "Filter_date";
        public const string DEFAULT = "Default";
        public string export_choice;

        public ComfirmExportDateForm()
        {
            InitializeComponent();
            export_choice = DEFAULT;
        }

        private void export_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void all_hist_btn_Click(object sender, EventArgs e)
        {
            export_choice = ALL_RANGE;
            this.Close();
        }

        private void select_date_hist_Click(object sender, EventArgs e)
        {
            export_choice = FILTER_DATE;
            this.Close();
        }
    }
}
