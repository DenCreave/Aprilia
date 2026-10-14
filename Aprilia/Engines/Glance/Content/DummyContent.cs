using CommunityToolkit.Mvvm.ComponentModel;
using Aprilia.Engines.Glance;

namespace Aprilia.Engines.Glance.Content;

public partial class DummyContent : ObservableObject, IGlanceContent
{
    public GlancePage PageName => GlancePage.Dummy;
    
    [ObservableProperty] 
    private double _pageOpacity = 0.0; // MUST be Observable!
    
    public bool IsActivePage => PageOpacity > 0.0;
    public bool IsAtTop { get; set; } = true;
    public bool IsAtBottom { get; set; } = true;
    
    public string Headline { get; set; } = "";
    public string BodyText { get; set; } = "";
}