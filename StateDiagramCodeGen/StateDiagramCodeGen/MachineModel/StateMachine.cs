﻿using System.Collections.Generic;

namespace StateDiagramCodeGen.MachineModel
{
    public sealed class StateMachine : IVertex
    {
        public string Name { get; }

        private readonly List<IVertex> _children = new List<IVertex>();

        public StateMachine(string name)
        {
            Name = name;
        }

        public void AddChild(IVertex child)
        {
            _children.Add(child);
        }
    }
}
