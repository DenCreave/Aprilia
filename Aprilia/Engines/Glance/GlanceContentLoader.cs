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
        
        // Generate 4 dummy pages to test the scroll bounds
        for (int i = 0; i < 4; i++)
        {
            Pages.Add(new DummyContent() 
            { 
                Headline = $"THIS IS PAGE {i}",
                BodyText = "Lorem ipsum dolor sit amet. \n\nImagine a picture here.\n\nImagine 10 pictures here.\n\nKeep scrolling down...",
                PageOpacity = (i == 0) ? 1 : 0 // <-- THIS FIXES IT. Page 0 starts visible!
            });
        }
        
        foreach (var type in types)
        {
            if (ActivatorUtilities.CreateInstance(App.Services, type) is IGlanceContent content)
            {
                Pages.Add(content);
            }
        }
        
        Pages = Pages.OrderBy(x => (int)x.PageName).ToList();
    }
}