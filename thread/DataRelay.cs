using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using MsgExpress;
using System.Threading;
using QQDemo.databus;
using System.Collections.Concurrent;
using QQDemo.common;

namespace QQDemo.thread
{

    public class DataRelay : BaseThread
    {
        //const string qqIp = "183.60.15.242";   // QQ服务器IP
        //const int qqPort = 8080;               // QQ服务器端口
        const string qqIp = "140.207.123.177";   // QQ服务器IP
        const int qqPort = 8080;                 // QQ服务器端口
        static bool bInit = false;

        public DataRelay(DataBus bus)
        {
            mClientSocket = null;
            mDataBus = bus;
            mDataBus.SetDataRelay(this);
        }

        public bool SendData(byte[] data)
        {
            if (null != mClientSocket)
            {
                int sendLen = 0;
                try
                {
                	sendLen = mClientSocket.Send(data);
                }
                catch (System.Exception ex)
                {
                    Logger.Error("Send Data To QQ Failed!" + ex.ToString());
                }
                if (sendLen >= data.Length)
                {
                    return true;
                }
            }
            Logger.Error("Send Data failed");
            return false;
        }

        override protected void ThreadEntrance()
        {
            Logger.Info("DataRelay Thread Start...");
            while (!IsNeedStop())
            {
                if (Connect(qqIp, qqPort))
                {
                    Logger.Info("Connect qq server success!");
                    break;
                }
                else
                {
                    Logger.Error("Connect qq server failed!");
                    Thread.Sleep(2000);
                }
            }
            if (bInit)
            {
                mDataBus.ReConnect();
            }
            bInit = true;

            mDataBuf = new DataBuf(mClientSocket);

            while (!IsNeedStop())
            {
                bool bOk = false;
                int total = mDataBuf.ReadInt();
                bOk = total >= 0;
                if (total > 0)
                {
                    byte[] data = mDataBuf.GetBytes(total);
                    if (null != data && data.Length >= total)
                    {
                        mDataBus.ReciveDataRelay(data);
                    }
                    else
                    {
                        bOk = false;
                    }
                }
                else
                {
                    Thread.Sleep(100);
                }

                if (!bOk)
                {
                    this.Stop();
                    new DataRelay(mDataBus).Start();
                }
            }

            Logger.Info("DataRelay Thread End...");
        }

        protected bool Connect(string addr, int port)
        {
            Logger.Info("Connect to server, addr = " + addr + " port = " + port);

            IPAddress ip = IPAddress.Parse(addr);
            mClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                mClientSocket.Connect(ip, port);
                return true;
            }
            catch
            {
                Logger.Error("Connect failed!");
                mClientSocket.Close();
                return false;
            }
        }


        protected Socket mClientSocket;
        protected DataBus mDataBus;
        protected DataBuf mDataBuf;
    }
}
