namespace StateDiagramNet;

public sealed record StateDefinition(
    string ShortName,
    string LongName,
    IEnumerable<IDiagramElement> Contents)
    : IDiagramElement;
