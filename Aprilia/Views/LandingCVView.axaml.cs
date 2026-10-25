using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using Aprilia.Engines.Glance;

namespace Aprilia.Views;

public partial class LandingCVView : UserControl
{
    private DateTime _lastGestureTime = DateTime.MinValue;
    private Point _lastTouchPoint;
    private bool _isTouching;
    private GlanceController? _glanceController;
    public LandingCVView()
    {
        InitializeComponent();
        _glanceController = App.Services.GetRequiredService<GlanceController>();
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel != null)
        {
            // TUNNEL: Catch the scroll BEFORE the ScrollViewer gets it!
            topLevel.AddHandler(InputElement.PointerWheelChangedEvent, OnPointerWheelChanged, RoutingStrategies.Tunnel);
            topLevel.AddHandler(InputElement.ScrollGestureEvent, OnScrollGesture, RoutingStrategies.Tunnel);
        }
    }

    public void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        RouteScroll(e.Delta.Y);
        // DO NOT set e.Handled = true here! Let the ScrollViewer have the event if we didn't flip the page!
        //e.Handled = true; 
    }

    private void RouteScroll(double deltaY)
    {
       // var glanceController = App.Services.GetRequiredService<GlanceController>();
        _glanceController.HandleScroll(deltaY);
    }

    public void OnScrollGesture(object? sender, ScrollGestureEventArgs e)
    {
        /*if ((DateTime.Now - _lastGestureTime).TotalMilliseconds < 16) 
        {
            e.Handled = true;
            return;
        }*/
        _lastGestureTime = DateTime.Now;

        RouteScroll(-e.Delta.Y); 
        // DO NOT set e.Handled = true here! Let the ScrollViewer have the event if we didn't flip the page!
        //e.Handled = true;

    }
    
    public void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Pointer.Type == PointerType.Touch)
        {
            _lastTouchPoint = e.GetPosition(this);
            _isTouching = true;
        }
    }
    
    public void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isTouching && e.Pointer.Type == PointerType.Touch)
        {
            var currentPoint = e.GetPosition(this);
            
            double pixelDeltaY = currentPoint.Y - _lastTouchPoint.Y;
            _lastTouchPoint = currentPoint;

            //var glanceController = App.Services.GetRequiredService<GlanceController>();
            _glanceController.HandleMobile(pixelDeltaY);
        }
    }
    
    public void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.Pointer.Type == PointerType.Touch)
        {
            _isTouching = false;
        }
    }

}