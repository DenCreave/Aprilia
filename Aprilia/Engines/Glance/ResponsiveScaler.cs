namespace Aprilia.Engines.Glance;

public static class ResponsiveScaler
{
    public static double GetScale(double currentWidth)
    {
        double maxWidth = 1000.0;
        double minWidth = 500.0;
        double maxScale = 1.0;
        double minScale = 0.65;

        if (currentWidth > 2000.0) 
        {
            return currentWidth / 2000.0;
        }
        if (currentWidth >= maxWidth) 
        {
            return maxScale;
        }
        if (currentWidth <= minWidth) 
        {
            return minScale;
        }
        
        // Linear interpolation for everything in between
        double percentage = (currentWidth - minWidth) / (maxWidth - minWidth);
        return minScale + (percentage * (maxScale - minScale));
    }

}