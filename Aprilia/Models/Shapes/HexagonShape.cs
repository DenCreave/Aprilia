using Aprilia.Interfaces;
using Aprilia.Models.Props;

namespace Aprilia.Models.Shapes;

public sealed class HexagonShape : IVexShape<Hex2TrapProps>
{
    public Vertex[] Shapes { get; init; }

    public Hex2TrapProps UniqueProps { get; init; }

    public HexagonShape()
    {
        
        Shapes = new[]
        {
            new Vertex { Cox = 2, Coy = -19 },   // Top Right
            new Vertex { Cox = 10, Coy = 0 },    // Mid Right
            new Vertex { Cox = 1, Coy = 13 },    // Bot Right
            new Vertex { Cox = -1, Coy = 13 },   // Bot Left
            new Vertex { Cox = -10, Coy = 0 },   // Mid Left
            new Vertex { Cox = -2, Coy = -19 }   // Top Left
        };
        UniqueProps = new Hex2TrapProps()
        {
            HexMidRight2TrapTopLeft = Shapes[1],
            HexBotRight2TrapBotLeft = Shapes[2],
            HexMidLeft2TrapTopRight = Shapes[4],
            HexBotLeft2TrapBotRight = Shapes[3],
        };
    }
}