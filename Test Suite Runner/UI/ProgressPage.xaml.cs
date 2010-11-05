using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Test_Suite_Runner.UI
{
    /// <summary>
    /// Interaction logic for ProgressPage.xaml
    /// </summary>
    public partial class ProgressPage : Page
    {
        private Thread backgroundThread;
        private SuiteRunner runner;

        public ProgressPage(SuiteRunner runner)
        {
            InitializeComponent();
            this.runner = runner;
        }


        /// <summary>
        /// Called after the user control has finished loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            // Create a new background worker thread.
            this.backgroundThread = new Thread(() =>
            {
                // Enumerate the tests.
                this.runner.EnumerateTests();

                // Hook up all the events.
                this.runner.TestSuiteStarted += (sender2, e2) => this.Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnTestSuiteStarted), sender2, e2);
                this.runner.TestSucceeded += (sender2, e2) => this.Dispatcher.BeginInvoke(new EventHandler<TestEventArgs>(OnTestSucceeded), sender2, e2);
                this.runner.TestFailed += (sender2, e2) => this.Dispatcher.BeginInvoke(new EventHandler<TestEventArgs>(OnTestFailed), sender2, e2);
                this.runner.TestSkipped += (sender2, e2) => this.Dispatcher.BeginInvoke(new EventHandler<TestEventArgs>(OnTestSkipped), sender2, e2);

                // Start running.
                this.runner.Run(null);

                // Raise the complete event.
                this.Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnComplete), this, EventArgs.Empty);
            });

            // Make the thread low priority.
            this.backgroundThread.IsBackground = true;
            this.backgroundThread.Priority = ThreadPriority.BelowNormal;

            // Start the background thread.
            this.backgroundThread.Start();
        }

        private void OnTestSuiteStarted(object sender, EventArgs e)
        {
            UpdateStatus();
        }

        private void OnTestSucceeded(object sender, TestEventArgs e)
        {
            UpdateStatus();
        }

        private System.Collections.ObjectModel.ObservableCollection<Test> failedTests;

        private void OnTestFailed(object sender, TestEventArgs e)
        {
            UpdateStatus();

            // Initialize the list of failed tests.
            if (this.failedTests == null)
            {
                this.failedTests = new System.Collections.ObjectModel.ObservableCollection<Test>();
                this.failureInformationListView.ItemsSource = this.failedTests;
            }

            // Add a new test to the list of failed tests - the UI will update automatically.
            this.failedTests.Add(e.Test);
        }

        /// <summary>
        /// Called when a test is skipped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTestSkipped(object sender, TestEventArgs e)
        {
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            this.testSuiteRun.Text = this.runner.CurrentTestSuite.Name;
            this.statusRun.Text = string.Format("Running test {0} of {1}",
                this.runner.CurrentTestSuite.ExecutedTestCount,
                this.runner.CurrentTestSuite.TotalTestCount);
            this.progressBar.Value = this.runner.CurrentTestSuite.ExecutedTestCount;
            this.progressBar.Maximum = this.runner.CurrentTestSuite.TotalTestCount;
        }

        /// <summary>
        /// Called when all the tests have finished running.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnComplete(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new SummaryPage(this.runner));
        }

        /// <summary>
        /// Called when the user control is removed from the visual tree.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_Unloaded(object sender, RoutedEventArgs e)
        {
            StopProcessing();
        }

        /// <summary>
        /// Called when the cancel button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            StopProcessing();
            this.NavigationService.GoBack();
        }

        /// <summary>
        /// Stops the background thread.
        /// </summary>
        private void StopProcessing()
        {
            if (this.backgroundThread != null)
                this.backgroundThread.Abort();
            this.backgroundThread = null;
        }
    }
}
