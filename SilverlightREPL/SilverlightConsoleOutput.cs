using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Jurassic;
using Jurassic.Library;

namespace SilverlightREPL
{
    public enum SilverlightMessageStyle
    {
        Regular,
        Information,
        Warning,
        Error,
        Command,
        Result,
    }


    /// <summary>
    /// Represents an implementation of the Firebug API using a silverlight RichTextBox.
    /// </summary>
    public class SilverlightConsoleOutput : IFirebugConsoleOutput
    {
        private RichTextBox richTextBox;

        /// <summary>
        /// Creates a new SilverlightConsoleOutput instance.
        /// </summary>
        /// <param name="richTextBox"> The RichTextBox to append text to. </param>
        public SilverlightConsoleOutput(RichTextBox richTextBox)
        {
            this.richTextBox = richTextBox;
        }

        /// <summary>
        /// The command to be output if anything is logged to the console.
        /// </summary>
        public string PendingCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Logs a message to the console.
        /// </summary>
        /// <param name="style"> A style which influences the icon and text color. </param>
        /// <param name="objects"> The objects to output to the console. These can be strings or
        /// ObjectInstances. </param>
        public void Log(FirebugConsoleMessageStyle style, object[] objects)
        {
            Log((SilverlightMessageStyle)style, objects);
        }

        /// <summary>
        /// Logs a message to the console.
        /// </summary>
        /// <param name="style"> A style which influences the icon and text color. </param>
        /// <param name="message"> The message to write to the console. </param>
        public void Log(SilverlightMessageStyle style, object message)
        {
            Log(style, new object[] { message });
        }

        public event EventHandler BeforeLog;
        public event EventHandler AfterLog;

        /// <summary>
        /// Logs a message to the console.
        /// </summary>
        /// <param name="style"> A style which influences the icon and text color. </param>
        /// <param name="objects"> The objects to output to the console. These can be strings or
        /// ObjectInstances. </param>
        public void Log(SilverlightMessageStyle style, object[] objects)
        {
            // Trigger BeforeLog event.
            if (this.BeforeLog != null)
                BeforeLog(this, EventArgs.Empty);

            // Convert the objects to a string.
            var message = new System.Text.StringBuilder();
            for (int i = 0; i < objects.Length; i++)
            {
                if (i > 0)
                    message.Append(' ');
                message.Append(TypeConverter.ToString(objects[i]));
            }

            // Determine the color and icon.
            Color color = Colors.Black;
            UIElement icon = null;
            switch (style)
            {
                case SilverlightMessageStyle.Information:
                    break;
                case SilverlightMessageStyle.Warning:
                    break;
                case SilverlightMessageStyle.Error:
                    color = Colors.Red;
                    icon = new ErrorIcon();
                    break;
                case SilverlightMessageStyle.Command:
                    color = Color.FromArgb(255, 0, 128, 255);
                    icon = new CommandIcon() { Foreground = new SolidColorBrush(Colors.Gray) };
                    break;
                case SilverlightMessageStyle.Result:
                    if (objects.Length == 1 && objects[0] == Undefined.Value)
                        color = Colors.Gray;
                    else
                        color = Color.FromArgb(255, 0, 0, 210);
                    icon = new ResultIcon();
                    break;
            }

            // Output the message to the console.
            var paragraph = new Paragraph();
            if (icon != null)
            {
                var canvas = new Canvas() { Width = 0, Height = 12 };
                canvas.Children.Add(icon);
                Canvas.SetLeft(icon, -17);
                Canvas.SetTop(icon, 1);
                paragraph.Inlines.Add(new InlineUIContainer() { Child = canvas });
            }
            paragraph.Inlines.Add(new Run() { Text = message.ToString(), Foreground = new SolidColorBrush(color) });
            this.richTextBox.Blocks.Add(paragraph);

            // Trigger AfterLog event.
            if (this.AfterLog != null)
                AfterLog(this, EventArgs.Empty);
        }

        /// <summary>
        /// Clears the console.
        /// </summary>
        public void Clear()
        {
            this.richTextBox.Blocks.Clear();
        }

        /// <summary>
        /// Starts grouping messages together.
        /// </summary>
        /// <param name="title"> The title for the group. </param>
        /// <param name="initiallyCollapsed"> <c>true</c> if subsequent messages should be hidden by default. </param>
        public void StartGroup(string title, bool initiallyCollapsed)
        {
        }

        /// <summary>
        /// Ends the most recently started group.
        /// </summary>
        public void EndGroup()
        {
        }
    }

}