using System;
using System.Collections.Generic;
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
        private List<string> commandHistory = new List<string>();
        private int commandHistoryPosition = 0;
        private string incompleteCommand = string.Empty;
        private string pendingCommand;

        public MainPage()
        {
            InitializeComponent();

            // Initialize the script engine.
            this.engine = new Jurassic.ScriptEngine();

            // Initialize silverlight console.
            this.console = new SilverlightConsoleOutput(this.HistoryTextBox);
            this.console.BeforeLog += new EventHandler(Console_BeforeLog);
            this.console.AfterLog += new EventHandler(Console_AfterLog);

            // Register the firebug console object.
            var consoleObject = new Jurassic.Library.FirebugConsole(engine);
            consoleObject.Output = this.console;
            engine.Global["console"] = consoleObject;
        }

        private void CommandTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Get the command to execute.
                string command = this.CommandTextBox.Text;

                // Queue the command - will be output if any other text is written.
                this.pendingCommand = command;

                try
                {
                    var result = this.engine.Evaluate(command);

                    // Write the result to the console.
                    this.console.Log(SilverlightMessageStyle.Result, result);
                }
                catch (Exception ex)
                {
                    if (ex is Jurassic.JavaScriptException &&
                        ((Jurassic.JavaScriptException)ex).Name == "SyntaxError" &&
                        ex.Message.Contains("end of input"))
                    {
                        // Do not write the pending command.
                        this.console.PendingCommand = null;

                        // Register a callback to scroll the console to the bottom.
                        this.CommandTextBox.TextChanged += new TextChangedEventHandler(CommandTextBox_TextChanged);
                        
                        return;
                    }
                    else
                    {
                        // Log the error message to the console.
                        //this.console.Log(SilverlightMessageStyle.Error, ex.ToString());
                        this.console.Log(SilverlightMessageStyle.Error, ex.Message);
                    }
                }

                // Add to the command history.
                this.commandHistory.Add(command);
                this.commandHistoryPosition = this.commandHistory.Count;

                // Clear the command textbox.
                this.CommandTextBox.Text = string.Empty;

                // Insert an empty line for spacing.
                this.console.Log(SilverlightMessageStyle.Regular, "");

                // The key was handled (i.e. discard the return character).
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                // Record the incomplete command.
                if (this.commandHistoryPosition == this.commandHistory.Count)
                    this.incompleteCommand = this.CommandTextBox.Text;

                // Go up in the command history.
                this.commandHistoryPosition--;
                if (this.commandHistoryPosition < 0)
                {
                    this.commandHistoryPosition = 0;
                    return;
                }
                this.CommandTextBox.Text = this.commandHistory[this.commandHistoryPosition];
                this.CommandTextBox.Select(this.CommandTextBox.Text.Length, 0);

                // The key was handled.
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                // Record the incomplete command.
                if (this.commandHistoryPosition == this.commandHistory.Count)
                    this.incompleteCommand = this.CommandTextBox.Text;

                // Go down in the command history.
                this.commandHistoryPosition ++;
                if (this.commandHistoryPosition > this.commandHistory.Count)
                {
                    this.commandHistoryPosition = this.commandHistory.Count;
                    return;
                }
                if (this.commandHistoryPosition < this.commandHistory.Count)
                    this.CommandTextBox.Text = this.commandHistory[this.commandHistoryPosition];
                else
                    this.CommandTextBox.Text = this.incompleteCommand;
                this.CommandTextBox.Select(this.CommandTextBox.Text.Length, 0);

                // The key was handled.
                e.Handled = true;
            }
            else if (e.Key == Key.Tab)
            {
                // Insert some spaces.
                this.CommandTextBox.SelectedText = "    ";

                // The key was handled.
                e.Handled = true;
            }
        }

        void CommandTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Called just after a newline is inserted.
            // Scroll the console to the bottom.
            this.ScrollViewer.ScrollToBottom();

            // Unregister the event handler.
            this.CommandTextBox.TextChanged -= CommandTextBox_TextChanged;
        }

        private void Console_BeforeLog(object sender, EventArgs e)
        {
            // Clear the command textbox.
            this.CommandTextBox.Text = string.Empty;

            // Output any pending command.
            if (this.pendingCommand != null)
            {
                var pendingCommand = this.pendingCommand;
                this.pendingCommand = null;
                this.console.Log(SilverlightMessageStyle.Command, pendingCommand);
            }
        }

        private void Console_AfterLog(object sender, EventArgs e)
        {
            // Scroll the scroll viewer so the console output is visible.
            this.ScrollViewer.ScrollToBottom();
        }

        private void HistoryTextBox_MouseSelectionEnded(object sender, EventArgs e)
        {
            // Selects the command textbox if the user clicks on the history text without selecting anything.
            if (this.HistoryTextBox.Selection.Start.CompareTo(this.HistoryTextBox.Selection.End) == 0)
                this.CommandTextBox.Focus();
        }

        private void ScrollViewer_GotFocus(object sender, RoutedEventArgs e)
        {
            // Set focus to the command textbox when the user clicks below the command textbox.
            if (e.OriginalSource == this.ScrollViewer)
                this.CommandTextBox.Focus();
        }
    }
}
