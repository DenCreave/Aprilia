using System;
using Aprilia.Engines.Security;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Aprilia.Engines.Glance.Content;

public partial class CharacterContent: ObservableObject, IGlanceContent
{
    public GlancePage PageName => GlancePage.Character;
    public DateTime? HideAt { get; set; }

    [ObservableProperty] private double _pageOpacity;

    [ObservableProperty] private bool _isActivePage = false;
    
    [ObservableProperty] private int _zIndex;

    [ObservableProperty] private bool _isAtTop = true;
    [ObservableProperty] private bool _isAtBottom = true;

    public CharacterContent(EncVault cache)
    {
        ThroughTheSky = cache.GetImage("sky");
    }

    public string Title => "Character";
    public string P1_Intro => "This is the section where I'd talk about myself without any " +
                              "professional setting. I find it irrelevant but on interviews " +
                              "i do get asked. I don't know how my hobbies change writing code, " +
                              "maybe its for \"breaking the ice\".";
    public string P2_Anime => "I cherish in many things. I value things, activities and " +
                              "people that are true to their essences. For example I like anime, " +
                              "there exists some truly honest and beautiful works out there which " +
                              "I find value in. These values are not always found or seen and I " +
                              "wanted others to see them as well. So I used to live 3 years at the " +
                              "college dormitory, and organized a weekly anime club with a friend. " +
                              "We'd watch shows and movies, either for represents something of value " +
                              "or because something was beautiful or just out of fun. I started this " +
                              "club and led it for 1 semester, only later I've learned that once " +
                              "ive left, 3 years later the club was still ongoing. The guy who " +
                              "overtook after me still organized it after i left. He also let me " +
                              "know its thanks to me that he started to watch and appreciate anime again. :) <3";
    public string P3_Skydiving => "I love adrenalin. I used to ride motorbikes as well " +
                                  "and when i graduated i celebrated by going skydiving twice. " +
                                  "Sadly it was only tandem jumping. but from a 4.4kms altitude, " +
                                  "so we had a whole minute of free fall. The moment we jumped out " +
                                  "of the plane... everything went silent, all i could see was the " +
                                  "sky->ground->sky->ground->sky->grouuuuuuuuuuuuuuuuuuuuuund 240kms/h! " +
                                  "then when chute opened and we were spinning down. I went there twice " +
                                  "actually. It changed my life, for a week or two afterwards when i laid " +
                                  "down in bed or closed my eyes i still felt like im flying through the sky, " +
                                  "if a stronger wind blown into my face i got flashbacks. Once i get my career " +
                                  "set, im going to do the AFF (Accelerated Free Fall) course, and get my USPA " +
                                  "license and become and an official skydiver.";
    
    public Bitmap ThroughTheSky  {get; set;}
    public string P4_Community => "I also helped organize the freshmen camp at university. The community " +
                                  "both at our faculty and both in the dormitory was valuable. And i " +
                                  "always made sure to pass on the value I've been given by my upperclassmen, " +
                                  "so the chain never broke. I've also seen some of my freshmen's become " +
                                  "an organizer later as well for this reason also.";
    public string P5_Outro => "Needless to say i also fancy music.";
}