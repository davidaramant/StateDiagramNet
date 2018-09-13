using System.Collections.Generic;

namespace StateDiagramCodeGen.MachineModel
{
    public sealed class State : IVertex
    {
        public string Name { get; }
        
        private readonly IVertex _parent;
        private readonly List<IVertex> _children = new List<IVertex>();

        public State(IVertex parent, string name)
        {
            _parent = parent;
            Name = name;
        }

        public void AddChild(IVertex child)
        {
            _children.Add(child);
        }
    }
}
