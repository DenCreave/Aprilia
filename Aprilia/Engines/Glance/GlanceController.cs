using System;
using System.Collections.Generic;
using Avalonia.Threading;
using Aprilia.Engines.Glance;

namespace Aprilia.Engines.Glance;

public sealed class GlanceController
{
    public GlanceState State { get; }
    private GlanceContentLoader _contentLoader { get; }
    
    private readonly HashSet<IGlanceContent> _fadingPages = new();
    
    private readonly DispatcherTimer _physicsTimer;
    private DateTime _lastScrollTime;
    
    // camera weight props
    private double _targetTicks = 0;
    private double _progressedTicks = 0;
    
    public double TicksRequired { get; private set; } = 3.0; 
    public readonly int BaseCost = 5;
    public readonly int PenaltyCost = 7;
    public readonly TimeSpan InactiveDelay = TimeSpan.FromMilliseconds(500);

    private readonly int _idleDecayDelayMs = 1500; 
    private readonly double _decayRate = 0.1;
    
    private bool _isDecaying  { get; set; } = false;
    //public bool FlipInduced { get; set; } = false;

    public GlanceController(GlanceState state, GlanceContentLoader contentLoader)
    {
        _isDecaying = false;
        state.JustFlipped = false;
        State = state;
        _contentLoader = contentLoader;
        State.MaxPages = contentLoader.Pages.Count;
        

    }

    public void HandleScroll(double deltaY)
    {
        _isDecaying = false;
        _lastScrollTime = DateTime.Now;
        var currentPage = _contentLoader.Pages[State.CurrentFocus];
        Console.WriteLine(deltaY+" delta val");
        if (deltaY < 0 && !currentPage.IsAtBottom) return; 
        if (deltaY > 0 && !currentPage.IsAtTop) return;
        
        double tickAmount = -deltaY; 
        _targetTicks += tickAmount;
        if (State.CurrentFocus == 0 && _targetTicks < 0) _targetTicks = 0;
        if (State.CurrentFocus == State.MaxPages - 1 && _targetTicks > 0) _targetTicks = 0;
        Console.WriteLine($"delta Y: {deltaY}\n Tagerticks {_targetTicks}");
    }


    private void DecayChecker()
    {
        double idleMs = (DateTime.Now - _lastScrollTime).TotalMilliseconds;
        bool pastDecayMs = idleMs > _idleDecayDelayMs;
        // decay trigger
        /*Console.WriteLine($"{idleMs}ms {pastDecayMs}");
        Console.WriteLine($"is decaying value: {_isDecaying}");*/
        if (pastDecayMs && !_isDecaying)
        {
            _targetTicks = 0;
            _isDecaying = true;
        }
        if (pastDecayMs && _isDecaying && Math.Abs(State.StrainProgress) < 0.01)
        {
            _isDecaying = false;
            _progressedTicks = 0;
            TicksRequired = BaseCost;
        }
    }

    public void UpdatePhysics()
    {
        if (State.JustFlipped) 
        {
            State.JustFlipped = false;
            TicksRequired = PenaltyCost;
        }
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
        
        if (_fadingPages.Count > 0)
        {
            _fadingPages.RemoveWhere(page => 
            {
                if (page.HideAt.HasValue && DateTime.Now >= page.HideAt.Value)
                {
                    //page.IsActivePage = false; // Remove from GPU!
                    page.HideAt = null;        // Clear the timer
                    return true;               // Tells HashSet to stop tracking this page
                }
                return false; // Keep tracking it, timer hasn't finished yet
            });
        }
        
        
    }
    
    private void FlipPage(int direction)
    {
        int oldFocus = State.CurrentFocus;

        State.CurrentFocus = Math.Clamp(State.CurrentFocus + direction, 0, State.MaxPages - 1);
        // Safety check: If the clamp prevented us from moving, don't do anything!
        if (oldFocus == State.CurrentFocus) return; 

        
        
        
        _targetTicks = 0;
        _progressedTicks = 0;
        State.StrainProgress = 0;
        State.JustFlipped = true;
       // TicksRequired = PenaltyCost; 
        /*
        for (int i = 0; i < _contentLoader.Pages.Count; i++)
        {
            bool isCurrent = (i == State.CurrentFocus);
            _contentLoader.Pages[i].PageOpacity = isCurrent ? 1 : 0;
        }*/
        // 1. FADE OUT THE OLD PAGE
        var oldPage = _contentLoader.Pages[oldFocus];
        oldPage.PageOpacity = 0;
        oldPage.HideAt = DateTime.Now.Add(InactiveDelay); // Set the death timer
        oldPage.ZIndex = 0;
        _fadingPages.Add(oldPage); // Add to the Garbage Collector watch list

        // 2. FADE IN THE NEW PAGE
        var newPage = _contentLoader.Pages[State.CurrentFocus];
        newPage.PageOpacity = 1;
        //newPage.IsActivePage = true; // Wake it up instantly
        newPage.HideAt = null;       // Cancel any death timers
        newPage.ZIndex = 1;
        _fadingPages.Remove(newPage); // Remove from GC watch list (in case user scrolled back quickly)
        

    }
    
    
}