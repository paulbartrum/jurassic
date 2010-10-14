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
    /// Custom ScrollViewer that scrolls to the bottom whenever the content changes.
    /// </summary>
    public class CustomScrollViewer : ContentControl
    {
        public CustomScrollViewer()
        {
            this.DefaultStyleKey = typeof(CustomScrollViewer);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);

            //var content = this.Content as UIElement;
            //if (content == null)
            //    return base.MeasureOverride(availableSize);
            //content.Measure(availableSize);

            //var scrollBar = this.GetTemplateChild("VerticalScrollBar") as ScrollBar;
            //scrollBar.ViewportSize = availableSize.Height;
            //scrollBar.Maximum = content.DesiredSize.Height;
            ////SetValue(ExtentHeightProperty, content.DesiredSize.Height);
            //return new Size(
            //    Math.Min(availableSize.Width, content.DesiredSize.Width),
            //    Math.Min(availableSize.Height, content.DesiredSize.Height));
        }

        public double ExtentHeight
        {
            get { return (double)GetValue(ExtentHeightProperty); }
            set { SetValue(ExtentHeightProperty, value); }
        }

        public static readonly DependencyProperty ExtentHeightProperty =
            DependencyProperty.Register("ExtentHeight", typeof(double), typeof(CustomScrollViewer), new PropertyMetadata(0.0));

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(CustomScrollViewer), new PropertyMetadata(0.0));

        
    }
}
