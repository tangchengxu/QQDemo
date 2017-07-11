using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QQDemo.databus;

namespace QQDemo
{
    public partial class SmsForm : Form
    {
        DataBus mDataBus;

        public SmsForm(DataBus bus)
        {
            InitializeComponent();

            mDataBus = bus;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string smsCode = textBoxSms.Text;

            if (mDataBus.WriteSmsCode(smsCode))
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.No;
            }
            this.Close();
        }
    }
}
