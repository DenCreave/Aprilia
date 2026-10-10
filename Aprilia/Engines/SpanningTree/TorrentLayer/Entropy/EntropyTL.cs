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
    public ResNote ResolutionNote => new ResNote() { X = 1920, Y = 1080, Ratio = "16:9" };

    
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
        // --- THE HASH TRICK (Zero-GC Pseudo-Randomness) ---
        // Scrambles the Halton index so the dots don't form visible ribbons
        uint id = (uint)vertex.ID;
        uint hashX = id * 2654435761u;
        uint hashY = id * 2246822519u;

        int indexX = (int)(hashX % (uint)UniqueProps.Noise.HaltonValuesX.Length);
        int indexY = (int)(hashY % (uint)UniqueProps.Noise.HaltonValuesY.Length);

        // 1. RANDOM STARTING POSITION (Using ResNote)
        vertex.Cox = UniqueProps.Noise.HaltonValuesX[indexX] * ResolutionNote.X;
        vertex.Coy = UniqueProps.Noise.HaltonValuesY[indexY] * ResolutionNote.Y;

        // 2. CALCULATE THE ANGLE (0.0 to 1.0) * (2 * PI)
        double angleInRadians = UniqueProps.Noise.HaltonValuesX[indexX] * (Math.PI * 2);

        // 3. CONVERT ANGLE TO X/Y VECTORS
        vertex.Vex = Math.Cos(angleInRadians);
        vertex.Vey = Math.Sin(angleInRadians);

        // 4. VARY THE SPEED
        double speedModifier = 0.5 + UniqueProps.Noise.HaltonValuesY[indexY];
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