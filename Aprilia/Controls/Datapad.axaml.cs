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

    public object InnerContent
    {
        get => GetValue(InnerContentProperty);
        set => SetValue(InnerContentProperty, value);
    }

    public Datapad()
    {
        InitializeComponent();
        
    }

    // The Safety valve (for scroll) Logic lives here permanently!
    private void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (sender is ScrollViewer sv && DataContext is IGlanceContent content)
        {
            content.IsAtTop = sv.Offset.Y <= 0;
            content.IsAtBottom = sv.Offset.Y >= (sv.Extent.Height - sv.Viewport.Height - 1.0);
        }
    }

}