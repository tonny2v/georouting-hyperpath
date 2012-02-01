using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NetworkLib2.GeneralNetwork.Concept;

namespace NetworkLib2.GeneralNetwork.Algorithm
{
    interface IAlgorithm_GeneralNetwork
    {
        double GetTime();
        List<string> GetPathLinks();
        void Run(string _algName);
    }


    public class SinglePathAlgorithm:IAlgorithm_GeneralNetwork,IDisposable
    {
        public GNetwork Net { get; set; }
        public string O_ID { get; set; }
        public string D_ID { get; set; }
        
        Stopwatch stw;
        public SinglePathAlgorithm() {stw = new Stopwatch(); }

        public void InitializeVariables(string algName)
        {
            switch (algName)
            {
                case "RawDijkstra":
                    foreach (Vertex i in Net.Vertices.Values)
                    {
                        i.Heuristic = double.PositiveInfinity;
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
            List<Vertex> Updated = new List<Vertex>();
            List<Vertex> ProcessFinished = new List<Vertex>();
            Net.Vertices[O_ID].Heuristic = 0;
            Net.Vertices[O_ID].HasProcessed = true;
            Updated.Add(Net.Vertices[O_ID]);
            double temp;
            int flag = 0;
            
            while (true)
            {
                temp = double.PositiveInfinity;
                //得到P标号点

                for (int i = 0; i < Updated.Count; i++)
                {
                    if (Updated[i].HasProcessed == false)
                    {
                        if (Updated[i].Heuristic <= temp)
                        {
                            temp = Updated[i].Heuristic;
                            flag = i;
                        }
                    }
                }
               
                if (flag == Updated.Count)
                {
                    break;
                }
                Vertex _thisVertex,_nextVertex;
                _thisVertex = Updated[flag];
                _thisVertex.HasProcessed = true;
                ProcessFinished.Add(Updated[flag]);//Push into the collection with permanent label
                Updated.Remove(Updated[flag]);

                foreach (Edge j in _thisVertex.OutEdges.Values)
                {
                    _nextVertex = j.To;
                    if (_nextVertex.Heuristic > _thisVertex.Heuristic + j.Cost)
                    {
                        _nextVertex.Heuristic = _thisVertex.Heuristic + j.Cost;
                        _nextVertex.SP_Next = _thisVertex.ID;
                        if (_nextVertex.HasProcessed == false)
                        {
                            Updated.Add(_nextVertex);
                        }
                    }
                }
            }


            return true;
        }
        


        #region IAlgorithm_TransNetwork 成员
        public void Run(string _algName) 
        {
            stw.Start();
            switch (_algName) 
            {
                case "RawDijkstra":
                    InitializeVariables(_algName);
                    RawDij();
                    break;
                default:
                    break;
            }
            stw.Stop();
        }

        public double GetTime() { return stw.ElapsedMilliseconds; }

        public List<string> GetPathLinks() 
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
            return pathLinks;
        }
        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            
        }

        #endregion
    }

    
}
