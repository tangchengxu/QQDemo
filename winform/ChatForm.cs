using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QQDemo.databus;
using QQServer;
using MsgExpress;
using MsgExpress.databus;
using Google.ProtocolBuffers;
using QQDemo.thread;
using QQDemo.common;
using QQDemo.util;
using System.Threading;

namespace QQDemo
{
    public partial class ChatForm : Form
    {
        QuoteForm mQuoteForm = null;
        DataBus mDataBus = null;

        int mSelectType = -1;
        string mSelectUin = "";

        Dictionary<string, TreeNode> mQQUserToNodeMap = new Dictionary<string, TreeNode>();
        Dictionary<string, TreeNode> mQQRoomToNodeMap = new Dictionary<string, TreeNode>();

        public ChatForm(DataBus bus)
        {
            InitializeComponent();

            DataManager.Instance.mDataChangeHander += this.OnDataNotify;

            mDataBus = bus;
            mQuoteForm = new QuoteForm();
            mQuoteForm.Show();
        }

        void OnDataNotify(EDataChangeType type, object param)
        {
            Invoke(new OnDataChange(delegate(EDataChangeType cType, object arg) {
                switch (cType)
                {
                    case EDataChangeType.Type_QQ_GroupInfo:
                        {
                            IList<GroupInfo> infoList = arg as IList<GroupInfo>;
                            this.OnQQFriendInfo(infoList);
                        }
                        break;
                    case EDataChangeType.Type_QQ_RoomInfo:
                        {
                            IList<RoomInfo> infoList = arg as IList<RoomInfo>;
                            this.OnQQRoomInfo(infoList);
                        }
                        break;
                    case EDataChangeType.Type_QQ_RoomMemberInfo:
                        {
                            QQRoomMemberChange memberChange = arg as QQRoomMemberChange;
                            this.OnQQRoomMemberInfo(memberChange);
                        }
                        break;
                    case EDataChangeType.Type_QQ_Message:
                        {
                            QQServer.PubReciveMessage msg = arg as QQServer.PubReciveMessage;
                            this.OnReciveQQMessage(msg);
                        }
                        break;
                    case EDataChangeType.Type_QQ_Disconnect:
                        {
                            this.Disconnect();
                        }
                        break;
                    default:
                        {

                        }
                        break;
                }
            }), new object[] { type, param });
        }

        private void ChatForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (null != mDataBus)
            {
                mDataBus.Release();
            }

            System.Environment.Exit(0);
        }

        // 显示QQ好友信息
        void OnQQFriendInfo(IList<GroupInfo> infoList)
        {
            if (null == infoList)
            {
                return;
            }
            TreeNode node = treeViewRoster.Nodes[0];
            foreach (GroupInfo info in infoList)
            {
                TreeNode groupNode = new TreeNode(info.GroupName);
                foreach (UserInfo uInfo in info.UserInfoList)
                {
                    TreeNode userNode = new TreeNode(uInfo.ShowName);
                    userNode.Tag = uInfo;
                    groupNode.Nodes.Add(userNode);

                    mQQUserToNodeMap.Add(uInfo.Uin, userNode);
                }
                node.Nodes.Add(groupNode);
            }
        }

        // 显示QQ群信息
        void OnQQRoomInfo(IList<RoomInfo> infoList)
        {
            if (null == infoList)
            {
                return;
            }
            TreeNode node = treeViewRoster.Nodes[1];
            foreach (RoomInfo roomInfo in infoList)
            {
                TreeNode roomNode = new TreeNode(roomInfo.RoomName);
                roomNode.Tag = roomInfo;
                node.Nodes.Add(roomNode);

                mQQRoomToNodeMap.Add(roomInfo.Code, roomNode);
            }
        }

        // 显示QQ群用户信息
        void OnQQRoomMemberInfo(QQRoomMemberChange memberChange)
        {
            if (null == memberChange || null == memberChange.code || null == memberChange.infoList)
            {
                return;
            }
            if (mQQRoomToNodeMap.ContainsKey(memberChange.code))
            {
                TreeNode roomNode = mQQRoomToNodeMap[memberChange.code];
                RoomInfo roomInfo = roomNode.Tag as RoomInfo;
                roomNode.Text = roomInfo.RoomName + "(" + memberChange.infoList.Count + "人)";

                foreach (QQServer.UserInfo userInfo in memberChange.infoList)
                {
                    TreeNode roomMemberNode = new TreeNode(userInfo.ShowName);
                    roomMemberNode.Tag = userInfo.Uin;
                    roomNode.Nodes.Add(roomMemberNode);

                    if (!DataManager.Instance.IsQQFriend(userInfo.Uin))
                    {
                        if (!mQQUserToNodeMap.ContainsKey(userInfo.Uin))
                        {
                            mQQUserToNodeMap.Add(userInfo.Uin, roomMemberNode);
                        }
                    }
                }
            }
        }

