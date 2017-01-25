using RedBlackTree.Utils;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using DataStructureVisualizer;

namespace RedBlackTree.Impl
{
    public class RBTree<TData> : IVisualizerGraphNode, IRBTree<TData> where TData : IComparable<TData>
    {
        private TreeNode _root;
        public INode<TData> Root { get { return _root; } }

        private readonly ILogger _logger;

        public RBTree(TData rootValue)
        {
            _root = new TreeNode(rootValue);
            _logger = new ConsoleLogger();
        }

        public RBTree(TData rootValue, ILogger logger)
        {
            _root = new TreeNode(rootValue);
            _logger = logger;
        }

        public INode<TData> AddNode(TData value)
        {
            var newNode = new TreeNode(value);
            AddNodeInternal(newNode);
            if (newNode.Parent == null)
            {
                _root = newNode;
            }
            return newNode;
        }

        private TreeNode AddNodeInternal(TreeNode newNode)
        {
            var isLeftNode = false;
            var parent = FindParentForValueOrNode(newNode.Value, out isLeftNode);
            if (isLeftNode)
            {
                parent.Left = newNode;
            }
            else
            {
                parent.Right = newNode;
            }
            newNode.Parent = parent;
            this.TakeVisualizerSnapshot();

            RebalanceInsert(newNode);
            return newNode;
        }

        private void RebalanceInsert(TreeNode node)
        {
            if (node.Parent == null)
            {
                node.NodeType = NodeType.Black;
            }
            else if (node.Parent.NodeType == NodeType.Black)
            {
                return;
            }
            else if (node.Uncle != null &&
                node.Uncle.NodeType == NodeType.Red)
            {
                node.Parent.NodeType = NodeType.Black;
                node.Uncle.NodeType = NodeType.Black;
                node.GrandParent.NodeType = NodeType.Red;
                this.TakeVisualizerSnapshot();

                RebalanceInsert(node.GrandParent);
            }
            else
            {
                if (node.IsRight && node.Parent.IsLeft)
                {
                    RotateLeft(node.Parent);
                    node = node.Left;
                }
                else if (node.IsLeft && node.Parent.IsRight)
                {
                    RotateRight(node.Parent);
                    node = node.Right;
                }
                this.TakeVisualizerSnapshot();

                node.Parent.NodeType = NodeType.Black;

                if (node.GrandParent == null)
                {
                    return;
                }

                node.GrandParent.NodeType = NodeType.Red;
                this.TakeVisualizerSnapshot();

                if (node.IsLeft && node.Parent.IsLeft)
                {
                    RotateRight(node.GrandParent);
                }
                else
                {
                    RotateLeft(node.GrandParent);
                }
            }
        }

        public void DeleteNode(TData value)
        {
            var nodeToDelete = GetNodeInternal(value);

            if (nodeToDelete == null)
            {
                _logger.Log("Such value dosn't exist");
                return;
            }

            if (nodeToDelete.IsLeaf)
            {
                nodeToDelete.Parent.ReplaceChildValue(nodeToDelete, null);
                return;
            }

            var child = nodeToDelete.Right == null ? nodeToDelete.Left
                            : nodeToDelete.Left == null ? nodeToDelete.Right
                        : null;

            if (child != null)
            {
                nodeToDelete.Parent.ReplaceChildValue(nodeToDelete, child);
                this.TakeVisualizerSnapshot();

                RebalanceDelete(child);
                return;
            }

            var nextNode = nodeToDelete.Right;
            while (nextNode.Left != null) nextNode = nextNode.Left;

            nextNode.Parent.ReplaceChildValue(nextNode, nextNode.Right);
            nextNode.Right = null;

            nodeToDelete.Parent.ReplaceChildValue(nodeToDelete, nextNode);
            this.TakeVisualizerSnapshot();

            RebalanceDelete(nextNode);
        }

