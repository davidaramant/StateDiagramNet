using System;
using System.Collections.Generic;
using System.Text;

namespace StateDiagramCodeGen.Model
{
    public sealed class InternalTransition : IDiagramElement
    {
        public string Source { get; }
        public string EventName { get; }
        public string GuardName { get; }
        public string ActionName { get; }

        public InternalTransition(
            string source,
            string eventName,
            string guardName,
            string actionName)
        {
            Source = source;
            EventName = eventName;
            GuardName = guardName;
            ActionName = actionName;
        }
    }
}
