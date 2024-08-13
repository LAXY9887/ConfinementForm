namespace RigsterForm
{
    partial class ReasomForm
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
            this.label_reason = new System.Windows.Forms.Label();
            this.textBoxreason = new System.Windows.Forms.TextBox();
            this.reason_confirmBtn = new System.Windows.Forms.Button();
            this.reason_cancelBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label_reason
            // 
            this.label_reason.AutoSize = true;
            this.label_reason.Font = new System.Drawing.Font("微軟正黑體", 22.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_reason.Location = new System.Drawing.Point(12, 9);
            this.label_reason.Name = "label_reason";
            this.label_reason.Size = new System.Drawing.Size(214, 47);
            this.label_reason.TabIndex = 0;
            this.label_reason.Text = "不符合事由:";
            // 
            // textBoxreason
            // 
            this.textBoxreason.Font = new System.Drawing.Font("微軟正黑體", 22.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBoxreason.Location = new System.Drawing.Point(20, 72);
            this.textBoxreason.Name = "textBoxreason";
            this.textBoxreason.Size = new System.Drawing.Size(760, 57);
            this.textBoxreason.TabIndex = 1;
            // 
            // reason_confirmBtn
            // 
            this.reason_confirmBtn.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.reason_confirmBtn.Location = new System.Drawing.Point(527, 149);
            this.reason_confirmBtn.Name = "reason_confirmBtn";
            this.reason_confirmBtn.Size = new System.Drawing.Size(119, 46);
            this.reason_confirmBtn.TabIndex = 2;
            this.reason_confirmBtn.Text = "確定";
            this.reason_confirmBtn.UseVisualStyleBackColor = true;
            this.reason_confirmBtn.Click += new System.EventHandler(this.reason_confirmBtn_Click);
            // 
            // reason_cancelBtn
            // 
            this.reason_cancelBtn.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.reason_cancelBtn.ForeColor = System.Drawing.Color.Maroon;
            this.reason_cancelBtn.Location = new System.Drawing.Point(661, 149);
            this.reason_cancelBtn.Name = "reason_cancelBtn";
            this.reason_cancelBtn.Size = new System.Drawing.Size(119, 46);
            this.reason_cancelBtn.TabIndex = 3;
            this.reason_cancelBtn.Text = "取消";
            this.reason_cancelBtn.UseVisualStyleBackColor = true;
            this.reason_cancelBtn.Click += new System.EventHandler(this.reason_cancelBtn_Click);
            // 
            // ReasomForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 210);
            this.Controls.Add(this.reason_cancelBtn);
            this.Controls.Add(this.reason_confirmBtn);
            this.Controls.Add(this.textBoxreason);
            this.Controls.Add(this.label_reason);
            this.Name = "ReasomForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "審核視窗";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_reason;
        private System.Windows.Forms.TextBox textBoxreason;
        private System.Windows.Forms.Button reason_confirmBtn;
        private System.Windows.Forms.Button reason_cancelBtn;
    }
}