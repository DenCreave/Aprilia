using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Aprilia.Engines.Glance.Content;

public partial class SkillsContent : ObservableObject, IGlanceContent
{
    public GlancePage PageName => GlancePage.Skills;
    public DateTime? HideAt { get; set; }

    [ObservableProperty] private double _pageOpacity;

    [ObservableProperty] private bool _isActivePage = false;
    
    [ObservableProperty] private int _zIndex;

    [ObservableProperty] private bool _isAtTop = true;
    [ObservableProperty] private bool _isAtBottom = true;

    public string CoreSkillsText => "Core Stack";
    public string ConceptsText => "Concepts";
    public string OtherText => "Other tech I've used";
    public string InfrastructureText => "Infrastructure";
    public string LanguageText => "English IELTS Band Scores";
    public string HunLang => "Hungarian: Native";

    // The Arrays of Stats
    public string[] CoreStack { get; } = { "C# .NET", "Entity Framework", "Avalonia UI", "ASP.NET", "PostgreSQL", "SQLite", "Spectre Console"};
    public string[] Concepts { get; } = { "MVVM","MVC", "N-Layered Architecture", "SOLID", "REST APIs", "JWT Auth", "Message Queuing", "Backend","NoSQL","Serverless","Async", "Concurrency", "Race Condition" };
    public string[] OtherTech { get; } = { "xUnit", "Moq","Docker", "Blazor", "WebAssembly", "Typescript", "Angular fundamentals", "Ruby", "Ruby on Rails", "C / C++", "Samba" };
    public string[] Infrastructure { get; } = { "Linux (Arch)", "Docker", "AWS Serverless", "Azure", "GitHub  (CI/CD)", "Cloudflare Pages/DNS" };
    public string[] English { get; } = { "Reading: 9", "Listening: 8.5", "Writing 7.5", "Speaking: 7.5", "CEFR level: C1", "Date: 11/08/2025"};
    
}