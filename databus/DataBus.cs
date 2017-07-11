using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MsgExpress;
using MsgExpress.databus;
using QQServer;
using Google.ProtocolBuffers;
using System.IO;
using QQDemo.common;
using QQDemo.thread;

namespace QQDemo.databus
{
    public enum ELoginError
    {
        LoginError_Success,                    //成功
        LoginError_NeedId,                     //需要验证码
        LoginError_WrongUserOrPass,            //用户名密码错误
        LoginError_Other,                      //其他错误
        LoginError_NeedSmsId,                  //手机验证码
    }

    public enum EIDError
    {
        IDError_Success,                      //成功
        IDError_IdWrong,                      //验证码输入错误
        IDError_Other,                        //其他异常
        IDError_WrongUserOrPass,              //用户名密码错误
        IDError_NeedSmsId,                    //输入手机验证码
    }

    public class DataBus
    {
        static string s_ConnectId = util.Common.GetIPAddress();
        const int timeOut = 20000;

        public DataBus()
        {
            Logger.Init(null);

            mDataBus = new DataBusManager();
            mDataBus.Initialize(null);
            mDataBus.PublishPackage += this.OnPublish;

            // for test
            mToken = "1";
            mUin = "1";
        }

        public void Release()
        {
            this.LogOut();
            if (null != mDataRelay)
            {
                mDataRelay.Stop();
            }
            mDataBus.Release();
        }

        // 分配Token
        public bool InitToken(String userName)
        {
            TokenReq.Builder build = new TokenReq.Builder();
            build.UserName = userName;

            object obj = mDataBus.SendMessage(build.Build(), timeOut, null);
            if (null == obj)
            {
                Logger.Error("Send InitToken failed!");
                return false;
            }
            TokenRsp rsp = obj as TokenRsp;
            mToken = rsp.Token;

            return true;
        }

        // 登录网关(管理)
        public bool LoginGateWay()
        {
            Gateway.Login.Builder build = new Gateway.Login.Builder();
            build.Token = mToken;

            object obj = mDataBus.SendMessage(build.Build(), timeOut, null);
            if (null == obj)
            {
                Logger.Error("Send LoginGateWay failed!");
                return false;
            }
            Gateway.CommonResponse rsp = obj as Gateway.CommonResponse;
            if (rsp.Retcode != 0)
            {
                Logger.Error("LoginGateWay failed!");
                return false;
            }

            return true;
        }

        // 登录
        public ELoginError Login(String userName, String passWord, int qqType)
        {
            LoginReq.Builder build = new LoginReq.Builder();
            build.UserName = userName;
            build.PassWord = passWord;
            build.ConnectId = s_ConnectId + this.GetHashCode();
            build.Token = mToken;
            build.LoginType = 0;
            build.ServerType = qqType;

            object obj = mDataBus.SendMessage(build.Build(), timeOut, null);
            if (null == obj || !(obj is LoginRsp))
            {
                Logger.Error("Send Login failed!");
                return ELoginError.LoginError_Other;
            }
            LoginRsp rsp = obj as LoginRsp;
            ELoginError errorCode = (ELoginError)(rsp.ErrorCode);
            if (ELoginError.LoginError_Success == errorCode)
            {
                mUin = rsp.Uin;
                mUin = mUin.Length == 0 ? userName : mUin;
            }
            else if (ELoginError.LoginError_NeedId == errorCode)
            {
                mImage = rsp.Image.ToByteArray();
                mUin = rsp.Uin;
                mUin = mUin.Length == 0 ? userName : mUin;
            }

            return errorCode;
        }

        // 获取验证码图片
        public Byte[] GetImage()
        {
            return mImage;
        }

