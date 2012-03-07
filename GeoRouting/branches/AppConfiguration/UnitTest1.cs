using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Npgsql;
using System.Data;
using System.Configuration;
namespace AppConfiguration
    
{
    //[TestClass]
    public class UnitTest1
    {      
        public TestContext TestContext { get; set; }

        public static void Main(string[] args)
        {
            Console.WriteLine("main");
        }

        //[TestMethod]
        public void TestMethod1()
        {
            TestContext.WriteLine("afaf");
            Trace.WriteLine("ajiofjaoOK");
            Trace.Indent();
            Trace.WriteLine("after");
        }

        //[TestMethod]
        public void TestMethod2()
        {
            TestContext.WriteLine("Test2");
            NpgsqlConnection conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["tokyo"].ToString());
            if (conn.State == ConnectionState.Closed) conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("select * from road where gid<10");
            NpgsqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Trace.WriteLine(dr.GetString(1));
            }
            dr.Close();
            conn.Close();
 
        }
        
    }
}
