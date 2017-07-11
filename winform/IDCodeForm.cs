using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using QQDemo.databus;

namespace QQDemo
{
    public partial class IDCodeForm : Form
    {
        DataBus mDataBus;

        public IDCodeForm(DataBus bus)
        {
            InitializeComponent();

            mDataBus = bus;
            LoadImage();
        }

        private void LoadImage()
        {
            Byte[] imag = mDataBus.GetImage();
            if (null != imag)
            {
                MemoryStream ms = new MemoryStream(imag);
                pictureBoxID.Image = System.Drawing.Image.FromStream(ms);
                ms.Close();
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string idCode = idCodetextBox.Text;

            EIDError errorCode = mDataBus.WriteIdCode(idCode);
            //DialogResult用来记录返回状态
            if (EIDError.IDError_Success == errorCode)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else if (EIDError.IDError_IdWrong == errorCode)
            {
                LoadImage();
            }
            else if (EIDError.IDError_Other == errorCode)
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            }
            else if (EIDError.IDError_WrongUserOrPass == errorCode)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            else if (EIDError.IDError_NeedSmsId == errorCode)
            {
                this.DialogResult = DialogResult.Ignore;
                this.Close();
            }
        }
    }
}
