namespace ELImageDownload
{
    partial class Form1
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.but_Query = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btn_Download = new System.Windows.Forms.Button();
            this.txt_PackageNo = new System.Windows.Forms.TextBox();
            this.txt_address = new System.Windows.Forms.TextBox();
            this.lab_packageNo = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.lab_name = new System.Windows.Forms.Label();
            this.btn_logout = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_LotNo = new System.Windows.Forms.TextBox();
            this.btn_LotQuery = new System.Windows.Forms.Button();
            this.btn_LotDownload = new System.Windows.Forms.Button();
            this.lab_msg = new System.Windows.Forms.Label();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // but_Query
            // 
            this.but_Query.Location = new System.Drawing.Point(548, 22);
            this.but_Query.Name = "but_Query";
            this.but_Query.Size = new System.Drawing.Size(75, 23);
            this.but_Query.TabIndex = 0;
            this.but_Query.Text = "查询";
            this.but_Query.UseVisualStyleBackColor = true;
            this.but_Query.Click += new System.EventHandler(this.but_Query_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(449, 167);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "选择文件夹";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btn_Download
            // 
            this.btn_Download.Location = new System.Drawing.Point(629, 22);
            this.btn_Download.Name = "btn_Download";
            this.btn_Download.Size = new System.Drawing.Size(75, 23);
            this.btn_Download.TabIndex = 2;
            this.btn_Download.Text = "下载";
            this.btn_Download.UseVisualStyleBackColor = true;
            this.btn_Download.Click += new System.EventHandler(this.btn_Download_Click);
            // 
            // txt_PackageNo
            // 
            this.txt_PackageNo.Location = new System.Drawing.Point(104, 22);
            this.txt_PackageNo.Multiline = true;
            this.txt_PackageNo.Name = "txt_PackageNo";
            this.txt_PackageNo.Size = new System.Drawing.Size(420, 62);
            this.txt_PackageNo.TabIndex = 3;
            // 
            // txt_address
            // 
            this.txt_address.Location = new System.Drawing.Point(104, 167);
            this.txt_address.Name = "txt_address";
            this.txt_address.Size = new System.Drawing.Size(339, 21);
            this.txt_address.TabIndex = 4;
            // 
            // lab_packageNo
            // 
            this.lab_packageNo.AutoSize = true;
            this.lab_packageNo.Location = new System.Drawing.Point(39, 22);
            this.lab_packageNo.Name = "lab_packageNo";
            this.lab_packageNo.Size = new System.Drawing.Size(41, 12);
            this.lab_packageNo.TabIndex = 5;
            this.lab_packageNo.Text = "包装号";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 170);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "文件夹地址";
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 216);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(913, 256);
            this.dataGridView1.TabIndex = 7;
            this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(785, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "你好：";
            // 
            // lab_name
            // 
            this.lab_name.AutoSize = true;
            this.lab_name.Location = new System.Drawing.Point(833, 21);
            this.lab_name.Name = "lab_name";
            this.lab_name.Size = new System.Drawing.Size(41, 12);
            this.lab_name.TabIndex = 9;
            this.lab_name.Text = "label3";
            // 
            // btn_logout
            // 
            this.btn_logout.Location = new System.Drawing.Point(787, 37);
            this.btn_logout.Name = "btn_logout";
            this.btn_logout.Size = new System.Drawing.Size(75, 23);
            this.btn_logout.TabIndex = 10;
            this.btn_logout.Text = "注销";
            this.btn_logout.UseVisualStyleBackColor = true;
            this.btn_logout.Click += new System.EventHandler(this.btn_logout_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "批次号";
            // 
            // txt_LotNo
            // 
            this.txt_LotNo.Location = new System.Drawing.Point(104, 90);
            this.txt_LotNo.Multiline = true;
            this.txt_LotNo.Name = "txt_LotNo";
            this.txt_LotNo.Size = new System.Drawing.Size(420, 71);
            this.txt_LotNo.TabIndex = 12;
            // 
            // btn_LotQuery
            // 
            this.btn_LotQuery.Location = new System.Drawing.Point(548, 90);
            this.btn_LotQuery.Name = "btn_LotQuery";
            this.btn_LotQuery.Size = new System.Drawing.Size(75, 23);
            this.btn_LotQuery.TabIndex = 13;
            this.btn_LotQuery.Text = "查询";
            this.btn_LotQuery.UseVisualStyleBackColor = true;
            this.btn_LotQuery.Click += new System.EventHandler(this.btn_LotQuery_Click);
            // 
            // btn_LotDownload
            // 
            this.btn_LotDownload.Location = new System.Drawing.Point(629, 90);
            this.btn_LotDownload.Name = "btn_LotDownload";
            this.btn_LotDownload.Size = new System.Drawing.Size(75, 23);
            this.btn_LotDownload.TabIndex = 14;
            this.btn_LotDownload.Text = "下载";
            this.btn_LotDownload.UseVisualStyleBackColor = true;
            this.btn_LotDownload.Click += new System.EventHandler(this.btn_LotDownload_Click);
            // 
            // lab_msg
            // 
            this.lab_msg.AutoSize = true;
            this.lab_msg.Location = new System.Drawing.Point(548, 148);
            this.lab_msg.Name = "lab_msg";
            this.lab_msg.Size = new System.Drawing.Size(53, 12);
            this.lab_msg.TabIndex = 15;
            this.lab_msg.Text = "欢迎使用";
            // 
            // dataGridView2
            // 
            this.dataGridView2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(0, 20);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dataGridView2.RowTemplate.Height = 23;
            this.dataGridView2.Size = new System.Drawing.Size(913, 298);
            this.dataGridView2.TabIndex = 16;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataGridView2);
            this.groupBox1.Location = new System.Drawing.Point(12, 502);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(913, 303);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "未下载的图片";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(937, 817);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lab_msg);
            this.Controls.Add(this.btn_LotDownload);
            this.Controls.Add(this.btn_LotQuery);
            this.Controls.Add(this.txt_LotNo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_logout);
            this.Controls.Add(this.lab_name);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lab_packageNo);
            this.Controls.Add(this.txt_address);
            this.Controls.Add(this.txt_PackageNo);
            this.Controls.Add(this.btn_Download);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.but_Query);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "下载";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button but_Query;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btn_Download;
        private System.Windows.Forms.TextBox txt_PackageNo;
        private System.Windows.Forms.TextBox txt_address;
        private System.Windows.Forms.Label lab_packageNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lab_name;
        private System.Windows.Forms.Button btn_logout;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_LotNo;
        private System.Windows.Forms.Button btn_LotQuery;
        private System.Windows.Forms.Button btn_LotDownload;
        private System.Windows.Forms.Label lab_msg;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