        // 刷新界面消息
        void RefreshMessage()
        {
            if (mSelectType < 0)
            {
                return;
            }
            string msgText = "";

            lock (DataManager.Instance)
            {
                List<PubReciveMessage> msgList = null;
                if (mSelectType == 0 || mSelectType == 2)
                {
                    msgList = DataManager.Instance.GetQQUserMsgList(mSelectUin);
                }
                else if (mSelectType == 1)
                {
                    msgList = DataManager.Instance.GetQQRoomMsgList(mSelectUin);
                }
                else
                {
                    return;
                }
                if (null != msgList)
                {
                    foreach (PubReciveMessage msg in msgList)
                    {
                        QQServer.UserInfo userInfo = DataManager.Instance.GetQQUserInfo(msg.FromUin);
                        if (null != userInfo)
                        {
                            msgText += userInfo.ShowName + ":\r\n";
                            msgText += "    " + Common.GetMsgText(msg) + "\r\n";
                        }
                    }
                }
            }

            richTextBoxShow.Text = msgText;
            richTextBoxShow.Focus();
            richTextBoxShow.Select(richTextBoxShow.TextLength, 0);
            richTextBoxShow.ScrollToCaret();
            textBoxEdit.Focus();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (mSelectType < 0)
            {
                MessageBox.Show("请选择发送对象！");
                return;
            }
            string msg = textBoxEdit.Text;

            string code = "";
            if (2 == mSelectType)
            {
                if (mQQUserToNodeMap.ContainsKey(mSelectUin))
                {
                    TreeNode node = mQQUserToNodeMap[mSelectUin];
                    RoomInfo info = node.Parent.Tag as RoomInfo;
                    if (null != info)
                    {
                        code = info.Uin;
                    }
                    else
                    {
                        MessageBox.Show("获取群信息失败！");
                        Logger.Error("Don't get roomInfo.");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("获取用户节点失败！");
                    Logger.Error("Can't find userNode");
                    return;
                }
            }
            this.SendMessage(mSelectType, mSelectUin, msg, code);
        }

        void SendMessage(int type, string uin, string msg, string code)
        {
            bool bOk = mDataBus.SendMessage(type, uin, msg, code) == 0;
            if (bOk && 1 != type)
            {
                DataManager.Instance.AddQQMessage(mDataBus.mUin, uin, msg);
            }

            if (bOk)
            {
                textBoxEdit.Text = "";
                RefreshMessage();

                if (0 == type || 2 == type)
                {
                    QQServer.UserInfo user = DataManager.Instance.GetQQUserInfo(uin);
                    if (null != user)
                    {
                        AddQuote(true, user.ShowName, uin, msg, Common.GetCurrentTime(), null, null, uin);
                    }
                    else
                    {
                        AddQuote(true, uin, uin, msg, Common.GetCurrentTime(), null, null, uin);
                    }
                }
            }
            else
            {
                MessageBox.Show("消息发送失败！");
            }
        }

        private void treeViewRoster_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode node = e.Node;
            if (null == node || node.Tag == null)
            {
                return;
            }

            if (node.Tag is UserInfo)
            {
                UserInfo user = node.Tag as UserInfo;
                mSelectType = 0;
                mSelectUin = user.Uin;

                node.Text = user.ShowName;
                this.Text = "QQ用户：" + user.ShowName;
            }
            else if (node.Tag is RoomInfo)
            {
                RoomInfo room = node.Tag as RoomInfo;
                mSelectType = 1;
                mSelectUin = room.Code;

                node.Text = room.RoomName + "(" + node.Nodes.Count + "人)";
                this.Text = "QQ群：" + room.RoomName;
            }
            else if (node.Tag is String)
            {
                String uin = node.Tag as String;
                if (uin.Equals(mDataBus.mUin))
                {
                    richTextBoxShow.Text = "";
                    textBoxEdit.ReadOnly = true;
                    return;
                }
                UserInfo user = DataManager.Instance.GetQQUserInfo(uin);
                if (DataManager.Instance.IsQQFriend(uin))
                {
                    mSelectType = 0;
                    mSelectUin = uin;

                    node.Text = user.ShowName;
                    this.Text = "QQ用户：" + user.ShowName;
                }
                else
                {
                    mSelectType = 2;
                    mSelectUin = uin;

                    node.Text = user.ShowName;
                    this.Text = "QQ陌生人：" + user.ShowName;
                }
            }
            textBoxEdit.ReadOnly = false;

            RefreshMessage();
        }

