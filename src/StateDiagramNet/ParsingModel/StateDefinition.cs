namespace StateDiagramNet.ParsingModel;

public sealed record StateDefinition(
    string ShortName,
    string LongName,
    IEnumerable<IDiagramElement> Contents) 
    : IDiagramElement;
