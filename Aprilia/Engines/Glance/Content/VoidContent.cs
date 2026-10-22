using System;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Aprilia.Engines.Glance.Content;

public partial class VoidContent : ObservableObject, IGlanceContent
{
    public GlancePage PageName => GlancePage.Void;
    public double PageOpacity { get; set; } = 0;
    public bool IsActivePage { get; set; } = false;
    public bool IsAtTop { get; set; } = true;
    public bool IsAtBottom { get; set; } = true;
    public DateTime? HideAt { get; set; } = null;
    
    [ObservableProperty] private int _zIndex;
}