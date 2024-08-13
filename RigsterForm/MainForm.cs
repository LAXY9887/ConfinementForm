using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RigsterForm
{
    public partial class MainForm : Form
    {
        /* 儲存 Flags */
        public bool isEditingPrevData;          // 檢查是否在編輯舊資料中

        /** 組件 **/
        #region Components

        /* 系統設置 */
        private SystemSettings settingCtrl;

        /* 使用地址選擇器 */
        private AdressPicker RgisAdressPicker;
        private AdressPicker CommAdressPicker;

        /* 使用日期選擇器 */
        private DatePicker dateFilterPicker_start;
        private DatePicker dateFilterPicker_end;

        /* 使用參考選擇器 */
        private RefPicker refPicker;

        /* 使用 CheckBox controller */
        private AdressCheckController adressCheckControl;

        /* 使用自製函式庫 */
        private Utilities utilities;

        /* 儲存可新增式Panel */
        public AddablePanel apply_phone_panels;
        public AddablePanel mate_phone_panels;
        public AddablePanel query_phone_panels;
        public newBornPanel newBorn_panels;

        /* 儲存群組資訊 */
        public enum GroupBoxID  { Apply, Mate, Query, Account, NewBorn, ButtonPanel }       // 群組名稱
        public Dictionary<GroupBoxID, List<Control>> groupBoxInfluenced;                           // 組合被影響群組名稱和ID
        public Dictionary<GroupBoxID, string> groupBoxIDNameMatrix;                                  // 組合群組名稱與ID

        #endregion

        /** Initialization Functions **/
        #region Initialization_Functions

        // 綁定禁用滑鼠滾輪
        public void BindingMouseWheelFunction()
        {
            List<ComboBox> influenced_list = new List<ComboBox>()
            {
                refComboBox, Rgis_City_combobox, Comm_City_combobox, Rgis_Country_combobox, Comm_Country_combobox,
                birthYearCB_0, birthMonthCB_0, birthdayCB_0, search_start_yy,search_start_mm,search_start_dd,
                search_end_yy,search_end_mm,search_end_dd
            };
            foreach (ComboBox cb in influenced_list)
            {
                cb.MouseWheel += ComboBox_MouseWheel;
            }
        }

        // 處理系統設定相關
        private void InitializeSettings()
        {
            /* 系統設置 */
            settingCtrl = new SystemSettings();
        }

        // 新建Database
        private bool CreateNewDatabase()
        {
            // 跳出對話視窗, 詢問是否真的要新建一個
            DialogResult result = MessageBox.Show(
            "沒有偵測到資料庫, 是否新增一個?",
            "系統提示",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.FileName = $"Database.json";
                    saveFileDialog.Filter = "Json Files|*.json";
                    saveFileDialog.Title = "Save an Database File";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        settingCtrl.InitializeDataBase(saveFileDialog.FileName);
                        settingCtrl.data_basePath = saveFileDialog.FileName;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        // 初始化字典, 組合等等
        public void InitializeMatrices() 
        {
            groupBoxInfluenced = new Dictionary<GroupBoxID, List<Control>>
            {
                { GroupBoxID.Apply , new List<Control> { groupBox_mate, groupBox_query, groupBox_Adress, groupBox_newBorn, ButtonPannel }},
                { GroupBoxID.Mate , new List<Control> { groupBox_query, groupBox_Adress, groupBox_newBorn, ButtonPannel }},
                { GroupBoxID.Query,   new List<Control> { groupBox_Adress, groupBox_newBorn, ButtonPannel }},
                { GroupBoxID.NewBorn, new List<Control> { ButtonPannel }}
            };

            groupBoxIDNameMatrix = new Dictionary<GroupBoxID, string>
            {
                { GroupBoxID.Apply, "groupBox_apply" },
                { GroupBoxID.Mate, "groupBox_mate" },
                { GroupBoxID.Query, "groupBox_query" },
                { GroupBoxID.Account, "groupBox_Adress"},
                { GroupBoxID.NewBorn, "groupBox_newBorn"},
                { GroupBoxID.ButtonPanel, "ButtonPannel"}
            };
        }

        // 初始化可新增式面板
        private void InitializePanels() 
        {
            // 初始化連絡電話和新生兒列表
            apply_phone_panels = new AddablePanel(a_phone_pannel_0, groupBox_apply);
            mate_phone_panels = new AddablePanel(m_phone_pannel_0, groupBox_mate);
            query_phone_panels = new AddablePanel(q_phone_pannel_0, groupBox_query);
            newBorn_panels = new newBornPanel(_newBorn_panel_0, groupBox_newBorn);
        }

        // 初始化部份顯示資訊
        private void InitializeSomeInfo() 
        {
            /* 初始化登錄日期資訊 */
            firstLogTimeLabel.Text = "-";                                           // 初始化日期時間顯示-初次登錄
            RecentEditTimeLabel.Text = firstLogTimeLabel.Text;     // 初始化日期時間顯示-最近修改
            nb_num_TextBOX.Text = newBorn_panels.panel_nums.ToString();       // 新生兒數量顯示

            /* 刪除多的 addable panels */
            apply_phone_panels.InitializePanels(groupBoxInfluenced[GroupBoxID.Apply]);
            mate_phone_panels.InitializePanels(groupBoxInfluenced[GroupBoxID.Mate]);
            query_phone_panels.InitializePanels(groupBoxInfluenced[GroupBoxID.Query]);
            newBorn_panels.InitializePanels(groupBoxInfluenced[GroupBoxID.NewBorn]);

            /* 清空所有 TextBox */
            List<Control> gbList2Clear = new List<Control> 
            {
                groupBox_apply, groupBox_mate, 
                groupBox_query, groupBox_Adress,
                apply_phone_panels.panel_list[0], 
                mate_phone_panels.panel_list[0], 
                query_phone_panels.panel_list[0],
                newBorn_panels.panel_list[0]
            };
            foreach (Control gb in gbList2Clear)
            {
                clearTextBox(gb);
            }

            // 重設 Combobox
            RgisAdressPicker.InitializeBoxValues();
            CommAdressPicker.InitializeBoxValues();

            // 更新生兒數量
            nb_num_TextBOX.Text = newBorn_panels.panel_nums.ToString();
        }

        // 初始化流水號顯示
        private void InitializeSerialNumDisplay() 
        {
            // 擷取最後一份資料
            List<dataStruct> current_database = utilities.ReadDatabase(settingCtrl.data_basePath);
            dataStruct latestData = current_database[current_database.Count - 1];

            // 取得年分
            string current_year_str = (DateTime.Now.Year - 1911).ToString();
            string latest_year_str = latestData.serial_num.ToString().Split('-')[0];

            // 顯示流水號
            int latest_serial_num = latestData.serial_index + 1;

            // 當年分不一樣時, 改變流水號
            if (current_year_str != latest_year_str)
            {
                latest_serial_num = 1;
            }

            // 顯示流水號
            string serial2Show = current_year_str + '-' + latest_serial_num.ToString();
            SerialNumLabel.Text = serial2Show;
        }

        // 初始化Flags
        private void InitializeFlags() 
        {
            isEditingPrevData = false;
        }

        // 實體化選擇器
        private void InitializeComboBoxPickers()
        {
            // 使用地址選擇器--戶籍地
            RgisAdressPicker = new AdressPicker(
                utilities.ResolvePath(ConstParameters.district_db_path),
                Rgis_City_combobox, ConstParameters.InitialCity,
                Rgis_Country_combobox, ConstParameters.InitialCountry,
                Rgis_Road_TextBox, regisAdressTb);

            // 使用地址選擇器--通訊地
            CommAdressPicker = new AdressPicker(
                utilities.ResolvePath(ConstParameters.district_db_path),
                Comm_City_combobox, ConstParameters.InitialCity,
                Comm_Country_combobox, ConstParameters.InitialCountry,
                Comm_Road_TextBox, commAdressTb);

            // 使用日期選擇器--篩選起始日期
            dateFilterPicker_start = new DatePicker(search_start_yy, search_start_mm, search_start_dd, new DateTime(DateTime.Now.Year - 1911, DateTime.Now.Month, 1));
            dateFilterPicker_end = new DatePicker(search_end_yy, search_end_mm, search_end_dd, new DateTime(DateTime.Now.Year - 1911, DateTime.Now.Month+1, 1));

            // 使用參考選擇
            List<Control> targetCtrls = new List<Control>() { textBox_account_name , accountID_tb };
            string[] refOptions = new string[] { "-選擇-", "產婦", "配偶", "委託人", "新生兒1" };
            refPicker = new RefPicker(refComboBox, refOptions, "-選擇-", targetCtrls);
        }

        // 實體化CheckBox控制器
        private void InitializeCheckBoxControl()
        {
            // 地址
            adressCheckControl = new AdressCheckController(AdressCheck, RgisAdressPicker, CommAdressPicker);
        }

        #endregion

        public MainForm()
        {
            // 預設路徑
            string default_path = ConstParameters.default_database_path;

            // 設置系統
            InitializeSettings();

            // 使用自製函式
            settingCtrl.SetDatabasePath(default_path);
            utilities = new Utilities(settingCtrl);

            // 檢查資料庫連線
            bool isDatabaseConnected = utilities.Database_connected(settingCtrl.data_basePath);

            // 若初始沒有偵測到資料庫, 詢問是否新增一個
            if (!isDatabaseConnected) 
            {
                bool isCreate = CreateNewDatabase();
                if (!isCreate)
                {
                    return;
                }
            }

            // 初始化區塊
            #region InitializeThings

            // 初始化頁面
            InitializeComponent();

            // 初始化連絡電話和新生兒面板
            InitializePanels();

            // 初始化流水號顯示
            InitializeSerialNumDisplay();

            // 載入組合
            InitializeMatrices();

            // 初始化 Flags
            InitializeFlags();

            // 顯示歷史資料
            InitializeDataGridView();
            LoadJsonToDataGridView(
                startYear: DateTime.Now.Year, startMonth: 1, startDay:1,
                endYear: DateTime.Now.Year + 1, endMonth: 1, endDay:1
                );

            // 調整使窗大小
            adjustWindowSize();

            // 實體化選擇器
            InitializeComboBoxPickers();

            // 初始化部分資訊顯示
            InitializeSomeInfo();

            // 初始化 CheckBox 控制器
            InitializeCheckBoxControl();

            // 綁定功能
            BindingMouseWheelFunction();

            

            #endregion
        }

        /** 處理資料登錄database **/
        #region DataLogin

        // 處裡日期: {初次登入, 最近一次修改}
        private string[] HandleDateTime() 
        {
            // 得到當前日期
            string currentDateTime = utilities.GetCurrentDate();
            string recentDateTime = currentDateTime;

            // 如果正在修改. 則把當前日期改回原本
            if (isEditingPrevData)
            {
                currentDateTime = firstLogTimeLabel.Text;
            }

            return new string[] { currentDateTime, recentDateTime };
        }

        // 處理流水號: {編號, 年}
        private int[] HandleSerialNum(dataStruct lastestData) 
        {
            // 計算流水號
            int inputSearialN = 0;
            int inputYear = 0;

            // 資料庫中最近一次的流水號
            int lastSerialN = lastestData.serial_index;
            int latest_year = lastestData.login_year;

            // 當年分不一樣時, 改變流水號
            if (DateTime.Now.Year-1911 != latest_year)
            {
                lastSerialN = 0;
            }

            // 如果是修改模式, 則把流水號調整成該次的資訊
            if (isEditingPrevData)
            {
                string shown_serial_num = SerialNumLabel.Text;
                int shown_serialN = Int32.Parse(shown_serial_num.Split('-')[1]);
                int shown_serialYEAR = Int32.Parse(shown_serial_num.Split('-')[0]);
                inputSearialN = shown_serialN;
                inputYear = shown_serialYEAR;
            }
            else
            {
                inputSearialN = lastSerialN + 1;
                inputYear = latest_year;
            }

            return new int[] {inputSearialN, inputYear};
        }
       
        // 處理電話號碼
        private List<string> HandlePhones(List<Panel> panel_List) 
        {
            List<string> phones = new List<string>();

            // 加入電話
            foreach (Panel p in panel_List)
            {
                foreach (Control ctrl in p.Controls)
                {
                    if (ctrl is TextBox)
                    {
                        phones.Add(ctrl.Text);
                    }
                }
            }

            return phones;
        }

        // 處理新生兒 - 資料結構
        public struct newBornInfo 
        {
            public string nbNames { get; set; }        // 姓名
            public string newbornID { get; set; }     // 身分證號
            public string nbBirthDay { get; set; }    // 生日

            public newBornInfo(string name, string id, string birthday) 
            { 
                nbNames = name;
                newbornID = id;
                nbBirthDay = birthday;
            }
        }

        // 處理新生兒 - 資料列表
        public List<newBornInfo> HandleNewBornInfo(List<Panel> newBornPanels) 
        {
            char sep = ';';

            List<newBornInfo> nbInfoList = new List<newBornInfo>();

            // 加入新生兒 Panel infomation
            foreach (Panel p in newBornPanels)
            {
                // 新生兒資料結構
                newBornInfo nbInfo = new newBornInfo();

                // 名字
                string Name = "";

                // 身分證
                string ID = "";

                // 生日
                string birthday = "";

                // 加入名字和身分證
                foreach (Control ctrl in p.Controls)
                {
                    // 姓名
                    if (ctrl is TextBox && ctrl.Name.Contains("textBox_name_"))
                    {
                        Name = ctrl.Text.ToString();
                    }

                    // 身分證
                    if (ctrl is TextBox && ctrl.Name.Contains("textBox_newBorn_IDnumber"))
                    {
                        ID = ctrl.Text.ToString();
                    }

                    // 生日
                    if (ctrl is ComboBox)
                    {
                        birthday += ctrl.Text.ToString() + sep;
                    }
                }

                // 加入名字和身分證和生日
                nbInfo.nbNames = Name;
                nbInfo.newbornID = ID;
                nbInfo.nbBirthDay = $"{birthday.Split(sep)[2]}-{birthday.Split(sep)[1]}-{birthday.Split(sep)[0]}";

                // 加入列表
                nbInfoList.Add(nbInfo);
            }

            return nbInfoList;
        }

        // 蒐集並統整表單中的資訊轉換成JSON字串
        private dataStruct GatherInfo() 
        {
            // 讀取資料庫
            List<dataStruct> dataList = utilities.ReadDatabase(settingCtrl.data_basePath);

            // 日期
            string[] GetDateTime = HandleDateTime();
            string currentDateTime = GetDateTime[0];
            string recentDateTime = GetDateTime[1];

            // 流水號
            dataStruct lastData = dataList[dataList.Count - 1];
            int[] SerialNInfo = HandleSerialNum(lastData);
            int inputSearialN = SerialNInfo[0];
            int inputYear = SerialNInfo[1];
            string inputSerialNum = inputYear.ToString() + "-" + inputSearialN.ToString();

            // 儲存電話字串
            List<string> apply_phones = HandlePhones(apply_phone_panels.panel_list);
            List<string> mate_phones = HandlePhones(mate_phone_panels.panel_list);
            List<string> query_phones = HandlePhones(query_phone_panels.panel_list);

            // 儲存新生兒資訊 
            List<string> nbNames = new List<string>();       // 姓名
            List<string> newbornID = new List<string>();    // 身分證號
            List<string> nbBirthDay = new List<string>();    // 生日
            List<newBornInfo> nbInfoSummary = HandleNewBornInfo(newBorn_panels.panel_list);
            foreach (newBornInfo info in nbInfoSummary)
            {
                nbNames.Add(info.nbNames);
                newbornID.Add(info.newbornID);
                nbBirthDay.Add(info.nbBirthDay);
            }

            // 實體化資料結構
            dataStruct data = new dataStruct(
                // 審核
                sensorRes:"未審核",
                // 流水號與日期
                year: DateTime.Now.Year - 1911, month: DateTime.Now.Month, day: DateTime.Now.Day, 
                serialdx: inputSearialN, serialNumStr: inputSerialNum, remitDate:"尚未匯款",
                FirstLogDate: currentDateTime, RecentEditDate: recentDateTime,
                // 申請人
                appName: textBox_apply_name.Text, appID: textBox_apply_IDnumber.Text, appPhones: apply_phones,
                // 配偶
                mateName: textBox_mate_name.Text, mateID: textBox_mate_IDnumber.Text, matePhones: mate_phones,
                // 委託人
                queryName: textBox_query_name.Text, queryID: textBox_query_IDnumber.Text, queryRelate: query_relation.Text, queryPhones: query_phones,
                // 郵局和地址資訊
                ac_name: textBox_account_name.Text, ac_ID: accountID_tb.Text, ac_div: textBox_account_divn.Text, ac_number: textBox_account_number.Text,
                regisAdress: regisAdressTb.Text, cmAdress: commAdressTb.Text,
                // 新生兒資訊
                nbName: nbNames, newBornID: newbornID, nbBitrhDate: nbBirthDay,
                //備註
                note: ""
            );
            return data;
        }

        #endregion

        /** 按鈕功能和GUI功能等 **/
        #region Button Functions

        // 清除TextBox內容
        public void clearTextBox(Control group)
        {
            foreach (Control ctrl in group.Controls)
            {
                if (ctrl is TextBox)
                {
                    ctrl.Text = "";
                }
            }
        }

        // 更新參考ComboBox列表
        private void UpdateRefComboBox()
        {
            List<string> refOptions = new List<string>() { "-選擇-", "產婦", "配偶", "委託人" };
            for (int i = 0; i < newBorn_panels.panel_nums; i++)
            {
                refOptions.Add($"新生兒{i + 1}");
            }
            refPicker.updateOptions(refOptions.ToArray());
        }

        // 參考ComboBox功能
        private void ChangeRefComboBox()
        {
            // 選中的那一個選項
            int selectedIDX = refComboBox.SelectedIndex;

            // 被變更的位置
            List<Control> targerCtrls = new List<Control>() { textBox_account_name, accountID_tb };

            // 判斷如何更動
            switch (selectedIDX)
            {
                // 產婦
                case 1:
                    List<Control> refCtrls_app = new List<Control>() { textBox_apply_name, textBox_apply_IDnumber };
                    refPicker.ApplyChoice(refCtrls_app);
                    break;

                // 配偶
                case 2:
                    List<Control> refCtrls_mate = new List<Control>() { textBox_mate_name, textBox_mate_IDnumber };
                    refPicker.ApplyChoice(refCtrls_mate);
                    break;

                // 委託人
                case 3:
                    List<Control> refCtrls_query = new List<Control>() { textBox_query_name, textBox_query_IDnumber };
                    refPicker.ApplyChoice(refCtrls_query);
                    break;
            }

            // 新生兒
            if (selectedIDX > 3)
            {
                int newBornIdx = refComboBox.SelectedIndex - 4;
                Panel SelectedNewBornPanel = newBorn_panels.panel_list[newBornIdx];
                List<Control> refCtrls_nb = new List<Control>();
                foreach (Control control in SelectedNewBornPanel.Controls)
                {
                    if (control.Name.Contains(ConstParameters.TextBoxNamePrefix))
                    {
                        refCtrls_nb.Add(control);
                    }
                    if (control.Name.Contains(ConstParameters.TextBoxIDPrefix))
                    {
                        refCtrls_nb.Add(control);
                    }
                }
                refPicker.ApplyChoice(refCtrls_nb);
            }
        }

        // 偵測改變並應用參考Combobox
        public void SelectedChange(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox == refComboBox)
            {
                ChangeRefComboBox();
            }
        }

        // 調整UI大小以及位置
        private void adjustWindowSize()
        {
            // 获取屏幕分辨率
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            // 視窗
            this.Width = screenWidth;
            this.Height = screenHeight;

            // 頁簽版面
            tabControl.Size = new Size(screenWidth - 50, screenHeight - 100);
            tabControl.Location = new Point((screenWidth - tabControl.Width) / 2, 10);
            LogPage.Size = LogPage.Parent.Size;
            historyPage.Size = historyPage.Parent.Size;
            historyGridView.Size = new Size((int)(historyGridView.Parent.Width * 0.98), (int)(historyGridView.Parent.Height * 0.78));
            collectionPanel1.Location = new Point(collectionPanel1.Parent.Width - collectionPanel1.Width - 50, collectionPanel1.Location.Y);

            // 頁簽內容
            collectionPanle.Location = new Point((collectionPanle.Parent.Width - collectionPanle.Width) / 2, 0);
        }

        // 新增一個面板(手機號碼或新生兒)
        public void Add_panel_Btn_Click(object sender, EventArgs e)
        {
            // 檢測是哪一顆按鈕
            Button target = (Button)sender;

            // 取得當前按鈕和父物件和群組
            GroupBox Group = target.Parent as GroupBox;
            string Group_name = Group.Name;
            GroupBoxID groupID = groupBoxIDNameMatrix.First(g => g.Value == Group_name).Key;

            // 新增時輸入空字串
            string emptyStr = "";

            switch (groupID)
            {
                /**申請人手機**/
                case GroupBoxID.Apply:
                    apply_phone_panels.AddButtonClicked(groupBoxInfluenced[GroupBoxID.Apply],emptyStr);                     // 新增一個版面
                    apply_phone_panels.deleteButtons[apply_phone_panels.panel_nums-1].Click += Delete_panel_Btn_Click; // 將刪除函式綁定到新按鈕上面
                    break;

                /**配偶手機**/
                case GroupBoxID.Mate:
                    mate_phone_panels.AddButtonClicked(groupBoxInfluenced[GroupBoxID.Mate], emptyStr);                       // 新增一個版面
                    mate_phone_panels.deleteButtons[mate_phone_panels.panel_nums - 1].Click += Delete_panel_Btn_Click; // 將刪除函式綁定到新按鈕上面
                    break;

                /**委託人手機**/
                case GroupBoxID.Query:
                    query_phone_panels.AddButtonClicked(groupBoxInfluenced[GroupBoxID.Query], emptyStr);                       // 新增一個版面
                    query_phone_panels.deleteButtons[query_phone_panels.panel_nums - 1].Click += Delete_panel_Btn_Click; // 將刪除函式綁定到新按鈕上面
                    break;

                 /** 新生兒面板 **/
                case GroupBoxID.NewBorn:
                    newBorn_panels.AddNBButtonClicked(groupBoxInfluenced[GroupBoxID.NewBorn]);                                   // 新增一個版面
                    newBorn_panels.deleteButtons[newBorn_panels.panel_nums - 1].Click += Delete_panel_Btn_Click;              // 將刪除函式綁定到新按鈕上面
                    nb_num_TextBOX.Text = newBorn_panels.panel_nums.ToString();
                    foreach (Control ctrl in newBorn_panels.panel_list[newBorn_panels.panel_nums - 1].Controls)
                    {
                        if (ctrl is ComboBox)
                        {
                            ComboBox cb = ctrl as ComboBox;
                            cb.MouseWheel += ComboBox_MouseWheel;
                        }
                    }
                    UpdateRefComboBox(); // 更新參考選項
                    break;
            }

        }

        // 刪除一個面板(手機號碼或新生兒)
        public void Delete_panel_Btn_Click(object sender, EventArgs e)
        {
            // 取得當前按鈕和父物件和群組
            Button button = sender as Button;
            Panel parentPanel = button.Parent as Panel;
            GroupBox Group = parentPanel.Parent as GroupBox;
            string Group_name = Group.Name;
            GroupBoxID groupID = groupBoxIDNameMatrix.First(g => g.Value == Group_name).Key;

            switch (groupID)
            {
                /**申請人手機**/
                case GroupBoxID.Apply:
                    apply_phone_panels.DeleteButtonClicked(parentPanel, groupBoxInfluenced[GroupBoxID.Apply]);
                    break;

                /**配偶手機**/
                case GroupBoxID.Mate:
                    mate_phone_panels.DeleteButtonClicked(parentPanel, groupBoxInfluenced[GroupBoxID.Mate]);
                    break;

                /**委託人手機**/
                case GroupBoxID.Query:
                    query_phone_panels.DeleteButtonClicked(parentPanel, groupBoxInfluenced[GroupBoxID.Query]);
                    break;

                /** 新生兒面板 **/
                case GroupBoxID.NewBorn:
                    newBorn_panels.DeleteButtonClicked(parentPanel, groupBoxInfluenced[GroupBoxID.NewBorn]);
                    nb_num_TextBOX.Text = newBorn_panels.panel_nums.ToString();
                    UpdateRefComboBox(); // 更新參考選項
                    break;
            }
        }

        // 查詢按鈕
        private void Search_ID_Btn_Click(object sender, EventArgs e)
        {
            // 回報
            bool Report(string inputStr)
            {
                bool findID = false;

                // 檢查身分證格式
                if (inputStr.Length == 0)
                {
                    MessageBox.Show("請輸入身分證/居留證!", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return findID;
                }

                string dupReport = ValideID(inputStr);

                if (dupReport != "")
                {
                    MessageBox.Show($"該筆資料已經登錄!\n\n{dupReport}", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    findID = true;
                    return findID;
                }
                else
                {
                    MessageBox.Show($"搜尋:{inputStr}\n\n尚未登入該筆資料。", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return findID;
                }
            }

            // 檢查新生兒ID
            void checkNewBornID(GroupBox group)
            {
                foreach (Control ctrls in group.Controls)
                {
                    if (ctrls is Panel)
                    {
                        foreach (Control handle in ctrls.Controls)
                        {
                            if (handle.Name.Contains("textBox_newBorn_IDnumber_") && handle.Text.Length > 0)
                            {
                                bool isfind = Report(handle.Text);
                                if (isfind) { return; } // 找到即中斷
                            }
                        }
                    }
                }
            }

            // 取得當前按鈕和父物件和群組
            Button button = sender as Button;
            GroupBox Group = button.Parent as GroupBox;
            string Group_name = Group.Name;
            GroupBoxID groupID = groupBoxIDNameMatrix.First(g => g.Value == Group_name).Key;

            // 讀取資料庫
            List<dataStruct> dataList = utilities.ReadDatabase(settingCtrl.data_basePath);

            // 如果讀取失敗則不會執行以下
            if (dataList is null) { return; }

            switch (groupID)
            {
                case GroupBoxID.Apply:
                    Report(textBox_apply_IDnumber.Text);
                    break;
                case GroupBoxID.Mate:
                    Report(textBox_mate_IDnumber.Text);
                    break;
                case GroupBoxID.Query:
                    Report(textBox_query_IDnumber.Text);
                    break;
                case GroupBoxID.NewBorn:
                    checkNewBornID(Group);
                    break;
            }
        }

        #endregion

        /** 儲存資料功能 **/
        #region Saving Data

        // 驗證個欄位
        private bool ValidateInfomation(dataStruct NewInfomation, List<string> ids2check, List<string> checkItem) 
        {
            // 驗證各個欄位
            List<string> names = new List<string> { NewInfomation.apply_name };
            foreach (string nbName in NewInfomation.newBorn_name)
            {
                names.Add(nbName);
            }
            bool isNameInvalid = names.Any(n => n.Length == 0);

            if (isNameInvalid)
            {
                MessageBox.Show("申請人/新生兒姓名不能為空", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // 電話
            int phoneCount = 0;
            List<List<string>> phones = new List<List<string>> { NewInfomation.apply_phones, NewInfomation.query_phones, NewInfomation.mate_phones };
            foreach (var phone in phones)
            {
                foreach (var p in phone)
                {
                    if (p.Length > 0)
                    {
                        phoneCount++;
                    }
                }
            }
            if (phoneCount < 1)
            {
                MessageBox.Show("請至少輸入一個聯絡電話", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // 檢查身分證是否空缺
            for (int i = 0; i < ids2check.Count; i++)
            {
                if (checkItem[i].Contains("申請人") || checkItem[i].Contains("新生兒"))
                {
                    if (ids2check[i].Length == 0)
                    {
                        MessageBox.Show($"身分證欄不得為空! \n於: {checkItem[i]}-{ids2check[i]}", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }

            // 郵局資訊
            List<string> acc_list2Check = new List<string> { textBox_account_name.Text, textBox_account_divn.Text, textBox_account_number.Text };
            bool is_accOK = acc_list2Check.All(acc => acc != "");
            if (!is_accOK)
            {
                MessageBox.Show($"郵局帳戶資訊不完整!", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        // 檢查身分證
        public string ValideID(string ID, bool searching = true)
        {
            // 初始化回報資訊
            string report = "";
            bool isfind = false;

            // 讀取資料庫
            List<dataStruct> dataList = utilities.ReadDatabase(settingCtrl.data_basePath);

            // 在資料庫中尋找重複
            foreach (dataStruct data in dataList)
            {
                List<string> ids2check = new List<string>();
                if (searching)
                {
                    ids2check.AddRange(new string[] { data.query_id, data.apply_id, data.mate_id });
                }
                foreach (string id in data.newBorn_id)
                {
                    ids2check.Add(id);
                }

                isfind = ids2check.Any(c => c != "" && c == ID);

                // 當找到重複則中止並回報
                if (isfind)
                {
                    string dupinfo = $"申請人: {data.apply_name} - {data.apply_id} \n" +
                        $"配偶: {data.mate_name} - {data.mate_id} \n\n新生兒資訊:\n";

                    // 加入新生兒
                    for (int i = 0; i < data.newBorn_name.Count; i++)
                    {
                        dupinfo += $"({i + 1}) " + data.newBorn_name[i].ToString() + "-";
                        dupinfo += data.newBorn_id[i].ToString() + "\n";
                    }

                    report = $"重複登錄! 請檢查以下:\n流水號:{data.serial_num}\n身分證號:\n{dupinfo}";

                    return report;
                }
            }

            return report;
        }

        // 將資料寫入資料庫
        private void WriteDataToDatabase(List<dataStruct> dataList, bool showMessage = true)
        {
            string toWrite = "[\n";
            foreach (dataStruct data in dataList)
            {
                string dataStr = JsonConvert.SerializeObject(data, Formatting.Indented);
                toWrite += dataStr;
                toWrite += ",\n";
            }
            toWrite += "]";
            File.WriteAllText(settingCtrl.data_basePath, toWrite);
            if (showMessage)
            {
                MessageBox.Show("儲存成功", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // 儲存後處理GUI變化
        private void UpdateGUISaved(dataStruct NewInfomation,List<dataStruct> dataList)
        {
            // 更新流水號顯示
            string serial2show = NewInfomation.login_year.ToString() + "-" + (NewInfomation.serial_index + 1).ToString();
            if (isEditingPrevData)
            {
                dataStruct lastData = dataList[dataList.Count - 1];
                serial2show = lastData.login_year.ToString() + "-" + (lastData.serial_index + 1).ToString();
            }
            SerialNumLabel.Text = serial2show;
            firstLogTimeLabel.Text = "-";
            RecentEditTimeLabel.Text = "-";

            // Switch flags
            if (isEditingPrevData)
            {
                isEditingPrevData = false;
            }

            // 清空表單內容
            InitializeSomeInfo();

            // 切換至歷史紀錄
            CancelBtn.Enabled = false;

            tabControl.SelectedTab = tabControl.TabPages["historyPage"];

            // Highlight 那一行
            foreach (DataGridViewRow row in historyGridView.Rows)
            {
                if (row.Cells["Serial_num"].Value != null && row.Cells["Serial_num"].Value.ToString() == NewInfomation.serial_num)
                {
                    row.Selected = true;
                    break;
                }
            }
        }

        // 當儲存按鈕被按下時執行
        private void saveBtn_Click(object sender, EventArgs e)
        {
            // 讀取資料庫
            List<dataStruct> dataList = utilities.ReadDatabase(settingCtrl.data_basePath);

            // 如果讀取失敗則不會執行以下
            if (dataList is null){return;}

            // 從表單蒐集資訊
            dataStruct NewInfomation = GatherInfo();

            // 檢查身分證號碼是否已經登錄
            List<string> ids2check = new List<string> {
               NewInfomation.apply_id, NewInfomation.mate_id, NewInfomation.query_id
            };
            List<string> checkItem = new List<string> { "申請人", "配偶", "委託人" };

            int count = 1;
            foreach (string id in NewInfomation.newBorn_id)
            {
                ids2check.Add(id);
                checkItem.Add("新生兒" + count.ToString());
                count++;
            }

            // 驗證輸入資訊
            bool isValidated = ValidateInfomation(NewInfomation, ids2check, checkItem);
            if (!isValidated){return;}

            // 當在沒有在編輯模式時, 防止重複登入
            if (!isEditingPrevData)
            {
                // 驗證身分證是否重複
                bool isDuplicate = false;
                foreach (string id in ids2check)
                {
                    string dupReportStr = ValideID(id, searching: false);

                    if (dupReportStr.Length > 0)
                    {
                        MessageBox.Show($"資料已經登入\n\n{dupReportStr}");
                        isDuplicate = true;
                        break;
                    }
                }

                // 若重複登入則中斷
                if (isDuplicate){return;}

                // 將新資訊加入
                dataList.Add(NewInfomation);
            }

            // 當在正在編輯模式時, 取代原本的資料
            else
            {
                int replaceIndex = dataList.FindIndex(data => data.serial_num == NewInfomation.serial_num);
                dataList[replaceIndex] = NewInfomation;
            }

            // 將 JSON 寫入檔案
            WriteDataToDatabase(dataList);

            // 更新GUI
            UpdateGUISaved(NewInfomation, dataList);
        }

        // 清除按鈕
        private void ClearInfoBtn_Click(object sender, EventArgs e)
        {
            InitializeSomeInfo();
        }

        #endregion

        /** 修改資料功能**/
        #region Editing Data

        // 載入資訊到文字框
        private void LoadData2TextBox(dataStruct choosenRecords)
        {
            /** 載入資訊-系統 **/
            SerialNumLabel.Text = choosenRecords.serial_num;                            // 流水號
            firstLogTimeLabel.Text = choosenRecords.First_Login_Date;               // 初次登錄
            RecentEditTimeLabel.Text = choosenRecords.Recent_Edit_Date;         // 最近一次修改

            /** 載入資訊-申請人/配偶 **/
            textBox_apply_name.Text = choosenRecords.apply_name;                 // 申請人名字
            textBox_apply_IDnumber.Text = choosenRecords.apply_id;               // 申請人身分證
            textBox_mate_name.Text = choosenRecords.mate_name;                  // 配偶名字
            textBox_mate_IDnumber.Text = choosenRecords.mate_id;                // 配偶身分證
            textBox_query_name.Text = choosenRecords.query_name;                // 委託人姓名
            textBox_query_IDnumber.Text = choosenRecords.query_id;              // 委託人身分證
            query_relation.Text = choosenRecords.query_relation;                       // 委託人與產婦關係

            /** 載入地址 **/
            Rgis_City_combobox.Text = "---";
            Comm_City_combobox.Text = "---";
            Rgis_Country_combobox.Text = "---";
            Comm_Country_combobox.Text = "---";
            Rgis_Road_TextBox.Text = "";
            Comm_Road_TextBox.Text = "";
            regisAdressTb.Text = choosenRecords.regis_Adress;
            commAdressTb.Text = choosenRecords.comm_Adress;

            /** 加入郵局匯款資訊 **/
            textBox_account_name.Text = choosenRecords.account_name;            // 受款人
            accountID_tb.Text = choosenRecords.account_ID;                                    // 受款人身分證
            textBox_account_divn.Text = choosenRecords.account_div;                  // 局號
            textBox_account_number.Text = choosenRecords.account_number;   // 帳號
        }

        // 載入手機和新生兒
        private void LoadPhones2GroupBox(dataStruct choosenRecords)
        {
            // 加入按鈕功能的函式
            void BindFunction2Button(AddablePanel panel)
            {
                foreach (Button btn in panel.deleteButtons)
                {
                    btn.Click += Delete_panel_Btn_Click;
                }
            }

            /** 載入手機號碼-申請人 **/
            apply_phone_panels.LoadPhones(groupBoxInfluenced[GroupBoxID.Apply], choosenRecords.apply_phones);                     // 載入電話
            BindFunction2Button(apply_phone_panels);                                                                                                                                          // 加入按鈕功能

            /** 載入手機號碼-配偶 **/
            mate_phone_panels.LoadPhones(groupBoxInfluenced[GroupBoxID.Mate], choosenRecords.mate_phones);                       // 載入電話
            BindFunction2Button(mate_phone_panels);                                                                                                                                          // 加入按鈕功能

            /** 載入手機號碼-委託人 **/
            query_phone_panels.LoadPhones(groupBoxInfluenced[GroupBoxID.Query], choosenRecords.query_phones);                     // 載入電話
            BindFunction2Button(query_phone_panels);                                                                                                                                          // 加入按鈕功能

            /** 載入新生兒 **/
            List<newBornInfo> nbInfos = new List<newBornInfo>();
            int newBorn_num = choosenRecords.newBorn_name.Count;
            for (int i = 0; i < newBorn_num; i++)
            {
                nbInfos.Add(new newBornInfo(
                    choosenRecords.newBorn_name[i], choosenRecords.newBorn_id[i], choosenRecords.newbornBitrhDate[i]
                    ));
            }
            newBorn_panels.LoadnewBorns(groupBoxInfluenced[GroupBoxID.NewBorn], nbInfos);           // 加入新生兒
            BindFunction2Button(newBorn_panels);                                                                                                // 加入按鈕功能

            // 更新新生兒數
            nb_num_TextBOX.Text = newBorn_panels.panel_nums.ToString();
        }

        // 進入修改模式
        private void EnterEditMode(dataStruct choosenRecords)
        {
            // switch flags
            isEditingPrevData = true;

            // 初始化UI
            InitializeSomeInfo();

            // 將資料載入至文字框
            LoadData2TextBox(choosenRecords);

            // 載入手機
            LoadPhones2GroupBox(choosenRecords);

            // 切換頁面
            CancelBtn.Enabled = true;
            tabControl.SelectedTab = tabControl.TabPages["LogPage"];

            // Unckeck checkBoxes
            adressCheckControl.unCheckBox();
        }

        // 取消編輯按鈕
        private void CancelBtn_Click(object sender, EventArgs e)
        {
            // 讀取資料庫
            List<dataStruct> dataList = utilities.ReadDatabase(settingCtrl.data_basePath);

            if (isEditingPrevData)
            {
                isEditingPrevData = false;
                CancelBtn.Enabled = false;
                InitializeSomeInfo();

                // 計算流水號
                int inputSearialN = 0;
                int inputYear = 0;

                int lastSerialN = dataList[dataList.Count - 1].serial_index;
                int latest_year = dataList[dataList.Count - 1].login_year;

                // 當年分不一樣時, 改變流水號
                if (DateTime.Now.Year - 1911 != latest_year)
                {
                    lastSerialN = 0;
                }
                inputSearialN = lastSerialN + 1;
                inputYear = latest_year;
                string serialNum = inputYear + "-" + inputSearialN;
                SerialNumLabel.Text = serialNum;

                // 切換頁面
                tabControl.SelectedTab = tabControl.TabPages["historyPage"];
            }
        }

        #endregion

        /** 載入歷史資料功能 **/
        #region HistoryPage Functions

        // 禁用滾輪
        public void ComboBox_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true; // 禁用鼠标滚轮事件
        }

        // 初始化 DataGridView
        private void InitializeDataGridView()
        {
            // 添加按钮列
            DataGridViewButtonColumn btnCol1 = new DataGridViewButtonColumn();
            btnCol1.HeaderText = "";
            btnCol1.Name = "examDetailBTN";
            btnCol1.Text = "檢視";
            btnCol1.UseColumnTextForButtonValue = true;
            historyGridView.Columns.Add(btnCol1);

            DataGridViewButtonColumn btnCol2 = new DataGridViewButtonColumn();
            btnCol2.HeaderText = "";
            btnCol2.Name = "editDataBTN";
            btnCol2.Text = "修改";
            btnCol2.UseColumnTextForButtonValue = true;
            historyGridView.Columns.Add(btnCol2);

            DataGridViewButtonColumn btnCol3 = new DataGridViewButtonColumn();
            btnCol3.HeaderText = "";
            btnCol3.Name = "deleteDataBTN";
            btnCol3.Text = "刪除";
            btnCol3.UseColumnTextForButtonValue = true;
            historyGridView.Columns.Add(btnCol3);

            DataGridViewButtonColumn btnCol4 = new DataGridViewButtonColumn();
            btnCol4.HeaderText = "";
            btnCol4.Name = "ApprovedColunm";
            btnCol4.Text = "審核";
            btnCol4.UseColumnTextForButtonValue = true;
            historyGridView.Columns.Add(btnCol4);

            // 添加按钮列的点击事件
            historyGridView.CellClick += historyGridView_CellClick;
            historyGridView.MultiSelect = true;
            historyGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        // 檢視詳細資料
        private void ShowDetailInfo(dataStruct choosenRecords) 
        {
            // 新生兒
            string newBornInfoes = "";

            // 新生兒數量
            int newBornNum = choosenRecords.newBorn_name.Count;

            for (int i = 0; i < newBornNum; i++)
            {
                newBornInfoes += $"\n\t新生兒({i+1}): {choosenRecords.newBorn_name[i]}\t\t身分證: {choosenRecords.newBorn_id[i]}\t\t生日: {choosenRecords.newbornBitrhDate[i]}\n";
            }

            // 擷取資料
            string toShowDetail = $"\t流水號:{choosenRecords.serial_num}\t初次登錄: {choosenRecords.First_Login_Date}\t最近一次修改: {choosenRecords.Recent_Edit_Date}\n" +
                $"\n  *  =========================== 申請人 ===========================  *\n" +
                $"\n\t申請人(孕婦)姓名:    {choosenRecords.apply_name}\t\t申請人身分證:\t{choosenRecords.apply_id}\n" +
                $"\n\t申請人聯絡電話:    {string.Join(" , ", choosenRecords.apply_phones)}\n" +
                $"\n\t受款人:    {choosenRecords.account_name}    身分證:{choosenRecords.account_ID}     郵局帳號:    {choosenRecords.account_div}{choosenRecords.account_number}\n" +
                $"\n\t戶籍地址:    {choosenRecords.regis_Adress}\n" +
                $"\n\t通訊地址:    {choosenRecords.comm_Adress}\n" +
                $"\n  *  ============================ 配偶  ===========================  *\n" +
                $"\n\t配偶姓名:    {choosenRecords.mate_name}\t\t配偶身分證:\t{choosenRecords.mate_id}\n" +
                $"\n\t配偶聯絡電話:    {string.Join(" , ", choosenRecords.mate_phones)}\n" +
                $"\n  *  =========================== 委託人 ===========================  *\n" +
                $"\n\t委託人姓名:    {choosenRecords.query_name}    ({choosenRecords.query_relation})\t\t委託人身分證:\t{choosenRecords.query_id}\n" +
                $"\n\t委託人聯絡電話:    {string.Join(" , ", choosenRecords.query_phones)}\n" +
                $"\n  *  =========================== 新生兒 ===========================  *\n" + newBornInfoes;

            // 顯示詳細資料視窗
            DetailInfoForm detailInfoForm = new DetailInfoForm();
            detailInfoForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            RichTextBox richTextBox = detailInfoForm.getDetailBox();
            richTextBox.Text = toShowDetail;
            detailInfoForm.ShowDialog();
        }

        // 刪除一筆資料
        private void DeleteOneData(List<dataStruct> records, dataStruct choosenRecords) 
        {
            // 跳出對話視窗, 詢問是否真的要刪除
            DialogResult result = MessageBox.Show(
            "警告, 資料刪除後無法恢復, 請確認是否要刪除 ?",
            "系統提示",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // 尋找要移除的流水號
                    var itemToRemove = records.SingleOrDefault(item => item.serial_num == choosenRecords.serial_num);

                    // 移除
                    records.Remove(itemToRemove);

                    // 序列化回JSON
                    string updatedJson = JsonConvert.SerializeObject(records, Formatting.Indented);

                    // 保存回文件
                    File.WriteAllText(settingCtrl.data_basePath, updatedJson);

                    // 更新頁面
                    List<string> list2check = new List<string> { search_start_yy.Text, search_start_mm.Text, search_end_yy.Text, search_end_mm.Text };
                    bool isValidate = list2check.All(c => c.Length > 0);
                    if (isValidate)
                    {
                        LoadJsonToDataGridView(
                           startYear: Int32.Parse(search_start_yy.Text), startMonth: Int32.Parse(search_start_mm.Text),startDay: Int32.Parse(search_start_dd.Text),
                           endYear: Int32.Parse(search_end_yy.Text), endMonth: Int32.Parse(search_end_mm.Text),endDay: Int32.Parse(search_end_dd.Text)
                           );
                    }
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("流水號重複!請檢查資料庫", "錯誤訊息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw;
                }
            }
        }

        // 歷史紀錄表單功能按鈕
        private void historyGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // 如果點擊欄首, (row index = -1) 不要做任何事, 不然會出錯
            if (e.RowIndex < 0) { return; }

            // Highlight 選到的那一行
            historyGridView.Rows[e.RowIndex].Selected = true;

            // 讀取資料庫
            List<dataStruct> records = utilities.ReadDatabase(settingCtrl.data_basePath);

            // 取選中的那一筆資料
            string seleted_serial_num = historyGridView.Rows[e.RowIndex].Cells["Serial_num"].Value.ToString();
            int selected_index = records.FindIndex(r => r.serial_num == seleted_serial_num);
            dataStruct choosenRecords = records[selected_index];

            // 检查是否点击的是按钮列
            if (e.ColumnIndex == historyGridView.Columns["examDetailBTN"].Index)
            {
                // 顯示詳細資訊
                ShowDetailInfo(choosenRecords);
            }
            else if (e.ColumnIndex == historyGridView.Columns["editDataBTN"].Index)
            {
                // 進入資料編輯模式
                EnterEditMode(choosenRecords);
            }
            else if (e.ColumnIndex == historyGridView.Columns["deleteDataBTN"].Index)
            {
                // 刪除該筆資料
                DeleteOneData(records, choosenRecords);
            }
            else if ((e.ColumnIndex == historyGridView.Columns["ApprovedColunm"].Index))
            {
                SensorCase(records, choosenRecords);
            }
        }

        // 頁簽切換日期ComboBox設定, 以及更新DataGridView內容
        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl tabControl = sender as TabControl;

            // 受影響之ComboBox
            List<ComboBox> influencedBox = new List<ComboBox>
            {
                search_start_yy, search_start_mm, search_start_dd, search_end_yy, search_end_mm, search_end_dd
            };

            if (tabControl != null)
            {
                // 检查是否切换到特定的页签
                if (tabControl.SelectedTab.Name == "historyPage")
                {
                    // 當切換至歷史紀錄頁面時新增事件
                    foreach (ComboBox cb in influencedBox)
                    {
                        cb.SelectedIndexChanged += SearchDate_ComboBox_SelectedIndexChanged;
                        cb.TextChanged += SearchDate_ComboBox_SelectedIndexChanged;
                    }

                    // 顯示更新的歷史資料
                    List<string> list2check = new List<string> { search_start_yy.Text, search_start_mm.Text, search_end_yy.Text, search_end_mm.Text };
                    bool isValidate = list2check.All(c => c.Length > 0);
                    if (isValidate)
                    {
                        LoadJsonToDataGridView(
                           startYear: Int32.Parse(search_start_yy.Text), startMonth: Int32.Parse(search_start_mm.Text), startDay: Int32.Parse(search_start_dd.Text),
                           endYear: Int32.Parse(search_end_yy.Text), endMonth: Int32.Parse(search_end_mm.Text), endDay: Int32.Parse(search_end_dd.Text)
                           );
                    }
                }
                else
                {
                    // 當切換至別的頁面時移除事件
                    foreach (ComboBox cb in influencedBox)
                    {
                        cb.SelectedIndexChanged -= SearchDate_ComboBox_SelectedIndexChanged;
                        cb.TextChanged -= SearchDate_ComboBox_SelectedIndexChanged;
                    }
                }

                if (tabControl.SelectedTab.Name == "settingsPage")
                {
                    allowanceSetting_TB.Text = settingCtrl.allowance_per_nb.ToString();
                    dataBAsePath_setTB.Text = settingCtrl.data_basePath;
                    remitFormatTB1.Text = settingCtrl.remit_format_sub1;
                    remitFormatTB2.Text = settingCtrl.SpaceNum.ToString();
                    remitFormatTB3.Text = settingCtrl.remit_format_sub2;
                    remitFormatTB4.Text = settingCtrl.remit_format_sub3;
                }
            }
        }

        #endregion

        /** 歷史資料篩選功能 **/
        #region DataFiltering Functions

        // 處理資料篩選(日期)
        private List<string> DateFiltering(int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay, List<dataStruct> Recs)
        {
            List<string> target_serial_num = new List<string>();

            //  篩選日期
            if (startYear == endYear)
            {
                foreach (dataStruct data in Recs)
                {
                    if (data.login_year == startYear)
                    {
                        if (startMonth == endMonth)
                        {
                            if (data.login_month == startMonth)
                            {
                                if (data.login_day >= startDay && data.login_day <= endDay)
                                {
                                    target_serial_num.Add(data.serial_num);
                                }
                            }
                        }
                        else if (startMonth < endMonth)
                        {
                            if (data.login_month > startMonth && data.login_month < endMonth)
                            {
                                target_serial_num.Add(data.serial_num);
                            }
                            else if (data.login_month == startMonth)
                            {
                                if (data.login_day >= startDay)
                                {
                                    target_serial_num.Add(data.serial_num);
                                }
                            }
                            else if (data.login_month == endMonth)
                            {
                                if (data.login_day <= endDay)
                                {
                                    target_serial_num.Add(data.serial_num);
                                }
                            }
                        }
                    }
                }
            }
            else if (startYear < endYear)
            {
                foreach (dataStruct data in Recs)
                {
                    if (data.login_year > startYear && data.login_year < endYear)
                    {
                        target_serial_num.Add(data.serial_num);
                    }
                    else if (data.login_year == startYear)
                    {
                        if (data.login_month > startMonth)
                        {
                            target_serial_num.Add(data.serial_num);
                        }
                        else if (data.login_month == startMonth)
                        {
                            if (data.login_day >= startDay)
                            {
                                target_serial_num.Add(data.serial_num);
                            }
                        }
                    }
                    else if (data.login_year == endYear)
                    {
                        if (data.login_month < endMonth)
                        {
                            target_serial_num.Add(data.serial_num);
                        }
                        else if (data.login_month == endMonth)
                        {
                            if (data.login_day <= endDay)
                            {
                                target_serial_num.Add(data.serial_num);
                            }
                        }
                    }
                }
            }

            return target_serial_num;
        }

        // 加入資料標題
        private void UpdateHistoryGridView(DataGridView dataGridView, List<dataStruct> data)
        {
            // Apply data
            List<ExportRecord> display = data.Select(
                record => new ExportRecord
                {
                    SensorRes = record.sensor_result,
                    Serial_num = record.serial_num,
                    firstLoginDate = record.First_Login_Date,
                    recentEditDate = record.Recent_Edit_Date,
                    remitDate = record.remit_date,
                    apply_name = record.apply_name,
                    apply_id = record.apply_id,
                    newBornID = string.Join(",", record.newBorn_id),
                    newBornNames = string.Join(",", record.newBorn_name),
                    account_name = record.account_name,
                    accound_ID = record.account_ID,
                    account_div = record.account_div,
                    account_num = record.account_number,
                    allowance = record.newBorn_name.Count * settingCtrl.allowance_per_nb,
                    note = record.notes
                }).ToList();

            // 将数据绑定到 DataGridView
            dataGridView.DataSource = display;

            // 自定義欄位名稱
            dataGridView.Columns["SensorRes"].HeaderText = "審核結果";
            dataGridView.Columns["Serial_num"].HeaderText = "流水號";
            dataGridView.Columns["firstLoginDate"].HeaderText = "初次登錄";
            dataGridView.Columns["recentEditDate"].HeaderText = "最近修改";
            dataGridView.Columns["remitDate"].HeaderText = "匯款日期";
            dataGridView.Columns["apply_name"].HeaderText = "申請人(孕婦)";
            dataGridView.Columns["apply_id"].HeaderText = "申請人身分證";
            dataGridView.Columns["newBornID"].HeaderText = "新生兒身分證";
            dataGridView.Columns["newBornNames"].HeaderText = "新生兒名字";
            dataGridView.Columns["account_name"].HeaderText = "受款人";
            dataGridView.Columns["accound_ID"].HeaderText = "受款人身分證";
            dataGridView.Columns["account_div"].HeaderText = "郵局局號";
            dataGridView.Columns["account_num"].HeaderText = "郵局帳號";
            dataGridView.Columns["allowance"].HeaderText = "補助金額";
            dataGridView.Columns["note"].HeaderText = "備註";

            // 漆上審核顏色
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                string sensor_res_str = row.Cells["SensorRes"].Value.ToString();

                switch (sensor_res_str)
                {
                    case SensorForm.STR_APPROVED:
                        row.Cells["SensorRes"].Style.ForeColor = Color.DarkGreen;
                        break;

                    case SensorForm.STR_NOT_SENSRO:
                        row.Cells["SensorRes"].Style.ForeColor = Color.DarkBlue;
                        break;
                }

                if (sensor_res_str.Contains(SensorForm.STR_DISAPPROVED))
                {
                    row.Cells["SensorRes"].Style.ForeColor = Color.Maroon;
                }

                string remitDate_str = row.Cells["remitDate"].Value.ToString();
                if (remitDate_str == "尚未匯款")
                {
                    row.Cells["remitDate"].Style.ForeColor = Color.DarkBlue;
                }
                else
                {
                    row.Cells["remitDate"].Style.ForeColor = Color.DarkCyan;
                }
            }
        }

        // 處理搜索日期改變的事件
        public void SearchDate_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox target = (ComboBox)sender;

            // 登錄日期
            List<ComboBox> search_date_cb = new List<ComboBox> {
                search_start_yy, search_start_mm, search_start_dd, search_end_yy, search_end_mm, search_end_dd
            };

            if (search_date_cb.Contains(target))
            {
                // 顯示更新的歷史資料
                List<string> list2check = new List<string> { 
                    search_start_yy.Text, search_start_mm.Text, search_start_dd.Text, search_end_yy.Text, search_end_mm.Text, search_end_dd.Text
                };
                bool isValidate = list2check.All(c => c.Length > 0);
                if (isValidate)
                {

                    if (Int32.Parse(search_start_yy.Text) > Int32.Parse(search_end_yy.Text))
                    {
                        search_end_yy.Text = (Int32.Parse(search_start_yy.Text)).ToString();
                        search_end_mm.Text = 1.ToString();
                        search_end_dd.Text = 15.ToString();
                    }
                    else if (Int32.Parse(search_start_yy.Text) == Int32.Parse(search_end_yy.Text) && Int32.Parse(search_start_mm.Text) > Int32.Parse(search_end_mm.Text))
                    {
                        search_end_mm.Text = (Int32.Parse(search_start_mm.Text)+1).ToString();
                        search_end_dd.Text = 1.ToString();
                    }
                    else if (Int32.Parse(search_start_yy.Text) == Int32.Parse(search_end_yy.Text) && Int32.Parse(search_start_mm.Text) == Int32.Parse(search_end_mm.Text) 
                        && Int32.Parse(search_start_dd.Text) > Int32.Parse(search_end_dd.Text))
                    {
                        search_end_mm.Text = (Int32.Parse(search_start_mm.Text)).ToString();
                        search_end_dd.Text = 31.ToString();
                    }

                    LoadJsonToDataGridView(
                       startYear: Int32.Parse(search_start_yy.Text), startMonth: Int32.Parse(search_start_mm.Text), startDay: Int32.Parse(search_start_dd.Text),
                       endYear: Int32.Parse(search_end_yy.Text), endMonth: Int32.Parse(search_end_mm.Text), endDay: Int32.Parse(search_end_dd.Text)
                       );

                }
            }
        }

        // 載入顯示資訊到DataGridView
        private void LoadJsonToDataGridView(int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay)
        {
            if (File.Exists(settingCtrl.data_basePath))
            {
                try
                {
                    // 反序列化 JSON 内容為對象列表
                    List<dataStruct> records = utilities.ReadDatabase(settingCtrl.data_basePath);

                    // 篩選日期
                    List<string> target_serial_num = DateFiltering(startYear, startMonth, startDay, endYear, endMonth, endDay, records);

                    // 將數組轉換為字串 (依照日期篩選)
                    List<dataStruct> displayRecords = records.Where(record => target_serial_num.Contains(record.serial_num)).ToList();

                    // 應用變更
                    UpdateHistoryGridView(historyGridView, displayRecords);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading JSON file: {ex.StackTrace}","錯誤",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show($"File not found: {settingCtrl.data_basePath}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 搜尋身分證的按鈕功能
        private void IDsearchBtn_Click(object sender, EventArgs e)
        {
            // 載入資料庫
            List<dataStruct> records = utilities.ReadDatabase(settingCtrl.data_basePath);

           // ID BOX (使用者輸入)
           string idText = IDsearchBOX.Text.ToString();

            // 如果沒有輸入則提示
            if (idText.Length == 0)
            {
                MessageBox.Show("請輸入身分證", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 將數組轉換為字串
            List<dataStruct> displayRecords = records.Where(record => record.apply_id == idText || record.query_id == idText || record.newBorn_id.Contains(idText)).ToList();
        
            // 應用變更
            UpdateHistoryGridView(historyGridView, displayRecords);
        }

        // 清除搜尋按鈕功能
        private void clearSearchBtn_Click(object sender, EventArgs e)
        {
            IDsearchBOX.Clear();
            List<string> list2check = new List<string> { search_start_yy.Text, search_start_mm.Text, search_end_yy.Text, search_end_mm.Text };
            bool isValidate = list2check.All(c => c.Length>0);
            if (isValidate)
            {
                LoadJsonToDataGridView(
                       startYear: Int32.Parse(search_start_yy.Text), startMonth: Int32.Parse(search_start_mm.Text), startDay: Int32.Parse(search_start_dd.Text),
                       endYear: Int32.Parse(search_end_yy.Text), endMonth: Int32.Parse(search_end_mm.Text), endDay: Int32.Parse(search_end_dd.Text)
                       );
            }
        }

        #endregion

        /** 審核功能**/
        #region Sensor Functions

        public void SensorCase(List<dataStruct> records, dataStruct case2Sensor)
        {
            // 尋找要審核的流水號
            dataStruct caseData = records.SingleOrDefault(item => item.serial_num == case2Sensor.serial_num);

            // 跳出視窗
            SensorForm sensorForm = new SensorForm();
            sensorForm.ShowDialog();

            // 審核結果 (初始化)
            string sensor_result = sensorForm.SensorResMatrix[SensorForm.ButtonID.NULL];

            // 點擊選項
            SensorForm.ButtonID selection = sensorForm.clickTarget;

            switch (selection)
            {
                case SensorForm.ButtonID.Approved:
                    sensor_result = sensorForm.SensorResMatrix[selection];
                    caseData.sensor_result = sensor_result;
                    break;
                case SensorForm.ButtonID.Denied:
                    // 填寫不符合事由
                    ReasomForm reason = new ReasomForm();
                    reason.ShowDialog();
                    string reasonStr = reason.ReasonStr;
                    if (reasonStr.Count() > 0)
                    {
                        sensor_result = $"{sensorForm.SensorResMatrix[selection]}({reasonStr})";
                    }
                    else
                    {
                        MessageBox.Show("請輸入事由!", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        sensor_result = sensorForm.SensorResMatrix[SensorForm.ButtonID.NULL];
                    }
                    caseData.sensor_result = sensor_result;
                    break;
                case SensorForm.ButtonID.NULL:
                    sensor_result = sensorForm.SensorResMatrix[selection];
                    caseData.sensor_result = sensor_result;
                    break;
                case SensorForm.ButtonID.Cancel:
                    return;
            }

            // 取代原本的
            int replaceIndex = records.FindIndex(data => data.serial_num == caseData.serial_num);
            records[replaceIndex] = caseData;

            // 將 JSON 寫入檔案
            WriteDataToDatabase(records, false);

            // 更新表格顯示
            LoadJsonToDataGridView(
                           startYear: Int32.Parse(search_start_yy.Text), startMonth: Int32.Parse(search_start_mm.Text), startDay: Int32.Parse(search_start_dd.Text),
                           endYear: Int32.Parse(search_end_yy.Text), endMonth: Int32.Parse(search_end_mm.Text), endDay: Int32.Parse(search_end_dd.Text)
                           );
        }

        public void SensorBatch(List<dataStruct> records)
        {
            // 跳出視窗
            SensorForm sensorForm = new SensorForm();
            sensorForm.ShowDialog();

            // 審核結果 (初始化)
            string sensor_result = sensorForm.SensorResMatrix[SensorForm.ButtonID.NULL];

            // 點擊選項
            SensorForm.ButtonID selection = sensorForm.clickTarget;

            switch (selection)
            {
                case SensorForm.ButtonID.Approved:
                    sensor_result = sensorForm.SensorResMatrix[selection];
                    break;
                case SensorForm.ButtonID.Denied:
                    // 填寫不符合事由
                    ReasomForm reason = new ReasomForm();
                    reason.ShowDialog();
                    string reasonStr = reason.ReasonStr;
                    if (reasonStr.Count() > 0)
                    {
                        sensor_result = $"{sensorForm.SensorResMatrix[selection]}({reasonStr})";
                    }
                    else
                    {
                        MessageBox.Show("請輸入事由!","系統提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                        sensor_result = sensorForm.SensorResMatrix[SensorForm.ButtonID.NULL];
                    }
                    break;
                case SensorForm.ButtonID.NULL:
                    sensor_result = sensorForm.SensorResMatrix[selection];
                    break;
                case SensorForm.ButtonID.Cancel:
                    return;
            }

            // 獲取所有被選中的行的索引
            List<string> selectedRowIndexes = new List<string>();
            foreach (DataGridViewRow row in historyGridView.SelectedRows)
            {
                selectedRowIndexes.Add(row.Cells["Serial_num"].Value.ToString());
            }

            // 審核全部選中的
            foreach (string sern in selectedRowIndexes)
            {
                // 取得INDEX
                int replaceIndex = records.FindIndex(data => data.serial_num == sern);

                // 取得該筆資料
                dataStruct caseData = records[replaceIndex];

                // 變更審核結果
                caseData.sensor_result = sensor_result;

                // 取代原本的
                records[replaceIndex] = caseData;

                // 將 JSON 寫入檔案
                WriteDataToDatabase(records,false);

                // 更新表格顯示
                LoadJsonToDataGridView(
                               startYear: Int32.Parse(search_start_yy.Text), startMonth: Int32.Parse(search_start_mm.Text), startDay: Int32.Parse(search_start_dd.Text),
                               endYear: Int32.Parse(search_end_yy.Text), endMonth: Int32.Parse(search_end_mm.Text), endDay: Int32.Parse(search_end_dd.Text)
                               );
            }

            
        }

        // 批次審核
        private void batchSensorBtn_Click(object sender, EventArgs e)
        {
            // 載入資料庫
            List<dataStruct> records = utilities.ReadDatabase(settingCtrl.data_basePath);

            // 執行審核
            SensorBatch(records);
        }

        #endregion

        /** 輸出功能 **/
        #region Export Functions

        // 輸出Excel的功能
        private void exportExcelBtn_Click(object sender, EventArgs e)
        {
            // 讀取資料庫
            List<dataStruct> dataList = utilities.ReadDatabase(settingCtrl.data_basePath);

            ComfirmExportDateForm comfirmExportDateForm = new ComfirmExportDateForm();
            comfirmExportDateForm.ShowDialog();

            string selection = comfirmExportDateForm.export_choice;
            List<string> select_serial_nums = new List<string>();

            switch (selection)
            {
                case ComfirmExportDateForm.ALL_RANGE:
                    foreach (var item in dataList)
                    {
                        select_serial_nums.Add(item.serial_num);
                    }
                    break;

                case ComfirmExportDateForm.FILTER_DATE:
                    foreach (DataGridViewRow row in historyGridView.Rows)
                    {
                        select_serial_nums.Add(row.Cells["Serial_num"].Value.ToString());
                    }
                    break;

                case ComfirmExportDateForm.DEFAULT:
                    return;
            }

            string excelFilePath = ShowSaveFileDialog("xlsx","歷史紀錄");

            if (excelFilePath != null && select_serial_nums.Count > 0)
            {
                utilities.exportExcel(settingCtrl.data_basePath, excelFilePath, select_serial_nums,settingCtrl.allowance_per_nb);
            }
            
        }

        // 存檔對話視窗
        private static string ShowSaveFileDialog(string format, string appendInfo = "")
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                switch (format)
                {
                    case "xlsx":
                        saveFileDialog.Filter = "Excel Files|*.xlsx";
                        saveFileDialog.Title = "Save an Excel File";
                        break;
                    case "pdf":
                        saveFileDialog.Filter = "PDF Files|*.pdf";
                        saveFileDialog.Title = "Save an PDF File";
                        break;
                }

                DateTime now = DateTime.Now;
                int adjYear = now.Year - 1911;
                string currentTime = $"{adjYear}-{now.Month.ToString()}-{now.Day.ToString()}";

                saveFileDialog.FileName = $"坐月子津貼{appendInfo}_{currentTime}.{format}";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return saveFileDialog.FileName;
                }
            }

            return null;
        }

        // 輸出PDF的功能
        private void exportPDFBtn_Click(object sender, EventArgs e)
        {
            PDFmaker pdfMaker  = new PDFmaker(settingCtrl);

            // 讀取資料庫
            List<dataStruct> dataList = utilities.ReadDatabase(settingCtrl.data_basePath);

            // 跳出確認視窗
            ComfirmExportDateForm comfirmExportDateForm = new ComfirmExportDateForm();
            comfirmExportDateForm.ShowDialog();

            string selection = comfirmExportDateForm.export_choice;
            List<string> select_serial_nums = new List<string>();

            switch (selection)
            {
                case ComfirmExportDateForm.ALL_RANGE:
                    foreach (var item in dataList)
                    {
                        if (item.sensor_result == SensorForm.STR_APPROVED)
                        {
                            select_serial_nums.Add(item.serial_num);
                        }
                    }
                    break;

                case ComfirmExportDateForm.FILTER_DATE:
                    foreach (DataGridViewRow row in historyGridView.Rows)
                    {
                        if (row.Cells["SensorRes"].Value.ToString() == SensorForm.STR_APPROVED)
                        {
                            select_serial_nums.Add(row.Cells["Serial_num"].Value.ToString());
                        }
                    }
                    break;

                case ComfirmExportDateForm.DEFAULT:
                    return;
            }

            // 假設 selected_ser_nums 是空 (沒有任何審核通過的) 則提示, 不執行
            if (select_serial_nums.Count == 0)
            {
                MessageBox.Show("沒有資料/沒有審核通過的資料!\n請確認審核結果!", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string PDFFilePath = ShowSaveFileDialog("pdf","清冊");

            if (PDFFilePath != null && select_serial_nums.Count > 0)
            {
                pdfMaker.GeneratePDF(PDFFilePath, dataList, select_serial_nums);
            }
        }

        private string ShowDateComfirmDialog(bool disableUNRemit = false)
        {
            string DateStr = "";
            
            DateComfirmForm dateComfirm = new DateComfirmForm();
            if (disableUNRemit)
            {
                dateComfirm.DisableNOTRemitBtn();
            }
            dateComfirm.ShowDialog();

            DateComfirmForm.Options selection = dateComfirm.comfirm;

            switch (selection)
            {
                case DateComfirmForm.Options.Comfirm:
                    DateStr = dateComfirm.GetDate();
                    break;

                case DateComfirmForm.Options.NOT_REMIT:
                    DateStr = "尚未匯款";
                    break;
            }

            return DateStr;
        }

        private void exportRemitBTN_Click(object sender, EventArgs e)
        {
            string FormatNumber(int number)
            {
                // 在數字後面加上兩個0，然後轉為字串
                string formattedNumber = (number * 100).ToString();

                // 在數字前補0，使其總長度為10位
                return formattedNumber.PadLeft(10, '0');
            }

            // 讀取資料庫
            List<dataStruct> dataList = utilities.ReadDatabase(settingCtrl.data_basePath);

            // 確認輸出範圍
            ComfirmExportDateForm comfirmExportDateForm = new ComfirmExportDateForm();
            comfirmExportDateForm.ShowDialog();
            string selection = comfirmExportDateForm.export_choice;
            List<string> select_serial_nums = new List<string>();
            switch (selection)
            {
                case ComfirmExportDateForm.ALL_RANGE:
                    foreach (var item in dataList)
                    {
                        if (item.remit_date=="尚未匯款" && item.sensor_result == SensorForm.STR_APPROVED)
                        {
                            select_serial_nums.Add(item.serial_num);
                        }
                    }
                    break;

                case ComfirmExportDateForm.FILTER_DATE:
                    foreach (DataGridViewRow row in historyGridView.Rows)
                    {
                        if (row.Cells["remitDate"].Value.ToString() == "尚未匯款" && row.Cells["SensorRes"].Value.ToString() == SensorForm.STR_APPROVED)
                        {
                            select_serial_nums.Add(row.Cells["Serial_num"].Value.ToString());
                        }
                    }
                    break;

                case ComfirmExportDateForm.DEFAULT:
                    return;
            }

            // 假設 selected_ser_nums 是空 (沒有任何審核通過的) 則提示, 不執行
            if (select_serial_nums.Count == 0)
            {
                MessageBox.Show("沒有資料/沒有審核通過的資料!\n請確認!", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 得到存檔路徑
            string txtFilePath = ShowSaveFileDialog("txt", "轉帳資訊檔案");

            // 確認轉帳日期
            string remitDate = ShowDateComfirmDialog(true);

            if (remitDate.Count() > 0 && txtFilePath != null && select_serial_nums.Count > 0)
            {
                // 格式
                string toWrite = "";

                for (int i = 0; i < dataList.Count; i++)
                {
                    if (select_serial_nums.Contains(dataList[i].serial_num))
                    {
                        // 取得該筆資料
                        dataStruct caseData = dataList[i];

                        // 變更審核結果
                        caseData.remit_date = remitDate;

                        // 取代原本的
                        dataList[i] = caseData;

                        // 將 JSON 寫入檔案
                        WriteDataToDatabase(dataList, false);

                        // 更新表格顯示
                        LoadJsonToDataGridView(
                                       startYear: Int32.Parse(search_start_yy.Text), startMonth: Int32.Parse(search_start_mm.Text), startDay: Int32.Parse(search_start_dd.Text),
                                       endYear: Int32.Parse(search_end_yy.Text), endMonth: Int32.Parse(search_end_mm.Text), endDay: Int32.Parse(search_end_dd.Text)
                                       );

                        // 固定格式
                        string F1 = settingCtrl.remit_format_sub1;
                        string Sp = string.Join("",Enumerable.Repeat(" ", settingCtrl.SpaceNum));
                        string F2 = settingCtrl.remit_format_sub2;
                        string F3 = settingCtrl.remit_format_sub3;

                        // 日期
                        string reDateYY = remitDate.Split('年')[0];
                        string subDate = remitDate.Split('年')[1];
                        int reDateMMi = Int32.Parse(subDate.Split('月')[0]);
                        string reDateMM = reDateMMi.ToString();
                        if (reDateMMi < 10)
                        {
                            reDateMM = "0" + reDateMMi.ToString();
                        }
                        string subsubDate = subDate.Split('月')[1];
                        int reDateDDi = Int32.Parse(subsubDate.Split('日')[0]);
                        string reDateDD = reDateDDi.ToString();
                        if (reDateDDi < 10)
                        {
                            reDateMM = "0" + reDateMMi.ToString();
                        }

                        // 金額
                        int allowance_amount = settingCtrl.allowance_per_nb * caseData.newBorn_name.Count();
                        string allowanceStr = FormatNumber(allowance_amount);
                        toWrite += $"{F1}{Sp}{F2}{reDateYY}{reDateMM}{reDateDD}{caseData.account_div}{caseData.account_number}{caseData.account_ID}{allowanceStr}{F3}\n";
                    }
                }

                // 輸出
                File.WriteAllText(txtFilePath, toWrite);

                // 提示
                MessageBox.Show("輸出完成", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        private enum SettingsOption
        {
            Allowance_per_NB, Database_path, Remit_Format
        }

        private void UPdateSettings(SettingsOption toChange, string[] values)
        {
            // Load setting
            string settingContent = File.ReadAllText(ConstParameters.settingsPath);
            SettingStruct currentSettings = JsonConvert.DeserializeObject<SettingStruct>(settingContent);

            // New settings
            SettingStruct newSettings = new SettingStruct()
            {
                Allowance_per_new_born = currentSettings.Allowance_per_new_born,
                Database_path = currentSettings.Database_path,
                District_db_pth = currentSettings.District_db_pth,
                RemitFormat1 = currentSettings.RemitFormat1,
                RemitFormat2 = currentSettings.RemitFormat2,
                RemitFormat3 = currentSettings.RemitFormat3,
                SpaceNumber = currentSettings.SpaceNumber
            };

            switch (toChange)
            {
                case SettingsOption.Allowance_per_NB:
                    newSettings.Allowance_per_new_born = Int32.Parse(values[0]);
                    break;
                case SettingsOption.Database_path:
                    newSettings.Database_path = values[0];
                    break;
                case SettingsOption.Remit_Format:
                    newSettings.RemitFormat1 = values[0];
                    newSettings.SpaceNumber = Int32.Parse(values[1]);
                    newSettings.RemitFormat2 = values[2];
                    newSettings.RemitFormat3 = values[3];
                    break;
                default:
                    break;
            }

            string toWrite = JsonConvert.SerializeObject(newSettings, Formatting.Indented);
            File.WriteAllText(ConstParameters.settingsPath, toWrite);
            settingCtrl.ApplySettings();
        }

        private void BatchChangeRemitDate(List<dataStruct> records)
        {
            string selectedRemitDate = ShowDateComfirmDialog();

            if (selectedRemitDate.Count() == 0)
            {
                return;
            }

            // 獲取所有被選中的行的索引
            List<string> selectedRowIndexes = new List<string>();
            foreach (DataGridViewRow row in historyGridView.SelectedRows)
            {
                selectedRowIndexes.Add(row.Cells["Serial_num"].Value.ToString());
            }

            // 審核全部選中的
            foreach (string sern in selectedRowIndexes)
            {
                // 取得INDEX
                int replaceIndex = records.FindIndex(data => data.serial_num == sern);

                // 取得該筆資料
                dataStruct caseData = records[replaceIndex];

                // 變更審核結果
                caseData.remit_date = selectedRemitDate;

                // 取代原本的
                records[replaceIndex] = caseData;

                // 將 JSON 寫入檔案
                WriteDataToDatabase(records, false);

                // 更新表格顯示
                LoadJsonToDataGridView(
                               startYear: Int32.Parse(search_start_yy.Text), startMonth: Int32.Parse(search_start_mm.Text), startDay: Int32.Parse(search_start_dd.Text),
                               endYear: Int32.Parse(search_end_yy.Text), endMonth: Int32.Parse(search_end_mm.Text), endDay: Int32.Parse(search_end_dd.Text)
                               );
            }
        }

        private void changeSettingsBtn_Click(object sender, EventArgs e)
        {
            const string CHANGE = "變更";
            const string COMFIRM = "確定";

            Button ChangeBtn = (Button)sender;
            string BtnName = ChangeBtn.Name;

            switch (BtnName)
            {
                case "changeAllowanceBtn": 
                    if (ChangeBtn.Text == CHANGE)
                    {
                        allowanceSetting_TB.Enabled = true;
                        ChangeBtn.Text = COMFIRM;
                        ChangeBtn.ForeColor = Color.DarkGreen;
                    }
                    else if (ChangeBtn.Text == COMFIRM)
                    {
                        allowanceSetting_TB.Enabled = false;
                        ChangeBtn.Text = CHANGE;
                        ChangeBtn.ForeColor = Color.Black;
                        settingCtrl.SetAllowancePerNB(Int32.Parse(allowanceSetting_TB.Text));
                    }

                    // 更新設定
                    UPdateSettings(SettingsOption.Allowance_per_NB, new string[] { allowanceSetting_TB.Text });
                    break;

                case "change_databasePthBtn":
                    using (OpenFileDialog chooseFileDialog = new OpenFileDialog())
                    {
                        chooseFileDialog.FileName = "";
                        chooseFileDialog.Filter = "Json Files|*.json";
                        chooseFileDialog.Title = "Load an Database File";
                        if (chooseFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            dataBAsePath_setTB.Text = chooseFileDialog.FileName;
                        }
                        settingCtrl.SetDatabasePath(dataBAsePath_setTB.Text);
                    }

                    // 更新流水號
                    List<dataStruct> newData = utilities.ReadDatabase(settingCtrl.data_basePath);
                    dataStruct lastData = newData[newData.Count - 1];

                    int lastLoginYear = lastData.login_year;
                    int last_serial_idx = lastData.serial_index;
                    int updateYear = DateTime.Now.Year - 1911;
                    int updateSerialIDX = last_serial_idx + 1;
                    if (lastLoginYear != updateYear)
                    {
                        updateSerialIDX = 0;
                    }
                    SerialNumLabel.Text = $"{updateYear}-{updateSerialIDX}";

                    // 更新設定
                    UPdateSettings(SettingsOption.Database_path, new string[] { dataBAsePath_setTB.Text });
                    break;

                case "change_remitFormatBtn":
                    if (ChangeBtn.Text == CHANGE)
                    {
                        foreach (Control ctrl in settings_panel.Controls)
                        {
                            ctrl.Enabled = true;
                        }
                        ChangeBtn.Text = COMFIRM;
                        ChangeBtn.ForeColor = Color.DarkGreen;
                    }
                    else if (ChangeBtn.Text == COMFIRM)
                    {
                        foreach (Control ctrl in settings_panel.Controls)
                        {
                            ctrl.Enabled = false;
                        }
                        ChangeBtn.Text = CHANGE;
                        ChangeBtn.ForeColor = Color.Black;

                        // 更新 settingCtrl
                        settingCtrl.SetRemitFormat(remitFormatTB1.Text, Int32.Parse(remitFormatTB2.Text), remitFormatTB3.Text, remitFormatTB4.Text);

                        // 更新設定
                        UPdateSettings(SettingsOption.Remit_Format, new string[]
                        {
                            remitFormatTB1.Text, remitFormatTB2.Text, remitFormatTB3.Text, remitFormatTB4.Text
                        });
                    }
                    break;
            }

        }

        private void RemitDateBtn_Click(object sender, EventArgs e)
        {
            List<dataStruct> records = utilities.ReadDatabase(settingCtrl.data_basePath);
            BatchChangeRemitDate(records);
        }
    }
}


