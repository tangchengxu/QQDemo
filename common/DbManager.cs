using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using MsgExpress;

namespace QQDemo.common
{
    class DbManager
    {
        public static MySqlConnection Getmysqlcon()
        {
            string M_str_sqlcon = "server=172.16.75.81;user id=root;password=sumscope;port=3306;database=qqinfo;Charset=utf8";
            MySqlConnection myCon = new MySqlConnection(M_str_sqlcon);
            return myCon;
        }

        public static void Getmysqlcom(string M_str_sqlstr)
        {
            //try
            //{
            //    MySqlConnection mysqlcon = Getmysqlcon();
            //    mysqlcon.Open();
            //    MySqlCommand mysqlcom = new MySqlCommand(M_str_sqlstr, mysqlcon);
            //    mysqlcom.ExecuteNonQuery();
            //    mysqlcom.Dispose();
            //    mysqlcon.Close();
            //    mysqlcon.Dispose();
            //}
            //catch (System.Exception ex)
            //{
            //    Logger.Error("Write Db error : " + ex.ToString() + " \r\nsql = " + M_str_sqlstr);
            //}
        }

        public static MySqlDataReader Getmysqlread(string M_str_sqlstr)
        {
            MySqlConnection mysqlcon = Getmysqlcon();
            MySqlCommand mysqlcom = new MySqlCommand(M_str_sqlstr, mysqlcon);
            mysqlcon.Open();
            MySqlDataReader mysqlread = mysqlcom.ExecuteReader(CommandBehavior.CloseConnection);
            return mysqlread;
        }
    }
}
