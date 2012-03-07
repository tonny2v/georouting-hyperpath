using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using System.Configuration;
using System.Diagnostics;
using System.Data;

namespace confTest
{
    class Program
    {
        static void Main(string[] args)
        {
            NpgsqlConnection conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["tokyo"].ToString());
      
            if (conn.State == ConnectionState.Closed) conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("select * from road where gid<10",conn);
            NpgsqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Trace.WriteLine(dr["gid"]);
            }
            
            dr.Close();
            conn.Close();
            Console.ReadKey();
        }
    }
}
