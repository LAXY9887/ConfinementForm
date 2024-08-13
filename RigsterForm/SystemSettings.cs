using iTextSharp.text;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Forms;

namespace RigsterForm
{
    public class SystemSettings
    {
        /* 補貼金額 */
        public int allowance_per_nb { get; set; }

        /* 資料庫路徑 */
        public string data_basePath { get; set; }

        /* 和美公所虛擬帳號 */
        public string remit_format_sub1 { get; set; }

        /* 空格 */
        public int SpaceNum { get; set; }

        /* 郵局虛擬帳號 */
        public string remit_format_sub2 { get; set; }

        /* 坐月子津貼計畫代碼(待決定) */
        public string remit_format_sub3 { get; set; }

        /* Utility */
        private Utilities utilities;

        /* 建構式 */
        public SystemSettings() 
        {
            utilities = new Utilities(this);
            ApplySettings();
        }

        /*設定補貼金額*/
        public void SetAllowancePerNB(int amount)
        {
            allowance_per_nb = amount;
        }

        /* 設定資料庫路徑 */
        public void SetDatabasePath(string path)
        {
            data_basePath = utilities.ResolvePath(path);
        }

        /* 設定匯款格式 */
        public void SetRemitFormat(string f1, int spaceN, string f2, string f3)
        {
            remit_format_sub1 = f1;
            remit_format_sub2 = f2;
            remit_format_sub3 = f3;
            SpaceNum = spaceN;
        }

        /** 初始化資料庫 **/
        public void InitializeDataBase(string saving_path)
        {
            string toWrite = "[\r\n{\r\n  \"login_year\": 113,\r\n  \"login_month\": 7,\r\n  \"login_day\": 20,\r\n  \"serial_index\": 0,\r\n  \"First_Login_Date\": \"113-07-20\",\r\n  \"Recent_Edit_Date\": \"113-07-20\",\r\n  \"notes\": \"這是範例\",\r\n  \"sensor_result\":\"未審核\",\r\n  \"serial_num\": \"113-0\",\r\n  \"remit_date\": \"尚未匯款\",\r\n  \"apply_name\": \"示範人\",\r\n  \"apply_id\": \"A000000000\",\r\n  \"apply_phones\": [\r\n\t\"0900-123-456\",\r\n\t\"04-7890000\"\r\n  ],\r\n  \"mate_name\": \"示範配偶\",\r\n  \"mate_id\": \"F000000000\",\r\n  \"mate_phones\": [\r\n\t\"0955-123456\",\r\n\t\"04-7812000\"\r\n  ],\r\n  \"query_name\": \"委託人\",\r\n  \"query_id\": \"B000000000\",\r\n  \"query_relation\": \"母女\",\r\n  \"query_phones\": [\r\n\t\"0900-123456\",\r\n\t\"0900-123456\"\r\n  ],\r\n  \"newBorn_name\": [\r\n\t\"新生兒1\"\r\n  ],\r\n  \"newBorn_id\": [\r\n\t\"C000000000\"\r\n  ],\r\n  \"newbornBitrhDate\": [\r\n\t\"113-1-1\"\r\n  ],\r\n  \"account_name\": \"新生兒1\",\r\n  \"account_ID\": \"Q000000000\",\r\n  \"account_div\": \"0080000\",\r\n  \"account_number\": \"0040000\",\r\n  \"regis_Adress\": \"彰化縣和美鎮OO里XX路123號\",\r\n  \"comm_Adress\": \"彰化縣和美鎮OO里XX路123號\"\r\n},\r\n]";

            File.WriteAllText(saving_path, toWrite);

            // 更新路徑
            data_basePath = saving_path;

            MessageBox.Show("資料庫設定完成", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /* 應用設定 */
        public void ApplySettings()
        {
            // Read settings
            string settingContent = File.ReadAllText(ConstParameters.settingsPath);
            SettingStruct currentSettings = JsonConvert.DeserializeObject<SettingStruct>(settingContent);

            // Apply settings
            SetAllowancePerNB(currentSettings.Allowance_per_new_born);
            SetDatabasePath(currentSettings.Database_path);
            SetRemitFormat(currentSettings.RemitFormat1, currentSettings.SpaceNumber, currentSettings.RemitFormat2, currentSettings.RemitFormat3);
        }
    }
}
