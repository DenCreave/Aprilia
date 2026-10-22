using System;

namespace Aprilia.Engines.Glance;

public interface IGlanceContent
{
    GlancePage PageName { get; }
    double PageOpacity { get; set; }
    //bool IsActivePage { get; set; }
    DateTime? HideAt { get; set; }
    bool IsAtTop { get; set; } 
    bool IsAtBottom { get; set; }
    public int ZIndex { get; set; }
}