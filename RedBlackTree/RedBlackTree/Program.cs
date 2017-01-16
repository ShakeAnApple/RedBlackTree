using System;
using System.Collections.Generic;

namespace RedBlackTree
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = new RBTree<Int16>(0);
            var a = tree.GetNode(0);
            var ll = new LinkedList<int>();

            ll.AddLast(2);
            var b = ll.Find(2);
        }
    }

    public enum NodeType
    {
        Red,
        Black
    }

    public interface INode<TData> where TData : IComparable<TData>
    {
        TData Value { get; }
        INode<TData> Left { get; }
        INode<TData> Right { get; }
    }

    public class RBTree<TData> where TData : IComparable<TData>
    {
        TreeNode root;

        public RBTree(TData rootValue)
        {
            root = new TreeNode(rootValue);
        }

        public void AddNode(TData value)
        {
            throw new NotImplementedException();
        }

        public void DeleteNode(TData value)
        {
            throw new NotImplementedException();
        }

        public INode<TData> GetNode(TData value)
        {
            throw new NotImplementedException("");
        }

        class TreeNode : INode<TData>
        {
            public TreeNode(TData value)
            {
                this.Value = value;
            }

            public TreeNode(TData value, NodeType nodeType) : this(value)
            {
                this.NodeType = nodeType;
            }

            INode<TData> INode<TData>.Left { get { return this.Left; } }
            INode<TData> INode<TData>.Right { get { return this.Right; } }
            TData INode<TData>.Value { get { return this.Value; } }

            public TData Value { get; set; }
            public INode<TData> Left { get; set; }
            public INode<TData> Right { get; set; }
            public NodeType NodeType { get; set; }

            public void Recolor()
            {
                throw new NotImplementedException();
            }
        }
    }
}
