using CommunityToolkit.Mvvm.ComponentModel;
using Aprilia.Engines.SpanningTree.Controller;
using Aprilia.Engines.SpanningTree.UI;

namespace Aprilia.ViewModels;

public partial class LandingCVViewModel(
    SpanningTreeController stc, 
    SpanningTreeTheme stt) : ViewModelBase
{
    public SpanningTreeController Stc { get; } = stc;
    public SpanningTreeTheme Stt { get; } = stt;
}