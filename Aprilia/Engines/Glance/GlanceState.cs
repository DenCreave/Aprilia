using CommunityToolkit.Mvvm.ComponentModel;

namespace Aprilia.Engines.Glance;

public partial class GlanceState : ObservableObject
{
    [ObservableProperty] 
    private int _currentFocus = 0;
    
    [ObservableProperty] 
    private double _strainProgress = 0.0; // -1.0 (Up) to 1.0 (Down)

    public bool JustFlipped { get; set; } = false;
    
    public int MaxPages { get; set; }
}