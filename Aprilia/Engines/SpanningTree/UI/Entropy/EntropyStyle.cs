using Aprilia.Engines.SpanningTree.Theme;
using Aprilia.Engines.SpanningTree.TorrentLayer;
using Aprilia.Interfaces;
using Aprilia.Models;
using Avalonia;
using Avalonia.Media;

namespace Aprilia.Engines.SpanningTree.UI.Entropy;

public class EntropyStyle
{
    public ResNote ResolutionNote { get; }
    public LinearGradientBrush BGLayer { get; }
    public RadialGradientBrush[] GlowPalette { get; }

    public double DotRadius { get; } 
    public double DotGlowRadius { get; }
    public double DotGlowRatio { get; }
    public int LUTCount { get; }

    public EntropyStyle(TorrentLayerLoader loader)
    {
        ResolutionNote = loader.CreateNewLayer(ThemeGroup.Entropy, TLName.Entropy).ResolutionNote;
        
        Color colorStart = Color.FromArgb(255, 255, 0, 144);  // Pink
        Color colorEnd = Color.FromArgb(255, 255, 109, 61);   // Orange

        BGLayer = new LinearGradientBrush()
        {
            StartPoint = new RelativePoint(0, ResolutionNote.Y, RelativeUnit.Absolute),
            EndPoint = new RelativePoint(ResolutionNote.X, 0, RelativeUnit.Absolute),
            Opacity = 0.8,
            GradientStops = new GradientStops
            {
                new GradientStop(colorStart, 0.0),
                new GradientStop(colorEnd, 1.0),
            }
        };

        LUTCount = 100;
        GlowPalette = new RadialGradientBrush[LUTCount];
        DotRadius = 2.5;
        DotGlowRatio = 3;
        DotGlowRadius = DotRadius * DotGlowRatio;
        
        GeneratePalette(colorStart, colorEnd);
    }

    private void GeneratePalette(Color colorStart, Color colorEnd)
    {
        for (int i = 0; i < LUTCount; i++)
        {
            double amount = i / (double)LUTCount; 
            Color color = InterpolateColor(colorStart, colorEnd, amount);
            
            GlowPalette[i] = new RadialGradientBrush()
            {
                Center = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
                GradientOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
                // Avalonia 11 Fix:
                RadiusX = new RelativeScalar(0.5, RelativeUnit.Relative), 
                RadiusY = new RelativeScalar(0.5, RelativeUnit.Relative), 
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.FromArgb(0, color.R, color.G, color.B), 0.0),
                    new GradientStop(Color.FromArgb(200, color.R, color.G, color.B), DotRadius/DotGlowRadius),
                    new GradientStop(Color.FromArgb((byte)(200/3), color.R, color.G, color.B), (((DotGlowRadius-DotRadius)/3)+DotRadius)/DotGlowRadius),
                    new GradientStop(Color.FromArgb(0, color.R, color.G, color.B), 1.0),
                }
            };
        }
    }

    private Color InterpolateColor(Color c1, Color c2, double amount)
    {
        byte r = (byte)(c1.R + (c2.R - c1.R) * amount);
        byte g = (byte)(c1.G + (c2.G - c1.G) * amount);
        byte b = (byte)(c1.B + (c2.B - c1.B) * amount);
        return Color.FromArgb(255, r, g, b);
    }
}