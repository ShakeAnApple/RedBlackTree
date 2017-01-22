using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStructureVisualizer
{
    public interface IVisualizerService
    {
        void TakeSnapshot();
    }

    public interface IVisualizerGraphNode
    {
        string Tag { get; }
        IEnumerable<IVisualizerGraphNode> GetConnectedNodes();        
    }

    public interface IVisualizerCommandsSource : IVisualizerGraphNode
    {
        void SetVisualizerService(IVisualizerService svc);

        IVisualizerCommand[] GetCommands();
    }

    public interface IVisualizerCommand
    {
        string Name { get; }
        int MaxArgsCount { get; }
        int MinArgsCount { get; }
        string Perform(params string[] args);
    }
}
