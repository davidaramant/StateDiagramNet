using System.Collections.Generic;
using StateDiagramNet.MachineModel;

namespace StateDiagramNet.ParsingModel
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

            var vertexLookup = new VertexLookup();

            // First pass : state declarations
            foreach (var component in Components)
            {
                switch (component)
                {
                    case StateDefinition stateDef:
                        machine.AddChild(CreateState(machine, stateDef, vertexLookup));
                        break;

                    default:
                        // Ignore everything else in this pass
                        break;
                }
            }

            // Second pass : internal and external transitions
            foreach (var component in Components)
            {
                switch (component)
                {
                    case StateDefinition stateDef:
                        HandleStateComponents(stateDef, vertexLookup);
                        break;

                    

                    default:
                        // Ignore everything else in this pass
                        break;
                }
            }

            return machine;
        }

        private State CreateState(IVertex parent, StateDefinition stateDef, VertexLookup vertexLookup)
        {
            var state = new State(parent, stateDef.ShortName);
            vertexLookup.Add(state);

            foreach (var component in stateDef.Contents)
            {
                switch (component)
                {
                    case StateDefinition childState:
                        state.AddChild(CreateState(state, childState, vertexLookup));
                        break;

                    default:
                        // Ignore everything else in this pass
                        break;
                }
            }

            return state;
        }

        private void HandleStateComponents(
            StateDefinition stateDefinition,
            VertexLookup lookup)
        {
            var state = (State)lookup[stateDefinition.ShortName];

            foreach (var component in Components)
            {
                switch (component)
                {
                    case StateDefinition childStateDef:
                        // already handled
                        break;

                    case ExternalTransition initialTrans when initialTrans.IsInitialTransition:
                        // add initial transition
                        break;

                    default:
                        // Ignore everything else in this pass
                        break;
                }
            }
        }
    }
}
