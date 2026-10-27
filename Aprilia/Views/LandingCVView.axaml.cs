using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using Aprilia.Engines.Glance;
using Aprilia.ViewModels;

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
            // Set initial scale
            if (DataContext is LandingCVViewModel vm)
                vm.UiScale = ResponsiveScaler.GetScale(topLevel.Bounds.Width);

            // Listen for window resizes/rotations
            topLevel.SizeChanged += OnWindowSizeChanged;
            
            // TUNNEL: Catch the scroll BEFORE the ScrollViewer gets it!
            topLevel.AddHandler(InputElement.PointerWheelChangedEvent, OnPointerWheelChanged, RoutingStrategies.Tunnel);
            topLevel.AddHandler(InputElement.ScrollGestureEvent, OnScrollGesture, RoutingStrategies.Tunnel);
        }
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel != null)
        {
            topLevel.SizeChanged -= OnWindowSizeChanged;
            
            topLevel.RemoveHandler(InputElement.PointerWheelChangedEvent, OnPointerWheelChanged);
            topLevel.RemoveHandler(InputElement.ScrollGestureEvent, OnScrollGesture);
        }
    }
    
    private void OnWindowSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (sender is TopLevel topLevel && DataContext is LandingCVViewModel vm)
        {
            // Update the global scale whenever the phone rotates!
            vm.UiScale = ResponsiveScaler.GetScale(topLevel.Bounds.Width);
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