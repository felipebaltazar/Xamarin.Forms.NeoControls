using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Runtime.CompilerServices;

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

        public Color BaseColor = Color.Gray;
        
        public NeoView() {
            InitializeComponent();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (BackgroundColor != Color.Transparent)
            {
                base.OnPropertyChanged(propertyName);
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
                paint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, Convert.ToSingle(ShadowBlur));

                DrawControl(paint, args);
            }
            
        }

        protected abstract void DrawControl(SKPaint paint, SKPaintSurfaceEventArgs args);

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