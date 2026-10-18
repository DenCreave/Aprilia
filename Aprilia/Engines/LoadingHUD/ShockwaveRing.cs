namespace Aprilia.Engines.LoadingHUD;

public sealed class ShockwaveRing
{
    public bool IsAnimating { get; set; } = false;
    public double AnimationProgress { get; set; } = 0.0; // 0.0 to 1.0
    public int NodeCount { get; set; } // Was it a 3-node or 5-node ring that exploded?
}