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
        
        //variables for LUT
        double x = StyleArchive.EntropyStyle.ResolutionNote.X;
        double y = StyleArchive.EntropyStyle.ResolutionNote.Y;
        double lengthSq = (x * x) + (y * y);
        //draw individual layer
        // we start from the furthest layer, current +1, since we are looking downwards (--)


        for (int i = (_camera.CurrentIndex + 1) % totalLayers; loopCount < totalLayers; ++loopCount)
        {
            var animap = currentScene[i];

            double centerX = animap.TorrentLayer.ResolutionNote.X / 2.0;
            double centerY = animap.TorrentLayer.ResolutionNote.Y / 2.0;

            double scale = GetScale(portion, i);
            var scaleMatrix = Matrix.CreateTranslation(-centerX, -centerY)
                              * Matrix.CreateScale(scale, scale)
                              * Matrix.CreateTranslation(centerX, centerY);

            using (context.PushTransform(scaleMatrix))
            using (context.PushOpacity(GetOpacity(i, totalLayers)))
            {
                foreach (var vertex in animap.Graph.Vertices)
                {
                    //get the LUT glow brush from stylearchive
                    var glowBrush = GetGlowBrush(vertex, x, y, lengthSq);

                    // glow first
                    context.DrawEllipse(glowBrush, null, new Point(vertex.Cox, vertex.Coy),
                        StyleArchive.EntropyStyle.DotGlowRadius, StyleArchive.EntropyStyle.DotGlowRadius);

                    // core second
                    context.DrawEllipse(StyleArchive.EntropyStyle.BGLayer, null, new Point(vertex.Cox, vertex.Coy),
                        StyleArchive.EntropyStyle.DotRadius, StyleArchive.EntropyStyle.DotRadius);
                }
            }

            // Move backwards through the layers, wrapping around
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
        }else if (layerIndex == (_camera.CurrentIndex + 1) % totalLayers)
        {
            return _camera.StrainProgress;
        }
        else
        {
            return 1; //otherwise full, non-opaque
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RadialGradientBrush GetGlowBrush(Vertex dot, double x, double y, double lengthSq)
    {
        double dotProduct = (dot.Cox * x) + ((dot.Coy - y) * (-y));
        double t = Math.Clamp(dotProduct / lengthSq, 0.0, 1.0);
        int paletteIndex = (int)(t * (StyleArchive.EntropyStyle.LUTCount - 1));
        return StyleArchive.EntropyStyle.GlowPalette[paletteIndex];
    }
}