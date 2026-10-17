using Aprilia.Interfaces;

namespace Aprilia.Models.Props;

public class Hex2TrapProps : IShapeProperties
{
    public Vertex HexMidRight2TrapTopLeft { get; set; }
    public Vertex HexMidLeft2TrapTopRight { get; set; }
    public Vertex HexBotRight2TrapBotLeft { get; set; }
    public Vertex HexBotLeft2TrapBotRight { get; set; }
    
    public double Opacity { get; set; } = 0.0;
    public double Extrusion { get; set; } = 0.0;
}