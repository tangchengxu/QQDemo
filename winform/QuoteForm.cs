using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using QQDemo.util;
using QQDemo.common;

namespace QQDemo
{
    public partial class QuoteForm : Form
    {
        public QuoteForm()
        {
            InitializeComponent();

            //IList<Parsing.RangeBondQuote> infoList = new List<Parsing.RangeBondQuote>();

            //Parsing.RangeBondQuote.Builder build = new Parsing.RangeBondQuote.Builder();
            //build.Direction = "出";
            //build.TermStart = "1D";
            //build.TermEnd = "2M";
            //build.RatingStart = "AA";
            //build.RatingEnd = "AAA+";
            //build.VolumeStart = "100";
            //build.VolumeEnd = "2000";
            //build.AddBondType("国债");
            //build.AddBondType("利率债");
            //build.Text = "你好啊！";
            //build.LineNo = 1;
            //build.Probobility = 100;

            //infoList.Add(build.Build());
            //this.AddRangeQuote(Common.GetCurrentTime(), "QQ测试群", "胡锐", "840801567", infoList);
        }

        // 添加范围报价
        public void AddRangeQuote(Int64 sendTime, string roomName, string sender, string qqNum, IList<Parsing.RangeBondQuote> infoList)
        {
            string strTime = Common.FormatTime(sendTime);
            foreach (Parsing.RangeBondQuote info in infoList)
            {
                string bondTypes = string.Join(",", info.BondTypeList);
                dataGridViewRange.Rows.Add(new object[] { strTime, roomName, sender, qqNum, 
                    info.Direction, info.TermStart, info.TermEnd, GetRatingDesc(info.RatingStart.Trim()), 
                    GetRatingDesc(info.RatingEnd.Trim()), info.VolumeStart, info.VolumeEnd, bondTypes, 
                    info.Text, info.LineNo, info.Probobility});
            }
            dataGridViewRange.FirstDisplayedScrollingRowIndex = dataGridViewRange.RowCount - 1;
        }

        // 添加债券报价
        public void AddBondQuote(string sender, Int64 sendTime, string text, IList<Parsing.BondQuoteInfo> infoList)
        {
            string strTime = Common.FormatTime(sendTime);
            foreach (Parsing.BondQuoteInfo info in infoList)
            {
                dataGridViewBond.Rows.Add(new object[] { strTime , sender, info.Direction, info.Bondkey, info.Code, info.Name, info.Volume, info.Price, info.PriceTag, info.Text, info.LineNo, text, info.Probobility});
            }
            dataGridViewBond.FirstDisplayedScrollingRowIndex = dataGridViewBond.RowCount - 1;
        }

       // 添加今日成交
        public void AddBondQuoteToday(string sender, Int64 sendTime, string text, IList<Parsing.BondQuoteInfo> infoList)
        {
            string strTime = Common.FormatTime(sendTime);
            foreach (Parsing.BondQuoteInfo info in infoList)
            {
                dataGridViewBondToday.Rows.Add(new object[] { strTime, sender, info.Direction, info.Bondkey, info.Code,
                    info.Name, info.Volume, info.Price, info.PriceTag, info.Text, info.LineNo, text, info.Probobility });
            }
            dataGridViewBondToday.FirstDisplayedScrollingRowIndex = dataGridViewBondToday.RowCount - 1;
        }
        
