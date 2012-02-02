//选择在GIS环境下要运行的算法
//#define DHS
//此处的HP为原始Spiess&Florian算法，另外程序中还包括了Node-Directed HP（NDHP）算法，但没有定义其预编译的块
//#define HP
#define FDHS
//#define NonGISTopo //数据源不存在节点层，没有对应的GIS图
//#define HPD
//#define RSA
//#define RegretPath
//算法时间测试
//#define AlgTest
//#define RecordScanned

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

using Npgsql;

using SharpMapUITest;
using NetworkLib.Element;
using WinChart;
using System.Drawing;

namespace GeoHyperstar.Forms
{
    public partial class GeoHyperStar_MainForm : Form
    {
        public static string ConnStr = GeoHyperstar.Properties.Settings.Default["DefaultCon"].ToString();

        public Network CurrentNet { get; set; }
        public Network SubNet { get; set; }
        double Alpha1 = 0.5;
        double Alpha2 = 0.5;
        double Beta = 0.654;
        //double Gama = 0.3333;//useless here
        double maxdelaylevel = 0;
        List<int> NodeDirect_animation = new List<int>();

        List<int> SelectedLink_animation = new List<int>();

        //for monte carlo simulation
        Dictionary<string, List<int>> Vectors = null;
        Dictionary<string, List<int>> LinkSets = null;


        //用于绘制描述探索点费用变化的图形
        List<double> YPoints_Ui;

        List<double> YPoints_UiAddCa;
        //for simulation
        double[] SumRunTime = new double[5];
        int[] SumLinkNum = new int[5];
        double[] SumTime = new double[5];
        double[] SumSimilarity = new double[5];
        List<double>[] RunTimes = new List<double>[5];
        List<int>[] LinkNums = new List<int>[5];
        List<double>[] Times = new List<double>[5];
        List<double> []Similarities = new List<double>[5];
        

        public GeoHyperStar_MainForm()
        {
            InitializeComponent();
            YPoints_Ui = new List<double>();
            YPoints_UiAddCa = new List<double>();
        }

        //数据库连接，从数据库构建网络
        private long AccessData()
        {
            long LoadingTime = -1;
            Network_Dispose(CurrentNet);//消除已有网络
            if (PrepareData(out LoadingTime))
            {
                //MessageBox.Show("Data Accessed From PostgreSQL Database", "Data Access");
                toolStripStatusLabel1.Text += " || Current Network: " + CurrentNet.AllNodes.Count.ToString() + " nodes and " + CurrentNet.AllLinks.Count.ToString() + " links";
            }
            else MessageBox.Show(this, "Failed to Access!", "Data Access");
            return LoadingTime;
        }

        //保存网络拓扑到二进制文件
        private void SaveToFile_btn_Click(object sender, EventArgs e)
        {
            string filepath = "\\..\\..\\Data\\Network.dat";
            SaveFileDialog sf = new SaveFileDialog();
            sf.InitialDirectory = Directory.GetCurrentDirectory();
            sf.DefaultExt = "dat";
            sf.Filter = "Network data files (*.dat)|*.dat|All files (*.*)|*.*";
            if (sf.ShowDialog(this) == DialogResult.OK)
            {
                filepath = sf.FileName;
            }
            if (SaveNetwork(filepath)) MessageBox.Show(this, "Network Saved Successfully.", "Save Network to File");
            else MessageBox.Show(this, "Error!Network Cannot Be Saved!");
            sf.Dispose();
        }

        //从二进制文件中读取网络拓扑
        private void ReadFromFile_btn_Click(object sender, EventArgs e)
        {
            Network_Dispose(CurrentNet);//消除已有网络
            string filepath = "\\..\\..\\Data\\Network.dat";
            OpenFileDialog of = new OpenFileDialog();
            of.InitialDirectory = Directory.GetCurrentDirectory();
            of.DefaultExt = "dat";
            of.Filter = "Network data files (*.dat)|*.dat|All files (*.*)|*.*";
            if (of.ShowDialog(this) == DialogResult.OK)
            {
                filepath = of.FileName;
            }
            Network network = new Network();
            of.InitialDirectory = Directory.GetCurrentDirectory();

            if (ReadNetwork(filepath, out network))
            {
                MessageBox.Show(this, "Read Network Successfully", "Read Network From File");
                CurrentNet.AllLinks = network.AllLinks;
                CurrentNet.AllNodes = network.AllNodes;
                toolStripStatusLabel1.Text += " || Current Network: " + CurrentNet.AllNodes.Count.ToString() + " nodes and " + CurrentNet.AllLinks.Count.ToString() + " links";
            }
            else MessageBox.Show(this, "Read Network Failed", "Read Network From File");
            of.Dispose();
        }

        //最短路算法
        private void RunAlg_btn_Click(object sender, EventArgs e)
        {
            if (Origin_nud.Value > CurrentNet.AllNodes.Count || Destination_nud.Value > CurrentNet.AllNodes.Count)
                MessageBox.Show(this, "Searching Points Out of Range, Only " + CurrentNet.AllNodes.Count.ToString() + " in the Network", "Error!");
            else
            {
                long TimeSpan_SP = -1;
                //if (!Dijkstra((int)Origin_nud.Value, (int)Destination_nud.Value, out TimeSpan_SP)) MessageBox.Show(show,"Error!");
                if (!FibDijkstra(CurrentNet, (int)Origin_nud.Value, (int)Destination_nud.Value, true, out TimeSpan_SP)) MessageBox.Show(this, "Error!");
                else
                {
                    bool Accessible;
                    List<int> shortestpath = GetShortestPath(CurrentNet, (int)Origin_nud.Value, (int)Destination_nud.Value, true, out Accessible);
                    double distance=0;
                    Node oNode=CurrentNet.AllNodes[(int)Origin_nud.Value-1];
                    Node dNode=CurrentNet.AllNodes[(int)Destination_nud.Value-1];
                    distance = Math.Sqrt((oNode.X - dNode.X) * (oNode.X - dNode.X) + (oNode.Y - dNode.Y) * (oNode.Y - dNode.Y));
                    if (Accessible)
                    //MessageBox.Show(this,"Shortest Path(" + shortestpath.Count + " links) Search Completed!\nRunning Time: " + TimeSpan_SP.ToString() + " ms", "Path Search");
                    {
                        AlgLog_tb.Text += DateTime.Now.ToString("yyyy/mmm/d HH:MM") + "\r\n";
                        AlgLog_tb.Text += toolStripStatusLabel1.Text + "\r\nShortest Path(" + shortestpath.Count + " links) Search Completed!\r\nRunning Time: " + TimeSpan_SP.ToString() + " ms\r\n\r\n";
                        AlgLog_tb.Text += "Euclidean distance: " + distance + "\r\n";
                        AlgLog_tb.Text += "Least travel time: " + dNode.OptHeuristic;
                    }
                    else MessageBox.Show(this, "Not Accessible!");
                }
                Dijkstra_Recover(CurrentNet);//将网络恢复到初始状态
            }
        }

