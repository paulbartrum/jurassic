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
        private ScrollBar verticalScrollBar;

        public CustomScrollViewer()
        {
            this.DefaultStyleKey = typeof(CustomScrollViewer);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.verticalScrollBar = this.GetTemplateChild("VerticalScrollBar") as ScrollBar;
        }

        public void ScrollToBottom()
        {
            this.UpdateLayout();
            this.verticalScrollBar.Value = this.verticalScrollBar.Maximum;
        }

        //protected override Size ArrangeOverride(Size finalSize)
        //{
        //    //var content = this.Content as UIElement;
        //    //if (content == null)
        //        return base.ArrangeOverride(finalSize);
        //    //content.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
        //    //return finalSize;
        //}

        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    var result = new Size();

        //    var content = this.Content as UIElement;
        //    if (content == null)
        //        return result;

        //    var presenter = this.GetTemplateChild("Presenter") as ScrollContentPresenter;
        //    if (presenter == null)
        //        return result;

        //    var scrollBar = this.GetTemplateChild("VerticalScrollBar") as ScrollBar;
        //    if (scrollBar == null)
        //        return result;

        //    scrollBar.ViewportSize = availableSize.Height;
        //    scrollBar.Maximum = Math.Max(0.0, content.DesiredSize.Height - availableSize.Height);

        //    presenter.SetVerticalOffset(100);

        //    return result;

        //    //var result = base.MeasureOverride(new Size(availableSize.Width, double.PositiveInfinity));

        //    //var content = this.Content as UIElement;
        //    //if (content == null)
        //    //    return base.MeasureOverride(availableSize);
        //    //////content.Measure(new Size(availableSize.Width - 18, double.PositiveInfinity));

        //    //////SetValue(ExtentHeightProperty, content.DesiredSize.Height);
        //    //////return new Size(
        //    //////    Math.Min(availableSize.Width, content.DesiredSize.Width),
        //    //////    Math.Min(availableSize.Height, content.DesiredSize.Height));
        //    //return result;

        //    //var child = VisualTreeHelper.GetChild(this, 0) as UIElement;
        //    //if (child == null)
        //    //    return new Size();

        //    //child.Measure(availableSize);

        //    //var scrollBar = this.GetTemplateChild("VerticalScrollBar") as ScrollBar;
        //    //scrollBar.ViewportSize = availableSize.Height;
        //    //scrollBar.Maximum = Math.Max(0.0, child.DesiredSize.Height - availableSize.Height);

        //    //return child.DesiredSize;
        //}

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
