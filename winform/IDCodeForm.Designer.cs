namespace QQDemo
{
    partial class IDCodeForm
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
            this.pictureBoxID = new System.Windows.Forms.PictureBox();
            this.idCodetextBox = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxID)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxID
            // 
            this.pictureBoxID.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxID.Location = new System.Drawing.Point(13, 13);
            this.pictureBoxID.Name = "pictureBoxID";
            this.pictureBoxID.Size = new System.Drawing.Size(259, 116);
            this.pictureBoxID.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxID.TabIndex = 0;
            this.pictureBoxID.TabStop = false;
            // 
            // idCodetextBox
            // 
            this.idCodetextBox.Location = new System.Drawing.Point(13, 154);
            this.idCodetextBox.Name = "idCodetextBox";
            this.idCodetextBox.Size = new System.Drawing.Size(162, 21);
            this.idCodetextBox.TabIndex = 1;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(196, 154);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "确认";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // IDCodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 195);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.idCodetextBox);
            this.Controls.Add(this.pictureBoxID);
            this.MaximizeBox = false;
            this.Name = "IDCodeForm";
            this.Opacity = 0.95D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "请输入验证码：";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxID)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxID;
        private System.Windows.Forms.TextBox idCodetextBox;
        private System.Windows.Forms.Button buttonOK;
    }
}