using Avalonia.Media;

namespace Aprilia.Engines.LoadingHUD;

/// <summary>
/// This is a UI component. it draws itself.
/// </summary>
public interface ILoadingHUD
{
    void UpdateHUD();
    void Draw(DrawingContext context, double screenWidth, double screenHeight);
}