using System;
using Aprilia.Engines.Glance.Content.Templates;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace Aprilia.Views.Pages;

public partial class ProjectsPageView : UserControl
{
    public ProjectsPageView()
    {
        InitializeComponent();
    }
    private async void OnQrTapped(object? sender, TappedEventArgs e)
    {
        // 'Control' is universal. It works whether they clicked a StackPanel, Border, or TextBlock!
        if (sender is Control border && border.DataContext is ProjectItem project)
        {
            if (string.IsNullOrWhiteSpace(project.ProjectURL)) return;


            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel != null)
            {
                await topLevel.Launcher.LaunchUriAsync(new Uri(project.ProjectURL));
            }
        }
    }

}