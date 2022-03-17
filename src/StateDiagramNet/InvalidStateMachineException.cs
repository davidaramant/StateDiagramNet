using System;

namespace StateDiagramNet
{
    public sealed class InvalidStateMachineException : Exception
    {
        public InvalidStateMachineException(string message) : base(message)
        {
        }
    }
}
