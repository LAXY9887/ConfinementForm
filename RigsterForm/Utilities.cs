using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace RigsterForm
{
    public  class Utilities
    {
        // 嘗試連線資料庫 (檢查NAS路徑)
        public bool Database_connected(string filePath)
        {
            // 初始化flag
            bool isConnected = false;

            // 檢查該檔案是否存在
            if (File.Exists(filePath))
            {
                // 若存在則將Flag設為true
                isConnected = true;

                // 若能夠找到檔案, 接著檢查讀取權限
                try
                {
                    // 嘗試讀取文件，檢查是否有權限問題
                    using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)){}
                }
                catch (Exception e)
                {
                    MessageBox.Show("無法連線至資料庫:檔案權限問題\n" + e.Message, "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isConnected = false;
                }
            }
            else
            {
                MessageBox.Show("無法連線至資料庫:連線問題", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return isConnected;
        }

        // 存取資料庫資訊
        public List<dataStruct> ReadDatabase(string database_path) 
        {
            // 如果沒有連到database則無法存檔
            bool isDatabaseConnected = Database_connected(database_path);
            if (!isDatabaseConnected)
            {
                MessageBox.Show("無法連線至資料庫, 無法儲存!", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            // 讀取Database內容為字串
            string databaseContent = File.ReadAllText(database_path); 

            // 反序列化為列表
            List<dataStruct>  dataList = JsonConvert.DeserializeObject<List<dataStruct>>(databaseContent);
            
            // 回傳列表
            return dataList;
        }

        /**
             * 
             *  以下待改
             * 
        **/

        // 設定下拉式選單的數值
        public void InitializeComboBoxDistrict(ComboBox comboBox, List<string> items, string defaultValue)
        {
            // 清空 ComboBox 项目
            comboBox.Items.Clear();

            // 添加数字到 ComboBox
            foreach (string item in items)
            {
                comboBox.Items.Add(item);
            }

            // 设置默认值
            comboBox.Text = defaultValue;
        }

        // 取得當前日期
        public string GetCurrentDate()
        {
            // 當下時間
            DateTime current = DateTime.Now;

            // 調整為民國年
            int adjust_year = (int)current.Year - 1911;

            // 生成修改過的日期
            DateTime adjustedDate = new DateTime(adjust_year, current.Month, current.Day, current.Hour, current.Minute, current.Second);

            // 修正格式
            string Str_current_date = adjustedDate.ToString("yyy-MM-dd");

            return Str_current_date;
        }

        // 設定下拉式選單的數值
        public void InitializeComboBox(ComboBox comboBox, int start, int end, int defaultValue)
        {
            // 清空 ComboBox 项目
            comboBox.Items.Clear();

            // 添加数字到 ComboBox
            for (int i = start; i <= end; i++)
            {
                comboBox.Items.Add(i);
            }

            // 设置默认值
            comboBox.SelectedItem = defaultValue;
        }

        // 輸出Excel
        public void exportExcel(string database_path, string excelFilePath, List<string> serial_num_selection) 
        {
            // 讀取 JSON 文件内容
            string jsonContent = File.ReadAllText(database_path);

            // 反序列化 JSON 内容為對象列表
            List<dataStruct> records = JsonConvert.DeserializeObject<List<dataStruct>>(jsonContent);

            using (var package = new ExcelPackage()) 
            {
                var worksheet = package.Workbook.Worksheets.Add("輸出");

                int columnIdx = 1;

                if (records != null && records.Count > 0)
                {
                        // 標題
                       worksheet.Cells[columnIdx, 1].Value = "流水號";
                       worksheet.Cells[columnIdx, 2].Value = "初次登錄";
                       worksheet.Cells[columnIdx, 3].Value = "最近修改";
                       worksheet.Cells[columnIdx, 4].Value = "申請人(孕婦)";
                       worksheet.Cells[columnIdx, 5].Value = "申請人身分證";
                       worksheet.Cells[columnIdx, 6].Value = "申請人出生日期";
                       worksheet.Cells[columnIdx, 7].Value = "申請人聯絡電話";
                       worksheet.Cells[columnIdx, 8].Value = "郵局戶名";
                       worksheet.Cells[columnIdx, 9].Value = "郵局局號";
                       worksheet.Cells[columnIdx, 10].Value = "郵局帳號";
                       worksheet.Cells[columnIdx, 11].Value = "生產胎數";
                       worksheet.Cells[columnIdx, 12].Value = "補助金額";
                       worksheet.Cells[columnIdx, 13].Value = "申請人戶籍地址";
                       worksheet.Cells[columnIdx, 14].Value = "申請人聯絡地址";
                       worksheet.Cells[columnIdx, 15].Value = "生產日期";
                       worksheet.Cells[columnIdx, 16].Value = "配偶";
                       worksheet.Cells[columnIdx, 17].Value = "配偶身分證";
                       worksheet.Cells[columnIdx, 18].Value = "配偶出生日期";
                       worksheet.Cells[columnIdx, 19].Value = "配偶聯絡電話";
                       worksheet.Cells[columnIdx, 20].Value = "配偶戶籍地址";
                       worksheet.Cells[columnIdx, 21].Value = "配偶聯絡地址";
                       worksheet.Cells[columnIdx, 22].Value = "委託人";
                       worksheet.Cells[columnIdx, 23].Value = "委託人身分證";
                       worksheet.Cells[columnIdx, 24].Value = "與產婦關係";
                       worksheet.Cells[columnIdx, 25].Value = "委託人聯絡電話";
                       worksheet.Cells[columnIdx, 26].Value = "委託人戶籍地址";
                       worksheet.Cells[columnIdx, 27].Value = "委託人聯絡地址";
                       worksheet.Cells[columnIdx, 28].Value = "新生兒";
                       worksheet.Cells[columnIdx, 29].Value = "新生兒身分證";
                       worksheet.Cells[columnIdx, 30].Value = "新生兒出生日期";
                       worksheet.Cells[columnIdx, 31].Value = "新生兒戶籍地";
                       columnIdx ++;

                    // 寫入數據
                    for (int i = 1; i < records.Count; i++)
                    {
                        if (serial_num_selection.Contains(records[i].serial_num))
                        {
                            worksheet.Cells[columnIdx, 1].Value = records[i].serial_num;
                            worksheet.Cells[columnIdx, 2].Value = records[i].First_Login_Date;
                            worksheet.Cells[columnIdx, 3].Value = records[i].Recent_Edit_Date;
                            worksheet.Cells[columnIdx, 4].Value = records[i].apply_name;
                            worksheet.Cells[columnIdx, 5].Value = records[i].apply_id;

                            string app_phones = string.Join(" , ", records[i].apply_phones);
                            worksheet.Cells[columnIdx, 7].Value = app_phones;
                            worksheet.Cells[columnIdx, 8].Value = records[i].account_name;
                            worksheet.Cells[columnIdx, 9].Value = records[i].account_div;
                            worksheet.Cells[columnIdx, 10].Value = records[i].account_number;

                            int nbCount = records[i].newBorn_name.Count;
                            int amount = nbCount * 8000;
                            worksheet.Cells[columnIdx, 11].Value = nbCount;
                            worksheet.Cells[columnIdx, 12].Value = amount;
                            worksheet.Cells[columnIdx, 16].Value = records[i].mate_name;
                            worksheet.Cells[columnIdx, 17].Value = records[i].mate_id;

                            string mate_phones = string.Join(" , ", records[i].mate_phones);
                            worksheet.Cells[columnIdx, 19].Value = mate_phones;
                            worksheet.Cells[columnIdx, 22].Value = records[i].query_name;
                            worksheet.Cells[columnIdx, 23].Value = records[i].query_id;
                            worksheet.Cells[columnIdx, 24].Value = records[i].query_relation;

                            string query_phones = string.Join(" , ", records[i].query_phones);
                            worksheet.Cells[columnIdx, 25].Value = query_phones;

                            string nbNames = string.Join(" , ", records[i].newBorn_name);
                            string nbID = string.Join(" , ", records[i].newBorn_id);
                            string nbBitrhDay = string.Join(" , ", records[i].newbornBitrhDate);
                            worksheet.Cells[columnIdx, 28].Value = nbNames;
                            worksheet.Cells[columnIdx, 29].Value = nbID;
                            worksheet.Cells[columnIdx, 30].Value = nbBitrhDay;

                            for (int j = 1; j < 32; j++)
                            {
                                if (worksheet.Cells[columnIdx, j].Value.ToString() == "")
                                {
                                    worksheet.Cells[columnIdx, j].Value = "無";
                                }
                            }

                            columnIdx++;
                        }
                    }

                    // 保存 Excel 文件
                    package.SaveAs(new FileInfo(excelFilePath));
                }
            }

            MessageBox.Show("輸出完成","系統提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
