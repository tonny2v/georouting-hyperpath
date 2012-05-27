using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;


using NetworkLib.Element;
using NetworkLib.FibHeap;
using NetworkLib;
using System.Text;

namespace GeoHyperstar.Forms
{
    public partial class GeoHyperStar_MainForm
    {
        #region Dijkstra最短路算法

        /// <summary>
        /// Dijkstra最短路算法
        /// </summary>
        /// <param name="o">起点</param>
        /// <param name="d"></param>
        /// <param name="TimeSpan"></param>
        /// <returns></returns>

        bool Dijkstra(Network WorkingNet, int o, out long TimeSpan)//这里directed为最短路计算时是否考虑道路方向的参数
        {
            bool success = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                List<Node> Updated = new List<Node>();  //Updated node collection, add the new updated nodes and delete the permanent nodes dynamically
                List<Node> ProcessFinished = new List<Node>();//Permanent collection
                WorkingNet.AllNodes[o - 1].OptHeuristic = 0;
                WorkingNet.AllNodes[o - 1].HasProcessed = true;
                Updated.Add(WorkingNet.AllNodes[o - 1]);
                double temp;
                int flag = 0;
                //bool Status = true;
                while (true)//go on the loop before every node gets into the permanent collection
                {
                    temp = double.PositiveInfinity;
                    //得到P标号点

                    for (int i = 0; i < Updated.Count; i++)
                    {
                        if (Updated[i].HasProcessed == false)
                        {
                            if (Updated[i].OptHeuristic <= temp)
                            {
                                temp = Updated[i].OptHeuristic;
                                flag = i;
                            }
                        }
                    }
                    //如果updated集合里全部是已经获得P标号的点，则跳出循环（有些节点由于路段的方向限制而不可达，所以无法更新到其标号，这种情况下这些节点的Heuristic标号保持无穷大，即不可达）
                    if (flag == Updated.Count)
                    {
                        break;
                    }
                    Node tempnode, tempnextnode;
                    tempnode = Updated[flag];
                    tempnode.HasProcessed = true;
                    ProcessFinished.Add(Updated[flag]);//Push into the collection with permanent label
                    Updated.Remove(Updated[flag]);
                    for (int j = 0; j < tempnode.OutLinks.Count; j++)
                    {
                        //******************************************************************错误就在于这里，不一定是ToID，也有可能更新FromID
                        //tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].ToID - 1];
                        //应该是更新流出路段的另一节点，而非一定是ToID。因为在路网拓扑构建的时候就已经考虑了路段的方向性。因此此处的tonode实际上是outlink的除了当前点之外的另一节点，可能是tonode，也可能是fromnode
                        if (tempnode.GID == tempnode.OutLinks[j].ToGID)
                            tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].From.ID - 1];
                        else tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].To.ID - 1];
                        // ********************************************************************
                        if (tempnextnode.OptHeuristic > tempnode.OptHeuristic + tempnode.OutLinks[j].TravelTime_variable)
                        {
                            tempnextnode.OptHeuristic = tempnode.OptHeuristic + tempnode.OutLinks[j].TravelTime_variable;
                            tempnextnode.NextNodeID = tempnode.GID;
                            if (tempnextnode.HasProcessed == false)
                            {
                                Updated.Add(tempnextnode);
                            }
                        }
                    }
                }
                success = true;
            }
            catch { success = false; }
            sw.Stop();
            TimeSpan = sw.ElapsedMilliseconds;
            return success;
        }

        /// <summary>
        /// 以节点或路段形式得到最短路径
        /// </summary>
        /// <param name="o">起点</param>
        /// <param name="d">终点</param>
        /// <param name="pathtype">true: link-represented path/ false: node-represented path</param>
        /// <param name="Accessible">是否存在最短路径</param>
        /// <returns></returns>
        public List<int> GetShortestPath(Network WorkingNet, int o, int d, bool pathtype, out bool Accessible)
        {
            List<int> path_node = new List<int>();
            List<int> path_link = new List<int>();
            int Next = d;
            path_node.Add(Next);
            Accessible = true;
            while (Next != o)
            {
                if (WorkingNet.AllNodes[Next - 1].NextNodeID == -1)
                {
                    Accessible = false;
                    break;
                }
                else
                {
                    path_link.Add(GetLinkID(WorkingNet, WorkingNet.AllNodes[Next - 1].NextNodeID, Next));
                    Next = WorkingNet.AllNodes[Next - 1].NextNodeID;
                    path_node.Add(Next);
                }
            }
            if (!pathtype) return path_node;
            else return path_link;
        }
        public List<int> GetShortestPathForSubNet(Network WorkingNet, int o, int d, bool pathtype, out bool Accessible)
        {
            List<int> path_node = new List<int>();
            List<int> path_link = new List<int>();
            int Next = d;
            path_node.Add(Next);
            Accessible = true;
            while (Next != o)
            {
                if (WorkingNet.AllNodes[Next - 1].NextNodeID == -1)
                {
                    Accessible = false;
                    break;
                }
                else
                {
                    path_link.Add(GetLinkIDForSubNet(WorkingNet, WorkingNet.AllNodes[Next - 1].NextNodeID, Next));
                    Next = WorkingNet.AllNodes[Next - 1].NextNodeID;
                    path_node.Add(Next);
                }
            }
            if (!pathtype) return path_node;
            else return path_link;
        }

        /// <summary>
        /// 通过节点id得到路段id
        /// </summary>
        /// <param name="fromid">ID of from node</param>
        /// <param name="toid">ID of to node</param>
        /// <returns></returns>

        private int GetLinkID(Network WorkingNet, int fromid, int toid)
        {
            int linkid = -1;
            foreach (Link i in WorkingNet.AllNodes[fromid - 1].OutLinks)
            {
                foreach (Link j in WorkingNet.AllNodes[toid - 1].InLinks)
                {
                    if (i.ID == j.ID) linkid = i.ID;
                }
            }
            return linkid;
        }

        private int GetLinkIDForSubNet(Network WorkingNet, int fromid, int toid)
        {
            int linkid = -1;
            foreach (Link i in WorkingNet.AllNodes[fromid - 1].OutLinks)
            {
                foreach (Link j in WorkingNet.AllNodes[toid - 1].InLinks)
                {
                    if (i.ID == j.ID) linkid = i.SubID;
                }
            }
            return linkid;
        }

        /// <summary>
        /// 最短路探索结束后，将网络恢复至初始状态
        /// </summary>

        public void Dijkstra_Recover(Network WorkingNet)
        {
            //算法完成，清空HasProcessed标记
            foreach (Node i in WorkingNet.AllNodes)
            {
                i.HasProcessed = false;
                i.OptHeuristic = double.PositiveInfinity;
                i.PessHeuristic = double.PositiveInfinity;
                i.RegretModifiedHeuristic = double.PositiveInfinity;
                i.NextNodeID = -1;
            }
        }


        /// <summary>
        /// 恢复Dijkstra探索所需的条件，但保留已经产生的heuristics值
        /// </summary>


        #endregion

        #region Fibonacci堆优化的Dijkstra最短路算法
        /// <summary>
        /// 基于Fibonacci堆的Dijkstra最短路算法,算法到达终点即停止，不保证每个节点都获得其Heuristics
        /// </summary>
        /// <param name="o">起点</param>
        /// <param name="d">终点</param>
        /// <param name="prior">true产生Optimistic Heuristics， false产生Pessimistic Heuristics</param>
        /// <param name="TimeSpan">算法消耗时间</param>
        /// <returns></returns>
        bool GoalDirectedFibDijkstra(Network WorkingNet, int o, int d, bool prior, out long TimeSpan)//这里directed为最短路计算时是否考虑道路方向的参数,piror为ture代表optimistic，false为pessimistic
        {
            bool success = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                FibonacciHeap<Node> Updated = new FibonacciHeap<Node>();//Updated node collection, add the new updated nodes and delete the permanent nodes dynamically
                List<Node> ProcessFinished = new List<Node>();//Permanent collection
                WorkingNet.AllNodes[o - 1].OptHeuristic = 0;
                WorkingNet.AllNodes[o - 1].HasProcessed = true;
                Dictionary<Node, FibonacciHeapNode<Node>> FibNodeDict = new Dictionary<Node, FibonacciHeapNode<Node>>();
                Node tempnode = WorkingNet.AllNodes[o - 1];
                Node tempnextnode;
                Updated.insert(new FibonacciHeapNode<Node>(tempnode), 0);
                while (tempnode != WorkingNet.AllNodes[d - 1])//go on the loop before every node gets into the permanent collection
                {
                    if (Updated.isEmpty()) break;//如果所有的点都已经获得P标号（剩下的没有获得P标号的是不可达点），主要用防止在单向网络中可能发生的错误
                    tempnode = Updated.removeMin().getData();
                    for (int j = 0; j < tempnode.OutLinks.Count; j++)
                    {
                        //******************************************************************错误就在于这里，不一定是ToID，也有可能更新FromID
                        //tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].ToID - 1];
                        //应该是更新流出路段的另一节点，而非一定是ToID。因为在路网拓扑构建的时候就已经考虑了路段的方向性。因此此处的tonode实际上是outlink的除了当前点之外的另一节点，可能是tonode，也可能是fromnode
                        if (tempnode.GID == tempnode.OutLinks[j].ToGID)
                            tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].From.ID - 1];
                        else tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].To.ID - 1];
                        // ********************************************************************
                        if (prior == true)
                        {
                            if (tempnextnode.OptHeuristic > tempnode.OptHeuristic + tempnode.OutLinks[j].TravelTime_variable) //乐观最短路与悲观最短路的费用
                            {
                                tempnextnode.OptHeuristic = tempnode.OptHeuristic + tempnode.OutLinks[j].TravelTime_variable;
                                tempnextnode.NextNodeID = tempnode.GID;
                                if (tempnextnode.HasProcessed == false)
                                {
                                    FibonacciHeapNode<Node> FibNode;
                                    if (!FibNodeDict.ContainsKey(tempnextnode))
                                    {
                                        FibNode = new FibonacciHeapNode<Node>(tempnextnode);
                                        FibNodeDict.Add(tempnextnode, FibNode);
                                        Updated.insert(FibNode, tempnextnode.OptHeuristic);
                                    }
                                    else
                                    {
                                        FibNodeDict.TryGetValue(tempnextnode, out FibNode);
                                        Updated.decreaseKey(FibNode, tempnextnode.OptHeuristic);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (tempnextnode.PessHeuristic > tempnode.PessHeuristic + tempnode.OutLinks[j].TravelTime_variable + 1 / tempnode.OutLinks[j].Fa) //乐观最短路与悲观最短路的费用
                            {
                                tempnextnode.PessHeuristic = tempnode.PessHeuristic + tempnode.OutLinks[j].TravelTime_variable + 1 / tempnode.OutLinks[j].Fa;
                                tempnextnode.NextNodeID = tempnode.GID;
                                if (tempnextnode.HasProcessed == false)
                                {
                                    FibonacciHeapNode<Node> FibNode;
                                    if (!FibNodeDict.ContainsKey(tempnextnode))
                                    {
                                        FibNode = new FibonacciHeapNode<Node>(tempnextnode);
                                        FibNodeDict.Add(tempnextnode, FibNode);
                                        Updated.insert(FibNode, tempnextnode.PessHeuristic);
                                    }
                                    else
                                    {
                                        FibNodeDict.TryGetValue(tempnextnode, out FibNode);
                                        Updated.decreaseKey(FibNode, tempnextnode.PessHeuristic);
                                    }
                                }
                            }
                        }
                    }
                }
                success = true;
            }
            catch { success = false; }
            sw.Stop();
            TimeSpan = sw.ElapsedMilliseconds;

            return success;

        }


        /// <summary>
        /// 原始基于Fibonacci堆的Dijkstra算法，确保每个节点都获得其Heuristics
        /// </summary>
        /// <param name="o">起点</param>
        /// <param name="d">终点</param>
        /// <param name="prior">true if Optimistic heuristic is used, false if Pessimistic heuristic is used</param>
        /// <param name="TimeSpan">最短路算法消耗的时间</param>
        /// <returns></returns>

        bool FibDijkstra(Network WorkingNet, int o, int d, bool prior, out long TimeSpan)//这里directed为最短路计算时是否考虑道路方向的参数,piror为ture代表optimistic，false为pessimistic
        {
            TimeSpan = -1;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            FibonacciHeap<Node> Updated = new FibonacciHeap<Node>();//Updated node collection, add the new updated nodes and delete the permanent nodes dynamically
            List<Node> ProcessFinished = new List<Node>();//Permanent collection
            if (prior == true)
                WorkingNet.AllNodes[o - 1].OptHeuristic = 0;
            else WorkingNet.AllNodes[o - 1].PessHeuristic = 0;
            WorkingNet.AllNodes[o - 1].HasProcessed = true;
            Dictionary<Node, FibonacciHeapNode<Node>> FibNodeDict = new Dictionary<Node, FibonacciHeapNode<Node>>();
            Node tempnode = WorkingNet.AllNodes[o - 1];
            Node tempnextnode;
            Updated.insert(new FibonacciHeapNode<Node>(tempnode), 0);
            int ClosedNodes = 1;
            while (ClosedNodes != WorkingNet.AllNodes.Count)//go on the loop before every node gets into the permanent collection
            {
                if (Updated.isEmpty()) break;//如果所有的点都已经获得P标号（剩下的没有获得P标号的是不可达点），主要用防止在单向网络中可能发生的错误
                tempnode = Updated.removeMin().getData();
                tempnode.HasProcessed = true;
                ClosedNodes++;
                for (int j = 0; j < tempnode.OutLinks.Count; j++)
                {
                    //******************************************************************错误就在于这里，不一定是ToID，也有可能更新FromID
                    //tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].ToID - 1];
                    //应该是更新流出路段的另一节点，而非一定是ToID。因为在路网拓扑构建的时候就已经考虑了路段的方向性。因此此处的tonode实际上是outlink的除了当前点之外的另一节点，可能是tonode，也可能是fromnode
                    if (tempnode.GID == tempnode.OutLinks[j].ToGID)
                        tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].From.ID - 1];
                    else tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].To.ID - 1];
                    // ********************************************************************
                    if (prior == true)
                    {
                        if (tempnextnode.OptHeuristic > tempnode.OptHeuristic + tempnode.OutLinks[j].TravelTime_variable) //乐观最短路与悲观最短路的费用
                        {
                            tempnextnode.OptHeuristic = tempnode.OptHeuristic + tempnode.OutLinks[j].TravelTime_variable;
                            tempnextnode.NextNodeID = tempnode.GID;
                            if (tempnextnode.HasProcessed == false)
                            {
                                FibonacciHeapNode<Node> FibNode;
                                if (!FibNodeDict.ContainsKey(tempnextnode))
                                {
                                    FibNode = new FibonacciHeapNode<Node>(tempnextnode);
                                    FibNodeDict.Add(tempnextnode, FibNode);
                                    Updated.insert(FibNode, tempnextnode.OptHeuristic);
                                }
                                else
                                {
                                    FibNodeDict.TryGetValue(tempnextnode, out FibNode);
                                    Updated.decreaseKey(FibNode, tempnextnode.OptHeuristic);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tempnextnode.PessHeuristic > tempnode.PessHeuristic + tempnode.OutLinks[j].TravelTime_variable + 1 / tempnode.OutLinks[j].Fa) //乐观最短路与悲观最短路的费用
                        {
                            tempnextnode.PessHeuristic = tempnode.PessHeuristic + tempnode.OutLinks[j].TravelTime_variable + 1 / tempnode.OutLinks[j].Fa;
                            tempnextnode.NextNodeID = tempnode.GID;
                            if (tempnextnode.HasProcessed == false)
                            {
                                FibonacciHeapNode<Node> FibNode;
                                if (!FibNodeDict.ContainsKey(tempnextnode))
                                {
                                    FibNode = new FibonacciHeapNode<Node>(tempnextnode);
                                    FibNodeDict.Add(tempnextnode, FibNode);
                                    Updated.insert(FibNode, tempnextnode.PessHeuristic);
                                }
                                else
                                {
                                    FibNodeDict.TryGetValue(tempnextnode, out FibNode);
                                    Updated.decreaseKey(FibNode, tempnextnode.PessHeuristic);
                                }
                            }
                        }
                    }
                }
            }



            sw.Stop();
            TimeSpan = sw.ElapsedMilliseconds;

            return true;
        }



        bool FibDijkstraBackward(Network WorkingNet, int o, int d, bool prior, out long TimeSpan)//这里directed为最短路计算时是否考虑道路方向的参数,piror为ture代表optimistic，false为pessimistic
        {
            TimeSpan = -1;


            Stopwatch sw = new Stopwatch();
            sw.Start();

            FibonacciHeap<Node> Updated = new FibonacciHeap<Node>();//Updated node collection, add the new updated nodes and delete the permanent nodes dynamically
            List<Node> ProcessFinished = new List<Node>();//Permanent collection
            if (prior == true)
                WorkingNet.AllNodes[o - 1].OptHeuristic = 0;
            else WorkingNet.AllNodes[o - 1].PessHeuristic = 0;
            WorkingNet.AllNodes[o - 1].HasProcessed = true;
            Dictionary<Node, FibonacciHeapNode<Node>> FibNodeDict = new Dictionary<Node, FibonacciHeapNode<Node>>();
            Node tempnode = WorkingNet.AllNodes[o - 1];
            Node tempnextnode;
            Updated.insert(new FibonacciHeapNode<Node>(tempnode), 0);
            int ClosedNodes = 1;
            while (ClosedNodes != WorkingNet.AllNodes.Count)//go on the loop before every node gets into the permanent collection
            {
                if (Updated.isEmpty()) break;//如果所有的点都已经获得P标号（剩下的没有获得P标号的是不可达点），主要用防止在单向网络中可能发生的错误
                tempnode = Updated.removeMin().getData();
                tempnode.HasProcessed = true;
                ClosedNodes++;
                for (int j = 0; j < tempnode.InLinks.Count; j++)
                {
                    //******************************************************************错误就在于这里，不一定是ToID，也有可能更新FromID
                    //tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].ToID - 1];
                    //应该是更新流出路段的另一节点，而非一定是ToID。因为在路网拓扑构建的时候就已经考虑了路段的方向性。因此此处的tonode实际上是outlink的除了当前点之外的另一节点，可能是tonode，也可能是fromnode
                    if (tempnode.GID == tempnode.InLinks[j].ToGID)
                        tempnextnode = WorkingNet.AllNodes[tempnode.InLinks[j].From.ID - 1];
                    else tempnextnode = WorkingNet.AllNodes[tempnode.InLinks[j].To.ID - 1];
                    // ********************************************************************
                    if (prior == true)
                    {
                        if (tempnextnode.OptHeuristic > tempnode.OptHeuristic + tempnode.InLinks[j].TravelTime_variable) //乐观最短路与悲观最短路的费用
                        {
                            tempnextnode.OptHeuristic = tempnode.OptHeuristic + tempnode.InLinks[j].TravelTime_variable;
                            tempnextnode.NextNodeID = tempnode.GID;
                            if (tempnextnode.HasProcessed == false)
                            {
                                FibonacciHeapNode<Node> FibNode;
                                if (!FibNodeDict.ContainsKey(tempnextnode))
                                {
                                    FibNode = new FibonacciHeapNode<Node>(tempnextnode);
                                    FibNodeDict.Add(tempnextnode, FibNode);
                                    Updated.insert(FibNode, tempnextnode.OptHeuristic);
                                }
                                else
                                {
                                    FibNodeDict.TryGetValue(tempnextnode, out FibNode);
                                    Updated.decreaseKey(FibNode, tempnextnode.OptHeuristic);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tempnextnode.PessHeuristic > tempnode.PessHeuristic + tempnode.InLinks[j].TravelTime_variable + 1 / tempnode.InLinks[j].Fa) //乐观最短路与悲观最短路的费用
                        {
                            tempnextnode.PessHeuristic = tempnode.PessHeuristic + tempnode.InLinks[j].TravelTime_variable + 1 / tempnode.InLinks[j].Fa;
                            tempnextnode.NextNodeID = tempnode.GID;
                            if (tempnextnode.HasProcessed == false)
                            {
                                FibonacciHeapNode<Node> FibNode;
                                if (!FibNodeDict.ContainsKey(tempnextnode))
                                {
                                    FibNode = new FibonacciHeapNode<Node>(tempnextnode);
                                    FibNodeDict.Add(tempnextnode, FibNode);
                                    Updated.insert(FibNode, tempnextnode.PessHeuristic);
                                }
                                else
                                {
                                    FibNodeDict.TryGetValue(tempnextnode, out FibNode);
                                    Updated.decreaseKey(FibNode, tempnextnode.PessHeuristic);
                                }
                            }
                        }
                    }
                }
            }



            sw.Stop();
            TimeSpan = sw.ElapsedMilliseconds;

            return true;
        }

        bool FibDijkstraBackwardForSubNet(Network WorkingNet, int o, int d, bool prior, out long TimeSpan)//这里directed为最短路计算时是否考虑道路方向的参数,piror为ture代表optimistic，false为pessimistic
        {
            TimeSpan = -1;


            Stopwatch sw = new Stopwatch();
            sw.Start();

            FibonacciHeap<Node> Updated = new FibonacciHeap<Node>();//Updated node collection, add the new updated nodes and delete the permanent nodes dynamically
            List<Node> ProcessFinished = new List<Node>();//Permanent collection
            if (prior == true)
                WorkingNet.AllNodes[o - 1].OptHeuristic = 0;
            else WorkingNet.AllNodes[o - 1].PessHeuristic = 0;
            WorkingNet.AllNodes[o - 1].HasProcessed = true;
            Dictionary<Node, FibonacciHeapNode<Node>> FibNodeDict = new Dictionary<Node, FibonacciHeapNode<Node>>();
            Node tempnode = WorkingNet.AllNodes[o - 1];
            Node tempnextnode;
            Updated.insert(new FibonacciHeapNode<Node>(tempnode), 0);
            int ClosedNodes = 1;
            while (ClosedNodes != WorkingNet.AllNodes.Count)//go on the loop before every node gets into the permanent collection
            {
                if (Updated.isEmpty()) break;//如果所有的点都已经获得P标号（剩下的没有获得P标号的是不可达点），主要用防止在单向网络中可能发生的错误
                tempnode = Updated.removeMin().getData();
                tempnode.HasProcessed = true;
                ClosedNodes++;
                for (int j = 0; j < tempnode.InLinks.Count; j++)
                {
                    //******************************************************************错误就在于这里，不一定是ToID，也有可能更新FromID
                    //tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].ToID - 1];
                    //应该是更新流出路段的另一节点，而非一定是ToID。因为在路网拓扑构建的时候就已经考虑了路段的方向性。因此此处的tonode实际上是outlink的除了当前点之外的另一节点，可能是tonode，也可能是fromnode
                    if (tempnode.GID == tempnode.InLinks[j].ToGID)
                        tempnextnode = WorkingNet.AllNodes[tempnode.InLinks[j].From.SubID - 1];
                    else tempnextnode = WorkingNet.AllNodes[tempnode.InLinks[j].To.SubID - 1];
                    // ********************************************************************
                    if (prior == true)
                    {
                        if (tempnextnode.OptHeuristic > tempnode.OptHeuristic + tempnode.InLinks[j].TravelTime_variable) //乐观最短路与悲观最短路的费用
                        {
                            tempnextnode.OptHeuristic = tempnode.OptHeuristic + tempnode.InLinks[j].TravelTime_variable;
                            tempnextnode.NextNodeID = tempnode.SubID;
                            if (tempnextnode.HasProcessed == false)
                            {
                                FibonacciHeapNode<Node> FibNode;
                                if (!FibNodeDict.ContainsKey(tempnextnode))
                                {
                                    FibNode = new FibonacciHeapNode<Node>(tempnextnode);
                                    FibNodeDict.Add(tempnextnode, FibNode);
                                    Updated.insert(FibNode, tempnextnode.OptHeuristic);
                                }
                                else
                                {
                                    FibNodeDict.TryGetValue(tempnextnode, out FibNode);
                                    Updated.decreaseKey(FibNode, tempnextnode.OptHeuristic);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tempnextnode.PessHeuristic > tempnode.PessHeuristic + tempnode.InLinks[j].TravelTime_variable + 1 / tempnode.InLinks[j].Fa) //乐观最短路与悲观最短路的费用
                        {
                            tempnextnode.PessHeuristic = tempnode.PessHeuristic + tempnode.InLinks[j].TravelTime_variable + 1 / tempnode.InLinks[j].Fa;
                            tempnextnode.NextNodeID = tempnode.SubID;
                            if (tempnextnode.HasProcessed == false)
                            {
                                FibonacciHeapNode<Node> FibNode;
                                if (!FibNodeDict.ContainsKey(tempnextnode))
                                {
                                    FibNode = new FibonacciHeapNode<Node>(tempnextnode);
                                    FibNodeDict.Add(tempnextnode, FibNode);
                                    Updated.insert(FibNode, tempnextnode.PessHeuristic);
                                }
                                else
                                {
                                    FibNodeDict.TryGetValue(tempnextnode, out FibNode);
                                    Updated.decreaseKey(FibNode, tempnextnode.PessHeuristic);
                                }
                            }
                        }
                    }
                }
            }



            sw.Stop();
            TimeSpan = sw.ElapsedMilliseconds;

            return true;
        }

        private void ApplyBackwardPenalty(Network WorkingNet, int o, int d, double BackwardPenalty)
        {
            foreach (Link i in WorkingNet.AllLinks)
            {
                //如果是Backward Link，则对该路段增加BackwardPenalty
                if (JudgeBackward(WorkingNet.AllNodes[d - 1], i))
                {
                    i.TravelTime_variable += BackwardPenalty * i.TravelTime_Fixed;
                }
            }
        }
        private bool JudgeBackward(Node D, Link a)
        {
            //路段末端距离终点的欧几里德距离更远
            if (GetDistance(a.From, D) < GetDistance(a.To, D))
            { return true; }
            else return false;
        }
        private double GetDistance(Node A, Node B)
        {
            return Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2));
        }

        //重设分配Heuristics的初始条件，保留已经产生的Heuristics，用于生成RSA算法中的optimistic和pessimistic heuristics
        public void Dijkstra_RecoverForRSA(Network WorkingNet)
        {
            //算法完成，清空HasProcessed标记
            foreach (Node i in WorkingNet.AllNodes)
            {
                i.HasProcessed = false;
                i.NextNodeID = -1;
            }
        }
        #endregion

        #region 多路径算法

        #region DHS算法
        /// <summary>
        /// DHS 算法，以无堆优化的Dijkstra最短路算法为基础，且DHS算法本身也不经过堆优化
        /// </summary>
        /// <param name="o"></param>
        /// <param name="d"></param>
        /// <param name="TimeSpan_SP"></param>
        /// <param name="TimeSpan_DHS"></param>
        /// <returns></returns>
        bool DHS(Network WorkingNet, int o, int d, ref List<Link> _RawHyperpath, out long TimeSpan_SP, out long TimeSpan_DHS)
        {
            //路段比较器，UiaddCa越大的越小，所以Heap里面的max其实是UiaddCa最小的路段
            DefinedComparison Compare = new DefinedComparison();

            Stopwatch stw = new Stopwatch();
            bool success = false;
            long timeSP = 0;
            stw.Start();
            try
            {
                //先用Dijkstra算法初始化各节点的Heuristic
                //Dijkstra(WorkingNet, o, out timeSP);
                FibDijkstra(WorkingNet, o, d, true, out timeSP);
                #region Initialization
                double beta = 0.0;
                Node tempi = WorkingNet.AllNodes[d - 1], tempj;
                Link tempa = new Link();
                bool status = true;
                List<Node> updatednodes = new List<Node>();
                //用比较器初始化堆，注意这个比较器是逆序比较器，UiaddCa越小的被堆认为是越大的link，主要是为了避免重复写代码（后面对全路段集排序正好也要用）
                List<Link> updatedlinks = new List<Link>();
                updatednodes.Add(WorkingNet.AllNodes[d - 1]);
                WorkingNet.AllNodes[d - 1].Ui = 0;
                WorkingNet.AllNodes[o - 1].Yi = 1.0;
                #endregion

                #region Updating
                while (status)
                {

                    foreach (Link j in tempi.InLinks)
                    {
                        Node tempnodej = null;
                        tempnodej = WorkingNet.AllNodes[j.From.ID - 1];
                        if (j.UiAddCa > tempi.Ui + j.TravelTime_variable + tempnodej.OptHeuristic)
                        {
                            j.UiAddCa = tempi.Ui + j.TravelTime_variable + tempnodej.OptHeuristic;
                            if (j.Hasbeenremoved != true && updatedlinks.Contains(j) == false)
                                if (j.InPathsCollection == false && j.HasUpdated == false)
                                {
                                    updatedlinks.Add(j);
                                    j.HasUpdated = true;
                                }
                        }
                    }
                    double temp = double.PositiveInfinity;
                    foreach (Link i in updatedlinks)
                    {
                        if (i.UiAddCa < temp)
                        {
                            temp = i.UiAddCa;
                            tempa = i;
                        }
                    }

                    tempi = WorkingNet.AllNodes[tempa.From.ID - 1];
                    tempj = WorkingNet.AllNodes[tempa.To.ID - 1];
                    updatedlinks.Remove(tempa);
                    tempa.Hasbeenremoved = true;


                    //update node i

                    if (tempi.Ui >= tempj.Ui + tempa.TravelTime_variable)
                    {
                        if ((tempi.Ui >= double.PositiveInfinity) && tempi.Fi == 0)
                        {
                            beta = 1.0;
                        }
                        else
                        {
                            beta = tempi.Ui * tempi.Fi;
                        }
                        tempi.Ui = (beta + tempa.Fa * (tempj.Ui + tempa.TravelTime_variable)) / (tempi.Fi + tempa.Fa);

                        updatednodes.Add(tempi);

                        tempi.Fi += tempa.Fa;

                        _RawHyperpath.Add(tempa);

                        tempa.InPathsCollection = true;
                    }

                    if (tempj.Ui + tempa.TravelTime_variable + tempi.OptHeuristic > WorkingNet.AllNodes[o - 1].Ui || updatednodes.Count == 0)
                    {
                        status = false;
                    }
                }

                #endregion

                #region Loading
                //Loading step

                WorkingNet.AllLinks.Sort(Compare);

                foreach (Link i in WorkingNet.AllLinks)
                {
                    if (i.InPathsCollection)
                    {
                        i.Pa = (i.Fa / WorkingNet.AllNodes[i.From.ID - 1].Fi) * WorkingNet.AllNodes[i.From.ID - 1].Yi;

                        WorkingNet.AllNodes[i.To.ID - 1].Yi += i.Pa;
                    }

                }
                #endregion

                success = true;
            }
            catch { success = false; }
            stw.Stop();
            TimeSpan_DHS = (int)stw.ElapsedMilliseconds;
            TimeSpan_SP = timeSP;
            return success;
        }
        //定义路段排序准则
        #endregion

        #region HP Seies算法比较

        bool NDHP(Network WorkingNet, int o, int d, ref List<Link> _RawHyperpath, out long TimeSpan_HP)
        {
            Stopwatch stw = new Stopwatch();
            bool success = false;

            stw.Start();
            try
            {
                #region Initialization
                double beta = 0.0;
                Node tempi = WorkingNet.AllNodes[d - 1], tempj;
                Link tempa = new Link();
                bool status = true;
                List<Node> updatednodes = new List<Node>();
                List<Link> updatedlinks = new List<Link>();
                updatednodes.Add(WorkingNet.AllNodes[d - 1]);
                WorkingNet.AllNodes[d - 1].Ui = 0;
                WorkingNet.AllNodes[o - 1].Yi = 1.0;

                #endregion

                #region Updating
                while (status)
                {
                    foreach (Link j in tempi.InLinks)
                    {
                        Node tempnodej = WorkingNet.AllNodes[j.From.ID - 1];
                        if (j.UiAddCa > tempi.Ui + j.TravelTime_variable)
                        {
                            j.UiAddCa = tempi.Ui + j.TravelTime_variable;
                            if (j.Hasbeenremoved != true && j.HasUpdated == false)
                            {
                                updatedlinks.Add(j);
                                j.HasUpdated = true;
                            }
                        }
                    }
                    double temp = double.PositiveInfinity;
                    Link templink = new Link();

                    foreach (Link i in updatedlinks)
                    {
                        if (i.UiAddCa < temp)
                        {
                            temp = i.UiAddCa;
                            templink = i;
                        }
                    }
                    tempa = templink;
                    tempi = WorkingNet.AllNodes[tempa.From.ID - 1];
                    tempj = WorkingNet.AllNodes[tempa.To.ID - 1];
                    updatedlinks.Remove(templink);
                    templink.Hasbeenremoved = true;



                    if (tempi.Ui >= tempj.Ui + tempa.TravelTime_variable)
                    {
                        if ((tempi.Ui >= double.PositiveInfinity) && tempi.Fi == 0)
                        {
                            beta = 1.0;
                        }
                        else
                        {
                            beta = tempi.Ui * tempi.Fi;
                        }

                        tempi.Ui = (beta + tempa.Fa * (tempj.Ui + tempa.TravelTime_variable)) / (tempi.Fi + tempa.Fa);
                        tempi.Fi += tempa.Fa;
                        _RawHyperpath.Add(tempa);
                        tempa.InPathsCollection = true;
                    }
                    if (tempj.Ui + tempa.TravelTime_variable > WorkingNet.AllNodes[o - 1].Ui)
                    {
                        status = false;
                    }
                }
                #endregion

                #region Loading
                //Loading step
                DefinedComparison Compare = new DefinedComparison();
                WorkingNet.AllLinks.Sort(Compare);

                foreach (Link i in WorkingNet.AllLinks)
                {
                    if (i.InPathsCollection)
                    {
                        i.Pa = (i.Fa / WorkingNet.AllNodes[i.From.ID - 1].Fi) * WorkingNet.AllNodes[i.From.ID - 1].Yi;

                        WorkingNet.AllNodes[i.To.ID - 1].Yi += i.Pa;
                    }

                }

                #endregion

                stw.Stop();
                success = true;
            }
            catch { success = false; }
            TimeSpan_HP = stw.ElapsedMilliseconds;
            return success;
        }

        bool HP(Network WorkingNet, int o, int d, ref List<Link> _RawHyperpath, out long TimeSpan_HP)
        {
            Stopwatch stw = new Stopwatch();
            bool success = false;
            _RawHyperpath = new List<Link>();
            stw.Start();
            try
            {
                #region Initialization
                double beta = 0.0;
                Node tempi = WorkingNet.AllNodes[d - 1], tempj;
                Link tempa = new Link();
                bool status = true;
                List<Node> updatednodes = new List<Node>();
                List<Link> updatedlinks = new List<Link>(WorkingNet.AllLinks.ToArray());
                updatednodes.Add(WorkingNet.AllNodes[d - 1]);
                WorkingNet.AllNodes[d - 1].Ui = 0;
                WorkingNet.AllNodes[o - 1].Yi = 1.0;
                #endregion
#if RecordScanned
                StreamWriter tmpsw = new StreamWriter("..\\..\\ScannedLinks.txt");
#endif
                #region Updating
                while (status)
                {
                    tempa = new Link();
                    double Minui = double.PositiveInfinity;
                    //select link a
                    foreach (Link i in updatedlinks)
                    {
                        if (i.UiAddCa > WorkingNet.AllNodes[i.To.ID - 1].Ui + i.TravelTime_variable)
                            i.UiAddCa = WorkingNet.AllNodes[i.To.ID - 1].Ui + i.TravelTime_variable;

                        if (Minui >= i.UiAddCa)
                        {
                            Minui = i.UiAddCa;
                            tempa = i;
                        }
                    }
#if RecordScanned
                    tmpsw.WriteLine(tempa.GID);
#endif
                    updatedlinks.Remove(tempa);
                    Minui = double.PositiveInfinity;
                    tempi = WorkingNet.AllNodes[tempa.From.ID - 1];
                    tempj = WorkingNet.AllNodes[tempa.To.ID - 1];
                    YPoints_Ui.Add(tempj.Ui);
                    YPoints_UiAddCa.Add(tempa.UiAddCa);

                    //update node i
                    if (tempi.Ui >= tempj.Ui + tempa.TravelTime_variable)
                    {
                        if ((tempi.Ui >= double.PositiveInfinity) && tempi.Fi == 0)
                        {
                            beta = 1.0;
                        }
                        else
                        {
                            beta = tempi.Ui * tempi.Fi;
                        }
                        tempi.Ui = (beta + tempa.Fa * (tempj.Ui + tempa.TravelTime_variable)) / (tempi.Fi + tempa.Fa);
                        tempi.Fi += tempa.Fa;

                        if (!tempa.InPathsCollection)

                        { _RawHyperpath.Add(tempa); tempa.InPathsCollection = true; }
                    }
                    if (tempj.Ui + tempa.TravelTime_variable >= WorkingNet.AllNodes[o - 1].Ui) status = false;
                }
                #endregion

                #region Loading
                //Loading step

                DefinedComparison Compare = new DefinedComparison();
                WorkingNet.AllLinks.Sort(Compare);

                foreach (Link i in WorkingNet.AllLinks)
                {
                    if (i.InPathsCollection)
                    {
                        i.Pa = (i.Fa / i.From.Fi) * i.From.Yi;

                        WorkingNet.AllNodes[i.To.ID - 1].Yi += i.Pa;
                    }

                }

                #endregion
#if RecordScanned
                tmpsw.Close();
#endif
                stw.Stop();
                success = true;
            }
            catch { success = false; TimeSpan_HP = -1; }
            TimeSpan_HP = stw.ElapsedMilliseconds;
            return success;
        }

        bool HP_heu(Network WorkingNet, int o, int d, ref List<Link> _RawHyperpath, out long TimeSpan_SP, out long TimeSpan_HP)
        {
            Stopwatch stw = new Stopwatch();
            bool success = false;
            _RawHyperpath = new List<Link>();
#if RecordScanned
            StreamWriter tmpsw = new StreamWriter("..\\..\\ScannedLinks.txt");
#endif
            stw.Start();
            TimeSpan_SP = -1;
            try
            {
                #region Initialization
                double beta = 0.0;
                Node tempi = WorkingNet.AllNodes[d - 1], tempj;
                Link tempa = new Link();
                bool status = true;

                List<Link> updatedlinks = new List<Link>(WorkingNet.AllLinks.ToArray());

                WorkingNet.AllNodes[d - 1].Ui = 0;
                WorkingNet.AllNodes[o - 1].Yi = 1.0;
                FibDijkstra(WorkingNet, o, d, true, out TimeSpan_SP);
                #endregion

                #region Updating
                while (status)
                {
                    tempa = new Link();
                    double Minui = double.PositiveInfinity;
                    //select link a
                    foreach (Link i in updatedlinks)
                    {
                        if (i.UiAddCa > WorkingNet.AllNodes[i.To.ID - 1].Ui + i.TravelTime_variable + WorkingNet.AllNodes[i.From.ID - 1].OptHeuristic)
                            i.UiAddCa = WorkingNet.AllNodes[i.To.ID - 1].Ui + i.TravelTime_variable + WorkingNet.AllNodes[i.From.ID - 1].OptHeuristic;

                        if (Minui >= i.UiAddCa)
                        {
                            Minui = i.UiAddCa;
                            tempa = i;
                        }
                    }
#if RecordScanned
                    tmpsw.WriteLine(tempa.GID);
#endif
                    updatedlinks.Remove(tempa);
                    Minui = double.PositiveInfinity;
                    tempi = WorkingNet.AllNodes[tempa.From.ID - 1];
                    tempj = WorkingNet.AllNodes[tempa.To.ID - 1];
                    YPoints_Ui.Add(tempj.Ui);
                    YPoints_UiAddCa.Add(tempa.UiAddCa);

                    //update node i
                    if (tempi.Ui >= tempj.Ui + tempa.TravelTime_variable)
                    {
                        if ((tempi.Ui >= double.PositiveInfinity) && tempi.Fi == 0)
                        {
                            beta = 1.0;
                        }
                        else
                        {
                            beta = tempi.Ui * tempi.Fi;
                        }
                        tempi.Ui = (beta + tempa.Fa * (tempj.Ui + tempa.TravelTime_variable)) / (tempi.Fi + tempa.Fa);
                        tempi.Fi += tempa.Fa;

                        if (!tempa.InPathsCollection)

                        { _RawHyperpath.Add(tempa); tempa.InPathsCollection = true; }
                    }
                    if (tempj.Ui + tempa.TravelTime_variable + tempi.OptHeuristic >= WorkingNet.AllNodes[o - 1].Ui) status = false;
                }
                #endregion

                #region Loading
                //Loading step

                DefinedComparison Compare = new DefinedComparison();
                WorkingNet.AllLinks.Sort(Compare);

                foreach (Link i in WorkingNet.AllLinks)
                {
                    if (i.InPathsCollection)
                    {
                        i.Pa = (i.Fa / i.From.Fi) * i.From.Yi;

                        WorkingNet.AllNodes[i.To.ID - 1].Yi += i.Pa;
                    }

                }

                #endregion
#if RecordScanned
                tmpsw.Close();
#endif
                stw.Stop();
                success = true;
            }
            catch { success = false; TimeSpan_HP = -1; }
            TimeSpan_HP = stw.ElapsedMilliseconds;
            return success;
        }

        bool HP_twist(Network WorkingNet, int o, int d, ref List<Link> _RawHyperpath, out long TimeSpan_DHS)
        {
            //路段比较器，UiaddCa越大的越小，所以Heap里面的max其实是UiaddCa最小的路段
            DefinedComparison Compare = new DefinedComparison();

            Stopwatch stw = new Stopwatch();
            bool success = false;
#if RecordScanned
            StreamWriter tmpsw = new StreamWriter("..\\..\\ScannedLinks.txt");
#endif
            stw.Start();
            try
            {
                //先用Dijkstra算法初始化各节点的Heuristic
                //Dijkstra(WorkingNet, o, out timeSP);
                //FibDijkstra(WorkingNet, o, d, true, out timeSP);
                #region Initialization
                double beta = 0.0;
                Node tempi = WorkingNet.AllNodes[d - 1], tempj;
                Link tempa = new Link();
                bool status = true;
                List<Node> updatednodes = new List<Node>();
                //用比较器初始化堆，注意这个比较器是逆序比较器，UiaddCa越小的被堆认为是越大的link，主要是为了避免重复写代码（后面对全路段集排序正好也要用）
                List<Link> updatedlinks = new List<Link>();
                updatednodes.Add(WorkingNet.AllNodes[d - 1]);
                WorkingNet.AllNodes[d - 1].Ui = 0;
                WorkingNet.AllNodes[o - 1].Yi = 1.0;
                #endregion

                #region Updating
                while (status)
                {

                    foreach (Link j in tempi.InLinks)
                    {
                        Node tempnodej = null;
                        tempnodej = WorkingNet.AllNodes[j.From.ID - 1];
                        if (j.UiAddCa > tempi.Ui + j.TravelTime_variable)
                        {
                            j.UiAddCa = tempi.Ui + j.TravelTime_variable;
                            if (j.Hasbeenremoved != true && updatedlinks.Contains(j) == false)
                                if (j.InPathsCollection == false && j.HasUpdated == false)
                                {
                                    updatedlinks.Add(j);
                                    j.HasUpdated = true;
                                }
                        }
                    }
                    double temp = double.PositiveInfinity;
                    foreach (Link i in updatedlinks)
                    {
                        if (i.UiAddCa < temp)
                        {
                            temp = i.UiAddCa;
                            tempa = i;
                        }
                    }
#if RecordScanned
                    tmpsw.WriteLine(tempa.GID);
#endif
                    tempi = WorkingNet.AllNodes[tempa.From.ID - 1];
                    tempj = WorkingNet.AllNodes[tempa.To.ID - 1];
                    updatedlinks.Remove(tempa);
                    tempa.Hasbeenremoved = true;


                    //update node i

                    if (tempi.Ui >= tempj.Ui + tempa.TravelTime_variable)
                    {
                        if ((tempi.Ui >= double.PositiveInfinity) && tempi.Fi == 0)
                        {
                            beta = 1.0;
                        }
                        else
                        {
                            beta = tempi.Ui * tempi.Fi;
                        }
                        tempi.Ui = (beta + tempa.Fa * (tempj.Ui + tempa.TravelTime_variable)) / (tempi.Fi + tempa.Fa);

                        updatednodes.Add(tempi);

                        tempi.Fi += tempa.Fa;

                        _RawHyperpath.Add(tempa);

                        tempa.InPathsCollection = true;
                    }

                    if (tempj.Ui + tempa.TravelTime_variable > WorkingNet.AllNodes[o - 1].Ui || updatednodes.Count == 0)
                    {
                        status = false;
                    }
                }

                #endregion

                #region Loading
                //Loading step

                WorkingNet.AllLinks.Sort(Compare);

                foreach (Link i in WorkingNet.AllLinks)
                {
                    if (i.InPathsCollection)
                    {
                        i.Pa = (i.Fa / WorkingNet.AllNodes[i.From.ID - 1].Fi) * WorkingNet.AllNodes[i.From.ID - 1].Yi;

                        WorkingNet.AllNodes[i.To.ID - 1].Yi += i.Pa;
                    }

                }
                #endregion

                success = true;
            }
            catch { success = false; }
#if RecordScanned
            tmpsw.Close();
#endif
            stw.Stop();
            TimeSpan_DHS = (int)stw.ElapsedMilliseconds;

            return success;
        }

        #endregion

        #region Fibonacci堆优化的DHS算法,以Fibnacci堆优化的Dijkstra算法作为最短路算法

        bool FDHS(Network WorkingNet, int o, int d, ref List<Link> _RawHyperpath, out long TimeSpan_SP, out long TimeSpan_FDHS)
        {
            //路段比较器，UiaddCa越大的越小
            DefinedComparison Compare = new DefinedComparison();
            Stopwatch stw = new Stopwatch();
            stw.Start();
            bool success = false;
            long timeSP = 0;
            bool Accessible;//节点之间是否可达
            try
            {
                #region Initialization
                //GoalDirectedFibDijkstra(o, d, true, out timeSP);
                FibDijkstra(WorkingNet, o, d, true, out timeSP);
                GetShortestPath(WorkingNet, o, d, true, out Accessible);
                if (!Accessible)//如果节点之间不可达，则不去探索Hyperpath
                {
                    TimeSpan_SP = timeSP;
                    TimeSpan_FDHS = -1;
                    return false;
                }
                double beta = 0.0;
                FibonacciHeap<Link> updatedlinks = new FibonacciHeap<Link>();
                List<Link> ls = new List<Link>();
                WorkingNet.AllNodes[d - 1].Ui = 0;
                WorkingNet.AllNodes[o - 1].Yi = 1.0;
                Link tempa = new Link();
                Node tempi = WorkingNet.AllNodes[d - 1];

                NodeDirect_animation.Add(tempi.GID);

                Dictionary<Link, FibonacciHeapNode<Link>> FibNodeDict = new Dictionary<Link, FibonacciHeapNode<Link>>();
                #endregion
#if RecordScanned
                StreamWriter tmpsw = new StreamWriter("..\\..\\ScannedNodes.txt");
                StreamWriter tmpsw2 = new StreamWriter("..\\..\\ScannedLinks.txt");
                tmpsw.WriteLine(d);
#endif
                #region Updating
                while (true)
                {
                    foreach (Link j in tempi.InLinks)
                    {
                        if (j.From != tempi)
                        {
                            double tempdouble = tempi.Ui + j.TravelTime_variable + WorkingNet.AllNodes[j.From.ID - 1].OptHeuristic;
                            if (j.UiAddCa > tempdouble)
                            {
                                j.UiAddCa = tempdouble;

                                if (!(j.HasUpdated))
                                {
                                    FibonacciHeapNode<Link> FibNode = new FibonacciHeapNode<Link>(j);
                                    FibNodeDict.Add(j, FibNode);
                                    updatedlinks.insert(FibNode, j.UiAddCa);
                                    j.HasUpdated = true;
                                }
                                else
                                {
                                    updatedlinks.decreaseKey(FibNodeDict[j], tempdouble);
                                }
                            }
                        }
                    }
                    tempa = updatedlinks.removeMin().getData();
                    tempi = WorkingNet.AllNodes[tempa.From.ID - 1];

                    //write the id of scanned nodes to file so that they can be displayed
#if RecordScanned
                    tmpsw.WriteLine(tempi.GID);
                    tmpsw2.WriteLine(tempa.GID);
#endif
                    SelectedLink_animation.Add(tempa.GID);
                    NodeDirect_animation.Add(tempi.GID);

                    YPoints_UiAddCa.Add(tempa.UiAddCa);



                    Node tempj = WorkingNet.AllNodes[tempa.To.ID - 1];
                    YPoints_Ui.Add(tempj.Ui);
                    //update node i
                    if (tempi.Ui >= tempj.Ui + tempa.TravelTime_variable)
                    {
                        if ((tempi.Ui >= double.PositiveInfinity) && tempi.Fi == 0)
                        {
                            beta = 1.0;
                        }
                        else
                        {
                            beta = tempi.Ui * tempi.Fi;
                        }
                        tempi.Ui = (beta + tempa.Fa * (tempj.Ui + tempa.TravelTime_variable)) / (tempi.Fi + tempa.Fa);

                        tempi.Fi += tempa.Fa;
                        _RawHyperpath.Add(tempa);
                        tempa.InPathsCollection = true;
                    }
                    if (tempj.Ui + tempa.TravelTime_variable + tempi.OptHeuristic > WorkingNet.AllNodes[o - 1].Ui)
                        break;

                }

                #endregion

                #region Loading
                //Loading step

                WorkingNet.AllLinks.Sort(Compare);
                foreach (Link i in WorkingNet.AllLinks)
                {
                    if (i.InPathsCollection)
                    {
                        i.Pa = (i.Fa / WorkingNet.AllNodes[i.From.ID - 1].Fi) * WorkingNet.AllNodes[i.From.ID - 1].Yi;

                        WorkingNet.AllNodes[i.To.ID - 1].Yi += i.Pa;
                    }

                }
                #endregion
#if RecordScanned
                tmpsw.Close();
                tmpsw2.Close();
#endif
                stw.Stop();
                success = true;
            }
            catch { success = false; }
            TimeSpan_SP = timeSP;
            TimeSpan_FDHS = stw.ElapsedMilliseconds;

            return success;
        }


        #endregion

        #region HPD Series算法比较

        //HPD_NB
        public bool HPD_NB(Network WorkingNet, int o, int d, ref List<Link> _RawHyperpath, out long TimeSpan_HPD)
        {

            Stopwatch stw = new Stopwatch();
#if RecordScanned
            StreamWriter tmpsw = new StreamWriter("..\\..\\ScannedLinks.txt");
#endif
            stw.Start();
            bool success = false;

            #region Initialization
            WorkingNet.AllNodes[d - 1].UiAddCa = 0;
            WorkingNet.AllNodes[d - 1].Ui = 0;//Tk=0
            var j = new Node();
            WorkingNet.AllNodes[o - 1].Yi = 1.0;

            HashSet<Node> UpdatedNodes = new HashSet<Node>();
            HashSet<Link> UpdatedLinks = new HashSet<Link>();
            UpdatedNodes.Add(WorkingNet.AllNodes[d - 1]);
            foreach (Link link in WorkingNet.AllNodes[d - 1].InLinks)
            { UpdatedLinks.Add(link); }
            #endregion


            #region Updating
            while (true)//while S != node number
            {
                //find the node with smallest label
                double minimum = double.PositiveInfinity;
                foreach (Node node in UpdatedNodes)
                {
                    if (node == WorkingNet.AllNodes[d - 1]) j = node;
                    else if (node.HasProcessed == false)//在已经更新过的但没有processed的点里面找最小
                    {
                        //if (node.OutLinks.Count == 1) node.UiAddCa = node.OutLinks[0].To.Ui + node.OutLinks[0].TravelTime_variable;
                        //else if (node.OutLinks.Count > 1)
                        //{
                            double MinCa = double.PositiveInfinity;
                            foreach (Link i in node.OutLinks)
                            {
                                if (i.To.HasProcessed)
                                {
                                    if (MinCa > i.To.Ui + i.TravelTime_variable)
                                    {
                                        MinCa = i.To.Ui + i.TravelTime_variable;
                                    }
                                }
                            }
                            node.UiAddCa = MinCa;
                        //}
                        if (minimum > node.UiAddCa)
                        {
                            minimum = node.UiAddCa;
                            j = node;
                        }
                       
                    }
                }
                j.HasProcessed = true;
                UpdatedNodes.Remove(j);
                if (j.GID == o) break;
                //Link a = WorkingNet.AllLinks[j.MinID - 1];
                foreach (Link a in j.InLinks)
                {
                    var i = WorkingNet.AllNodes[a.From.ID - 1];

                    if (i.HasProcessed == false)
                    {
                        a.UiAddCa = j.Ui + a.TravelTime_variable;//UiAddCa即为波浪线ta

                        if (i.Ui > a.UiAddCa)
                        {
                            if (i.Ui == double.PositiveInfinity && i.Fi == 0)
                                i.Ui = (1 + a.Fa * a.UiAddCa) / (i.Fi + a.Fa);
                            else
                                i.Ui = (i.Fi * i.Ui + a.Fa * a.UiAddCa) / (i.Fi + a.Fa);
                            i.Fi += a.Fa;
                            a.InPathsCollection = true;
                            _RawHyperpath.Add(a);
                        }
                    }
                    UpdatedNodes.Add(i);
                    UpdatedLinks.Add(a);
                }

                if (j.Ui > WorkingNet.AllNodes[o - 1].Ui) break;
            }
            #endregion

            #region Loading
            //loading

            DefinedComparison Compare = new DefinedComparison();
            WorkingNet.AllLinks.Sort(Compare);

            foreach (Link i in WorkingNet.AllLinks)
            {
                if (i.InPathsCollection)
                {
                    i.Pa = (i.Fa / WorkingNet.AllNodes[i.From.ID - 1].Fi) * WorkingNet.AllNodes[i.From.ID - 1].Yi;


                    WorkingNet.AllNodes[i.To.ID - 1].Yi += i.Pa;
                }
            }
            #endregion
#if RecordScanned
            tmpsw.Flush();
            tmpsw.Close();
#endif
            stw.Stop();
            success = true;



            TimeSpan_HPD = stw.ElapsedMilliseconds;
            return success;
        }

        //here is the HPD_raw algorithm
        public bool HPD_raw(Network WorkingNet, int o, int d, ref List<Link> _RawHyperpath, out long TimeSpan_HPD)
        {

            Stopwatch stw = new Stopwatch();

#if RecordScanned
            StreamWriter tmpsw = new StreamWriter("..\\..\\ScannedLinks.txt");
#endif
            stw.Start();
            bool success = false;
            try
            {
                #region Initialization
                int S_Count = 0;
                WorkingNet.AllNodes[d - 1].Ui = 0;     //Tk=0
                var j = new Node();
                WorkingNet.AllNodes[o - 1].Yi = 1.0;

                #endregion

                #region Updating
                while (S_Count != WorkingNet.AllNodes.Count)//while S != node number
                {
                    //find the node with smallest label
                    double minimum = double.PositiveInfinity;
                    foreach (Node node in WorkingNet.AllNodes)
                    {
                        if (node.HasProcessed == false)
                        {
                            if (node.Ui < minimum)
                            {
                                minimum = node.Ui;
                                j = node;
                            }
                        }
                    }
                    foreach (Link a in j.InLinks)
                    {
#if RecordScanned
                        tmpsw.WriteLine(a.GID);
#endif
                        var i = WorkingNet.AllNodes[a.From.ID - 1];
                        if (i.H == null) i.H = new List<Link>();
                        if (i.HasProcessed == false)
                        {
                            a.UiAddCa = j.Ui + a.TravelTime_variable;//UiAddCa即为波浪线ta
                            if (i.Ui > a.UiAddCa)
                            {
                                //Add(a) phase, note that a.FromNode is the node i, and a itself is a.
                                i.H.Add(a);
                                if (i.Ui == double.PositiveInfinity && i.Fi == 0)
                                    i.Ui = (1 + a.Fa * a.UiAddCa) / (i.Fi + a.Fa);
                                else
                                    i.Ui = (i.Fi * i.Ui + a.Fa * a.UiAddCa) / (i.Fi + a.Fa);
                                i.Fi += a.Fa;
                                a.InPathsCollection = true;
                            }
                        }
                        //link checking
                        for (int k = 0; k < i.H.Count; k++)
                        {
                            Link b = i.H[k];
                            if (b.UiAddCa >= i.Ui)
                            {
                                //remove(b) phase
                                i.H.Remove(b);
                                if (i.Ui == double.PositiveInfinity && i.Fi == 0) i.Ui = (1 - b.Fa * b.UiAddCa) / (i.Fi - b.Fa);
                                else i.Ui = (i.Fi * i.Ui - b.Fa * b.UiAddCa) / (i.Fi - b.Fa);
                                i.Fi -= b.Fa;
                                b.InPathsCollection = false;
                            }
                        }

                    }
                    j.HasProcessed = true;
                    S_Count++;
                }

                foreach (Link i in WorkingNet.AllLinks)
                {
                    if (i.InPathsCollection) _RawHyperpath.Add(i);
                }
                #endregion

                #region Loading
                //loading

                DefinedComparison Compare = new DefinedComparison();
                WorkingNet.AllLinks.Sort(Compare);

                foreach (Link i in WorkingNet.AllLinks)
                {
                    if (i.InPathsCollection)
                    {
                        i.Pa = (i.Fa / WorkingNet.AllNodes[i.From.ID - 1].Fi) * WorkingNet.AllNodes[i.From.ID - 1].Yi;


                        WorkingNet.AllNodes[i.To.ID - 1].Yi += i.Pa;
                    }
                }
                #endregion
#if RecordScanned
                tmpsw.Flush();//清理缓冲区，确保数据写入基础流
                tmpsw.Close();
#endif
                stw.Stop();
                success = true;
            }
            catch { success = false; }
            TimeSpan_HPD = stw.ElapsedMilliseconds;
            return success;
        }

        public bool HPD_heu(Network WorkingNet, int o, int d, ref List<Link> _RawHyperpath, out long TimeSpan_HPD)
        {

            Stopwatch stw = new Stopwatch();
#if RecordScanned
            StreamWriter tmpsw = new StreamWriter("..\\..\\ScannedLinks.txt");
#endif
            stw.Start();
            bool success = false;
            try
            {
                long temp = -1;
                FibDijkstra(WorkingNet, o, d, true, out temp);//产生Optimistic heuristics

                Dijkstra_RecoverForRSA(WorkingNet);
                #region Initialization
                //int S_Count = 0;
                WorkingNet.AllNodes[d - 1].Ui = 0;     //Tk=0
                var j = new Node();
                WorkingNet.AllNodes[o - 1].Yi = 1.0;

                #endregion

                #region Updating
                //while (S_Count != WorkingNet.AllNodes.Count)//while S != node number
                while (true)
                {
                    //find the node with smallest label
                    double minimum = double.PositiveInfinity;
                    foreach (Node node in WorkingNet.AllNodes)
                    {
                        if (node.HasProcessed == false)
                        {
                            //if (node.Ui < minimum)
                            if (node.Ui + node.OptHeuristic < minimum)
                            {
                                minimum = node.Ui + node.OptHeuristic;
                                j = node;
                            }
                        }
                    }
                    foreach (Link a in j.InLinks)
                    {
#if RecordScanned
                        tmpsw.WriteLine(a.GID);
#endif
                        var i = WorkingNet.AllNodes[a.From.ID - 1];
                        if (i.H == null) i.H = new List<Link>();
                        if (i.HasProcessed == false)
                        {
                            a.UiAddCa = j.Ui + a.TravelTime_variable;//UiAddCa即为波浪线ta
                            if (i.Ui > a.UiAddCa)
                            {
                                //Add(a) phase, note that a.FromNode is the node i, and a itself is a.
                                i.H.Add(a);
                                if (i.Ui == double.PositiveInfinity && i.Fi == 0)
                                    i.Ui = (1 + a.Fa * a.UiAddCa) / (i.Fi + a.Fa);
                                else
                                    i.Ui = (i.Fi * i.Ui + a.Fa * a.UiAddCa) / (i.Fi + a.Fa);
                                i.Fi += a.Fa;
                                a.InPathsCollection = true;
                            }
                        }
                        //link checking
                        for (int k = 0; k < i.H.Count; k++)
                        {
                            Link b = i.H[k];
                            if (b.UiAddCa >= i.Ui)
                            {
                                //remove(b) phase
                                i.H.Remove(b);
                                if (i.Ui == double.PositiveInfinity && i.Fi == 0) i.Ui = (1 - b.Fa * b.UiAddCa) / (i.Fi - b.Fa);
                                else i.Ui = (i.Fi * i.Ui - b.Fa * b.UiAddCa) / (i.Fi - b.Fa);
                                i.Fi -= b.Fa;
                                b.InPathsCollection = false;
                            }
                        }

                    }
                    j.HasProcessed = true;
                    if (j.Ui + j.OptHeuristic > WorkingNet.AllNodes[o - 1].Ui) break;
                    //S_Count++;
                }

                foreach (Link i in WorkingNet.AllLinks)
                {
                    if (i.InPathsCollection) _RawHyperpath.Add(i);
                }
                #endregion

                #region Loading
                //loading

                DefinedComparison Compare = new DefinedComparison();
                WorkingNet.AllLinks.Sort(Compare);

                foreach (Link i in WorkingNet.AllLinks)
                {
                    if (i.InPathsCollection)
                    {
                        i.Pa = (i.Fa / WorkingNet.AllNodes[i.From.ID - 1].Fi) * WorkingNet.AllNodes[i.From.ID - 1].Yi;


                        WorkingNet.AllNodes[i.To.ID - 1].Yi += i.Pa;
                    }
                }
                #endregion
#if RecordScanned
                tmpsw.Flush();
                tmpsw.Close();
#endif
                stw.Stop();
                success = true;


            }
            catch { success = false; }
            TimeSpan_HPD = stw.ElapsedMilliseconds;
            return success;
        }

        //Here includes the node to node search,twist      
        public bool HPD_twist(Network WorkingNet, int o, int d, ref List<Link> _RawHyperpath, out long TimeSpan_HPD)
        {

            Stopwatch stw = new Stopwatch();
#if RecordScanned
            StreamWriter tmpsw = new StreamWriter("..\\..\\ScannedLinks.txt");
#endif
            stw.Start();
            bool success = false;

            #region Initialization
            WorkingNet.AllNodes[d - 1].Ui = 0;     //Tk=0
            var j = new Node();
            WorkingNet.AllNodes[o - 1].Yi = 1.0;

            HashSet<Node> UpdatedNodes = new HashSet<Node>();
            UpdatedNodes.Add(WorkingNet.AllNodes[d - 1]);
            #endregion

            #region Updating
            while (true)//while S != node number
            {
                //find the node with smallest label
                double minimum = double.PositiveInfinity;
                foreach (Node node in UpdatedNodes)
                {
                    if (node.HasProcessed == false)//在已经更新过的但没有processed的点里面找最小
                    {
                        if (node.Ui < minimum)
                        {
                            minimum = node.Ui;
                            j = node;
                        }
                    }
                }
                j.HasProcessed = true;
                if (j.GID == o) break;


                foreach (Link a in j.InLinks)
                {
#if RecordScanned
                    tmpsw.WriteLine(a.GID);
#endif
                    var i = WorkingNet.AllNodes[a.From.ID - 1];
                    if (i.H == null) i.H = new List<Link>();
                    if (i.HasProcessed == false)
                    {
                        a.UiAddCa = j.Ui + a.TravelTime_variable;//UiAddCa即为波浪线ta
                        if (i.Ui > a.UiAddCa)
                        {
                            //Add(a) phase, note that a.FromNode is the node i, and a itself is a.
                            i.H.Add(a);
                            if (i.Ui == double.PositiveInfinity && i.Fi == 0)
                                i.Ui = (1 + a.Fa * a.UiAddCa) / (i.Fi + a.Fa);
                            else
                                i.Ui = (i.Fi * i.Ui + a.Fa * a.UiAddCa) / (i.Fi + a.Fa);
                            i.Fi += a.Fa;
                            a.InPathsCollection = true;
                        }
                    }
                    //link checking
                    for (int k = 0; k < i.H.Count; k++)
                    {
                        Link b = i.H[k];
                        if (b.UiAddCa >= i.Ui)
                        {
                            //remove(b) phase
                            i.H.Remove(b);
                            if (i.Ui == double.PositiveInfinity && i.Fi == 0) i.Ui = (1 - b.Fa * b.UiAddCa) / (i.Fi - b.Fa);
                            else i.Ui = (i.Fi * i.Ui - b.Fa * b.UiAddCa) / (i.Fi - b.Fa);
                            i.Fi -= b.Fa;
                            b.InPathsCollection = false;
                        }
                    }
                    UpdatedNodes.Add(i);
                }
                if (j.Ui > WorkingNet.AllNodes[o - 1].Ui) break;
            }

            foreach (Link i in WorkingNet.AllLinks)
            {
                if (i.InPathsCollection) _RawHyperpath.Add(i);
            }
            #endregion

            #region Loading
            //loading

            DefinedComparison Compare = new DefinedComparison();
            WorkingNet.AllLinks.Sort(Compare);

            foreach (Link i in WorkingNet.AllLinks)
            {
                if (i.InPathsCollection)
                {
                    i.Pa = (i.Fa / WorkingNet.AllNodes[i.From.ID - 1].Fi) * WorkingNet.AllNodes[i.From.ID - 1].Yi;


                    WorkingNet.AllNodes[i.To.ID - 1].Yi += i.Pa;
                }
            }
            #endregion
#if RecordScanned
            tmpsw.Flush();
            tmpsw.Close();
#endif
            stw.Stop();
            success = true;



            TimeSpan_HPD = stw.ElapsedMilliseconds;
            return success;
        }

        //twist and heu
        public bool HPD_twistheu(Network WorkingNet, int o, int d, ref List<Link> _RawHyperpath, out long TimeSpan_HPD)
        {
            Stopwatch stw = new Stopwatch();
#if RecordScanned
            StreamWriter tmpsw = new StreamWriter("..\\..\\ScannedLinks.txt");
#endif
            stw.Start();
            bool success = false;

            #region Initialization
            long temp = -1;
            FibDijkstra(WorkingNet, o, d, true, out temp);//产生Optimistic heuristics

            Dijkstra_RecoverForRSA(WorkingNet);
            WorkingNet.AllNodes[d - 1].Ui = 0;     //Tk=0
            var j = new Node();
            WorkingNet.AllNodes[o - 1].Yi = 1.0;

            HashSet<Node> UpdatedNodes = new HashSet<Node>();
            UpdatedNodes.Add(WorkingNet.AllNodes[d - 1]);
            #endregion

            //StreamWriter tmpsw = new StreamWriter("..\\..\\ScannedNodes.txt");
            //StreamWriter tmpsw2 = new StreamWriter("..\\..\\ScannedLinks.txt");
            #region Updating
            while (true)//while S != node number
            {
                //find the node with smallest label
                double minimum = double.PositiveInfinity;
                foreach (Node node in UpdatedNodes)
                {
                    if (node.HasProcessed == false)//在已经更新过的但没有processed的点里面找最小
                    {
                        if (node.Ui + node.OptHeuristic < minimum)
                        {
                            minimum = node.Ui + node.OptHeuristic;
                            j = node;
                        }
                    }
                }

                //if (!stop)
                //{
                //    tmpsw.WriteLine(j.GID);
                //}
                //if (j.GID == o) stop = true;

                j.HasProcessed = true;
                foreach (Link a in j.InLinks)
                {
                    //tmpsw2.WriteLine(a.GID);
#if RecordScanned
                    tmpsw.WriteLine(a.GID);
#endif
                    var i = WorkingNet.AllNodes[a.From.ID - 1];
                    if (i.H == null) i.H = new List<Link>();
                    if (i.HasProcessed == false)
                    {
                        a.UiAddCa = j.Ui + a.TravelTime_variable;//UiAddCa即为波浪线ta
                        if (i.Ui > a.UiAddCa)
                        {
                            //Add(a) phase, note that a.FromNode is the node i, and a itself is a.
                            i.H.Add(a);
                            if (i.Ui == double.PositiveInfinity && i.Fi == 0)
                                i.Ui = (1 + a.Fa * a.UiAddCa) / (i.Fi + a.Fa);
                            else
                                i.Ui = (i.Fi * i.Ui + a.Fa * a.UiAddCa) / (i.Fi + a.Fa);
                            i.Fi += a.Fa;
                            a.InPathsCollection = true;
                        }
                    }
                    //link checking
                    for (int k = 0; k < i.H.Count; k++)
                    {
                        Link b = i.H[k];
                        if (b.UiAddCa >= i.Ui)
                        {
                            //remove(b) phase
                            i.H.Remove(b);
                            if (i.Ui == double.PositiveInfinity && i.Fi == 0) i.Ui = (1 - b.Fa * b.UiAddCa) / (i.Fi - b.Fa);
                            else i.Ui = (i.Fi * i.Ui - b.Fa * b.UiAddCa) / (i.Fi - b.Fa);
                            i.Fi -= b.Fa;
                            b.InPathsCollection = false;
                        }
                    }
                    UpdatedNodes.Add(i);
                }
                if (j.Ui + j.OptHeuristic > WorkingNet.AllNodes[o - 1].Ui) break;
            }

            foreach (Link i in WorkingNet.AllLinks)
            {
                if (i.InPathsCollection) _RawHyperpath.Add(i);
            }
            #endregion
            //tmpsw.Close();
            //tmpsw2.Close();
            #region Loading
            //loading

            DefinedComparison Compare = new DefinedComparison();
            WorkingNet.AllLinks.Sort(Compare);

            foreach (Link i in WorkingNet.AllLinks)
            {
                if (i.InPathsCollection)
                {
                    i.Pa = (i.Fa / WorkingNet.AllNodes[i.From.ID - 1].Fi) * WorkingNet.AllNodes[i.From.ID - 1].Yi;
                    WorkingNet.AllNodes[i.To.ID - 1].Yi += i.Pa;
                }
            }
            #endregion
#if RecordScanned
            tmpsw.Flush();
            tmpsw.Close();
#endif
            stw.Stop();
            success = true;
            TimeSpan_HPD = stw.ElapsedMilliseconds;
            return success;
        }


        //能够处理delay为0情形的HPD算法//未完成
        public bool HPD_INFI(Network WorkingNet, int o, int d, ref List<Link> _RawHyperpath, out long TimeSpan_HPD)
        {

            Stopwatch stw = new Stopwatch();
            stw.Start();
            bool success = false;
            try
            {
                #region Initialization
                int S_Count = 0;
                WorkingNet.AllNodes[d - 1].Ui = 0;     //Tk=0
                WorkingNet.AllNodes[d - 1].Ui_Infinity = 0;
                var j = new Node();
                WorkingNet.AllNodes[o - 1].Yi = 1.0;

                #endregion

                #region Updating
                while (S_Count != WorkingNet.AllNodes.Count)//while S != node number
                {
                    //find the node with smallest label
                    double minimum = double.PositiveInfinity;
                    foreach (Node node in WorkingNet.AllNodes)
                    {
                        if (node.HasProcessed == false)
                        {
                            if (Math.Min(node.Ui, node.Ui_Infinity) < minimum)
                            {
                                minimum = Math.Min(node.Ui, node.Ui_Infinity);
                                j = node;
                            }
                        }
                    }
                    foreach (Link a in j.InLinks)
                    {
                        var i = WorkingNet.AllNodes[a.From.ID - 1];
                        if (i.H == null) i.H = new List<Link>();
                        if (i.H_Infinity == null) i.H_Infinity = new List<Link>();
                        if (i.HasProcessed == false)
                        {
                            a.UiAddCa = Math.Min(j.Ui, j.Ui_Infinity) + a.TravelTime_variable;//UiAddCa即为波浪线ta
                            if (i.Ui_Infinity > a.UiAddCa && a.Fa == double.PositiveInfinity)
                            {
                                i.H_Infinity.Add(a);
                                i.Ui_Infinity = a.UiAddCa;
                                i.Fi += a.Fa;
                                a.InPathsCollection = true;
                            }
                            if (i.Ui > a.UiAddCa && a.Fa < double.PositiveInfinity)
                            {
                                //Add(a) phase, note that a.FromNode is the node i, and a itself is a.
                                i.H.Add(a);
                                if (i.Ui == double.PositiveInfinity && i.Fi == 0)
                                    i.Ui = (1 + a.Fa * a.UiAddCa) / (i.Fi + a.Fa);
                                else
                                    i.Ui = (i.Fi * i.Ui + a.Fa * a.UiAddCa) / (i.Fi + a.Fa);
                                i.Fi += a.Fa;
                                a.InPathsCollection = true;
                            }
                        }
                        //link checking
                        for (int k = 0; k < i.H.Count; k++)
                        {
                            Link b = i.H[k];
                            if (b.UiAddCa >= i.Ui)
                            {
                                //remove(b) phase
                                i.H.Remove(b);
                                i.Ui = (i.Fi * i.Ui - b.Fa * b.UiAddCa) / (i.Fi - b.Fa);
                                i.Fi -= b.Fa;
                                b.InPathsCollection = false;
                            }
                        }

                    }
                    j.HasProcessed = true;
                    S_Count++;
                }

                foreach (Link i in WorkingNet.AllLinks)
                {
                    if (i.InPathsCollection) _RawHyperpath.Add(i);
                }
                #endregion

                #region Loading
                //loading

                DefinedComparison Compare = new DefinedComparison();
                WorkingNet.AllLinks.Sort(Compare);

                foreach (Link i in WorkingNet.AllLinks)
                {
                    if (i.InPathsCollection)
                    {
                        if (i.Fa != double.PositiveInfinity)
                            i.Pa = (i.Fa / WorkingNet.AllNodes[i.From.ID - 1].Fi) * WorkingNet.AllNodes[i.From.ID - 1].Yi;
                        else
                            i.Pa = WorkingNet.AllNodes[i.From.ID - 1].Yi;
                        WorkingNet.AllNodes[i.To.ID - 1].Yi += i.Pa;
                    }
                }
                #endregion
                stw.Stop();
                success = true;
                TimeSpan_HPD = stw.ElapsedMilliseconds;
                return true;

            }
            catch { success = false; }
            TimeSpan_HPD = stw.ElapsedMilliseconds;
            return success;
        }

        #endregion

        /// <summary>
        /// Heuristic algorithm(Achille, Schmocher, Bell, Fukuda, Ma Jiangshan 2011)
        /// </summary>
        /// <param name="o">起点</param>
        /// <param name="d">终点</param>
        /// <param name="TimeSpan_RSA">算法时间</param>
        /// <returns></returns>
        public bool RSA(Network WorkingNet, int o, int d, out List<Link> POPathSet, out long TimeSpan_BothDij, out long TimeSpan_RSA)
        {
            Stopwatch stw = new Stopwatch();
            stw.Start();
            TimeSpan_RSA = -1;
            long TimeSpanDij1 = -1;
            long TimeSpanDij2 = -1;

            FibDijkstra(WorkingNet, d, o, true, out TimeSpanDij1);//产生Optimistic heuristics
            Dijkstra_RecoverForRSA(WorkingNet);//恢复Dijkstra探索所需的初始条件，但保留已经生成的Heuristics值
            FibDijkstra(WorkingNet, d, o, false, out TimeSpanDij2);//产生Pessimistic heuristics
            Dijkstra_RecoverForRSA(WorkingNet);//恢复Dijkstra探索所需的初始条件，但保留已经生成的Heuristics值
            TimeSpan_BothDij = TimeSpanDij1 + TimeSpanDij2;

            # region Initilization
            System.Collections.Generic.HashSet<Node> No = new System.Collections.Generic.HashSet<Node>();
            No.Add(WorkingNet.AllNodes[o - 1]);
            List<Node> Nc = new List<Node>(WorkingNet.AllNodes.ToArray());
            Nc.Remove(WorkingNet.AllNodes[d - 1]);
            //List<Link> Ac = WorkingNet.AllLinksList;
            System.Collections.Generic.HashSet<Link> Ao = new System.Collections.Generic.HashSet<Link>();
            System.Collections.Generic.HashSet<Link> Au = new System.Collections.Generic.HashSet<Link>();
            System.Collections.Generic.HashSet<Link> An = new System.Collections.Generic.HashSet<Link>();
            #endregion


            #region Link Judging
            int NcCount = 0;
            while (true)//iterate loop1 until Nc is not changed anymore 
            {
                //Loop1
                for (int I = 0; I < Nc.Count; I++)
                {

                    if (No.Contains(Nc[I]))
                    {

                        //Loop2
                        foreach (Link i in Nc[I].OutLinks)
                        {
                            int Counter = 0;
                            //Loop3
                            foreach (Link j in Nc[I].OutLinks)
                            {
                                if (j != i)
                                {
                                    if (i.TravelTime_variable + WorkingNet.AllNodes[i.To.ID - 1].OptHeuristic < j.TravelTime_variable + 1 / j.Fa + WorkingNet.AllNodes[j.To.ID - 1].PessHeuristic)  //Equation 10 in the paper
                                    {
                                        double var1 = j.TravelTime_variable + 1 / j.Fa + WorkingNet.AllNodes[j.To.ID - 1].OptHeuristic - WorkingNet.AllNodes[i.To.ID - 1].OptHeuristic;
                                        double var2 = j.TravelTime_variable + 1 / j.Fa + WorkingNet.AllNodes[j.To.ID - 1].PessHeuristic - WorkingNet.AllNodes[i.To.ID - 1].PessHeuristic;
                                        double HTij = Math.Max(var1, var2); //Equation 12
                                        if (i.TravelTime_variable > HTij)// if Equation 12 is not fulfilled
                                        {
                                            Au.Add(i);
                                            break;//iterate loop2
                                        }
                                        else
                                        {
                                            Counter++;
                                            continue;
                                        }//iterate loop3
                                    }
                                    else
                                    {
                                        An.Add(i);
                                        break;//iterate loop2
                                    }
                                }
                            }
                            if (Counter == Nc[I].OutLinks.Count - 1)
                            {
                                Ao.Add(i);
                                No.Add(WorkingNet.AllNodes[i.To.ID - 1]);
                            }
                        }

                        Nc.Remove(Nc[I]);

                    }

                }

                if (Nc.Count == NcCount) break;
                NcCount = Nc.Count;

            }
            stw.Stop();
            double time = stw.ElapsedMilliseconds;
            #endregion

            POPathSet = new List<Link>(Ao.ToArray());

            stw.Stop();


            TimeSpan_RSA = stw.ElapsedMilliseconds;
            if (POPathSet.Count != 0)
                return true;
            else return false;
        }

        internal class DefinedComparison : IComparer<Link>
        {
            public int Compare(Link x, Link y)
            {
                if (x.UiAddCa == y.UiAddCa) return 0;
                else if (x.UiAddCa > y.UiAddCa) return -1;
                else return 1;
            }
        }

        internal class DefinedComparison2 : IComparer<Link>
        {
            public int Compare(Link x, Link y)
            {
                if (x.From.Ui + x.TravelTime_variable == y.From.Ui + x.TravelTime_variable) return 0;
                else if (x.From.Ui + x.TravelTime_variable > y.From.Ui + y.TravelTime_variable) return -1;
                else return 1;
            }
        }

        //得到考虑选择概率后的Hyperpath路段集
        private List<Link> GetHyperpath(List<Link> Input)
        {
            List<Link> Output = new List<Link>();
            foreach (Link i in Input)
            {
                if (i.Pa > 0) Output.Add(i);
            }
            return Output;

        }

        //将进行DHS路径探索后的网络恢复到初始状态
        public void DHS_Recover(Network WorkingNet, List<Link> _FinalPathSet)
        {

            if (_FinalPathSet != null)
                _FinalPathSet.Clear();
            foreach (Node i in WorkingNet.AllNodes)
            {
                i.Ui = double.PositiveInfinity;
                i.HasProcessed = false;
                i.Fi = 0;
                i.Yi = 0;
                i.NextNodeID = -1;
                i.H = null;
                i.H_Infinity = null;
                i.OptHeuristic = double.PositiveInfinity;
                i.PessHeuristic = double.PositiveInfinity;
            }
            foreach (Link i in WorkingNet.AllLinks)
            {
                i.Pa = 0;
                i.UiAddCa = double.PositiveInfinity;
                i.Hasbeenremoved = false;
                i.HasUpdated = false;
                i.InPathsCollection = false;
                i.UiAddCa_Infinity = double.PositiveInfinity;
                //i.TravelTime_variable = i.TravelTime_Fixed;
                i.Regret = double.PositiveInfinity;
                i.Cost = 0;
                i.TravelTime_variable = i.TravelTime_Fixed;
            }
        }

        //导入新的路网时，消除已有路网
        public void Network_Dispose(Network WorkingNet)
        {
            if (WorkingNet != null)
            {
                WorkingNet.Dispose();
                WorkingNet = null;
            }

        }

        //保存Hyperpath到文件
        public bool SaveHyperpath(string filepath, List<Link> _FinalPathSet, long SPTime, long DHSTime)
        {
            try
            {
                StreamWriter sw = new StreamWriter(filepath);
                sw.WriteLine("id,gid,from,to,possiblity");
                foreach (Link i in _FinalPathSet)
                {
                    sw.WriteLine(i.ID + "," + i.GID + "," + i.FromGID + "," + i.ToGID + "," + i.Pa);
                }
                sw.WriteLine("SPTime" + "," + SPTime + " ms");
                sw.WriteLine("TotalTime" + "," + DHSTime + " ms");
                sw.Close();
                sw.Dispose();
                return true;
            }
            catch { return false; }
        }

        #endregion

        #region Regret-based Path Search

        //TranSci Regret-based path in PO set, loops may occur
        public bool RegretPath_TranSci(Network WorkingNet, int o, int d, List<Link> POPathSet, out List<Link> RBP, out long TimeSpan_RBP)
        {
            Stopwatch stw = new Stopwatch();
            stw.Start();
            try
            {
                Node UpdatedNode = WorkingNet.AllNodes[o - 1];
                List<Link> RegretPath = new List<Link>();

                Link ChosenLink = new Link();
                //Node Last=new Node();

                while (UpdatedNode != WorkingNet.AllNodes[d - 1])
                {
                    List<Link> PairLinks = new List<Link>();


                    //添加用于计算Regret的比较路段
                    foreach (Link linki in UpdatedNode.OutLinks)//从起点出去的路段
                    {
                        if (POPathSet.Contains(linki))
                            PairLinks.Add(linki);
                    }
                    if (PairLinks.Count == 1)
                    {
                        ChosenLink = PairLinks[0];
                    }

                    else if (PairLinks.Count >= 2)
                    {
                        List<double> mius = new List<double>();
                        List<double> Ms = new List<double>();
                        for (int j = 0; j < PairLinks.Count; j++)
                        {
                            for (int k = 0; k < PairLinks.Count; k++)
                            {
                                if (j != k)
                                {
                                    //mius.Add(Calculate_miu(PairLinks[j], PairLinks[k],Last));
                                    mius.Add(Calculate_miu(WorkingNet, PairLinks[j], PairLinks[k]));
                                }
                            }
                        }
                        //计算各路段的M值
                        double SumMiu = 0;
                        for (int i = 1; i <= mius.Count; i++)
                        {
                            SumMiu += mius[i - 1];
                            //因为每两个路段产生一个miu值
                            if (i % (PairLinks.Count - 1) == 0)
                            {
                                Ms.Add((1.0 / PairLinks.Count) / (1.0 - 1.0 / PairLinks.Count) * SumMiu);
                                SumMiu = 0;
                            }
                        }
                        //选取M值最小的路段
                        double minM = Ms.Min();
                        for (int i = 0; i < PairLinks.Count; i++)
                        {
                            if (Ms[i] == minM) ChosenLink = PairLinks[i];
                        }
                    }

                    //Last = UpdatedNode;
                    UpdatedNode = WorkingNet.AllNodes[ChosenLink.To.ID - 1];
                    RegretPath.Add(ChosenLink);
                    if (RegretPath.Count > WorkingNet.AllLinks.Count)
                    {
                        MessageBox.Show(this, "Error! Loop occurs in Regret path!");
                        break;
                    }
                }
                stw.Stop();
                TimeSpan_RBP = stw.ElapsedMilliseconds;
                RBP = RegretPath;
                return true;
            }
            catch { TimeSpan_RBP = -1; RBP = null; return false; throw new Exception(); }
        }

        //TRB Regret-based path in PO set????(take PO set as a subnetwork or do the global search to the whole network), global search
        //t(a)+R(a) as cost and calculate the shortest path
        //far_sighted(narrow mind?????) 指示如何计算Regret
        //如果是far_sighted，则M为Regret，如果是short_sighted,则

        public void AssignRegretForTRBGlobal(Network WorkingNet, int o, int d, bool far_sighted, out long TimeSpan_AssignRegrets)
        {
            Stopwatch stw = new Stopwatch();
            stw.Start();
            long TimeSpanDij1 = -1;
            long TimeSpanDij2 = -1;
            //分配乐观和悲观heuristics
            FibDijkstraBackward(WorkingNet, d, o, true, out TimeSpanDij1);//产生Optimistic heuristics
            Dijkstra_RecoverForRSA(WorkingNet);//恢复Dijkstra探索所需的初始条件，但保留已经生成的Heuristics值
            FibDijkstraBackward(WorkingNet, d, o, false, out TimeSpanDij2);//产生Pessimistic heuristics
            //为每个节点的流出边进行Regret比较
            //shall I consider whether the outgoing link is belonged to the PO link or not????? now not considered.

            foreach (Node node in WorkingNet.AllNodes)
            {
                List<Link> PairLinks = new List<Link>();
                foreach (Link link in node.OutLinks)
                {
                    PairLinks.Add(link);
                }
                if (PairLinks.Count == 1) PairLinks[0].Regret = 0;
                else if (PairLinks.Count >= 2)
                {
                    List<double> mius = new List<double>();
                    List<double> Ms = new List<double>();
                    for (int j = 0; j < PairLinks.Count; j++)
                    {
                        for (int k = 0; k < PairLinks.Count; k++)
                        {
                            //if (j != k)
                            //{
                            //mius.Add(Calculate_miu(PairLinks[j], PairLinks[k],Last));
                            mius.Add(Calculate_miu(WorkingNet, PairLinks[j], PairLinks[k]));
                            //}
                        }
                    }
                    //计算各路段的M值
                    double SumMiu = 0;
                    for (int i = 1; i <= mius.Count; i++)
                    {
                        SumMiu += mius[i - 1];

                        /****************************************************************************/
                        //因为每n个路段产生一个miu值，之前是每n-1个路段产生一个miu值，这里不一样?????
                        if (i % (PairLinks.Count) == 0)
                        {
                            Ms.Add((1.0 / PairLinks.Count) / (1.0 - 1.0 / PairLinks.Count) * SumMiu);
                            SumMiu = 0;
                        }
                    }
                    for (int i = 1; i <= PairLinks.Count; i++)
                    {
                        if (far_sighted)
                            PairLinks[i - 1].Regret = Ms[i - 1];
                        else
                            PairLinks[i - 1].Regret = mius[(i - 1) * PairLinks.Count + i];//第xx个miu
                        /*
                         miu    i       (i-1)*3+i
                         11     0           0
                         12   
                         13
                         21
                         22     1           4
                         23
                         31
                         32
                         33     2           8
                         */
                    }
                }
                else throw new ArgumentNullException("PairLinks");
            }
            stw.Stop();
            TimeSpan_AssignRegrets = stw.ElapsedMilliseconds;
        }

        private void AssignRegretForTRBGlobalForSubNet(Network WorkingNet, int o, int d, bool far_sighted, out long TimeSpan_AssignRegrets)
        {
            Stopwatch stw = new Stopwatch();
            stw.Start();
            long TimeSpanDij1 = -1;
            long TimeSpanDij2 = -1;
            //分配乐观和悲观heuristics
            FibDijkstraBackwardForSubNet(WorkingNet, d, o, true, out TimeSpanDij1);//产生Optimistic heuristics
            Dijkstra_RecoverForRSA(WorkingNet);//恢复Dijkstra探索所需的初始条件，但保留已经生成的Heuristics值
            FibDijkstraBackwardForSubNet(WorkingNet, d, o, false, out TimeSpanDij2);//产生Pessimistic heuristics
            //为每个节点的流出边进行Regret比较
            //shall I consider whether the outgoing link is belonged to the PO link or not????? now not considered.

            foreach (Node node in WorkingNet.AllNodes)
            {
                List<Link> PairLinks = new List<Link>();
                foreach (Link link in node.OutLinks)
                {
                    PairLinks.Add(link);
                }
                if (PairLinks.Count == 1) PairLinks[0].Regret = 0;
                else if (PairLinks.Count >= 2)
                {
                    List<double> mius = new List<double>();
                    List<double> Ms = new List<double>();
                    for (int j = 0; j < PairLinks.Count; j++)
                    {
                        for (int k = 0; k < PairLinks.Count; k++)
                        {
                            //if (j != k)
                            //{
                            //mius.Add(Calculate_miu(PairLinks[j], PairLinks[k],Last));
                            mius.Add(Calculate_miuForSubNet(WorkingNet, PairLinks[j], PairLinks[k]));
                            //}
                        }
                    }
                    //计算各路段的M值
                    double SumMiu = 0;
                    for (int i = 1; i <= mius.Count; i++)
                    {
                        SumMiu += mius[i - 1];

                        /****************************************************************************/
                        //因为每n个路段产生一个miu值，之前是每n-1个路段产生一个miu值，这里不一样?????
                        if (i % (PairLinks.Count) == 0)
                        {
                            Ms.Add((1.0 / PairLinks.Count) / (1.0 - 1.0 / PairLinks.Count) * SumMiu);
                            SumMiu = 0;
                        }
                    }
                    for (int i = 1; i <= PairLinks.Count; i++)
                    {
                        if (far_sighted)
                            PairLinks[i - 1].Regret = Ms[i - 1];
                        else
                            PairLinks[i - 1].Regret = mius[(i - 1) * PairLinks.Count + i];//第xx个miu
                        /*
                         miu    i       (i-1)*3+i
                         11     0           0
                         12   
                         13
                         21
                         22     1           4
                         23
                         31
                         32
                         33     2           8
                         */
                    }
                }
                else throw new ArgumentNullException("PairLinks");
            }
            stw.Stop();
            TimeSpan_AssignRegrets = stw.ElapsedMilliseconds;
        }

        //public bool RegretPath_TRBGlobal(Network WorkingNet,int o, int d, out List<Link> RBP, out long TimeSpan_RBP)
        //{
        //    long TimeSpan_AssignRegrets=-1;
        //    AssignRegretForTRBGlobal(WorkingNet,o,d,true,out TimeSpan_AssignRegrets);
        //    Dijkstra_RecoverForRSA();
        //    //调用之前需要先调用Dijkstra_RecoverForRSA()来复原HasProceed和NextNodeID标记
        //    bool success = false;
        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();

        //    FibonacciHeap<Node> Updated = new FibonacciHeap<Node>();//Updated node collection, add the new updated nodes and delete the permanent nodes dynamically
        //    List<Node> ProcessFinished = new List<Node>();//Permanent collection
        //    WorkingNet.AllNodes[o - 1].RegretModifiedHeuristic = 0;
        //    WorkingNet.AllNodes[o - 1].HasProcessed = true;
        //    Dictionary<Node, FibonacciHeapNode<Node>> FibNodeDict = new Dictionary<Node, FibonacciHeapNode<Node>>();
        //    Node tempnode = WorkingNet.AllNodes[o - 1];
        //    Node tempnextnode;
        //    Updated.insert(new FibonacciHeapNode<Node>(tempnode), 0);
        //    while (tempnode != WorkingNet.AllNodes[d - 1])//go on the loop before every node gets into the permanent collection
        //    {
        //        if (Updated.isEmpty()) break;//如果所有的点都已经获得P标号（剩下的没有获得P标号的是不可达点），主要用防止在单向网络中可能发生的错误
        //        tempnode = Updated.removeMin().getData();
        //        for (int j = 0; j < tempnode.OutLinks.Count; j++)
        //        {
        //            //******************************************************************错误就在于这里，不一定是ToID，也有可能更新FromID
        //            //tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].ToID - 1];
        //            //应该是更新流出路段的另一节点，而非一定是ToID。因为在路网拓扑构建的时候就已经考虑了路段的方向性。因此此处的tonode实际上是outlink的除了当前点之外的另一节点，可能是tonode，也可能是fromnode
        //            if (tempnode.GID == tempnode.OutLinks[j].ToGID)
        //                tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].From.ID - 1];
        //            else tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].To.ID - 1];
        //            // ********************************************************************
        //            //TravelTime + AssignedRegret
        //            if (tempnextnode.RegretModifiedHeuristic > tempnode.RegretModifiedHeuristic + tempnode.OutLinks[j].TravelTime_variable + tempnode.OutLinks[j].Regret) //此处加入了Regret的Penalty
        //            {
        //                tempnextnode.RegretModifiedHeuristic = tempnode.RegretModifiedHeuristic + tempnode.OutLinks[j].TravelTime_variable + tempnode.OutLinks[j].Regret;
        //                tempnextnode.NextNodeID = tempnode.GID;
        //                if (tempnextnode.HasProcessed == false)
        //                {
        //                    FibonacciHeapNode<Node> FibNode;
        //                    if (!FibNodeDict.ContainsKey(tempnextnode))
        //                    {
        //                        FibNode = new FibonacciHeapNode<Node>(tempnextnode);
        //                        FibNodeDict.Add(tempnextnode, FibNode);
        //                        Updated.insert(FibNode, tempnextnode.RegretModifiedHeuristic);
        //                    }
        //                    else
        //                    {
        //                        FibNodeDict.TryGetValue(tempnextnode, out FibNode);
        //                        Updated.decreaseKey(FibNode, tempnextnode.RegretModifiedHeuristic);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    success = true;

        //    sw.Stop();
        //    TimeSpan_RBP = sw.ElapsedMilliseconds;
        //    bool Accessible=false;
        //    List<int> PathIDs = GetShortestPath(o, d, true, out Accessible);
        //    List<Link> OutRBP = new List<Link>();
        //    for (int i = 0; i < PathIDs.Count; i++)
        //    {
        //        //这里要小心，不一定对，如果AllLinks是按照ID顺序排列的才对，否则不对
        //        OutRBP.Add(WorkingNet.AllLinks[PathIDs[i]-1]);
        //    }
        //    RBP = OutRBP;
        //    return success;

        //}

        public bool RegretPath_TRBGlobal(Network WorkingNet, int o, int d, out List<Link> RBP, out long TimeSpan_RBP)
        {
            long TimeSpan_AssignRegrets = -1;
            AssignRegretForTRBGlobal(WorkingNet, o, d, true, out TimeSpan_AssignRegrets);
            Dijkstra_RecoverForRSA(WorkingNet);
            //调用之前需要先调用Dijkstra_RecoverForRSA()来复原HasProceed和NextNodeID标记
            bool success = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            FibonacciHeap<Node> Updated = new FibonacciHeap<Node>();//Updated node collection, add the new updated nodes and delete the permanent nodes dynamically
            List<Node> ProcessFinished = new List<Node>();//Permanent collection
            WorkingNet.AllNodes[o - 1].RegretModifiedHeuristic = 0;
            WorkingNet.AllNodes[o - 1].HasProcessed = true;
            Dictionary<Node, FibonacciHeapNode<Node>> FibNodeDict = new Dictionary<Node, FibonacciHeapNode<Node>>();
            Node tempnode = WorkingNet.AllNodes[o - 1];
            Node tempnextnode;
            Updated.insert(new FibonacciHeapNode<Node>(tempnode), 0);
            while (tempnode != WorkingNet.AllNodes[d - 1])//go on the loop before every node gets into the permanent collection
            {
                if (Updated.isEmpty()) break;//如果所有的点都已经获得P标号（剩下的没有获得P标号的是不可达点），主要用防止在单向网络中可能发生的错误
                tempnode = Updated.removeMin().getData();
                for (int j = 0; j < tempnode.OutLinks.Count; j++)
                {
                    //******************************************************************错误就在于这里，不一定是ToID，也有可能更新FromID
                    //tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].ToID - 1];
                    //应该是更新流出路段的另一节点，而非一定是ToID。因为在路网拓扑构建的时候就已经考虑了路段的方向性。因此此处的tonode实际上是outlink的除了当前点之外的另一节点，可能是tonode，也可能是fromnode
                    if (tempnode.GID == tempnode.OutLinks[j].ToGID)
                        tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].From.ID - 1];
                    else tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].To.ID - 1];
                    // ********************************************************************
                    //ExpectedTravelTime + AssignedRegret
                    var ExpectedTravelTime = Alpha1 * tempnode.OutLinks[j].TravelTime_variable + (1 - Alpha1) * (tempnode.OutLinks[j].TravelTime_variable + 1 / tempnode.OutLinks[j].Fa);
                    if (tempnextnode.RegretModifiedHeuristic > tempnode.RegretModifiedHeuristic + ExpectedTravelTime + tempnode.OutLinks[j].Regret) //此处加入了Regret的Penalty
                    {
                        tempnextnode.RegretModifiedHeuristic = tempnode.RegretModifiedHeuristic + ExpectedTravelTime + tempnode.OutLinks[j].Regret;
                        tempnextnode.NextNodeID = tempnode.GID;
                        if (tempnextnode.HasProcessed == false)
                        {
                            FibonacciHeapNode<Node> FibNode;
                            if (!FibNodeDict.ContainsKey(tempnextnode))
                            {
                                FibNode = new FibonacciHeapNode<Node>(tempnextnode);
                                FibNodeDict.Add(tempnextnode, FibNode);
                                Updated.insert(FibNode, tempnextnode.RegretModifiedHeuristic);
                            }
                            else
                            {
                                FibNodeDict.TryGetValue(tempnextnode, out FibNode);
                                Updated.decreaseKey(FibNode, tempnextnode.RegretModifiedHeuristic);
                            }
                        }
                    }
                }
            }

            success = true;

            sw.Stop();
            TimeSpan_RBP = sw.ElapsedMilliseconds;
            bool Accessible = false;
            List<int> PathIDs = GetShortestPath(WorkingNet, o, d, true, out Accessible);
            List<Link> OutRBP = new List<Link>();
            for (int i = 0; i < PathIDs.Count; i++)
            {
                //这里要小心，不一定对，如果AllLinks是按照ID顺序排列的才对，否则不对
                OutRBP.Add(WorkingNet.AllLinks[PathIDs[i] - 1]);
            }
            RBP = OutRBP;
            return success;

        }
        private void RecoverTravelTime(Network WorkingNet)
        {
            foreach (var i in WorkingNet.AllLinks)
            {
                i.TravelTime_variable = i.TravelTime_Fixed;
            }
        }
        public bool RegretPath_TRBGlobalForSubNet(Network WorkingNet, int o, int d, out List<Link> RBP, out long TimeSpan_RBP)
        {
            Dijkstra_RecoverForRSA(WorkingNet);
            //调用之前需要先调用Dijkstra_RecoverForRSA()来复原HasProceed和NextNodeID标记
            bool success = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            FibonacciHeap<Node> Updated = new FibonacciHeap<Node>();//Updated node collection, add the new updated nodes and delete the permanent nodes dynamically
            List<Node> ProcessFinished = new List<Node>();//Permanent collection
            WorkingNet.AllNodes[o - 1].RegretModifiedHeuristic = 0;
            WorkingNet.AllNodes[o - 1].HasProcessed = true;
            Dictionary<Node, FibonacciHeapNode<Node>> FibNodeDict = new Dictionary<Node, FibonacciHeapNode<Node>>();
            Node tempnode = WorkingNet.AllNodes[o - 1];
            Node tempnextnode;
            Updated.insert(new FibonacciHeapNode<Node>(tempnode), 0);
            while (tempnode != WorkingNet.AllNodes[d - 1])//go on the loop before every node gets into the permanent collection
            {
                if (Updated.isEmpty()) break;//如果所有的点都已经获得P标号（剩下的没有获得P标号的是不可达点），主要用防止在单向网络中可能发生的错误
                tempnode = Updated.removeMin().getData();
                for (int j = 0; j < tempnode.OutLinks.Count; j++)
                {
                    //******************************************************************错误就在于这里，不一定是ToID，也有可能更新FromID
                    //tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].ToID - 1];
                    //应该是更新流出路段的另一节点，而非一定是ToID。因为在路网拓扑构建的时候就已经考虑了路段的方向性。因此此处的tonode实际上是outlink的除了当前点之外的另一节点，可能是tonode，也可能是fromnode
                    if (tempnode.GID == tempnode.OutLinks[j].ToGID)
                        tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].From.SubID - 1];
                    else tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].To.SubID - 1];
                    // ********************************************************************
                    //ExpectedTravelTime + AssignedRegret
                    var ExpectedTravelTime = Alpha1 * tempnode.OutLinks[j].TravelTime_variable + (1 - Alpha1) * (tempnode.OutLinks[j].TravelTime_variable + 1 / tempnode.OutLinks[j].Fa);

                    if (tempnextnode.RegretModifiedHeuristic > tempnode.RegretModifiedHeuristic + ExpectedTravelTime + tempnode.OutLinks[j].Regret) //此处加入了Regret的Penalty
                    {
                        tempnextnode.RegretModifiedHeuristic = tempnode.RegretModifiedHeuristic + ExpectedTravelTime + tempnode.OutLinks[j].Regret;
                        tempnextnode.NextNodeID = tempnode.SubID;
                        if (tempnextnode.HasProcessed == false)
                        {
                            FibonacciHeapNode<Node> FibNode;
                            if (!FibNodeDict.ContainsKey(tempnextnode))
                            {
                                FibNode = new FibonacciHeapNode<Node>(tempnextnode);
                                FibNodeDict.Add(tempnextnode, FibNode);
                                Updated.insert(FibNode, tempnextnode.RegretModifiedHeuristic);
                            }
                            else
                            {
                                FibNodeDict.TryGetValue(tempnextnode, out FibNode);
                                Updated.decreaseKey(FibNode, tempnextnode.RegretModifiedHeuristic);
                            }
                        }
                    }
                }
            }

            success = true;

            sw.Stop();
            TimeSpan_RBP = sw.ElapsedMilliseconds;
            bool Accessible = false;
            List<int> PathIDs = GetShortestPathForSubNet(WorkingNet, o, d, true, out Accessible);
            List<Link> OutRBP = new List<Link>();
            for (int i = 0; i < PathIDs.Count; i++)
            {
                //这里要小心，不一定对，如果AllLinks是按照ID顺序排列的才对，否则不对
                OutRBP.Add(WorkingNet.AllLinks[PathIDs[i] - 1]);
            }
            RBP = OutRBP;
            return success;

        }
        //TRB Regret-based path in PO set, local search
        //Take the R(a),这里的R(a)与之前的TranSCi的R(a)计算略有不同，也考虑了路段与自己本身对比的regret
        //可能会产生loop
        public bool RegretPath_TRBLocal_NoPenalty(Network WorkingNet, int o, int d, List<Link> POPathSet, out List<Link> RBP, out long TimeSpan_RBP)
        {
            Stopwatch stw = new Stopwatch();
            stw.Start();
            try
            {
                Node UpdatedNode = WorkingNet.AllNodes[o - 1];
                List<Link> RegretPath = new List<Link>();

                Link ChosenLink = new Link();
                //Node Last=new Node();

                while (UpdatedNode != WorkingNet.AllNodes[d - 1])
                {
                    List<Link> PairLinks = new List<Link>();


                    //添加用于计算Regret的比较路段
                    foreach (Link linki in UpdatedNode.OutLinks)//从起点出去的路段
                    {
                        if (POPathSet.Contains(linki))
                            PairLinks.Add(linki);
                    }
                    if (PairLinks.Count == 1)
                    {
                        ChosenLink = PairLinks[0];
                    }

                    else if (PairLinks.Count >= 2)
                    {
                        List<double> mius = new List<double>();
                        List<double> Ms = new List<double>();
                        for (int j = 0; j < PairLinks.Count; j++)
                        {
                            for (int k = 0; k < PairLinks.Count; k++)
                            {
                                //if (j != k)
                                //{
                                mius.Add(Calculate_miu(WorkingNet, PairLinks[j], PairLinks[k]));
                                //}
                            }
                        }
                        //计算各路段的M值
                        double SumMiu = 0;
                        for (int i = 1; i <= mius.Count; i++)
                        {
                            SumMiu += mius[i - 1];
                            //这里与原TranSci paper不同，也考虑了路段与自己相比的regret
                            if (i % (PairLinks.Count) == 0)
                            {
                                Ms.Add((1.0 / PairLinks.Count) / (1.0 - 1.0 / PairLinks.Count) * SumMiu);
                                SumMiu = 0;
                            }
                        }
                        //选取M值最小的路段
                        double minM = Ms.Min();
                        for (int i = 0; i < PairLinks.Count; i++)
                        {
                            if (Ms[i] == minM) ChosenLink = PairLinks[i];
                        }
                    }

                    //Last = UpdatedNode;
                    UpdatedNode = WorkingNet.AllNodes[ChosenLink.To.ID - 1];
                    RegretPath.Add(ChosenLink);
                    //如果RegretPath中的Link数超过了PoPath，则肯定是出现了Loop
                    if (RegretPath.Count > POPathSet.Count)
                    {
                        MessageBox.Show(this, "Error! Loop occurs in Regret path!");
                        break;
                    }
                }
                stw.Stop();
                TimeSpan_RBP = stw.ElapsedMilliseconds;
                RBP = RegretPath;
                return true;
            }
            catch { TimeSpan_RBP = -1; RBP = null; return false; throw new Exception(); }
        }

        //TRB Regret-based path in PO set, local search
        //Take the R(a),这里的R(a)与之前的TranSCi的R(a)计算略有不同，也考虑了路段与自己本身对比的regret
        //加入BackwardPenalty，若产生了loop，则加入10%的backward penalty，重新生成OptiHeuristic和Pessimistic，然后再次计算RegretPath
        //如果还是有loop，递增penalty，直到没有loop产生为止
        public bool RegretPath_TRBLocal_BackwardPenalty(Network WorkingNet, int o, int d, List<Link> POPathSet, out List<Link> RBP, out long TimeSpan_RBP)
        {
            Stopwatch stw = new Stopwatch();
            stw.Start();
            bool LoopOccur = false;
            double BackwardPenalty = 0.05;
            List<Link> RegretPath = null;
            int penaltytimes = 0;
            do
            {
                long TimeSpanDij1 = -1;
                long TimeSpanDij2 = -1;
                if (LoopOccur)
                {
                    Dijkstra_Recover(WorkingNet);
                    ApplyBackwardPenalty(WorkingNet, o, d, BackwardPenalty);
                    penaltytimes++;
                    FibDijkstra(WorkingNet, d, o, true, out TimeSpanDij1);//产生Optimistic heuristics
                    Dijkstra_RecoverForRSA(WorkingNet);//恢复Dijkstra探索所需的初始条件，但保留已经生成的Heuristics值
                    FibDijkstra(WorkingNet, d, o, false, out TimeSpanDij2);//产生Pessimistic heuristics
                }
                RegretPath = new List<Link>();
                Node UpdatedNode = WorkingNet.AllNodes[o - 1];


                Link ChosenLink = new Link();
                //Node Last=new Node();

                while (UpdatedNode != WorkingNet.AllNodes[d - 1])
                {
                    List<Link> PairLinks = new List<Link>();


                    //添加用于计算Regret的比较路段
                    foreach (Link linki in UpdatedNode.OutLinks)//从起点出去的路段
                    {
                        if (POPathSet.Contains(linki))
                            PairLinks.Add(linki);
                    }
                    if (PairLinks.Count == 1)
                    {
                        ChosenLink = PairLinks[0];
                    }

                    else if (PairLinks.Count >= 2)
                    {
                        List<double> mius = new List<double>();
                        List<double> Ms = new List<double>();
                        for (int j = 0; j < PairLinks.Count; j++)
                        {
                            for (int k = 0; k < PairLinks.Count; k++)
                            {
                                //if (j != k)
                                //{
                                mius.Add(Calculate_miu(WorkingNet, PairLinks[j], PairLinks[k]));
                                //}
                            }
                        }
                        //计算各路段的M值
                        double SumMiu = 0;
                        for (int i = 1; i <= mius.Count; i++)
                        {
                            SumMiu += mius[i - 1];
                            //这里与原TranSci paper不同，也考虑了路段与自己相比的regret
                            if (i % (PairLinks.Count) == 0)
                            {
                                Ms.Add((1.0 / PairLinks.Count) / (1.0 - 1.0 / PairLinks.Count) * SumMiu);
                                SumMiu = 0;
                            }
                        }
                        //选取M值最小的路段
                        double minM = Ms.Min();
                        for (int i = 0; i < PairLinks.Count; i++)
                        {
                            if (Ms[i] == minM) ChosenLink = PairLinks[i];
                        }
                    }

                    //Last = UpdatedNode;
                    UpdatedNode = WorkingNet.AllNodes[ChosenLink.To.ID - 1];
                    if (!RegretPath.Contains(ChosenLink))
                    { RegretPath.Add(ChosenLink); LoopOccur = false; }
                    else
                    {
                        LoopOccur = true;
                        break;
                    }
                    //如果RegretPath中的Link数超过了PoPath，则肯定是出现了Loop
                    //if (RegretPath.Count > POPathSet.Count)
                    //{
                    //    //MessageBox.Show(this, "Error! Loop occurs in Regret path!");
                    //    LoopOccur = true;
                    //    break;
                    //}
                    //else { LoopOccur = false; }
                }
            }
            while (LoopOccur);
            stw.Stop();
            TimeSpan_RBP = stw.ElapsedMilliseconds;
            RBP = RegretPath;
            if (penaltytimes > 0) AlgLog_tb.Text +=
                String.Format("Loop occurs, solve it through {0}% backward penalty,{1} times loop with {2}% every loop\r\n",
                penaltytimes * 100 * BackwardPenalty, penaltytimes, 100 * BackwardPenalty);
            return true;

        }

        //Add expected to minus regret as the selecting standard. 
        public bool RegretPath_TRBLocal_BackwardPenaltyWithEU(Network WorkingNet, int o, int d, List<Link> POPathSet, out List<Link> RBP, out long TimeSpan_RBP)
        {
            Stopwatch stw = new Stopwatch();
            stw.Start();
            bool LoopOccur = false;
            double BackwardPenalty = 0.05;
            List<Link> RegretPath = null;
            int penaltytimes = 0;
            do
            {
                long TimeSpanDij1 = -1;
                long TimeSpanDij2 = -1;
                if (LoopOccur)
                {
                    Dijkstra_Recover(WorkingNet);
                    ApplyBackwardPenalty(WorkingNet, o, d, BackwardPenalty);
                    penaltytimes++;
                    FibDijkstra(WorkingNet, d, o, true, out TimeSpanDij1);//产生Optimistic heuristics
                    Dijkstra_RecoverForRSA(WorkingNet);//恢复Dijkstra探索所需的初始条件，但保留已经生成的Heuristics值
                    FibDijkstra(WorkingNet, d, o, false, out TimeSpanDij2);//产生Pessimistic heuristics
                }
                RegretPath = new List<Link>();
                Node UpdatedNode = WorkingNet.AllNodes[o - 1];


                Link ChosenLink = new Link();
                //Node Last=new Node();

                while (UpdatedNode != WorkingNet.AllNodes[d - 1])
                {
                    List<Link> PairLinks = new List<Link>();


                    //添加用于计算Regret的比较路段
                    foreach (Link linki in UpdatedNode.OutLinks)//从起点出去的路段
                    {
                        if (POPathSet.Contains(linki))
                            PairLinks.Add(linki);
                    }
                    if (PairLinks.Count == 1)
                    {
                        ChosenLink = PairLinks[0];
                    }

                    else if (PairLinks.Count >= 2)
                    {
                        List<double> mius = new List<double>();
                        List<double> Ms = new List<double>();
                        for (int j = 0; j < PairLinks.Count; j++)
                        {
                            for (int k = 0; k < PairLinks.Count; k++)
                            {
                                //if (j != k)
                                //{
                                mius.Add(Calculate_miu(WorkingNet, PairLinks[j], PairLinks[k]));
                                //}
                            }
                        }
                        //计算各路段的M值
                        double SumMiu = 0;
                        for (int i = 1; i <= mius.Count; i++)
                        {
                            SumMiu += mius[i - 1];
                            //这里与原TranSci paper不同，也考虑了路段与自己相比的regret
                            if (i % (PairLinks.Count) == 0)
                            {
                                Ms.Add((1.0 / PairLinks.Count) / (1.0 - 1.0 / PairLinks.Count) * SumMiu);
                                SumMiu = 0;
                            }
                        }
                        //选取M值最小的路段
                        List<double> MsAddEU = new List<double>();
                        for (int i = 0; i < Ms.Count; i++)
                        {
                            var Et = Alpha1 * PairLinks[i].TravelTime_variable + (1 - Alpha1) * (PairLinks[i].TravelTime_variable + 1 / PairLinks[i].Fa);
                            var kadash = Et + Ms[i];
                            var Egpa = kadash + Alpha2 * PairLinks[i].To.OptHeuristic + (1 - Alpha2) * PairLinks[i].To.PessHeuristic;

                            MsAddEU.Add(Egpa + Ms[i]);
                        }
                        double minM = MsAddEU.Min();
                        for (int i = 0; i < PairLinks.Count; i++)
                        {
                            if (MsAddEU[i] == minM) ChosenLink = PairLinks[i];
                        }
                    }


                    UpdatedNode = WorkingNet.AllNodes[ChosenLink.To.ID - 1];
                    if (!RegretPath.Contains(ChosenLink))
                    { RegretPath.Add(ChosenLink); LoopOccur = false; }
                    else
                    {
                        LoopOccur = true;
                        break;
                    }
                }
            }
            while (LoopOccur);
            stw.Stop();
            TimeSpan_RBP = stw.ElapsedMilliseconds;
            RBP = RegretPath;
            if (penaltytimes > 0)
            {
                AlgLog_tb.Text +=
                   String.Format("Loop occurs, solve it through {0}% backward penalty,{1} times loop with {2}% every loop\r\n",
                   penaltytimes * 100 * BackwardPenalty, penaltytimes, 100 * BackwardPenalty);
                RecoverTravelTime(WorkingNet);//由于对某些backward link进行了traveltime_variable惩罚，在结束后应当将其恢复
            }
            return true;

        }

        double Calculate_miu(Network WorkingNet, Link link1, Link link2)
        //double Calculate_miu(Link link1, Link link2,Node Last)
        {
            int[,] scenarios = new int[,]{{0,0,0,0},{1,0,0,0},{0,1,0,0},{1,1,0,0},{0,0,1,0},
                                          {1,0,1,0},{0,1,1,0},{1,1,1,0},{0,0,0,1},{1,0,0,1},
                                          {0,1,0,1},{1,1,0,1},{0,0,1,1},{1,0,1,1},{0,1,1,1},{1,1,1,1}};

            double miu12 = 0;
            //if (link1.To == Last) return double.PositiveInfinity;
            //if (link2.To == Last) return double.NegativeInfinity;
            //else
            //{
            for (int i = 0; i < 16; i++)
            {
                double temp = scenarios[i, 2] == 0 ? WorkingNet.AllNodes[link1.To.ID - 1].OptHeuristic : WorkingNet.AllNodes[link1.To.ID - 1].PessHeuristic;
                double temp2 = scenarios[i, 0] * (1 / link1.Fa) + link1.TravelTime_variable;

                link1.Cost = scenarios[i, 0] * (1 / link1.Fa) + link1.TravelTime_variable + (scenarios[i, 2] == 0 ? WorkingNet.AllNodes[link1.To.ID - 1].OptHeuristic : WorkingNet.AllNodes[link1.To.ID - 1].PessHeuristic);
                link2.Cost = scenarios[i, 1] * (1 / link2.Fa) + link2.TravelTime_variable + (scenarios[i, 3] == 0 ? WorkingNet.AllNodes[link2.To.ID - 1].OptHeuristic : WorkingNet.AllNodes[link2.To.ID - 1].PessHeuristic);
                double Delta12 = link1.Cost - link2.Cost;

                double Regret12 = (Delta12 < 0) ? 0 : Math.Pow(Delta12, Beta);

                double m12 = link1.Cost + Regret12;

                double pw = (scenarios[i, 0] == 0 ? (1 - Alpha1) : Alpha1) * (scenarios[i, 1] == 0 ? (1 - Alpha1) : Alpha1) * (scenarios[i, 2] == 0 ? (1 - Alpha1) : Alpha1) * (scenarios[i, 3] == 0 ? (1 - Alpha1) : Alpha1);
                miu12 += m12 * pw;//当Alpha=0.5，概率可以简化为0.0625.
            }
            return miu12;
            //}
        }

        double Calculate_miuForSubNet(Network WorkingNet, Link link1, Link link2)
        {
            int[,] scenarios = new int[,]{{0,0,0,0},{1,0,0,0},{0,1,0,0},{1,1,0,0},{0,0,1,0},
                                          {1,0,1,0},{0,1,1,0},{1,1,1,0},{0,0,0,1},{1,0,0,1},
                                          {0,1,0,1},{1,1,0,1},{0,0,1,1},{1,0,1,1},{0,1,1,1},{1,1,1,1}};
            //double Alpha1 = 0.5;
            //double Beta = 2;
            //double Gama = 0.3333;//useless here
            double miu12 = 0;
            //if (link1.To == Last) return double.PositiveInfinity;
            //if (link2.To == Last) return double.NegativeInfinity;
            //else
            //{
            for (int i = 0; i < 16; i++)
            {
                double temp = scenarios[i, 2] == 0 ? WorkingNet.AllNodes[link1.To.SubID - 1].OptHeuristic : WorkingNet.AllNodes[link1.To.SubID - 1].PessHeuristic;
                double temp2 = scenarios[i, 0] * (1 / link1.Fa) + link1.TravelTime_variable;

                link1.Cost = scenarios[i, 0] * (1 / link1.Fa) + link1.TravelTime_variable + (scenarios[i, 2] == 0 ? WorkingNet.AllNodes[link1.To.SubID - 1].OptHeuristic : WorkingNet.AllNodes[link1.To.SubID - 1].PessHeuristic);
                link2.Cost = scenarios[i, 1] * (1 / link2.Fa) + link2.TravelTime_variable + (scenarios[i, 3] == 0 ? WorkingNet.AllNodes[link2.To.SubID - 1].OptHeuristic : WorkingNet.AllNodes[link2.To.SubID - 1].PessHeuristic);
                double Delta12 = link1.Cost - link2.Cost;

                double Regret12 = (Delta12 < 0) ? 0 : Math.Pow(Delta12, Beta);

                double m12 = link1.Cost + Regret12;

                double pw = (scenarios[i, 0] == 0 ? (1 - Alpha1) : Alpha1) * (scenarios[i, 1] == 0 ? (1 - Alpha1) : Alpha1) * (scenarios[i, 2] == 0 ? (1 - Alpha1) : Alpha1) * (scenarios[i, 3] == 0 ? (1 - Alpha1) : Alpha1);
                miu12 += m12 * pw;//当Alpha=0.5，概率可以简化为0.0625.
            }
            return miu12;
            //}
        }

        #endregion

        

        #region INSTR

        /// <summary>
        /// This algorithm actually search from d to o
        /// </summary>
        /// <param name="WorkingNet"></param>
        /// <param name="o"></param>
        /// <param name="d"></param>
        /// <param name="prior"> ture代表optimistic，false为pessimistic </param>
        /// <param name="TimeSpan"></param>
        /// <returns></returns>
        bool FibDijkstraForwardForSubNet(Network WorkingNet, int o, int d, bool prior, out long TimeSpan)
        {
            TimeSpan = -1;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            FibonacciHeap<Node> Updated = new FibonacciHeap<Node>();//Updated node collection, add the new updated nodes and delete the permanent nodes dynamically
            List<Node> ProcessFinished = new List<Node>();//Permanent collection
            if (prior == true)
                WorkingNet.AllNodes[o - 1].OptHeuristic = 0;
            else WorkingNet.AllNodes[o - 1].PessHeuristic = 0;
            WorkingNet.AllNodes[o - 1].HasProcessed = true;
            Dictionary<Node, FibonacciHeapNode<Node>> FibNodeDict = new Dictionary<Node, FibonacciHeapNode<Node>>();
            Node tempnode = WorkingNet.AllNodes[o - 1];
            Node tempnextnode;
            Updated.insert(new FibonacciHeapNode<Node>(tempnode), 0);
            int ClosedNodes = 1;
            while (ClosedNodes != WorkingNet.AllNodes.Count)//go on the loop before every node gets into the permanent collection
            {
                if (Updated.isEmpty()) break;//如果所有的点都已经获得P标号（剩下的没有获得P标号的是不可达点），主要用防止在单向网络中可能发生的错误
                tempnode = Updated.removeMin().getData();
                tempnode.HasProcessed = true;
                ClosedNodes++;
                for (int j = 0; j < tempnode.OutLinks.Count; j++)
                {
                    //******************************************************************错误就在于这里，不一定是ToID，也有可能更新FromID
                    //tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].ToID - 1];
                    //应该是更新流出路段的另一节点，而非一定是ToID。因为在路网拓扑构建的时候就已经考虑了路段的方向性。因此此处的tonode实际上是outlink的除了当前点之外的另一节点，可能是tonode，也可能是fromnode
                    if (tempnode.GID == tempnode.OutLinks[j].FromGID)
                        tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].To.SubID - 1];
                    else tempnextnode = WorkingNet.AllNodes[tempnode.OutLinks[j].From.SubID - 1];
                    // ********************************************************************
                    if (prior == true)
                    {
                        if (tempnextnode.OptHeuristic > tempnode.OptHeuristic + tempnode.OutLinks[j].TravelTime_variable) //乐观最短路与悲观最短路的费用
                        {
                            tempnextnode.OptHeuristic = tempnode.OptHeuristic + tempnode.OutLinks[j].TravelTime_variable;
                            tempnextnode.NextNodeID = tempnode.SubID;
                            if (tempnextnode.HasProcessed == false)
                            {
                                FibonacciHeapNode<Node> FibNode;
                                if (!FibNodeDict.ContainsKey(tempnextnode))
                                {
                                    FibNode = new FibonacciHeapNode<Node>(tempnextnode);
                                    FibNodeDict.Add(tempnextnode, FibNode);
                                    Updated.insert(FibNode, tempnextnode.OptHeuristic);
                                }
                                else
                                {
                                    FibNodeDict.TryGetValue(tempnextnode, out FibNode);
                                    Updated.decreaseKey(FibNode, tempnextnode.OptHeuristic);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tempnextnode.PessHeuristic > tempnode.PessHeuristic + tempnode.OutLinks[j].TravelTime_variable + 1 / tempnode.OutLinks[j].Fa) //乐观最短路与悲观最短路的费用
                        {
                            tempnextnode.PessHeuristic = tempnode.PessHeuristic + tempnode.OutLinks[j].TravelTime_variable + 1 / tempnode.OutLinks[j].Fa;
                            tempnextnode.NextNodeID = tempnode.SubID;
                            if (tempnextnode.HasProcessed == false)
                            {
                                FibonacciHeapNode<Node> FibNode;
                                if (!FibNodeDict.ContainsKey(tempnextnode))
                                {
                                    FibNode = new FibonacciHeapNode<Node>(tempnextnode);
                                    FibNodeDict.Add(tempnextnode, FibNode);
                                    Updated.insert(FibNode, tempnextnode.PessHeuristic);
                                }
                                else
                                {
                                    FibNodeDict.TryGetValue(tempnextnode, out FibNode);
                                    Updated.decreaseKey(FibNode, tempnextnode.PessHeuristic);
                                }
                            }
                        }
                    }
                }
            }



            sw.Stop();
            TimeSpan = sw.ElapsedMilliseconds;

            return true;
        }



        /// <summary>
        /// Single Reliable Shortest Path
        /// </summary>
        /// <param name="workingNet"></param>C
        /// <param name="o"></param>
        /// <param name="d"></param>
        /// <param name="path"></param>
        /// <param name="TimeSpan_SRSP"></param>
        public bool SRSP(Network workingNet, int o, int d, out Route path, out long TimeSpan_SRSP)
        {
            path = null;
            TimeSpan_SRSP = -1;

            //function entity

            //Run shortest path

            //link removal

            //

            return true;
        }


        public double CalculateReliability(Route route)
        {
            return 0;
        }

        void penalize(Route r, double parameter)
        {
            foreach (Link i in r.Links)
            {
                i.TravelTime_variable = (1 + parameter) * i.TravelTime_variable;
            }
        }
        /// <summary>
        /// Get the shortest path calculated by t (no delay)
        /// </summary>
        /// <param name="workingNet">mother network</param>
        /// <param name="SubNet_hyperpath">subnetwork</param>
        /// <param name="o_subID">sub_ID of origin in subnetwork</param>
        /// <param name="d_subID">sub_ID of destination in subnetwork</param>
        /// <returns></returns>
        Route getOptmisticRoute(Network workingNet, Network SubNet_hyperpath, int o_subID, int d_subID)
        {
            //find the shortest path and calculate reliability 
            long TimeSpan_SP = -1;
            bool Accessible = false;

            //Optimisitc shortest path
            FibDijkstraForwardForSubNet(SubNet_hyperpath, o_subID, d_subID, true, out TimeSpan_SP);
            List<int> opt_prior_SubIDs = GetShortestPathForSubNet(SubNet_hyperpath, o_subID, d_subID, true, out Accessible);

            List<Link> opt_prior_links = new List<Link>();
            for (int i = 0; i < opt_prior_SubIDs.Count; i++)
            {
                Link l = SubNet_hyperpath.AllLinks[opt_prior_SubIDs[i] - 1];
                opt_prior_links.Add(l);
            }
            Route opt_prior = new Route(opt_prior_links.ToArray());
            Dijkstra_Recover(SubNet_hyperpath);
            return opt_prior;
        }

        Route getPessimisticRoute(Network workingNet, Network SubNet_hyperpath, int o_subID, int d_subID)
        {
            long TimeSpan_SP = -1;
            bool Accessible = false;
             //Pessimistic shortest path
            FibDijkstraForwardForSubNet(SubNet_hyperpath, o_subID, d_subID, true, out TimeSpan_SP);
            List<int> pes_prior_SubIDs = GetShortestPathForSubNet(SubNet_hyperpath, o_subID, d_subID, true, out Accessible);

            List<Link> pes_prior_links = new List<Link>();
            for (int i = 0; i < pes_prior_SubIDs.Count; i++)
            {
                Link l = SubNet_hyperpath.AllLinks[pes_prior_SubIDs[i] - 1];
                pes_prior_links.Add(l);
            }
            Route pes_prior = new Route(pes_prior_links.ToArray());
            Dijkstra_Recover(SubNet_hyperpath);
            return pes_prior;
        }


        /// <summary>
        /// INSTR Conference work, create several routes with the highest defined absolute reliability index, Multiple Reliable Shortest Path
        /// </summary>
        /// <param name="workingNet">工作母网络</param>
        /// <param name="o">母网络中的起点ID</param>
        /// <param name="d">母网络中的终点ID</param>
        /// <param name="subNet_Hyperpath">已经创建的hyperpath子网络</param>
        /// <param name="topN"></param>
        /// <param name="reliableRoutes"></param>
        /// <param name="TimeSpan_RBP"></param>
        /// <returns></returns>
        
        public bool MRSP(Network workingNet, int o, int d , int topN, out List<Route> reliableRoutes , out long TimeSpan_MRSP)
        {
            int maxIter = 1000;
            int K = 20; //the number different routes
            double lambda = 0.5; //penal the large delays: d >= 0.8t


            reliableRoutes = null;
            TimeSpan_MRSP = -1;

            //下面是函数的主体部分
            //Create hyperpath links by FDHS algorithm
            List<Link> hyperpath_raw = new List<Link>();
            long time_sp = -1;
            long time_hp=-1;

            //get the raw hyperpath (links with probability also included, Potentially optimal? What is the rule to identify this?)
            FDHS(workingNet, o, d, ref hyperpath_raw, out time_sp, out time_hp);
            
            //get the hyperpath, p!=0
            List<Link> hyperpath = GetHyperpath(hyperpath_raw);

            List<Link> temp = new List<Link>();
            DHS_Recover(workingNet, temp); 

            //Create subnetwork from calculated hyperpath links
            Network SubNet_hyperpath = workingNet.CreateSubNetwork(hyperpath.ToArray());

            int o_subID = 0;
            int d_subID = 0;
            foreach (Node i in SubNet_hyperpath.AllNodes)
            {
                if (i.GID == o)
                    o_subID = i.SubID;
                else if (i.GID == d)
                    d_subID = i.SubID;
            }

            //k-shortest procedure
           
            List<Route> routes = new List<Route>();
            HashSet<string> existingRoutes = new HashSet<string>();

            int iter = 0;
            while (routes.Count < K)
            {
                Route opt_prior = getOptmisticRoute(workingNet, SubNet_hyperpath, o_subID, d_subID);
                
                //accept the route if it is different with the past ones
                string currentRouteString = VectorToString(GetLinkVector(SubNet_hyperpath, opt_prior.ToIDs()));
                

                if (!existingRoutes.Contains(currentRouteString)) 
                {
                    existingRoutes.Add(currentRouteString);
                    routes.Add(opt_prior); 
                }
                
                //penal the links on determined shortest path (according to the Doctoral paper in matlab code, ask wei-san)
                penalize(opt_prior, 0.2);
                SubNet_hyperpath.penalized++;
                iter++;
                if (iter > maxIter) break;
            }
            //the reliability calculation procedure
            foreach (Route r in routes)
            {
                CalReliability(r,lambda);
            }
            
            CompareRoutes compare=new CompareRoutes();
            routes.Sort(compare);

            DHS_Recover(workingNet,hyperpath_raw);
            return true;
        }

        internal class CompareRoutes : IComparer<Route>
        {
            public int Compare(Route x, Route y)
            {
                if (x.Reliability == y.Reliability) return 0;
                else if (x.Reliability > y.Reliability) return 1;
                else return -1;
            }
        }

        public string VectorToString(List<int> vector) 
        {
            StringBuilder sb = new StringBuilder();
            foreach (int i in vector)
            {
                sb.Append(i.ToString());
            }
            return sb.ToString();
        }

        public void CalReliability(Route r, double lambda)
        {
            
            double sum_t = 0;
            double sum_d = 0;
            double sum_x = 0;
            foreach (Link i in r.Links)
            {
                sum_t += i.TravelTime_Fixed;
                sum_d += 1 / i.Fa;
                double x= (1/i.Fa)/i.TravelTime_Fixed < lambda ? 0:1;
                sum_x += x;
            }
            r.Reliability = sum_t / (sum_d * sum_x);
        }


        #endregion
    }
}
