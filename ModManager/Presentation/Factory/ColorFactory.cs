namespace ModManager.Presentation.Factory;

public static class ColorFactory
{
    public static GradientStopCollection CreateStripedCollection(Color evenColor, Color oddColor, int stripes = 10)
    {
        var stops = new GradientStopCollection();
        double stripeWidth = 1.0 / stripes;

        for (var i = 0; i < stripes; i++)
        {
            double start = i * stripeWidth;
            double mid = start + stripeWidth / 2;
            double end = start + stripeWidth;

            Color color = i % 2 == 0 ? evenColor : oddColor;
            stops.Add(new GradientStop {Color = color, Offset = start,});
            stops.Add(new GradientStop {Color = color, Offset = mid,});

            Color nextColor = i % 2 == 0 ? oddColor : evenColor;
            stops.Add(new GradientStop {Color = nextColor, Offset = mid,});
            stops.Add(new GradientStop {Color = nextColor, Offset = end,});
        }

        return stops;
    }
}
