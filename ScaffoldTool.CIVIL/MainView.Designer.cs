namespace CADImportCommon
{
    partial class MainView
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.panelBox_Elevation = new System.Windows.Forms.Panel();
            this.btnLogPlaneLine = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnVertical = new DevComponents.DotNetBar.ButtonX();
            this.SuspendLayout();
            // 
            // cadPictBox
            // 
            this.cadPictBox.Dock = System.Windows.Forms.DockStyle.None;
            this.cadPictBox.Location = new System.Drawing.Point(1, 320);
            this.cadPictBox.Size = new System.Drawing.Size(1205, 257);
            this.cadPictBox.VirtualSize = new System.Drawing.Size(423, 257);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(830, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(911, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(993, 11);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // panelBox_Elevation
            // 
            this.panelBox_Elevation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBox_Elevation.BackColor = System.Drawing.Color.Black;
            this.panelBox_Elevation.Location = new System.Drawing.Point(1, 41);
            this.panelBox_Elevation.Name = "panelBox_Elevation";
            this.panelBox_Elevation.Size = new System.Drawing.Size(1205, 274);
            this.panelBox_Elevation.TabIndex = 0;
            // 
            // btnLogPlaneLine
            // 
            this.btnLogPlaneLine.Location = new System.Drawing.Point(12, 12);
            this.btnLogPlaneLine.Name = "btnLogPlaneLine";
            this.btnLogPlaneLine.Size = new System.Drawing.Size(95, 23);
            this.btnLogPlaneLine.TabIndex = 4;
            this.btnLogPlaneLine.Text = "载入桥梁中线线";
            this.btnLogPlaneLine.UseVisualStyleBackColor = true;
            this.btnLogPlaneLine.Click += new System.EventHandler(this.btnLogPlaneLine_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnVertical
            // 
            this.btnVertical.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnVertical.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnVertical.Location = new System.Drawing.Point(113, 12);
            this.btnVertical.Name = "btnVertical";
            this.btnVertical.Size = new System.Drawing.Size(83, 23);
            this.btnVertical.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnVertical.TabIndex = 5;
            this.btnVertical.Text = "纵断面曲线";
            this.btnVertical.Click += new System.EventHandler(this.btnVertical_Click);
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1206, 646);
            this.Controls.Add(this.btnVertical);
            this.Controls.Add(this.btnLogPlaneLine);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panelBox_Elevation);
            this.Name = "MainView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.CADImprot_Load);
            this.Click += new System.EventHandler(this.CADImprot_Click);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CADImprot_MouseClick);
            this.Controls.SetChildIndex(this.panelBox_Elevation, 0);
            this.Controls.SetChildIndex(this.button1, 0);
            this.Controls.SetChildIndex(this.button2, 0);
            this.Controls.SetChildIndex(this.button3, 0);
            this.Controls.SetChildIndex(this.btnLogPlaneLine, 0);
            this.Controls.SetChildIndex(this.cadPictBox, 0);
            this.Controls.SetChildIndex(this.btnVertical, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private CADForm cadBoxForm;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Panel panelBox_Elevation;
        private System.Windows.Forms.Button btnLogPlaneLine;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private DevComponents.DotNetBar.ButtonX btnVertical;
    }
}

