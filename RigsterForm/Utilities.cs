using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace RigsterForm
{
    public  class Utilities
    {
        public SystemSettings Settings { get; set; }

        public Utilities(SystemSettings systemSet) 
        {
            Settings = systemSet;
        }

        // 處理路徑
        public string ResolvePath(string path)
        {
            // 检查是否包含特殊标记，并进行替换
            if (path.Contains("%AppDataLocal%"))
            {
                string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                path = path.Replace("%AppDataLocal%", localAppDataPath);
            }

            return Path.GetFullPath(path);
        }

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

        // 輸出Excel
        public void exportExcel(string database_path, string excelFilePath, List<string> serial_num_selection, int allowance_per_nb) 
        {
            // 資料
            List<string> items = new List<string>()
            {
                "流水號", "案件登錄日期", "審核結果", "匯款日期","申請人(產婦)", "申請人身分證", "新生兒姓名", "新生兒身分證",
                "受款人", "受款人身分證", "郵局局號", "郵局帳號","補助金額","備註"
            };

            // Matrix
            Dictionary<string, int> matrix = new Dictionary<string, int>();
            for (int i = 0; i < items.Count; i++)
            {
                matrix.Add(items[i], i + 1);
            }

            // 讀取 JSON 文件内容
            List<dataStruct> records = ReadDatabase(Settings.data_basePath);

            using (var package = new ExcelPackage()) 
            {
                var worksheet = package.Workbook.Worksheets.Add("輸出");

                int columnIdx = 1; 

                if (records != null && records.Count > 0)
                {
                    // 標題
                    for (int i = 0; i < items.Count; i++)
                    {
                        worksheet.Cells[columnIdx, i+1].Value = items[i]; 
                    }

                    columnIdx++;

                    // 寫入數據 (跳過第一筆範例)
                    for (int i = 1; i < records.Count; i++)
                    {
                        if (serial_num_selection.Contains(records[i].serial_num))
                        {
                            worksheet.Cells[columnIdx, matrix["流水號"]].Value = records[i].serial_num;
                            worksheet.Cells[columnIdx, matrix["案件登錄日期"]].Value = records[i].First_Login_Date;
                            worksheet.Cells[columnIdx, matrix["審核結果"]].Value = records[i].sensor_result;
                            worksheet.Cells[columnIdx, matrix["匯款日期"]].Value = records[i].remit_date;
                            worksheet.Cells[columnIdx, matrix["申請人(產婦)"]].Value = records[i].apply_name;
                            worksheet.Cells[columnIdx, matrix["申請人身分證"]].Value = records[i].apply_id;
                            worksheet.Cells[columnIdx, matrix["新生兒姓名"]].Value = string.Join(" , ", records[i].newBorn_name);
                            worksheet.Cells[columnIdx, matrix["新生兒身分證"]].Value = string.Join(" , ", records[i].newBorn_id);
                            worksheet.Cells[columnIdx, matrix["受款人"]].Value = records[i].account_name;
                            worksheet.Cells[columnIdx, matrix["受款人身分證"]].Value = records[i].account_ID;
                            worksheet.Cells[columnIdx, matrix["郵局局號"]].Value = records[i].account_div;
                            worksheet.Cells[columnIdx, matrix["郵局帳號"]].Value = records[i].account_number;
                            worksheet.Cells[columnIdx, matrix["補助金額"]].Value = records[i].newBorn_name.Count * allowance_per_nb;
                            worksheet.Cells[columnIdx, matrix["備註"]].Value = records[i].notes;
                            columnIdx++;
                        }
                    }

                    // 自動調整欄寬
                    worksheet.Cells.AutoFitColumns();

                    // 保存 Excel 文件
                    package.SaveAs(new FileInfo(excelFilePath));
                }
            }

            MessageBox.Show("輸出完成","系統提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void exportNameListExcel(string database_path, string excelFilePath, List<string> serial_num_selection, int allowance_per_nb,DatePicker startDatePicker, DatePicker endDatePicker)
        {
            // 資料
            List<string> items = new List<string>()
            {
                "序號","申請人(產婦)姓名", "申請人身分證字號", "新生兒姓名", "新生兒身分證字號",
                "受款人姓名", "受款人身分證字號", "受款人局號/分行代號", "受款人帳號","補助金額","備註"
            };

            // Matrix
            Dictionary<string, int> matrix = new Dictionary<string, int>();
            for (int i = 0; i < items.Count; i++)
            {
                matrix.Add(items[i], i + 1);
            }

            // 讀取 JSON 文件内容
            List<dataStruct> records = ReadDatabase(Settings.data_basePath);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("輸出");

                // 加上日期資訊
                DateTime now = DateTime.Now;
                int current_year = now.Year - 1911;
                int current_month = now.Month;
                int current_day = now.Day;
                string startYear = startDatePicker.YearStr;
                string endYear = endDatePicker.YearStr;
                string startMonth = startDatePicker.MonthStr;
                string endMonth = endDatePicker.MonthStr;
                string startDay = startDatePicker.DayStr;
                string endDay = endDatePicker.DayStr;
                worksheet.Cells[1, 2].Value = "申請日期";
                worksheet.Cells[1, 3].Value = $"{startYear}年{startMonth}月{startDay}日";
                worksheet.Cells[1, 4].Value = "到";
                worksheet.Cells[1, 5].Value = $"{endYear}年{endMonth}月{endDay}日";
                worksheet.Cells[1, 6].Value = "列印日期";
                worksheet.Cells[1, 7].Value = $"{current_year}年{current_month}月{current_day}日";
                for (int k = 2; k <= 7; k++)
                {
                    worksheet.Cells[1, k].Style.Font.Size = 12;
                    worksheet.Cells[1, k].Style.Font.Bold = true;
                    worksheet.Cells[1, k].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, k].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                int columnIdx = 3;
                int ser_num = 1;
                int total_allowance = 0;

                if (records != null && records.Count > 0)
                {
                    // 標題
                    for (int i = 0; i < items.Count; i++)
                    {
                        worksheet.Cells[columnIdx, i + 1].Value = items[i];
                        worksheet.Cells[columnIdx, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[columnIdx, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[columnIdx, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }

                    columnIdx++;

                    // 寫入數據 (跳過第一筆範例)
                    for (int i = 1; i < records.Count; i++)
                    {
                        if (serial_num_selection.Contains(records[i].serial_num))
                        {
                            worksheet.Cells[columnIdx, matrix["序號"]].Value = ser_num.ToString();
                            worksheet.Cells[columnIdx, matrix["序號"]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[columnIdx, matrix["申請人(產婦)姓名"]].Value = records[i].apply_name;
                            worksheet.Cells[columnIdx, matrix["申請人(產婦)姓名"]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[columnIdx, matrix["申請人身分證字號"]].Value = records[i].apply_id;
                            worksheet.Cells[columnIdx, matrix["新生兒姓名"]].Value = string.Join(" , ", records[i].newBorn_name);
                            worksheet.Cells[columnIdx, matrix["新生兒身分證字號"]].Value = string.Join(" , ", records[i].newBorn_id);
                            worksheet.Cells[columnIdx, matrix["受款人姓名"]].Value = records[i].account_name;
                            worksheet.Cells[columnIdx, matrix["受款人身分證字號"]].Value = records[i].account_ID;
                            worksheet.Cells[columnIdx, matrix["受款人局號/分行代號"]].Value = records[i].account_div;
                            worksheet.Cells[columnIdx, matrix["受款人帳號"]].Value = records[i].account_number;
                            worksheet.Cells[columnIdx, matrix["補助金額"]].Value = records[i].newBorn_name.Count * allowance_per_nb;
                            worksheet.Cells[columnIdx, matrix["備註"]].Value = records[i].notes;
                            columnIdx++;
                            ser_num++;
                            total_allowance += records[i].newBorn_name.Count * allowance_per_nb;
                        }
                    }

                    // 合計列
                    worksheet.Cells[columnIdx, matrix["申請人(產婦)姓名"]].Value = "合計金額";
                    worksheet.Cells[columnIdx, matrix["申請人(產婦)姓名"]].Style.Font.Bold = true;
                    worksheet.Cells[columnIdx, matrix["申請人(產婦)姓名"]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[columnIdx, matrix["申請人(產婦)姓名"]].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // 總計
                    worksheet.Cells[columnIdx, matrix["補助金額"]].Value = total_allowance.ToString();
                    worksheet.Cells[columnIdx, matrix["補助金額"]].Style.Font.Bold = true;
                    worksheet.Cells[columnIdx, matrix["補助金額"]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[columnIdx, matrix["補助金額"]].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // 自動調整欄寬
                    worksheet.Cells.AutoFitColumns();

                    // 保存 Excel 文件
                    package.SaveAs(new FileInfo(excelFilePath));
                }
            }
        }
    }
}
