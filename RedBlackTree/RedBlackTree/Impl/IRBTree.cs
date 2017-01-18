using System;

namespace RedBlackTree
{
    public interface INode<TData> where TData : IComparable<TData>
    {
        TData Value { get; }
        INode<TData> Left { get; }
        INode<TData> Right { get; }
    }

    public interface IRBTree<TData> where TData : IComparable<TData>
    {
        INode<TData> Root { get; }

        INode<TData> AddNode(TData value);
        INode<TData> GetNode(TData value);
        void DeleteNode(TData value);
    }
}
