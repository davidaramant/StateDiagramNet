﻿using System.Collections.Immutable;

namespace StateDiagramNet.PlantUmlModel;
public sealed record StateDefinition(
    string Name,
    string LongName,
    ImmutableArray<IDiagramElement> Children)
    : IDiagramElement;
