using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace RigsterForm
{
    public class PDFmaker
    {
        // 字體
        public BaseFont bfChinese;
        public BaseFont sinhei_Content;
        public Font cellFont;
        public Font LcellFont;
        public Font headerFont;

        // 使用系統設置
        public Utilities utilities;

        /* 系統設置 */
        private SystemSettings settingCtrl;

        // 建構式
        public PDFmaker(SystemSettings systemCtrl)
        {
            // 設置字體
            string kaiu = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KAIU.TTF");
            bfChinese = BaseFont.CreateFont(kaiu, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            string sinhei = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "msjh.ttc,0");
            sinhei_Content = BaseFont.CreateFont(sinhei, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            // 自型
            headerFont = new Font(bfChinese, 10, Font.NORMAL, BaseColor.BLACK); // 字體大小12, 普通字體, 黑色
            cellFont = new Font(bfChinese, 10, Font.NORMAL, BaseColor.BLACK); // 字體大小12, 普通字體, 黑色
            LcellFont = new Font(bfChinese, 12, Font.NORMAL, BaseColor.BLACK); // 字體大小12, 普通字體, 黑色

            // 使用自製函式
            utilities = new Utilities(settingCtrl);
            settingCtrl = systemCtrl;
        }

        // 寫字
        private void WriteContent(PdfContentByte cb, string content,BaseFont font, int font_size,float x, float y, int ALIGN)
        {
            cb.BeginText();
            cb.SetFontAndSize(font, font_size);
            cb.ShowTextAligned(ALIGN, content, x, y, 0);
            cb.EndText();
        }

        // 畫線
        private void DrawUnderLine(PdfContentByte cb, string refText, float refX, float refY)
        {
            // 底線位置
            float underlineY = refY - 2; // 調整底線與文字的距離
            float underlineWidth = cb.GetEffectiveStringWidth(refText, false);

            // 繪製底線
            cb.MoveTo(refX - underlineWidth - 25f, underlineY);
            cb.LineTo(refX, underlineY);
            cb.Stroke();
        }

        // 預算科目 (空表)
        private void DrawEmptyTable(PdfContentByte cb, Document document, float X, float Y, float width, string context)
        {
            // 設置表格標題
            PdfPTable table = new PdfPTable(1);

            for (int i = 0; i < 4; i++)
            {
                PdfPCell cell = new PdfPCell(new Phrase(context, LcellFont));
                table.AddCell(cell);
            }

            // 完成表格行
            table.CompleteRow();

            // 設置絕對位置
            float xPos = X;  // X 坐標
            float yPos = document.PageSize.Top - Y;  // Y 坐標，從頁面的頂部開始

            // 設置表格總寬度
            table.TotalWidth = width;
            table.LockedWidth = true;

            // 寫入表格到指定位置
            table.WriteSelectedRows(0, -1, xPos, yPos, cb);
        }

        // 做表
        private void DrawTable(Document document, PdfContentByte cb, List<dataStruct> dataList, List<string> target_serial_num, int division_num, int start_idx) 
        {
            // Making Cells
            PdfPCell makeCell(string context, Font font, float padding_bottom = 10.5f)
            {
                PdfPCell cell = new PdfPCell(new Phrase(context, font));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;  // 水平置中
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;    // 垂直置中
                cell.PaddingBottom = padding_bottom;
                return cell;
            }

            // 資料篩選的欄位
            Dictionary<string,string> usedCols = new Dictionary<string,string>()
            {
                {"apply_name", "申請人\n(產婦)姓名" },
                {"apply_id", "申請人\n身分證字號" },
                {"newBorn_name", "新生兒\n姓名" },
                {"newBorn_id", "新生兒\n身分證字號" },
                {"account_name", "受款人\n姓名" },
                {"account_ID", "受款人\n身分證字號" },
                {"account_div", "受款人\n局號/分行代號" },
                {"account_number","受款人\n帳號" },
            };

            // 表目
            int column_num = usedCols.Count + 3;

            // 設置表格標題
            PdfPTable table = new PdfPTable(column_num);

            // 加入標題
            table.AddCell(makeCell("序\n號", headerFont));
            foreach (var kvp in usedCols)
            {
                table.AddCell(makeCell(kvp.Value, headerFont));
            }
            table.AddCell(makeCell("補助\n金額", headerFont));
            table.AddCell(makeCell("備註", headerFont));

            // 加入表格內容
            int idx_count = 0;
            foreach (dataStruct data in dataList)
            {
                if (target_serial_num.Contains(data.serial_num))
                {
                    table.AddCell(makeCell((start_idx + idx_count + 1).ToString(), cellFont));    //序列號
                    table.AddCell(makeCell(data.apply_name, cellFont));    //申請人
                    table.AddCell(makeCell(data.apply_id, cellFont));    //申請人身分證
                    table.AddCell(makeCell(string.Join(",\n", data.newBorn_name), cellFont));    //新生兒名字
                    table.AddCell(makeCell(string.Join(",\n", data.newBorn_id), cellFont));    //新生兒身分證
                    table.AddCell(makeCell(data.account_name, cellFont));    //受款人
                    table.AddCell(makeCell(data.account_ID, cellFont));    //受款人身分證
                    table.AddCell(makeCell(data.account_div, cellFont));    //郵局局號
                    table.AddCell(makeCell(data.account_number, cellFont));    //郵局局號
                    table.AddCell(makeCell((data.newBorn_name.Count * settingCtrl.allowance_per_nb).ToString(), cellFont));    //補助金額
                    table.AddCell(makeCell("", cellFont));    // 備註
                    idx_count++;
                }

                if (idx_count == division_num || idx_count == dataList.Count)
                {
                    break;
                }
            }

            // 完成表格行
            table.CompleteRow();

            // 設置絕對位置
            float xPos = 50f;  // X 坐標
            float yPos = document.PageSize.Height - 150f;  // Y 坐標，從頁面的頂部開始

            // 設置表格總寬度
            table.TotalWidth = document.PageSize.Width * 0.9f;
            table.LockedWidth = true;

            // 設定每個欄位的寬度 (相對比例)
            float[] columnWidths = new float[column_num];
            columnWidths[0] = 2.5f;
            float remainingWidth = (100- columnWidths[0]) / 10f;
            for (int i = 1; i < columnWidths.Length; i++)
            {
                columnWidths[i] = remainingWidth;
            }
            table.SetWidths(columnWidths);

            // 寫入表格到指定位置
            table.WriteSelectedRows(0, -1, xPos, yPos, cb);
        }

        // 畫文字框
        private void DrawTextRect(PdfContentByte cb, Document document,float X, float Y, float width, float height, string content)
        {
            float posY = document.PageSize.Height - height + Y;

            cb.Rectangle(X, posY, width, height);
            cb.Stroke(); // 繪製邊框

            // 設置文字並置中
            string text = content;
            ColumnText ct = new ColumnText(cb);

            // 計算文字的X和Y的起始點，以便置中
            float llx = X + 3;
            float lly = posY;
            float urx = X + width;
            float ury = posY + height;

            ct.SetSimpleColumn(new Phrase(text, headerFont), llx, lly, urx, ury, 14, Element.ALIGN_CENTER | Element.ALIGN_MIDDLE);
            ct.Go();
        }

        // 頁首 (第一頁)
        private void PlotPageHead(Document document, PdfContentByte cb)
        {
            // 設定標題文字和位置
            string titleText1 = "彰化縣和美鎮公所";
            float xT1 = document.PageSize.Width - document.RightMargin - 150; // 右上角的x坐標
            float yT1 = document.PageSize.Height - document.TopMargin - 25; // 右上角的y坐標
            WriteContent(cb, titleText1, bfChinese, 16, xT1, yT1, Element.ALIGN_RIGHT);

            // 設定標題文字和位置
            WriteContent(cb, "114年    坐月子津貼發放清冊", bfChinese, 16,
            (document.PageSize.Width - document.RightMargin - 145),
            (document.PageSize.Height - document.TopMargin - 48),
                Element.ALIGN_RIGHT);

            // 設定日期
            DateTime now = DateTime.Now;
            int year = now.Year - 1911;
            int month = now.Month;
            int day = now.Day;
            WriteContent(cb, $"{year} 年 {month} 月 {day} 日", sinhei_Content, 13,
            (document.PageSize.Width - document.RightMargin - 0),
            (document.PageSize.Height - document.TopMargin - 80),
            Element.ALIGN_RIGHT);

            // 畫底線
            DrawUnderLine(cb, titleText1, xT1, yT1);

            // 文字框
            DrawTextRect(cb, document, 50f, -75f, 18, 64, "預\n算\n科\n目\n");

            // 空表格
            DrawEmptyTable(cb, document, 68, 75, 175, " ");
        }

        // 頁尾 (最後一頁)
        private void PlotPageEND(Document document, PdfContentByte cb)
        {
            // 推算編號
            WriteContent(cb, "推算編號:", sinhei_Content, 13,
                (document.LeftMargin + 505),
                (document.BottomMargin + 50),
                Element.ALIGN_RIGHT);

            int anchorX = 80;
            int anchorY = 20;
            int space = 160;

            System.Collections.Generic.List<string> list2Add = new System.Collections.Generic.List<string>()
            {
                "經辦單位: ", "財政課:","主計室:","機關長官:"
            };

            for (int i = 0; i < list2Add.Count; i++)
            {
                WriteContent(cb, list2Add[i], sinhei_Content, 13,
                (document.LeftMargin + anchorX + space * i),
                (document.BottomMargin + anchorY),
                Element.ALIGN_RIGHT);
            }
        }

        public void GeneratePDF(string saving_path, List<dataStruct> records, List<string> selected_ser_nums, int row_per_page = 10)
        {
            // 創建文件流
            using (FileStream stream = new FileStream(saving_path, FileMode.Create))
            {
                // 初始化 (橫向)
                Document document = new Document(PageSize.A4.Rotate());
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);

                // 開啟文件
                document.Open();
                PdfContentByte cb = pdfWriter.DirectContent;

                // 頁首
                PlotPageHead(document, cb);

                /** 表格 **/
                List<string> tmp_search = new List<string>(selected_ser_nums); // 複製一份目標名單
                int page_count = 0;
                while (tmp_search.Count > 0)
                {
                    // 製表 (每頁 row_per_page 行)
                    DrawTable(document, cb, records, tmp_search, row_per_page, row_per_page * page_count);

                    // 換頁, 處裡剩下的
                    int remaining_count = tmp_search.Count - row_per_page;

                    // 當剩下的數量 > 0 時繼續迴圈
                    if (remaining_count > 0)
                    {
                        document.NewPage();
                        page_count++;
                        tmp_search = tmp_search.GetRange(row_per_page, remaining_count);
                    }

                    // 當剩下的數量 > 0 時終止迴圈
                    else
                    {
                        break;
                    }
                }

                // 頁尾
                PlotPageEND(document, cb);

                // 關閉文檔
                document.Close();
            }

            // 提示
            MessageBox.Show("輸出完成!", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
