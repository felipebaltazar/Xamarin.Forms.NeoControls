using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms.NeoControls.Extensions;

namespace Xamarin.Forms.NeoControls
{
    [ContentProperty(nameof(InnerView))]
    public abstract partial class NeoView : ContentView
    {
        public static readonly BindableProperty ShadowBlurProperty = BindableProperty.Create(
            propertyName: nameof(ShadowBlur),
            returnType: typeof(double),
            declaringType: typeof(NeoView),
            defaultValue: 10.0,
            propertyChanged: OnVisualPropertyChanged);

        public static readonly BindableProperty DrawModeProperty = BindableProperty.Create(
            propertyName: nameof(DrawMode),
            returnType: typeof(DrawMode),
            declaringType: typeof(NeoView),
            defaultValue: DrawMode.Flat,
            propertyChanged: OnVisualPropertyChanged);

        public static readonly BindableProperty ShadowDrawModeProperty = BindableProperty.Create(
            propertyName: nameof(NeoControls.ShadowDrawMode),
            returnType: typeof(ShadowDrawMode),
            declaringType: typeof(NeoView),
            defaultValue: ShadowDrawMode.OuterOnly,
            propertyChanged: OnVisualPropertyChanged);

        public static readonly BindableProperty ElevationProperty = BindableProperty.Create(
            propertyName: nameof(Elevation),
            returnType: typeof(double),
            declaringType: typeof(NeoView),
            defaultValue: .6,
            propertyChanged: OnVisualPropertyChanged);

        public static readonly BindableProperty ShadowDistanceProperty = BindableProperty.Create(
            propertyName: nameof(ShadowDistance),
            returnType: typeof(double),
            declaringType: typeof(NeoView),
            defaultValue: 9.0,
            propertyChanged: OnVisualPropertyChanged);

        public static readonly BindableProperty LightShadowColorProperty = BindableProperty.Create(
            propertyName: nameof(LightShadowColor),
            returnType: typeof(Color),
            declaringType: typeof(NeoView),
            defaultValue: Color.White,
            propertyChanged: OnVisualPropertyChanged);

        public static readonly BindableProperty DarkShadowColorProperty = BindableProperty.Create(
            propertyName: nameof(DarkShadowColor),
            returnType: typeof(Color),
            declaringType: typeof(NeoView),
            defaultValue: Color.Black,
            propertyChanged: OnVisualPropertyChanged);

        public static readonly BindableProperty InnerViewProperty = BindableProperty.Create(
            propertyName: nameof(InnerView),
            returnType: typeof(View),
            declaringType: typeof(NeoView),
            defaultValue: null,
            propertyChanged: OnInnerViewChanged);

        public double Elevation
        {
            get => (double)GetValue(ElevationProperty);
            set => SetValue(ElevationProperty, value);
        }

        public double ShadowDistance
        {
            get => (double)GetValue(ShadowDistanceProperty);
            set => SetValue(ShadowDistanceProperty, value);
        }

        public double ShadowBlur
        {
            get => (double)GetValue(ShadowBlurProperty);
            set => SetValue(ShadowBlurProperty, value);
        }

        public Color LightShadowColor
        {
            get => (Color)GetValue(LightShadowColorProperty);
            set => SetValue(LightShadowColorProperty, value);
        }

        public Color DarkShadowColor
        {
            get => (Color)GetValue(DarkShadowColorProperty);
            set => SetValue(DarkShadowColorProperty, value);
        }

        public View InnerView
        {
            get => (View)GetValue(InnerViewProperty);
            set => SetValue(InnerViewProperty, value);
        }

        public DrawMode DrawMode
        {
            get => (DrawMode)GetValue(DrawModeProperty);
            set => SetValue(DrawModeProperty, value);
        }

        public ShadowDrawMode ShadowDrawMode
        {
            get => (ShadowDrawMode)GetValue(ShadowDrawModeProperty);
            set => SetValue(ShadowDrawModeProperty, value);
        }

        public Color BaseColor = Color.Gray;