        //Run Algorithm
        private void DHS_btn_Click(object sender, EventArgs e)
        {
            List<Link> FinalPathSet = null;
            List<Link> RawHyperpath = new List<Link>();
            NodeDirect_animation.Clear();
            SelectedLink_animation.Clear();
            long TimeSpan_SP = -1;
            long TimeSpan_MPA = -1;//runtime of multipath algorithm
            
#if DHS
            //Run DHS
            if (!DHS(CurrentNet,(int)HyperFrom_nud.Value, (int)HyperTo_nud.Value,ref RawHyperpath, out TimeSpan_SP, out TimeSpan_MPA))
#elif HP
            //Run HP
            //if (!HP_twist(CurrentNet, (int)HyperFrom_nud.Value, (int)HyperTo_nud.Value, ref RawHyperpath, out TimeSpan_MPA))
            if (!HP_heu(CurrentNet, (int)HyperFrom_nud.Value, (int)HyperTo_nud.Value, ref RawHyperpath, out TimeSpan_SP, out TimeSpan_MPA))

#elif FDHS
            //Run FDHS
            if (!FDHS(CurrentNet,(int)HyperFrom_nud.Value, (int)HyperTo_nud.Value, 
                ref RawHyperpath,out TimeSpan_SP, out TimeSpan_MPA))
#elif HPD
            //Run HPD
            if (!HPD_NB(CurrentNet, (int)HyperFrom_nud.Value, (int)HyperTo_nud.Value, ref RawHyperpath, out TimeSpan_MPA))
            //if (!HPD_twistheu(CurrentNet,(int)HyperFrom_nud.Value, (int)HyperTo_nud.Value,ref RawHyperpath, out TimeSpan_MPA))
#elif RSA
            //Run RSA
            bool RSASuccess = false;
            long tempt1=-1;
            //如果可达才探索PO
            GoalDirectedFibDijkstra(CurrentNet, (int)HyperFrom_nud.Value, (int)HyperTo_nud.Value, true, out tempt1);
            bool SPAccessible = false;

            GetShortestPath(CurrentNet, (int)HyperFrom_nud.Value, (int)HyperTo_nud.Value, true, out SPAccessible);
            Dijkstra_Recover(CurrentNet);
            
            if (SPAccessible)
            {
                RSASuccess = RSA(CurrentNet, (int)HyperFrom_nud.Value, (int)HyperTo_nud.Value, out FinalPathSet, out TimeSpan_SP, out TimeSpan_MPA);
            }
            if (!RSASuccess)//RSA探索失败
#endif
            {
#if RSA
                MessageBox.Show(this, "Not Accessible", "Failed!");
#else
                MessageBox.Show(this,"Failed!", "Hyperpath Search Error");
#endif
            }

            else//RSA探索成功，生成PO set保存至FinalPathSet中
            {
                List<Link> RBP = new List<Link>();
#if RSA&&RegretPath
                if (GloablOnPOset_ckb.Checked)
                {

                    //用SubNetwork进行探索
                    if (GlobalSearch_rb.Checked)
                    {
                        long tempt = -1;
                        AssignRegretForTRBGlobal(CurrentNet, (int)HyperFrom_nud.Value, (int)HyperTo_nud.Value, true, out tempt);
                        /*Constuct Subnetwork,Regret延用父网络的值*/
                        /**********************************************************************************************/
                        Network PONetwork = CurrentNet.CreateSubNetwork(FinalPathSet.ToArray());
                        SubNet = PONetwork;//设置当前子网络为PONetwork
                        int NewOriginID = 0;
                        int NewDestinationID = 0;
                        foreach (Node i in PONetwork.AllNodes)
                        {
                            if (i.GID == (int)this.HyperFrom_nud.Value)
                                NewOriginID = i.SubID;
                            else if (i.GID == (int)this.HyperTo_nud.Value)
                                NewDestinationID = i.SubID;
                        }
                        /**********************************************************************************************/
                        RegretPath_TRBGlobalForSubNet(PONetwork, NewOriginID, NewDestinationID, out RBP, out TimeSpan_RBP);
                    }
                    else if (LocalSearch_rb.Checked)
                    //local search还是用CurrentNet，因为Subnetwork的判断已经包含在内
                    {
                        if(!LocalIncludeEt_ckb.Checked)
                            RegretPath_TRBLocal_BackwardPenalty(CurrentNet,
                                (int)this.HyperFrom_nud.Value, (int)this.HyperTo_nud.Value,
                                FinalPathSet, out RBP, out TimeSpan_RBP);
                        else
                            RegretPath_TRBLocal_BackwardPenaltyWithEU(CurrentNet,
                                (int)this.HyperFrom_nud.Value, (int)this.HyperTo_nud.Value,
                                FinalPathSet, out RBP, out TimeSpan_RBP);
                    }
                }
#endif
#if RSA &&!RegretPath
                AlgLog_tb.Text += DateTime.Now.ToString("yyyy/mmm/d HH:MM") + "\r\n";

                AlgLog_tb.Text += toolStripStatusLabel1.Text + "\r\nPO(" + FinalPathSet.Count + " links) set Generated.\r\nDijkstra Initialization Running Time:" + TimeSpan_SP.ToString()
                    + " ms\r\nRSA Running Time: " + TimeSpan_MPA + " ms\r\n";
                
#elif DHS||FDHS||HPD||HP
                FinalPathSet = GetHyperpath(RawHyperpath);
                //MessageBox.Show(this,"Hyperpath(" + FinalPathSet.Count + " links) Generated.\nDijkstra Initialization Running Time:" + TimeSpan_SP.ToString() + " ms\nTotal Running Time: " + TimeSpan_MPA.ToString() + " ms", "Hyperpath Search");
                AlgLog_tb.Text += DateTime.Now.ToString("yyyy/mmm/d HH:MM")+"\r\n";
                AlgLog_tb.Text +=toolStripStatusLabel1.Text + "\r\nHyperpath(" + FinalPathSet.Count + " links) Generated.\r\nDijkstra Initialization Running Time:" + TimeSpan_SP.ToString() + " ms\r\nTotal Running Time: " + TimeSpan_MPA.ToString() + " ms\r\n\r\n";

#endif

#if RSA&&RegretPath //因为在Regret path中需要Heuristics信息，不能清空

                //用RawNetwork进行探索
                if (!GloablOnPOset_ckb.Checked)
                {
                    if (GlobalSearch_rb.Checked)
                        RegretPath_TRBGlobal(CurrentNet, (int)this.HyperFrom_nud.Value, (int)this.HyperTo_nud.Value, out RBP, out TimeSpan_RBP);
                    else if (LocalSearch_rb.Checked)
                        RegretPath_TRBLocal_BackwardPenalty(CurrentNet,
                            (int)this.HyperFrom_nud.Value, (int)this.HyperTo_nud.Value,
                            FinalPathSet, out RBP, out TimeSpan_RBP);
                }

                AlgLog_tb.Text += DateTime.Now.ToString("yyyy/mmm/d HH:MM") + "\r\n";

                AlgLog_tb.Text += toolStripStatusLabel1.Text + "\r\nPO(" + FinalPathSet.Count + " links) set Generated.\r\nDijkstra Initialization Running Time:" + TimeSpan_SP.ToString()
                    + " ms\r\nRSA Running Time: " + TimeSpan_MPA + " ms\r\n"
                    + "Regret Path Running Time: " + TimeSpan_RBP + " ms\r\n"
                    + "Total Running Time:" + (TimeSpan_MPA + TimeSpan_RBP) + "ms\r\n\r\n";

#endif
                //询问是否保存路径
                switch (MessageBox.Show(this, "Do you want to save the path set?",
                           "Save path set",
                           MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        // "Yes" processing
                        if (SaveToFile_rb.Checked)
                        {
                            string filepath = "\\..\\..\\Data\\pathset.csv";
                            SaveFileDialog sf = new SaveFileDialog();
                            sf.InitialDirectory = Directory.GetCurrentDirectory();
                            sf.DefaultExt = "csv";
                            sf.Filter = "Network data files (*.csv)|*.csv|All files (*.*)|*.*";
                            if (sf.ShowDialog() == DialogResult.OK)
                            {
                                filepath = sf.FileName;
                            }
                            if (SaveHyperpath(filepath, FinalPathSet, TimeSpan_SP, TimeSpan_MPA))
                            {
                                MessageBox.Show(this, "Path Set Saved Successfully.", "Save to File");
                            }
                            else MessageBox.Show(this, "Error!Network Cannot Be Saved!");
                        }
                        else if (SaveToDatabase_rb.Checked)
                        {
                            //try
                            //{
                            using (NpgsqlConnection conn = new NpgsqlConnection(ConnStr))
                            {
                                NpgsqlCommand cmd = new NpgsqlCommand();
                                cmd.Connection = conn;
                                cmd.CommandType = CommandType.Text;
                                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);


                                DataSet DsToUpdate = new DataSet();


#if RSA
                                cmd.CommandText = "DROP TABLE IF EXISTS popath;"
                                    + "CREATE TABLE popath (id INTEGER, choice_possibility DOUBLE PRECISION); SELECT * FROM popath;";

#else
                            cmd.CommandText = "DROP TABLE IF EXISTS hyperpath;"
                                + "CREATE TABLE hyperpath (id INTEGER, choice_possibility DOUBLE PRECISION); SELECT * FROM hyperpath";
#endif
                                da.Fill(DsToUpdate);
                                //手动创建Datatable的关键语句
                                //hyperpath.Columns.Add("id", Type.GetType("System.Int32"));
                                //hyperpath.Columns.Add("choice_possibility", Type.GetType("System.Double"));
                                for (int i = 0; i < FinalPathSet.Count; i++)
                                {
                                    DataRow dr = DsToUpdate.Tables[0].NewRow();
                                    dr["id"] = FinalPathSet[i].GID;
#if RSA
                                    dr["choice_possibility"] = 1;
#else
                                dr["choice_possibility"] = FinalPathSet[i].Pa;
#endif
                                    DsToUpdate.Tables[0].Rows.Add(dr);
                                }
                                NpgsqlCommandBuilder cmb = new NpgsqlCommandBuilder(da);
                                da.Update(DsToUpdate);
                                DsToUpdate.Dispose();
                                cmb.Dispose();
                                da.Dispose();

#if RSA&&RegretPath
                                cmd = new NpgsqlCommand();
                                cmd.Connection = conn;
                                cmd.CommandType = CommandType.Text;
                                da = new NpgsqlDataAdapter(cmd);
                                DsToUpdate = new DataSet();
                                cmd.CommandText += "DROP TABLE IF EXISTS regretpath;"
                                    + "CREATE TABLE regretpath (id INTEGER, choice_possibility DOUBLE PRECISION); SELECT * FROM regretpath;";
                                da = new NpgsqlDataAdapter(cmd);
                                DsToUpdate = new DataSet();
                                da.Fill(DsToUpdate);
                                //此处填充regretpath
                                for (int i = 0; i < RBP.Count; i++)
                                {
                                    DataRow dr = DsToUpdate.Tables[0].NewRow();
                                    dr["id"] = RBP[i].GID;
                                    dr["choice_possibility"] = 1;
                                    DsToUpdate.Tables[0].Rows.Add(dr);
                                }
                                cmb = new NpgsqlCommandBuilder(da);
                                da.Update(DsToUpdate);
#endif

#if !RSA
                            cmd.CommandText = "DROP TABLE IF EXISTS hyperpath_lyr;select * into hyperpath_lyr from road join hyperpath on (hyperpath.id=road.gid);Drop table if exists hyperpath";
                            if (conn.State == ConnectionState.Closed) conn.Open();
                            cmd.ExecuteNonQuery();
#else
                                cmd.CommandText = "DROP TABLE IF EXISTS popath_lyr;select * into popath_lyr from road join popath on (popath.id=road.gid);Drop table if exists popath";
                                if (conn.State == ConnectionState.Closed) conn.Open();
                                cmd.ExecuteNonQuery();
#endif

#if RegretPath
                                cmd.CommandText = "DROP TABLE IF EXISTS regretpath_lyr;select * into regretpath_lyr from road join regretpath on (regretpath.id=road.gid);Drop table if exists regretpath";
                                if (conn.State == ConnectionState.Closed) conn.Open();
                                cmd.ExecuteNonQuery();
#endif
                                conn.Close();
                                da.Dispose();
                                cmd.Dispose();
                                cmb.Dispose();
                            }
                            //如果需要产生带有几何属性的表，则需要进行关联操作，以下是关键语句
                            //DataTable dt= Join(dt_road, dt_hyperpath, "gid", "id");
                            //dataGridView1.DataSource =dt;

                            ///////////////////////////////////
                            //将产生的DataTable更新到数据库的关键语句
                            //hyperpath= dt.Clone();
                            //for (int i = 0; i < dt.Rows.Count;i++ )
                            //{ hyperpath.Rows.Add(dt.Rows[i].ItemArray); }
                            ////////////////////////////////
                            //}
                            //catch (Exception error) { MessageBox.Show(error.ToString()); }
                        }
                        break;

                    case DialogResult.No:
                        // "No" processing
                        break;

                }
            }

            FinalPathSet = null;


            Dijkstra_Recover(CurrentNet);//因为DHS调用了Dijkstra算法，对于节点的Heristic值（即P标号）进行了分配，因此需要将网络恢复到初始状态
            RecoverTravelTime(CurrentNet);
            DHS_Recover(CurrentNet, FinalPathSet);
        }

