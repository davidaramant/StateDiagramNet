using System.Collections.Generic;
using System.Diagnostics;
using Functional.Maybe;

namespace StateDiagramNet.MachineModel
{
    [DebuggerDisplay("{Name}")]
    public sealed class State : IVertex
    {
        public string Name { get; }
        
        private readonly IVertex _parent;
        private readonly List<ActionReference> _entryActions = new List<ActionReference>();
        private readonly List<ActionReference> _exitActions = new List<ActionReference>();

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

        public void AddEntryAction(ActionReference entryAction)
        {
            _entryActions.Add(entryAction);
        }

        public void AddExitAction(ActionReference exitAction)
        {
            _exitActions.Add(exitAction);
        }
    }
}
