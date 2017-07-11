using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using QQDemo.util;
using MsgExpress;

namespace QQDemo.common
{
    public class DataBuf
    {
        const int bufSize = 4028;

        public DataBuf(Socket sk)
        {
            mClientSocket = sk;
            mDataBuf = new byte[bufSize];
            mStart = 0;
            mEnd = 0;
        }

        // 读取int
        public int ReadInt()
        {
            if (GetTotalSize() < 4)
            {
                if (ReadDataFromSocket() < 0)
                {
                    return -1;
                }

                if (GetTotalSize() < 4)
                {
                    return 0;
                }
            }
            return Common.bytes2Int(mDataBuf, mStart);
        }

        // 读取指定的字节数
        public byte[] GetBytes(int len)
        {
            var ms = new MemoryStream();
            if (len <= 0)
            {
                return ms.ToArray();
            }

            int readLen = 0;
            do 
            {
                if (GetTotalSize() <= 0)
                {
                    if (ReadDataFromSocket() <= 0)
                    {
                        return null;
                    }
                }
                int bufLen = len - readLen < GetTotalSize() ? len - readLen : GetTotalSize();
                byte[] buff = new byte[bufLen];
                System.Array.Copy(mDataBuf, mStart, buff, 0, bufLen);
                ms.Write(buff, 0, buff.Length);

                mStart += bufLen;
                readLen += bufLen;
            } while (readLen < len);

            return ms.ToArray();
        }

        int GetTotalSize()
        {
            return mEnd - mStart;
        }

        // 对齐数据
        void OrderDataBuf()
        {
            if (mStart > 0)
            {
                System.Array.Copy(mDataBuf, mStart, mDataBuf, 0, mEnd - mStart);
                mEnd = mEnd - mStart;
                mStart = 0;
            }
        }

        // 从socket读取数据
        int ReadDataFromSocket()
        {
            OrderDataBuf();

            try
            {
                int recLen = -1;
                try
                {
                	recLen = mClientSocket.Receive(mDataBuf, mEnd, mDataBuf.Length - mEnd, SocketFlags.None);
                }
                catch (System.Exception ex)
                {
                    Logger.Error("Recive Data From QQ Failed!" + ex.ToString());
                }
                if (recLen > 0)
                {
                    mEnd += recLen;
                }
                return recLen;
            }
            catch
            {
                return -1;
            }
        }

        protected Socket mClientSocket;
        protected byte[] mDataBuf;
        protected int mStart;
        protected int mEnd;
    }
}
