using System;
using System.Collections.Generic;

/*  資料結構, 蒐集彙整輸入資料 */

namespace RigsterForm
{
    // 定義結構-用於儲存申請人的資料
    public struct districtStruct
    {
        public string city { get ; set; }

        public List<string> district { get; set; }

        // 建構函數
        public districtStruct(string CITY, List<string> DISTRICT)
        {
            city = CITY;
            district = DISTRICT;
        }
    }
}
