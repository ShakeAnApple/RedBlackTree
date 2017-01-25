using RedBlackTree.Utils;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using DataStructureVisualizer;

namespace RedBlackTree.Impl
{
    public class PersistentRBTree<TData> : IVisualizerGraphNode // , IRBTree<TData> 
        where TData : IComparable<TData>
    {
        abstract class TreeNodeStateInfo
        {
            public abstract TreeNodeStateContainer Left { get; }
            public abstract TreeNodeStateContainer Right { get; }
            public abstract TreeNodeStateContainer Parent { get; }

            public abstract TData Value { get; }
            public abstract NodeType NodeType { get; }

            public abstract int StatesListDepth { get; }
            public abstract int VersionsDepth { get; }

            public int Version { get; private set; }

            public TreeNodeStateInfo(int version) { this.Version = version; }

            public TreeNodeStateInfo ChangeLeft(int version, TreeNodeStateContainer newLeft)
            {
                return new TreeNodeLeftChangedInfo(this, version, newLeft);
            }

            public TreeNodeStateInfo ChangeRight(int version, TreeNodeStateContainer newRight)
            {
                return new TreeNodeRightChangedInfo(this, version, newRight);
            }

            public TreeNodeStateInfo ChangeParent(int version, TreeNodeStateContainer newParent)
            {
                return new TreeNodeParentChangedInfo(this, version, newParent);
            }

            public TreeNodeStateInfo ChangeNodeType(int version, NodeType newType)
            {
                return new TreeNodeNodeTypeChangedInfo(this, version, newType);
            }

            public TreeNodeStateInfo ChangeValue(int version, TData value)
            {
                return new TreeNodeValueChangedInfo(this, version, value);
            }

            public TreeNodeStateInfo CollectFullState()
            {
                return new TreeNodeFullStateInfo(
                    this.Version,
                    this.Left,
                    this.Right,
                    this.Parent,
                    this.Value,
                    this.NodeType
                );
            }

            public abstract TreeNodeStateInfo GetState(int version);
        }

        sealed class TreeNodeFullStateInfo : TreeNodeStateInfo
        {
            TreeNodeStateContainer _left, _right, _parent;
            TData _value;
            NodeType _nodeType;

            public override TreeNodeStateContainer Left { get { return _left; } }
            public override TreeNodeStateContainer Right { get { return _right; } }
            public override TreeNodeStateContainer Parent { get { return _parent; } }

            public override TData Value { get { return _value; } }
            public override NodeType NodeType { get { return _nodeType; } }

            public override int StatesListDepth { get { return 0; } }
            public override int VersionsDepth { get { return 0; } }

            public TreeNodeFullStateInfo(int version, TreeNodeStateContainer left, TreeNodeStateContainer right, TreeNodeStateContainer parent, TData value, NodeType nodeType)
                : base(version)
            {
                _left = left;
                _right = right;
                _parent = parent;
                _value = value;
                _nodeType = nodeType;
            }

            public override TreeNodeStateInfo GetState(int version)
            {
                if (version < this.Version)
                    throw new ApplicationException();

                return this;
            }
        }

        abstract class TreeNodeChangedStateInfo : TreeNodeStateInfo
        {
            TreeNodeStateInfo _prevState;

            public override TreeNodeStateContainer Left { get { return _prevState.Left; } }
            public override TreeNodeStateContainer Right { get { return _prevState.Right; } }
            public override TreeNodeStateContainer Parent { get { return _prevState.Parent; } }

            public override TData Value { get { return _prevState.Value; } }
            public override NodeType NodeType { get { return _prevState.NodeType; } }

            public override int StatesListDepth { get { return _prevState.StatesListDepth + 1; } }
            public override int VersionsDepth { get { return _prevState.VersionsDepth + (_prevState.Version == this.Version ? 0 : 1); } }

            public TreeNodeChangedStateInfo(TreeNodeStateInfo prevState, int version)
                : base(version)
            {
                _prevState = prevState;
            }

            public override TreeNodeStateInfo GetState(int version)
            {
                if (version < this.Version)
                    return _prevState.GetState(version);

                return this;
            }
        }

        sealed class TreeNodeLeftChangedInfo : TreeNodeChangedStateInfo
        {
            TreeNodeStateContainer _left;

            public override TreeNodeStateContainer Left { get { return _left; } }

            public TreeNodeLeftChangedInfo(TreeNodeStateInfo prevState, int version, TreeNodeStateContainer left)
                : base(prevState, version)
            {
                _left = left;
            }
        }

        sealed class TreeNodeRightChangedInfo : TreeNodeChangedStateInfo
        {
            TreeNodeStateContainer _right;

            public override TreeNodeStateContainer Right { get { return _right; } }

            public TreeNodeRightChangedInfo(TreeNodeStateInfo prevState, int version, TreeNodeStateContainer right)
                : base(prevState, version)
            {
                _right = right;
            }
        }

        sealed class TreeNodeParentChangedInfo : TreeNodeChangedStateInfo
        {
            TreeNodeStateContainer _parent;

            public override TreeNodeStateContainer Parent { get { return _parent; } }

            public TreeNodeParentChangedInfo(TreeNodeStateInfo prevState, int version, TreeNodeStateContainer parent)
                : base(prevState, version)
            {
                _parent = parent;
            }
        }

        sealed class TreeNodeValueChangedInfo : TreeNodeChangedStateInfo
        {
            TData _value;

            public override TData Value { get { return _value; } }

            public TreeNodeValueChangedInfo(TreeNodeStateInfo prevState, int version, TData value)
                : base(prevState, version)
            {
                _value = value;
            }
        }

        sealed class TreeNodeNodeTypeChangedInfo : TreeNodeChangedStateInfo
        {
            NodeType _nodeType;

            public override NodeType NodeType { get { return _nodeType; } }

            public TreeNodeNodeTypeChangedInfo(TreeNodeStateInfo prevState, int version, NodeType nodeType)
                : base(prevState, version)
            {
                _nodeType = nodeType;
            }
        }

        class TreeNodeStateContainer
        {
            int _versionFrom;
            TreeNodeStateContainer _prev;
            TreeNodeStateInfo _currentState;

            public TreeNodeStateContainer(int version, TData value)
            {
                _versionFrom = version;
                _prev = null;
                _currentState = new TreeNodeFullStateInfo(version, null, null, null, value, NodeType.Red);
            }

            private TreeNodeStateContainer(TreeNodeStateContainer prev, TreeNodeStateInfo state)
            {
                _prev = prev;
                _versionFrom = state.Version;
                _currentState = state;
            }

            public TreeNodeStateContainer AppendChangedStates(TreeNodeStateInfo stateInfo)
            {
                if (stateInfo.Version < _versionFrom || stateInfo.Version < _currentState.Version)
                    throw new ApplicationException();

                _currentState = stateInfo;

                if (_currentState.VersionsDepth > 10)
                {
                    return new TreeNodeStateContainer(this, stateInfo);
                }
                else
                {
                    return this;
                }
            }

            public TreeNodeStateInfo GetState(int version)
            {
                TreeNodeStateInfo state;

                if (_versionFrom <= version)
                {
                    state = _currentState.GetState(version);
                }
                else if (_prev != null)
                {
                    state = _prev.GetState(version);
                }
                else
                {
                    throw new ApplicationException();
                }

                return state;
            }
        }

        class ActiveTreeNode : INode<TData>, IVisualizerGraphNode
        {
            readonly int _version;

            TreeNodeStateContainer _node;
            TreeNodeStateInfo _state;

            public ActiveTreeNode(TreeNodeStateContainer node, int version)
            {
                _node = node;
                _version = version;
                _state = node.GetState(version);
            }

            INode<TData> INode<TData>.Left { get { return this.Left; } }
            INode<TData> INode<TData>.Right { get { return this.Right; } }
            TData INode<TData>.Value { get { return this.Value; } }

            ActiveTreeNode _left = null, _right = null, _parent = null;
            public ActiveTreeNode Left { get { return _left ?? (_state.Left == null ? null : (_left = new ActiveTreeNode(_state.Left, _version) { _parent = this })); } }
            public ActiveTreeNode Right { get { return _right ?? (_state.Right == null ? null : (_right = new ActiveTreeNode(_state.Right, _version) { _parent = this })); } }
            public ActiveTreeNode Parent { get { return _parent ?? (_state.Parent == null ? null : (_parent = new ActiveTreeNode(_state.Parent, _version))); } }

            public TData Value { get { return _state.Value; } }
            public NodeType NodeType { get { return _state.NodeType; } }

            public ActiveTreeNode GrandParent
            {
                get
                {
                    return _state.Parent != null ? this.Parent.Parent : null;
                }
            }

            public ActiveTreeNode Uncle
            {
                get
                {
                    if (this.GrandParent == null)
                        return null;

                    return this.Parent._state == this.GrandParent.Left._state ? this.GrandParent.Right : this.GrandParent.Left;
                }
            }

            public ActiveTreeNode Sibling
            {
                get
                {
                    return this.IsLeft ? this.Parent.Right : (
                        this.IsRight ? this.Parent.Left : null
                    );
                }
            }

            public bool IsLeft
            {
                get
                {
                    return (_state.Parent != null) && (_state == this.Parent.Left._state);
                }
            }

            public bool IsRight
            {
                get
                {
                    return (_state.Parent != null) && (_state == this.Parent.Right._state);
                }
            }

            public bool IsLeaf
            {
                get
                {
                    return (_state.Left == null) && (_state.Right == null);
                }
            }

            //public void ReplaceChildValue(TreeNode oldNode, TreeNode newNode)
            //{
            //    if (newNode != null)
            //    {
            //        if (oldNode.IsLeft)
            //        {
            //            this.Left.Value = newNode.Value;
            //        }
            //        else
            //        {
            //            this.Right.Value = newNode.Value;
            //        }
            //    }
            //    else
            //    {
            //        if (oldNode.IsLeft)
            //        {
            //            this.Left = null;
            //        }
            //        else
            //        {
            //            this.Right = null;
            //        }
            //    }
            //}

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

            public TreeNodeStateContainer GetStateContainer()
            {
                return _node;
            }

            public ActiveTreeNode CreateLeftChild(TData value)
            {
                if (_state.Left != null)
                    throw new ApplicationException();

                var newNode = new TreeNodeStateContainer(_version, value);
                _node = _node.AppendChangedStates(_state.ChangeLeft(_version, newNode));
                _state = _node.GetState(_version);
                newNode = newNode.AppendChangedStates(newNode.GetState(_version).ChangeParent(_version, _node));
                return _left = new ActiveTreeNode(newNode, _version);
            }

            public ActiveTreeNode CreateRightChild(TData value)
            {
                throw new NotImplementedException();
            }
        }

        private TreeNodeStateContainer _root;
        private int _version;

        private readonly ILogger _logger;

        public PersistentRBTree(TData rootValue)
        {
            _root = new TreeNodeStateContainer(0, rootValue);
            _version = 0;
            _logger = new ConsoleLogger();
        }

        private ActiveTreeNode GetCurrentRoot()
        {
            return new ActiveTreeNode(_root, _version);
        }

        private ActiveTreeNode GetRoot(int version)
        {
            return new ActiveTreeNode(_root, version);
        }

        private ActiveTreeNode GetNewRoot()
        {
            _version++;
            return new ActiveTreeNode(_root, _version);
        }

        public INode<TData> AddNode(TData value)
        {
            var newNode = AddNodeInternal(value);
            if (newNode.Parent == null)
            {
                _root = newNode.GetStateContainer();
            }
            return newNode;
        }

        private ActiveTreeNode AddNodeInternal(TData value)
        {
            bool isLeftNode;
            ActiveTreeNode newNode;
            var parent = FindParentForValueOrNode(value, out isLeftNode);
            if (isLeftNode)
            {
                newNode = parent.CreateLeftChild(value);
            }
            else
            {
                newNode = parent.CreateRightChild(value);
            }
            this.TakeVisualizerSnapshot();

            RebalanceInsert(newNode);
            return newNode;
        }

        //private void RebalanceInsert(TreeNode node)
        //{
        //    if (node.Parent == null)
        //    {
        //        node.NodeType = NodeType.Black;
        //    }
        //    else if (node.Parent.NodeType == NodeType.Black)
        //    {
        //        return;
        //    }
        //    else if (node.Uncle != null &&
        //        node.Uncle.NodeType == NodeType.Red)
        //    {
        //        node.Parent.NodeType = NodeType.Black;
        //        node.Uncle.NodeType = NodeType.Black;
        //        node.GrandParent.NodeType = NodeType.Red;
        //        this.TakeVisualizerSnapshot();

        //        RebalanceInsert(node.GrandParent);
        //    }
        //    else
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
        //        this.TakeVisualizerSnapshot();

        //        node.Parent.NodeType = NodeType.Black;

        //        if (node.GrandParent == null)
        //        {
        //            return;
        //        }

        //        node.GrandParent.NodeType = NodeType.Red;
        //        this.TakeVisualizerSnapshot();

        //        if (node.IsLeft && node.Parent.IsLeft)
        //        {
        //            RotateRight(node.GrandParent);
        //        }
        //        else
        //        {
        //            RotateLeft(node.GrandParent);
        //        }
        //    }
        //}

        //public void DeleteNode(TData value)
        //{
        //    var nodeToDelete = GetNodeInternal(value);

        //    if (nodeToDelete == null)
        //    {
        //        _logger.Log("Such value dosn't exist");
        //        return;
        //    }

        //    if (nodeToDelete.IsLeaf)
        //    {
        //        nodeToDelete.Parent.ReplaceChildValue(nodeToDelete, null);
        //        return;
        //    }

        //    var child = nodeToDelete.Right == null ? nodeToDelete.Left
        //                    : nodeToDelete.Left == null ? nodeToDelete.Right
        //                : null;

        //    if (child != null)
        //    {
        //        nodeToDelete.Parent.ReplaceChildValue(nodeToDelete, child);
        //        this.TakeVisualizerSnapshot();

        //        RebalanceDelete(child);
        //        return;
        //    }

        //    var nextNode = nodeToDelete.Right;
        //    while (nextNode.Left != null) nextNode = nextNode.Left;

        //    nextNode.Parent.ReplaceChildValue(nextNode, nextNode.Right);
        //    nextNode.Right = null;

        //    nodeToDelete.Parent.ReplaceChildValue(nodeToDelete, nextNode);
        //    this.TakeVisualizerSnapshot();

        //    RebalanceDelete(nextNode);
        //}

        //private void RebalanceDelete(TreeNode node)
        //{
        //    if (node.Parent == null)
        //    {
        //        return;
        //    }

        //    if (node.Sibling != null && node.Sibling.NodeType == NodeType.Red)
        //    {
        //        node.Parent.NodeType = NodeType.Red;
        //        node.Sibling.NodeType = NodeType.Black;

        //        if (node.IsLeft)
        //        {
        //            RotateLeft(node.Parent);
        //        }
        //        else
        //        {
        //            RotateRight(node.Parent);
        //        }
        //    }
        //    this.TakeVisualizerSnapshot();

        //    if (node.Sibling != null && node.Parent.NodeType == NodeType.Black &&
        //    node.Sibling.NodeType == NodeType.Black &&
        //    node.Sibling.Left.NodeType == NodeType.Black &&
        //    node.Sibling.Right.NodeType == NodeType.Black)
        //    {
        //        node.Sibling.NodeType = NodeType.Red;
        //        RebalanceDelete(node.Parent);
        //    }
        //    else if (node.Sibling != null && node.Parent.NodeType == NodeType.Red &&
        //        node.Sibling.NodeType == NodeType.Black &&
        //    node.Sibling.Left.NodeType == NodeType.Black &&
        //    node.Sibling.Right.NodeType == NodeType.Black)
        //    {

        //        node.Sibling.NodeType = NodeType.Red;
        //        node.Parent.NodeType = NodeType.Black;
        //    }
        //    else
        //    {
        //        if (node.Sibling != null && node.Sibling.NodeType == NodeType.Black)
        //        {
        //            if (node.IsLeft &&
        //                node.Sibling.Right.NodeType == NodeType.Black &&
        //                node.Sibling.Left.NodeType == NodeType.Red)
        //            {
        //                node.Sibling.NodeType = NodeType.Red;
        //                node.Sibling.Left.NodeType = NodeType.Black;

        //                RotateRight(node.Sibling);
        //            }
        //            else if (node.IsRight &&
        //                        node.Sibling.Right.NodeType == NodeType.Black &&
        //                        node.Sibling.Left.NodeType == NodeType.Red)
        //            {
        //                node.Sibling.NodeType = NodeType.Red;
        //                node.Sibling.Right.NodeType = NodeType.Black;

        //                RotateLeft(node.Sibling);
        //            }
        //        }
        //        this.TakeVisualizerSnapshot();

        //        if (node.Sibling != null)
        //        {
        //            node.Sibling.NodeType = node.Parent.NodeType;
        //        }
        //        node.Parent.NodeType = NodeType.Black;
        //        this.TakeVisualizerSnapshot();

        //        if (node.IsLeft)
        //        {
        //            if (node.Sibling != null)
        //            {
        //                node.Sibling.Right.NodeType = NodeType.Black;
        //            }

        //            RotateLeft(node.Parent);
        //        }
        //        else
        //        {
        //            if (node.Sibling != null)
        //            {
        //                node.Sibling.Left.NodeType = NodeType.Black;
        //            }

        //            RotateRight(node.Parent);
        //        }
        //    }
        //}

        //public INode<TData> GetNode(TData value)
        //{
        //    return GetNodeInternal(value);
        //}

        //private TreeNode GetNodeInternal(TData value)
        //{
        //    var isLeftNode = false;
        //    return FindParentForValueOrNode(value, out isLeftNode);

        //    //if (parent == null)
        //    //{
        //    //    return _root;
        //    //}

        //    //if (isLeftNode && parent.Left.Value.CompareTo(value) == 0)
        //    //{
        //    //    return parent.Left;
        //    //}

        //    //if (parent.Right.Value.CompareTo(value) == 0)
        //    //{
        //    //    return parent.Right;
        //    //}

        //    //return null;
        //}

        private ActiveTreeNode FindParentForValueOrNode(TData nodeValue, out bool isLeftNode)
        {
            ActiveTreeNode parent = null;
            isLeftNode = false;

            var prevNode = this.GetCurrentRoot();
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

        //private void RotateLeft(TreeNode node)
        //{
        //    var pivot = node.Right;
        //    pivot.Parent = node.Parent;
        //    if (node.Parent != null)
        //    {
        //        if (node.IsLeft)
        //        {
        //            node.Parent.Left = pivot;
        //        }
        //        else
        //        {
        //            node.Parent.Right = pivot;
        //        }
        //    }
        //    else
        //    {
        //        _root = pivot;
        //    }

        //    node.Right = pivot.Left;
        //    if (pivot.Left != null)
        //    {
        //        pivot.Left.Parent = node;
        //    }

        //    node.Parent = pivot;
        //    pivot.Left = node;
        //}

        //private void RotateRight(TreeNode node)
        //{
        //    var pivot = node.Left;

        //    pivot.Parent = node.Parent;
        //    if (node.Parent != null)
        //    {
        //        if (node.IsLeft)
        //        {
        //            node.Parent.Left = pivot;
        //        }
        //        else
        //        {
        //            node.Parent.Right = pivot;
        //        }
        //    }
        //    else
        //    {
        //        _root = pivot;
        //    }

        //    node.Left = pivot.Right;
        //    if (pivot.Right != null)
        //    {
        //        pivot.Right.Parent = node;
        //    }

        //    node.Parent = pivot;
        //    pivot.Right = node;
        //}

        #region 4visualizer

        string IVisualizerGraphNode.Tag
        {
            get { return "tree"; }
        }

        IEnumerable<IVisualizerGraphNode> IVisualizerGraphNode.GetConnectedNodes()
        {
            yield return this.GetCurrentRoot();
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

        #endregion
    }

    public class PersistentTestTree : PersistentRBTree<int>, IVisualizerCommandsSource
    {
        public PersistentTestTree()
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
