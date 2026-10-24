using System;
using System.Collections.Generic;
using Aprilia.Engines.Glance.Content.Templates;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Aprilia.Engines.Glance.Content;

public partial class EducationContent : ObservableObject, IGlanceContent
{
    public GlancePage PageName => GlancePage.Education;
    
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
            Title = ".NET Backend Specialization",
            DateRange = "2025 - 2026",
            Description = "A comprehensive .NET backend specialization focused on enterprise architecture, completed by building and deploying a full-scale project from the ground up.",
            AcquiredSkills = ["Clean Code", "Microservices", "CI/CD", "N-layered","Repository pattern","Pipeline pattern", "Azure", "Claim-Basd Auth (JWT/RBAC)", "Custom Middleware","Angular Fundamentals","Devops Fundamentals","Polyglot Persistance (SQL/NoSQL)"]
        },
        new TimelineItem
        {
            Place = "University of Szeged Faculty of Science and Informatics",
            Title = "Computer Science",
            DateRange = "2019 - 2025",
            Description = "Bachelors Degree",
            AcquiredSkills = ["Computer Scientist", "Complexity theory", "Formal Languages", "Discrete mathematics", "Geoinformatics"]
        },
        new TimelineItem
        {
            Place = "Budapest Műszaki Szakképzési Centrum",
            Title = "CAD / CAM / CNC",
            DateRange = "2017 - 2019",
            Description = "Manual and 2.5D CNC through G codes. First year was programming.",
            AcquiredSkills = ["CAD", "CAM", "CNC", "Programming fundamentals"]
        },
    };
}