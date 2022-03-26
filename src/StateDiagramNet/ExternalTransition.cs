namespace StateDiagramNet;

public sealed record ExternalTransition(
    string Source,
    string Destination,
    string EventName,
    string GuardName,
    string ActionName)
    : IDiagramElementOld
{
    public bool IsInitialTransition => Source == Constants.Star;
}
