namespace Aprilia.Engines.Glance.Content.Templates;

public class ProjectItem
{
    public string Title { get; set; } = string.Empty;
    public string QRPath { get; set; } = string.Empty;
    public string ProjectURL { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Footnote => $"Scan / Tap\nor visit:\n{ProjectURL}";
    
    public string[] TechStack { get; set; } = System.Array.Empty<string>();
}