using System;
using System.Collections.Generic;
using Aprilia.Engines.Glance.Content.Templates;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Aprilia.Engines.Glance.Content;

public partial class ProjectsContent: ObservableObject, IGlanceContent
{
    public GlancePage PageName => GlancePage.Projects;
    public DateTime? HideAt { get; set; }

    [ObservableProperty] private double _pageOpacity;

    [ObservableProperty] private bool _isActivePage = false;
    
    [ObservableProperty] private int _zIndex;

    [ObservableProperty] private bool _isAtTop = true;
    [ObservableProperty] private bool _isAtBottom = true;
    
    public List<ProjectItem> Projects { get; } = new()
    {
        new ProjectItem
        {
            Title = "SirenDisplay",
            Description = "I created this project because I had an issue with sleeping through alarm clocks, and I decided to make my own stylish alarm clock that will allow me to set a playlist as an alarm sound. I made it so I could set up playlists, for different moods. In version 1.1 I also introduced a stylish animated background framework, that framework is also used in this CV project, although ive added a different theme this time, that would be more fitting to a web layout. SirenDisplay runs on my raspberry pi which I've equipped with a touch screen. It's truly tabletop accessory! Autologin, savestates and systemctl enables me to even account for a power outage during the night!",
            TechStack = ["C#", "AvaloniaUI", "Fonts", "Skia", "MVVM", "SOLID", "Linux", "Concurrency", "LibVLC", "Audio", "Direct Rendering"],
            QRPath = "avares://Aprilia/Assets/QR/siren_qr.svg",
            ProjectURL = "https://github.com/DenCreave/SirenDisplay"
        },
        new ProjectItem
        {
            Title = "Devii",
            Description = "This is my fun project, I made this so I could show it to my friends, have them play it and watch as they die inside. To make it worthwhile, I used this project to learn concurrency/multi threading and thread safety. This project contains my first ever watchdogs and async/concurrent programming. I really fancied the terminal/ascii look, so i picked SpectreConsole as the xplat tui, so I could run it natively on linux, and my friends could play it on windows.",
            TechStack = ["C#", "SpectreConsole", "Console Game", "Ascii", "NSFW", "Crossplatform", "tui", "Concurrency"],
            QRPath = "avares://Aprilia/Assets/QR/devii_qr.svg",
            ProjectURL = "https://github.com/DenCreave/Devii"
        },
        new ProjectItem
        {
            Title = "SilentGrub",
            Description = "A silent hill grub theme. After years I just wasn't content with the grub theme I could find online so I made one for myself. I posted it online so if anyone takes a liking to it could use it.",
            TechStack = ["GRUB", "Linux"],
            QRPath = "avares://Aprilia/Assets/QR/grub_qr.svg",
            ProjectURL = "https://github.com/DenCreave/SilentGrub"
        },
        new ProjectItem
        {
            Title = "Aprilia",
            Description = "This the this website you are looking at. I wanted to keep my CV.pdf 1 page so it could act as an aggregated info about me, nowadays CV-s are read less and everyone has their own opinion how it should look like and how detailed. So chose to leave it as a thoroughly designed one pager and this would serve as my portfolio. I've also read a doomposting that on average a CV is looked at on average for 6 seconds, that's more than enough attention to find the link here, where the reviewer can just smoothly scroll through my stats and competencies.",
            TechStack = ["C#", "AvaloniaUI", "Fonts", "MVVM", "SOLID", "WebAssembly", "Direct Rendering", "Concurrency"],
            QRPath = "avares://Aprilia/Assets/QR/aprilia_qr.svg",
            ProjectURL = "https://github.com/DenCreave/Aprilia"
        }
    };

}