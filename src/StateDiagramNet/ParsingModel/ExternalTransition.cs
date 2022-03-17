namespace StateDiagramNet.ParsingModel;

public sealed record ExternalTransition(
    string Source,
    string Destination,
    string EventName,
    string GuardName,
    string ActionName) 
    : IDiagramElement
{
    public bool IsInitialTransition => Source == Constants.Star;
}
