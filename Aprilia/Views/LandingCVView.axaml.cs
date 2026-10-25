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

    public void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        RouteScroll(e.Delta.Y);
        e.Handled = true; 
    }

    private void RouteScroll(double deltaY)
    {
       // var glanceController = App.Services.GetRequiredService<GlanceController>();
        _glanceController.HandleScroll(deltaY);
    }

    public void OnScrollGesture(object? sender, ScrollGestureEventArgs e)
    {
        if ((DateTime.Now - _lastGestureTime).TotalMilliseconds < 16) 
        {
            e.Handled = true;
            return;
        }
        _lastGestureTime = DateTime.Now;

        RouteScroll(-e.Delta.Y); 
        e.Handled = true;

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