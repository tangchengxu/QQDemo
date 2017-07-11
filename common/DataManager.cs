using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using QQDemo.util;
using Google.ProtocolBuffers;
using MsgExpress;

namespace QQDemo.common
{
    enum EDataChangeType
    {
        Type_QQ_GroupInfo,
        Type_QQ_RoomInfo,
        Type_QQ_RoomMemberInfo,
        Type_QQ_Message,
        Type_QQ_Disconnect,
    }
    delegate void OnDataChange(EDataChangeType type, object param);

    class QQRoomMemberChange
    {
        public string code;
        public IList<QQServer.UserInfo> infoList;
    }

    class DataManager
    {
        public static DataManager Instance = new DataManager();
        public OnDataChange mDataChangeHander = null;

        private DataManager()
        {
            mQQFriendUinSet = new HashSet<string>();
            mQQUserInfoMap = new Dictionary<string, QQServer.UserInfo>();
            mQQRoomMap = new Dictionary<string, QQServer.RoomInfo>();
            mQQUserToMsgMap = new Dictionary<string, List<QQServer.PubReciveMessage>>();
            mQQRoomToMsgMap = new Dictionary<string, List<QQServer.PubReciveMessage>>();
        }

        // 存储QQ用户组
        public void SetQQGroup(IList<QQServer.GroupInfo> infoList)
        {
            mQQGroupList = infoList;
            foreach (QQServer.GroupInfo info in infoList)
            {
                foreach (QQServer.UserInfo userInfo in info.UserInfoList)
                {
                    mQQFriendUinSet.Add(userInfo.Uin);
                    mQQUserInfoMap.Add(userInfo.Uin, userInfo);
                }
            }
            if (null != mDataChangeHander)
            {
                mDataChangeHander(EDataChangeType.Type_QQ_GroupInfo, infoList);
            }
        }

        // 存储QQ群
        public void SetQQRoom(IList<QQServer.RoomInfo> infoList)
        {
            foreach (QQServer.RoomInfo info in infoList)
            {
                mQQRoomMap.Add(info.Code, info);
            }
            if (null != mDataChangeHander)
            {
                mDataChangeHander(EDataChangeType.Type_QQ_RoomInfo, infoList);
            }
        }

        // 存储QQ群成员
        public void SetQQRoomMembers(string code, IList<QQServer.UserInfo> infoList)
        {
            if (mQQRoomMap.ContainsKey(code))
            {
                QQServer.RoomInfo roomInfo = mQQRoomMap[code];
                QQServer.RoomInfo.Builder build = new QQServer.RoomInfo.Builder();
                build.Uin = roomInfo.Uin;
                build.RoomName = roomInfo.RoomName;
                build.Code = roomInfo.Code;
                build.UserInfoList.Add(infoList);

                mQQRoomMap[code] = build.Build();

                foreach (QQServer.UserInfo userInfo in infoList)
                {
                    if (!mQQUserInfoMap.ContainsKey(userInfo.Uin))
                    {
                        mQQUserInfoMap.Add(userInfo.Uin, userInfo);
                    }
                }
            }
            if (null != mDataChangeHander)
            {
                QQRoomMemberChange ch = new QQRoomMemberChange();
                ch.code = code;
                ch.infoList = infoList;
                mDataChangeHander(EDataChangeType.Type_QQ_RoomMemberInfo, ch);
            }
        }

        // 是否QQ为好友
        public bool IsQQFriend(string uin)
        {
            return mQQFriendUinSet.Contains(uin);
        }

        // 获取QQ用户信息
        public QQServer.UserInfo GetQQUserInfo(string uin)
        {
            if (mQQUserInfoMap.ContainsKey(uin))
            {
                return mQQUserInfoMap[uin];
            }
            else
            {
                return null;
            }
        }

        // 获取QQ群信息
        public QQServer.RoomInfo GetQQRoomInfo(string code)
        {
            if (mQQRoomMap.ContainsKey(code))
            {
                return mQQRoomMap[code];
            }
            else
            {
                return null;
            }
        }

