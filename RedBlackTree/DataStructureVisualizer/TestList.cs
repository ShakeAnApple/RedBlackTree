using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStructureVisualizer
{
    class MyList<T> : IVisualizerGraphNode
    {
        class Node : IVisualizerGraphNode
        {
            string IVisualizerGraphNode.Tag { get { return this.Value.ToString(); } }

            public Node Next { get; set; }
            public Node Prev { get; set; }

            public T Value { get; private set; }

            public Node(T value)
            {
                this.Next = null;
                this.Prev = null;
                this.Value = value;
            }

            IEnumerable<IVisualizerGraphNode> IVisualizerGraphNode.GetConnectedNodes()
            {
                yield return this.Next;
                yield return this.Prev;
            }
        }

        Node _first = null, _last = null;
        IEqualityComparer<T> _comparer = EqualityComparer<T>.Default;

        public int Count { get; private set; }

        public MyList()
        {
            this.Count = 0;
        }

        public void AddLast(T value)
        {
            var newNode = new Node(value);

            if (_last == null)
            {
                _first = _last = newNode;
            }
            else
            {
                newNode.Prev = _last;
                _last.Next = newNode;
                _last = newNode;
            }

            this.Count++;
        }

        public void AddFirst(T value)
        {
            var newNode = new Node(value);

            if (_first == null)
            {
                _first = _last = newNode;
            }
            else
            {
                newNode.Next = _first;
                _first.Prev = newNode;
                _first = newNode;
            }

            this.Count++;
        }

        public void RemoveLast()
        {
            if (_last != null)
                _last = _last.Prev;

            this.Count--;
        }

        public void RemoveFirst()
        {
            if (_first != null)
                _first = _first.Next;

            this.Count--;
        }

        public void AddAfter(T value, T newValue)
        {
            var node = _first;
            while (node != null)
            {
                if (_comparer.Equals(node.Value, value))
                    break;

                node = node.Next;
            }

            if (node != null)
                this.AddAfterInternal(node, newValue);
        }

        private void AddAfterInternal(Node prevNode, T newValue)
        {
            var newNode = new Node(newValue);
            var nextNode = prevNode.Next;

            newNode.Prev = prevNode;
            newNode.Next = nextNode;

            prevNode.Next = newNode;
            if (nextNode != null)
                nextNode.Prev = newNode;

            if (prevNode == _last)
                _last = newNode;

            this.Count++;
        }

        public void Remove(T value)
        {
            var node = _first;
            while (node != null)
            {
                if (_comparer.Equals(node.Value, value))
                    break;

                node = node.Next;
            }

            if (node != null)
                this.RemoveInternal(node);
        }

        private void RemoveInternal(Node node)
        {
            if (_first == node)
            {
                _first = node.Next;
            }

            if (_last == node)
            {
                _last = node.Prev;
            }

        }

        string IVisualizerGraphNode.Tag
        {
            get { return "list"; }
        }

        IEnumerable<IVisualizerGraphNode> IVisualizerGraphNode.GetConnectedNodes()
        {
            yield return _first;
            yield return _last;
        }

        public void SetVisualizerService(IVisualizerService svc)
        {
        }
    }

    class TestList : MyList<string>, IVisualizerCommandsSource
    {
        IVisualizerCommand[] IVisualizerCommandsSource.GetCommands()
        {
            return new[] {
                new VisalizerCommand("AddLast", 1, 1, ss => this.AddLast(ss[0])),
                new VisalizerCommand("AddFirst", 1, 1, ss => this.AddFirst(ss[0])),
                new VisalizerCommand("AddAfter", 2, 2, ss => this.AddAfter(ss[0], ss[1])),
                new VisalizerCommand("RemoveFirst", 0, 0, ss => this.RemoveFirst()),
                new VisalizerCommand("RemoveLast", 0, 0, ss => this.RemoveLast())
            };
        }
    }

    public class VisalizerCommand : IVisualizerCommand
    {
        public int MaxArgsCount { get; private set; }
        public int MinArgsCount { get; private set; }
        public string Name { get; private set; }

        readonly Func<string[], string> _act;

        public VisalizerCommand(string name, int min, int max, Action<string[]> act)
            : this(name, min, max, args => { act(args); return null; }) { }

        public VisalizerCommand(string name, int min, int max, Func<string[], string> act)
        {
            this.Name = name;
            this.MinArgsCount = min;
            this.MaxArgsCount = max;
            _act = act;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public string Perform(params string[] args)
        {
            return _act(args);
        }
    }
}
