namespace StateDiagramNet;

public sealed class Diagram
{
    public string Name { get; }
    public IEnumerable<IDiagramElementOld> Components { get; }

    public Diagram(string name, IEnumerable<IDiagramElementOld> components)
    {
        Name = name;
        Components = components;
    }
}
