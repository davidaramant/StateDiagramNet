namespace StateDiagramNet;

public sealed record StateDefinitionOld(
    string ShortName,
    string LongName,
    IEnumerable<IDiagramElementOld> Contents)
    : IDiagramElementOld;
