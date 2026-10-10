using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Aprilia.Views;

public class TestRenderer : Control
{
    public override void Render(DrawingContext context)
    {
        // Log the size to the browser console
        Console.WriteLine($"Rendering at Width: {Bounds.Width}, Height: {Bounds.Height}");

        var fillBrush = new SolidColorBrush(Colors.Cyan);
        var strokePen = new Pen(new SolidColorBrush(Colors.HotPink), 5);
        
        var centerPoint = new Point(Bounds.Width / 2, Bounds.Height / 2);
        
        context.DrawEllipse(fillBrush, strokePen, centerPoint, 150, 150);
    }
}