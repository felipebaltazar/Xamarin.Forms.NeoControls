using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;

namespace Xamarin.Forms.NeoControls
{
    public class NeoFrame : NeoRoundedView
    {
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
           propertyName: nameof(BorderColor),
           returnType: typeof(Color),
           declaringType: typeof(NeoFrame),
           defaultValue: Color.Transparent);

        public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create(
            propertyName: nameof(BorderWidth),
            returnType: typeof(double),
            declaringType: typeof(NeoView),
            defaultValue: 1.0);

        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public double BorderWidth
        {
            get => (double)GetValue(BorderWidthProperty);
            set => SetValue(BorderWidthProperty, value);
        }

        protected override void DrawControl(SKPaint paint, SKPaintSurfaceEventArgs args)
        {
            var info = args.Info;
            var surface = args.Surface;
            var canvas = surface.Canvas;

            var drawPadding = ShadowDrawMode == ShadowDrawMode.InnerOnly ?
                0 : Convert.ToSingle(ShadowBlur * 2);

            var diameter = drawPadding * 2;
            var retangleWidth = info.Width - diameter;
            var retangleHeight = info.Height - diameter;

            using (var path = CreatePath(retangleWidth, retangleHeight, drawPadding))
            {
                paint.ImageFilter = null;
                if (DrawMode == DrawMode.Flat)
                    paint.MaskFilter = null;

                canvas.DrawPath(path, paint);

                if (BorderColor != Color.Transparent)
                    DrawBorder(paint, canvas, path);
            }
        }

        protected virtual void DrawBorder(SKPaint paint, SKCanvas canvas, SKPath path)
        {
            paint.Style = SKPaintStyle.Stroke;
            paint.Color = BorderColor.ToSKColor();
            paint.StrokeWidth = Convert.ToSingle(BorderWidth);
            canvas.DrawPath(path, paint);
        }
    }
}
