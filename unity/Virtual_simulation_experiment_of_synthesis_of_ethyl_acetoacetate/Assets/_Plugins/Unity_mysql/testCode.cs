using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

public class test : MonoBehaviour
{
    class DBProgram
    {
        //远程连接
        //  string connectionString= "User ID = ; Password =.; Host =; Port =3306;Database = ;Charset = ";
        //本地连接
        //public static string connectionString = "User ID = Administrator; Password = Xunifangzhenshiyan1; Host = 182.254.187.40; Port = 3306;Database = ;Charset = ";
        public static string connectionString = "User ID = root; Password = Xunifangzhenshiyan1; Host = 182.254.187.40; Port = 3306; Database = user_info;";
        public static MySqlConnection dbConnection;

        public static void Main()
        {
            DBProgram.Add();
        }



        //打开数据库链接
        static void OpenSqlConnection(string connectionString)
        {
            dbConnection = new MySqlConnection(connectionString);
            dbConnection.Open();
            Debug.Log(dbConnection.Ping());
        }

        //关闭数据库连接
        static void CloseConnection()
        {
            if (dbConnection != null)
            {
                dbConnection.Close();
                dbConnection.Dispose();
                dbConnection = null;
            }
        }

        //保存数据
        public static DataSet GetDataSet(string sqlString)
        {
            DataSet ds = new DataSet();
            try
            {
                //用于检索和保存数据
                //Fill(填充)能改变DataSet中的数据以便于数据源中数据匹配
                //Update(更新)能改变数据源中的数据以便于DataSet中的数据匹配

                MySqlDataAdapter da = new MySqlDataAdapter(sqlString, dbConnection);
                da.Fill(ds);
            }
            catch (Exception ee)
            {
                throw new Exception("SQL:" + sqlString + "\n" + ee.Message.ToString());
            }
            return ds;
        }

        //增 insert
        public static void Add()
        {
            OpenSqlConnection(connectionString);
            string sqlstring = "insert into info values('000','1','1','1')";
            GetDataSet(sqlstring);
            CloseConnection();
        }

        //删 delete
        static void Delete()
        {
            OpenSqlConnection(connectionString);
            string sqlstring = "delete from 表名;";
            GetDataSet(sqlstring);
            CloseConnection();
        }

        //改 update
        static void Update()
        {
            OpenSqlConnection(connectionString);
            string sqlstring = "update 表名 set 字段=值 where 条件;";
            GetDataSet(sqlstring);
            CloseConnection();

        }

        //查 select
        static void Select()
        {
            OpenSqlConnection(connectionString);
            MySqlCommand mysqlcommand = new MySqlCommand("select * from 表名;", dbConnection);
            MySqlDataReader reader = mysqlcommand.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        //reader.getstring(0)/getint(0).....
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("failed to select");

            }
            finally
            {
                reader.Close();
            }

            CloseConnection();
        }


    }


    void Start()
    {

    }
    
    private void OnGUI()
    {
        if (GUILayout.Button("try"))
        {
            DBProgram.Add();
        }
    }



}
