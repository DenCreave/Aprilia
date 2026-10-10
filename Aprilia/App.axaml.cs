using System;
using Aprilia.Controllers;
using Aprilia.Engines.SpanningTree.Controller;
using Aprilia.Engines.SpanningTree.DotMap;
using Aprilia.Engines.SpanningTree.Theme;
using Aprilia.Engines.SpanningTree.TorrentLayer;
using Aprilia.Engines.SpanningTree.UI;
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
        
        
       /* // Services || Controllers
        services.AddSingleton<NavigationService>();


        services.AddSingleton<InitViewModel>();
        //views (their models)
        services.AddTransient<ClockViewModel>();
        services.AddTransient<AlarmViewModel>();
        services.AddTransient<MusicViewModel>();
        services.AddTransient<STCViewModel>();
        
        
        Services = services.BuildServiceProvider();
        
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var initViewModel = Services.GetRequiredService<InitViewModel>();

            desktop.MainWindow = new InitView();
        }
        */
       
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // If running on Linux/Windows Desktop
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            // If running in WebAssembly (Browser) or Mobile
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    
    
    
    
    
    
    
    
    
    
    /*
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
#if DEBUG
        this.AttachDeveloperTools();
#endif
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime singleViewFactoryApplicationLifetime)
        {
            singleViewFactoryApplicationLifetime.MainViewFactory = () => new MainView { DataContext = new MainViewModel() };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }*/
}