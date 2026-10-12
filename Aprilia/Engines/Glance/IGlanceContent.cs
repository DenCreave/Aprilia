namespace Aprilia.Engines.Glance;

public interface IGlanceContent
{
    GlancePage PageName { get; }
    double PageOpacity { get; set; }
    bool IsActivePage { get; }
    bool IsAtTop { get; set; } 
    bool IsAtBottom { get; set; }
}