        void OnReciveQQMessage(QQServer.PubReciveMessage msg)
        {
            if (0 == msg.Type || 2 == msg.Type)
            {
                if (msg.FromUin == mSelectUin)
                {
                    RefreshMessage();
                }
                else
                {
                    if (mQQUserToNodeMap.ContainsKey(msg.FromUin))
                    {
                        TreeNode node = mQQUserToNodeMap[msg.FromUin];
                        if (0 == msg.Type)
                        {
                            UserInfo user = node.Tag as UserInfo;
                            if (null != user)
                            {
                                node.Text = user.ShowName + "(新消息)";
                            }
                        }
                        else
                        {
                            string uin = node.Tag as string;
                            QQServer.UserInfo user = DataManager.Instance.GetQQUserInfo(uin);
                            if (null != user)
                            {
                                node.Text = user.ShowName + "(新消息)";
                            }
                        }
                    }
                }

                QQServer.UserInfo userInfo = DataManager.Instance.GetQQUserInfo(msg.FromUin);
                if (null != userInfo)
                {
                    this.AddQuote(false, userInfo.ShowName, msg.FromUin, Common.GetMsgText(msg), msg.Time, null, msg.FromUin, null);
                }
            }
            else if (1 == msg.Type)
            {
                if (msg.ToUin == mSelectUin)
                {
                    RefreshMessage();
                }
                else
                {
                    if (mQQRoomToNodeMap.ContainsKey(msg.ToUin))
                    {
                        TreeNode node = mQQRoomToNodeMap[msg.ToUin];
                        RoomInfo info = node.Tag as RoomInfo;
                        if (null != info)
                        {
                            node.Text = info.RoomName + "(新消息)";
                        }
                    }
                }

                bool bSend = mDataBus.mUin.Equals(msg.FromUin);
                string showName = "";
                if (bSend)
                {
                    QQServer.RoomInfo roomInfo = DataManager.Instance.GetQQRoomInfo(msg.ToUin);
                    if (null != roomInfo)
                    {
                        showName = roomInfo.RoomName;
                    }
                }
                else
                {
                    QQServer.UserInfo userInfo = DataManager.Instance.GetQQUserInfo(msg.FromUin);
                    if (null != userInfo)
                    {
                        showName = userInfo.ShowName;
                    }
                }
                this.AddQuote(bSend, showName, msg.FromUin, Common.GetMsgText(msg), msg.Time, msg.ToUin, msg.FromUin, msg.ToUin);
            }
        }

        void AddQuote(bool bSend, string showName, string uin, string text, Int64 sendTime, string roomNum, string from, string to)
        {
            Parsing.QuoteRsp rsp = mDataBus.GetQuoteInfoList(text, from, to);
            if (null != rsp)
            {
                if (rsp.MmQuoteListCount > 0)
                {
                    if (null != roomNum)
                    {
                        QQServer.RoomInfo roomInfo = DataManager.Instance.GetQQRoomInfo(roomNum);
                        if (null != roomInfo)
                        {
                            roomNum = roomInfo.RoomName;
                        }
                    }
                    mQuoteForm.AddQuote(bSend, showName, uin, rsp.MmQuoteListList, sendTime, text, roomNum);
                }
                else if (rsp.BondQuoteListCount > 0)
                {
                      if(rsp.IsDeal==false)
                        mQuoteForm.AddBondQuote(showName.Length > 0 ? showName : uin, sendTime, text, rsp.BondQuoteListList);
                      else
                        mQuoteForm.AddBondQuoteToday(showName.Length > 0 ? showName : uin, sendTime, text, rsp.BondQuoteListList);
                }
                else if (rsp.RangeQuoteListCount > 0)
                {
                    if (null != roomNum)
                    {
                        QQServer.RoomInfo roomInfo = DataManager.Instance.GetQQRoomInfo(roomNum);
                        if (null != roomInfo)
                        {
                            roomNum = roomInfo.RoomName;
                        }
                    }
                    mQuoteForm.AddRangeQuote(sendTime, roomNum, showName, uin, rsp.RangeQuoteListList);
                }
                else if (rsp.HasOtherQuote)
                {
                    if (null != roomNum)
                    {
                        QQServer.RoomInfo roomInfo = DataManager.Instance.GetQQRoomInfo(roomNum);
                        if (null != roomInfo)
                        {
                            roomNum = roomInfo.RoomName;
                        }
                    }
                    mQuoteForm.AddOtherQuote(sendTime, roomNum, showName, uin, text);
                }
            }
        }

        void Disconnect()
        {
            MessageBox.Show("连接断开");
        }

    }
}
