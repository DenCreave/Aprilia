using System;
using Aprilia.Engines.SpanningTree.DotMap;
using Aprilia.Engines.SpanningTree.Theme;
using Aprilia.Models;

namespace Aprilia.Interfaces;

public interface IDotMapFactory
{
    ThemeGroup Group { get; }
    DMName Name { get; } 
    int LayerLevel { get; } //to set an order
    Constellation GenerateVertices();
}

public interface IDotMapFactory<T> : IDotMapFactory
    where T : IDMProperties
{
    T UniqueProps { get; }
    IDotMapFactory<T> Configure(Action<T> configOptions);
}