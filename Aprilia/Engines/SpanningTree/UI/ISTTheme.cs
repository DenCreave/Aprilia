using Aprilia.Engines.SpanningTree.Theme;
using Avalonia.Media;

namespace Aprilia.Engines.SpanningTree.UI;

public interface ISTTheme
{
    ThemeGroup ThemeGroup { get; }
    StyleArchive StyleArchive { get; }
    public void Draw(DrawingContext context, Animap[] currentScene);
}