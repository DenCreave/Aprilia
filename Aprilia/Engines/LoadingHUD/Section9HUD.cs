using System;
using Aprilia.Controllers;
using Aprilia.Engines.Glance;
using Aprilia.Engines.SpanningTree.UI.Entropy;
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
    
    private readonly ShockwaveRing[] _shockwaves = new ShockwaveRing[11];
    private readonly TrapezoidShape _fullTrapezoidBase;    // e.g. for 3 nodes
    private readonly TrapezoidShape _fullTrapezoidPenalty; // e.g. for 5 nodes
    
    private readonly double RADIUS = 50;
    private double GAP = 5.0;
    private double GAP_ANGLE;
    private double LENGTHMULTIPLIER = 0.4;

    public Section9HUD(CameraState camera,
        GlanceContentLoader contentLoader,
        GlanceController glanceController,
        AnimatrixController animatrixController,
        EntropyStyle colorStyle)
    {
        _camera = camera;
        _contentLoader = contentLoader;
        _glanceController = glanceController;
        _animatrixController = animatrixController;
        // looooool, theres a new "index from end" expression [^1]
        // never heard of it! thy jetbrains!
        _hexagonBrush = colorStyle.CorePalette[0];
        _trapezoidBrush = colorStyle.CorePalette[colorStyle.CorePalette.Length / 3 * 2];
        //_hexagonBrush = new SolidColorBrush(Colors.Cyan);
        //_trapezoidBrush = new SolidColorBrush(Colors.HotPink);
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
        
        
        // claculations for shockwave rings.
        for (int i = 0; i < _shockwaves.Length; i++)
        {
            _shockwaves[i] = new ShockwaveRing();
        }
        
        // 1. Calculate the thinned LEFT edge of the base hexagon
        // (This will become the right edge of our trapezoids after rotation!)
        double startLeftX = _baseHexagon.UniqueProps.HexMidLeft2TrapTopRight.Cox;
        double startLeftY = _baseHexagon.UniqueProps.HexMidLeft2TrapTopRight.Coy;
        double vecLeftX = _baseHexagon.UniqueProps.HexBotLeft2TrapBotRight.Cox - startLeftX;
        double vecLeftY = _baseHexagon.UniqueProps.HexBotLeft2TrapBotRight.Coy - startLeftY;
        
        Vertex thinnedBotLeft = new Vertex { Cox = startLeftX + vecLeftX * LENGTHMULTIPLIER, Coy = startLeftY + vecLeftY * LENGTHMULTIPLIER };

        // 2. Base Cost (3 Nodes)
        _fullTrapezoidBase = new TrapezoidShape();
        double sliceAngleBase = (Math.PI * 2) / _glanceController.BaseCost;
        double targetRotationBase = sliceAngleBase - GAP_ANGLE;
        
        // Anchor left side
        _fullTrapezoidBase.UniqueProps.HexMidRight2TrapTopLeft.Cox = _baseTrapezoid.UniqueProps.HexMidRight2TrapTopLeft.Cox;
        _fullTrapezoidBase.UniqueProps.HexMidRight2TrapTopLeft.Coy = _baseTrapezoid.UniqueProps.HexMidRight2TrapTopLeft.Coy;
        _fullTrapezoidBase.UniqueProps.HexBotRight2TrapBotLeft.Cox = _baseTrapezoid.UniqueProps.HexBotRight2TrapBotLeft.Cox;
        _fullTrapezoidBase.UniqueProps.HexBotRight2TrapBotLeft.Coy = _baseTrapezoid.UniqueProps.HexBotRight2TrapBotLeft.Coy;
        
        // Extrude right side (Rotate the left edge of the hexagon!)
        _animatrixController.Rotate(_baseHexagon.UniqueProps.HexMidLeft2TrapTopRight, _fullTrapezoidBase.UniqueProps.HexMidLeft2TrapTopRight, targetRotationBase);
        _animatrixController.Rotate(thinnedBotLeft, _fullTrapezoidBase.UniqueProps.HexBotLeft2TrapBotRight, targetRotationBase);
        _fullTrapezoidBase.UniqueProps.Extrusion = 1.0; 

        // 3. Penalty Cost (5 Nodes)
        _fullTrapezoidPenalty = new TrapezoidShape();
        double sliceAnglePenalty = (Math.PI * 2) / _glanceController.PenaltyCost;
        double targetRotationPenalty = sliceAnglePenalty - GAP_ANGLE;
        
        // Anchor left side
        _fullTrapezoidPenalty.UniqueProps.HexMidRight2TrapTopLeft.Cox = _baseTrapezoid.UniqueProps.HexMidRight2TrapTopLeft.Cox;
        _fullTrapezoidPenalty.UniqueProps.HexMidRight2TrapTopLeft.Coy = _baseTrapezoid.UniqueProps.HexMidRight2TrapTopLeft.Coy;
        _fullTrapezoidPenalty.UniqueProps.HexBotRight2TrapBotLeft.Cox = _baseTrapezoid.UniqueProps.HexBotRight2TrapBotLeft.Cox;
        _fullTrapezoidPenalty.UniqueProps.HexBotRight2TrapBotLeft.Coy = _baseTrapezoid.UniqueProps.HexBotRight2TrapBotLeft.Coy;
        
        // Extrude right side (Rotate the left edge of the hexagon!)
        _animatrixController.Rotate(_baseHexagon.UniqueProps.HexMidLeft2TrapTopRight, _fullTrapezoidPenalty.UniqueProps.HexMidLeft2TrapTopRight, targetRotationPenalty);
        _animatrixController.Rotate(thinnedBotLeft, _fullTrapezoidPenalty.UniqueProps.HexBotLeft2TrapBotRight, targetRotationPenalty);
        _fullTrapezoidPenalty.UniqueProps.Extrusion = 1.0;
        
        
        
    }


    public void UpdateHUD()
    {
        UpdateHexagons();
        UpdateTrapezoids();
        UpdateShockwaves();
    }
    
    private void UpdateShockwaves()
    {
        // 1. THE TRIGGER: Did the physics engine just flip a page?
        if (_glanceController.State.JustFlipped)
        {
            TriggerAvailableShockwave();
        }

        // 2. THE MATH: Advance the animation for any active shockwaves
        //Console.WriteLine("we are in UPDATE Shockerwaven");
        foreach (var wave in _shockwaves)
        {
            if (wave.IsAnimating)
            {
                //Console.WriteLine("we found one thats animatin in  Shockerwaven");
                // 0.033 per frame = exactly 0.5 seconds at 60fps
                
                wave.AnimationProgress += 1/(60.0/2);
                //Console.WriteLine($"animation value: {wave.AnimationProgress}");
                if (wave.AnimationProgress >= 1.0)
                {
                    wave.IsAnimating = false; // Kill it, return to the pool
                }
            }
        }
    }

    private void TriggerAvailableShockwave()
    {
        foreach (var wave in _shockwaves)
        {
            if (!wave.IsAnimating)
            {
                wave.IsAnimating = true;
                wave.AnimationProgress = 0.0;
                
                // Capture the current node count (3 or 5) so the shockwave matches the UI!
                wave.NodeCount = (int)_glanceController.TicksRequired; 
                break;
            }
        }
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
        double step = 1.0 / (activeNodes * 2.0);

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
        double centerX = screenWidth * 0.2; 
        double centerY = screenHeight * 0.8;
        
        // Shift the box up and left so (0,0) is perfectly in the middle of it!
        var effectBounds = new Rect(-centerX, -centerY, screenWidth, screenHeight);

        using (context.PushTransform(Matrix.CreateTranslation(centerX, centerY)))
        {
            DrawShockwaves(context, effectBounds);
            
            DrawRolodex(context);

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
    
    private void DrawRolodex(DrawingContext context)
    {
        int totalPages = _contentLoader.Pages.Count;
        if (totalPages == 0) return;

        double lineHeight = 30.0; // How far apart the words are
        double strain = _camera.StrainProgress; // -1.0 to 1.0

        // We draw 3 labels: Previous (-1), Current (0), Next (1)
        for (int offset = -1; offset <= 1; offset++)
        {
            // 1. CALCULATE Y POSITION
            // If strain is 0.5 (scrolling down), the text slides UP by half a line.
            double finalY = (offset * lineHeight) - (strain * lineHeight);

            // 2. CALCULATE OPACITY
            // 1.0 at the exact center (finalY = 0). Fades to 0.0 as it hits the lineHeight distance.
            double opacity = Math.Clamp(1.0 - Math.Abs(finalY / lineHeight), 0.0, 1.0);

            if (opacity <= 0.01) continue;

            // 3. GET THE TEXT (With safe modulo wrapping for infinite scrolling!)
            int pageIndex = (_camera.CurrentIndex + offset + totalPages) % totalPages;
            string pageName = _contentLoader.Pages[pageIndex].PageName.ToString().ToUpper();

            // 4. FORMAT THE TEXT (Avalonia 11 syntax)
            var typeface = new Typeface("sans-serif", FontStyle.Normal, FontWeight.Bold);
            var formattedText = new FormattedText(
                pageName,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                16, // Font size
                _hexagonBrush // Cyan text! lol no.
            );

            // 5. DRAW IT (Offset by half its width/height so it is perfectly centered)
            double textX = -formattedText.Width / 2.0;
            double textY = finalY - (formattedText.Height / 2.0);

            using (context.PushOpacity(opacity))
            {
                context.DrawText(formattedText, new Point(textX, textY));
            }
        }
    }

    
    private void DrawShockwaves(DrawingContext context, Rect effectBounds)
    {
        foreach (var wave in _shockwaves)
        {
            if (!wave.IsAnimating) continue;
            // Calculate blur: Starts at 0, blurs out to 20 pixels as it expands
            double blurAmount = wave.AnimationProgress * 20.0;

            double waveScale = 1.0 + (wave.AnimationProgress * 1.5); 
            double waveOpacity = (1.0 - wave.AnimationProgress); 
            double angleStep = 360.0 / wave.NodeCount;

            using (context.PushTransform(Matrix.CreateScale(waveScale, waveScale)))
            using (context.PushOpacity(waveOpacity))
            using (context.PushEffect(new BlurEffect { Radius = blurAmount }, effectBounds))
            {
                for (int i = 0; i < wave.NodeCount; i++)
                {
                    using (context.PushTransform(Matrix.CreateRotation(Math.PI / 180.0 * (i * angleStep))))
                    {
                        // Draw the Hexagon (DrawShape ignores opacity checks)
                        DrawShape(context, _baseHexagon, _hexagonBrush);
                        
                        // Draw the Trapezoid (DrawTrap uses the beautiful ArcTo curves!)
                        var fullTrap = (wave.NodeCount == _glanceController.BaseCost) ? _fullTrapezoidBase : _fullTrapezoidPenalty;
                        DrawTrap(context, fullTrap);
                    }
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