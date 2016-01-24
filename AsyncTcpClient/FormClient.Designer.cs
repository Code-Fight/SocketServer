namespace AsyncTcpClient
{
    partial class FormClient
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
            this.label1 = new System.Windows.Forms.Label();
            this.txt_UserName = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btn_SendeMessage = new System.Windows.Forms.Button();
            this.rtf_SendMessage = new System.Windows.Forms.RichTextBox();
            this.infolist = new System.Windows.Forms.ListBox();
            this.statuslist = new System.Windows.Forms.ListBox();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "name：";
            // 
            // txt_UserName
            // 
            this.txt_UserName.Location = new System.Drawing.Point(79, 12);
            this.txt_UserName.Name = "txt_UserName";
            this.txt_UserName.Size = new System.Drawing.Size(147, 21);
            this.txt_UserName.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.infolist);
            this.groupBox2.Location = new System.Drawing.Point(11, 39);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(629, 140);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Message";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.comboBox1);
            this.groupBox3.Controls.Add(this.btn_SendeMessage);
            this.groupBox3.Controls.Add(this.rtf_SendMessage);
            this.groupBox3.Location = new System.Drawing.Point(11, 176);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(629, 83);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "发送信息";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "10001",
            "10002",
            "10004",
            "10005",
            "10006",
            "10007",
            "10008",
            "10009",
            "10010"});
            this.comboBox1.Location = new System.Drawing.Point(526, 21);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(86, 20);
            this.comboBox1.TabIndex = 4;
            // 
            // btn_SendeMessage
            // 
            this.btn_SendeMessage.Location = new System.Drawing.Point(526, 56);
            this.btn_SendeMessage.Name = "btn_SendeMessage";
            this.btn_SendeMessage.Size = new System.Drawing.Size(86, 23);
            this.btn_SendeMessage.TabIndex = 3;
            this.btn_SendeMessage.Text = "发 送";
            this.btn_SendeMessage.UseVisualStyleBackColor = true;
            this.btn_SendeMessage.Click += new System.EventHandler(this.btn_SendeMessage_Click);
            // 
            // rtf_SendMessage
            // 
            this.rtf_SendMessage.Location = new System.Drawing.Point(15, 18);
            this.rtf_SendMessage.Name = "rtf_SendMessage";
            this.rtf_SendMessage.Size = new System.Drawing.Size(502, 61);
            this.rtf_SendMessage.TabIndex = 1;
            this.rtf_SendMessage.Text = "";
            // 
            // infolist
            // 
            this.infolist.FormattingEnabled = true;
            this.infolist.ItemHeight = 12;
            this.infolist.Location = new System.Drawing.Point(11, 26);
            this.infolist.Name = "infolist";
            this.infolist.Size = new System.Drawing.Size(612, 100);
            this.infolist.TabIndex = 1;
            // 
            // statuslist
            // 
            this.statuslist.FormattingEnabled = true;
            this.statuslist.ItemHeight = 12;
            this.statuslist.Location = new System.Drawing.Point(22, 270);
            this.statuslist.Name = "statuslist";
            this.statuslist.Size = new System.Drawing.Size(612, 88);
            this.statuslist.TabIndex = 6;
            // 
            // FormClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 370);
            this.Controls.Add(this.statuslist);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.txt_UserName);
            this.Controls.Add(this.label1);
            this.Name = "FormClient";
            this.Text = "客户端";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClient_FormClosing);
            this.Load += new System.EventHandler(this.FormClient_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_UserName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RichTextBox rtf_SendMessage;
        private System.Windows.Forms.Button btn_SendeMessage;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ListBox infolist;
        private System.Windows.Forms.ListBox statuslist;
    }
}

