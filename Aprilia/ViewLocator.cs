using System;
using System.Diagnostics.CodeAnalysis;
using Aprilia.Engines.Glance;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Aprilia.ViewModels;

namespace Aprilia;

/// <summary>
/// Given a view model, returns the corresponding view if possible.
/// </summary>
[RequiresUnreferencedCode(
    "Default implementation of ViewLocator involves reflection which may be trimmed away.",
    Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
            return null;
        
        string fullName = data.GetType().FullName!;
        string viewName=String.Empty;


        if (fullName.Contains("ViewModel"))
        {
            viewName = fullName.Replace("ViewModel", "View", StringComparison.Ordinal);
        }
        // 2. Handle your custom CMS Content!
        else if (fullName.Contains("Engines.Glance.Content"))
        {
            // Translates: Aprilia.Engines.Glance.Content.IntroContent 
            // To:         Aprilia.Views.Pages.IntroPageView
            viewName = fullName
                .Replace("Engines.Glance.Content", "Views.Pages")
                .Replace("Content", "PageView");
        }
        else
        {
            return new TextBlock { Text = $"Locator Failed!\nFull Name: {fullName}\nViewName: {viewName} "};
        }
        
        
        
        var type = Type.GetType(viewName);

        if (type != null)
        {
            var control = (Control)Activator.CreateInstance(type)!;
            control.DataContext = data;
            return control;

        }
        else
        {
            return new TextBlock { Text = "Locator Failed: Type was null" };
        }
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase || data is IGlanceContent;
    }
}