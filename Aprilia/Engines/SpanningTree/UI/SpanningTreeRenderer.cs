using System;
using System.Diagnostics;
using Aprilia.Engines.SpanningTree.Controller;
using Aprilia.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace Aprilia.Engines.SpanningTree.UI;

public class SpanningTreeRenderer : Control
{
    /// <summary>
    /// this following is how the Frontend (as in AXAML) inject dependency.
    /// it inserts it in as a property, not as constructor.
    /// </summary>
    // 1. We register a  property that XAML is allowed to talk to
    // the reason its public is because Avalonia's engine needs to read it.
    // could call it XAML binding or XAML parsing
    public static readonly StyledProperty<SpanningTreeController> STCProperty =
        AvaloniaProperty.Register<SpanningTreeRenderer, SpanningTreeController>(nameof(STC));

    // 2. This is the actual property the XAML binding writes to!
    public SpanningTreeController STC
    {
        get => GetValue(STCProperty);
        set => SetValue(STCProperty, value);
    }
    
    public static readonly StyledProperty<SpanningTreeTheme> STTProperty =
        AvaloniaProperty.Register<SpanningTreeRenderer, SpanningTreeTheme>(nameof(STT));

    public SpanningTreeTheme STT
    {
        get => GetValue(STTProperty);
        set => SetValue(STTProperty, value);
    }

    
    private DispatcherTimer _renderTimer;

    public SpanningTreeRenderer()
    {
        _renderTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS
        };

        _renderTimer.Tick += (sender, args) =>
        {
            if (STC == null || STT == null || STC.CurrentScene == null) 
            { 
                Console.WriteLine("TICK FAILED: STT is null. Check XAML Binding."); 
                return; 
            }
            
            // this is how we get the the LIVE size of the browser/canvas/ui [FROM THE CONTROL CLASS]
            int liveWidth = (int)this.Bounds.Width;
            int liveHeight = (int)this.Bounds.Height;

            
            if (liveWidth!=STC.CurrentScene[0].TorrentLayer.ResolutionNote.X
                || liveHeight!=STC.CurrentScene[0].TorrentLayer.ResolutionNote.Y)
            {
                foreach (var animap in STC.CurrentScene)
                {
                    animap.TorrentLayer.ResolutionNote = new ResNote { X = liveWidth, Y = liveHeight, Ratio = "Dynamic" };
                }
            }


            STC.UpdateFrame();
            // tells Avalonia: "The math changed redraw the screen!"
            // This automatically triggers the Render() method below.
            this.InvalidateVisual(); 
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
        if (STC == null) { Debug.WriteLine("RENDER ABORTED: STC is NULL (Binding Failed)"); return; }
        if (STT == null) { Debug.WriteLine("RENDER ABORTED: STT is NULL (Binding Failed)"); return; }
        if (STC.CurrentScene == null) { Debug.WriteLine("RENDER ABORTED: CurrentScene is NULL"); return; }
        
        Debug.WriteLine($"RENDER SUCCESS: Drawing {STC.CurrentScene.Length} layers at 60 FPS.");
        
        
        STT.DrawTheme(context, STC.CurrentScene, STC.CurrentTheme);
    }

}