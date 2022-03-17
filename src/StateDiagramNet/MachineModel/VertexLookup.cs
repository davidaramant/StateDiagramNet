using System.Collections.Generic;

namespace StateDiagramNet.MachineModel
{
    public sealed class VertexLookup
    {
        private readonly Dictionary<string, IVertex> _lookup = new Dictionary<string, IVertex>();

        public void Add(IVertex vertex)
        {
            if (_lookup.ContainsKey(vertex.Name))
            {
                throw new InvalidStateMachineException("Duplicate vertex defined: " + vertex.Name);
            }
            _lookup.Add(vertex.Name, vertex);
        }

        public IVertex this[string name]
        {
            get
            {
                if (!_lookup.ContainsKey(name))
                {
                    throw new InvalidStateMachineException("Undefined vertex: " + name);
                }

                return _lookup[name];
            }
        }
    }
}
