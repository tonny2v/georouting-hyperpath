using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoHyperstar.Forms
{
    public partial class DBSetting_Form : Form
    {
        public string Connstr { get; set; }
        public DBSetting_Form()
        {
            InitializeComponent();
        }

        private void DBSetting_Load(object sender, EventArgs e)
        {
        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Connstr = "Server=" + server_tb.Text
            + ";DataBase=" + database_tb.Text
            + ";Port=" + port_tb.Text
            + ";Userid=" + user_tb.Text
            + ";password=" + password_tb.Text;
            GeoHyperStar_MainForm.ConnStr = Connstr;
            this.Dispose();
        }

        private void SetDefault_btn_Click(object sender, EventArgs e)
        {
            GeoHyperstar.Properties.Settings.Default["DefaultCon"] =
                "Server=" + server_tb.Text +
                ";DataBase=" + database_tb.Text +
                ";Port=" + port_tb.Text +
                ";Userid=" + user_tb.Text +
                ";password=" + password_tb.Text;
            GeoHyperstar.Properties.Settings.Default.Save();
        }
    }
}
