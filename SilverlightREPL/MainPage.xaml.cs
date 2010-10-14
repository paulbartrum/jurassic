using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace SilverlightREPL
{
    public partial class MainPage : UserControl
    {
        private Jurassic.ScriptEngine engine;
        private StringBuilder source = new StringBuilder();

        public MainPage()
        {
            InitializeComponent();

            // Initialize the script engine.
            this.engine = new Jurassic.ScriptEngine();

            // Register the firebug console object.
            var console = new Jurassic.Library.FirebugConsole(engine);
            console.CurrentIndentation = 2;
            engine.Global["console"] = console;
        }

        private void CommandTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Get the command to execute.
                string command = this.CommandTextBox.Text;

                // Hide the command textbox.
                //this.CommandTextBox.Visibility = System.Windows.Visibility.Collapsed;

                // Write the text to evaluate.
                WriteMessage(command);

                try
                {
                    var result = this.engine.Evaluate(source.ToString());

                    // Write the result to the console.
                    WriteMessage(Jurassic.TypeConverter.ToString(result));

                    // Clear the command textbox.
                    this.CommandTextBox.Text = string.Empty;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("end of input"))
                        this.CommandTextBox.Text += Environment.NewLine;
                    else
                    {
                        WriteError(ex.ToString());

                        // Clear the command textbox.
                        this.CommandTextBox.Text = string.Empty;
                    }
                }

                // Show the command textbox.
                //this.CommandTextBox.Visibility = System.Windows.Visibility.Collapsed;
                
                e.Handled = true;
            }
        }

        private void WriteMessage(string message)
        {
            WriteLine(null, Colors.Black, message);
        }

        private void WriteError(string message)
        {
            WriteLine(new ErrorIcon(), Colors.Red, message);
        }

        private void WriteLine(UIElement icon, Color color, string message)
        {
            var paragraph = new Paragraph();
            if (icon != null)
            {
                var canvas = new Canvas() { Height = 12 };
                canvas.Children.Add(icon);
                Canvas.SetLeft(icon, -16);
                Canvas.SetTop(icon, 2);
                paragraph.Inlines.Add(new InlineUIContainer() { Child = canvas });
            }
            paragraph.Inlines.Add(new Run() { Text = message, Foreground = new SolidColorBrush(color) });
            this.HistoryTextBox.Blocks.Add(paragraph);
        }
    }
}
