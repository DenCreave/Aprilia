using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Aprilia.Engines.Glance.Content;

public partial class IntroContent : ObservableObject, IGlanceContent
{
    public GlancePage PageName => GlancePage.Intro;
    
    [ObservableProperty] 
    //[NotifyPropertyChangedFor(nameof(IsActivePage))]
    private double _pageOpacity;

    [ObservableProperty] private bool _isActivePage = false;
    
    
    [ObservableProperty] private bool _isAtTop = true;
    [ObservableProperty] private bool _isAtBottom = true;
    
    public DateTime? HideAt { get; set; } = null;

    public string Headline => "JULIUS DEER";

}