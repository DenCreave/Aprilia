using Aprilia.Interfaces;
using Aprilia.Models.Props;

namespace Aprilia.Models.Shapes;

public class TrapezoidShape : IVexShape<Hex2TrapProps>
{
    public Vertex[] Shapes { get; init; }
    public Hex2TrapProps UniqueProps { get; set; }

    public TrapezoidShape()
    {
        
        Shapes = new[]
        {
            // i mean, they are responsive to loading, so...
            new Vertex { Cox = 0, Coy = 0 },
            new Vertex { Cox = 0, Coy = 0 },
            new Vertex { Cox = 0, Coy = 0 },
            new Vertex { Cox = 0, Coy = 0 },
        };
        UniqueProps = new Hex2TrapProps()
        {
            HexMidRight2TrapTopLeft = Shapes[0], // Top Left
            HexMidLeft2TrapTopRight = Shapes[1], // Top Right
            HexBotLeft2TrapBotRight = Shapes[2], // Bottom Right
            HexBotRight2TrapBotLeft = Shapes[3], // Bottom Left
        };
    }
}