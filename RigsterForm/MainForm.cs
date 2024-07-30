using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RigsterForm
{
    public partial class MainForm : Form
    {
        /** Global variables **/
        #region Global_variables

        /* Database file path */
        public const string database_path = "Database.json";               // 申請資訊
        public const string district_db_path = "Taiwan_Districts.json";  //  地址列表

        /* 地址預設 */
        private const string InitialCity = "彰化縣";         // 初始化選擇縣市
        private const string InitialCountry = "和美鎮";  // 初始化選擇鄉鎮

        /* 儲存 Flags */
        public bool isEditingPrevData;          // 檢查是否在編輯舊資料中

        #endregion

        /** 組件 **/
        #region Components

        /* 使用地址選擇器 */
        private AdressPicker adressPicker;

        /* 使用自製函式庫 */
        private Utilities utilities;

        /* 儲存可新增式Panel */
        public AddablePanel apply_phone_panels;
        public AddablePanel mate_phone_panels;
        public AddablePanel query_phone_panels;
        public newBornPanel newBorn_panels;

        /* 儲存群組資訊 */
        private enum GroupBoxID  { Apply, Mate, Query, Account, NewBorn, ButtonPanel }       // 群組名稱
        private Dictionary<GroupBoxID, List<Control>> groupBoxInfluenced;                           // 組合被影響群組名稱和ID
        private Dictionary<GroupBoxID, string> groupBoxIDNameMatrix;                                  // 組合群組名稱與ID

        #endregion

        // 雜七雜八
        public string dupSerialNum;
        public string dupID;
        public string selected_serial_num;
        
        /** Initialization Functions **/
        #region Initialization_Functions

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

        // 初始化地址選項
        private void InitializeAdressPicker(ComboBox cityCB, ComboBox countryCB, string default_city, string default_country) 
        {
            adressPicker.LoadCityList(cityCB);
            adressPicker.SetDefaultValue(cityCB, default_city);
            adressPicker.LoadCountryList(countryCB, default_city);
            adressPicker.SetDefaultValue(countryCB, default_country);
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
            // nb_num_TextBOX.Text = newBorn_panels.panel_nums.ToString();       // 新生兒數量顯示

            /* 初始化委託人參考選項Combo box */
            queryRef_comboBox.Items.Clear();
            queryRef_comboBox.Items.Add("同申請人");
            queryRef_comboBox.Items.Add("同配偶");
            queryRef_comboBox.Items.Add("其他");
            queryRef_comboBox.SelectedItem = "其他";

            /* 將切換頁面事件註冊到頁籤 */
            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
        }

        // 初始化流水號顯示
        private void InitializeSerialNumDisplay() 
        {
            // 擷取最後一份資料
            List<dataStruct> current_database = utilities.ReadDatabase(database_path);
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

        #endregion

        public MainForm()
        {
            // 使用自製函式
            utilities = new Utilities();

            // 使用地址選擇器
            adressPicker = new AdressPicker(district_db_path);

            // 檢查資料庫連線
            bool isDatabaseConnected = utilities.Database_connected(database_path);

            // 若初始沒有偵測到資料庫, 則...
            if (!isDatabaseConnected) { }

            // 初始化區塊
            #region InitializeThings

            // 初始化頁面
            InitializeComponent();

            // 初始化地址選項 - 設定戶籍地址
            InitializeAdressPicker(Rgis_City_combobox, Rgis_Country_combobox, InitialCity, InitialCountry);

            // 初始化地址選項 - 設定通訊地址
            InitializeAdressPicker(Comm_City_combobox, Comm_Country_combobox, InitialCity, InitialCountry);

            // 初始化連絡電話和新生兒面板
            InitializePanels();

            // 初始化部分資訊顯示
            InitializeSomeInfo();

            // 初始化流水號顯示
            InitializeSerialNumDisplay();

            // 載入組合
            InitializeMatrices();

            // 初始化 Flags
            InitializeFlags();

            // 顯示歷史資料
            InitializeDataGridView();
            LoadJsonToDataGridView(
                startYear: DateTime.Now.Year, startMonth: 1,
                endYear: DateTime.Now.Year + 1, endMonth: 1
                );

            #endregion

            /**
             * 
             *  以下待改
             * 
             **/

            // 雜七雜八
            dupSerialNum = "";
            dupID = "";
            selected_serial_num = "";
        }

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
            if (DateTime.Now.Year != latest_year)
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

                // 名字和ID
                string NameAndID = "";

                // 生日
                string birthday = "";

                // 加入名字和身分證
                foreach (Control ctrl in p.Controls)
                {
                    // 姓名 + 身分證
                    if (ctrl is TextBox)
                    {
                        NameAndID += ctrl.Text.ToString() + sep;
                    }

                    // 生日
                    if (ctrl is ComboBox)
                    {
                        birthday += ctrl.Text.ToString() + sep;
                    }
                }

                // 加入名字和身分證和生日
                nbInfo.nbNames = NameAndID.Split(sep)[1];
                nbInfo.newbornID = NameAndID.Split(sep)[0];
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
            List<dataStruct> dataList = utilities.ReadDatabase(database_path);

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
                // 流水號與日期
                year: DateTime.Now.Year - 1911, month: DateTime.Now.Month, serialdx: inputSearialN, serialNumStr: inputSerialNum,
                FirstLogDate: currentDateTime,RecentEditDate: recentDateTime,
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
                nbName: nbNames, newBornID: newbornID, nbBitrhDate: nbBirthDay
            );
            return data;
        }

        // 檢查身分證
        public bool ValideID(string ID) 
        {
            // 讀取資料庫
            List<dataStruct> dataList = utilities.ReadDatabase(database_path);

            // 初始化Flags
            bool isfind = false;

            // 在資料庫中尋找重複
            foreach (dataStruct data in dataList)
            {
                List<string> ids2check = new List<string> {
                    data.query_id , data.apply_id, data.mate_id
                };
                foreach (string id in data.newBorn_id)
                {
                    ids2check.Add(id);
                }
                isfind = ids2check.Any(c => c!="" && c==ID);

                // 當找到重複則中止並回報
                if (isfind)
                {
                    dupSerialNum = data.serial_num;
                    string dupinfo = $"申請人: {data.apply_name} - {data.apply_id} \n" +
                        $"配偶: {data.mate_name} - {data.mate_id} \n\n新生兒資訊:\n";
                    dupID += dupinfo;

                    // 加入新生兒
                    for (int i = 0;  i < data.newBorn_name.Count;  i++)
                    {
                        dupID += $"({i+1}) " + data.newBorn_name[i].ToString() + "-";
                        dupID += data.newBorn_id[i].ToString() + "\n";
                    }

                    return isfind;
                }
            }

            return isfind;
        }

        // 移除所有格子的內容
        
        public void ClearBoxContent() 
        {
            // 初始化部分資訊顯示
            InitializeSomeInfo();

            /** 重新寫一個 **/
        }

        /** ==================================== 按鈕點擊事件 ==================================== **/

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
                    newBorn_panels.AddNBButtonClicked(groupBoxInfluenced[GroupBoxID.NewBorn]);                                    // 新增一個版面
                    newBorn_panels.deleteButtons[newBorn_panels.panel_nums - 1].Click += Delete_panel_Btn_Click;              // 將刪除函式綁定到新按鈕上面
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
                    break;
            }
        }

        // 當儲存按鈕被按下時執行
        private void saveBtn_Click(object sender, EventArgs e)
        {
            // 讀取資料庫
            List<dataStruct> dataList = utilities.ReadDatabase(database_path);

            // 如果讀取失敗則不會執行以下
            if (dataList is null){return;}

            // 從表單蒐集資訊
            dataStruct NewInfomation = GatherInfo();

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
                return;
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
                return;
            }

            // 檢查身分證號碼是否已經登錄
            List<string> ids2check = new List<string> {
               NewInfomation.apply_id, NewInfomation.mate_id, NewInfomation.query_id
            };
            List<string> checkItem = new List<string> {"申請人", "配偶", "委託人"};

            int count = 1;
            foreach (string id in NewInfomation.newBorn_id)
            {
                ids2check.Add(id);
                checkItem.Add("新生兒"+count.ToString());
                count++;
            }

            for (int i = 0; i < ids2check.Count; i++)
            {
                if (checkItem[i].Contains("申請人") || checkItem[i].Contains("新生兒"))
                {
                    if (ids2check[i].Length == 0)
                    {
                        MessageBox.Show($"身分證欄不得為空! \n於: {checkItem[i]}-{ids2check[i]}", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            
            // 郵局資訊
            List<string> acc_list2Check = new List<string> { textBox_account_name.Text, textBox_account_divn.Text, textBox_account_number.Text };
            bool is_accOK = acc_list2Check.All(acc => acc !="");
            if (!is_accOK)
            {
                MessageBox.Show($"郵局帳戶資訊不完整!", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool isDuplicate = ids2check.Any(c => ValideID(c));
            if (!isEditingPrevData)
            {
                // 檢測是否重複登入
                if (isDuplicate)
                {
                    if (dupSerialNum.Length > 0 && dupID.Length > 0)
                    {
                        MessageBox.Show($"重複登錄! 請檢查以下:\n流水號:{dupSerialNum}\n身分證號:\n{dupID}", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        dupSerialNum = "";
                        dupID = "";
                        return;
                    }
                }

                // 將新資訊加入
                dataList.Add(NewInfomation);
            }
            else
            {
                int replaceIndex = dataList.FindIndex(data => data.serial_num == NewInfomation.serial_num);
                dataList[replaceIndex] = NewInfomation;
            }

            // 將 JSON 寫入檔案
            string toWrite = "[\n";
            foreach (dataStruct data in dataList)
            {
                string dataStr = JsonConvert.SerializeObject(data, Formatting.Indented);
                toWrite += dataStr;
                toWrite += ",\n";
            }
            toWrite += "]";
            File.WriteAllText(database_path, toWrite);
            MessageBox.Show("儲存成功", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // 更新流水號顯示
            string serial2show = NewInfomation.login_year.ToString() + "-" + (NewInfomation.serial_index+1).ToString();
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
            ClearBoxContent();

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

        // 清除按鈕
        private void ClearInfoBtn_Click(object sender, EventArgs e)
        {
            ClearBoxContent();
        }

        /**
         * 
         * 以下重寫
         * 
         * 
         * **/

        // 申請人查詢
        private void SearchBtn_apply_Click(object sender, EventArgs e)
        {
            // 讀取資料庫
            List<dataStruct> dataList = utilities.ReadDatabase(database_path);

            // 如果讀取失敗則不會執行以下
            if (dataList is null) { return; }

            // 檢查身分證格式
            if (textBox_apply_IDnumber.Text.Length == 0)
            {
                MessageBox.Show("請輸入身分證/居留證!", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool isInDatabase = ValideID(textBox_apply_IDnumber.Text);

            if (isInDatabase)
            {
                MessageBox.Show($"該筆資料已經在資料庫\n\n流水號:{dupSerialNum}\n{dupID}", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dupSerialNum = "";
                dupID = "";
            }
            else
            {
                MessageBox.Show("尚未登入該筆資料。", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // 委託人查詢
        private void SearchBtn_query_Click(object sender, EventArgs e)
        {
            // 讀取資料庫
            List<dataStruct> dataList = utilities.ReadDatabase(database_path);

            // 檢查身分證格式
            if (textBox_query_IDnumber.Text.Length == 0)
            {
                MessageBox.Show("請輸入身分證/居留證!", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool isInDatabase = ValideID(textBox_query_IDnumber.Text);

            if (isInDatabase)
            {
                MessageBox.Show($"該筆資料已經在資料庫\n\n流水號:{dupSerialNum}\n{dupID}", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dupSerialNum = "";
                dupID = "";
            }
            else
            {
                MessageBox.Show("尚未登入該筆資料。", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // 新生兒查詢
        private void SearchBtn_newBorn_Click(object sender, EventArgs e)
        {
           /*重新寫*/
        }

        // 配偶查詢
        private void SearchBtn_mate_Click(object sender, EventArgs e)
        {
            // 讀取資料庫
            List<dataStruct> dataList = utilities.ReadDatabase(database_path);

            // 檢查身分證格式
            if (textBox_mate_IDnumber.Text.Length == 0)
            {
                MessageBox.Show("請輸入身分證/居留證!", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool isInDatabase = ValideID(textBox_mate_IDnumber.Text);

            if (isInDatabase)
            {
                MessageBox.Show($"該筆資料已經在資料庫\n\n流水號:{dupSerialNum}\n{dupID}", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dupSerialNum = "";
                dupID = "";
            }
            else
            {
                MessageBox.Show("尚未登入該筆資料。", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /* ==================================== 載入歷史資料 ==================================== */
        
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

            /** 加入郵局匯款資訊 **/
            textBox_account_name.Text = choosenRecords.account_name;            // 受款人
            textBox_account_divn.Text = choosenRecords.account_div;                  // 局號
            textBox_account_number.Text = choosenRecords.account_number;    // 帳號
        }

        // 載入手機
        private void LoadPhones2GroupBox(dataStruct choosenRecords) 
        {
            /** 載入手機號碼-申請人 **/
            apply_phone_panels.InitializeGUI(groupBoxInfluenced[GroupBoxID.Apply]);     // 初始化 GUI
            apply_phone_panels.LoadPhones(choosenRecords.apply_phones);                     // 載入電話

            /** 載入手機號碼-配偶 **/
            mate_phone_panels.InitializeGUI(groupBoxInfluenced[GroupBoxID.Mate]);       // 初始化 GUI
            mate_phone_panels.LoadPhones(choosenRecords.mate_phones);                       // 載入電話

            /** 載入手機號碼-委託人 **/
            query_phone_panels.InitializeGUI(groupBoxInfluenced[GroupBoxID.Query]);     // 初始化 GUI
            query_phone_panels.LoadPhones(choosenRecords.query_phones);                     // 載入電話
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
                newBornInfoes += $"\n新生兒({i+1}): {choosenRecords.newBorn_name[i]}\t\t身分證: {choosenRecords.newBorn_id[i]}\t\t生日: {choosenRecords.newbornBitrhDate[i]}\n";
            }

            // 擷取資料
            string toShowDetail = $"\t流水號:{choosenRecords.serial_num}\t初次登錄: {choosenRecords.First_Login_Date}\t最近一次修改: {choosenRecords.Recent_Edit_Date}\n" +
                $"\n  *  =========================== 申請人 ===========================  *\n" +
                $"\n\t申請人(孕婦)姓名:    {choosenRecords.apply_name}\t\t申請人身分證:\t{choosenRecords.apply_id}\n" +
                $"\n\t申請人聯絡電話:    {string.Join(" , ", choosenRecords.apply_phones)}\n" +
                $"\n\t受款人:    {choosenRecords.account_name}    身分證:{choosenRecords.account_ID} \t\t郵局帳號:    {choosenRecords.account_div}{choosenRecords.account_number}\n" +
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

        /** 
         * 
         * 以下要修改
         * 
         * **/

        private void historyGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // 如果點擊欄首, (row index = -1) 不要做任何事, 不然會出錯
            if (e.RowIndex < 0) { return; }

            // Highlight 選到的那一行
            historyGridView.Rows[e.RowIndex].Selected = true;

            // 讀取資料庫
            List<dataStruct> records = utilities.ReadDatabase(database_path);

            // 取選中的那一筆資料
            string seleted_serial_num = historyGridView.Rows[e.RowIndex].Cells["Serial_num"].Value.ToString();
            int selected_index = records.FindIndex(r => r.serial_num == seleted_serial_num);
            MessageBox.Show(seleted_serial_num + "-" + selected_index.ToString());
            dataStruct choosenRecords = records[selected_index];

            // 检查是否点击的是按钮列
            if (e.ColumnIndex == historyGridView.Columns["examDetailBTN"].Index)
            {
                ShowDetailInfo(choosenRecords);
            }
            else if (e.ColumnIndex == historyGridView.Columns["editDataBTN"].Index)
            {
                // switch flags
                isEditingPrevData = true;

                // 將資料載入至文字框
                LoadData2TextBox(choosenRecords);

                // 載入手機
                LoadPhones2GroupBox(choosenRecords);

                // 處理委託人Combo box
                if (choosenRecords.query_id == choosenRecords.apply_id)
                {
                    queryRef_comboBox.Text = "同申請人";
                }
                else if (choosenRecords.query_id == choosenRecords.mate_id)
                {
                    queryRef_comboBox.Text = "同配偶";
                }
                else
                {
                    queryRef_comboBox.Text = "其他";
                }

                // 切換頁面
                CancelBtn.Enabled = true;
                tabControl.SelectedTab = tabControl.TabPages["LogPage"];
            }
            else if (e.ColumnIndex == historyGridView.Columns["deleteDataBTN"].Index)
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
                        File.WriteAllText(database_path, updatedJson);

                        // 更新頁面
                        List<string> list2check = new List<string> { search_start_yy.Text, search_start_mm.Text, search_end_yy.Text, search_end_mm.Text };
                        bool isValidate = list2check.All(c => c.Length > 0);
                        if (isValidate)
                        {
                            LoadJsonToDataGridView(
                           startYear: Int32.Parse(search_start_yy.Text), startMonth: Int32.Parse(search_start_mm.Text),
                           endYear: Int32.Parse(search_end_yy.Text), endMonth: Int32.Parse(search_end_mm.Text)
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
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl tabControl = sender as TabControl;

            if (tabControl != null)
            {
                // 检查是否切换到特定的页签
                if (tabControl.SelectedTab.Name == "historyPage")
                {
                    // 當切換至歷史紀錄頁面時新增事件
                    search_start_yy.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
                    search_start_mm.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
                    search_end_yy.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
                    search_end_mm.SelectedIndexChanged += ComboBox_SelectedIndexChanged;

                    search_start_yy.TextChanged += ComboBox_SelectedIndexChanged;
                    search_start_mm.TextChanged += ComboBox_SelectedIndexChanged;
                    search_end_yy.TextChanged += ComboBox_SelectedIndexChanged;
                    search_end_mm.TextChanged += ComboBox_SelectedIndexChanged;

                    // 顯示更新的歷史資料
                    List<string> list2check = new List<string> { search_start_yy.Text, search_start_mm.Text, search_end_yy.Text, search_end_mm.Text };
                    bool isValidate = list2check.All(c => c.Length > 0);
                    if (isValidate)
                    {
                        LoadJsonToDataGridView(
                        startYear: Int32.Parse(search_start_yy.Text), startMonth: Int32.Parse(search_start_mm.Text),
                        endYear: Int32.Parse(search_end_yy.Text), endMonth: Int32.Parse(search_end_mm.Text)
                        );
                    }
                }
                else
                {
                    // 當切換至別的頁面時移除事件
                    search_start_yy.SelectedIndexChanged -= ComboBox_SelectedIndexChanged;
                    search_start_mm.SelectedIndexChanged -= ComboBox_SelectedIndexChanged;
                    search_end_yy.SelectedIndexChanged -= ComboBox_SelectedIndexChanged;
                    search_end_mm.SelectedIndexChanged -= ComboBox_SelectedIndexChanged;

                    search_start_yy.TextChanged -= ComboBox_SelectedIndexChanged;
                    search_start_mm.TextChanged -= ComboBox_SelectedIndexChanged;
                    search_end_yy.TextChanged -= ComboBox_SelectedIndexChanged;
                    search_end_mm.TextChanged -= ComboBox_SelectedIndexChanged;
                }
            }
        }

        private void InitializeDataGridView()
        {
            // 添加按钮列
            DataGridViewButtonColumn btnCol1 = new DataGridViewButtonColumn();
            btnCol1.HeaderText = "檢視詳細";
            btnCol1.Name = "examDetailBTN";
            btnCol1.Text = "檢視";
            btnCol1.UseColumnTextForButtonValue = true;
            historyGridView.Columns.Add(btnCol1);

            DataGridViewButtonColumn btnCol2 = new DataGridViewButtonColumn();
            btnCol2.HeaderText = "修改資料";
            btnCol2.Name = "editDataBTN";
            btnCol2.Text = "修改";
            btnCol2.UseColumnTextForButtonValue = true;
            historyGridView.Columns.Add(btnCol2);

            DataGridViewButtonColumn btnCol3 = new DataGridViewButtonColumn();
            btnCol3.HeaderText = "刪除資料";
            btnCol3.Name = "deleteDataBTN";
            btnCol3.Text = "刪除";
            btnCol3.UseColumnTextForButtonValue = true;
            historyGridView.Columns.Add(btnCol3);

            // 添加按钮列的点击事件
            historyGridView.CellClick += historyGridView_CellClick;
        }

        private void LoadJsonToDataGridView(int startYear, int startMonth, int endYear, int endMonth)
        {
            if (File.Exists(database_path))
            {
                try
                {
                    // 讀取 JSON 文件内容
                    string jsonContent = File.ReadAllText(database_path);

                    // 反序列化 JSON 内容為對象列表
                    List<dataStruct> records = JsonConvert.DeserializeObject<List<dataStruct>>(jsonContent);

                    // 條件篩選
                    List<string> target_serial_num = new List<string>();

                    // 篩選日期
                    if (startYear == endYear)
                    {
                        foreach (dataStruct data in records)
                        {
                            if (data.login_year == startYear)
                            {
                                if (data.login_month >= startMonth && data.login_month <= endMonth)
                                {
                                    target_serial_num.Add(data.serial_num);
                                }
                            }
                        }
                    }
                    else if (startYear < endYear)
                    {
                        foreach (dataStruct data in records)
                        {
                            if (data.login_year > startYear & data.login_year < endYear)
                            {
                                target_serial_num.Add(data.serial_num);
                            }
                            else if (data.login_year == startYear)
                            {
                                if (data.login_month >= startMonth)
                                {
                                    target_serial_num.Add(data.serial_num);
                                }
                            }
                            else if (data.login_year == endYear)
                            {
                                if (data.login_month <= endMonth)
                                {
                                    target_serial_num.Add(data.serial_num);
                                }
                            }
                        }
                    }

                    // 將數組轉換為字串
                    var displayRecords = records.Where(
                        record => target_serial_num.Contains(record.serial_num)
                        ).Select(record => new DisplayRecord
                    {
                        // 決定要顯示的內容
                        Serial_num = record.serial_num,
                        firstLoginDate = record.First_Login_Date,
                        recentEditDate = record.Recent_Edit_Date,
                        apply_name = record.apply_name,
                        apply_id = record.apply_id,
                        apply_phones = string.Join(", ", record.apply_phones), // 多個
                        account_name = record.account_name,
                        account_full_number = record.account_div + record.account_number,
                        newBorn_num = record.newBorn_name.Count,
                        newBorn_names = string.Join(", ", record.newBorn_name),
                        newBorn_IDs = string.Join(", ", record.newBorn_id)
                    }).ToList();

                    // 将数据绑定到 DataGridView
                    historyGridView.DataSource = displayRecords;

                    // 自定義欄位名稱
                    historyGridView.Columns["Serial_num"].HeaderText = "流水號";
                    historyGridView.Columns["firstLoginDate"].HeaderText = "初次登錄";
                    historyGridView.Columns["recentEditDate"].HeaderText = "最近修改";
                    historyGridView.Columns["apply_name"].HeaderText = "申請人(孕婦)";
                    historyGridView.Columns["apply_id"].HeaderText = "申請人身分證";
                    historyGridView.Columns["apply_phones"].HeaderText = "連絡電話";
                    historyGridView.Columns["account_name"].HeaderText = "郵局戶名";
                    historyGridView.Columns["account_full_number"].HeaderText = "郵局帳號";
                    historyGridView.Columns["apply_give_birth_date"].HeaderText = "孕婦生產日期";
                    historyGridView.Columns["newBorn_num"].HeaderText = "生產胎數";
                    historyGridView.Columns["newBorn_names"].HeaderText = "新生兒名字";
                    historyGridView.Columns["newBorn_IDs"].HeaderText = "新生兒身分證";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading JSON file: {ex.Message}","錯誤",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show($"File not found: {database_path}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 顯示用的數據結構
        public class DisplayRecord
        {
            // 流水號
            public string Serial_num { get; set; }
            // 初次登入日期
            public string firstLoginDate {  get; set; }
            // 最近修改日期
            public string recentEditDate { get; set; }
            // 申請人(孕婦)
            public string apply_name { get; set; }
            // 申請人(孕婦)身分證
            public string apply_id { get; set; }
            // 生產日期
            public string apply_give_birth_date { get; set; }
            // 申請人連絡電話
            public string apply_phones { get; set; }
            // 郵局戶名
            public string account_name { get; set; }
            // 郵局局號+帳號
            public string account_full_number { get; set; }
            //小孩數量
            public int newBorn_num { get; set; }
            // 小孩名字
            public string newBorn_names { get; set; }
            // 小孩身分證
            public string newBorn_IDs { get; set; }
        }

        private void IDsearchBtn_Click(object sender, EventArgs e)
        {
            // 讀取 JSON 文件内容
            string jsonContent = File.ReadAllText(database_path);

            // 反序列化 JSON 内容為對象列表
            List<dataStruct> records = JsonConvert.DeserializeObject<List<dataStruct>>(jsonContent);

            // ID BOX
            string idText = IDsearchBOX.Text.ToString();

            if (idText.Length == 0)
            {
                MessageBox.Show("請輸入身分證", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 將數組轉換為字串
            var displayRecords = records.Where(
                record => record.apply_id == idText || record.query_id == idText || record.newBorn_id.Contains(idText)
                ).Select(record => new DisplayRecord
                {
                    // 決定要顯示的內容
                    Serial_num = record.serial_num,
                    firstLoginDate = record.First_Login_Date,
                    recentEditDate = record.Recent_Edit_Date,
                    apply_name = record.apply_name,
                    apply_id = record.apply_id,
                    apply_phones = string.Join(", ", record.apply_phones), // 多個
                    account_name = record.account_name,
                    account_full_number = record.account_div + record.account_number,
                    newBorn_num = record.newBorn_name.Count,
                    newBorn_names = string.Join(", ", record.newBorn_name),
                    newBorn_IDs = string.Join(", ", record.newBorn_id)
                }).ToList();

            // 将数据绑定到 DataGridView
            historyGridView.DataSource = displayRecords;

            // 自定義欄位名稱
            historyGridView.Columns["Serial_num"].HeaderText = "流水號";
            historyGridView.Columns["firstLoginDate"].HeaderText = "初次登錄";
            historyGridView.Columns["recentEditDate"].HeaderText = "最近修改";
            historyGridView.Columns["apply_name"].HeaderText = "申請人(孕婦)";
            historyGridView.Columns["apply_id"].HeaderText = "申請人身分證";
            historyGridView.Columns["apply_phones"].HeaderText = "連絡電話";
            historyGridView.Columns["account_name"].HeaderText = "郵局戶名";
            historyGridView.Columns["account_full_number"].HeaderText = "郵局帳號";
            historyGridView.Columns["apply_give_birth_date"].HeaderText = "孕婦生產日期";
            historyGridView.Columns["newBorn_num"].HeaderText = "生產胎數";
            historyGridView.Columns["newBorn_names"].HeaderText = "新生兒名字";
            historyGridView.Columns["newBorn_IDs"].HeaderText = "新生兒身分證";
        }

        private void clearSearchBtn_Click(object sender, EventArgs e)
        {
            IDsearchBOX.Clear();
            List<string> list2check = new List<string> { search_start_yy.Text, search_start_mm.Text, search_end_yy.Text, search_end_mm.Text };
            bool isValidate = list2check.All(c => c.Length>0);
            if (isValidate)
            {
                LoadJsonToDataGridView(
                    startYear: Int32.Parse(search_start_yy.Text), startMonth: Int32.Parse(search_start_mm.Text),
                    endYear: Int32.Parse(search_end_yy.Text), endMonth: Int32.Parse(search_end_mm.Text)
                    );
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            // 讀取資料庫
            List<dataStruct> dataList = utilities.ReadDatabase(database_path);

            if (isEditingPrevData)
            {
                isEditingPrevData = false;
                CancelBtn.Enabled = false;
                ClearBoxContent();

                // 計算流水號
                int inputSearialN = 0;
                int inputYear = 0;

                int lastSerialN = dataList[dataList.Count - 1].serial_index;
                int latest_year = dataList[dataList.Count - 1].login_year;

                // 當年分不一樣時, 改變流水號
                if (DateTime.Now.Year != latest_year)
                {
                    lastSerialN = 0;
                }

                inputSearialN = lastSerialN + 1;
                inputYear = latest_year;

                string serialNum = inputYear + "-" + inputSearialN;
                SerialNumLabel.Text = serialNum;

                tabControl.SelectedTab = tabControl.TabPages["historyPage"];

                // Highlight 那一行
                foreach (DataGridViewRow row in historyGridView.Rows)
                {
                    if (row.Cells["Serial_num"].Value != null && row.Cells["Serial_num"].Value.ToString() == selected_serial_num)
                    {
                        row.Selected = true;
                        break;
                    }
                }
            }
        }

        private void exportExcelBtn_Click(object sender, EventArgs e)
        {
            // 讀取資料庫
            List<dataStruct> dataList = utilities.ReadDatabase(database_path);

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

            string excelFilePath = ShowSaveFileDialog();

            if (excelFilePath != null && select_serial_nums.Count > 0)
            {
                utilities.exportExcel(database_path, excelFilePath, select_serial_nums);
            }
            
        }

        private static string ShowSaveFileDialog()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel Files|*.xlsx";
                saveFileDialog.Title = "Save an Excel File";

                DateTime now = DateTime.Now;
                int adjYear = now.Year - 1911;
                string currentTime = $"{adjYear}-{now.Month.ToString()}-{now.Day.ToString()}";

                saveFileDialog.FileName = $"坐月子津貼輸出_{currentTime}.xlsx";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return saveFileDialog.FileName;
                }
            }

            return null;
        }

        /* ==================================== Check box ==================================== */
        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            void updateText(TextBox from, TextBox to) 
            {
                to.Text = from.Text;
                to.ForeColor = System.Drawing.Color.DarkGray;
            }

            void switch_txt_color(TextBox target_tb) 
            { 
                target_tb.ForeColor = System.Drawing.Color.DarkBlue;
            }

            void update_selection_Box(Control from, Control to) 
            {
                to.Text = from.Text;
                to.Enabled = false;
            }

            void unlock_selection_Box(Control target_ctrl) 
            {
                target_ctrl.Enabled = true;
            }

            CheckBox target = (CheckBox)sender;
            string targetName = target.Name;
            bool isChecked = target.Checked;

            if (isChecked)
            {
                switch (targetName)
                {
                    case "appAdress_checkBox":

                        break;

                    case "mateAdress_checkBox":

                        break;
                }
            }
            else
            {
                switch (targetName)
                {
                    case "appAdress_checkBox":

                        break;

                    case "mateAdress_checkBox":

                        break;
                }
            }
        }

        private void queryRef_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            string cbName = cb.Name;
            int selectIdx = cb.SelectedIndex;

            void switchGroupBox(bool switchG) 
            {
                foreach (Control Ctrl in groupBox_query.Controls)
                {
                    if (Ctrl.Name != cbName)
                    {
                        Ctrl.Enabled = switchG;
                    }
                }
            }

            switch (selectIdx)
            {
                // 申請人
                case 0:
                    query_relation.Text = "本人";
                    switchGroupBox(false);
                    break;

                // 配偶
                case 1:
                    query_relation.Text = "配偶";
                    switchGroupBox(false);
                    break;

                // 其他
                case 2:
                    query_relation.Text = "";
                    switchGroupBox(true);
                    break;
            }
        }

        private void nbAdressComboBox_ref_SelectedIndexChanged(object sender, EventArgs e)
        {
            void updateText(Control from, Control to)
            {
                to.Text = from.Text;
                to.ForeColor = System.Drawing.Color.DarkGray;
            }

            ComboBox cb = (ComboBox)sender;
            string cbName = cb.Name;
            int selectIdx = cb.SelectedIndex;

            switch (selectIdx)
            {
                // 申請人
                case 1:

                    break;

                // 配偶
                case 2:

                    break;

                // 委託人
                case 3:
                    
                    break;

                // Default
                case 0:
                    
                    break;
            }
        }

        /* ========================== 處理 ComboBox 的 SelectedIndexChanged 事件=====================  */
        // 
        public void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox target = (ComboBox)sender;
            string targetName = target.Name;

            // 生產日期
            List<string> apply_give_birth_cbName = new List<string> {
                "apply_give_birth_yy" , "apply_give_birth_mm" , "apply_give_birth_dd"
            };

            // 登錄日期
            List<string> search_date_cbName = new List<string> {
                "search_start_yy", "search_start_mm", "search_end_yy", "search_end_mm"
            };

            if (apply_give_birth_cbName.Contains(targetName))
            {

            }
            else if (search_date_cbName.Contains(targetName))
            {
                // 顯示更新的歷史資料
                List<string> list2check = new List<string> { search_start_yy.Text, search_start_mm.Text, search_end_yy.Text, search_end_mm.Text };
                bool isValidate = list2check.All(c => c.Length > 0);
                if (isValidate)
                {

                    if (Int32.Parse(search_start_yy.Text) > Int32.Parse(search_end_yy.Text))
                    {
                        search_end_yy.Text = (Int32.Parse(search_start_yy.Text) + 1).ToString();
                        search_end_mm.Text = 1.ToString();
                    }
                    else if (Int32.Parse(search_start_yy.Text) == Int32.Parse(search_end_yy.Text) && Int32.Parse(search_start_mm.Text) > Int32.Parse(search_end_mm.Text))
                    {
                        search_end_mm.Text = (Int32.Parse(search_start_mm.Text) + 1).ToString();
                    }

                    LoadJsonToDataGridView(
                    startYear: Int32.Parse(search_start_yy.Text), startMonth: Int32.Parse(search_start_mm.Text),
                    endYear: Int32.Parse(search_end_yy.Text), endMonth: Int32.Parse(search_end_mm.Text)
                    );

                }
            }
        }

        // 縣市選擇改變
        public void City_ComboBox_SelectedIndexChange(object sender, EventArgs e)
        {
            ComboBox target = (ComboBox)sender;
            string targetName = target.Name;
            string districtContent = File.ReadAllText(district_db_path);
            List<districtStruct> districtList = JsonConvert.DeserializeObject<List<districtStruct>>(districtContent);
            List<string> districtLList = new List<string>();
            districtStruct selectedCity = districtList.Where(d => d.city == target.Text).First();
            foreach (string d in selectedCity.district)
            {
                districtLList.Add(d);
            }
        }

        // 更新地址顯示
        public void Update_Combobox_Text(object sender, EventArgs e)
        {
            Control target = (Control)sender;
            string targetName = target.Name;
        }

    }
}


