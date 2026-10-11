using Aprilia.Engines.SpanningTree.UI.Entropy;

namespace Aprilia.Engines.SpanningTree.UI;

public sealed class StyleArchive(EntropyStyle entropyStyle)
{
    public EntropyStyle EntropyStyle { get; } = entropyStyle;
}