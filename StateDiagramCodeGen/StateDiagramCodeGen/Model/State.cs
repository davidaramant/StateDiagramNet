using System.Collections.Generic;

namespace StateDiagramCodeGen.Model
{
    public sealed class State : IDiagramElement
    {
        public string ShortName { get; }
        public string LongName { get; }
        public IEnumerable<IDiagramElement> Contents { get; }

        public State(
            string shortName, 
            string longName,
            IEnumerable<IDiagramElement> contents)
        {
            ShortName = shortName;
            LongName = longName;
            Contents = contents;
        }
    }
}
