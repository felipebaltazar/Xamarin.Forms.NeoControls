using SkiaSharp;
using System;
using Xamarin.Forms.NeoControls.Extensions;

namespace Xamarin.Forms.NeoControls
{
    public abstract class NeoRoundedView : NeoView
    {
        private const int DEFAULT_CORNER_RADIUS = 3;

        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
            propertyName: nameof(CornerRadius),
            returnType: typeof(CornerRadius),
            declaringType: typeof(NeoRoundedView),
            defaultValue: new CornerRadius(DEFAULT_CORNER_RADIUS),
            propertyChanged: OnVisualPropertyChanged);

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        protected override SKPath CreatePath(float retangleWidth, float retangleHeight, float drawPadding)
        {
            var path = new SKPath();
            var fTopLeftRadius = Convert.ToSingle(CornerRadius.TopLeft);
            var fTopRightRadius = Convert.ToSingle(CornerRadius.TopRight);
            var fBottomLeftRadius = Convert.ToSingle(CornerRadius.BottomLeft);
            var fBottomRightRadius = Convert.ToSingle(CornerRadius.BottomRight);

            var startX = fTopLeftRadius + drawPadding;
            var startY = drawPadding;

            path.MoveTo(startX, startY);

            path.LineTo(retangleWidth - fTopRightRadius + drawPadding, startY);
            path.ArcTo(fTopRightRadius,
                new SKPoint(retangleWidth + drawPadding, fTopRightRadius + drawPadding));

            path.LineTo(retangleWidth + drawPadding, retangleHeight - fBottomRightRadius + drawPadding);
            path.ArcTo(fBottomRightRadius,
                 new SKPoint(retangleWidth - fBottomRightRadius + drawPadding, retangleHeight + drawPadding));

            path.LineTo(fBottomLeftRadius + drawPadding, retangleHeight + drawPadding);
            path.ArcTo(fBottomLeftRadius,
                new SKPoint(drawPadding, retangleHeight - fBottomLeftRadius + drawPadding));

            path.LineTo(drawPadding, fTopLeftRadius + drawPadding);
            path.ArcTo(fTopLeftRadius, new SKPoint(startX, startY));

            path.Close();

            return path;
        }
    }
}
