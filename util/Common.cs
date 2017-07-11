using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

namespace QQDemo.util
{
    class Common
    {
        const string mUrl = "http://professor.sumscoped.com/mm_parse.php";

        public static String GetMd5(String str)
        {
            MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bytValue, bytHash;
            bytValue = System.Text.Encoding.UTF8.GetBytes(str);
            bytHash = md5.ComputeHash(bytValue);
            md5.Clear();
            string sTemp = "";
            for (int i = 0; i < bytHash.Length; i++)
            {
                sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
            }
            return sTemp.ToLower(); 
        }

        public static long GetCurrentTime()
        {
            return (long)DateTime.Now.Subtract(DateTime.Parse("1970-1-1")).TotalMilliseconds;
        }

        public static string FormatTime(long time)
        {
            return DateTime.Parse("1970-1-1").AddMilliseconds(time).ToString();
        }

        public static string GetIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] localIpList = Dns.GetHostAddresses(hostName);
            if (null == localIpList || localIpList.Length == 0)
            {
                return "" + GetCurrentTime();
            }
            else
            {
                return localIpList[0].ToString();
            }
        }

        public static string QuoteParse(string context, string id)
        {
            string req = "{\"context\":\"" + context + "\", \"id\":\"" + id + "\"}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(mUrl);
            request.Method = "POST";
            request.ContentType = "application/json";
            byte[] buffer = Encoding.GetEncoding("UTF-8").GetBytes(req);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        public static List<Parsing.QuoteInfo> JsonToInfo(string json)
        {
            if (null == json || json.Length == 0)
            {
                return null;
            }
            try
            {
                JObject obj = JObject.Parse(json);
                string status = obj["STATUS"].ToString();
                if (!status.Equals("0"))
                {
                    return null;
                }
                List<Parsing.QuoteInfo> infoList = new List<Parsing.QuoteInfo>();
                
                JObject result = JObject.Parse(obj["RESULT"].ToString());
                JObject parsedMsg = JObject.Parse(result["parsedMsg"].ToString());
                JArray parser_result = JArray.Parse(parsedMsg["parser_result"].ToString());
                foreach (JObject jobj in parser_result)
                {
                    Parsing.QuoteInfo.Builder build = new Parsing.QuoteInfo.Builder();

                    string side = jobj["side"].ToString();
                    build.Direction = side.Equals("0") ? "借" : "出";

                    string volume_low = jobj["volume_low"].ToString();
                    string volume_high = jobj["volume_high"].ToString();
                    string amount = null;
                    if (volume_low.Equals(volume_high))
                    {
                        amount = volume_low;
                    }
                    else
                    {
                        amount = volume_low + "-" + volume_high;
                    }
                    if (amount.Length > 0)
                    {
                        build.AddAmount(amount);
                    }

                    string buyout_repo = jobj["buyout_repo"].ToString();
                    string pledge_repo = jobj["pledge_repo"].ToString();
                    string bank_credit = jobj["bank_credit"].ToString();
                    if (null != buyout_repo && buyout_repo.Length > 0)
                    {
                        build.AddType("买断");
                    }
                    if (null != pledge_repo && pledge_repo.Length > 0)
                    {
                        build.AddType("质押");
                    }
                    if (null != bank_credit && bank_credit.Length > 0)
                    {
                        build.AddType("拆借");
                    }

                    string tenors = jobj["tenors"].ToString();
                    JArray jTenor = JArray.Parse(tenors);
                    string tenor = "";
                    foreach (JObject ten in jTenor)
                    {
                        string days_low = ten["days_low"].ToString();
                        string days_high = ten["days_high"].ToString();
                        string tenor_flag = ten["tenor_flag"].ToString();

                        string tenorStr = "";
                        if (tenor_flag == "0")
                        {
                            tenorStr = "以下";
                        }
                        else if (tenor_flag == "1")
                        {
                            tenorStr = "以上";
                        }

                        if (tenor.Length > 0)
                        {
                            tenor += " ";
                        }
                        if (days_low.Equals(days_high))
                        {
                            tenor += days_low + tenorStr;
                        }
                        else
                        {
                            tenor += days_low + "-" + days_high;
                        }
                    }
                    if (tenor.Length > 0)
                    {
                        build.AddTenor(tenor);
                    }

                    infoList.Add(build.Build());
                }
                return infoList;
            }
            catch (System.Exception /*ex*/)
            {
                return null;
            }
        }

        public static byte[] short2Bytes(short num)
        {
            byte[] bt = new byte[2];
            bt[0] = (byte)(num >> 8);
            bt[1] = (byte)(num & 0xff);
            return bt;
        }

        public static short bytes2Short(byte[] buffer)
        {
            if (buffer.Length >= 2)
            {
                return (short)((buffer[0] << 8) | buffer[1]);
            }
            else
            {
                return 0;
            }
        }

        public static byte[] int2Bytes(int num)
        {
            byte[] bt = new byte[4];
            bt[0] = (byte)(num >> 24 & 0xff);
            bt[1] = (byte)(num >> 16 & 0xff);
            bt[2] = (byte)(num >> 8 & 0xff);
            bt[3] = (byte)(num & 0xff);
            return bt;
        }

        public static int bytes2Int(byte[] buffer)
        {
            if (buffer.Length >= 4)
            {
                return ((buffer[0] << 24) | (buffer[1] << 16) | (buffer[2] << 8) | buffer[3]);
            }
            else
            {
                return 0;
            }
        }

        public static int bytes2Int(byte[] buffer, int offset)
        {
            if (buffer.Length >= 4)
            {
                return ((buffer[0 + offset] << 24) | (buffer[1 + offset] << 16) 
                    | (buffer[2 + offset] << 8) | buffer[3 + offset]);
            }
            else
            {
                return 0;
            }
        }

        public static string GetMsgText(QQServer.PubReciveMessage msg)
        {
            string text = "";
            foreach (QQServer.QQMessage qqMsg in msg.MsgList)
            {
                if (qqMsg.MsgType == 0)
                {
                    text += qqMsg.Content.ToStringUtf8();
                }
                else
                {
                    text += "  ";
                }
            }
            return text;
        }
    
    }
}
