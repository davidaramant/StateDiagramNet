namespace StateDiagramNet.PlantUmlModel;
public sealed record TransitionDefinition(
    string Source,
    string Destination,
    string? Description)
    : IDiagramElement;
