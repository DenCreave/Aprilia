using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Aprilia.Engines.Glance;

public sealed class GlanceContentLoader
{
    public List<IGlanceContent> Pages { get; }

    public GlanceContentLoader()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => x.IsClass && !x.IsAbstract && x.IsAssignableTo(typeof(IGlanceContent)));

        var pages = new List<IGlanceContent>();

        foreach (var type in types)
        {
            if (ActivatorUtilities.CreateInstance(App.Services, type) is IGlanceContent content)
            {
                pages.Add(content);
            }
        }
        
        Pages = pages.OrderBy(x => (int)x.PageName).ToList();
    }
}