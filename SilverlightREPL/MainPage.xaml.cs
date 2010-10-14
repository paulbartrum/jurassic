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
        private SilverlightConsoleOutput console;
        private StringBuilder source = new StringBuilder();

        public MainPage()
        {
            InitializeComponent();

            // Initialize the script engine.
            this.engine = new Jurassic.ScriptEngine();

            // Register the firebug console object.
            var consoleObject = new Jurassic.Library.FirebugConsole(engine);
            this.console = new SilverlightConsoleOutput(this.HistoryTextBox);
            consoleObject.Output = this.console;
            engine.Global["console"] = consoleObject;
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
                this.console.Log(SilverlightMessageStyle.Command, command);

                try
                {
                    var result = this.engine.Evaluate(command);

                    // Write the result to the console.
                    this.console.Log(SilverlightMessageStyle.Result, result);

                    // Clear the command textbox.
                    this.CommandTextBox.Text = string.Empty;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("end of input"))
                        this.CommandTextBox.Text += Environment.NewLine;
                    else
                    {
                        this.console.Log(SilverlightMessageStyle.Error, ex.Message);

                        // Clear the command textbox.
                        this.CommandTextBox.Text = string.Empty;
                    }
                }

                // Show the command textbox.
                //this.CommandTextBox.Visibility = System.Windows.Visibility.Collapsed;
                
                e.Handled = true;
            }
        }
    }
}
