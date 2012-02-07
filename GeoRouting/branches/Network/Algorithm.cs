using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkLib.Element;
using System.Diagnostics;

namespace NetworkLib.Algorithm
{
    interface IAlgorithm
    {
        bool Run(Network _net, int _o, int _d, bool _direction);
    }

    class Dijkstra_Raw : IAlgorithm
    {


        #region IAlgorithm 成员


        public bool Run(Network _net, int o, int d, bool _directed)
        {
            bool success = false;
            try
            {
                List<Node> Updated = new List<Node>();  //Updated node collection, add the new updated nodes and delete the permanent nodes dynamically
                List<Node> ProcessFinished = new List<Node>();//Permanent collection
                _net.AllNodes[o - 1].OptHeuristic = 0;
                _net.AllNodes[o - 1].HasProcessed = true;
                Updated.Add(_net.AllNodes[o - 1]);
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
                        //tempnextnode = _net.AllNodes[tempnode.OutLinks[j].ToID - 1];
                        //应该是更新流出路段的另一节点，而非一定是ToID。因为在路网拓扑构建的时候就已经考虑了路段的方向性。因此此处的tonode实际上是outlink的除了当前点之外的另一节点，可能是tonode，也可能是fromnode
                        if (tempnode.GID == tempnode.OutLinks[j].ToGID)
                            tempnextnode = _net.AllNodes[tempnode.OutLinks[j].FromGID - 1];
                        else tempnextnode = _net.AllNodes[tempnode.OutLinks[j].ToGID - 1];
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
            return success;
        }

        #endregion
    }

    class FibDijkstra_Goal { }
}
