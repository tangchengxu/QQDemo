using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QQDemo.databus;
using QQDemo.util;
using MsgExpress;

namespace QQDemo.common
{
    class QuoteComp
    {
        public QuoteComp(DataBus bus)
        {
            mBus = bus;
        }

        public void CompareQuote(Int64 sendTime, string text)
        {
            //IList<Parsing.QuoteInfo> infoListOld = GetQuoteInfoOld(text);
            //IList<Parsing.QuoteInfo> infoListNew = mBus.GetQuoteInfoList(text);

            //if (IsEqul(infoListOld, infoListNew))
            //{
            //    if (infoListOld != null && infoListOld.Count > 0)
            //    {
            //        this.SaveEqulQuote(sendTime, text, infoListOld, infoListNew);
            //    }
            //}
            //else
            //{
            //    this.SaveDiffQuote(sendTime, text, infoListOld, infoListNew);
            //}
        }

        List<Parsing.QuoteInfo> GetQuoteInfoOld(string text)
        {
            string jsonStr = QQDemo.util.Common.QuoteParse(text, "test");
            if (null == jsonStr)
            {
                return null;
            }
            else
            {
                return QQDemo.util.Common.JsonToInfo(jsonStr);
            }
        }

        bool IsEqul(IList<Parsing.QuoteInfo> infoListOld, IList<Parsing.QuoteInfo> infoListNew)
        {
            if ((null == infoListOld || infoListOld.Count == 0) && (null == infoListNew || infoListNew.Count == 0))
            {
                return true;
            }
            if ((null == infoListOld && null != infoListNew) || (null != infoListOld && null == infoListNew))
            {
                return false;
            }

            // 报价条数
            if (infoListOld.Count != infoListNew.Count)
            {
                return false;
            }

            for (int i = 0; i < infoListOld.Count; i++)
            {
                Parsing.QuoteInfo infoOld = infoListOld[i];
                Parsing.QuoteInfo infoNew = infoListNew[i];

                // 方向
                string DirectionOld = infoOld.Direction.Trim();
                string DirectionNew = infoNew.Direction.Trim();
                if (!DirectionOld.Equals(DirectionNew))
                {
                    return false;
                }

                // 数量
                if (infoOld.AmountCount != infoNew.AmountCount)
                {
                    return false;
                }
                HashSet<string> amountSet = new HashSet<string>();
                for (int j = 0; j < infoOld.AmountCount; j++)
                {
                    amountSet.Add(infoOld.AmountList[j].Trim());
                }
                for (int j = 0; j < infoNew.AmountCount; j++)
                {
                    string amount = infoNew.AmountList[j].Trim();
                    if (!amountSet.Contains(amount))
                    {
                        return false;
                    }
                }

                // 期限
                if (infoOld.TenorCount != infoNew.TenorCount)
                {
                    return false;
                }
                HashSet<string> tenorSet = new HashSet<string>();
                for (int j = 0; j < infoOld.TenorCount; j++)
                {
                    tenorSet.Add(infoOld.TenorList[j].Trim());
                }
                for (int j = 0; j < infoNew.TenorCount; j++)
                {
                    string tenor = infoNew.TenorList[j].Trim();
                    if (!tenorSet.Contains(tenor))
                    {
                        return false;
                    }
                }

                // 类型
                if (infoOld.TypeCount != infoNew.TypeCount)
                {
                    return false;
                }
                HashSet<string> typeSet = new HashSet<string>();
                for (int j = 0; j < infoOld.TypeCount; j++)
                {
                    typeSet.Add(infoOld.TypeList[j].Trim());
                }
                for (int j = 0; j < infoNew.TypeCount; j++)
                {
                    string type = infoNew.TypeList[j].Trim();
                    if (!typeSet.Contains(type))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // 保存不一致的报价
        void SaveDiffQuote(Int64 sendTime, string text, IList<Parsing.QuoteInfo> infoListOld, IList<Parsing.QuoteInfo> infoListNew)
        {
            string strTime = Common.FormatTime(sendTime);
            
            int oldCount = null == infoListOld ? 0 : infoListOld.Count;
            int newCount = null == infoListNew ? 0 : infoListNew.Count;
            int nCount = oldCount > newCount ? oldCount : newCount;
            for (int i = 0; i < nCount; i++)
            {
                Parsing.QuoteInfo infoOld = null;
                Parsing.QuoteInfo infoNew = null;
                if (oldCount > i)
                {
                    infoOld = infoListOld[i];
                }
                if (newCount > i)
                {
                    infoNew = infoListNew[i];
                }

                string direction = "";
                string amount = "";
                string tenor = "";
                string type = "";

                if (null != infoOld)
                {
                    direction = infoOld.Direction;
                    foreach (string am in infoOld.AmountList)
                    {
                        amount += am + " ";
                    }
                    foreach (string tn in infoOld.TenorList)
                    {
                        tenor += tn + " ";
                    }
                    foreach (string tp in infoOld.TypeList)
                    {
                        type += tp + " ";
                    }
                }
                else
                {
                    direction = "无";
                    amount = "无";
                    tenor = "无";
                    type = "无";
                }
                direction += " | ";
                amount += " | ";
                tenor += " | ";
                type += " | ";
                if (null != infoNew)
                {
                    direction += infoNew.Direction;
                    foreach (string am in infoNew.AmountList)
                    {
                        amount += am + " ";
                    }
                    foreach (string tn in infoNew.TenorList)
                    {
                        tenor += tn + " ";
                    }
                    foreach (string tp in infoNew.TypeList)
                    {
                        type += tp + " ";
                    }
                }
                else
                {
                    direction += "无";
                    amount += "无";
                    tenor += "无";
                    type += "无";
                }

                this.SaveQuoteRec(strTime, text, direction, amount, tenor, type, "不同");
            }
        }

        // 保存一致的报价
        void SaveEqulQuote(Int64 sendTime, string text, IList<Parsing.QuoteInfo> infoListOld, IList<Parsing.QuoteInfo> infoListNew)
        {
            string strTime = Common.FormatTime(sendTime);
            

            foreach (Parsing.QuoteInfo info in infoListOld)
            {
                string direction = info.Direction;

                string amount = "";
                foreach (string am in info.AmountList)
                {
                    amount += am + " ";
                }

                string tenor = "";
                foreach (string tn in info.TenorList)
                {
                    tenor += tn + " ";
                }

                string type = "";
                foreach (string tp in info.TypeList)
                {
                    type += tp + " ";
                }

                this.SaveQuoteRec(strTime, text, direction, amount, tenor, type, "相同");
            }
        }

        void SaveQuoteRec(string strTime, string text, string direction, string amount, string tenor, string type, string flag)
        {
            string sql = "insert into QuoteRec values(";
            sql += "'" + strTime.Replace("'", "\\'") + "','" + text.Replace("'", "\\'")
                + "','" + direction.Replace("'", "\\'") + "','" + amount.Replace("'", "\\'")
                + "','" + tenor.Replace("'", "\\'") + "','" + type.Replace("'", "\\'")
                + "','" + flag.Replace("'", "\\'");
            sql += "')";

            //Logger.Info("sql = " + sql);

            DbManager.Getmysqlcom(sql);
        }

        DataBus mBus;
    }
}
