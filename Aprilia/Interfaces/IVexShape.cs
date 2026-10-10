using Aprilia.Models;

namespace Aprilia.Interfaces;

public interface IVexShape
{
    public Vertex[] Shapes { get; init; }
}

public interface IVexShape<T> : IVexShape
    where T : IShapeProperties
{
    public T UniqueProps { get; }
}