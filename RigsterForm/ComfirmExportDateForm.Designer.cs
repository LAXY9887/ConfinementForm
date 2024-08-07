namespace RigsterForm
{
    partial class ComfirmExportDateForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.all_hist_btn = new System.Windows.Forms.Button();
            this.export_cancel = new System.Windows.Forms.Button();
            this.select_date_hist = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(32, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 28);
            this.label1.TabIndex = 0;
            this.label1.Text = "請選擇輸出範圍:";
            // 
            // all_hist_btn
            // 
            this.all_hist_btn.Font = new System.Drawing.Font("微軟正黑體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.all_hist_btn.Location = new System.Drawing.Point(49, 51);
            this.all_hist_btn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.all_hist_btn.Name = "all_hist_btn";
            this.all_hist_btn.Size = new System.Drawing.Size(133, 40);
            this.all_hist_btn.TabIndex = 1;
            this.all_hist_btn.Text = "全部歷史紀錄";
            this.all_hist_btn.UseVisualStyleBackColor = true;
            this.all_hist_btn.Click += new System.EventHandler(this.all_hist_btn_Click);
            // 
            // export_cancel
            // 
            this.export_cancel.Font = new System.Drawing.Font("微軟正黑體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.export_cancel.Location = new System.Drawing.Point(87, 158);
            this.export_cancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.export_cancel.Name = "export_cancel";
            this.export_cancel.Size = new System.Drawing.Size(56, 34);
            this.export_cancel.TabIndex = 3;
            this.export_cancel.Text = "取消";
            this.export_cancel.UseVisualStyleBackColor = true;
            this.export_cancel.Click += new System.EventHandler(this.export_cancel_Click);
            // 
            // select_date_hist
            // 
            this.select_date_hist.Font = new System.Drawing.Font("微軟正黑體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.select_date_hist.Location = new System.Drawing.Point(49, 100);
            this.select_date_hist.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.select_date_hist.Name = "select_date_hist";
            this.select_date_hist.Size = new System.Drawing.Size(133, 40);
            this.select_date_hist.TabIndex = 4;
            this.select_date_hist.Text = "日期篩選範圍";
            this.select_date_hist.UseVisualStyleBackColor = true;
            this.select_date_hist.Click += new System.EventHandler(this.select_date_hist_Click);
            // 
            // ComfirmExportDateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(242, 202);
            this.ControlBox = false;
            this.Controls.Add(this.select_date_hist);
            this.Controls.Add(this.export_cancel);
            this.Controls.Add(this.all_hist_btn);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ComfirmExportDateForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "輸出選項";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button all_hist_btn;
        private System.Windows.Forms.Button export_cancel;
        private System.Windows.Forms.Button select_date_hist;
    }
}