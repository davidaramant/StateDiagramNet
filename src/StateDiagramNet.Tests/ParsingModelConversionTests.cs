using Sprache;
using StateDiagramNet.ParsingModel;
using Xunit;

namespace StateDiagramNet.Tests
{
    public sealed class ParsingModelConversionTests
    {
        [Fact]
        public void ShouldConvertStates()
        {
            const string input = @"@startuml ""Simple Diagram""
                state Off
                state On {
                    state Idle
                    state Responding
                }
                @enduml";

            var diagram = PlantUmlParser.Diagram.End().Parse(input);

            diagram.ToMachineModel();
        }
    }
}