        //导出网络到csv文件
        private void Export_btn_Click(object sender, EventArgs e)
        {
            Network net = new Network(CurrentNet.AllNodes, CurrentNet.AllLinks);
            string LinkPath = "..\\..\\Data\\CurrentNet.AllLinks.csv";
            string NodePath = "..\\..\\Data\\CurrentNet.AllNodes.csv";
            ExportTopo(net, NodePath, LinkPath);
        }

        private void GeoHyperStar_MainForm_Load(object sender, EventArgs e)
        {
//#if AlgTest
//            (Main_tp as Control).Enabled = false;
//#else 
//            (Other_tp as Control).Enabled = false;
//#endif
            
#if FDHS||HP
            WinChart_btn.Visible = true;
#endif
            ReSet();
        }

        private void MaxDelayLevel_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Reloading Completed. Loading Time: " + AccessData() + " ms", "Loading Network", MessageBoxButtons.OK);
            ReSet();
            toolStripStatusLabel1.Text += " || Current Network: " + CurrentNet.AllNodes.Count.ToString() + " nodes and " + CurrentNet.AllLinks.Count.ToString() + " links";
        }

        private void MapDisplay_btn_Click(object sender, EventArgs e)
        {
            SharpMapUITest.MapDisplay.ConnStr = ConnStr;
            SharpMapUITest.MapDisplay MapDisplayForm = new MapDisplay();

            MapDisplayForm.Nodes_Animation = NodeDirect_animation.ToArray();
            MapDisplayForm.Links_Animation = SelectedLink_animation.ToArray();
            MapDisplayForm.Owner = this;

            MapDisplayForm.ShowDialog(this);
            MapDisplayForm.Dispose();
        }

