using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms.NeoControls.Extensions;

namespace Xamarin.Forms.NeoControls
{
    public class NeoFrame : NeoView
    {
        private const int DEFAULT_CORNER_RADIUS = 3;

        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
            propertyName: nameof(CornerRadius),
            returnType: typeof(CornerRadius),
            declaringType: typeof(NeoButton),
            defaultValue: new CornerRadius(DEFAULT_CORNER_RADIUS),
            propertyChanged: OnVisualPropertyChanged);

        public static readonly BindableProperty ShowOutProperty = BindableProperty.Create(
            propertyName: nameof(ShowOut),
            returnType: typeof(bool),
            declaringType: typeof(NeoFrame),
            defaultValue: true
            );

        public static readonly BindableProperty ShowInProperty = BindableProperty.Create(
            propertyName: nameof(ShowIn),
            returnType: typeof(bool),
            declaringType: typeof(NeoFrame),
            defaultValue: false
            );

        public static readonly BindableProperty IsSoftProperty = BindableProperty.Create(
            propertyName: nameof(IsSoft),
            returnType: typeof(bool),
            declaringType: typeof(NeoFrame),
            defaultValue: false
            );

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
           propertyName: nameof(BorderColor),
           returnType: typeof(Color),
           declaringType: typeof(NeoFrame),
           defaultValue: Color.Transparent
           );

        public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create(
            propertyName: nameof(BorderWidth),
            returnType: typeof(double),
            declaringType: typeof(NeoView),
            defaultValue: 1.0);

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public bool IsSoft
        {
            get => (bool)GetValue(IsSoftProperty);
            set => SetValue(IsSoftProperty, value);
        }

        public bool ShowOut
        {
            get => (bool)GetValue(ShowOutProperty);
            set => SetValue(ShowOutProperty, value);
        }

        public bool ShowIn
        {
            get => (bool)GetValue(ShowInProperty);
            set => SetValue(ShowInProperty, value);
        }

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

            var drawPadding = Convert.ToSingle(ShadowBlur * 2);

            if (!ShowOut)
                drawPadding = 0; // just innershadow 

            var diameter = drawPadding * 2;
            var retangleWidth = info.Width - diameter;
            var retangleHeight = info.Height - diameter;
            var fShadowDistance = Convert.ToSingle(ShadowDistance);


            using (var path = CreatePath(retangleWidth, retangleHeight, drawPadding))
            {
                var darkShadow = Color.FromRgba(DarkShadowColor.R, DarkShadowColor.G, DarkShadowColor.B, Elevation);



                if (ShowOut)
                {


                    // MODO NORMAL
                    paint.ImageFilter = darkShadow.ToSKDropShadow(fShadowDistance);
                    canvas.DrawPath(path, paint);

                    paint.ImageFilter = LightShadowColor.ToSKDropShadow(-fShadowDistance);
                    canvas.DrawPath(path, paint);


                    // background
                    paint.ImageFilter = null;
                    if (!IsSoft)
                        paint.MaskFilter = null; // REMOVER PARA FICAR SOFT
                    canvas.DrawPath(path, paint);

                    
                }

                

                if (ShowIn)
                {

                    canvas.ClipPath(path);

                    

                    paint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, Convert.ToSingle(ShadowBlur));
                    paint.Style = SKPaintStyle.Stroke;
                    paint.StrokeWidth = fShadowDistance;
                    paint.ImageFilter = LightShadowColor.ToSKDropShadow(-fShadowDistance);
                    canvas.DrawPath(path, paint);

                    paint.Style = SKPaintStyle.Stroke;
                    paint.StrokeWidth = fShadowDistance;
                    paint.ImageFilter = darkShadow.ToSKDropShadow(fShadowDistance);
                    canvas.DrawPath(path, paint);

                }

                if (BorderColor != Color.Transparent)
                {
                    ///border
                    paint.ImageFilter = null;
                    paint.MaskFilter = null;
                    paint.Style = SKPaintStyle.Stroke;
                    paint.Color = BorderColor.ToSKColor();
                    paint.StrokeWidth = Convert.ToSingle(BorderWidth);
                    canvas.DrawPath(path, paint);
                }

            }

        }

        protected virtual SKPath CreatePath(float retangleWidth, float retangleHeight, float drawPadding)
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
