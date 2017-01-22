using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QuickGraph;

namespace DataStructureVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IVisualizerService
    {
        public ObservableCollection<CollectionTypeInfo> KnownTypes { get; private set; }
        public ObservableCollection<IVisualizerCommand> KnownMethods { get; private set; }
        public ObservableCollection<VisualizerLog> KnownLogs { get; private set; }

        IVisualizerCommandsSource _instance;

        public MainWindow()
        {
            this.KnownTypes = new ObservableCollection<CollectionTypeInfo>();
            this.KnownMethods = new ObservableCollection<IVisualizerCommand>();
            this.KnownLogs = new ObservableCollection<VisualizerLog>();

            InitializeComponent();

            if (Environment.GetCommandLineArgs().Skip(1).Any())
                this.LoadAsm(Environment.GetCommandLineArgs().Skip(1).First());
            else
                this.LoadAsm(typeof(App).Assembly);
        }

        private void G(QuickGraph.IBidirectionalGraph<object, QuickGraph.IEdge<object>> gg)
        {
            throw new NotImplementedException();
        }

        private void btnLoad_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Assembly (*.exe;*.dll)|*.exe;*.dll|Any file|*",
                CheckFileExists = true,
                Multiselect = false
            };

            if (dlg.ShowDialog(this) == true)
            {
                this.LoadAsm(dlg.FileName);
            }
        }

        private void LoadAsm(string filename)
        {
            try
            {
                var asm = System.Reflection.Assembly.LoadFile(filename);
                this.LoadAsm(asm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void LoadAsm(System.Reflection.Assembly asm)
        {
            this.KnownTypes.Clear();

            var list = asm.GetTypes().Where(t => t.GetConstructor(Type.EmptyTypes) != null && t.GetInterfaces().Any(iface => iface == typeof(IVisualizerCommandsSource))).ToList();
            list.ForEach(t => this.KnownTypes.Add(new CollectionTypeInfo(t)));
        }

        private void lstTypes_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var typeInfo = (CollectionTypeInfo)((ListView)sender).SelectedItem;
            _instance = typeInfo.CreateInstance();

            this.KnownMethods.Clear();
            _instance.SetVisualizerService(this);
            _instance.GetCommands().ToList().ForEach(c => this.KnownMethods.Add(c));
        }

        private void lstMethods_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var cmd = (IVisualizerCommand)((ListView)sender).SelectedItem;

            var args = new string[cmd.MinArgsCount];
            for (int i = 0; i < cmd.MinArgsCount; i++)
                args[i] = Microsoft.VisualBasic.Interaction.InputBox(cmd.Name + "[" + i + "]");

            this.KnownLogs.Add(new VisualizerLog(cmd.Name));

            this.TakeSnapshot();
            var result = cmd.Perform(args);
            this.TakeSnapshot();

            if (result != null)
                Microsoft.VisualBasic.Interaction.MsgBox(result.ToString(), Microsoft.VisualBasic.MsgBoxStyle.OkOnly, "Result");
        }

        public void TakeSnapshot()
        {
            var g = new Snapshot(this.KnownLogs.Last().Graphs.Count);

            var nodes = new Dictionary<IVisualizerGraphNode, VisualizerVertex>();
            var edges = new SortedSet<VisualizerEdge>(Comparer<VisualizerEdge>.Default);
            var q = new Queue<IVisualizerGraphNode>();
            q.Enqueue(_instance);

            while (q.Count > 0)
            {
                var node = q.Dequeue();
                if (node != null && !nodes.ContainsKey(node))
                {
                    nodes.Add(node, new VisualizerVertex(node.Tag));
                    var targets = node.GetConnectedNodes().Where(t => t != null).ToList();
                    targets.ForEach(t => q.Enqueue(t));
                }
            }

            foreach (var item in nodes)
            {
                var targets = item.Key.GetConnectedNodes().Where(t => t != null).ToList();
                targets.Select(n => new VisualizerEdge(item.Value, nodes[n])).ToList().ForEach(t => edges.Add(t));
            }

            g.AddVertexRange(nodes.Values);
            g.AddEdgeRange(edges);

            this.KnownLogs.Last().Graphs.Add(g);
        }

        private void tvwTree_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var log = ((TreeView)sender).SelectedItem as Snapshot;
            if (log != null)
                layout.Graph = log;
        }
    }

    public class Cmp : IComparer<VisualizerVertex>
    {
        public int Compare(VisualizerVertex x, VisualizerVertex y)
        {
            return x.Equals(y) ? 0 : 1;
        }
    }

    public class VisualizerEdge : IEdge<object>, IEquatable<VisualizerEdge>, IComparable<VisualizerEdge>, IComparable
    {
        public object Source { get; private set; }
        public object Target { get; private set; }

        public VisualizerEdge(object from, object to)
        {
            this.Source = from;
            this.Target = to;
        }

        public bool Equals(VisualizerEdge other)
        {
            return other != null && this.Source == other.Source && this.Target == other.Target;
        }

        public int CompareTo(VisualizerEdge other)
        {
            return this.Equals(other) ? 0 : 1;
        }

        public int CompareTo(object obj)
        {
            return this.Equals(obj as VisualizerEdge) ? 0 : 1;
        }
    }

    public class VisualizerVertex
    {
        public string Tag { get; private set; }

        public VisualizerVertex(string tag)
        {
            this.Tag = tag;
        }
    }

    public class CollectionTypeInfo
    {
        public Type Type { get; private set; }

        public CollectionTypeInfo(Type t)
        {
            this.Type = t;
        }

        public IVisualizerCommandsSource CreateInstance()
        {
            return (IVisualizerCommandsSource)this.Type.GetConstructor(Type.EmptyTypes).Invoke(null);
        }

        public override string ToString()
        {
            return this.Type.Name;
        }
    }

    public class VisualizerLog
    {
        public ObservableCollection<Snapshot> Graphs { get; private set; }

        public string Name { get; private set; }

        public VisualizerLog(string name)
        {
            this.Name = name;
            this.Graphs = new ObservableCollection<Snapshot>();
        }
    }

    public class Snapshot : BidirectionalGraph<object, IEdge<object>>
    {
        public int Index { get; private set; }

        public Snapshot(int index)
        {
            this.Index = index;
        }
    }
}
