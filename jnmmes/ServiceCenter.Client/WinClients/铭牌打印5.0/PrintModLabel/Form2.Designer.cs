namespace PrintModLabel
{
    partial class Form2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.button1 = new System.Windows.Forms.Button();
            this.txtContent = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblPowerName = new System.Windows.Forms.Label();
            this.lblColor = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblGrade = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblCoefIPM = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblCoefISC = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lblCoefPM = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lblPowersetSubCode = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lblCoefVPM = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.lblCoefVOC = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbLabels = new System.Windows.Forms.ComboBox();
            this.checkLot = new System.Windows.Forms.CheckBox();
            this.BtnLot = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(382, 30);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(146, 43);
            this.button1.TabIndex = 1;
            this.button1.Text = "铭牌打印";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtContent
            // 
            this.txtContent.Location = new System.Drawing.Point(150, 48);
            this.txtContent.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtContent.MaxLength = 15;
            this.txtContent.Name = "txtContent";
            this.txtContent.Size = new System.Drawing.Size(215, 25);
            this.txtContent.TabIndex = 0;
            this.txtContent.TextChanged += new System.EventHandler(this.txtContent_TextChanged);
            this.txtContent.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtContent_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(340, 98);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "图片：";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(344, 122);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(561, 402);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoEllipsis = true;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 13F);
            this.label2.Location = new System.Drawing.Point(28, 49);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 24);
            this.label2.TabIndex = 4;
            this.label2.Text = "组件批次号：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(24, 98);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 28);
            this.label3.TabIndex = 5;
            this.label3.Text = "档位：";
            // 
            // lblPowerName
            // 
            this.lblPowerName.AutoSize = true;
            this.lblPowerName.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblPowerName.Location = new System.Drawing.Point(120, 98);
            this.lblPowerName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPowerName.Name = "lblPowerName";
            this.lblPowerName.Size = new System.Drawing.Size(0, 28);
            this.lblPowerName.TabIndex = 6;
            // 
            // lblColor
            // 
            this.lblColor.AutoSize = true;
            this.lblColor.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblColor.Location = new System.Drawing.Point(134, 490);
            this.lblColor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(0, 28);
            this.lblColor.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(24, 490);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 28);
            this.label6.TabIndex = 7;
            this.label6.Text = "花色：";
            // 
            // lblGrade
            // 
            this.lblGrade.AutoSize = true;
            this.lblGrade.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblGrade.Location = new System.Drawing.Point(110, 268);
            this.lblGrade.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGrade.Name = "lblGrade";
            this.lblGrade.Size = new System.Drawing.Size(0, 28);
            this.lblGrade.TabIndex = 10;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(23, 268);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 28);
            this.label8.TabIndex = 9;
            this.label8.Text = "等级：";
            // 
            // lblCoefIPM
            // 
            this.lblCoefIPM.AutoSize = true;
            this.lblCoefIPM.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCoefIPM.Location = new System.Drawing.Point(119, 441);
            this.lblCoefIPM.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCoefIPM.Name = "lblCoefIPM";
            this.lblCoefIPM.Size = new System.Drawing.Size(0, 28);
            this.lblCoefIPM.TabIndex = 16;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(22, 441);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(73, 28);
            this.label10.TabIndex = 15;
            this.label10.Text = "IPM：";
            // 
            // lblCoefISC
            // 
            this.lblCoefISC.AutoSize = true;
            this.lblCoefISC.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCoefISC.Location = new System.Drawing.Point(120, 329);
            this.lblCoefISC.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCoefISC.Name = "lblCoefISC";
            this.lblCoefISC.Size = new System.Drawing.Size(0, 28);
            this.lblCoefISC.TabIndex = 14;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(24, 320);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 28);
            this.label12.TabIndex = 13;
            this.label12.Text = "ISC：";
            // 
            // lblCoefPM
            // 
            this.lblCoefPM.AutoSize = true;
            this.lblCoefPM.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCoefPM.Location = new System.Drawing.Point(112, 179);
            this.lblCoefPM.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCoefPM.Name = "lblCoefPM";
            this.lblCoefPM.Size = new System.Drawing.Size(0, 28);
            this.lblCoefPM.TabIndex = 12;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label14.Location = new System.Drawing.Point(22, 179);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(75, 28);
            this.label14.TabIndex = 11;
            this.label14.Text = "功率：";
            // 
            // lblPowersetSubCode
            // 
            this.lblPowersetSubCode.AutoSize = true;
            this.lblPowersetSubCode.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblPowersetSubCode.Location = new System.Drawing.Point(148, 138);
            this.lblPowersetSubCode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPowersetSubCode.Name = "lblPowersetSubCode";
            this.lblPowersetSubCode.Size = new System.Drawing.Size(0, 28);
            this.lblPowersetSubCode.TabIndex = 22;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label16.Location = new System.Drawing.Point(24, 138);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(96, 28);
            this.label16.TabIndex = 21;
            this.label16.Text = "子档位：";
            // 
            // lblCoefVPM
            // 
            this.lblCoefVPM.AutoSize = true;
            this.lblCoefVPM.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCoefVPM.Location = new System.Drawing.Point(122, 382);
            this.lblCoefVPM.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCoefVPM.Name = "lblCoefVPM";
            this.lblCoefVPM.Size = new System.Drawing.Size(0, 28);
            this.lblCoefVPM.TabIndex = 20;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label18.Location = new System.Drawing.Point(24, 382);
            this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(81, 28);
            this.label18.TabIndex = 19;
            this.label18.Text = "VPM：";
            // 
            // lblCoefVOC
            // 
            this.lblCoefVOC.AutoSize = true;
            this.lblCoefVOC.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCoefVOC.Location = new System.Drawing.Point(116, 218);
            this.lblCoefVOC.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCoefVOC.Name = "lblCoefVOC";
            this.lblCoefVOC.Size = new System.Drawing.Size(0, 28);
            this.lblCoefVOC.TabIndex = 18;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label20.Location = new System.Drawing.Point(23, 218);
            this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(78, 28);
            this.label20.TabIndex = 17;
            this.label20.Text = "VOC：";
            // 
            // label4
            // 
            this.label4.AutoEllipsis = true;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 13F);
            this.label4.Location = new System.Drawing.Point(28, 9);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 24);
            this.label4.TabIndex = 23;
            this.label4.Text = "铭牌模板：";
            // 
            // cmbLabels
            // 
            this.cmbLabels.Enabled = false;
            this.cmbLabels.FormattingEnabled = true;
            this.cmbLabels.Location = new System.Drawing.Point(150, 9);
            this.cmbLabels.Name = "cmbLabels";
            this.cmbLabels.Size = new System.Drawing.Size(212, 27);
            this.cmbLabels.TabIndex = 24;
            // 
            // checkLot
            // 
            this.checkLot.AutoSize = true;
            this.checkLot.Location = new System.Drawing.Point(382, -1);
            this.checkLot.Name = "checkLot";
            this.checkLot.Size = new System.Drawing.Size(93, 23);
            this.checkLot.TabIndex = 25;
            this.checkLot.Text = "打印批次号";
            this.checkLot.UseVisualStyleBackColor = true;
            // 
            // BtnLot
            // 
            this.BtnLot.Location = new System.Drawing.Point(566, 30);
            this.BtnLot.Name = "BtnLot";
            this.BtnLot.Size = new System.Drawing.Size(128, 43);
            this.BtnLot.TabIndex = 26;
            this.BtnLot.Text = "批次号打印";
            this.BtnLot.UseVisualStyleBackColor = true;
            this.BtnLot.Click += new System.EventHandler(this.BtnLot_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 531);
            this.Controls.Add(this.BtnLot);
            this.Controls.Add(this.checkLot);
            this.Controls.Add(this.cmbLabels);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblPowersetSubCode);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.lblCoefVPM);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.lblCoefVOC);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.lblCoefIPM);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.lblCoefISC);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.lblCoefPM);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.lblGrade);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lblColor);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblPowerName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtContent);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "铭牌打印";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form2_FormClosing);
            this.Load += new System.EventHandler(this.Form2_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblPowerName;
        private System.Windows.Forms.Label lblColor;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblGrade;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblCoefIPM;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblCoefISC;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblCoefPM;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lblPowersetSubCode;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblCoefVPM;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label lblCoefVOC;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbLabels;
        private System.Windows.Forms.CheckBox checkLot;
        private System.Windows.Forms.Button BtnLot;
    }
}