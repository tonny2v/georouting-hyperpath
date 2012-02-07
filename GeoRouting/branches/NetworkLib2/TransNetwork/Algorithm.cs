using System;
using System.Collections.Generic;
using NetworkLib2.TransNetwork.Concept;
using NetworkLib2.Basis;
using System.Diagnostics;

namespace NetworkLib2.TransNetwork.Algorithm
{

    interface IAlgorithm_TransNetwork
    {
        double GetTime();
        string[] GetResultSet();
        void Run(AlgName _algName);
    }

    public enum AlgName { RawDijkstra,Astar,GoalDij,FibDij,FibGoalDij}

    public class SinglePathAlgorithm : IAlgorithm_TransNetwork, IDisposable
    {
        public TNetwork Net { get; set; }
        public string O_ID { get; set; }
        public string D_ID { get; set; }

        Stopwatch stw;
        public SinglePathAlgorithm()
        { stw = new Stopwatch(); }
        public SinglePathAlgorithm(TNetwork _net) 
        {
            stw = new Stopwatch();
            Net = _net;
        }

        public void InitializeVariables(AlgName algName)
        {
            switch (algName)
            {
                case AlgName.RawDijkstra:
                    foreach (Node i in Net.Vertices.Values)
                    {
                        i.OptHeuristic = double.PositiveInfinity;
                        i.HasProcessed = false;
                        i.SP_Next = "-1";
                    }
                    break;
                default:
                    break;
            }

        }

        public bool RawDij()
        {
            List<Node> Updated = new List<Node>();
            Element_Dict<string, Node> ProcessFinished = new Element_Dict<string,Node>();
            Net.Vertices[O_ID].OptHeuristic = 0;
            Net.Vertices[O_ID].HasProcessed = true;
            Updated.Add(Net.Vertices[O_ID]);
            double temp;
            int flag = 0;

            ProcessFinished.OnAddEvent += new  Element_Dict<string, Node>.AddEvent(OnAddEvent_Handler);

            while (true)
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

                if (flag == Updated.Count)
                {
                    break;
                }
                Node _this, _next;
                _this = Updated[flag];
                _this.HasProcessed = true;
                ProcessFinished.Add(Updated[flag].ID,Updated[flag]);//Push into the collection with permanent label
                Updated.Remove(Updated[flag]);

                foreach (Link j in _this.OutLinks.Values)
                {
                    _next = j.To;
                    if (_next.OptHeuristic > _this.OptHeuristic + j.TravelTime)
                    {
                        _next.OptHeuristic = _this.OptHeuristic + j.TravelTime;
                        _next.SP_Next = _this.ID;
                        if (_next.HasProcessed == false)
                        {
                            Updated.Add(_next);
                        }
                    }
                }
            }


            return true;
        }

        public bool Astar()
        {
            return true;
        }

        public bool GoalDij()
        { return true; }

        public bool FibDij()
        {
            return true;
        }

        public bool FibGoalDij()
        {
            return true;
        }


        public virtual void OnAddEvent_Handler(object sender, EventArgs e, Node i)
        {
            Console.WriteLine("Node " + i.ID + " is Rendered.");
            //此处通过子类继承基类后添加用户定义的事件处理函数
        }

        #region IAlgorithm_TransNetwork 成员
        public void Run(AlgName _algName)
        {
            stw.Start();
            switch (_algName)
            {
                case AlgName.RawDijkstra:
                    InitializeVariables(AlgName.RawDijkstra);
                    RawDij();
                    break;
                default:
                    break;
            }
            stw.Stop();
        }

        public double GetTime() { return stw.ElapsedMilliseconds; }

        public string[] GetResultSet()
        {
            List<string> pathLinks = new List<string>();
            string Next = D_ID;


            while (Next != O_ID)
            {
                if (Net.Vertices[Next].SP_Next == "-1")
                {
                    Console.WriteLine("UnAccessiable");
                    break;
                }
                else
                {
                    pathLinks.Add(Next);
                    Next = Net.Vertices[Next].SP_Next;
                }
            }
            pathLinks.Add(Next);
            return pathLinks.ToArray();
        }
        #endregion

        #region IDisposable 成员

        public void Dispose()
        {

        }

        #endregion
    }


}
