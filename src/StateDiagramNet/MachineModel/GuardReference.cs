using System.Diagnostics;

namespace StateDiagramNet.MachineModel
{
    [DebuggerDisplay("{_name}")]
    public sealed class GuardReference
    {
        private readonly string _name;

        public GuardReference(string name)
        {
            _name = name;
        }
    }
}
