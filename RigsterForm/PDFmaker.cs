using System;
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

        // 建構式
        public PDFmaker()
        {
            // 設置字體
            string kaiu = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KAIU.TTF");
            bfChinese = BaseFont.CreateFont(kaiu, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            string sinhei = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "msjh.ttc,0");
            sinhei_Content = BaseFont.CreateFont(sinhei, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
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

        public void GeneratePDF(string saving_path)
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