        private void RebalanceDelete(TreeNode node)
        {
            if (node.Parent == null)
            {
                return;
            }

            if (node.Sibling != null && node.Sibling.NodeType == NodeType.Red)
            {
                node.Parent.NodeType = NodeType.Red;
                node.Sibling.NodeType = NodeType.Black;

                if (node.IsLeft)
                {
                    RotateLeft(node.Parent);
                }
                else
                {
                    RotateRight(node.Parent);
                }
            }
            this.TakeVisualizerSnapshot();

            if (node.Sibling != null && node.Parent.NodeType == NodeType.Black &&
            node.Sibling.NodeType == NodeType.Black &&
            node.Sibling.Left.NodeType == NodeType.Black &&
            node.Sibling.Right.NodeType == NodeType.Black)
            {
                node.Sibling.NodeType = NodeType.Red;
                RebalanceDelete(node.Parent);
            }
            else if (node.Sibling != null && node.Parent.NodeType == NodeType.Red &&
                node.Sibling.NodeType == NodeType.Black &&
            node.Sibling.Left.NodeType == NodeType.Black &&
            node.Sibling.Right.NodeType == NodeType.Black)
            {

                node.Sibling.NodeType = NodeType.Red;
                node.Parent.NodeType = NodeType.Black;
            }
            else
            {
                if (node.Sibling != null && node.Sibling.NodeType == NodeType.Black)
                {
                    if (node.IsLeft &&
                        node.Sibling.Right.NodeType == NodeType.Black &&
                        node.Sibling.Left.NodeType == NodeType.Red)
                    {
                        node.Sibling.NodeType = NodeType.Red;
                        node.Sibling.Left.NodeType = NodeType.Black;

                        RotateRight(node.Sibling);
                    }
                    else if (node.IsRight &&
                                node.Sibling.Right.NodeType == NodeType.Black &&
                                node.Sibling.Left.NodeType == NodeType.Red)
                    {
                        node.Sibling.NodeType = NodeType.Red;
                        node.Sibling.Right.NodeType = NodeType.Black;

                        RotateLeft(node.Sibling);
                    }
                }
                this.TakeVisualizerSnapshot();

                if (node.Sibling != null)
                {
                    node.Sibling.NodeType = node.Parent.NodeType;
                }
                node.Parent.NodeType = NodeType.Black;
                this.TakeVisualizerSnapshot();

                if (node.IsLeft)
                {
                    if (node.Sibling != null)
                    {
                        node.Sibling.Right.NodeType = NodeType.Black;
                    }

                    RotateLeft(node.Parent);
                }
                else
                {
                    if (node.Sibling != null)
                    {
                        node.Sibling.Left.NodeType = NodeType.Black;
                    }

                    RotateRight(node.Parent);
                }
            }
        }

        public INode<TData> GetNode(TData value)
        {
            return GetNodeInternal(value);
        }

        private TreeNode GetNodeInternal(TData value)
        {
            var isLeftNode = false;
            return FindParentForValueOrNode(value, out isLeftNode);

            //if (parent == null)
            //{
            //    return _root;
            //}

            //if (isLeftNode && parent.Left.Value.CompareTo(value) == 0)
            //{
            //    return parent.Left;
            //}

            //if (parent.Right.Value.CompareTo(value) == 0)
            //{
            //    return parent.Right;
            //}

            //return null;
        }

        private TreeNode FindParentForValueOrNode(TData nodeValue, out bool isLeftNode)
        {
            TreeNode parent = null;
            isLeftNode = false;

            var prevNode = _root;
            while (parent == null)
            {
                if (nodeValue.CompareTo(prevNode.Value) < 0)
                {
                    if (prevNode.Left == null)
                    {
                        parent = prevNode;
                        isLeftNode = true;
                    }
                    prevNode = prevNode.Left;
                }
                else if (nodeValue.CompareTo(prevNode.Value) > 0)
                {
                    if (prevNode.Right == null)
                    {
                        parent = prevNode;
                        isLeftNode = false;
                    }
                    prevNode = prevNode.Right;
                }
                else
                {
                    return prevNode;
                }
            }
            return parent;
        }

        private void RotateLeft(TreeNode node)
        {
            var pivot = node.Right;
            pivot.Parent = node.Parent;
            if (node.Parent != null)
            {
                if (node.IsLeft)
                {
                    node.Parent.Left = pivot;
                }
                else
                {
                    node.Parent.Right = pivot;
                }
            }
            else
            {
                _root = pivot;
            }

            node.Right = pivot.Left;
            if (pivot.Left != null)
            {
                pivot.Left.Parent = node;
            }

            node.Parent = pivot;
            pivot.Left = node;
        }

