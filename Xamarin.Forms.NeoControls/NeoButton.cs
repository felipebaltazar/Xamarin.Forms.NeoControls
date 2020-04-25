using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms.NeoControls.Extensions;

namespace Xamarin.Forms.NeoControls
{
    public class NeoButton : NeoView
    {
        private const int DEFAULT_CORNER_RADIUS = 3;

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
            propertyName: nameof(Command),
            returnType: typeof(ICommand),
            declaringType: typeof(NeoButton),
            defaultValue: null);

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
            propertyName: nameof(CommandParameter),
            returnType: typeof(object),
            declaringType: typeof(NeoButton),
            defaultValue: null);

        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
            propertyName: nameof(CornerRadius),
            returnType: typeof(CornerRadius),
            declaringType: typeof(NeoButton),
            defaultValue: new CornerRadius(DEFAULT_CORNER_RADIUS),
            propertyChanged: OnVisualPropertyChanged);

        public static readonly BindableProperty ClickModeProperty = BindableProperty.Create(
            propertyName: nameof(ClickMode),
            returnType: typeof(ClickMode),
            declaringType: typeof(NeoButton),
            defaultValue: ClickMode.SingleTap);

        public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(
            propertyName: nameof(IsChecked),
            returnType: typeof(bool),
            declaringType: typeof(NeoButton),
            defaultValue: false,
            propertyChanged: OnIsCheckedChanged);

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => (object)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public ClickMode ClickMode
        {
            get => (ClickMode)GetValue(ClickModeProperty);
            set => SetValue(ClickModeProperty, value);
        }

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public NeoButton()
        {
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnButtonTapped;
            GestureRecognizers.Add(tapGesture);
        }

        public virtual Task<bool> AnimateClick(double toValue, uint length = 250, Easing easing = null)
        {
            double transform(double t) => (t * (toValue));
            return PressAnimation(nameof(AnimateClick), transform, length, easing);
        }

        protected virtual async void OnButtonTapped(object sender, EventArgs e)
        {
            if (Command?.CanExecute(CommandParameter) ?? false)
                Command.Execute(CommandParameter);

            if(ClickMode == ClickMode.Toggle)
            {
                IsChecked = !IsChecked;
                return;
            }

            await AnimateClick(ShadowDistance * -1);
            await AnimateClick(ShadowDistance * -1);
        }

        protected override void DrawControl(SKPaint paint, SKPaintSurfaceEventArgs args)
        {
            var info = args.Info;
            var surface = args.Surface;
            var canvas = surface.Canvas;

            var drawPadding = Convert.ToSingle(ShadowBlur * 2);
            var diameter = drawPadding * 2;
            var retangleWidth = info.Width - diameter;
            var retangleHeight = info.Height - diameter;

            using (var path = CreatePath(retangleWidth, retangleHeight, drawPadding))
            {
                var darkShadow = Color.FromRgba(DarkShadowColor.R, DarkShadowColor.G, DarkShadowColor.B, Elevation);
                var fShadowDistance = Convert.ToSingle(ShadowDistance);

                paint.ImageFilter = darkShadow.ToSKDropShadow(fShadowDistance);
                canvas.DrawPath(path, paint);

                paint.ImageFilter = LightShadowColor.ToSKDropShadow(-fShadowDistance);
                canvas.DrawPath(path, paint);

                paint.ImageFilter = null;
                paint.MaskFilter = null;
                canvas.DrawPath(path, paint);
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

        protected virtual Task<bool> PressAnimation(string name, Func<double, double> transform, uint length, Easing easing)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            (this).Animate(
                name,
                transform,
                (distance) => ShadowDistance = distance,
                8,
                length,
                easing ?? Easing.Linear,
                (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }

        private static async void OnIsCheckedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if(bindable is NeoButton neoButton)
            { 
                await neoButton.AnimateClick(neoButton.ShadowDistance * -1);
                await neoButton.AnimateClick(neoButton.ShadowDistance);
            }
        }
    }
}
