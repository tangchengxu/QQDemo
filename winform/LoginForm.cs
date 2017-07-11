using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MsgExpress;
using MsgExpress.databus;
using QQDemo.databus;
using QQDemo.thread;
using QQDemo.util;

namespace QQDemo
{
    public partial class LoginForm : Form
    {
        DataBus mDataBus = null;
        InitThread mInitTd = null;
        DataRelay mRelayThread = null;

        const string mQQUserName = "3449147393";
        const string mQQPassWord = "sumscope";
        //const string mQQUserName = "";
        //const string mQQPassWord = "";

        public LoginForm()
        {
            InitializeComponent();
            comboBoxQQType.SelectedIndex = 0;
            this.userName.Text = mQQUserName;
            this.passWord.Text = mQQPassWord;

            mDataBus = new DataBus();
            mRelayThread = new DataRelay(mDataBus);
            mRelayThread.Start();
        }

        private void login_Click(object sender, EventArgs e)
        {
            String uName = userName.Text;
            String pWord = passWord.Text;
            if (uName == null || uName.Length == 0 || pWord == null || pWord.Length == 0)
            {
                errInfo.Text = "请输入用户名和密码！";
                return;
            }

            LoginQQ(uName, pWord, comboBoxQQType.SelectedIndex);
        }

        void LoginQQ(String userName, String passWord, int qqType)
        {
            bool bOk = mDataBus.InitToken(userName);
            if (bOk)
            {
                bOk = mDataBus.LoginGateWay();
            }

            ELoginError errorCode = ELoginError.LoginError_Other;
            if (bOk)
            {
                errorCode = mDataBus.Login(userName, passWord, qqType); 
            }
            bOk = false;

            if (ELoginError.LoginError_Success == errorCode)
            {
                bOk = true;
            }
            else if (ELoginError.LoginError_NeedId == errorCode)
            {
                errInfo.Text = "";
                IDCodeForm form = new IDCodeForm(mDataBus);
                DialogResult result = form.ShowDialog();
                if (DialogResult.OK == result)
                {
                    bOk = true;
                }
                else if (DialogResult.No == result)
                {
                    errInfo.Text = "验证码输入异常！";
                }
                else if (DialogResult.Cancel == result)
                {
                    errInfo.Text = "用户名密码错误！";
                }
                else if (DialogResult.Ignore == result)
                {
                    SmsForm smsForm = new SmsForm(mDataBus);
                    DialogResult smsResult = smsForm.ShowDialog();
                    if (DialogResult.OK == smsResult)
                    {
                        bOk = true;
                    }
                    else
                    {
                        errInfo.Text = "输入手机验证码失败！";
                    }
                }
            }
            else if (ELoginError.LoginError_WrongUserOrPass == errorCode)
            {
                errInfo.Text = "用户名或密码错误！";
            }
            else if (ELoginError.LoginError_Other == errorCode)
            {
                errInfo.Text = "发送请求失败！";
            }
            else if (ELoginError.LoginError_NeedSmsId == errorCode)
            {
                SmsForm smsForm = new SmsForm(mDataBus);
                DialogResult smsResult = smsForm.ShowDialog();
                if (DialogResult.OK == smsResult)
                {
                    bOk = true;
                }
                else
                {
                    errInfo.Text = "输入手机验证码失败！";
                }
            }
            else
            {
                errInfo.Text = "未知错误：" + errorCode;
            }

            if (bOk)
            {
                errInfo.Text = "";

                ChatForm form = new ChatForm(mDataBus);
                form.Show();
                this.Visible = false;

                mInitTd = new InitThread(mDataBus);
                mInitTd.Start();
            }
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (null != mInitTd)
            {
                mInitTd.Stop();
            }
            if (null != mRelayThread)
            {
                mRelayThread.Stop();
            }

            if (null != mDataBus)
            {
                mDataBus.Release();
            }

            System.Environment.Exit(0);
        }
    }
}