        private void ReSet()
        {
#if RSA
            toolStripStatusLabel1.Text = "Release version: RSA";
#elif DHS
            toolStripStatusLabel1.Text = "Release verison: DHS";
#elif HPD
            toolStripStatusLabel1.Text = "Release version: HPD";
#elif FDHS
            toolStripStatusLabel1.Text = "Release version: FDHS";
#elif HP
            toolStripStatusLabel1.Text = "Release version: HP";
#endif
            SaveToDatabase_rb.Checked = true;
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutbox1 = new AboutBox();
            aboutbox1.ShowDialog(this);
            //MessageBox.Show(this,"Code by Tonny(Ma Jiangshan), CopyRight 2010-2011\nEmail:Tonny.Achilles@gmail.com", "Help", MessageBoxButtons.OK);
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MaxDelayLevel_cmb.SelectedIndex < 0)
                MaxDelayLevel_cmb.SelectedIndex = 1;
            DHS_btn.PerformClick();
        }

        private void testNetwrokToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ConnStr != GeoHyperstar.Properties.Settings.Default["TestNetwork"].ToString())
            {
                ConnStr = GeoHyperstar.Properties.Settings.Default["TestNetwork"].ToString();
                if (MaxDelayLevel_cmb.SelectedIndex == -1) MaxDelayLevel_cmb.SelectedIndex = 1;
                ReSet();
                long LoadingTime = AccessData();
                MessageBox.Show(this, "Loading Completed. Loading Time: " + LoadingTime + " ms", "Loading Network", MessageBoxButtons.OK);
            }
        }

        private void mesh533945ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ConnStr != GeoHyperstar.Properties.Settings.Default["Mesh533935"].ToString())
            {
                ConnStr = GeoHyperstar.Properties.Settings.Default["Mesh533935"].ToString();
                if (MaxDelayLevel_cmb.SelectedIndex == -1) MaxDelayLevel_cmb.SelectedIndex = 1;
                ReSet();
                long LoadingTime = AccessData();
                MessageBox.Show(this, "Loading Completed. Loading Time:" + LoadingTime + " ms", "Loading Network", MessageBoxButtons.OK);
            }
        }

        private void uSNetworkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ConnStr != GeoHyperstar.Properties.Settings.Default["USNetwork"].ToString())
            {
                ConnStr = GeoHyperstar.Properties.Settings.Default["USNetwork"].ToString();
                if (MaxDelayLevel_cmb.SelectedIndex == -1) MaxDelayLevel_cmb.SelectedIndex = 1;
                ReSet();
                long LoadingTime = AccessData();
                MessageBox.Show(this, "Loading Completed. Loading Time: " + LoadingTime + " ms", "Loading Network", MessageBoxButtons.OK);
            }

        }

        private void advancedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DBSetting_Form NewForm = new DBSetting_Form();
            NewForm.Owner = this;
            NewForm.ShowDialog();

            if (NewForm.IsDisposed)
            {
                ReSet();
                long LoadingTime = AccessData();
                MessageBox.Show(this, "Loading Completed. Loading Time:" + LoadingTime + " ms", "Loading Network", MessageBoxButtons.OK);
            }
        }

        private void WinChart_btn_Click(object sender, EventArgs e)
        {
            WinChart.ChartForm chart_form1 = new ChartForm();
            chart_form1.YPoints1 = YPoints_Ui.ToArray();
            chart_form1.YPoints2 = YPoints_UiAddCa.ToArray();

            chart_form1.Show(this);
        }

        private void Swap_btn_Click(object sender, EventArgs e)
        {
            decimal temp = HyperFrom_nud.Value;
            HyperFrom_nud.Value = HyperTo_nud.Value;
            HyperTo_nud.Value = temp;
        }

        private void RandOD_btn_Click(object sender, EventArgs e)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            HyperFrom_nud.Value = rand.Next(1, CurrentNet.AllNodes.Count);

            int temp = rand.Next(1, CurrentNet.AllNodes.Count);
            if (temp != HyperFrom_nud.Value) HyperTo_nud.Value = temp;
        }

        private void SimStart_btn_Click(object sender, EventArgs e)
        {
            //清空上次仿真记录
            SumRunTime = new double[5];
            SumLinkNum = new int[5];
            SumTime = new double[5];
            SumSimilarity = new double[5];
            RunTimes = new List<double>[5];
            LinkNums = new List<int>[5];
            Times = new List<double>[5];
            Similarities = new List<double>[5];
            Vectors = null;
            LinkSets = null;
            long temp = -1;
            bool Accessible = false;
            GoalDirectedFibDijkstra(CurrentNet, (int)HyperFrom_nud.Value, (int)HyperTo_nud.Value, true, out temp);
            GetShortestPath(CurrentNet, (int)HyperFrom_nud.Value, (int)HyperTo_nud.Value, true, out Accessible);
            Dijkstra_Recover(CurrentNet);
            if (Accessible)
            {
                Stopwatch stw = new Stopwatch();
                stw.Start();
                progressBar1.Maximum = (int)Iteration_nud.Value;
                progressBar1.Value = 0;
                do
                {
                    StartSimulation((int)HyperFrom_nud.Value, (int)HyperTo_nud.Value, CurrentNet, SimGlobalOnPO_ckb.Checked);
                    progressBar1.Value++;
                }
                while (progressBar1.Value < Iteration_nud.Value);
                stw.Stop();
                FileStream fs = new FileStream(@"D:\SimRecords.csv", FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine("///////////////////Simulation Conclusion///////////////////");
                sw.WriteLine(String.Format("/////////////{0}-->{1} {2} iterations simulation time: {3} seconds/////////////",
                    HyperFrom_nud.Value, HyperTo_nud.Value, Iteration_nud.Value, stw.ElapsedMilliseconds / 1000));
                int n = (int)Iteration_nud.Value;
                sw.WriteLine("PathName\\Record,AverRunTime,AverLinkNum,AverPathTime,AverPathSimilarity");
                for (int i = 0; i < 5; i++)
                {
                    string name = "";
                    if (i == 0) name = "SimSP";
                    else if (i == 1) name = "OptSP";
                    else if (i == 2) name = "PesSP";
                    else if (i == 3) name = "LSP";
                    else if (i == 4) name = "GSP";
                    sw.WriteLine(String.Format("{0}, {1}, {2}, {3}, {4}", name,
                        SumRunTime[i] / n, SumLinkNum[i] / n, SumTime[i] / n, SumSimilarity[i] / n));
                }
                sw.Close();
                fs = new FileStream(@"D:\SimRecordsForChart.csv", FileMode.Create);
                sw = new StreamWriter(fs);
                sw.WriteLine("///////////////////Simulation Conclusion///////////////////");
                sw.WriteLine(String.Format("{0}-->{1}, {2} iterations, simulation time: {3} seconds",
                    HyperFrom_nud.Value, HyperTo_nud.Value, Iteration_nud.Value, stw.ElapsedMilliseconds / 1000));
                sw.WriteLine("RunTime,LinkNum,TravelTime,Similarity");
                foreach (var i in Enumerable.Range(0, 5))
                {
                    switch (i)
                    {
                        case 0: sw.WriteLine("SimSP");
                            break;
                        case 1: sw.WriteLine("OptSP");
                            break;
                        case 2: sw.WriteLine("PesSP");
                            break;
                        case 3: sw.WriteLine("LSP");
                            break;
                        case 4: sw.WriteLine("GSP");
                            break;
                    }
                    foreach (var j in Enumerable.Range(0, RunTimes[0].Count))
                        sw.WriteLine(String.Format("{0},{1},{2},{3}",
                            RunTimes[i][j], LinkNums[i][j], Times[i][j], Similarities[i][j]));
                }
                sw.WriteLine("***********************************************************************");
                sw.Close();
                MessageBox.Show(this, "Compelted, Running Time: " + stw.ElapsedMilliseconds, "Notice", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(this, "Unaccessible OD, simulation cannot be carried out.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Chart_btn_Click(object sender, EventArgs e)
        {
            //for (int i = 0; i < 5; i++)
            //{
            //    string FigName="";
            //    switch (i) 
            //    {
            //        case 0: FigName = "Simulation Shortest Path";
            //            break;
            //        case 1: FigName = "Optimistic Path";
            //            break;
            //        case 2: FigName = "Pessimistic Path";
            //            break;
            //        case 3: FigName = "Local Regret Path";
            //            break;
            //        case 4: FigName = "Global Regret Path";
            //            break;
            //    }
            //    ChartForm newform = new ChartForm();
            //    newform.Plot(RunTimes[i].ToArray(), "Running Time(ms)",FigName);
            //    newform.Plot(LinkNums[i].ToArray(), "Link Number",FigName);
            //    newform.Plot(Times[i].ToArray(), "Actual Travel Time",FigName);
            //    newform.Plot(Similarities[i].ToArray(), "Cosine Similarity",FigName);
            //    //newform.Show();
            //}
            ChartForm form1 = new ChartForm();
            form1.Plot(Times[0].ToArray(), "SimSP", "Path Travel Time Comparison", "traveltime");
            form1.Plot(Times[1].ToArray(), "OptSP", "", "");
            form1.Plot(Times[2].ToArray(), "PesSP", "", "");
            form1.Plot(Times[3].ToArray(), "LSP", "", "");
            form1.Plot(Times[4].ToArray(), "GSP", "", "");

            ChartForm form2 = new ChartForm();
            form2.Plot2(Similarities[0], "SimSP", "Path Cosine Similarity Comparison", "similarity");
            form2.Plot2(Similarities[1], "OptSP", "", "");
            form2.Plot2(Similarities[2], "PesSP", "", "");
            form2.Plot2(Similarities[3], "LSP", "", "");
            form2.Plot2(Similarities[4], "GSP", "", "");
            form1.Show();
            form2.Show();

        }

        private void StartAlgTest_btn_Click(object sender, EventArgs e)
        {
            List<int>[] All = new List<int>[8];
            for (int i = 0; i < 8; i++) { All[i] = new List<int>(); }
            WindowsFunc.t = AlgTestLog_tb;
            List<Link> l = new List<Link>();
            long t = -1;
            long t2 = -1;
            Random rand = new Random();
            int o, d = -1;
            AlgTest_progressbar.Maximum = (int)RandOD_nud.Value * (int)RunTimes_nud.Value;
            AlgTest_progressbar.Value = 0;
            string ODlist = @"..\..\Records\odlist.txt";
            Console.WriteLine("Get O and D from ODlist?");
            string ODFromFile = Console.ReadLine();
            List<int> OList = new List<int>();
            List<int> DList = new List<int>();
            if (ODFromFile == "Y")
            {
                using (StreamReader sr = new StreamReader(ODlist, true))
                {
                    while (sr.Peek() > 0)
                    {
                        string line = sr.ReadLine();
                        string[] arr = line.Split(',');
                        OList.Add(int.Parse(arr[0]));
                        DList.Add(int.Parse(arr[1]));
                    }
                }
            }
            for (int i = 0; i < RandOD_nud.Value; i++)
            {
                o = rand.Next(0, CurrentNet.AllNodes.Count);//from 1 to count
                do { d = rand.Next(0, CurrentNet.AllNodes.Count); }
                while (o == d);//from 1 to count
                if (ODFromFile == "N")
                {
                    using (StreamWriter sw2 = new StreamWriter(ODlist, true))
                    {
                        sw2.WriteLine("{0},{1}", o, d);
                        sw2.Close();
                    }
                }
                else
                {
                    o = OList[i];
                    d = DList[i];
                }

                GoalDirectedFibDijkstra(CurrentNet, o, d, true, out t);
                bool Accessible = false;
                GetShortestPath(CurrentNet, o, d, true, out Accessible);

                if (Accessible == true)
                {
                    FileStream fs = new FileStream("..\\..\\Records\\record.csv", FileMode.Append);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("o,d");//        "o,d"
                    sw.WriteLine("{0},{1}", o, d);//  "1,2355"
                    sw.WriteLine("{0},{1}", "Optimistic Time", CurrentNet.AllNodes[d - 1].OptHeuristic);//od shortest undelay travel time;
                    Dijkstra_Recover(CurrentNet);
                    int RunTime = 0;
                    WindowsFunc.Println("Testing " + AlgName_cb.Text + " from" + o + " to " + d);
                    while (RunTime < RunTimes_nud.Value)
                    {
                        switch (AlgName_cb.SelectedIndex)
                        {
                            case 0:
                                WindowsFunc.Println("Now Testing " + AlgName_cb.Text);
                                WindowsFunc.Measure(() => { HPD_raw(CurrentNet, o, d, ref l, out t); });
                                DHS_Recover(CurrentNet, l);
                                WindowsFunc.Println("-------------------------------------------------------------");
                                break;
                            case 1:
                                WindowsFunc.Println("Now Testing " + AlgName_cb.Text);
                                WindowsFunc.Measure(() => { HPD_heu(CurrentNet, o, d, ref l, out t); });
                                Dijkstra_Recover(CurrentNet);
                                DHS_Recover(CurrentNet, l);
                                WindowsFunc.Println("-------------------------------------------------------------");
                                break;
                            case 2:
                                WindowsFunc.Println("Now Testing " + AlgName_cb.Text);
                                WindowsFunc.Measure(() => { HPD_twist(CurrentNet, o, d, ref l, out t); });
                                WindowsFunc.Println("-------------------------------------------------------------");
                                DHS_Recover(CurrentNet, l);
                                break;
                            case 3:
                                WindowsFunc.Println("Now Testing " + AlgName_cb.Text);
                                WindowsFunc.Measure(() => { HPD_twistheu(CurrentNet, o, d, ref l, out t); });
                                WindowsFunc.Println("-------------------------------------------------------------");
                                Dijkstra_Recover(CurrentNet);
                                DHS_Recover(CurrentNet, l);
                                break;
                            //case 4:
                            //    WindowsFunc.Println("Testing from" + o + " to " + d);
                            //    WindowsFunc.Measure(() => { DHS(CurrentNet, o, d, ref l, out t, out t2); });
                            //    WindowsFunc.Println("-------------------------------------------------------------");
                            //    Dijkstra_Recover(CurrentNet);
                            //    DHS_Recover(CurrentNet, l);
                            //break;
                            case 4:
                                WindowsFunc.Println("Testing from" + o + " to " + d);
                                WindowsFunc.Measure(() => { HP(CurrentNet, o, d, ref l, out t); });
                                WindowsFunc.Println("-------------------------------------------------------------");
                                Dijkstra_Recover(CurrentNet);
                                DHS_Recover(CurrentNet, l);
                                break;
                            case 5:
                                WindowsFunc.Println("Testing from" + o + " to " + d);
                                WindowsFunc.Measure(() => { HP_heu(CurrentNet, o, d, ref l, out t, out t2); });
                                WindowsFunc.Println("-------------------------------------------------------------");
                                Dijkstra_Recover(CurrentNet);
                                DHS_Recover(CurrentNet, l);
                                break;
                            case 6:
                                WindowsFunc.Println("Testing from" + o + " to " + d);
                                WindowsFunc.Measure(() => { HP_twist(CurrentNet, o, d, ref l, out t); });
                                WindowsFunc.Println("-------------------------------------------------------------");
                                Dijkstra_Recover(CurrentNet);
                                DHS_Recover(CurrentNet, l);
                                break;
                            case 7:
                                WindowsFunc.Println("Testing from" + o + " to " + d);
                                WindowsFunc.Measure(() => { DHS(CurrentNet, o, d, ref l, out t, out t2); });
                                WindowsFunc.Println("-------------------------------------------------------------");
                                Dijkstra_Recover(CurrentNet);
                                DHS_Recover(CurrentNet, l);
                                break;
                            case 8:
                                //WindowsFunc.Println("-------------------------------------------------------------");
                                //WindowsFunc.Measure(() => { HPD_raw(CurrentNet, o, d, ref l, out t); });

                                //sw.WriteLine("{0},{1}","HPD",t);
                                //Dijkstra_Recover(CurrentNet);
                                ////DHS_Recover(CurrentNet, l);
                                //All[0].Add((int)t);
                                //WindowsFunc.Println("-------------------------------------------------------------");
                                //WindowsFunc.Measure(() => { HPD_heu(CurrentNet, o, d, ref l, out t); });
                                //sw.WriteLine("{0},{1}", "HPD_heu", t);
                                //Dijkstra_Recover(CurrentNet);
                                //DHS_Recover(CurrentNet, l);
                                //All[1].Add((int)t);

                                //WindowsFunc.Println("-------------------------------------------------------------");
                                //WindowsFunc.Measure(() => { HPD_twist(CurrentNet, o, d, ref l, out t); });
                                //sw.WriteLine("{0},{1}", "HPD_twist", t);
                                //Dijkstra_Recover(CurrentNet);
                                //DHS_Recover(CurrentNet, l);
                                //All[2].Add((int)t);

                                WindowsFunc.Println("-------------------------------------------------------------");
                                WindowsFunc.Measure(() => { HPD_twistheu(CurrentNet, o, d, ref l, out t); });
                                sw.WriteLine("{0},{1}", "HPD_twistheu", t);
                                Dijkstra_Recover(CurrentNet);
                                DHS_Recover(CurrentNet, l);
                                All[3].Add((int)t);

                                //WindowsFunc.Println("-------------------------------------------------------------");
                                //WindowsFunc.Measure(() => { HP(CurrentNet, o, d, ref l, out t); });
                                //sw.WriteLine("{0},{1}", "HP", t);
                                //Dijkstra_Recover(CurrentNet);
                                //DHS_Recover(CurrentNet, l);
                                //All[4].Add((int)t);

                                //WindowsFunc.Println("-------------------------------------------------------------");
                                //WindowsFunc.Measure(() => { HP_heu(CurrentNet, o, d, ref l, out t, out t2); });
                                //sw.WriteLine("{0},{1}", "HP_heu", t);
                                //Dijkstra_Recover(CurrentNet);
                                //DHS_Recover(CurrentNet, l);
                                //All[5].Add((int)t);

                                //WindowsFunc.Println("-------------------------------------------------------------");
                                //WindowsFunc.Measure(() => { HP_twist(CurrentNet, o, d, ref l, out t); });
                                //sw.WriteLine("{0},{1}", "HP_twist", t);
                                //Dijkstra_Recover(CurrentNet);
                                //DHS_Recover(CurrentNet, l);
                                //All[6].Add((int)t);

                                WindowsFunc.Println("-------------------------------------------------------------");
                                WindowsFunc.Measure(() => { DHS(CurrentNet, o, d, ref l, out t, out t2); });
                                sw.WriteLine("{0},{1}", "DHS", t2);
                                Dijkstra_Recover(CurrentNet);
                                DHS_Recover(CurrentNet, l);
                                All[7].Add((int)t2);

                                WindowsFunc.Println("-------------------------------------------------------------");
                                break;
                            default: break;
                        }
                        RunTime++;
                        AlgTest_progressbar.Value++;
                    }
                    sw.Close();
                    WindowsFunc.Println("**************************************************************");
                    WindowsFunc.Println("Completed");
                    WindowsFunc.Println("**************************************************************");
                    textBox1.Text = textBox1.Text + o + "-->" + d + "\r\n";
                    for (int algindex = 0; algindex < 8; algindex++)
                    {
                        textBox1.Text = textBox1.Text + "-------------------------------------------------------------\r\n";
                        textBox1.Text = textBox1.Text + AlgName_cb.Items[algindex] + "\r\n";
                        foreach (var element in All[algindex])
                        {
                            textBox1.Text = textBox1.Text + element + "\r\n";
                        }
                        textBox1.Text = textBox1.Text + "-------------------------------------------------------------\r\n";
                    }
                    foreach (var list in All) { list.Clear(); }
                    textBox1.Text = textBox1.Text + "**************************************************************\r\n";
                }
            }
        }

        private void ClearLog_btn_Click(object sender, EventArgs e)
        {
            AlgTestLog_tb.Text = string.Empty;

            textBox1.Text = string.Empty;
        }

        private void Clear_btn_Click(object sender, EventArgs e)
        {
            AlgLog_tb.Text = string.Empty;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
       
        
        
        #region 自定义的函数

        #region 数据准备
        //数据库连接，数据准备，读取网络


        public bool PrepareData(out long LoadingTime)
        {
            LoadingTime = -1;
            DataTable dt_node;
            //int nodecount=2500;//testnet 的节点个数
            //try
            //{
            Stopwatch sss = new Stopwatch();
            DataTable dt_link;
            sss.Start();
            //连接数据库
            CurrentNet = new Network();

            using (NpgsqlConnection conn = new NpgsqlConnection(ConnStr))
            {
                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);

#if NonGISTopo
                int nodecount = 2500;
                for (int i = 0; i < nodecount; i++)
                {
                    Node newnode=null;
                    //通过节点id产生新的节点
                    newnode = new Node(i+1, -1, -1);
                    CurrentNet.AllNodes.Add(newnode);
                }
#else
                //产生全节点集
                cmd.CommandText = "SELECT * FROM vertices_tmp ORDER BY id;";

                dt_node = new DataTable("nodes");

                da.Fill(dt_node);

                if (conn.State == ConnectionState.Closed) conn.Open();
                for (int i = 0; i < dt_node.Rows.Count; i++)
                {
                    Node newnode=null;
                    //通过节点id产生新的节点
                    if(dt_node.Columns.Contains("x")&&dt_node.Columns.Contains("y"))
                        newnode = new Node((int)dt_node.Rows[i]["id"], (double)dt_node.Rows[i]["x"], (double)dt_node.Rows[i]["y"]);
                    else
                        newnode = new Node((int)dt_node.Rows[i]["id"], -1, -1);

                    CurrentNet.AllNodes.Add(newnode);
                }
#endif
                cmd.CommandText = "SELECT * FROM road ORDER BY gid;";
                dt_link = new DataTable("links");
                da.Fill(dt_link);
                cmd.Dispose();
                da.Dispose();
                //dt_node.Dispose();
                dt_link.Dispose();
                conn.Close();
            }

            if (MaxDelayLevel_cmb.SelectedIndex == 6) maxdelaylevel = 10;
            else if (MaxDelayLevel_cmb.SelectedIndex == 7) maxdelaylevel = 0.5;
            else maxdelaylevel = MaxDelayLevel_cmb.SelectedIndex;

            //通过路段表将路段添加到全路段集中
            for (int i = 0; i < dt_link.Rows.Count; i++)
            {
                DataRow row = dt_link.Rows[i];
                 
                switch (Convert.ToInt32(row["direction"]))
                {
                    case 0://如果该路段是双向通行，则在路段链表中产生两条路段，分配id，并将他们加入各自对应的节点出入弧集合中
                        //添加数字化方向同向路段
                        Link newlink = new Link(CurrentNet.AllLinks.Count + 1, row, maxdelaylevel, true);
                        CurrentNet.AllLinks.Add(newlink);
                        CurrentNet.AllNodes[newlink.FromGID - 1].AddOutLink(newlink);
                        CurrentNet.AllNodes[newlink.ToGID - 1].AddInLink(newlink);

                        //添加数字化方向反向路段,true 和 false 标识了路段构建类型，他们的from和to正好相反
                        newlink = new Link(CurrentNet.AllLinks.Count + 1, row, maxdelaylevel, false);
                        CurrentNet.AllLinks.Add(newlink);
                        CurrentNet.AllNodes[newlink.FromGID - 1].AddOutLink(newlink);
                        CurrentNet.AllNodes[newlink.ToGID - 1].AddInLink(newlink);
                        break;
                    case 1:
                        newlink = new Link(CurrentNet.AllLinks.Count + 1, row, maxdelaylevel, true);
                        CurrentNet.AllLinks.Add(newlink);
                        CurrentNet.AllNodes[newlink.FromGID - 1].AddOutLink(newlink);
                        CurrentNet.AllNodes[newlink.ToGID - 1].AddInLink(newlink);
                        break;
                    case 2:
                        newlink = new Link(CurrentNet.AllLinks.Count + 1, row, maxdelaylevel, false);
                        CurrentNet.AllLinks.Add(newlink);
                        CurrentNet.AllNodes[newlink.FromGID - 1].AddOutLink(newlink);
                        CurrentNet.AllNodes[newlink.ToGID - 1].AddInLink(newlink);
                        break;
                }
            }


            sss.Stop();
            LoadingTime = sss.ElapsedMilliseconds;
            return true;
            //}
            //catch { return false; }
        }

        //保存网络到二进制文件Network.dat
        public bool SaveNetwork(string _filepath)
        {
            try
            {
                Network network = new Network(CurrentNet.AllNodes, CurrentNet.AllLinks);
                FileStream fs = new FileStream(_filepath, FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, network);
                fs.Close();
                fs.Dispose();
                return true;
            }
            catch { return false; }
        }

        //从保存的二进制文件中读取网络结构
        public bool ReadNetwork(string _filepath, out Network _network)
        {
            try
            {
                Network network = new Network();
                FileStream fs = new FileStream(_filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryFormatter bf = new BinaryFormatter();
                network = bf.Deserialize(fs) as Network;
                _network = network;
                fs.Close();
                fs.Dispose();
            }
            catch
            {
                _network = null; return false;
            }
            return true;
        }

        #endregion  

        #region 导出网络拓扑表

        void ExportTopo(Network WorkingNet, string NodePath, string LinkPath)
        {
            StreamWriter sw = new StreamWriter(LinkPath);
            sw.WriteLine("'ID,From,To,t,d,");
            foreach (Link i in WorkingNet.AllLinks)
            {
                //if (i.Fa == Link.INFINITE)
                if (i.Fa == double.PositiveInfinity)
                    sw.WriteLine(i.ID + "," + i.FromGID + "," + i.ToGID + "," + i.TravelTime_variable + "," + 0 + ",");
                else
                    sw.WriteLine(i.ID + "," + i.FromGID + "," + i.ToGID + "," + i.TravelTime_variable + "," + 1 / i.Fa + ",");
            }
            sw.Close();
            sw = new StreamWriter(NodePath);
            sw.WriteLine("'ID,");
            foreach (Node i in WorkingNet.AllNodes)
            {
                sw.WriteLine(i.GID + ",");
            }
            sw.Close();
        }

        #endregion

        #region 创建关联表(并没有用到)
        //创建关联表
        public static DataTable Join(DataTable First, DataTable Second, DataColumn[] FJC, DataColumn[] SJC)
        {

            //创建一个新的DataTable

            DataTable table = new DataTable("Join");


            // Use a DataSet to leverage DataRelation

            using (DataSet ds = new DataSet())
            {

                //把DataTable Copy到DataSet中

                ds.Tables.AddRange(new DataTable[] { First.Copy(), Second.Copy() });

                DataColumn[] parentcolumns = new DataColumn[FJC.Length];

                for (int i = 0; i < parentcolumns.Length; i++)
                {

                    parentcolumns[i] = ds.Tables[0].Columns[FJC[i].ColumnName];

                }

                DataColumn[] childcolumns = new DataColumn[SJC.Length];

                for (int i = 0; i < childcolumns.Length; i++)
                {

                    childcolumns[i] = ds.Tables[1].Columns[SJC[i].ColumnName];

                }




                DataRelation r = new DataRelation(string.Empty, parentcolumns, childcolumns, false);

                ds.Relations.Add(r);


                //为关联表创建列

                for (int i = 0; i < First.Columns.Count; i++)
                {

                    table.Columns.Add(First.Columns[i].ColumnName, First.Columns[i].DataType);

                }

                for (int i = 0; i < Second.Columns.Count; i++)
                {

                    //看看有没有重复的列，如果有在第二个DataTable的Column的列明后加_Second

                    if (!table.Columns.Contains(Second.Columns[i].ColumnName))

                        table.Columns.Add(Second.Columns[i].ColumnName, Second.Columns[i].DataType);

                    else

                        table.Columns.Add(Second.Columns[i].ColumnName + "_Second", Second.Columns[i].DataType);

                }


                table.BeginLoadData();

                foreach (DataRow firstrow in ds.Tables[0].Rows)
                {

                    //得到行的数据

                    DataRow[] childrows = firstrow.GetChildRows(r);

                    if (childrows != null && childrows.Length > 0)
                    {

                        object[] parentarray = firstrow.ItemArray;

                        foreach (DataRow secondrow in childrows)
                        {

                            object[] secondarray = secondrow.ItemArray;

                            object[] joinarray = new object[parentarray.Length + secondarray.Length];

                            Array.Copy(parentarray, 0, joinarray, 0, parentarray.Length);

                            Array.Copy(secondarray, 0, joinarray, parentarray.Length, secondarray.Length);

                            table.LoadDataRow(joinarray, true);

                        }

                    }

                }

                table.EndLoadData();

            }


            return table;

        }

        public static DataTable Join(DataTable First, DataTable Second, DataColumn FJC, DataColumn SJC)
        {

            return Join(First, Second, new DataColumn[] { FJC }, new DataColumn[] { SJC });

        }

        public static DataTable Join(DataTable First, DataTable Second, string FJC, string SJC)
        {

            return Join(First, Second, new DataColumn[] { First.Columns[FJC] }, new DataColumn[] { Second.Columns[SJC] });

        }
        #endregion

        #endregion

        #region Simulation

        //初始化各路径结果集的路段向量
        private Dictionary<string, List<int>> InitLinkSets(Network WorkingNet, int o, int d, bool GlobalOnPONet)
        {
            Dictionary<string, List<int>> LinkSetResults = new Dictionary<string, List<int>>();
          

            //Do the optimistic shortest path search and save the optimistic path as List<int> OptSP
            long t = -1;
            List<Link> temp = null;
            List<Link> POSet = null;
           
            FibDijkstra(WorkingNet, o, d, true, out t);
            bool Accessible = false;
            List<int> OptSP = GetShortestPath(WorkingNet, o, d, true, out Accessible);
            string info = String.Format("OptSP @ {0} ms", t);
            LinkSetResults.Add(info, OptSP);//乐观最短路
            Dijkstra_Recover(WorkingNet);
            //Do the pessimistic shortest path search and save the pessimistic PesSP
            FibDijkstra(WorkingNet, o, d, false, out t);
            List<int> PesSP = GetShortestPath(WorkingNet, o, d, true, out Accessible);
            info = String.Format("PesSP @ {0} ms", t);
            LinkSetResults.Add(info, PesSP);//悲观最短路
            //
            //temp code
            StreamWriter swtemp = new StreamWriter(@"D:\temp.txt");
            foreach ( int i in PesSP)
            {
                swtemp.WriteLine(WorkingNet.AllLinks[i-1].GID);
            }
            swtemp.Close();
            ////
            Dijkstra_Recover(WorkingNet);
            //DHS_Recover(WorkingNet, temp);
            //Do the local search and save the local regret path as LSP
            Stopwatch stw = new Stopwatch();
            Stopwatch stwtemp = new Stopwatch();
            stwtemp.Start();
            RSA(WorkingNet, o, d, out POSet, out t, out t);
            stwtemp.Stop();
            MessageBox.Show(stwtemp.ElapsedMilliseconds+"ms");
            List<Link> LSP = null;
            stw.Start();
            if(!SimLocalIncludeEt_ckb.Checked)
                RegretPath_TRBLocal_BackwardPenalty(WorkingNet, o, d, POSet, out LSP, out t);
            else
                RegretPath_TRBLocal_BackwardPenaltyWithEU(WorkingNet, o, d, POSet, out LSP, out t);
            stw.Stop();
            info = String.Format("LSP @ {0} ms", stw.ElapsedMilliseconds);
            List<int> LSPint = new List<int>();
            foreach (Link i in LSP) { LSPint.Add(i.ID); }
            LinkSetResults.Add(info, LSPint);//Local Regret最短路
            Dijkstra_Recover(WorkingNet);
            DHS_Recover(WorkingNet, temp);//不清空POSet，因为后面还要用到
            //Do the global search and save the global regret path as GSP
            List<Link> GSP = null;
            stw.Restart();
            //如果在PONetwork上执行Global Regret path，为使得Global与Local的网络Context保持一致一般不这么做
            if (!GlobalOnPONet)
            {
                stw.Restart();
                RegretPath_TRBGlobal(WorkingNet, o, d, out GSP, out t);
            }
            else
            {
               
                long tempt = -1;
                AssignRegretForTRBGlobal(CurrentNet, (int)HyperFrom_nud.Value, (int)HyperTo_nud.Value, true, out tempt);
                /*Constuct Subnetwork,Regret延用父网络的值*/
                /**********************************************************************************************/
                Network PONetwork = CurrentNet.CreateSubNetwork(POSet.ToArray());
                SubNet = PONetwork;//设置当前子网络为PONetwork
                int NewOriginID = 0;
                int NewDestinationID = 0;
                foreach (Node i in PONetwork.AllNodes)
                {
                    if (i.GID == (int)this.HyperFrom_nud.Value)
                        NewOriginID = i.SubID;
                    else if (i.GID == (int)this.HyperTo_nud.Value)
                        NewDestinationID = i.SubID;
                }
                /**********************************************************************************************/
                stw.Restart();
                RegretPath_TRBGlobalForSubNet(PONetwork, NewOriginID, NewDestinationID, out GSP, out t);
            }
            stw.Stop();
            Dijkstra_Recover(WorkingNet);
            DHS_Recover(WorkingNet, POSet);//清空POSet，结束
            info = String.Format("GSP @ {0} ms", stw.ElapsedMilliseconds);
            List<int> GSPint = new List<int>();
            foreach (Link i in GSP) { GSPint.Add(i.ID); }
            LinkSetResults.Add(info, GSPint);//Global Regret最短路
            //Build the link vector for the OptSP, PesSP, LSP, GSP
            return LinkSetResults;
        }

        private void StartSimulation(int o, int d, Network WorkingNet, bool GlobalOnPONet)
        {
            
            if (Vectors == null)
            {
                Vectors = new Dictionary<string, List<int>>();
                LinkSets = InitLinkSets(WorkingNet, o, d, GlobalOnPONet);
                foreach (var j in LinkSets.Keys)
                {
                    List<int> Vector = GetLinkVector(WorkingNet, LinkSets[j]);
                    Vectors.Add(j, Vector);
                }
            }
            //Set the Monte Carlo situation for the network by randomly generate the the maxdelay
            Random rand = new Random(DateTime.Now.Millisecond);
          

            foreach (Link i in WorkingNet.AllLinks)
            {
                int z = (rand.NextDouble() >= 0.5) ? 1 : 0;
                //i.TravelTime_variable = i.TravelTime_Fixed + /*da*/ (1 / i.Fa) * rand.NextDouble();
                i.TravelTime_variable = i.TravelTime_Fixed + /*da*/ (1 / i.Fa) * z;
            }
            long time = -1;
            bool accessible = false;

            //Search the actual shortest path for the simulated scenario
            GoalDirectedFibDijkstra(WorkingNet, o, d, true, out time);
            //Record the actual shortest path as SimSP
            string[,] records = new string[5, 4];
            //row: 5 paths
            //column: Info, link number, time, similarity
            List<int> SimSP = GetShortestPath(WorkingNet, o, d, true, out accessible);
            //Build the vector for SimSP
            List<int> VectSimSP = GetLinkVector(WorkingNet, SimSP);
            //List<double> similarities = new List<double>();
            //Call Cosine function between SimSP and other 4. (4 cosine similarities)
            records[0, 0] = "SimSP @ " + time + " ms";
            records[0, 1] = SimSP.Count.ToString();
            records[0, 2] = GetTravelTime(WorkingNet, SimSP).ToString();
            records[0, 3] = "1";
            int count = 0;
            foreach (var i in Vectors.Keys)
            {
                count++;
                double similarity = Cosine(VectSimSP, Vectors[i]);
                records[count, 0] = i;
                records[count, 1] = LinkSets[i].Count.ToString();
                records[count, 2] = GetTravelTime(WorkingNet, LinkSets[i]).ToString();
                records[count, 3] = similarity.ToString();
            }
            //Record the result

            WriteRecordsToFile(@"D:\SimRecords.csv", records);

            //Recycle
            Dijkstra_Recover(CurrentNet);
            foreach (Link i in CurrentNet.AllLinks)
            {
                i.TravelTime_variable = i.TravelTime_Fixed;
            }
        }

        private List<int> GetLinkVector(Network WorkingNet, List<int> LinkSetResult)
        {
            List<int> Vector = new List<int>();
            for (int i = 0; i < CurrentNet.AllLinks.Count; i++)
            {
                if (LinkSetResult.Contains(CurrentNet.AllLinks[i].ID))
                    Vector.Add(1);
                else Vector.Add(0);
            }
            return Vector;
        }

        private double Cosine(List<int> vect1, List<int> vect2)
        {
            double similarity = 0;
            double bc = 0;
            double bsquare = 0;
            double csquare = 0;
            for (int i = 0; i < vect1.Count; i++)
            {
                bc += vect1[i] * vect2[i];
                bsquare += vect1[i] * vect1[i];
                csquare += vect2[i] * vect2[i];
            }
            similarity = bc / (Math.Sqrt(bsquare) * Math.Sqrt(csquare));
            return similarity;
        }

        private double GetTravelTime(Network WorkingNet, List<int> Path)
        {
            double sum = 0;
            foreach (var i in Path)
            { sum += WorkingNet.AllLinks[i - 1].TravelTime_variable; }
            return sum;
        }

        private void WriteRecordsToFile(string path, string[,] records)
        {
            FileStream fs = new FileStream(path, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("***********************************************************************");
            sw.WriteLine("PathInfo,LinkNumber,ActualTime,Similarity");

            for (int i = 0; i < 5; i++)
            {
                string s = string.Empty;
                for (int j = 0; j < 4; j++)
                {
                    if (j != 3) s = s + records[i, j] + ",";
                    else s += records[i, j];
                }
                sw.WriteLine(s);
                if (RunTimes[i] == null) RunTimes[i] = new List<double>();
                RunTimes[i].Add(Convert.ToDouble(records[i, 0].Split('@')[1].TrimEnd(new char[] { 'm', 's' })));
                SumRunTime[i] += Convert.ToDouble(records[i, 0].Split('@')[1].TrimEnd(new char[] { 'm', 's' }));
                if (LinkNums[i] == null) LinkNums[i] = new List<int>();
                LinkNums[i].Add(Convert.ToInt32(records[i, 1]));
                SumLinkNum[i] += Convert.ToInt32(records[i, 1]);
                if (Times[i] == null) Times[i] = new List<double>();
                Times[i].Add(Convert.ToDouble(records[i, 2]));
                SumTime[i] += Convert.ToDouble(records[i, 2]);
                if (Similarities[i] == null) Similarities[i] = new List<double>();
                Similarities[i].Add(Convert.ToDouble(records[i, 3]));
                SumSimilarity[i] += Convert.ToDouble(records[i, 3]);
            }
            sw.WriteLine("***********************************************************************");
            sw.Close();
        }
        #endregion

        /// <summary>
        /// This starts the simulation of shortest paths' inclusion in hyperpath
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start2_btn_Click(object sender, EventArgs e)
        {
            //times of simulation
            int expSPs = (int)RunTimes2_nud.Value;

            //number of experimental OD pairs
            int expODs = (int)RandOD2_nud.Value;

            //the max value of progressbar
            SPSim_progressBar.Value = 0;
            SPSim_progressBar.Maximum = expODs*expSPs;

            int SPiteration = 0;
            int ODiteration = 0;
            int includeAll = 0;//indicates all links of SP are included in Hyperpath
            while (ODiteration < expODs)
            {
                SPiteration = 0;
                includeAll = 0;
                //the random seed to generate random OD pairs
                Random rand = new Random(DateTime.Now.Millisecond);
                int o = -1;
                int d = -1;
                while (o == d)
                {
                    o = rand.Next(CurrentNet.AllNodes.Count);
                    d = rand.Next(CurrentNet.AllNodes.Count);
                }
                long time = -1;//we actually don't care about the time here
                
                //get the hyperpath for the current test OD pair
                List<Link> RawHyperpath = new List<Link>();
                FDHS(CurrentNet, o, d, ref RawHyperpath, out time, out time);
                
                List<Link> Hyperpath = GetHyperpath(RawHyperpath);
                List<int> HP=new List<int>();
                foreach (Link x in Hyperpath)
                {
                    HP.Add(x.ID); 
                }
                //the random seed to generate network travel time changes
                Random rand2 = new Random(DateTime.Now.Millisecond);
                bool Accessible = false;
                int includedlink = 0;
                int SPLinkCount=0;
                while (SPiteration<expSPs)
                {
                    includedlink = 0;
                    //simulate the network travel time changes here.
                    foreach (Link i in CurrentNet.AllLinks)
                    {
                        //realtime T = undelayed T + R[0,1]* delay
                        i.TravelTime_variable = i.TravelTime_Fixed + /*da*/ (1 / i.Fa) * rand.NextDouble();
                    }
                    //calculate the realtime shortest path
                    FibDijkstra(CurrentNet, o, d, true, out time);
                    List<int> SP = GetShortestPath(CurrentNet, o, d, true, out Accessible);
                    
                    //Judge the inclusion of SP in Hyperpath
                    bool exception=false;
                    
                    foreach (int i in SP)
                    {
                        if (HP.Contains(i))
                            includedlink++;
                        else
                            exception = true;
                    }
                    if (!exception) includeAll++;
                    SPiteration++;
                    SPLinkCount=SP.Count;
                    if (Accessible)
                    {
                        using (StreamWriter sw = new StreamWriter(@"..\..\data"+ ODiteration.ToString()+".csv",true))
                        {
                            sw.WriteLine("{0}->{1}, {2}, {3}, {4}", o, d, Accessible, (double)includedlink / SPLinkCount, SPiteration);
                        }
                        Console.WriteLine("Experimental OD:{0}->{1}, {2}, LinkInclusionRatio: {3}, iter:{4}", o, d, Accessible, (double)includedlink / SPLinkCount, SPiteration);
                    }
                    else SPiteration--;
                    SPSim_progressBar.Value = ODiteration * SPiteration;
                    Dijkstra_Recover(CurrentNet);
                }
                ODiteration++;
                List<Link> temp = new List<Link>();
                DHS_Recover(CurrentNet, temp);
            }
            
        }

        private void PassOD_btn_Click(object sender, EventArgs e)
        {
            HyperFrom_nud.Value = Origin_nud.Value;
            HyperTo_nud.Value = Destination_nud.Value;
        }

        private void SP_Sim_Plot_btn_Click(object sender, EventArgs e)
        {
            Process proc = Process.Start(@"..\..\PlotSPSim.py");
            proc.WaitForExit();
            pictureBox1.Image=Image.FromFile(@"fig.png");
        }
    }

    public static class WindowsFunc
    {
        public static TextBox t = null;
        public static void Println(string s)
        {
            t.Text = t.Text + s + "\r\n";
        }
        //计算算法时间
        public static Action<Action> Measure = body =>
        {
            Stopwatch stw = new Stopwatch();
            stw.Start();
            body();
            Println(String.Format("{0} ms", stw.ElapsedMilliseconds));
        };
    }
}
