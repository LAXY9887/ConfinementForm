namespace RigsterForm
{
    partial class SensorForm
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
            this.ApproveBtn = new System.Windows.Forms.Button();
            this.DeniedBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.UnSensorBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ApproveBtn
            // 
            this.ApproveBtn.Font = new System.Drawing.Font("微軟正黑體", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ApproveBtn.ForeColor = System.Drawing.Color.DarkGreen;
            this.ApproveBtn.Location = new System.Drawing.Point(117, 24);
            this.ApproveBtn.Name = "ApproveBtn";
            this.ApproveBtn.Size = new System.Drawing.Size(174, 59);
            this.ApproveBtn.TabIndex = 0;
            this.ApproveBtn.Text = "通過";
            this.ApproveBtn.UseVisualStyleBackColor = true;
            this.ApproveBtn.Click += new System.EventHandler(this.ApproveBtn_Click);
            // 
            // DeniedBtn
            // 
            this.DeniedBtn.Font = new System.Drawing.Font("微軟正黑體", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.DeniedBtn.ForeColor = System.Drawing.Color.DarkRed;
            this.DeniedBtn.Location = new System.Drawing.Point(117, 89);
            this.DeniedBtn.Name = "DeniedBtn";
            this.DeniedBtn.Size = new System.Drawing.Size(174, 59);
            this.DeniedBtn.TabIndex = 1;
            this.DeniedBtn.Text = "不通過";
            this.DeniedBtn.UseVisualStyleBackColor = true;
            this.DeniedBtn.Click += new System.EventHandler(this.DeniedBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Font = new System.Drawing.Font("微軟正黑體", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cancelBtn.Location = new System.Drawing.Point(117, 219);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(174, 59);
            this.cancelBtn.TabIndex = 2;
            this.cancelBtn.Text = "取消";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // UnSensorBtn
            // 
            this.UnSensorBtn.Font = new System.Drawing.Font("微軟正黑體", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.UnSensorBtn.ForeColor = System.Drawing.Color.MidnightBlue;
            this.UnSensorBtn.Location = new System.Drawing.Point(117, 154);
            this.UnSensorBtn.Name = "UnSensorBtn";
            this.UnSensorBtn.Size = new System.Drawing.Size(174, 59);
            this.UnSensorBtn.TabIndex = 3;
            this.UnSensorBtn.Text = "未審核";
            this.UnSensorBtn.UseVisualStyleBackColor = true;
            this.UnSensorBtn.Click += new System.EventHandler(this.UnSensorBtn_Click);
            // 
            // SensorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 305);
            this.Controls.Add(this.UnSensorBtn);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.DeniedBtn);
            this.Controls.Add(this.ApproveBtn);
            this.Name = "SensorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "審核案件";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ApproveBtn;
        private System.Windows.Forms.Button DeniedBtn;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button UnSensorBtn;
    }
}