        // 添加线上资金报价
        public void AddQuote(bool bSend, string showName, string uin, IList<Parsing.QuoteInfo> infoList, Int64 sendTime, string text, string roomName)
        {
            DataGridView dataGridView = bSend ? dataGridView1 : dataGridView2;

            string strTime = Common.FormatTime(sendTime);
            for (int i = 0; i < infoList.Count; i++)
            {
                Parsing.QuoteInfo info = infoList[i];
                string direction = "", amount = "", tenor = "", type = "", tag = "", price = "";
                direction = info.Direction;
                for (int j = 0; j < info.AmountCount; j++)
                {
                    if (j > 0)
                    {
                        amount += " " + info.GetAmount(j);
                    }
                    else
                    {
                        amount += info.GetAmount(j);
                    }
                    
                }
                for (int j = 0; j < info.TenorCount; j++)
                {
                    if (j > 0)
                    {
                        tenor += " " + info.GetTenor(j) ;
                    }
                    else
                    {
                        tenor += info.GetTenor(j);
                    }
                    
                }
                for (int j = 0; j < info.TypeCount; j++)
                {
                    if (j > 0)
                    {
                        type += " " + info.GetType(j);
                    }
                    else
                    {
                        type += info.GetType(j);
                    }
                }
                for (int j = 0; j < info.TagCount; j++)
                {
                    if (j > 0)
                    {
                        tag += " " + info.GetTag(j);
                    }
                    else
                    {
                        tag += info.GetTag(j);
                    }
                }
                price = info.Price != null ? info.Price : "";

                if (bSend)
                {
                    dataGridView.Rows.Add(new object[] { showName, uin, direction, amount, tenor, type, tag, price, strTime, text, info.Probobility });
                }
                else
                {
                    dataGridView.Rows.Add(new object[] { roomName, showName, uin, direction, amount, tenor, type, tag, price, strTime, text, info.Probobility });
                }
                
                if (!bSend)
                {
                    string sql = "insert into QuoteInfo values(";
                    sql += "'" + showName.Replace("'", "\\'") + "','" + uin.Replace("'", "\\'")
                        + "','" + direction.Replace("'", "\\'") + "','" + amount.Replace("'", "\\'")
                        + "','" + tenor.Replace("'", "\\'") + "','" + type.Replace("'", "\\'")
                        + "','" + tag.Replace("'", "\\'") + "','" + price.Replace("'", "\\'")
                        + "','" + strTime.Replace("'", "\\'") + "','" + text.Replace("'", "\\'");
                    sql += "')";
                    DbManager.Getmysqlcom(sql);
                }
            }
            dataGridView.FirstDisplayedScrollingRowIndex = dataGridView.RowCount - 1;
        }

        // 添加其它报价
        public void AddOtherQuote(Int64 sendTime, string roomName, string sender, string qqNum, string msgText)
        {
            string strTime = Common.FormatTime(sendTime);
            dataGridViewOther.Rows.Add(new object[] { strTime, roomName, sender, qqNum, msgText});
            dataGridViewOther.FirstDisplayedScrollingRowIndex = dataGridViewOther.RowCount - 1;
        }

        string GetRatingDesc(string rating)
        {
            if (rating.Equals("0"))
            {
                return "A-";
            }
            else if (rating.Equals("1"))
            {
                return "A";
            }
            else if (rating.Equals("2"))
            {
                return "A+";
            }
            else if (rating.Equals("3"))
            {
                return "AA-";
            }
            else if (rating.Equals("4"))
            {
                return "AA";
            }
            else if (rating.Equals("5"))
            {
                return "AA+";
            }
            else if (rating.Equals("6"))
            {
                return "AAA-";
            }
            else if (rating.Equals("7"))
            {
                return "AAA";
            }
            else if (rating.Equals("8"))
            {
                return "AAA+";
            }
            else
            {
                return rating;
            }
        }

        private void export_Click(object sender, EventArgs e)
        {
            SaveFileDialog sDlg = new SaveFileDialog();
            sDlg.Filter = "csv files (*.csv)|*.csv";
            sDlg.RestoreDirectory = true;

            if (sDlg.ShowDialog() == DialogResult.OK)
            {
                DataGridView dataGridView = tabControl1.SelectedIndex == 0 ? dataGridView1 : dataGridView2;
                try
                {
                    using (StreamWriter sw = new StreamWriter(sDlg.OpenFile(), Encoding.Default))
                    {
                        foreach (DataGridViewRow row in dataGridView.Rows)
                        {
                            Console.Out.WriteLine(row.Cells[1].Value);
                            for (int i = 0; i < row.Cells.Count; i++)
                            {
                                if (i > 0)
                                {
                                    sw.Write(",");
                                }
                                string value = row.Cells[i].Value as string;
                                value = value.Replace("\r\n", "  ");
                                value = value.Replace('\r', ' ');
                                value = value.Replace('\n', ' ');
                                sw.Write(value);
                            }
                            sw.Write("\r\n");
                        }

                        sw.Close();
                    }
                }
                catch (System.Exception)
                {

                }
            }
        }
    }
}
