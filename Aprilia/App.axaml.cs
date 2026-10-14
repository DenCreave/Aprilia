using System;
using Aprilia.Controllers;
using Aprilia.Engines.Glance;
using Aprilia.Engines.Mediator;
using Aprilia.Engines.SpanningTree.Controller;
using Aprilia.Engines.SpanningTree.DotMap;
using Aprilia.Engines.SpanningTree.Theme;
using Aprilia.Engines.SpanningTree.TorrentLayer;
using Aprilia.Engines.SpanningTree.UI;
using Aprilia.Engines.SpanningTree.UI.Entropy;
using Aprilia.Models;
using Aprilia.Services;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Aprilia.ViewModels;
using Aprilia.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Aprilia;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        
        // Core Engin
        services.AddSingleton<AnimatrixController>();
        services.AddSingleton<DotMapLoader>();
        services.AddSingleton<TorrentLayerLoader>();
        services.AddSingleton<ThemeBinderLoader>();
        services.AddSingleton<SpanningTreeController>();
        services.AddSingleton<StyleArchive>();
        services.AddSingleton<UIThemeLoader>();
        services.AddSingleton<SpanningTreeTheme>();
        services.AddSingleton<CameraState>();
        services.AddSingleton<EntropyStyle>();
        services.AddSingleton<NavigationService>();
        services.AddSingleton<GlanceState>();
        services.AddSingleton<GlanceController>();
        services.AddSingleton<GlanceContentLoader>();
        services.AddSingleton<TwinRenderMediator>();
        
        //views (their models)
        services.AddSingleton<InitViewModel>();
        services.AddTransient<LandingCVViewModel>();
        
        Services = services.BuildServiceProvider();
        var navigator = Services.GetRequiredService<NavigationService>();
        navigator.NavigateTo<LandingCVViewModel>();
       
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Linux/Windows Desktop
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<InitViewModel>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            // webassembly
            singleViewPlatform.MainView = new InitView
            {
                DataContext = Services.GetRequiredService<InitViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}