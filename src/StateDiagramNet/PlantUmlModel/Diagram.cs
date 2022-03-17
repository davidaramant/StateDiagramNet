using System.Collections.Immutable;

namespace StateDiagramNet.PlantUmlModel;
public sealed record Diagram(
    string Name,
    ImmutableArray<IDiagramElement> Elements);
