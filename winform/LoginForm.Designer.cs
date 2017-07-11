namespace QQDemo
{
    partial class LoginForm
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
            this.labelUserName = new System.Windows.Forms.Label();
            this.labelPassWord = new System.Windows.Forms.Label();
            this.userName = new System.Windows.Forms.TextBox();
            this.passWord = new System.Windows.Forms.TextBox();
            this.login = new System.Windows.Forms.Button();
            this.errInfo = new System.Windows.Forms.Label();
            this.labelQQType = new System.Windows.Forms.Label();
            this.comboBoxQQType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // labelUserName
            // 
            this.labelUserName.AutoSize = true;
            this.labelUserName.Location = new System.Drawing.Point(28, 40);
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Size = new System.Drawing.Size(53, 12);
            this.labelUserName.TabIndex = 0;
            this.labelUserName.Text = "用户名：";
            // 
            // labelPassWord
            // 
            this.labelPassWord.AutoSize = true;
            this.labelPassWord.Location = new System.Drawing.Point(28, 72);
            this.labelPassWord.Name = "labelPassWord";
            this.labelPassWord.Size = new System.Drawing.Size(53, 12);
            this.labelPassWord.TabIndex = 1;
            this.labelPassWord.Text = "密  码：";
            // 
            // userName
            // 
            this.userName.Location = new System.Drawing.Point(88, 40);
            this.userName.MaxLength = 126;
            this.userName.Name = "userName";
            this.userName.Size = new System.Drawing.Size(178, 21);
            this.userName.TabIndex = 2;
            this.userName.WordWrap = false;
            // 
            // passWord
            // 
            this.passWord.Location = new System.Drawing.Point(88, 68);
            this.passWord.MaxLength = 126;
            this.passWord.Name = "passWord";
            this.passWord.Size = new System.Drawing.Size(178, 21);
            this.passWord.TabIndex = 3;
            this.passWord.UseSystemPasswordChar = true;
            this.passWord.WordWrap = false;
            // 
            // login
            // 
            this.login.Location = new System.Drawing.Point(32, 132);
            this.login.Name = "login";
            this.login.Size = new System.Drawing.Size(236, 23);
            this.login.TabIndex = 4;
            this.login.Text = "登录";
            this.login.UseVisualStyleBackColor = true;
            this.login.Click += new System.EventHandler(this.login_Click);
            // 
            // errInfo
            // 
            this.errInfo.AutoSize = true;
            this.errInfo.ForeColor = System.Drawing.Color.Red;
            this.errInfo.Location = new System.Drawing.Point(28, 143);
            this.errInfo.Name = "errInfo";
            this.errInfo.Size = new System.Drawing.Size(0, 12);
            this.errInfo.TabIndex = 5;
            // 
            // labelQQType
            // 
            this.labelQQType.AutoSize = true;
            this.labelQQType.Location = new System.Drawing.Point(30, 101);
            this.labelQQType.Name = "labelQQType";
            this.labelQQType.Size = new System.Drawing.Size(53, 12);
            this.labelQQType.TabIndex = 6;
            this.labelQQType.Text = "QQ类型：";
            // 
            // comboBoxQQType
            // 
            this.comboBoxQQType.FormattingEnabled = true;
            this.comboBoxQQType.Items.AddRange(new object[] {
            "普通版QQ",
            "企业版QQ"});
            this.comboBoxQQType.Location = new System.Drawing.Point(88, 98);
            this.comboBoxQQType.Name = "comboBoxQQType";
            this.comboBoxQQType.Size = new System.Drawing.Size(178, 20);
            this.comboBoxQQType.TabIndex = 7;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 183);
            this.Controls.Add(this.comboBoxQQType);
            this.Controls.Add(this.labelQQType);
            this.Controls.Add(this.errInfo);
            this.Controls.Add(this.login);
            this.Controls.Add(this.passWord);
            this.Controls.Add(this.userName);
            this.Controls.Add(this.labelPassWord);
            this.Controls.Add(this.labelUserName);
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.Opacity = 0.9D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QQ登录";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LoginForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelUserName;
        private System.Windows.Forms.Label labelPassWord;
        private System.Windows.Forms.TextBox userName;
        private System.Windows.Forms.TextBox passWord;
        private System.Windows.Forms.Button login;
        private System.Windows.Forms.Label errInfo;
        private System.Windows.Forms.Label labelQQType;
        private System.Windows.Forms.ComboBox comboBoxQQType;
    }
}

