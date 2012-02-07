using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkLib2.Basis
{
    public abstract class Network<K,V,E> : IDisposable
        where V:Element
        where E:Element
    {
        Element_Dict<K,V> vertices;
        public Element_Dict<K,V> Vertices { get { return vertices; } }

        Element_Dict<K,E> edges;
        public Element_Dict<K,E> Edges { get { return edges; } }

        public Network()
        {
            vertices = new Element_Dict<K,V>();
            edges = new Element_Dict<K,E>();
        }

        //由已经读取好的节点集和路段集构建网络(静态网络)
        public Network(Element_Dict<K,V> _vertices, Element_Dict<K,E> _Es)
        {
            vertices = _vertices;
            edges = _Es;
        }

        #region ReDesign

        public virtual void AddV(V _V)
        {
           
        }

        public virtual void AddE(E _E)
        {
        }

        public virtual void RemoveV(V _V)
        {
            
        }

        public virtual void RemoveE(E _E)
        {
            
        }

        #endregion




        public virtual void RemoveV(string _id)
        {
            
        }

        public virtual void RemoveE(string _id)
        {
           
        }

        public virtual void Clear()
        {
            vertices.Clear();
            edges.Clear();
        }


        #region IDisposable 成员

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }


        #endregion


        public override string ToString()
        {
            return vertices.Count.ToString() + " Nodes, " + edges.Count.ToString() + " Links";
        }
    }
}
