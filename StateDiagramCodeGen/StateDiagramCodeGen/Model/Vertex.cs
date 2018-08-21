using System.Collections.Generic;

namespace StateDiagramCodeGen.Model
{
    public enum VertexType
    {
        State,
        Choice,
        EntryPoint,
        ExitPoint
    }

    public sealed class Vertex
    {
        public VertexType Type { get; set; } = VertexType.State;
        public string Name { get; }
        public string Description { get; set; } = string.Empty;
        public List<string> EntryActions { get; } = new List<string>();
        public List<string> ExitActions { get; } = new List<string>();

        public Vertex(string name)
        {
            Name = name;
        }
    }
}