        // 输入验证码
        public EIDError WriteIdCode(string idCode)
        {
            WriteIDCodeReq.Builder build = new WriteIDCodeReq.Builder();
            build.Token = mToken;
            build.IdCode = idCode;

            object obj = mDataBus.SendMessage(build.Build(), timeOut, null);
            if (null == obj)
            {
                Logger.Error("Write idCode failed!");
                return EIDError.IDError_Other;
            }
            WriteIDCodeRsp rsp = obj as WriteIDCodeRsp;
            EIDError errorCode = (EIDError)(rsp.ErrorCode);
            if (EIDError.IDError_IdWrong == errorCode)
            {
                mImage = rsp.Image.ToByteArray();
            }

            return errorCode;
        }

        // 获取好友列表
        public IList<GroupInfo> GetGroupInfoList()
        {
            GetFriendListReq.Builder build = new GetFriendListReq.Builder();
            build.Token = mToken;

            object obj = mDataBus.SendMessage(build.Build(), timeOut, null);
            if (null == obj)
            {
                Logger.Error("get group info failed!");
                return null;
            }
            GetFriendListRsp rsp = obj as GetFriendListRsp;
            if (0 == rsp.ErrorCode)
            {
                return rsp.GroupList;
            }
            else
            {
                Logger.Error("get group info , errorCode : " + rsp.ErrorCode);
                return null;
            }
        }

        // 获取群列表
        public IList<RoomInfo> GetRoomInfoList()
        {
            GetRoomListReq.Builder build = new GetRoomListReq.Builder();
            build.Token = mToken;

            object obj = mDataBus.SendMessage(build.Build(), timeOut, null);
            if (null == obj)
            {
                Logger.Error("get room info failed!");
                return null;
            }
            GetRoomListRsp rsp = obj as GetRoomListRsp;
            if (0 == rsp.ErrorCode)
            {
                return rsp.RoomList;
            }
            else
            {
                Logger.Error("get room info , errorCode : " + rsp.ErrorCode);
                return null;
            }
        }

        // 发消息
        public int SendMessage(int type, string uin, string msg, string code)
        {
            SendMessageReq.Builder build = new SendMessageReq.Builder();
            build.Token = mToken;
            build.Uin = uin;
            build.Type = type;
            build.Code = code;
 
            QQMessage.Builder msgBuild = new QQMessage.Builder();
            msgBuild.MsgType = 0;
            msgBuild.Content = ByteString.CopyFromUtf8(msg);
            build.AddMsg(msgBuild);

            object obj = mDataBus.SendMessage(build.Build(), timeOut, null);
            if (null == obj)
            {
                Logger.Error("Send message failed!");
                return 1;
            }
            SendMessageRsp rsp = obj as SendMessageRsp;
            if (0 != rsp.ErrorCode)
            {
                Logger.Error("Send message, ErrorCode : " + rsp.ErrorCode);
            }
            return rsp.ErrorCode;
        }

        // 退出登录
        void LogOut()
        {
            LogOutReq.Builder build = new LogOutReq.Builder();
            build.Token = mToken;

            mDataBus.SendMessage(build.Build(), 5000, null);
        }

        // 获取群成员信息
        public IList<UserInfo> GetRoomMembers(string uin, string code)
        {
            GetRoomMemberInfoReq.Builder build = new GetRoomMemberInfoReq.Builder();
            build.Token = mToken;
            build.Uin = uin;
            build.Code = code;

            object obj = mDataBus.SendMessage(build.Build(), timeOut, null);
            if (null == obj)
            {
                Logger.Error("Get Roommember failed!");
                return null;
            }
            GetRoomMemberInfoRsp rsp = obj as GetRoomMemberInfoRsp;

            return rsp.UserInfoList;
        }

        // 输入手机验证码
        public bool WriteSmsCode(string smsCode)
        {
            WriteSmsReq.Builder build = new WriteSmsReq.Builder();
            build.Token = mToken;
            build.SmsCode = smsCode;

            object obj = mDataBus.SendMessage(build.Build(), timeOut, null);
            if (null == obj)
            {
                Logger.Error("Write SmsCode failed!");
                return false;
            }
            WriteSmsRsp rsp = obj as WriteSmsRsp;

            return rsp.ErrorCode == 0;
        }

