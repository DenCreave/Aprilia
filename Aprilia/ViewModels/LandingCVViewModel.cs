using Aprilia.Engines.Glance;
using Aprilia.Engines.Mediator;
using CommunityToolkit.Mvvm.ComponentModel;
using Aprilia.Engines.SpanningTree.Controller;
using Aprilia.Engines.SpanningTree.UI;

namespace Aprilia.ViewModels;

public partial class LandingCVViewModel(
    SpanningTreeController stc, 
    SpanningTreeTheme stt,
    TwinRenderMediator mediator,
    GlanceContentLoader contentLoader) : ViewModelBase
{
    private readonly TwinRenderMediator _mediator = mediator;
    public SpanningTreeController Stc { get; } = stc;
    public SpanningTreeTheme Stt { get; } = stt;
    public GlanceContentLoader ContentLoader { get; } = contentLoader;
}