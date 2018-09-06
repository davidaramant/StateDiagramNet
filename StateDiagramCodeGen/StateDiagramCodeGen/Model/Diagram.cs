using System.Collections.Generic;

namespace StateDiagramCodeGen.Model
{
    public sealed class Diagram
    {
        public string Name { get; }
        public IEnumerable<IDiagramElement> Components;

        public Diagram(string name, IEnumerable<IDiagramElement> components)
        {
            Name = name;
            Components = components;
        }
    }
}
