using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigsterForm
{
    public struct SettingStruct
    {
        public int Allowance_per_new_born { get; set; }

        public string Database_path { get; set; }

        public string District_db_pth { get; set; }

        /* 和美公所虛擬帳號 */
        public string RemitFormat1 { get; set; }

        /* 郵局虛擬帳號 */
        public string RemitFormat2 { get; set; }

        /* 坐月子津貼計畫代碼(待決定) */
        public string RemitFormat3 { get; set; }

        /* 空格 */
        public int SpaceNumber { get; set; }
    }
}
