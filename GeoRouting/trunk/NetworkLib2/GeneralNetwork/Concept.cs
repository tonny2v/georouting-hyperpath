//#define HPDInfinity

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NetworkLib2.Basis;

namespace NetworkLib2.GeneralNetwork.Concept
{
    //General Elements

    [Serializable]
    public partial class Vertex:Basis.Element
    {
        public Element_Dict<string,Edge> InEdges { get; set; }
        public Element_Dict<string,Edge> OutEdges { get; set; }

        public override string ToString()
        {
            return ID;
        }
    }

    //For Dijkstra Shortest path SinglePathAlgorithm
    
    public partial class Vertex:Basis.Element
    {
        public double Heuristic { get; set; }
        public bool HasProcessed { get; set; }
        //指示最短路径的回溯点
        public string SP_Next { get; set; }
        //构造函数
        public Vertex() { }
        public Vertex(string _id)
        {
            ID = _id;
            InEdges = new Element_Dict<string,Edge>();
            OutEdges = new Element_Dict<string,Edge>();
        }
    }

    [Serializable]
    public class Edge: Element
    {
        public Vertex From { get; set; }
        public Vertex To { get; set; }

        public double Cost { get; set; }

        public Edge() { }
        public Edge(string _id)
        {
            ID = _id;
        }

        public override string ToString()
        {
            return ID + ": " + From.ID + "->" + To.ID ;
        }


    }

    [Serializable]
    public class GNetwork : Network<string, Vertex, Edge>
    {

        #region ReDesign

        public override void AddV(Vertex _vertex)
        {
            Vertices.Add(_vertex.ID, _vertex);
        }
        
        public override void AddE(Edge _edge)
        {

            Edges.Add(_edge.ID, _edge);
            _edge.From.OutEdges.Add(_edge.ID, _edge);
            _edge.To.InEdges.Add(_edge.ID, _edge);

        }

        public override void RemoveV(Vertex _vertex)
        {
            //移除节点的关联边
            foreach (Edge i in _vertex.InEdges.Values)
            {
                Edges.Remove(i.ID);
            }
            foreach (Edge i in _vertex.OutEdges.Values)
            {
                Edges.Remove(i.ID);
            }
            //移除节点本身
            Vertices.Remove(_vertex.ID);
        }

        public override void RemoveE(Edge _edge)
        {
            //检查link的端点是否有其他link连接，若没有，则同时移除节点
            if (_edge.From.OutEdges.Count == 1 || _edge.From.InEdges.Count == 0)
                Vertices.Remove(_edge.From.ID);
            if (_edge.To.InEdges.Count == 1 || _edge.To.OutEdges.Count == 0)
                Vertices.Remove(_edge.To.ID);
            Edges.Remove(_edge.ID);
        }

        #endregion


        public override string ToString()
        {
            return Vertices.Count.ToString() + " Nodes, " + Edges.Count.ToString() + " Links";
        }

        public override void RemoveV(string _id)
        {
            //移除节点的关联边
            foreach (Edge i in Vertices[_id].InEdges.Values)
            {
                Edges.Remove(i.ID);
            }
            foreach (Edge i in Vertices[_id].OutEdges.Values)
            {
                Edges.Remove(i.ID);
            }
            //移除节点本身
            Vertices.Remove(Vertices[_id].ID);
        }

        public override void RemoveE(string _id)
        {
            //检查link的端点是否有其他link连接，若没有，则同时移除节点
            if (Edges[_id].From.OutEdges.Count == 1 || Edges[_id].From.InEdges.Count == 0)
                Vertices.Remove(Edges[_id].From.ID);
            if (Edges[_id].To.InEdges.Count == 1 || Edges[_id].To.OutEdges.Count == 0)
                Vertices.Remove(Edges[_id].To.ID);
            Edges.Remove(Edges[_id].ID);
        }

        public override void Clear()
        {
            Vertices.Clear();
            Edges.Clear();
        }

        #region IDisposable 成员

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

    }

  
  


}
