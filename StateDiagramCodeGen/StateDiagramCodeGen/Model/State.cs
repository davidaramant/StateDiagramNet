using System.Collections.Generic;

namespace StateDiagramCodeGen.Model
{
    public sealed class State
    {
        public string ShortName { get; }
        public string LongName { get; }
        public IEnumerable<EntryAction> EntryActions { get; }
        public IEnumerable<ExitAction> ExitActions { get; }
        public IEnumerable<InternalTransition> InternalTransitions { get; }
        public IEnumerable<ExternalTransition> ExternalTransitions { get; }
        public IEnumerable<State> Children { get; }

        public State(
            string shortName, 
            string longName, 
            IEnumerable<EntryAction> entryActions, 
            IEnumerable<ExitAction> exitActions, 
            IEnumerable<InternalTransition> internalTransitions, 
            IEnumerable<ExternalTransition> externalTransitions, 
            IEnumerable<State> children)
        {
            ShortName = shortName;
            LongName = longName;
            EntryActions = entryActions;
            ExitActions = exitActions;
            InternalTransitions = internalTransitions;
            ExternalTransitions = externalTransitions;
            Children = children;
        }
    }
}
