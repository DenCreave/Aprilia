using System;
using System.Collections.Generic;
using Aprilia.Engines.Glance.Content.Templates;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Aprilia.Engines.Glance.Content;

public partial class WorkContent: ObservableObject, IGlanceContent
{
    public GlancePage PageName => GlancePage.Work;
    
    public DateTime? HideAt { get; set; }

    [ObservableProperty] private double _pageOpacity;

    [ObservableProperty] private bool _isActivePage = false;
    
    [ObservableProperty] private int _zIndex;

    [ObservableProperty] private bool _isAtTop = true;
    [ObservableProperty] private bool _isAtBottom = true;

    public List<TimelineItem> Items { get; } = new()
    {
        new TimelineItem
        {
            Place = "EPAM Campus",
            Title = "ASP.NET Backend Developer (Project Education)",
            DateRange = "May 2026 - Aug 2026",
            Description = "Simulated Agile enterprise environment for EPAM Campus Students who successfully finish the course.",
            AcquiredSkills = ["AWS Lambda", "PostgreSQL", "JWT Auth", "Agile/Scrum"]
        },
        new TimelineItem
        {
            Place = "Education First",
            Title = "Salesforce Apex Developer",
            DateRange = "July 2025 - Sept 2025",
            Description = "Worked on the backend as an apex developer.",
            AcquiredSkills = ["Salesforce Apex", "Legacy System Refactoring"]
        },
        new TimelineItem
        {
            Place = "Evosoft",
            Title = ".NET WPF Intern",
            DateRange = "Feb 2023",
            Description = "WPF desktop Development",
            AcquiredSkills = ["C#", "WPF", "TIA Portal"]
        },
        new TimelineItem
        {
            Place = "Bitotron",
            Title = ".Net Blazor internship",
            DateRange = "Summer 2022",
            Description = "Blazor development",
            AcquiredSkills = ["C#", "Blazor"]
        }
    };

}