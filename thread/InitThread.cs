using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QQDemo.databus;
using QQServer;
using QQDemo.common;
using System.IO;

namespace QQDemo.thread
{
    class InitThread : BaseThread
    {
        public InitThread(DataBus bus)
        {
            mDataBus = bus;
        }

        override protected void ThreadEntrance()
        {
            if (this.IsNeedStop())
                return;

            IList<GroupInfo> groupInfoList = mDataBus.GetGroupInfoList();
            if (this.IsNeedStop())
                return;

            //WriteUserGroupInfo(groupInfoList);
            DataManager.Instance.SetQQGroup(groupInfoList);

            IList<RoomInfo> roomInfoList = mDataBus.GetRoomInfoList();
            if (this.IsNeedStop())
                return;

            //WriteQQRoomInfo(roomInfoList);
            DataManager.Instance.SetQQRoom(roomInfoList);

            if (null == roomInfoList)
            {
                return;
            }

            //FileStream fs = new FileStream("roomMember.csv", FileMode.Create);
            //StreamWriter sw = new StreamWriter(fs);
            for (int i = 0; i < roomInfoList.Count; i++)
            {
                if (this.IsNeedStop())
                    return;

                string uin = roomInfoList[i].Uin;
                string code = roomInfoList[i].Code;
                IList<UserInfo> userInfoList = mDataBus.GetRoomMembers(uin, code);
                if (null != userInfoList)
                {
                    DataManager.Instance.SetQQRoomMembers(code, userInfoList);
                }
                //WriteRoomMemberInfo(sw, roomInfoList[i].Code, roomInfoList[i].RoomName, userInfoList);
            }
            //sw.Close();
            //fs.Close();
        }

        void WriteUserGroupInfo(IList<GroupInfo> groupInfoList)
        {
            FileStream fs = new FileStream("friend.csv", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            foreach (GroupInfo info in groupInfoList)
            {
                foreach (UserInfo userInfo in info.UserInfoList)
                {
                    sw.Write(userInfo.Uin);
                    sw.Write(",");
                    if (userInfo.ShowName.Length > 0)
                    {
                        sw.Write(userInfo.ShowName);
                    }
                    else
                    {
                        sw.Write(userInfo.Uin);
                    }
                    sw.Write("\r\n");
                }
            }
            sw.Flush();
            sw.Close();
            fs.Close();
        }

        void WriteRoomMemberInfo(StreamWriter sw, string roomCode, string roomName, IList<UserInfo> userInfoList)
        {
            foreach (UserInfo userInfo in userInfoList)
            {
                sw.Write(roomName);
                sw.Write(",");
                sw.Write(roomCode);
                sw.Write(",");
                sw.Write(userInfo.Uin);
                sw.Write(",");
                if (userInfo.NickName.Length > 0)
                {
                    sw.Write(userInfo.NickName);
                }
                else if (userInfo.ShowName.Length > 0)
                {
                    sw.Write(userInfo.ShowName);
                }
                else
                {
                    sw.Write(userInfo.Uin);
                }
                sw.Write("\r\n");
            }
        }

        void WriteQQRoomInfo(IList<RoomInfo> roomInfoList)
        {

        }

        protected DataBus mDataBus;
    }
}
