using System;
using Aprilia.Controllers;
using Aprilia.Engines.SpanningTree.Theme;
using Aprilia.Interfaces;
using Aprilia.Models;
using Aprilia.Models.EntropyProps;

namespace Aprilia.Engines.SpanningTree.TorrentLayer.Entropy;

public class EntropyTL : ITorrentLayer<EntropyProperties>
{
    public ThemeGroup Group => ThemeGroup.Entropy;
    public TLName Name => TLName.Entropy;
    public ResNote ResolutionNote { get; set; }

    
    public EntropyProperties UniqueProps { get; }
    public AnimatrixController Controls { get; }
    public RenderAlignment Align { get; }

    public bool IsAffecting { get; }
    public bool IsVisible { get; }

    public EntropyTL(AnimatrixController controls)
    {
        IsVisible = true;
        IsAffecting = true;
        Controls = controls;
        Align = RenderAlignment.Unchanged;
        int dotCount = 900; // 300 dots * 3 layers.
        int baseX = 7;
        int baseY = 13;
        ResolutionNote = new ResNote() { X = 1920, Y = 1080, Ratio = "16:9" };
        
        UniqueProps = new EntropyProperties()
        {
            FlowSpeed = 0.5, 
            Noise = new Noise()
            {
                BaseX = 2,
                BaseY = 3,
                AffectsX = true,
                AffectsY = true,
                HaltonDimension = 2,
                NoiseInstances = dotCount,
                HaltonValuesX = Controls.HaltonSequencer1D(dotCount, baseX),
                HaltonValuesY = Controls.HaltonSequencer1D(dotCount, baseY)
            }
        };
    }

    public void Spawn(Vertex vertex)
    {
        // 1. STARTING POSITION (Keep Halton here so the dots don't clump together)
        uint id = (uint)vertex.ID;
        uint hashX = id * 2654435761u;
        uint hashY = id * 2246822519u;

        int indexX = (int)(hashX % (uint)UniqueProps.Noise.HaltonValuesX.Length);
        int indexY = (int)(hashY % (uint)UniqueProps.Noise.HaltonValuesY.Length);

        vertex.Cox = UniqueProps.Noise.HaltonValuesX[indexX] * ResolutionNote.X;
        vertex.Coy = UniqueProps.Noise.HaltonValuesY[indexY] * ResolutionNote.Y;

        // 2. TRUE RANDOM ANGLE & SPEED (Seeded by Layer Signature)
        // By using BaseX and BaseY in the seed, every layer gets a completely unique set of random numbers.
        int layerSignature = (UniqueProps.Noise.BaseX * 100) + (UniqueProps.Noise.BaseY * 10);
        int seed = vertex.ID + layerSignature;
        
        Random rand = new Random(seed);
        
        // Random angle between 0 and 360 degrees (in radians)
        double angleInRadians = rand.NextDouble() * (Math.PI * 2);

        // 3. CONVERT ANGLE TO X/Y VECTORS
        vertex.Vex = Math.Cos(angleInRadians);
        vertex.Vey = Math.Sin(angleInRadians);

        // 4. VARY THE SPEED (Random multiplier between 0.5x and 1.5x)
        double speedModifier = 0.5 + rand.NextDouble();
        vertex.Speed = UniqueProps.FlowSpeed * speedModifier;

        vertex.IsEnabled = true;
    }
    public void AffectVector(Vertex vertex)
    {
        if (!vertex.IsEnabled)
        {
            Spawn(vertex);
            return;
        }

        // 1. MOVE THE DOT
        vertex.Cox += vertex.Vex * vertex.Speed;
        vertex.Coy += vertex.Vey * vertex.Speed;

        // 2. PAC-MAN EDGE WRAPPING (Using ResNote)
        if (vertex.Cox > ResolutionNote.X) vertex.Cox = 0;
        else if (vertex.Cox < 0) vertex.Cox = ResolutionNote.X;

        if (vertex.Coy > ResolutionNote.Y) vertex.Coy = 0;
        else if (vertex.Coy < 0) vertex.Coy = ResolutionNote.Y;
    }

    public void UpdateState(double deltaTime) { }
    public void Reset() { }
    public void Despawn(Vertex vertex) { vertex.IsEnabled = false; }

    public ITorrentLayer<EntropyProperties> Configure(Action<EntropyProperties> configOptions)
    {
        configOptions(UniqueProps);
        
        UniqueProps.Noise.HaltonValuesX = Controls.HaltonSequencer1D(UniqueProps.Noise.NoiseInstances, UniqueProps.Noise.BaseX);
        UniqueProps.Noise.HaltonValuesY = Controls.HaltonSequencer1D(UniqueProps.Noise.NoiseInstances, UniqueProps.Noise.BaseY);
        return this;
    }
}