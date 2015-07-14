using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;

namespace SilverlightREPL
{
    /// <summary>
    /// Implements the actual scrolling - used by CustomScrollViewer.
    /// </summary>
    public class CustomScrollContentPresenter : ContentPresenter
    {
        private RectangleGeometry clipGeometry;

        protected override Size MeasureOverride(Size availableSize)
        {
            // Get the content control.
            var content = this.Content as UIElement;
            if (content == null)
                return new Size();

            // Indicate that unlimited space is available in the vertical direction.
            content.Measure(new Size(availableSize.Width, double.PositiveInfinity));

            // Populate ExtentHeight, ViewportHeight and ExtentMinusViewportHeight.
            SetValue(ViewportHeightProperty, availableSize.Height);
            SetValue(ExtentHeightProperty, content.DesiredSize.Height);
            SetValue(ExtentMinusViewportHeightProperty, Math.Max(0, content.DesiredSize.Height - availableSize.Height));

            // Limit the height of this control to the available height.
            return new Size(
                content.DesiredSize.Width,
                Math.Min(content.DesiredSize.Height, availableSize.Height));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            // Get the content control.
            var content = this.Content as UIElement;
            if (content == null)
                return finalSize;

            // Clip the child content.
            if (this.Clip == null)
            {
                this.clipGeometry = new RectangleGeometry();
                this.Clip = this.clipGeometry;
            }
            this.clipGeometry.Rect = new Rect(0.0, 0.0, finalSize.Width, finalSize.Height);

            // Offset the content by VerticalOffset pixels.
            content.Arrange(new Rect(0, -this.VerticalOffset, finalSize.Width, content.DesiredSize.Height));

            return finalSize;
        }

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(CustomScrollContentPresenter), new PropertyMetadata(0.0, VerticalOffsetChangedCallback));

        private static void VerticalOffsetChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CustomScrollContentPresenter)d).InvalidateArrange();
        }

        public double ExtentHeight
        {
            get { return (double)GetValue(ExtentHeightProperty); }
            set { SetValue(ExtentHeightProperty, value); }
        }

        public static readonly DependencyProperty ExtentHeightProperty =
            DependencyProperty.Register("ExtentHeight", typeof(double), typeof(CustomScrollContentPresenter), new PropertyMetadata(0.0));

        public double ViewportHeight
        {
            get { return (double)GetValue(ViewportHeightProperty); }
            set { SetValue(ViewportHeightProperty, value); }
        }

        public static readonly DependencyProperty ViewportHeightProperty =
            DependencyProperty.Register("ViewportHeight", typeof(double), typeof(CustomScrollContentPresenter), new PropertyMetadata(0.0));

        public double ExtentMinusViewportHeight
        {
            get { return (double)GetValue(ExtentMinusViewportHeightProperty); }
            set { SetValue(ExtentMinusViewportHeightProperty, value); }
        }

        public static readonly DependencyProperty ExtentMinusViewportHeightProperty =
            DependencyProperty.Register("ExtentMinusViewportHeight", typeof(double), typeof(CustomScrollContentPresenter), new PropertyMetadata(0.0));
    }
}
