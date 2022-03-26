namespace StateDiagramNet.PlantUmlModel;
public sealed record StateDescription(
    string StateName,
    string Description)
    : IDiagramElement;
