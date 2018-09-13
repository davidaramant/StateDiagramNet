using System.Collections.Generic;
using StateDiagramCodeGen.MachineModel;

namespace StateDiagramCodeGen.ParsingModel
{
    public sealed class Diagram
    {
        public string Name { get; }
        public IEnumerable<IDiagramElement> Components;

        public Diagram(string name, IEnumerable<IDiagramElement> components)
        {
            Name = name;
            Components = components;
        }

        public StateMachine ToMachineModel()
        {
            var machine = new StateMachine(Name);

            foreach (var component in Components)
            {
                switch (component)
                {
                    case StateDefinition state:
                        machine.AddChild(ConvertState(machine, state));
                        break;

                    default:
                        throw new InvalidStateMachineException("Component type not allowed at top level: " + component);
                }
            }

            return machine;
        }

        private State ConvertState(IVertex parent, StateDefinition state)
        {
            var modelState = new State(parent, state.ShortName);

            foreach (var component in state.Contents)
            {
                switch (component)
                {
                    case StateDefinition childState:
                        modelState.AddChild(ConvertState(modelState, childState));
                        break;

                    default:
                        throw new InvalidStateMachineException("Component type not allowed at top level: " + component);
                }
            }

            return modelState;
        }
    }
}
