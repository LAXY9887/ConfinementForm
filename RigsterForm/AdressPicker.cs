
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RigsterForm
{
    /** 地址選擇器 **/

    public class AdressPicker
    {
        // 資料庫
        public string database_Path;

        // 資料庫內容
        public string districtContent;
        public List<districtStruct> districtList;

        public AdressPicker(string dbPath) 
        {
            database_Path = dbPath;
            
            // 讀取資料庫
            districtContent = File.ReadAllText(database_Path);
            districtList = JsonConvert.DeserializeObject<List<districtStruct>>(districtContent);
        }

        // 載入縣市列表
        public void LoadCityList(ComboBox CityCB) 
        {
            // 加入列表
            foreach (districtStruct dis in districtList)
            {
                CityCB.Items.Add(dis.city);
            }
        }

        // 載入鄉鎮市區列表
        public void LoadCountryList(ComboBox CountryCB, string citySelect)
        {
            // 得到該城市的鄉鎮列表
            districtStruct selectedCity = districtList.Where(d => d.city == citySelect).First();

            // 加入列表
            foreach (string dis in selectedCity.district)
            {
                CountryCB.Items.Add(dis);
            }
        }

        // 設定預設的值
        public void SetDefaultValue(ComboBox CB, string default_value) 
        { 
            CB.Text = default_value;
        }
    }
}
