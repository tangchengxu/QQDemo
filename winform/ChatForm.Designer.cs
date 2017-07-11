namespace QQDemo
{
    partial class ChatForm
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("用户组");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("群列表");
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeViewRoster = new System.Windows.Forms.TreeView();
            this.buttonSend = new System.Windows.Forms.Button();
            this.textBoxEdit = new System.Windows.Forms.TextBox();
            this.richTextBoxShow = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeViewRoster);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.buttonSend);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxEdit);
            this.splitContainer1.Panel2.Controls.Add(this.richTextBoxShow);
            this.splitContainer1.Size = new System.Drawing.Size(844, 539);
            this.splitContainer1.SplitterDistance = 258;
            this.splitContainer1.TabIndex = 0;
            // 
            // treeViewRoster
            // 
            this.treeViewRoster.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewRoster.HideSelection = false;
            this.treeViewRoster.HotTracking = true;
            this.treeViewRoster.Location = new System.Drawing.Point(0, 0);
            this.treeViewRoster.Name = "treeViewRoster";
            treeNode1.Name = "qqUserGroup";
            treeNode1.Text = "用户组";
            treeNode2.Name = "qqRoom";
            treeNode2.Text = "群列表";
            this.treeViewRoster.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            this.treeViewRoster.Size = new System.Drawing.Size(258, 539);
            this.treeViewRoster.TabIndex = 0;
            this.treeViewRoster.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewRoster_NodeMouseClick);
            // 
            // buttonSend
            // 
            this.buttonSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSend.Location = new System.Drawing.Point(493, 403);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(86, 133);
            this.buttonSend.TabIndex = 2;
            this.buttonSend.Text = "发送";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // textBoxEdit
            // 
            this.textBoxEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxEdit.Location = new System.Drawing.Point(4, 403);
            this.textBoxEdit.Multiline = true;
            this.textBoxEdit.Name = "textBoxEdit";
            this.textBoxEdit.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxEdit.Size = new System.Drawing.Size(483, 133);
            this.textBoxEdit.TabIndex = 1;
            // 
            // richTextBoxShow
            // 
            this.richTextBoxShow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxShow.Location = new System.Drawing.Point(4, 3);
            this.richTextBoxShow.Name = "richTextBoxShow";
            this.richTextBoxShow.Size = new System.Drawing.Size(575, 393);
            this.richTextBoxShow.TabIndex = 0;
            this.richTextBoxShow.Text = "";
            // 
            // ChatForm
            // 
            this.AcceptButton = this.buttonSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(844, 539);
            this.Controls.Add(this.splitContainer1);
            this.Name = "ChatForm";
            this.Text = "ChatForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ChatForm_FormClosed);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeViewRoster;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TextBox textBoxEdit;
        private System.Windows.Forms.RichTextBox richTextBoxShow;
    }
}