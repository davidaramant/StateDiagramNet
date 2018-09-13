using System.Collections.Generic;

namespace StateDiagramCodeGen.ParsingModel
{
    public sealed class StateDefinition : IDiagramElement
    {
        public string ShortName { get; }
        public string LongName { get; }
        public IEnumerable<IDiagramElement> Contents { get; }

        public StateDefinition(
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
