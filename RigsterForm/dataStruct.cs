using System.Collections.Generic;

/*  資料結構, 蒐集彙整輸入資料 */

namespace RigsterForm
{
    // 定義結構-用於儲存申請人的資料
    public struct dataStruct
    {
        /* ======================= 系統資料 ===================== */
        public int serial_index { get ; set; }

        public int login_year { get; set; }

        public int login_month { get; set; }

        public int login_day { get; set; }

        /* ===================== 流水號與日期 ==================== */

        // 審核結果
        public string sensor_result { get ; set; }

        // 流水號
        public string serial_num { get; set; }

        // 匯款日期
        public string remit_date { get; set; }

        // 初次登錄日期時間
        public string First_Login_Date { get; set; }

        // 上次修改時間
        public string Recent_Edit_Date { get; set; }

        // 註記
        public string notes { get; set; }

        /* ===================== 申請人資料 ===================== */

        // 申請人姓名
        public string apply_name { get; set; }

        // 申請人身分證字號
        public string apply_id { get; set; }

        // 申請人連絡電話
        public List<string> apply_phones { get; set; }

        /* ===================== 配偶資料 ===================== */

        // 申請人姓名
        public string mate_name { get; set; }

        // 申請人身分證字號
        public string mate_id { get; set; }

        // 申請人連絡電話
        public List<string> mate_phones { get; set; }

        /* ===================== 委託人資料 ===================== */

        // 委託人姓名
        public string query_name { get; set; }

        // 委託人身分證字號
        public string query_id { get; set; }

        // 與產婦關係
        public string query_relation { get; set; }

        // 委託人連絡電話
        public List<string> query_phones { get; set; }

        /* ===================== 新生兒資料 ===================== */

        // 新生兒姓名
        public List<string> newBorn_name { get; set; }

        // 新生兒身分證字號
        public List<string> newBorn_id { get; set; }

        // 新生兒生日
        public List<string> newbornBitrhDate { get; set; }

        /* ===================== 郵局帳戶+地址資料 ==================== */

        // 郵局戶名 (受款人)
        public string account_name { get; set; }

        // 受款人身分證
        public string account_ID { get; set; }

        // 郵局局號
        public string account_div { get; set; }

        // 郵局帳號
        public string account_number { get; set; }

        // 戶籍地址
        public string regis_Adress { get ; set; }

        // 通訊地址
        public string comm_Adress { get; set; }

        // 建構函數
        public dataStruct(
            int serialdx, string serialNumStr, string FirstLogDate, string RecentEditDate,
            int year, int month, int day, string sensorRes, string remitDate,
            string appName, string appID, List<string> appPhones,
            string mateName, string mateID, List<string> matePhones,
            string queryName, string queryID, string queryRelate, List<string> queryPhones,
            string ac_name, string ac_ID,string ac_div, string ac_number, string regisAdress, string cmAdress,
            List<string> nbName, List<string> newBornID, List<string> nbBitrhDate, string note
            )
        {
            /* ===================== 系統參數 =================== */
            serial_index = serialdx;
            login_year = year;
            login_month = month;
            login_day = day;

            /* =================== 流水號與日期 ================== */
            First_Login_Date = FirstLogDate;
            Recent_Edit_Date = RecentEditDate;

            // 審核結果
            sensor_result = sensorRes;

            // 計算流水號 = 年分 + 序號
            serial_num = serialNumStr;

            // 匯款日期
            remit_date = remitDate;

            // 註記
            notes = note;

            /* =================== 申請人資料 =================== */
            apply_name = appName;
            apply_id = appID;
            apply_phones = appPhones;

            /* ==================== 配偶資料 =================== */
            mate_name = mateName;
            mate_id = mateID;
            mate_phones = matePhones;

            /* =================== 委託人資料 =================== */
            query_name = queryName;
            query_id = queryID;
            query_relation = queryRelate;
            query_phones = queryPhones;

            /* =================== 郵局+地址資料 ==================== */
            account_name = ac_name;
            account_ID = ac_ID;
            account_div = ac_div;
            account_number = ac_number;
            regis_Adress = regisAdress;
            comm_Adress = cmAdress;

            /* =================== 新生兒資料 =================== */
            newBorn_name = nbName;
            newBorn_id = newBornID;
            newbornBitrhDate = nbBitrhDate;
        }
    }
}
