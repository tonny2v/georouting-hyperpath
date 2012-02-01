//////////////////////////////////////////////////////////////////////////
//1.测试NetworkLib2中的TranNetwork
//2.利用NetworkLib2对数据库中不具有Probe data travel time的路段进行仿真（通过夹角小于alpha的相邻边的平均速度仿真）
//数据源为用Navicat导出的包含列名的txt文件
using System;
using NetworkLib2.TransNetwork.Concept;
using NetworkLib2.TransNetwork.TopoBuilder;
using NetworkLib2.TransNetwork.Algorithm;
using NetworkLib2.Basis;
using System.Collections.Generic;
using WktAnalysis;
using System.IO;

namespace NetworkLib2TranNetworkTest
{

    class Program
    {
        static TNetwork net;
        static void Main(string[] args)
        {
            //testSyntheticNetwork();
            //testCsvInput();
            //testTxtInput();
            //testRawDijkstra();
            ReadNetwork();
            SpeedSimulation();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
        #region 测试函数
       
        static void testSyntheticNetwork()
        {
            net = new TNetwork();
            SyntheticBuilder builder = new SyntheticBuilder();
            net = builder.GenNetwork(SyntheticBuilder.NetworkType.RandomNetwork, 1000, 4000, 50, 100, 25, 50);
            printNetInfo_detail(net);
        }

        static void testCsvInput()
        {
            net = new TNetwork();
            FileBuilder builder = new FileBuilder(@"..\..\TestNetworkData.csv", false);
            builder.GetNetwork();
            net = builder.Net;
            printNetInfo_detail(net);
        }

        static void testTxtInput()
        {
            net = new TNetwork();
            FileBuilder builder = new FileBuilder(@"..\..\TestNetworkData.txt", false);
            builder.GetNetwork();
            net = builder.Net;
            printNetInfo_detail(net);
        }

        static void testRawDijkstra()
        {
            testSyntheticNetwork();
            //testCsvInput();

            //SinglePathAlgorithm alg = new SinglePathAlgorithm(net);
            TempSPAlg alg = new TempSPAlg();
            alg.Net = net;
            alg.O_ID = "1";
            alg.InitializeVariables(AlgName.RawDijkstra);

            alg.Run(AlgName.RawDijkstra);
            alg.D_ID = "5";

            string[] output = alg.GetResultSet();
            string s = string.Empty;
            Console.WriteLine("Raw Dijkstra SinglePathAlgorithm Time: {0} ms", alg.GetTime());

            //反过来输出最短路，此处是以节点表示的最短路
            for (int i = output.Length - 1; i >= 0; i--)
            {
                if (i != output.Length - 1)
                    s = s + "->" + output[i];
                else
                    s = s + output[i];
            }

            Console.WriteLine("SP: {0}", s);


        }

        static void printNetInfo_detail(TNetwork net)
        {
            Console.WriteLine(net);

            foreach (Node i in net.Vertices.Values)
            {
                Console.WriteLine("Node {0}: InLinks ({1}) OutLinks ({2})", i, i.InLinks, i.OutLinks);
            }
            foreach (Link i in net.Edges.Values)
            {
                Console.WriteLine("Link {0}, t ({1}), d ({2})", i, i.TravelTime, i.Delay);
            }
        }

        static void printNetInfo_brief(TNetwork net)
        {
            Console.WriteLine(net);
        }

        #endregion

        #region 速度仿真
        //读取由GIS网络得到的csv文件并构建网络
        static void ReadNetwork()
        {
            net = new TNetwork();
            FileBuilder builder = new FileBuilder(@"..\..\road.txt", false);
            builder.GetNetwork();
            net = builder.Net;
            //printNetInfo_detail(net);
            printNetInfo_brief(net);
        }
        static void SpeedSimulation()
        {
            ////////////////////////////////////////////////
            /*********************************************/
            int reliable = 20;
            ///////////////////////////////////////////////
            Dictionary<string, Link> SpeedNullLinks = new Dictionary<string, Link>();
            HashSet<Link> SpeedLinks = new HashSet<Link>();
            Link[] LinkArray=null;
            AngleAlg.DefinedAngle = 15;
            foreach (Link i in net.Edges.Values)
            {
                if (i.RawData.n < reliable)//如果数据小于20，则认为是不可靠数据
                {
                    if (!SpeedNullLinks.ContainsKey(i.RawData.gid))
                    {
                        SpeedNullLinks.Add(i.RawData.gid, i);
                    }
                }
                else SpeedLinks.Add(i);
            }
            int speednulllinkscount = 0;
            while (true)
            {
                foreach (Link i in SpeedNullLinks.Values)
                {
                    //将路段i的所有邻接物理边添加到邻接集合中
                    Dictionary<string, RawLinkDataRow> NeighborLinks = new Dictionary<string, RawLinkDataRow>();
                    foreach (Link j in i.From.InLinks.Values)
                    {
                        if (!NeighborLinks.ContainsKey(j.RawData.gid))
                            if (j.RawData.gid != i.RawData.gid)
                                if (j.RawData.n >= reliable)
                                    NeighborLinks.Add(j.RawData.gid, j.RawData);
                    }
                    foreach (Link j in i.From.OutLinks.Values)
                    {
                        if (!NeighborLinks.ContainsKey(j.RawData.gid))
                            if (j.RawData.gid != i.RawData.gid)
                                if (j.RawData.n >= reliable)
                                    NeighborLinks.Add(j.RawData.gid, j.RawData);
                    }
                    foreach (Link j in i.To.InLinks.Values)
                    {
                        if (!NeighborLinks.ContainsKey(j.RawData.gid))
                            if (j.RawData.gid != i.RawData.gid)
                                if (j.RawData.n >=reliable)
                                    NeighborLinks.Add(j.RawData.gid, j.RawData);
                    }
                    foreach (Link j in i.To.OutLinks.Values)
                    {
                        if (!NeighborLinks.ContainsKey(j.RawData.gid))
                            if (j.RawData.gid != i.RawData.gid)
                                if (j.RawData.n >=reliable)
                                    NeighborLinks.Add(j.RawData.gid, j.RawData);
                    }
                    if (NeighborLinks.Count == 12) throw new Exception();
                    double sumspeed = 0;
                    double count = 0;
                    foreach (RawLinkDataRow j in NeighborLinks.Values)
                    {
                        //if (NeighborLinks.Count >= 2)
                        //{
                        if (AngleAlg.JudgeAngle(i.RawData.geomwkt, j.geomwkt) == "true")
                        {
                            sumspeed +=  j.length/j.mean;
                            count++;
                        }
                        //}
                        //else
                        //{
                        //    sumspeed += j.mean / j.length;
                        //    count++;
                        //}
                    }
                    if (count != 0)
                    {
                        i.RawData.traveltime = i.RawData.length / (sumspeed / count);
                        //有时可能会产生负的仿真值
                        if (i.RawData.traveltime - i.RawData.expected > 0)
                        {
                            i.RawData.maxdelay = i.RawData.traveltime - i.RawData.expected;
                            i.RawData.n = 999999;//在判断在下一步仿真时是否作为数据源时也要用到
                            SpeedLinks.Add(i);
                        }
                    }
                }
                foreach (var i in SpeedLinks)
                {
                    SpeedNullLinks.Remove(i.RawData.gid);
                    //finally still 29 dead links, not connected to the network, can be ignored
                }
                //判断是否SpeedNullLinks不再变化，如果不再变化，则仿真结束，将仿真后的表格输出为txt文件（新的road.txt）
                //再用NaviCat将仿真后的road.txt导入数据库
                if (speednulllinkscount == SpeedNullLinks.Count)
                {
                    LinkArray = new Link[SpeedLinks.Count + SpeedNullLinks.Count];
                    SpeedLinks.CopyTo(LinkArray);
                    SpeedNullLinks.Values.CopyTo(LinkArray, SpeedLinks.Count);
                    break; 
                }
                else speednulllinkscount = SpeedNullLinks.Count;
                //如果网络中不再有null links，一般条件下比较困难
                if (SpeedNullLinks.Count == 0)
                {
                    LinkArray = new Link[SpeedLinks.Count];
                    SpeedLinks.CopyTo(LinkArray);
                    break;
                }
              
                Console.WriteLine(speednulllinkscount);
            }

            int f = 0;
            foreach (var i in LinkArray)
            {
               
                if (i.RawData.maxdelay < 0)
                {
                    f++;
                    Console.WriteLine(i.RawData.n);
                }
            }
            Console.WriteLine(f);
            string[] allstrs = File.ReadAllLines("..\\..\\road.txt");
            StreamWriter sw = new StreamWriter("..\\..\\roadsim.txt");
            
            foreach (var i in LinkArray)
            {
                string[] strrow = allstrs[Convert.ToInt32(i.RawData.gid)].Split('\t'); 
                if (i.RawData.n == 999999)
                {
                    //更新txt中对应gid的路段的time和maxdelay数据
                    //更新结束后，将更新好的txt用NaviCat导入数据库
                    //time
                    strrow[17]=i.RawData.traveltime.ToString();
                    //maxdelay
                    strrow[18] = i.RawData.maxdelay.ToString();
                }
                sw.WriteLine(string.Join("\t", strrow));
            }
            sw.Close();
        }
       

        #endregion
    }
    public class TempSPAlg : SinglePathAlgorithm
    {
        public override void OnAddEvent_Handler(object sender, EventArgs e, Node i)
        {
            //base.OnAddEvent_Handler(sender, e, i);
            //此处通过子类继承基类后添加用户定义的事件处理函数，可忽略基类的事件处理函数
            Console.WriteLine("one new node is rendered");
        }
    }
}
