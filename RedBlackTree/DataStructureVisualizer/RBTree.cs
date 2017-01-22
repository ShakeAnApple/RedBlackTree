using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStructureVisualizer
{
    //public interface ILogger
    //{
    //    void Log(string message);
    //}

    //public class ConsoleLogger : ILogger
    //{
    //    public void Log(string message)
    //    {
    //        Console.WriteLine(message);
    //    }
    //}

    //public interface INode<TData> where TData : IComparable<TData>
    //{
    //    TData Value { get; }
    //    INode<TData> Left { get; }
    //    INode<TData> Right { get; }
    //}

    //public interface IRBTree<TData> where TData : IComparable<TData>
    //{
    //    INode<TData> Root { get; }

    //    INode<TData> AddNode(TData value);
    //    INode<TData> GetNode(TData value);
    //    void DeleteNode(TData value);
    //}

    //public class RBTree<TData> : IVisualizerGraphNode, IRBTree<TData> where TData : IComparable<TData>
    //{
    //    private TreeNode _root;

    //    public INode<TData> Root { get { return _root; } }

    //    private readonly ILogger _logger;

    //    public RBTree(TData rootValue)
    //    {
    //        // this.AddNode(rootValue);
    //        _root = new TreeNode(rootValue);
    //        _logger = new ConsoleLogger();
    //    }

    //    public RBTree(TData rootValue, ILogger logger)
    //    {
    //        // this.AddNode(rootValue);
    //        _root = new TreeNode(rootValue);
    //        _logger = logger;
    //    }

    //    public INode<TData> AddNode(TData value)
    //    {
    //        var newNode = new TreeNode(value);
    //        AddNodeInternal(newNode);
    //        if (newNode.Parent == null)
    //        {
    //            _root = newNode;
    //        }
    //        return newNode;
    //    }

    //    private TreeNode AddNodeInternal(TreeNode newNode)
    //    {
    //        var isLeftNode = false;
    //        var parent = FindParentForValue(newNode.Value, out isLeftNode);
    //        if (isLeftNode)
    //        {
    //            parent.Left = newNode;
    //        }
    //        else
    //        {
    //            parent.Right = newNode;
    //        }
    //        newNode.Parent = parent;

    //        RebalanceInsert(newNode);

    //        throw new NotImplementedException();
    //    }

    //    #region insert
    //    private void Insert1(TreeNode node)
    //    {
    //        if (node.Parent == null)
    //        {
    //            node.NodeType = NodeType.Black;
    //        }
    //        else
    //        {
    //            Insert2(node);
    //        }
    //    }

    //    private void Insert2(TreeNode node)
    //    {
    //        if (node.Parent.NodeType == NodeType.Black)
    //        {
    //            return;
    //        }
    //        Insert3(node);
    //    }

    //    private void Insert3(TreeNode node)
    //    {
    //        if (node.Uncle != null &&
    //            node.Uncle.NodeType == NodeType.Red &&
    //            node.Parent.NodeType == NodeType.Red)
    //        {
    //            node.Parent.NodeType = NodeType.Black;
    //            node.Uncle.NodeType = NodeType.Black;
    //            node.GrandParent.NodeType = NodeType.Red;

    //            Insert1(node.GrandParent);
    //        }
    //        else
    //        {
    //            Insert4(node);
    //        }
    //    }

    //    private void Insert4(TreeNode node)
    //    {
    //        if (node.IsRight && node.Parent.IsLeft)
    //        {
    //            RotateLeft(node.Parent);
    //            node = node.Left;
    //        }
    //        else if (node.IsLeft && node.Parent.IsRight)
    //        {
    //            RotateRight(node.Parent);
    //            node = node.Right;
    //        }
    //        Insert5(node);
    //    }

    //    private void Insert5(TreeNode node)
    //    {
    //        node.Parent.NodeType = NodeType.Black;
    //        node.GrandParent.NodeType = NodeType.Red;

    //        if (node.IsLeft && node.Parent.IsLeft)
    //        {
    //            RotateRight(node.GrandParent);
    //        }
    //        else
    //        {
    //            RotateLeft(node.GrandParent);
    //        }
    //    }
    //    #endregion

    //    private void RebalanceInsert(TreeNode newNode)
    //    {
    //        if (newNode.Parent == null)
    //        {
    //            newNode.NodeType = NodeType.Black;
    //        }
    //        throw new NotImplementedException();
    //    }

    //    private void RebalanceDelete(TreeNode node)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private TreeNode FindParentForValue(TData nodeValue, out bool isLeftNode)
    //    {
    //        TreeNode parent = null;
    //        isLeftNode = false;

    //        var prevNode = _root;
    //        while (parent == null)
    //        {
    //            if (nodeValue.CompareTo(prevNode.Value) == 0)
    //            {
    //                throw new ArgumentException("Equal nodes are not allowed");
    //            }

    //            if (nodeValue.CompareTo(prevNode.Value) < 0)
    //            {
    //                if (prevNode.Left == null)
    //                {
    //                    parent = prevNode;
    //                    isLeftNode = true;
    //                }
    //                prevNode = prevNode.Left;
    //            }
    //            else
    //            {
    //                if (prevNode.Right == null)
    //                {
    //                    parent = prevNode;
    //                    isLeftNode = false;
    //                }
    //                prevNode = prevNode.Right;
    //            }
    //        }
    //        return parent;
    //    }

    //    private void RotateLeft(TreeNode node)
    //    {
    //        var pivot = node.Right;
    //        pivot.Parent = node.Parent;
    //        if (node.Parent != null)
    //        {
    //            if (node.IsLeft)
    //            {
    //                node.Parent.Left = pivot;
    //            }
    //            else
    //            {
    //                node.Parent.Right = pivot;
    //            }
    //        }

    //        node.Right = pivot.Left;
    //        if (pivot.Left != null)
    //        {
    //            pivot.Left.Parent = node;
    //        }

    //        node.Parent = pivot;
    //        pivot.Left = node;
    //    }

    //    private void RotateRight(TreeNode node)
    //    {
    //        var pivot = node.Left;

    //        pivot.Parent = node.Parent;
    //        if (node.Parent != null)
    //        {
    //            if (node.IsLeft)
    //            {
    //                node.Parent.Left = pivot;
    //            }
    //            else
    //            {
    //                node.Parent.Right = pivot;
    //            }
    //        }

    //        node.Left = pivot.Right;
    //        if (pivot.Right != null)
    //        {
    //            pivot.Right.Parent = node;
    //        }

    //        node.Parent = pivot;
    //        pivot.Right = node;
    //    }

    //    public void DeleteNode(TData value)
    //    {
    //        var nodeToDelete = GetNodeInternal(value);

    //        if (nodeToDelete == null)
    //        {
    //            _logger.Log("Such value dosn't exist");
    //            return;
    //        }

    //        if (nodeToDelete.IsLeaf)
    //        {
    //            nodeToDelete.Parent.ReplaceChild(nodeToDelete, null);
    //            return;
    //        }

    //        var child = nodeToDelete.Right == null ? nodeToDelete.Left
    //                        : nodeToDelete.Left == null ? nodeToDelete.Right
    //                    : null;

    //        if (child != null)
    //        {
    //            nodeToDelete.Parent.ReplaceChild(nodeToDelete, child);
    //            RebalanceDelete(child);
    //            return;
    //        }

    //        var nextNode = nodeToDelete.Right;
    //        while (nextNode.Left != null) nextNode = nextNode.Left;

    //        nextNode.Parent.ReplaceChild(nextNode, nextNode.Right);
    //        nextNode.Right = null;

    //        nodeToDelete.Parent.ReplaceChild(nodeToDelete, nextNode);
    //        RebalanceDelete(nextNode);
    //    }

    //    #region delete
    //    private void Delete1(TreeNode node)
    //    {
    //        if (node.Parent != null)
    //        {
    //            Delete2(node);
    //        }
    //    }

    //    private void Delete2(TreeNode node)
    //    {
    //        if (node.Sibling.NodeType == NodeType.Red)
    //        {
    //            node.Parent.NodeType = NodeType.Red;
    //            node.Sibling.NodeType = NodeType.Black;

    //            if (node.IsLeft)
    //            {
    //                RotateLeft(node.Parent);
    //            }
    //            else
    //            {
    //                RotateRight(node.Parent);
    //            }
    //        }
    //        Delete3(node);
    //    }

    //    private void Delete3(TreeNode node)
    //    {
    //        if (node.Parent.NodeType == NodeType.Black &&
    //            node.Sibling.NodeType == NodeType.Black &&
    //            node.Sibling.Left.NodeType == NodeType.Black &&
    //            node.Sibling.Right.NodeType == NodeType.Black)
    //        {
    //            node.Sibling.NodeType = NodeType.Red;
    //            Delete1(node.Parent);
    //        }
    //        else
    //        {
    //            Delete4(node);
    //        }
    //    }

    //    private void Delete4(TreeNode node)
    //    {
    //        if (node.Parent.NodeType == NodeType.Red &&
    //             node.Sibling.NodeType == NodeType.Black &&
    //            node.Sibling.Left.NodeType == NodeType.Black &&
    //            node.Sibling.Right.NodeType == NodeType.Black)
    //        {
    //            node.Sibling.NodeType = NodeType.Red;
    //            node.Parent.NodeType = NodeType.Black;
    //        }
    //        else
    //        {
    //            Delete5(node);
    //        }
    //    }

    //    private void Delete5(TreeNode node)
    //    {
    //        /* this if statement is trivial, 
    //            due to case 2 (even though case 2 changed the sibling to a sibling's child, 
    //            the sibling's child can't be red, since no red parent can have a red child). */

    //        /* the following statements just force the red to be on the left of the left of the parent, 
    //           or right of the right, so case six will rotate correctly. */
    //        if (node.Sibling.NodeType == NodeType.Black)
    //        {
    //            if (node.IsLeft &&
    //                node.Sibling.Right.NodeType == NodeType.Black &&
    //                node.Sibling.Left.NodeType == NodeType.Red)
    //            {
    //                /* this last test is trivial too due to cases 2-4. */
    //                node.Sibling.NodeType = NodeType.Red;
    //                node.Sibling.Left.NodeType = NodeType.Black;

    //                RotateRight(node.Sibling);
    //            }
    //            else if (node.IsRight &&
    //                     node.Sibling.Right.NodeType == NodeType.Black &&
    //                     node.Sibling.Left.NodeType == NodeType.Red)
    //            {
    //                /* this last test is trivial too due to cases 2-4. */
    //                node.Sibling.NodeType = NodeType.Red;
    //                node.Sibling.Right.NodeType = NodeType.Black;

    //                RotateLeft(node.Sibling);
    //            }
    //        }

    //        Delete6(node);
    //    }

    //    private void Delete6(TreeNode node)
    //    {
    //        node.Sibling.NodeType = node.Parent.NodeType;
    //        node.Parent.NodeType = NodeType.Black;

    //        if (node.IsLeft)
    //        {
    //            node.Sibling.Right.NodeType = NodeType.Black;

    //            RotateLeft(node.Parent);
    //        }
    //        else
    //        {
    //            node.Sibling.Left.NodeType = NodeType.Black;
    //            RotateRight(node.Parent);
    //        }

    //    }

    //    #endregion

    //    public INode<TData> GetNode(TData value)
    //    {
    //        return GetNodeInternal(value);
    //    }

    //    private TreeNode GetNodeInternal(TData value)
    //    {
    //        var isLeftNode = false;
    //        var parent = FindParentForValue(value, out isLeftNode);

    //        if (parent == null)
    //        {
    //            return _root;
    //        }

    //        if (isLeftNode && parent.Left.Value.CompareTo(value) == 0)
    //        {
    //            return parent.Left;
    //        }

    //        if (parent.Right.Value.CompareTo(value) == 0)
    //        {
    //            return parent.Right;
    //        }

    //        return null;
    //    }

    //    class TreeNode : INode<TData>, IVisualizerGraphNode
    //    {
    //        public TreeNode(TData value)
    //        {
    //            this.Value = value;
    //            this.NodeType = NodeType.Red;
    //        }

    //        INode<TData> INode<TData>.Left { get { return this.Left; } }
    //        INode<TData> INode<TData>.Right { get { return this.Right; } }
    //        TData INode<TData>.Value { get { return this.Value; } }

    //        public TreeNode Left { get; set; }
    //        public TreeNode Right { get; set; }
    //        public TreeNode Parent { get; set; }

    //        public TData Value { get; set; }
    //        public NodeType NodeType { get; set; }

    //        public TreeNode GrandParent
    //        {
    //            get
    //            {
    //                return this.Parent != null ? this.Parent.Parent : null;
    //            }
    //        }

    //        public TreeNode Uncle
    //        {
    //            get
    //            {
    //                if (this.GrandParent == null)
    //                {
    //                    return null;
    //                }
    //                return this.Parent == this.GrandParent.Left ? this.GrandParent.Right
    //                    : this.GrandParent.Left;
    //            }
    //        }

    //        public TreeNode Sibling
    //        {
    //            get
    //            {
    //                return this.IsLeft ? this.Parent.Right :
    //                    this.IsRight ? this.Parent.Left :
    //                    null;
    //            }
    //        }

    //        public bool IsLeft
    //        {
    //            get
    //            {
    //                return (this.Parent != null) && (this == this.Parent.Left);
    //            }
    //        }

    //        public bool IsRight
    //        {
    //            get
    //            {
    //                return (this.Parent != null) && (this == this.Parent.Right);
    //            }
    //        }

    //        public bool IsLeaf
    //        {
    //            get
    //            {
    //                return (this.Left == null) && (this.Right == null);
    //            }
    //        }

    //        public void ReplaceChild(TreeNode oldNode, TreeNode newNode)
    //        {
    //            if (newNode != null)
    //            {
    //                if (oldNode.IsLeft)
    //                {
    //                    this.Left.Value = newNode.Value;
    //                }
    //                else
    //                {
    //                    this.Right.Value = newNode.Value;
    //                }
    //            }
    //            else
    //            {
    //                if (oldNode.IsLeft)
    //                {
    //                    this.Left = null;
    //                }
    //                else
    //                {
    //                    this.Right = null;
    //                }
    //            }
    //            oldNode.Parent = null;
    //        }

    //        string IVisualizerGraphNode.Tag
    //        {
    //            get { return string.Format("{0} [{1}{2}]", this.Value.ToString(), this.NodeType, this.IsLeaf ? " Leaf" : string.Empty); }
    //        }

    //        IEnumerable<IVisualizerGraphNode> IVisualizerGraphNode.GetConnectedNodes()
    //        {
    //            yield return this.Parent;
    //            yield return this.Left;
    //            yield return this.Right;
    //        }
    //    }

    //    string IVisualizerGraphNode.Tag
    //    {
    //        get { return "tree"; }
    //    }

    //    IEnumerable<IVisualizerGraphNode> IVisualizerGraphNode.GetConnectedNodes()
    //    {
    //        yield return _root;
    //    }

    //    IVisualizerService _visualier;

    //    public void SetVisualizerService(IVisualizerService svc)
    //    {
    //        _visualier = svc;
    //    }
    //}

    //public enum NodeType
    //{
    //    Black,
    //    Red,
    //}

    //public class TestTree : RBTree<string>, IVisualizerCommandsSource
    //{
    //    public TestTree()
    //         : base("0")
    //    {

    //    }

    //    public IVisualizerCommand[] GetCommands()
    //    {
    //        return new[]{
    //            new VisalizerCommand("add", 1, 1, ss => this.AddNode(ss[0])),
    //            new VisalizerCommand("remove", 1, 1, ss => this.DeleteNode(ss[0])),
    //            new VisalizerCommand("get", 1, 1, ss => this.GetNode(ss[0]).Value.ToString()),
    //        };
    //    }
    //}
}
