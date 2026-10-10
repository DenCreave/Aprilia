using Aprilia.Engines.SpanningTree.Theme;

namespace Aprilia.Interfaces;

public interface IThemeBinder
{
    ThemeGroup ThemeGroup { get; }
    Animap[] ElicitateAnimap();
}