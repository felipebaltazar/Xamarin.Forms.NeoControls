using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Threading.Tasks;
using Xamarin.Forms.NeoControls.Extensions;

namespace Xamarin.Forms.NeoControls
{
    public class NeoProgressView : NeoView
    {
        public static readonly BindableProperty BarColorProperty = BindableProperty.Create(
            propertyName: nameof(BarColor),
            returnType: typeof(Color),
            declaringType: typeof(NeoProgressView),
            defaultValue: Color.Red,
            propertyChanged: OnVisualPropertyChanged);

        public static readonly BindableProperty ProgressProperty = BindableProperty.Create(
            propertyName: nameof(Progress),
            returnType: typeof(double),
            declaringType: typeof(NeoProgressView),
            defaultValue: .4,
            propertyChanging: OnProgressChanging,
            propertyChanged: OnVisualPropertyChanged);

        public static readonly BindableProperty ThicknessProperty = BindableProperty.Create(
            propertyName: nameof(Thickness),
            returnType: typeof(double),
            declaringType: typeof(NeoProgressView),
            defaultValue: 5.0,
            propertyChanged: OnVisualPropertyChanged);

        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        public double Thickness
        {
            get => (double)GetValue(ThicknessProperty);
            set => SetValue(ThicknessProperty, value);
        }

        public Color BarColor
        {
            get => (Color)GetValue(BarColorProperty);
            set => SetValue(BarColorProperty, value);
        }

        public virtual Task<bool> AnimateProgress(float toValue, uint length = 250, Easing easing = null)
        {
            EnsureProgressRange(toValue);

            float transform(double t) => (float)(t * (toValue));
            return ProgressAnimation(nameof(AnimateProgress), transform, length, easing);
        }

        protected override void DrawControl(SKPaint paint, SKPaintSurfaceEventArgs args)
        {
            var info = args.Info;
            var surface = args.Surface;
            var canvas = surface.Canvas;

            var fShadowBlur = Convert.ToSingle(ShadowBlur);
            var padding = fShadowBlur * 3f;
            var diameter = padding * 2;
            var retangleWidth = info.Width - diameter;
            var retangleHeight = info.Height - diameter;
            var cornerRadius = retangleHeight / 2;

            using (var path = CreateStrokePath(padding, retangleWidth, retangleHeight, cornerRadius))
            {
                using (var barPath = CreateBarPath(paint, canvas, padding, retangleWidth, retangleHeight, cornerRadius ))
                {
                    paint.Color = BarColor.ToSKColor();
                    canvas.DrawPath(barPath, paint);

                    paint.Color = BaseColor.ToSKColor();
                    paint.Style = SKPaintStyle.Stroke;
                    paint.StrokeWidth = Convert.ToSingle(Thickness);
                    paint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, fShadowBlur);

                    var shadow = Color.FromRgba(DarkShadowColor.R, DarkShadowColor.G, DarkShadowColor.B, Elevation);
                    var fShadowDistance = Convert.ToSingle(ShadowDistance);

                    paint.ImageFilter = shadow.ToSKDropShadow(fShadowDistance);
                    canvas.DrawPath(path, paint);

                    paint.ImageFilter = LightShadowColor.ToSKDropShadow(-fShadowDistance);
                    canvas.DrawPath(path, paint);

                    paint.ImageFilter = null;
                    paint.MaskFilter = null;
                    canvas.DrawPath(path, paint);
                }
            }
        }

        protected virtual SKPath CreateStrokePath(float padding, float retangleWidth, float retangleHeight, float cornerRadius)
        {
            var path = new SKPath();
            path.MoveTo(cornerRadius + padding, padding);

            path.LineTo(retangleWidth - cornerRadius + padding, padding);
            path.ArcTo(cornerRadius,
                new SKPoint(retangleWidth - cornerRadius + padding, retangleHeight + padding));

            path.LineTo(cornerRadius + padding, retangleHeight + padding);
            path.ArcTo(cornerRadius, new SKPoint(cornerRadius + padding, padding));

            path.Close();
            return path;
        }

        protected virtual SKPath CreateBarPath(SKPaint paint, SKCanvas canvas, float padding, float retangleWidth, float retangleHeight, float cornerRadius)
        {
            var fProgress = Convert.ToSingle(Progress);
            var invertedProgress = fProgress <= 0.5f ? 0 : 2 - (1 - ((1 - fProgress) / 0.5f));
            var minProgress = Math.Max(0.01f, fProgress);
            var barPath = new SKPath();

            barPath.MoveTo(cornerRadius + padding, padding);
            barPath.LineTo(((retangleWidth - cornerRadius) * minProgress)  + padding, padding);

            barPath.ArcTo(cornerRadius * invertedProgress,
                new SKPoint(((retangleWidth - cornerRadius) * minProgress) + padding, retangleHeight + padding));

            barPath.LineTo(cornerRadius + padding, retangleHeight + padding);
            barPath.ArcTo(cornerRadius, new SKPoint(cornerRadius + padding, padding));

            barPath.Close();

            return barPath;
        }

        protected virtual Task<bool> ProgressAnimation(string name, Func<double, float> transform, uint length, Easing easing)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            (this).Animate(
                name,
                transform,
                (progress) => Progress = progress,
                8,
                length,
                easing ?? Easing.Linear,
                (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }

        private static void EnsureProgressRange(float progress)
        {
            if (progress > 1f || progress < 0)
                throw new ArgumentOutOfRangeException($"{nameof(progress)} should be between 0 and 1");
        }

        private static void OnProgressChanging(BindableObject bindable, object oldValue, object newValue)
        {
            //EnsureProgressRange((float)newValue);
        }
        
            
    }
}
