using System;
using Aprilia.Engines.LoadingHUD;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Aprilia.Engines.Glance;

/// <summary>
/// Renders ILoadingHud. tbh a rename might be good, but meh. 'tis i fancy
/// </summary>
public class GlanceRenderer : Control
{
    private readonly ILoadingHUD _hud;
    private readonly DispatcherTimer _renderTimer;

    public GlanceRenderer()
    {
        // Grab the HUD from your DI container!
        _hud = App.Services.GetRequiredService<ILoadingHUD>();

        _renderTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1.0 / 60)
        };

        _renderTimer.Tick += (sender, args) =>
        {
            _hud.UpdateHUD();
            InvalidateVisual(); // Tells Avalonia to trigger the Render method
        };
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _renderTimer.Start();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _renderTimer.Stop();
    }

    public override void Render(DrawingContext context)
    {
        _hud.Draw(context, Bounds.Width, Bounds.Height);
    }

}