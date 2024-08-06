using System;
using System.Drawing;
using System.Windows.Forms;

namespace RigsterForm
{
    public class CheckBoxController
    {
        // 取得 Control
        public CheckBox checkBox;

        // 建構式
        public CheckBoxController(CheckBox checkBox)
        {
            this.checkBox = checkBox;
        }

        // 更新TextBox
        public void updateText(TextBox from, TextBox to)
        {
            to.Text = from.Text;
            to.ForeColor = Color.DarkGray;
            to.Enabled = false;
        }

        // 更新顏色
        public void initialize_txt_color(TextBox target_tb)
        {
            target_tb.ForeColor = Color.DarkBlue;
            target_tb.Enabled = true;
        }

        // 更新鎖定ComboBox
        public void update_selection_Box(Control from, Control to)
        {
            to.Text = from.Text;
            to.Enabled = false;
        }

        // 解鎖ComboBox
        public void unlock_selection_Box(Control target_ctrl)
        {
            target_ctrl.Enabled = true;
        }
    }

    // 地址控制
    public class AdressCheckController : CheckBoxController
    {
        // 取得地址控制
        public AdressPicker ControlledAdressPicker_ref;
        public AdressPicker ControlledAdressPicker_target;

        // 縣市鄉鎮Combobox, 鄰里道路TextBox (參考)
        private ComboBox CityCB_ref;
        private ComboBox CountryCB_ref;
        private TextBox RoadTB_ref;

        // 縣市鄉鎮Combobox, 鄰里道路TextBox (目標)
        private ComboBox CityCB_target;
        private ComboBox CountryCB_target;
        private TextBox RoadTB_target;

        // 建構式
        public AdressCheckController(CheckBox checkBox, AdressPicker adressPicker_ref, AdressPicker adressPicker_target) : base(checkBox) 
        {
            // 取得 Adress 控制
            ControlledAdressPicker_ref = adressPicker_ref;
            ControlledAdressPicker_target = adressPicker_target;

            // 縣市鄉鎮Combobox, 鄰里道路TextBox (參考)
            CityCB_ref = ControlledAdressPicker_ref.CityComboBox; ;
            CountryCB_ref = ControlledAdressPicker_ref.CountryComboBox; ;
            RoadTB_ref = ControlledAdressPicker_ref.roadTextBox;

            // 縣市鄉鎮Combobox, 鄰里道路TextBox (目標)
            CityCB_target = ControlledAdressPicker_target.CityComboBox; ;
            CountryCB_target = ControlledAdressPicker_target.CountryComboBox; ;
            RoadTB_target = ControlledAdressPicker_target.roadTextBox;

            // 將 CheckBox 綁定變化功能
            checkBox.CheckedChanged += CheckChanged;
        }

        // 套用地址
        private void ApplyAdress()
        {
            // 更新TextBox和ComboBox
            update_selection_Box(CityCB_ref, CityCB_target);
            update_selection_Box(CountryCB_ref, CountryCB_target);
            updateText(RoadTB_ref, RoadTB_target);
        }

        // 解鎖地址
        private void UnlockAdress() 
        {
            unlock_selection_Box(CityCB_target);
            unlock_selection_Box(CountryCB_target);
            initialize_txt_color(RoadTB_target);
        }

        // 偵測變化
        public void CheckChanged(object sender, EventArgs e) 
        {
            bool Checked = checkBox.Checked;
            if (Checked) 
            {
                ApplyAdress();
            }
            else 
            {
                UnlockAdress();
            }
        }

        // 取消勾選
        public void unCheckBox()
        {
            if (checkBox.Checked)
            {
                checkBox.Checked = false;
            }
        }
    }
}
