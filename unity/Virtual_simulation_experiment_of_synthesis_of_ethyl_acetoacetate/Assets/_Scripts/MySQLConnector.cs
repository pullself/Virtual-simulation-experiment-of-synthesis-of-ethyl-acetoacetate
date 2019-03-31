using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

public class MySQLConnector
{
    private string connectionString;
    private MySqlConnection dbConnection;

    public bool OpenSqlConnection()
    {
        connectionString = "User ID = root; Password = Xunifangzhenshiyan1; Host = 182.254.187.40; Port = 3306; Database = user_info;";
        dbConnection = new MySqlConnection(connectionString);
        dbConnection.Open();
        return dbConnection.Ping();
    }
    public void CloseConnection()
    {
        if (dbConnection != null)
        {
            dbConnection.Close();
            dbConnection.Dispose();
            dbConnection = null;
            connectionString = string.Empty;
        }
    }

    private DataSet GetDataSet(string sqlString)
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

    public void Exec(string sqlStr)
    {
        if (connectionString == string.Empty)
            return;
        //OpenSqlConnection();
        GetDataSet(sqlStr);
        //CloseConnection();
    }

    public int Check_Id_Pwd(string userId, string userPwd)
    {
        int ans = 0;
        string sqlStr = string.Format("select stuPwd from stuinfo where stuId='{0}';", userId);
        MySqlCommand mysqlcommand = new MySqlCommand(sqlStr, dbConnection);
        MySqlDataReader reader = mysqlcommand.ExecuteReader();
        try
        {
            if (reader.Read())
            {
                if (reader.HasRows)
                {
                    string pwd = reader.GetString(0);
                    string pwd_md5 = MD5Utils.MD5_32(userPwd);
                    Debug.Log("database: " + pwd);
                    Debug.Log("userPwd" + userPwd);
                    Debug.Log("userPwd md5:" + pwd_md5);
                    if (!pwd.Equals(pwd_md5))
                    {
                        ans = -2;
                    }
                }   
            }
            else
            {
                ans = -1;
            }
        }
        catch (Exception)
        {
            Debug.Log("fail to select");
        }
        finally
        {
            reader.Close();
        }
        return ans;
    }

    
}
