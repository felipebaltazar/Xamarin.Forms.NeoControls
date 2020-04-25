using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace Xamarin.Forms.NeoControls.Extensions
{
    internal static class ColorExtensions
    {
        private const float DEFAULT_SIGMA = -6f;
        internal static SKImageFilter ToSKDropShadow(this Color shadowColor, float distance)
        {
            return SKImageFilter.CreateDropShadow(
                    distance,
                    distance,
                    DEFAULT_SIGMA,
                    DEFAULT_SIGMA,
                    shadowColor.ToSKColor(),
                    SKDropShadowImageFilterShadowMode.DrawShadowOnly);
        }
    }
}
