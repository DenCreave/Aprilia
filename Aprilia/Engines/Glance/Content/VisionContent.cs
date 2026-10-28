using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Aprilia.Engines.Glance.Content;

public partial class VisionContent: ObservableObject, IGlanceContent
{
    public GlancePage PageName => GlancePage.Vision;
    public DateTime? HideAt { get; set; }

    [ObservableProperty] private double _pageOpacity;

    [ObservableProperty] private bool _isActivePage = false;
    
    [ObservableProperty] private int _zIndex;

    [ObservableProperty] private bool _isAtTop = true;
    [ObservableProperty] private bool _isAtBottom = true;

    public string Title => "Vision & Perspective";
    public string Body => "When I was a kid, I was taught that the most valuable asset an individual " +
                          "can have is a vision, an idea. And those are not enough without the grit to " +
                          "see it through. Only as i went through university i came to understand what " +
                          "it meant. A vision sets the object of passion, when one is able to visualize " +
                          "their own future, passion is there as a force to make it happen, but to make " +
                          "that future an ardent reality one needs perseverance.\n\nI am creative. It's " +
                          "a perspective. I create something out of nothing and i love it, it feels like " +
                          "im actively breaking the law of equilibrium. \n\nI love computer science. This " +
                          "profession harnesses the individual with a beautiful perspective. I can describe " +
                          "it in 2 sentences:\n1) We are the modern days philosophers. - Philosophers are the " +
                          "researchers of the soul, and we have the math to prove if a realization is only a " +
                          "perspective or a universal truth.\n2) we are the physicists who possess the philosophers " +
                          "stone. -Physicists are the ones who try to uncover the essence of the universe, the " +
                          "philosophers stone is a mythical item that allows one to discard the \"cost\" in " +
                          "the equation of equilibrium. A physicist requires processed uranium, concrete, " +
                          "steel, coal, a huge facility, all these to boil water. Were they to possess the " +
                          "philosophers stone, they could instantly check their theses against reality. We " +
                          "computer scientist can do that, a computer is an essential household item, all we " +
                          "need is just time, and we can always just create something with time being the only " +
                          "price.\n\nAt university we were taught that the reason we have to study complexity " +
                          "theory, is so that when we meet a new real world problem, we'd be able to accurately " +
                          "recognize which complexity set it belongs to. And that's where it gets beautiful. " +
                          "how do we define a \"problem\"? It's essentially the same as above, its just an " +
                          "equation, one side being an unknown the other being the answer to it. Travelling " +
                          "Salesman Problem... Santa delivering presents... Anything we can think of. Life is a " +
                          "halting problem, as long as you are alive you can declare you are alive, but once " +
                          "you die, you can no longer declare you're not alive. - Love, is a NON-Halting problem, " +
                          "It's complexity is CoRE. (if meet in person feel free to ask me to elaborate more " +
                          "on why love is a non-halting problem, but id like you to think of it yourself.) Also, " +
                          "paradoxes dont exist, it was just \"us\" who never understood the nature of the " +
                          "problem. For example building a dam. How high and how durable should we build the dam? " +
                          "If we allocate too much money on it people will complain that we raised a 500m wall " +
                          "unreasonably, if the dam is too low or too weak the city gets flooded. This problem is " +
                          "a non-halting problem, the exact cost and structure of the dam will only be correct once " +
                          "comes a tsunami that breaks through and we learn the answer then. So the infinite loop " +
                          "exists while the answer is true (the dam holds) and we get the answer on fail (false)";
}