        private void RotateRight(TreeNode node)
        {
            var pivot = node.Left;

            pivot.Parent = node.Parent;
            if (node.Parent != null)
            {
                if (node.IsLeft)
                {
                    node.Parent.Left = pivot;
                }
                else
                {
                    node.Parent.Right = pivot;
                }
            }
            else
            {
                _root = pivot;
            }

            node.Left = pivot.Right;
            if (pivot.Right != null)
            {
                pivot.Right.Parent = node;
            }

            node.Parent = pivot;
            pivot.Right = node;
        }

        class TreeNode : IVisualizerGraphNode, INode<TData>
        {
            public TreeNode(TData value)
            {
                this.Value = value;
                this.NodeType = NodeType.Red;
            }

            public event PropertyChangedEventHandler PropertyChanged = delegate { };

            INode<TData> INode<TData>.Left { get { return this.Left; } }
            INode<TData> INode<TData>.Right { get { return this.Right; } }
            TData INode<TData>.Value { get { return this.Value; } }

            public TreeNode Left { get; set; }
            public TreeNode Right { get; set; }
            public TreeNode Parent { get; set; }

            public TData Value { get; set; }
            public NodeType NodeType { get; set; }

            public TreeNode GrandParent
            {
                get
                {
                    return this.Parent != null ? this.Parent.Parent : null;
                }
            }

            public TreeNode Uncle
            {
                get
                {
                    if (this.GrandParent == null)
                    {
                        return null;
                    }
                    return this.Parent == this.GrandParent.Left ? this.GrandParent.Right
                        : this.GrandParent.Left;
                }
            }

            public TreeNode Sibling
            {
                get
                {
                    return this.IsLeft ? this.Parent.Right :
                        this.IsRight ? this.Parent.Left :
                        null;
                }
            }

            public bool IsLeft
            {
                get
                {
                    return (this.Parent != null) && (this == this.Parent.Left);
                }
            }

            public bool IsRight
            {
                get
                {
                    return (this.Parent != null) && (this == this.Parent.Right);
                }
            }

            public bool IsLeaf
            {
                get
                {
                    return (this.Left == null) && (this.Right == null);
                }
            }

            public void ReplaceChildValue(TreeNode oldNode, TreeNode newNode)
            {
                if (newNode != null)
                {
                    if (oldNode.IsLeft)
                    {
                        this.Left.Value = newNode.Value;
                    }
                    else
                    {
                        this.Right.Value = newNode.Value;
                    }
                }
                else
                {
                    if (oldNode.IsLeft)
                    {
                        this.Left = null;
                    }
                    else
                    {
                        this.Right = null;
                    }
                }
            }

            string IVisualizerGraphNode.Tag
            {
                get
                {
                    return string.Format(
                        "{0} [{1}{2}{3}]",
                        this.Value.ToString(),
                        this.IsLeft ? "left " : (this.IsRight ? "right " : "wtf "),
                        this.NodeType,
                        this.IsLeaf ? " Leaf" : string.Empty
                    );
                }
            }

            IEnumerable<IVisualizerGraphNode> IVisualizerGraphNode.GetConnectedNodes()
            {
                yield return this.Parent;
                yield return this.Left;
                yield return this.Right;
            }
        }

        string IVisualizerGraphNode.Tag
        {
            get { return "tree"; }
        }

        IEnumerable<IVisualizerGraphNode> IVisualizerGraphNode.GetConnectedNodes()
        {
            yield return _root;
        }

        IVisualizerService _visualizer;

        public void SetVisualizerService(IVisualizerService svc)
        {
            _visualizer = svc;
        }

        private void TakeVisualizerSnapshot()
        {
            if (_visualizer != null)
                _visualizer.TakeSnapshot();
        }
    }

    public enum NodeType
    {
        Red,
        Black
    }

    public class TestTree : RBTree<int>, IVisualizerCommandsSource
    {
        public TestTree()
             : base(10)
        {
            base.AddNode(50);
            base.AddNode(9);
            base.AddNode(8);
            base.AddNode(7);
            base.AddNode(1);
            base.AddNode(3);
            base.AddNode(4);
        }

        public IVisualizerCommand[] GetCommands()
        {
            return new[]{
                new VisalizerCommand("add", 1, 1, ss => this.AddNode(int.Parse(ss[0]))),
                new VisalizerCommand("remove", 1, 1, ss => this.DeleteNode(int.Parse(ss[0]))),
                new VisalizerCommand("get", 1, 1, ss => this.GetNode(int.Parse(ss[0])).Value.ToString()),
            };
        }
    }
}
