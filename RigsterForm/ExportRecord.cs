
namespace RigsterForm
{
    public class ExportRecord
    {
        // 審核結果
        public string SensorRes { get; set; }
        // 流水號
        public string Serial_num { get; set; }
        // 匯款日期
        public string remitDate {  get; set; }
        // 申請人(孕婦)
        public string apply_name { get; set; }
        // 申請人(孕婦)身分證
        public string apply_id { get; set; }
        // 新生兒姓名
        public string newBornNames {  get; set; }
        // 新生兒身分證
        public string newBornID { get; set; }
        // 郵局戶名(受款人名稱)
        public string account_name { get; set; }
        //受款人身分證
        public string accound_ID { get; set; }
        // 郵局局號
        public string account_div { get; set; }
        // 郵局帳號
        public string account_num { get; set; }
        // 補助金額
        public int allowance { get; set; }
        // 初次登入日期
        public string firstLoginDate { get; set; }
        // 最近修改日期
        public string recentEditDate { get; set; }
        // 備註
        public string note {  get; set; }
    }
}
