using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SilverlightREPL
{
    /// <summary>
    /// Used to display an expander chevron and label.
    /// </summary>
    public class ExpanderButton : CheckBox
    {
        public ExpanderButton()
        {
            this.ClickMode = System.Windows.Controls.ClickMode.Press;
            this.DefaultStyleKey = typeof(ExpanderButton);
        }
    }
}
