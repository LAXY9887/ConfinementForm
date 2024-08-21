
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RigsterForm
{
    public abstract class ComboBoxPicker
    {
        // 設定列表
        public void LoadCBList(ComboBox CB, string[] items)
        {
            // 事先清空列表
            CB.Items.Clear();

            // 然後加入項目
            CB.Items.AddRange(items);
        }

        // 設定預設的值
        public void SetValue(ComboBox CB, string default_value) 
        { 
            CB.Text = default_value;
        }
    }

    /** 參考選擇器 **/
    public class RefPicker : ComboBoxPicker
    {
        // 得到 ComboBox
        public ComboBox refComboBox;

        // 影響目標物
        public List<Control> targetCtrls;

        // 選項
        public string[] options;

        // 預設值
        public string default_value;

        // 建構式
        public RefPicker(ComboBox refComboBox, string[] opts, string DefaultOption, List<Control> target_ctrls)
        { 
            // 設定輸入
            this.refComboBox = refComboBox;
            this.options = opts;
            this.default_value = DefaultOption;
            this.targetCtrls = target_ctrls;

            // 套用選項和預設值
            updateOptions(options);
        }

        // 更新選項
        public void updateOptions(string[] newOpts)
        {
            LoadCBList(refComboBox, newOpts);
            SetValue(refComboBox, default_value);
        }

        // 依照選項改變參考項目
        public void ApplyChoice(List<Control> Ref_Ctrls) 
        {
            for (int i = 0; i < Ref_Ctrls.Count; i++)
            {
                targetCtrls[i].Text = Ref_Ctrls[i].Text;
            }
        }
    }

    /** 日期選擇器 **/
    public class DatePicker : ComboBoxPicker
    {
        // 日期Combobox組合
        public ComboBox YearCB;
        public ComboBox MonthCB;
        public ComboBox DayCB;

        // 取得日期字串
        public string YearStr;
        public string MonthStr;
        public string DayStr;

        // 預設日期
        public DateTime defaultDate;

        // 建構式
        public DatePicker(ComboBox year_cb, ComboBox month_cb, ComboBox day_cb,DateTime default_date) 
        {
            // 日期Combobox組合
            YearCB = year_cb;
            MonthCB = month_cb;
            DayCB = day_cb;

            // 設定日期邊界
            setDateLimit(YearCB,1,999);
            setDateLimit(MonthCB, 1, 12);
            setDateLimit(DayCB, 1, 31);

            // 預設日期
            defaultDate = default_date;
            setDefaultDate();
        }

        // 設定日期上下限
        private void setDateLimit(ComboBox target,int min, int max)
        {
            // 生成列表並載入
            string[] limitedDateNums = Enumerable.Range(min, max).Select(n => n.ToString()).ToArray();
            LoadCBList(target, limitedDateNums);
        }

        // 設定預設日期
        public void setDefaultDate()
        {
            SetValue(YearCB, defaultDate.Year.ToString());
            SetValue(MonthCB, defaultDate.Month.ToString());
            SetValue(DayCB, defaultDate.Day.ToString());

            // 取得字串
            updateDateStr();
        }

        // 更新日期字串
        public void updateDateStr()
        {
            YearStr = YearCB.Text;
            MonthStr = MonthCB.Text;
            DayStr = DayCB.Text;
        }
    }

    /** 地址選擇器 **/
    public class AdressPicker : ComboBoxPicker
    {
        // 資料庫
        public string database_Path;

        // 資料庫內容
        public string districtContent;
        public List<districtStruct> districtList;

        // 縣市列表
        public string[] cityList;

        // 縣市鄉鎮Combobox組合
        public ComboBox CityComboBox;
        public ComboBox CountryComboBox;

        // 鄰里道路的TextBox
        public TextBox roadTextBox;

        // 完整地址顯示
        public TextBox FullAdressTextBox;

        // 預設縣市鄉鎮選擇
        public string DefaultCity;
        public string DefaultCountry;

        // 建構式
        public AdressPicker(string dbPath, ComboBox City_CB, string default_City, ComboBox Country_CB, string default_Country, TextBox road_TB, TextBox fullAdressTextBox)
        {
            // 資料庫路徑
            database_Path = dbPath;

            // 讀取資料庫
            districtContent = File.ReadAllText(database_Path);
            districtList = JsonConvert.DeserializeObject<List<districtStruct>>(districtContent);

            // 縣市鄉鎮Combobox組合
            CityComboBox = City_CB;
            CountryComboBox = Country_CB;

            // 鄰里道路
            roadTextBox = road_TB;

            // 預設縣市鄉鎮選擇
            DefaultCity = default_City;
            DefaultCountry = default_Country;

            // 初始化設定
            InitializeBoxValues();

            // 完整地址
            FullAdressTextBox = fullAdressTextBox;
            UpdateFullAdress();

            // 綁定變化偵測功能-選擇
            CityComboBox.SelectedIndexChanged += ContentChange;
            CountryComboBox.SelectedIndexChanged += ContentChange;

            // 綁定變化偵測功能-打字
            CityComboBox.TextChanged += ContentChange;
            CountryComboBox.TextChanged += ContentChange;
            roadTextBox.TextChanged += ContentChange;
        }

        // 初始化
        public void InitializeBoxValues()
        {
            // 載入預設值-縣市
            cityList = districtList.Select(dis => dis.city).ToArray();
            LoadCBList(CityComboBox, cityList);
            SetValue(CityComboBox, DefaultCity);

            // 載入預設值-鄉鎮市區
            LoadCountryList(DefaultCity);
            SetValue(CountryComboBox, DefaultCountry);
        }

        // 組合完整地址並顯示
        private void UpdateFullAdress()
        {
            string adressFull = $"{CityComboBox.Text}{CountryComboBox.Text}{roadTextBox.Text}";
            FullAdressTextBox.Text = adressFull;
        }

        // 載入鄉鎮市區列表
        public void LoadCountryList(string citySelect)
        {
            // 得到該城市的鄉鎮列表
            districtStruct selectedCity = districtList.Where(d => d.city == citySelect).First();

            // 加入列表
            LoadCBList(CountryComboBox, selectedCity.district.ToArray());
        }

        // ComboBox選擇和TextBox內容改變
        public void ContentChange(object sender, EventArgs e)
        {
            // 取得變化目標
            Control target = sender as Control;
            
            // 如果是縣市, 則改變鄉鎮
            if (target.Name == CityComboBox.Name)
            {
                ComboBox ChangedComboBox = (ComboBox)sender;
                LoadCountryList(ChangedComboBox.Text);
                SetValue(CountryComboBox, CountryComboBox.Items[0].ToString());
            }

            // 更新地址顯示
            UpdateFullAdress();
        }
    }
}
