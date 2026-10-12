using CommunityToolkit.Mvvm.ComponentModel;

namespace Aprilia.Engines.Glance.Content;

public partial class IntroContent : ObservableObject, IGlanceContent
{
    public GlancePage PageName => GlancePage.Landing;
    
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(IsActivePage))]
    private double _pageOpacity;

    public bool IsActivePage => PageOpacity > 0.0;

    /*[ObservableProperty] 
       private double _pageOpacity = 0.0;

       [ObservableProperty]
       private bool _isActivePage = false;

       partial void OnPageOpacityChanged(double value)
       {
           IsActivePage = value > 0.0;
       }*/
    

    //default is true, it the page is short, the it has no native scrollbars
    //so its always at top and at bottom.
    [ObservableProperty] private bool _isAtTop = true;
    [ObservableProperty] private bool _isAtBottom = true;
    
    public string Headline => "JULIUS DEER";

}