        public NeoView() => InitializeComponent();

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (BackgroundColor != Color.Transparent)

            {
                 if (BackgroundColorProperty.PropertyName.Equals(propertyName))
                    canvas.InvalidateSurface();
                BaseColor = BackgroundColor;
                BackgroundColor = Color.Transparent;
            }
        }

        protected virtual void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            var surface = args.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.Color = BaseColor.ToSKColor();
                paint.Style = SKPaintStyle.Fill;

                var drawOuterShadow = ShadowDrawMode == ShadowDrawMode.All || ShadowDrawMode == ShadowDrawMode.OuterOnly;
                var drawInnerShadow = ShadowDrawMode == ShadowDrawMode.All || ShadowDrawMode == ShadowDrawMode.InnerOnly;

                PreDraw(paint, args);

                if (drawOuterShadow)
                    DrawOuterShadow(paint, args);

                paint.Color = BaseColor.ToSKColor();
                DrawControl(paint, args);

                if (drawInnerShadow)
                    DrawInnerShadow(paint, args);
            }
        }

        protected virtual void PreDraw(SKPaint paint, SKPaintSurfaceEventArgs args)
        {
        }

        protected virtual void DrawInnerShadow(SKPaint paint, SKPaintSurfaceEventArgs args)
        {
            var info = args.Info;
            var surface = args.Surface;
            var canvas = surface.Canvas;
            var fShadowDistance = Convert.ToSingle(ShadowDistance);
            var darkShadow = Color.FromRgba(DarkShadowColor.R, DarkShadowColor.G, DarkShadowColor.B, Elevation);
            var drawPadding = ShadowDrawMode == ShadowDrawMode.InnerOnly ?
                0 : Convert.ToSingle(ShadowBlur * 2);

            var diameter = drawPadding * 2;
            var retangleWidth = info.Width - diameter;
            var retangleHeight = info.Height - diameter;

            using (var path = CreatePath(retangleWidth, retangleHeight, drawPadding))
            {
                paint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, Convert.ToSingle(ShadowBlur));

                canvas.ClipPath(path);
                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeWidth = fShadowDistance;

                paint.ImageFilter = LightShadowColor.ToSKDropShadow(-fShadowDistance);
                canvas.DrawPath(path, paint);

                paint.ImageFilter = darkShadow.ToSKDropShadow(fShadowDistance);
                canvas.DrawPath(path, paint);
            }
        }

        protected virtual void DrawOuterShadow(SKPaint paint, SKPaintSurfaceEventArgs args)
        {
            var info = args.Info;
            var surface = args.Surface;
            var canvas = surface.Canvas;
            var fShadowDistance = Convert.ToSingle(ShadowDistance);
            var darkShadow = Color.FromRgba(DarkShadowColor.R, DarkShadowColor.G, DarkShadowColor.B, Elevation);
            var drawPadding = Convert.ToSingle(ShadowBlur * 2);

            paint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, Convert.ToSingle(ShadowBlur));

            var diameter = drawPadding * 2;
            var retangleWidth = info.Width - diameter;
            var retangleHeight = info.Height - diameter;

            using (var path = CreatePath(retangleWidth, retangleHeight, drawPadding))
            {
                paint.ImageFilter = darkShadow.ToSKDropShadow(fShadowDistance);
                canvas.DrawPath(path, paint);

                paint.ImageFilter = LightShadowColor.ToSKDropShadow(-fShadowDistance);
                canvas.DrawPath(path, paint);
            }
        }

        protected abstract void DrawControl(SKPaint paint, SKPaintSurfaceEventArgs args);

        protected abstract SKPath CreatePath(float retangleWidth, float retangleHeight, float drawPadding);

        protected static void OnVisualPropertyChanged(BindableObject bindable, object oldValue, object newValue) =>
            ((NeoView)bindable).canvas.InvalidateSurface();

        private static void OnInnerViewChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is NeoView neoView)
            {
                if (newValue is View child)
                    neoView.rootView.Children.Add(child, 0, 0);
            }
        }
    }
}