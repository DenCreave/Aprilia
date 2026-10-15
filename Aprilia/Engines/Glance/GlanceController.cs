using System;
using Avalonia.Threading;
using Aprilia.Engines.Glance;

namespace Aprilia.Engines.Glance;

public sealed class GlanceController
{
    public GlanceState State { get; }
    private GlanceContentLoader _contentLoader { get; }
    
    private readonly DispatcherTimer _physicsTimer;
    private DateTime _lastScrollTime;
    
    // camera weight props
    private double _targetTicks = 0;
    private double _progressedTicks = 0;
    
    private double _ticksRequired = 3.0; 
    private readonly int _baseCost = 3;
    private readonly int _penaltyCost = 5;
    
    private readonly int _idleDecayDelayMs = 1500; 
    private readonly double _decayRate = 0.1; 

    public GlanceController(GlanceState state, GlanceContentLoader contentLoader)
    {
        State = state;
        _contentLoader = contentLoader;
        State.MaxPages = contentLoader.Pages.Count;
        
        _physicsTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.0 / 60) };
        _physicsTimer.Tick += OnPhysicsTick;
        _physicsTimer.Start();
    }

    public void HandleScroll(double deltaY)
    {
        _lastScrollTime = DateTime.Now;
        var currentPage = _contentLoader.Pages[State.CurrentFocus];

        if (deltaY < 0 && !currentPage.IsAtBottom) return; 
        if (deltaY > 0 && !currentPage.IsAtTop) return;
        
        double tickAmount = -deltaY; 
        _targetTicks += tickAmount;
        if (State.CurrentFocus == 0 && _targetTicks < 0) _targetTicks = 0;
        if (State.CurrentFocus == State.MaxPages - 1 && _targetTicks > 0) _targetTicks = 0;
    }

    private void OnPhysicsTick(object? sender, EventArgs e)
    {
        double idleMs = (DateTime.Now - _lastScrollTime).TotalMilliseconds;
        bool isDecaying = idleMs > _idleDecayDelayMs;

        // decay trigger
        if (isDecaying)
        {
            _targetTicks = 0;           
        }


        double distance = _targetTicks - _progressedTicks;
        
        // If decaying, drift slowly (4%). If actively scrolling, move fast (15%).
        double activeLerpRate = isDecaying ? 0.04 : 0.10;
        if (Math.Abs(distance) < 0.03) 
        {
            _progressedTicks = _targetTicks;
        }
        else
        {
            _progressedTicks += distance * activeLerpRate; 
        }

        // 3. UPDATE STATE
        State.StrainProgress = Math.Round(_progressedTicks / _ticksRequired, 4);
        
        if (State.StrainProgress >= 1 && State.CurrentFocus < State.MaxPages - 1)
        {
            FlipPage(1);
        }
        else if (State.StrainProgress <= -1 && State.CurrentFocus > 0)
        {
            FlipPage(-1);
        }
    }
    
    private void FlipPage(int direction)
    {
        State.CurrentFocus += direction;
        
        _targetTicks = 0;
        _progressedTicks = 0;
        State.StrainProgress = 0;
        _ticksRequired = _penaltyCost; 

        for (int i = 0; i < _contentLoader.Pages.Count; i++)
        {
            bool isCurrent = (i == State.CurrentFocus);
            _contentLoader.Pages[i].PageOpacity = isCurrent ? 1 : 0;
        }
    }
}