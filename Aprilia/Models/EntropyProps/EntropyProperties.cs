using Aprilia.Interfaces;

namespace Aprilia.Models.EntropyProps;

public class EntropyProperties : ILayerProperties
{
    public Noise Noise { get; set; }
    public double FlowSpeed { get; set; }

}