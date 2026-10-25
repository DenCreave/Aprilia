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
    public LandingCVView()
    {
        InitializeComponent();
    }

    public void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        RouteScroll(e.Delta.Y);
        e.Handled = true; 
    }

    public void OnScrollGesture(object? sender, ScrollGestureEventArgs e)
    {
        if ((DateTime.Now - _lastGestureTime).TotalMilliseconds < 33) 
        {
            e.Handled = true;
            return;
        }
        _lastGestureTime = DateTime.Now;

        RouteScroll(e.Delta.Y); 
        e.Handled = true;

    }

    private void RouteScroll(double deltaY)
    {
        var glanceController = App.Services.GetRequiredService<GlanceController>();
        glanceController.HandleScroll(deltaY);
    }
}