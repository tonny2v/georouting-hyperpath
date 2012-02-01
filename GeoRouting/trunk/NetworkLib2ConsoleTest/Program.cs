using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NetworkLib2.GeneralNetwork.Algorithm;
using NetworkLib2.GeneralNetwork.Concept;

namespace NetworkLib2ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int degree=4;
            int nodenumber=1000;
            int linknumber = degree * nodenumber;

            #region GNetwork Building Test
            //网络
            GNetwork net = new GNetwork();

            //节点
            for (int i = 1; i <=nodenumber; i++)
            {
                Vertex Vi = new Vertex(i.ToString());
                net.AddV(Vi);
            }
            //边
            Random rand=new Random(DateTime.Now.Millisecond);
            for (int i = 1; i <= linknumber; i++)
            {
                Edge Ei = new Edge(i.ToString());
                Ei.From = net.Vertices[((int)rand.Next(1,nodenumber+1)).ToString()];
                Ei.To = net.Vertices[((int)rand.Next(1, nodenumber+1)).ToString()];
                Ei.Cost = rand.Next(30,60);
                net.AddE(Ei);
            }

            Console.WriteLine("GNetwork: {0}",net);
            #endregion

            #region GNetwork Element Removing Test
            //net.RemoveVertex("1");

            //for (int i = 1; i < 51; i++)
            //{
            //    net.RemoveEdge(i.ToString());
            //}

            //Console.WriteLine("GNetwork: {0}", net);
            #endregion

            foreach (var Vi in net.Vertices.Values)
            {
                Console.WriteLine("Vertex {0}:\tInEdges: {1}\tOutEdges:{2}", Vi, Vi.InEdges,Vi.OutEdges);
            }


            foreach (var Ei in net.Edges.Values)
            {
                Console.WriteLine("Edge {0}:\tFrom: {1}\tTo: {2}\tCost: {3}",Ei,Ei.From,Ei.To,Ei.Cost);
            }

            SinglePathAlgorithm alg = new SinglePathAlgorithm();
            alg.Net = net;
            alg.O_ID = "1";
            alg.D_ID = "19";
            
            alg.Run("RawDijkstra");
            Console.WriteLine("Raw Dijkstra SinglePathAlgorithm Time: {0} ms",alg.GetTime());
            string s = string.Empty;
            List<string> output=alg.GetPathLinks();

            //反过来输出最短路，此处是以节点表示的最短路
            for (int i = output.Count-1; i >=0; i--)
            {
                if (i != output.Count-1)
                    s = s + "->"+ output[i];
                else
                    s = s + output[i];
            }

            Console.WriteLine("SP: {0}",s);
            alg.Dispose();
            Console.ReadKey();

        }
    }
}
