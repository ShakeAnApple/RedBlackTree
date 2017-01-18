using System;

namespace RedBlackTree
{
    public class RBTree<TData> : IRBTree<TData> where TData : IComparable<TData>
    {
        private TreeNode _root;
        public INode<TData> Root { get { return _root; } }

        public RBTree(TData rootValue)
        {
            this.AddNode(rootValue);
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
            // find a place for the node
            var isLeftNode = false;
            var parent = FindParent(newNode.Value, out isLeftNode);
            if (isLeftNode)
            {
                parent.Left = newNode;
            }
            else
            {
                parent.Right = newNode;
            }
            newNode.Parent = parent;

            Rebalance(newNode);
        }

        private void Rebalance(TreeNode newNode)
        {
            if (newNode.Parent == null)
            {
                newNode.NodeType = NodeType.Black;
            }


        }

        private TreeNode FindParent(TData nodeValue, out bool isLeftNode)
        {
            TreeNode parent = null;
            isLeftNode = false;

            var prevNode = _root;
            while (parent == null)
            {
                if (nodeValue.CompareTo(prevNode.Value) == 0)
                {
                    throw new ArgumentException("Equal nodes are not allowed");
                }

                if (nodeValue.CompareTo(prevNode.Value) < 0)
                {
                    if (prevNode.Left == null)
                    {
                        parent = prevNode;
                        isLeftNode = true;
                    }
                    prevNode = prevNode.Left;
                }
                else
                {
                    if (prevNode.Right == null)
                    {
                        parent = prevNode;
                        isLeftNode = false;
                    }
                    prevNode = prevNode.Right;
                }
            }
            return parent;
        }

        public void DeleteNode(TData value)
        {
            var isLeftNode = false;
            FindParent(value, out isLeftNode);

            throw new NotImplementedException();
        }

        public INode<TData> GetNode(TData value)
        {
            /// TODO not work
            var isLeftNode = false;
            var parent = FindParent(value, out isLeftNode);
            return isLeftNode ? parent.Left : parent.Right;
        }

        class TreeNode : INode<TData>
        {
            public TreeNode(TData value)
            {
                this.Value = value;
                this.NodeType = NodeType.Red;
            }

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
                    if (this.Parent == this.GrandParent.Left)
                    {
                        return this.GrandParent.Right;
                    }
                    return this.GrandParent.Left;
                }
            }

            public void SetNodeType(NodeType nodeType)
            {
                throw new NotImplementedException();
            }
        }
    }

    public enum NodeType
    {
        Red,
        Black
    }
}
