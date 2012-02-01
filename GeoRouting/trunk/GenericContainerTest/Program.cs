using System;
using System.Collections.Generic;

namespace GenericContainerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();
            Console.ReadKey();
        }
        static public void Test()
        {
            Element_Col<string,Node> Node_Col = new Element_Col<string,Node>();
            Node node = new Node("1");
            Node_Col.Add(node.id, node);
            Console.WriteLine(Node_Col["1"]);
        }
    }

    public class Node { public string id; public Node(string _id) { id = _id; } }
    public class Link { public string id; public Link(string _id) { id = _id; } }

    public class Element_Col<K,T> : Dictionary<K, T> 
    {
        //public Element_Col<T>{}

        public new void Add(K key, T element)
        {
                base.Add(key, element);
        }
    }

}