        // 设置转发类
        public void SetDataRelay(DataRelay ry)
        {
            mDataRelay = ry;
        }

        // 数据转发
        public bool ReciveDataRelay(byte[] data)
        {
            ReciveDataReq.Builder build = new ReciveDataReq.Builder();
            build.Token = mToken;
            build.Data = ByteString.CopyFrom(data);

            object obj = mDataBus.SendMessage(build.Build(), timeOut, null);
            if (null == obj)
            {
                Logger.Error("ReciveDataRelay failed!");
                return false;
            }
            ReciveDataRsp rsp = obj as ReciveDataRsp;
            return 0 == rsp.ErrorCode;
        }

        // 断线重连
        public bool ReConnect()
        {
            ReConnectReq.Builder build = new ReConnectReq.Builder();
            build.Token = mToken;

            return mDataBus.PostMessage(build.Build());
        }

        // 报价识别
        public Parsing.QuoteRsp GetQuoteInfoList(String text, string sender, string reciver)
        {
            Parsing.QuoteReq.Builder build = new Parsing.QuoteReq.Builder();
            build.Quote = text;
            if (null == sender)
            {
                build.FromQQ = mUin;
            }
            else
            {
                build.FromQQ = sender;
            }
            if (null == reciver)
            {
                build.ToQQ = mUin;
            }
            else
            {
                build.ToQQ = reciver;
            }

            object obj = mDataBus.SendMessage(build.Build(), timeOut, null);
            if (null != obj)
            {
                return obj as Parsing.QuoteRsp;
            }
            return null;
        }

        void OnPublish(object sender, PublishEventArgs e)
        {
            PublishData pubData = e.PublishData.mMsg as PublishData;
            if (null == pubData)
            {
                return;
            }
            if (Global.s_QQMessageTopic == pubData.Topic)
            {
                for (int i = 0; i < pubData.ItemCount; i++)
                {
                    DataItem item = pubData.ItemList[i];
                    if (item.Key == Global.s_QQMessageKey && item.RawValCount > 0)
                    {
                        PubReciveMessage msg = PubReciveMessage.ParseFrom(item.RawValList[0]);
                        if (null != msg)
                        {
                            if ((msg.Type == 0 || msg.Type == 2) && msg.FromUin == mUin)
                            {
                            }
                            else
                            {
                                PubReciveMessage.Builder build = new PubReciveMessage.Builder();
                                build.FromUin = msg.FromUin;
                                build.FromName = msg.FromName;
                                build.ToUin = msg.ToUin;
                                build.Type = msg.Type;
                                build.Time = msg.Time + Global.timeDiff;
                                build.AddRangeMsg(msg.MsgList);
                                DataManager.Instance.OnQQMessage(build.Build());
                            }
                        }
                    }
                }
            }
            else if (Global.s_QQDataTopic == pubData.Topic)
            {
                for (int i = 0; i < pubData.ItemCount; i++)
                {
                    DataItem item = pubData.ItemList[i];
                    if (item.Key == Global.s_QQMessageKey && item.RawValCount > 0)
                    {
                        mDataRelay.SendData(item.RawValList[0].ToByteArray());
                    }
                }
            }
            else if (Global.s_QQContentTopic == pubData.Topic)
            {
                for (int i = 0; i < pubData.ItemCount; i++)
                {
                    DataItem item = pubData.ItemList[i];
                    if (item.Key == Global.s_QQNameKey && item.StrValCount > 0)
                    {
                        string name = item.StrValList[0];
                        if (name.Equals("PubDisconnect"))
                        {
                            DataManager.Instance.OnDisConnect();
                        }
                    }
                }
            }
        }

        DataBusManager mDataBus;
        string mToken;
        public string mUin;
        Byte[] mImage;
        DataRelay mDataRelay = null;
    }
}
