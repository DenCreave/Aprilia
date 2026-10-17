using System;
using Aprilia.Controllers;
using Aprilia.Engines.Glance;
using Aprilia.Interfaces;
using Aprilia.Models;
using Aprilia.Models.Shapes;
using Avalonia;
using Avalonia.Media;

namespace Aprilia.Engines.LoadingHUD;

public class Section9HUD : ILoadingHUD
{
    private readonly CameraState _camera;
    private readonly GlanceContentLoader _contentLoader;
    private readonly GlanceController _glanceController;
    private readonly AnimatrixController _animatrixController;


    private readonly SolidColorBrush _hexagonBrush;
    private readonly SolidColorBrush _trapezoidBrush;

    public HexagonShape[] Hexagons;
    public TrapezoidShape[] Trapezoids;

    private readonly HexagonShape _baseHexagon;
    private readonly TrapezoidShape _baseTrapezoid;
    private readonly double RADIUS = 50;
    private double GAP = 5.0;
    private double GAP_ANGLE;
    private double LENGTHMULTIPLIER = 0.4;

    public Section9HUD(CameraState camera,
        GlanceContentLoader contentLoader,
        GlanceController glanceController,
        AnimatrixController animatrixController)
    {
        _camera = camera;
        _contentLoader = contentLoader;
        _glanceController = glanceController;
        _animatrixController = animatrixController;
        _hexagonBrush = new SolidColorBrush(Colors.Cyan);
        _trapezoidBrush = new SolidColorBrush(Colors.HotPink);
        Hexagons = new HexagonShape[_glanceController.PenaltyCost];
        Trapezoids = new TrapezoidShape[_glanceController.PenaltyCost];
        for (int i = 0; i < _glanceController.PenaltyCost; i++)
        {
            Hexagons[i] = new HexagonShape();
            Trapezoids[i] = new TrapezoidShape();
        }

        _baseHexagon = new HexagonShape();
        foreach (var vertex in _baseHexagon.Shapes)
        {
            vertex.Coy -= RADIUS;
        }

        _baseTrapezoid = new TrapezoidShape();
        
        // 1. THINNING LOGIC (Your exact vector math)
        double startX = _baseHexagon.UniqueProps.HexMidRight2TrapTopLeft.Cox;
        double startY = _baseHexagon.UniqueProps.HexMidRight2TrapTopLeft.Coy;

        double vectorMidToBottomX = _baseHexagon.UniqueProps.HexBotRight2TrapBotLeft.Cox - startX;
        double vectorMidToBottomY = _baseHexagon.UniqueProps.HexBotRight2TrapBotLeft.Coy - startY;

        double thinnedBotX = startX + (vectorMidToBottomX * LENGTHMULTIPLIER);
        double thinnedBotY = startY + (vectorMidToBottomY * LENGTHMULTIPLIER);

        // 2. GAP LOGIC (Convert pixels to Radians)
        GAP_ANGLE = GAP / RADIUS;

        // 3. APPLY THE GAP (Rotate the thinned points along the circle)
        var tempTop = new Vertex { Cox = startX, Coy = startY };
        var tempBot = new Vertex { Cox = thinnedBotX, Coy = thinnedBotY };

        _animatrixController.Rotate(
            tempTop, 
            _baseTrapezoid.UniqueProps.HexMidRight2TrapTopLeft, 
            GAP_ANGLE);

        _animatrixController.Rotate(
            tempBot, 
            _baseTrapezoid.UniqueProps.HexBotRight2TrapBotLeft, 
            GAP_ANGLE);
        
    }


    public void UpdateHUD()
    {
        UpdateHexagons();
        UpdateTrapezoids();
    }

    private void UpdateTrapezoids()
    {
        double absStrain = Math.Abs(_camera.StrainProgress);
        int activeNodes = (int)_glanceController.TicksRequired;
        double step = 1.0 / (activeNodes * 2.0);
        
        double sliceAngle = (Math.PI * 2) / activeNodes;

        // 1. CALCULATE THE TARGET RIGHT EDGE (Left edge of the NEXT hexagon, minus gap)
        double startLeftX = _baseHexagon.UniqueProps.HexMidLeft2TrapTopRight.Cox;
        double startLeftY = _baseHexagon.UniqueProps.HexMidLeft2TrapTopRight.Coy;
        double vecLeftX = _baseHexagon.UniqueProps.HexBotLeft2TrapBotRight.Cox - startLeftX;
        double vecLeftY = _baseHexagon.UniqueProps.HexBotLeft2TrapBotRight.Coy - startLeftY;
        
        Vertex thinnedBotLeft = new Vertex { Cox = startLeftX + vecLeftX * LENGTHMULTIPLIER, Coy = startLeftY + vecLeftY * LENGTHMULTIPLIER };

        Vertex targetTopRight = new Vertex();
        Vertex targetBotRight = new Vertex();
        double targetRotation = sliceAngle - GAP_ANGLE;
        
        _animatrixController.Rotate(_baseHexagon.UniqueProps.HexMidLeft2TrapTopRight, targetTopRight, targetRotation);
        _animatrixController.Rotate(thinnedBotLeft, targetBotRight, targetRotation);

        // 2. CALCULATE INDEPENDENT SWEEP ANGLES (Your brilliant observation!)
        Vertex topLeft = _baseTrapezoid.UniqueProps.HexMidRight2TrapTopLeft;
        Vertex botLeft = _baseTrapezoid.UniqueProps.HexBotRight2TrapBotLeft;

        double topSweep = GetSweepAngle(topLeft, targetTopRight);
        double botSweep = GetSweepAngle(botLeft, targetBotRight);

        // 3. APPLY TO ACTIVE TRAPEZOIDS
        for (int i = 0; i < activeNodes; i++)
        {
            double arcStart = (i * 2 + 1) * step;
            double localStrain = Math.Clamp((absStrain - arcStart) / step, 0.0, 1.0);
            Trapezoids[i].UniqueProps.Extrusion = localStrain;

            if (localStrain > 0)
            {
                Trapezoids[i].UniqueProps.HexMidRight2TrapTopLeft.Cox = topLeft.Cox;
                Trapezoids[i].UniqueProps.HexMidRight2TrapTopLeft.Coy = topLeft.Coy;
                Trapezoids[i].UniqueProps.HexBotRight2TrapBotLeft.Cox = botLeft.Cox;
                Trapezoids[i].UniqueProps.HexBotRight2TrapBotLeft.Coy = botLeft.Coy;

                // The inner circle (botSweep) will naturally rotate faster than the outer circle!
                _animatrixController.Rotate(topLeft, Trapezoids[i].UniqueProps.HexMidLeft2TrapTopRight, topSweep * localStrain);
                _animatrixController.Rotate(botLeft, Trapezoids[i].UniqueProps.HexBotLeft2TrapBotRight, botSweep * localStrain);
            }
        }
    }

