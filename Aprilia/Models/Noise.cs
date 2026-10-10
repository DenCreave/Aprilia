namespace Aprilia.Models;

public sealed class Noise
{
    /// i thought of adding a noise algorithm enum
    /// but i won't use anything other than halton
    public int BaseX { get; set; }
    public int BaseY { get; set; }
    public bool AffectsX { get; init; }
    public bool AffectsY { get; init; }
    public int HaltonDimension { get; init; }
    public int NoiseInstances { get; set; }
    public double NoiseScale { get; init; }
    //public double[] HaltonValues1D { get; set; }
    public double[] HaltonValuesX { get; set; }
    public double[] HaltonValuesY { get; set; }
}