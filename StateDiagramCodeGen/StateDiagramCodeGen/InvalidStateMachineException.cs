using System;

namespace StateDiagramCodeGen
{
    public sealed class InvalidStateMachineException : Exception
    {
        public InvalidStateMachineException(string message) : base(message)
        {
        }
    }
}
