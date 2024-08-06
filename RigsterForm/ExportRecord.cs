
namespace RigsterForm
{
    public class ExportRecord
    {
        // 流水號
        public string Serial_num { get; set; }
        // 初次登入日期
        public string firstLoginDate { get; set; }
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
}
