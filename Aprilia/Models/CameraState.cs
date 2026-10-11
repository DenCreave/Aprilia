namespace Aprilia.Models;

public sealed class CameraState
{
    public int CurrentIndex { get; set; } = 0;
    public double ScaleFrom { get; set; } = 1.0;
    public double ScaleMax { get; set; } = 4.0;
    
    // -1.0 to 1.0. 
    // > 0 means scrolling forward (diving deeper). 
    // < 0 means scrolling backward (pulling out).
    public double StrainProgress { get; set; } = 0.0; 

}