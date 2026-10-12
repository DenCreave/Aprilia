using System;
using Avalonia.Threading;

namespace Aprilia.Engines.Glance;

public sealed class GlanceController
{
    public GlanceState State { get; }
    private GlanceContentLoader _contentLoader { get; }
    
    private readonly DispatcherTimer _physicsTimer;
    private DateTime _lastScrollTime;
    
    // The Dynamic Friction Variables
    private double _currentTicks = 0;
    private double _ticksRequired = 3.0; // Starts at 3
    private readonly double _baseCost = 3.0;
    private readonly double _penaltyCost = 5.0;

    public GlanceController(GlanceState state, GlanceContentLoader contentLoader)
    {
        State = state;
        _contentLoader = contentLoader;
        State.MaxPages = contentLoader.Pages.Count;
        
        // 60fps loop strictly for handling the 1.5s decay
        _physicsTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        _physicsTimer.Tick += OnPhysicsTick;
        _physicsTimer.Start();
    }

    public void HandleScroll(double deltaY)
    {
        _lastScrollTime = DateTime.Now;
        
        // 1. Get the current page to check if the user is reading text
        var currentPage = _contentLoader.Pages[State.CurrentFocus];

        // 2. Text-Scroll Bypass
        // If scrolling DOWN (deltaY < 0) but text isn't at the bottom, do nothing.
        if (deltaY < 0 && !currentPage.IsAtBottom) return; 
        
        // If scrolling UP (deltaY > 0) but text isn't at the top, do nothing.
        if (deltaY > 0 && !currentPage.IsAtTop) return;

        // 3. Accumulate Strain
        double tickAmount = deltaY < 0 ? 1.0 : -1.0; 
        _currentTicks += tickAmount;

        // Hard clamp at the absolute ends of the CV
        if (State.CurrentFocus == 0 && _currentTicks < 0) _currentTicks = 0;
        if (State.CurrentFocus == State.MaxPages - 1 && _currentTicks > 0) _currentTicks = 0;

        State.StrainProgress = _currentTicks / _ticksRequired;

        // 4. Check for Page Flip
        if (State.StrainProgress >= 1.0 && State.CurrentFocus < State.MaxPages - 1)
        {
            FlipPage(1);
        }
        else if (State.StrainProgress <= -1.0 && State.CurrentFocus > 0)
        {
            FlipPage(-1);
        }
    }

    private void FlipPage(int direction)
    {
        State.CurrentFocus += direction;
        
        // Reset physics and apply the "fast scrolling" penalty
        _currentTicks = 0;
        State.StrainProgress = 0;
        _ticksRequired = _penaltyCost; 

        // Update Opacities
        for (int i = 0; i < _contentLoader.Pages.Count; i++)
        {
            bool isCurrent = (i == State.CurrentFocus);
            _contentLoader.Pages[i].PageOpacity = isCurrent ? 1.0 : 0.0;
        }
    }

    private void OnPhysicsTick(object? sender, EventArgs e)
    {
        double idleSeconds = (DateTime.Now - _lastScrollTime).TotalSeconds;

        // If the user hasn't scrolled for 1.5 seconds...
        if (idleSeconds > 1.5)
        {
            // 1. Remove the friction penalty
            _ticksRequired = _baseCost;

            // 2. Smoothly decay the strain back to 0
            if (Math.Abs(_currentTicks) > 0.05)
            {
                _currentTicks -= _currentTicks * 0.1; // 10% decay per frame
                State.StrainProgress = _currentTicks / _ticksRequired;
            }
            else if (_currentTicks != 0)
            {
                // Snap to absolute zero to stop the math
                _currentTicks = 0;
                State.StrainProgress = 0;
            }
        }
    }

}