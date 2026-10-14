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
        double portion = (_camera.ScaleMax - _camera.ScaleFrom) / (totalLayers + 2);
        int loopCount = 0;

        for (int i = (_camera.CurrentIndex + 1) % totalLayers; loopCount < totalLayers; ++loopCount)
        {
            var animap = currentScene[i];

            // THE DYNAMIC CENTER FIX
            // This will always be the exact dead-center of the user's screen, whether it's 4K or Mobile.
            double centerX = animap.TorrentLayer.ResolutionNote.X / 2.0;
            double centerY = animap.TorrentLayer.ResolutionNote.Y / 2.0;

            // We need these for the LUT calculation
            double x = animap.TorrentLayer.ResolutionNote.X;
            double y = animap.TorrentLayer.ResolutionNote.Y;

            double scale = GetScale(portion, i);

            // The Matrix scales perfectly from the dynamic center
            var scaleMatrix = Matrix.CreateTranslation(-centerX, -centerY)
                              * Matrix.CreateScale(scale, scale)
                              * Matrix.CreateTranslation(centerX, centerY);

            using (context.PushTransform(scaleMatrix))
            using (context.PushOpacity(GetOpacity(i, totalLayers)))
            {
                foreach (var vertex in animap.Graph.Vertices)
                {
                    var glowBrush = GetGlowBrush(vertex, x, y);

                    context.DrawEllipse(glowBrush, null, new Point(vertex.Cox, vertex.Coy),
                        StyleArchive.EntropyStyle.DotGlowRadius, StyleArchive.EntropyStyle.DotGlowRadius);

                    context.DrawEllipse(StyleArchive.EntropyStyle.BGLayer, null, new Point(vertex.Cox, vertex.Coy),
                        StyleArchive.EntropyStyle.DotRadius, StyleArchive.EntropyStyle.DotRadius);
                }
            }

            i = --i < 0 ? totalLayers - 1 : i;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double GetScale(double portion, int layerIndex)
    {
        int distanceFromFocus = (_camera.CurrentIndex - layerIndex < 0)
            ? Math.Abs(_camera.CurrentIndex - layerIndex) + _camera.CurrentIndex
            : _camera.CurrentIndex - layerIndex;
        double scale = _camera.ScaleFrom + portion * distanceFromFocus + portion * _camera.StrainProgress;
        return scale;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double GetOpacity(int layerIndex, int totalLayers)
    {
        //logic for the one fading out, and the one fading in
        if (layerIndex == _camera.CurrentIndex)
        {
            return 1 - _camera.StrainProgress;
        }
        else if (layerIndex == (_camera.CurrentIndex + 1) % totalLayers)
        {
            return _camera.StrainProgress;
        }
        else
        {
            return 1; //otherwise full, non-opaque
        }
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
}