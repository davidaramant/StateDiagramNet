namespace StateDiagramNet.ParsingModel;

public sealed record InternalTransition(
    string Source,
    string EventName,
    string GuardName,
    string ActionName) 
    : IDiagramElement;
