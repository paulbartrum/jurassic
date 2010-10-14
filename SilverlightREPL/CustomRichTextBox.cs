using System;
using System.Windows;
using System.Windows.Controls;

namespace SilverlightREPL
{
    /// <summary>
    /// Custom RichTextBox control that fires an event when the left mouse button is released,
    /// thus ending a selection.
    /// </summary>
    public class CustomRichTextBox : RichTextBox
    {
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (this.MouseSelectionEnded != null)
                this.MouseSelectionEnded(this, EventArgs.Empty);
        }

        /// <summary>
        /// Fired when the user finishes selecting text with the mouse.
        /// </summary>
        public event EventHandler MouseSelectionEnded;
    }
}
