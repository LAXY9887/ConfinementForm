namespace RigsterForm
{
    public class ConstParameters
    {
        /* Database file path */
        public const string database_path = "Database.json";               // 申請資訊
        public const string district_db_path = "Taiwan_Districts.json";  //  地址列表

        /* 地址預設 */
        public const string InitialCity = "彰化縣";         // 初始化選擇縣市
        public const string InitialCountry = "和美鎮";  // 初始化選擇鄉鎮

        /* 重要Control 的名字 */
        public const string PanelNamePrefix = "_add_pannel_";                               // Addable panel 名字
        public const string TextBoxNamePrefix = "textBox_name_";                       // 之中姓名TextBox 名字
        public const string TextBoxIDPrefix = "textBox_newBorn_IDnumber_";   // 之中身分證TextBox 名字
        public const string ButtonNamePrefix = "_add_button_";                           // 之中刪除按鈕之名字
    }
}
