using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
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
        public Font headerFont;

        // 使用系統設置
        public Utilities utilities;

        /* 系統設置 */
        private SystemSettings settingCtrl;

        // 建構式
        public PDFmaker()
        {
            // 設置字體
            string kaiu = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KAIU.TTF");
            bfChinese = BaseFont.CreateFont(kaiu, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            string sinhei = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "msjh.ttc,0");
            sinhei_Content = BaseFont.CreateFont(sinhei, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            headerFont = new Font(bfChinese, 10, Font.NORMAL, BaseColor.BLACK); // 字體大小12, 普通字體, 黑色
            cellFont = new Font(bfChinese, 10, Font.NORMAL, BaseColor.BLACK); // 字體大小12, 普通字體, 黑色

            // 使用自製函式
            utilities = new Utilities();
            settingCtrl = new SystemSettings();
            int Allowance_per_nb = 8000;
            settingCtrl.SetAllowancePerNB(Allowance_per_nb);
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
            cb.MoveTo(refX - underlineWidth, underlineY);
            cb.LineTo(refX, underlineY);
            cb.Stroke();
        }

        // 做表
        private void DrawTable(Document document, PdfContentByte cb, List<dataStruct> dataList, List<string> target_serial_num) 
        {
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
            PdfPCell headerCell1 = new PdfPCell(new Phrase("序\n號", headerFont));
            headerCell1.HorizontalAlignment = Element.ALIGN_CENTER;  // 水平置中
            headerCell1.VerticalAlignment = Element.ALIGN_MIDDLE;    // 垂直置中
            table.AddCell(headerCell1);

            foreach (var kvp in usedCols)
            {
                PdfPCell headerCelln = new PdfPCell(new Phrase(kvp.Value, headerFont));
                headerCelln.HorizontalAlignment = Element.ALIGN_CENTER;  // 水平置中
                headerCelln.VerticalAlignment = Element.ALIGN_MIDDLE;    // 垂直置中
                table.AddCell(headerCelln);
            }
            PdfPCell headerCell2 = new PdfPCell(new Phrase("補助\n金額", headerFont));
            headerCell2.HorizontalAlignment = Element.ALIGN_CENTER;  // 水平置中
            headerCell2.VerticalAlignment = Element.ALIGN_MIDDLE;    // 垂直置中
            table.AddCell(headerCell2);
            PdfPCell headerCell3 = new PdfPCell(new Phrase("備註", headerFont));
            headerCell3.HorizontalAlignment = Element.ALIGN_CENTER;  // 水平置中
            headerCell3.VerticalAlignment = Element.ALIGN_MIDDLE;    // 垂直置中
            table.AddCell(headerCell3);

            // 加入表格內容
            int idx_count = 1;
            foreach (dataStruct data in dataList)
            {
                if (target_serial_num.Contains(data.serial_num))
                {
                    PdfPCell serCell = new PdfPCell(new Phrase(idx_count.ToString(), cellFont));
                    serCell.HorizontalAlignment = Element.ALIGN_CENTER;  // 水平置中
                    serCell.VerticalAlignment = Element.ALIGN_MIDDLE;    // 垂直置中
                    table.AddCell(serCell);    //序列號

                    PdfPCell appCell = new PdfPCell(new Phrase(data.apply_name, cellFont));
                    table.AddCell(appCell);         //申請人

                    PdfPCell appIDCell = new PdfPCell(new Phrase(data.apply_id, cellFont));
                    table.AddCell(appIDCell);               //申請人身分證

                    PdfPCell nbNameCell = new PdfPCell(new Phrase(string.Join(",\n", data.newBorn_name), cellFont));
                    table.AddCell(nbNameCell);     //新生兒名字

                    PdfPCell nbIDCell = new PdfPCell(new Phrase(string.Join(",\n", data.newBorn_id), cellFont));
                    table.AddCell(nbIDCell);           //新生兒身分證

                    PdfPCell accNameCell = new PdfPCell(new Phrase(data.account_name, cellFont));
                    table.AddCell(accNameCell);       //受款人

                    PdfPCell accIDCell = new PdfPCell(new Phrase(data.account_ID, cellFont));
                    table.AddCell(accIDCell);            //受款人身分證

                    PdfPCell accDivCell = new PdfPCell(new Phrase(data.account_div, cellFont));
                    table.AddCell(accDivCell);            //郵局局號

                    PdfPCell accNumCell = new PdfPCell(new Phrase(data.account_number, cellFont));
                    table.AddCell(accNumCell);     //郵局帳號

                    PdfPCell allowanceCell = new PdfPCell(new Phrase((data.newBorn_name.Count * settingCtrl.allowance_per_nb).ToString(), cellFont));
                    table.AddCell(allowanceCell); //補助金額

                    PdfPCell noteCell = new PdfPCell(new Phrase("", cellFont));
                    table.AddCell(noteCell);   // 備註

                    idx_count++;
                }
            }

            // 完成表格行
            table.CompleteRow();

            // 設置絕對位置
            float xPos = 50f;  // X 坐標
            float yPos = document.PageSize.Height - 125f;  // Y 坐標，從頁面的頂部開始

            // 設置表格總寬度
            table.TotalWidth = document.PageSize.Width * 0.9f;
            table.LockedWidth = true;

            // 寫入表格到指定位置
            table.WriteSelectedRows(0, -1, xPos, yPos, cb);
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
            int year = 113;
            int month = 8;
            int day = 8;
            WriteContent(cb, $"{year} 年 {month} 月 {day} 日", sinhei_Content, 13,
            (document.PageSize.Width - document.RightMargin - 0),
            (document.PageSize.Height - document.TopMargin - 80),
            Element.ALIGN_RIGHT);

            // 畫底線
            DrawUnderLine(cb, titleText1, xT1, yT1);
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

        public void GeneratePDF(string saving_path, List<dataStruct> records, List<string> selected_ser_nums)
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
                DrawTable(document, cb, records, selected_ser_nums);

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