    // Helper to find the exact angle between two points on a circle
    private double GetSweepAngle(Vertex start, Vertex end)
    {
        double startAngle = Math.Atan2(start.Coy, start.Cox);
        double endAngle = Math.Atan2(end.Coy, end.Cox);
        double sweep = endAngle - startAngle;
        if (sweep < 0) sweep += Math.PI * 2;
        return sweep;
    }


    private void UpdateHexagons()
    {
        double absStrain = Math.Abs(_camera.StrainProgress);
        int activeNodes = (int)_glanceController.TicksRequired; // 3 or 5
        double step = 1.0 / (activeNodes * 2.0); //todo: steps might be incorrect cos no PI

        for (int i = 0; i < activeNodes; i++)
        {
            double nodeStart = (i * 2) * step;
            Hexagons[i].UniqueProps.Opacity = Math.Clamp((absStrain - nodeStart) / step, 0.0, 1.0);
        }
    }


    public void Draw(DrawingContext context, double screenWidth, double screenHeight)
    {
        int activeNodes = (int)_glanceController.TicksRequired;
        if (activeNodes <= 0) return;

        double angleStep = 360.0 / activeNodes;
        
        // LIVE SCREEN CENTERING
        double centerX = screenWidth * 0.3; 
        double centerY = screenHeight * 0.2;

        using (context.PushTransform(Matrix.CreateTranslation(centerX, centerY)))
        {
            for (int i = 0; i < activeNodes; i++)
            {
                using (context.PushTransform(Matrix.CreateRotation(Math.PI / 180.0 * (i * angleStep))))
                {
                    DrawHex(context, Hexagons[i]);
                    DrawTrap(context, Trapezoids[i]);
                }
            }
        }
    }
    
    private void DrawHex(DrawingContext context, HexagonShape hex)
    {
        if (hex.UniqueProps.Opacity <= 0) return;

        using (context.PushOpacity(hex.UniqueProps.Opacity))
        {
            DrawShape(context, _baseHexagon, _hexagonBrush);
        }
    }

    private void DrawTrap(DrawingContext context, TrapezoidShape trap)
    {
        if (trap.UniqueProps.Extrusion <= 0) return;

        var p0 = new Point(trap.UniqueProps.HexMidRight2TrapTopLeft.Cox, trap.UniqueProps.HexMidRight2TrapTopLeft.Coy); // Inner Start
        var p1 = new Point(trap.UniqueProps.HexMidLeft2TrapTopRight.Cox, trap.UniqueProps.HexMidLeft2TrapTopRight.Coy); // Inner End
        var p2 = new Point(trap.UniqueProps.HexBotLeft2TrapBotRight.Cox, trap.UniqueProps.HexBotLeft2TrapBotRight.Coy); // Outer End
        var p3 = new Point(trap.UniqueProps.HexBotRight2TrapBotLeft.Cox, trap.UniqueProps.HexBotRight2TrapBotLeft.Coy); // Outer Start

        // Calculate exact radii from the center (0,0)
        double innerRadius = Math.Sqrt(p0.X * p0.X + p0.Y * p0.Y);
        double outerRadius = Math.Sqrt(p3.X * p3.X + p3.Y * p3.Y);

        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            ctx.BeginFigure(p0, isFilled: true);
            
            // Curve along the inner ring
            ctx.ArcTo(p1, new Size(innerRadius, innerRadius), 0, false, SweepDirection.Clockwise);
            
            // Straight line down to the outer ring
            ctx.LineTo(p2);
            
            // Curve BACKWARDS along the outer ring
            ctx.ArcTo(p3, new Size(outerRadius, outerRadius), 0, false, SweepDirection.CounterClockwise);
            
            ctx.EndFigure(isClosed: true);
        }

        context.DrawGeometry(_trapezoidBrush, null, geometry); 
    }



    // Helper method to convert your Vertex array into an Avalonia geometry
    private void DrawShape(DrawingContext context, IVexShape shape, IBrush brush)
    {
        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            // Start at the first vertex
            ctx.BeginFigure(new Point(shape.Shapes[0].Cox, shape.Shapes[0].Coy), isFilled: true);

            // Draw lines to the rest of the vertices
            for (int j = 1; j < shape.Shapes.Length; j++)
            {
                ctx.LineTo(new Point(shape.Shapes[j].Cox, shape.Shapes[j].Coy));
            }

            ctx.EndFigure(isClosed: true);
        }

        // Paint it to the screen
        context.DrawGeometry(brush, null, geometry);
    }
}