using System;
using System.Threading;
using System.Threading.Tasks;
using Aprilia.Engines.Security;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Aprilia.Engines.Glance.Content;

public partial class IntroContent : ObservableObject, IGlanceContent
{
    private readonly EncVault _encVault;
    private int _clickCount = 0;

    // 0 = false, 1 = true. mutex for atomic lock
    private int _easterEggActive = 0;

    [ObservableProperty] private Bitmap _currentProfileImage;
    public GlancePage PageName => GlancePage.Intro;

    [ObservableProperty] private double _pageOpacity;

    [ObservableProperty] private bool _isActivePage = false;


    [ObservableProperty] private bool _isAtTop = true;
    [ObservableProperty] private bool _isAtBottom = true;

    public DateTime? HideAt { get; set; } = null;

    public string Name => "JULIUS DEER";
    public string Profession => "COMPUTER SCIENTIST";
    public string Telegram => $"t.me/dencreave";
    public string TelegramIcon => "\uE5BC";

    public string Email => $"dencreave@gmail.com";
    public string EmailIcon => "\uE214";
    public string Location => $"Berlin";
    public string LocationIcon => $"\uE316";
    public string Relocation => $"Open to EU/Global";
    public string RelocationIcon => $"\uE5D6";
    public string BriefBoutMe => "I'm a Computer Scientist (Szeged '25), who's specialized in" +
                                 " C# .Net, backend and daily drives linux. I've " +
                                 "recently relocated to Berlin because I'm passionate " +
                                 "and dedicated to this profession and I want to be " +
                                 "part of the research and development, I want to solve " +
                                 "real world problems and work with bleeding edge technologies, like KDE or Rust.";
    public string NotaBene => "The purpose of this website is to act as a portfolio and" +
                              " give you an elaborate version on who I am compared to" +
                              " the 1 page CV as that was meant to be a 1 page " +
                              "introduction. To download the CV click the button below.";

    public IntroContent(EncVault encVault)
    {
        _encVault = encVault;
        CurrentProfileImage = _encVault.GetImage("evolution");
    }
    

    
    public async Task ImageTapped()
    {
        Interlocked.Increment(ref _clickCount);

        if (_clickCount > 10)
        {
            _clickCount = 0;
        }
        Interlocked.Exchange(ref _easterEggActive, 0);
        if (_clickCount<10)
        {
            switch (_clickCount%3)
            {
                case 0:
                {
                    CurrentProfileImage = _encVault.GetImage("evolution");
                    break;
                }
                case 1:
                {
                    CurrentProfileImage = _encVault.GetImage("flipflap");
                    break;
                }
                case 2:
                {
                    CurrentProfileImage = _encVault.GetImage("outside");
                    break;
                }
            }
        }
        else
        {
            Interlocked.Exchange(ref _easterEggActive, 1);
            CurrentProfileImage = _encVault.GetImage("easter_egg");
            Interlocked.Exchange(ref _clickCount, 0);
            await Task.Delay(1000);
            
            // what is Volatile? first time im using it. the explanation stays here
            // Volatile.Read(ref _easterEggActive) is a strict command to the CPU
            // and the compiler. It says: "Do not trust your local cache.
            // Do not optimize this. Go all the way to Main RAM and get me
            // the absolute, freshest, most accurate value of this
            // variable right this exact nanosecond."
            if (Volatile.Read(ref _easterEggActive) == 1)
            {
                CurrentProfileImage = _encVault.GetImage("evolution");
                Interlocked.Exchange(ref _clickCount, 0);
                Interlocked.Exchange(ref _easterEggActive, 0);
            }
        }
    }
}