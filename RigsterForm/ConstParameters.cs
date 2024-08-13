using Newtonsoft.Json;
using System;
using System.IO;

namespace RigsterForm
{
    public class ConstParameters
    {
        // 獲取 Local 的 AppData 資料夾路徑
        private const string APPName = "ConfinementForm";
        private const string SETTINGNAME = "settings.json";
        private static string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string myLocalAppFolder = Path.Combine(localAppDataPath, APPName);
        public static string settingsPath = Path.Combine(myLocalAppFolder, SETTINGNAME);

        // Read settings
        private static string settingContent = File.ReadAllText(settingsPath);
        private static SettingStruct settingStruct = JsonConvert.DeserializeObject<SettingStruct>(settingContent);

        /* Database file path */
        public static string default_database_path = settingStruct.Database_path;
        public static string district_db_path = settingStruct.District_db_pth ;  //  地址列表

        /* 地址預設 */
        public const string InitialCity = "彰化縣";         // 初始化選擇縣市
        public const string InitialCountry = "和美鎮";  // 初始化選擇鄉鎮

        /* 重要Control 的名字 */
        public const string PanelNamePrefix = "_add_pannel_";                               // Addable panel 名字
        public const string TextBoxNamePrefix = "textBox_name_";                       // 之中姓名TextBox 名字
        public const string TextBoxIDPrefix = "textBox_newBorn_IDnumber_";      // 之中身分證TextBox 名字
        public const string ButtonNamePrefix = "_add_button_";                           // 之中刪除按鈕之名字
        public const string newBorn_panel_namePrefix = "_newBorn_panel_";       // 新生兒 panel 名字

        /* 生日 */
        public const string birthday_Year_CB_prefix = "birthYearCB_";
        public const string birthday_Month_CB_prefix = "birthMonthCB_";
        public const string birthday_Day_CB_prefix = "birthdayCB_";
    }
}
