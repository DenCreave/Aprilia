using System;
using System.Runtime.CompilerServices;
using Aprilia.Engines.SpanningTree.Theme;
using Aprilia.Models;
using Avalonia;
using Avalonia.Media;

namespace Aprilia.Engines.SpanningTree.UI.Entropy;

public class EntropyTheme(
    StyleArchive styleArchive,
    CameraState camera) : ISTTheme
{
    private readonly CameraState _camera = camera;
    public ThemeGroup ThemeGroup => ThemeGroup.Entropy;
    public StyleArchive StyleArchive { get; } = styleArchive;

    public void Draw(DrawingContext context, Animap[] currentScene)
    {
        int totalLayers = currentScene.Length;

        // 1. FIND THE FURTHEST LAYER (Z-Sorting)
        // We find the layer with the highest Z value so we can draw it first (Painter's Algorithm).
        int furthestLayerIndex = 0;
        double maxZ = -1;
        for (int i = 0; i < totalLayers; i++)
        {
            double z = GetZ(i, totalLayers);
            if (z > maxZ)
            {
                maxZ = z;
                furthestLayerIndex = i;
            }
        }

        // 2. DRAW FROM BACK TO FRONT
        for (int count = 0; count < totalLayers; count++)
        {
            // Go backwards from the furthest layer
            int i = (furthestLayerIndex - count + totalLayers) % totalLayers;
            var animap = currentScene[i];

            double z = GetZ(i, totalLayers);
            double scale = GetScale(z, totalLayers);
            double opacity = GetOpacity(z, totalLayers);

            // Skip drawing if it's completely invisible 
            if (opacity <= 0.001) continue;

            double centerX = animap.TorrentLayer.ResolutionNote.X / 2.0;
            double centerY = animap.TorrentLayer.ResolutionNote.Y / 2.0;

            double x = animap.TorrentLayer.ResolutionNote.X;
            double y = animap.TorrentLayer.ResolutionNote.Y;
            double lengthSq = (x * x) + (y * y);

            var scaleMatrix = Matrix.CreateTranslation(-centerX, -centerY)
                              * Matrix.CreateScale(scale, scale)
                              * Matrix.CreateTranslation(centerX, centerY);

            using (context.PushTransform(scaleMatrix))
            using (context.PushOpacity(opacity))
            {
                foreach (var vertex in animap.Graph.Vertices)
                {
                    int colorIndex = GetPaletteIndex(vertex, x, y);

                    var glowBrush = StyleArchive.EntropyStyle.GlowPalette[colorIndex];
                    var coreBrush = StyleArchive.EntropyStyle.CorePalette[colorIndex];

                    context.DrawEllipse(glowBrush, null, new Point(vertex.Cox, vertex.Coy),
                        StyleArchive.EntropyStyle.DotGlowRadius, StyleArchive.EntropyStyle.DotGlowRadius);

                    context.DrawEllipse(coreBrush, null, new Point(vertex.Cox, vertex.Coy),
                        StyleArchive.EntropyStyle.DotRadius, StyleArchive.EntropyStyle.DotRadius);


                    /*  //this would apply the gradient to individual dots. that would be static.
                     // looks nice but meh.
                    var glowBrush = GetGlowBrush(vertex, x, y);

                    context.DrawEllipse(glowBrush, null, new Point(vertex.Cox, vertex.Coy),
                        StyleArchive.EntropyStyle.DotGlowRadius, StyleArchive.EntropyStyle.DotGlowRadius);

                    context.DrawEllipse(StyleArchive.EntropyStyle.BGLayer, null, new Point(vertex.Cox, vertex.Coy),
                        StyleArchive.EntropyStyle.DotRadius, StyleArchive.EntropyStyle.DotRadius);*/
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double GetZ(int layerIndex, int totalLayers)
    {
        // 1. Where is the camera exactly? (e.g., Page 1 + 0.5 Strain = 1.5)
        double absoluteDepth = _camera.CurrentIndex + _camera.StrainProgress;

        // 2. How far is this layer from the camera?
        double z = (layerIndex - absoluteDepth) % totalLayers;

        // 3. Wrap it into a perfect 0.0 to totalLayers continuum
        if (z < 0) z += totalLayers;

        return z;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double GetScale(double z, int totalLayers)
    {
        // Z=0 is closest (ScaleMax). Z=totalLayers is furthest (ScaleFrom).
        double portion = z / totalLayers;
        return _camera.ScaleMax - (portion * (_camera.ScaleMax - _camera.ScaleFrom));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double GetOpacity(double z, int totalLayers)
    {
        // Fading out as it smashes into the camera
        if (z < 1.0) return z;

        // Fading in as it spawns in the deep void
        if (z > totalLayers - 1.0) return totalLayers - z;

        // Solid in the middle
        return 1.0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetPaletteIndex(Vertex dot, double screenWidth, double screenHeight)
    {
        // Universal Vector: Top-Left (0,0) to Bottom-Right (Width, Height)
        double startX = 0;
        double startY = 0;
        double endX = screenWidth;
        double endY = screenHeight;

        double vx = endX - startX;
        double vy = endY - startY;
        double wx = dot.Cox - startX;
        double wy = dot.Coy - startY;

        double dotProduct = (wx * vx) + (wy * vy);
        double lengthSq = (vx * vx) + (vy * vy);

        double t = Math.Clamp(dotProduct / lengthSq, 0.0, 1.0);
        return (int)(t * (StyleArchive.EntropyStyle.LUTCount - 1));
    }

    /*

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RadialGradientBrush GetGlowBrush(Vertex dot, double x, double y, double lengthSq)
    {
        double dotProduct = (dot.Cox * x) + ((dot.Coy - y) * (-y));
        double t = Math.Clamp(dotProduct / lengthSq, 0.0, 1.0);
        int paletteIndex = (int)(t * (StyleArchive.EntropyStyle.LUTCount - 1));
        return StyleArchive.EntropyStyle.GlowPalette[paletteIndex];
    }



    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RadialGradientBrush GetGlowBrush(Vertex dot, double screenWidth, double screenHeight)
    {
// Define your gradient direction here (Matches your RelativePoints above!)
// Example: Top-Left (0,0) to Bottom-Right (Width, Height)
        double startX = 0;
        double startY = 0;
        double endX = screenWidth;
        double endY = screenHeight;
        // Vector V: The line of the gradient
        double vx = endX - startX;
        double vy = endY - startY;

// Vector W: The line from the start point to the dot
        double wx = dot.Cox - startX;
        double wy = dot.Coy - startY;

// Mathematical Projection (Dot Product / Length Squared)
        double dotProduct = (wx * vx) + (wy * vy);
        double lengthSq = (vx * vx) + (vy * vy);

        double t = Math.Clamp(dotProduct / lengthSq, 0.0, 1.0);

        int paletteIndex = (int)(t * (StyleArchive.EntropyStyle.LUTCount - 1));
        return StyleArchive.EntropyStyle.GlowPalette[paletteIndex];
    }
    */
}