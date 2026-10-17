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
    
    public double TicksRequired { get; private set; } = 3.0; 
    public readonly int BaseCost = 3;
    public readonly int PenaltyCost = 5;
    
    private readonly int _idleDecayDelayMs = 1500; 
    private readonly double _decayRate = 0.1;
    
    private bool _isDecaying  { get; set; } = false;

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


    private void DecayChecker()
    {
        double idleMs = (DateTime.Now - _lastScrollTime).TotalMilliseconds;
        bool pastDecayMs = idleMs > _idleDecayDelayMs;
        // decay trigger
        if (pastDecayMs && !_isDecaying)
        {
            _targetTicks = 0;
            _isDecaying = true;
        }
        else if (pastDecayMs && _isDecaying && Math.Abs(_progressedTicks - _targetTicks) < 0.001)
        {
            _isDecaying = false;
            TicksRequired = BaseCost;
        }
    }
    private void OnPhysicsTick(object? sender, EventArgs e)
    {
        DecayChecker();
        
        double distance = _targetTicks - _progressedTicks;
        
        // If decaying, drift slowly (4%). If actively scrolling, move fast (15%).
        double activeLerpRate = _isDecaying ? 0.04 : 0.10;
        if (Math.Abs(distance) < 0.03) 
        {
            _progressedTicks = _targetTicks;
        }
        else
        {
            _progressedTicks += distance * activeLerpRate; 
        }

        //Console.WriteLine($"penalty cost atm: {TicksRequired}");
        // 3. UPDATE STATE
        State.StrainProgress = Math.Round(_progressedTicks / TicksRequired, 4);
        
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
        TicksRequired = PenaltyCost; 

        for (int i = 0; i < _contentLoader.Pages.Count; i++)
        {
            bool isCurrent = (i == State.CurrentFocus);
            _contentLoader.Pages[i].PageOpacity = isCurrent ? 1 : 0;
        }
    }
}