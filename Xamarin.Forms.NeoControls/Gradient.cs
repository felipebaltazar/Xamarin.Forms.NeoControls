using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Xamarin.Forms.NeoControls
{
    [ContentProperty(nameof(Stops))]
    public abstract class Gradient : BindableObject
    {
        #region Fields

        private GradientElements<GradientStop> _stops;

        #endregion

        #region Bindable Properties

        public static readonly BindableProperty IsRepeatingProperty = BindableProperty.Create(
            propertyName: nameof(IsRepeating),
            returnType: typeof(bool),
            declaringType: typeof(Gradient),
            defaultValue: false); 

        #endregion

        #region Properties

        public GradientElements<GradientStop> Stops
        {
            get => _stops;
            set
            {
                _stops?.Release();
                _stops = value;
                _stops.AttachTo(this);
            }
        }

        public bool IsRepeating
        {
            get => (bool)GetValue(IsRepeatingProperty);
            set => SetValue(IsRepeatingProperty, value);
        }

        #endregion

        #region Constructors

        protected Gradient() =>
            Stops = new GradientElements<GradientStop>();

        #endregion

        #region Public Methods

        public IEnumerable<Gradient> GetGradients() => new[] { this };

        public abstract SKShader BuildShader(RenderContext context);

        public virtual void Measure(int width, int height)
        {
            var fromIndex = 0;

            for (var i = 0; i < Stops.Count; i++)
            {
                if (Stops[i].Offset >= 0 || i == Stops.Count - 1)
                {
                    SetupUndefinedOffsets(fromIndex, i);
                    fromIndex = i;
                }
            }
        }

        #endregion

        #region Override Methods

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            Stops.SetInheritedBindingContext(BindingContext);
        }

        #endregion

        #region Private Methods

        private void SetupUndefinedOffsets(int fromIndex, int toIndex)
        {
            var currentOffset = Math.Max(Stops[fromIndex].Offset, 0);
            var endOffset = Math.Abs(Stops[toIndex].Offset);

            var step = (endOffset - currentOffset) / (toIndex - fromIndex);

            for (var i = fromIndex; i <= toIndex; i++)
            {
                var stop = Stops[i];

                if (stop.Offset < 0)
                {
                    stop.RenderOffset = currentOffset;
                }
                currentOffset += step;
            }
        }

        #endregion
    }
}
