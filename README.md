## The Aprilia Framework
### The Dual-Engine Architecture
#### The system is divided into two decoupled engines: the `SpanningTree` (Background 3D Void) and the `GlanceEngine` (UI, Physics, and Content). They never speak directly to each other. They are synchronized entirely by the `TwinRenderMediator`.

---

## Module 0: SpanningTree (engine for animated background. feel free to add a new background any time)
### SpanningTreeBackground
#### This was the name of this feature. It's purpose is to give an engine for making a highly customizable background animation
### SpanningTreeRender
#### This is the handler for the dedicated controllers. This is the component you'd wanna put into the UI. It's purpose is to call render and it's math engine Stc and ui representation Stt handles the task.
### SpanningTreeController
#### This handles the math. The math is derived from the relation between a DotMap and a TorrentLayer which is defined by the ThemeBinder. Common math functions are collected within AnimatrixController
### SpanningTreeTheme
#### This handles the visual representation of the data prepared by Stc each cycle. The colors are to be cached in StyleArchive which will be used by an ISTTheme. The ui render and representation ie. coloring and drawing onto the canvas is defined within the ISTTheme itself. Each cycle ISTTheme checks for the ThemeGroup and renders based on that defined visual representation. Therefore making the ui styles modular yet specifically defined.



### MODULE 1: The Glance Engine (Physics & State)

### GlanceState
#### The single source of truth for the user's navigation. It is an `ObservableObject` that holds the `CurrentFocus` (which page we are on) and the `StrainProgress` (-1.0 to 1.0). It does no math; it only holds data.

### GlanceController
#### The custom physics engine for the mouse wheel and trackpad. It intercepts raw scroll deltas and applies Linear Interpolation (Lerp) to create heavy, cinematic momentum. It features a "Dynamic Friction" system (requiring 5 ticks to flip a page normally, but penalizing fast scrolling by requiring 8 ticks). It also handles the 1.5-second idle decay, smoothly returning the camera to rest if the user stops scrolling.

---

### MODULE 2: The Content Management System (CMS)

### IGlanceContent
#### The strict interface contract for every page in the CV. It guarantees that every page has a `PageName` (Enum), a `PageOpacity`, a `ZIndex`, and the `IsAtTop`/`IsAtBottom` safety valve booleans. Concrete implementations (like `IntroContent` or `ProjectsContent`) act as the ViewModels for their respective pages.

### GlanceContentLoader
#### A headless, Reflection-based CMS. On startup, it scans the assembly for any class implementing `IGlanceContent`, instantiates them via Dependency Injection, and sorts them automatically based on their Enum integer value. This allows new pages to be added to the CV simply by creating a new class, with zero changes to the routing logic.

### Datapad (Control)
#### A reusable XAML wrapper that houses the CV text. It provides the glowing cyberpunk border and the `ScrollViewer`. Crucially, it acts as the "Safety Valve": it updates the `IsAtTop` and `IsAtBottom` booleans so the `GlanceController` knows to ignore the mouse wheel while the user is reading long paragraphs of text.

---

### MODULE 3: The Bridge (Synchronization)

### CameraState
#### A lightweight, globally injected Singleton that holds the absolute Z-axis depth of the 3D background. It holds `CurrentIndex` and `StrainProgress`. 

### TwinRenderMediator
#### The translator between the UI and the Math Engine. It observes the `GlanceState`. When the user scrolls, the Mediator reads the UI strain and translates it into 3D depth, writing it to the `CameraState`. This guarantees that the `SpanningTree` background zooms in perfect synchronization with the UI, while keeping the two engines completely decoupled.

---

### MODULE 4: The HUD (Heads-Up Display)

### ILoadingHUD
#### An interface for bespoke, hardware-accelerated UI components that draw themselves directly to the Avalonia `DrawingContext`, bypassing XAML entirely for maximum performance.

### Section9HUD
#### The concrete implementation of the HUD. It reads the `CameraState` and the `GlanceContentLoader` to draw three distinct elements at 60fps:
1. **The Geometric Ring:** Calculates the exact angles to draw a dynamic ring of Hexagons and extruding Trapezoids based on the current friction cost (5 or 8 nodes).
2. **The Rolodex:** Reads the page titles from the CMS and draws them in the center of the screen, sliding them vertically and fading their opacity (100% -> 33% -> 0%) based on the scroll strain.
3. **The Shockwave Pool:** An object-pooled animation system. When a page flips, it triggers a `ShockwaveRing` that rapidly expands and fades out, providing instant, tactile visual feedback without blocking the user's input.

***

### How the Flow Actually Works (The TL;DR for your memory):
1. User scrolls the mouse.
2. `Datapad` checks if the text is at the bottom. If yes, it lets the scroll pass.
3. `GlanceController` catches the scroll, does the Lerp math, and updates `GlanceState`.
4. `TwinRenderMediator` sees `GlanceState` change, and updates `CameraState`.
5. `EntropyTheme` reads `CameraState` and zooms the 3D dots.
6. `Section9HUD` reads `CameraState` and spins the Hexagons and the Rolodex text. 
