using Aprilia.Engines.Glance;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Aprilia.Controls;

public partial class Datapad : UserControl
{
    // This allows us to inject any XAML we want inside the Datapad!
    public static readonly StyledProperty<object> InnerContentProperty =
        AvaloniaProperty.Register<Datapad, object>(nameof(InnerContent));
    
    // 1. Create a property directly on the Datapad to hold the scale
    public static readonly StyledProperty<double> UiScaleProperty =
        AvaloniaProperty.Register<Datapad, double>(nameof(UiScale), 1.0);

    public double UiScale
    {
        get => GetValue(UiScaleProperty);
        set => SetValue(UiScaleProperty, value);
    }

    public object InnerContent
    {
        get => GetValue(InnerContentProperty);
        set => SetValue(InnerContentProperty, value);
    }
    
    

    public Datapad()
    {
        InitializeComponent();
        
    }
    private void OnScrollChanged(object? sender, ScrollChangedEventArgs e) => UpdateScrollState();
    private void OnSizeChanged(object? sender, SizeChangedEventArgs e) 
    {
        UpdateScrollState();

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel != null)
        {
            UiScale = ResponsiveScaler.GetScale(topLevel.Bounds.Width);
        }
    }


    private void UpdateScrollState()
    {
        if (PageScrollViewer != null && DataContext is IGlanceContent content)
        {
            // If the text is shorter than the screen, it doesn't need to scroll!
            if (PageScrollViewer.Extent.Height <= PageScrollViewer.Viewport.Height)
            {
                content.IsAtTop = true;
                content.IsAtBottom = true;
            }
            else
            {
                // -1.0 buffer for floating point rounding errors
                content.IsAtTop = PageScrollViewer.Offset.Y <= 0;
                content.IsAtBottom = PageScrollViewer.Offset.Y >= (PageScrollViewer.Extent.Height - PageScrollViewer.Viewport.Height - 1.0);
            }
        }
    }

}