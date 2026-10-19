using System;
using System.Runtime.CompilerServices;

namespace Aprilia.Engines.Glance.Content;

public class VoidContent : IGlanceContent
{
    public GlancePage PageName => GlancePage.Void;
    public double PageOpacity { get; set; } = 0;
    public bool IsActivePage { get; set; } = false;
    public bool IsAtTop { get; set; } = true;
    public bool IsAtBottom { get; set; } = true;
    public DateTime? HideAt { get; set; } = null;
}