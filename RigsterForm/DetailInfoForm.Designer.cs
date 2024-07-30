namespace RigsterForm
{
    partial class DetailInfoForm
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
            this.closeDetailBtn = new System.Windows.Forms.Button();
            this.detailInfoBOX = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // closeDetailBtn
            // 
            this.closeDetailBtn.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.closeDetailBtn.Location = new System.Drawing.Point(403, 734);
            this.closeDetailBtn.Margin = new System.Windows.Forms.Padding(4);
            this.closeDetailBtn.Name = "closeDetailBtn";
            this.closeDetailBtn.Size = new System.Drawing.Size(97, 38);
            this.closeDetailBtn.TabIndex = 0;
            this.closeDetailBtn.Text = "關閉";
            this.closeDetailBtn.UseVisualStyleBackColor = true;
            this.closeDetailBtn.Click += new System.EventHandler(this.closeDetailBtn_Click);
            // 
            // detailInfoBOX
            // 
            this.detailInfoBOX.Font = new System.Drawing.Font("新細明體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.detailInfoBOX.Location = new System.Drawing.Point(4, 8);
            this.detailInfoBOX.Margin = new System.Windows.Forms.Padding(4);
            this.detailInfoBOX.Name = "detailInfoBOX";
            this.detailInfoBOX.ReadOnly = true;
            this.detailInfoBOX.ShortcutsEnabled = false;
            this.detailInfoBOX.Size = new System.Drawing.Size(903, 718);
            this.detailInfoBOX.TabIndex = 1;
            this.detailInfoBOX.Text = "";
            // 
            // DetailInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(912, 780);
            this.ControlBox = false;
            this.Controls.Add(this.detailInfoBOX);
            this.Controls.Add(this.closeDetailBtn);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DetailInfoForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "詳細資訊";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button closeDetailBtn;
        private System.Windows.Forms.RichTextBox detailInfoBOX;
    }
}