        // 收到QQ消息
        public void OnQQMessage(QQServer.PubReciveMessage msg)
        {
            lock (this)
            {
                if (msg.Type == 0 || msg.Type == 2)
                {
                    if (!mQQUserToMsgMap.ContainsKey(msg.FromUin))
                    {
                        mQQUserToMsgMap.Add(msg.FromUin, new List<QQServer.PubReciveMessage>());
                    }
                    mQQUserToMsgMap[msg.FromUin].Add(msg);
                }
                else if (msg.Type == 1)
                {
                    if (!mQQRoomToMsgMap.ContainsKey(msg.ToUin))
                    {
                        mQQRoomToMsgMap.Add(msg.ToUin, new List<QQServer.PubReciveMessage>());
                    }
                    mQQRoomToMsgMap[msg.ToUin].Add(msg);

                    this.OnSaveFundMsg(msg);
                }
            }

            if (null != mDataChangeHander)
            {
                mDataChangeHander(EDataChangeType.Type_QQ_Message, msg);
            }
        }

        // 添加QQ消息
        public void AddQQMessage(string fromUin, string toUin, string msg)
        {
            QQServer.PubReciveMessage.Builder build = new QQServer.PubReciveMessage.Builder();
            build.FromUin = fromUin;
            build.FromName = "我";
            build.ToUin = toUin;
            build.Type = 0;
            build.Time = Common.GetCurrentTime();

            QQServer.QQMessage.Builder msgBuild = new QQServer.QQMessage.Builder();
            msgBuild.MsgType = 0;
            msgBuild.Content = ByteString.CopyFromUtf8(msg);
            build.AddMsg(msgBuild);

            lock (mQQUserToMsgMap)
            {
                if (!mQQUserToMsgMap.ContainsKey(toUin))
                {
                    mQQUserToMsgMap.Add(toUin, new List<QQServer.PubReciveMessage>());
                }
                mQQUserToMsgMap[toUin].Add(build.Build());
            }
        }

        // 获取QQ用户消息列表
        public List<QQServer.PubReciveMessage> GetQQUserMsgList(string uin)
        {
            if (mQQUserToMsgMap.ContainsKey(uin))
            {
                return mQQUserToMsgMap[uin];
            }
            else
            {
                return null;
            }
        }

        // 获取QQ群消息列表
        public List<QQServer.PubReciveMessage> GetQQRoomMsgList(string code)
        {
            if (mQQRoomToMsgMap.ContainsKey(code))
            {
                return mQQRoomToMsgMap[code];
            }
            else
            {
                return null;
            }
        }

        // 获取所有QQ好友
        public HashSet<string> GetAllQQFriend()
        {
            return mQQFriendUinSet;
        }

        // 断链通知
        public void OnDisConnect()
        {
            if (null != mDataChangeHander)
            {
                mDataChangeHander(EDataChangeType.Type_QQ_Disconnect, null);
            }
        }

        // 存储基金报价群的消息
        void OnSaveFundMsg(QQServer.PubReciveMessage msg)
        {
            if (msg.ToUin.Equals("417685737"))
            {
                QQServer.RoomInfo roomInfo = DataManager.Instance.GetQQRoomInfo(msg.ToUin);
                QQServer.UserInfo userInfo = this.GetQQUserInfo(msg.FromUin);
                if (null != roomInfo && null != userInfo)
                {
                    string strTime = Common.FormatTime(msg.Time);
                    string text = Common.GetMsgText(msg);

                    string sql = "insert into FundMsg values(";
                    sql += "'" + strTime.Replace("'", "\\'") + "','" + roomInfo.Uin.Replace("'", "\\'")
                        + "','" + roomInfo.RoomName.Replace("'", "\\'") + "','" + userInfo.Uin.Replace("'", "\\'")
                        + "','" + userInfo.ShowName.Replace("'", "\\'") + "','" + text.Replace("'", "\\'");
                    sql += "')";

                    DbManager.Getmysqlcom(sql);
                }
            }
        }

        IList<QQServer.GroupInfo> mQQGroupList;                              // QQ用户组
        HashSet<string> mQQFriendUinSet;                                     // QQ好友集合
        Dictionary<string, QQServer.UserInfo> mQQUserInfoMap;                // QQ用户信息
        Dictionary<string, QQServer.RoomInfo> mQQRoomMap;                    // QQ群信息
        Dictionary<string, List<QQServer.PubReciveMessage>> mQQUserToMsgMap; // QQ用户到消息列表的映射
        Dictionary<string, List<QQServer.PubReciveMessage>> mQQRoomToMsgMap; // QQ群到消息列表的映射
    }
}
