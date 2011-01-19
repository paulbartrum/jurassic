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

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            this.verticalScrollBar.Value -= e.Delta;
            e.Handled = true;
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
