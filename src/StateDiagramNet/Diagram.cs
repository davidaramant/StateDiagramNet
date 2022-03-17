namespace StateDiagramNet;

public sealed class Diagram
{
    public string Name { get; }
    public IEnumerable<IDiagramElement> Components { get; }

    public Diagram(string name, IEnumerable<IDiagramElement> components)
    {
        Name = name;
        Components = components;
    }
}
