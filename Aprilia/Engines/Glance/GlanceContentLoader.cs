using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Aprilia.Engines.Glance.Content;
using Microsoft.Extensions.DependencyInjection;

namespace Aprilia.Engines.Glance;

public sealed class GlanceContentLoader
{
    public List<IGlanceContent> Pages { get; }

    public GlanceContentLoader()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => x.IsClass && !x.IsAbstract && x.IsAssignableTo(typeof(IGlanceContent)));

        Pages = new List<IGlanceContent>();
        
        foreach (var type in types)
        {
            if (ActivatorUtilities.CreateInstance(App.Services, type) is IGlanceContent content)
            {
                Pages.Add(content);
            }
        }
        
        Pages = Pages
            .Where(x => x.PageName != GlancePage.Disabled
                        && x.PageName != GlancePage.Void
                        && x.PageName != GlancePage.Dummy)
            .OrderBy(x => (int)x.PageName)
            .ToList();
        
        
        for (int i = 0; i < 9; i++)
        {
            Pages.Add(new VoidContent());
        }
        
        if (Pages.Count > 0)
        {
            Pages[0].PageOpacity = 1.0;
        }
    }
}