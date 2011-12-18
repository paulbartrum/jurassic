using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Jurassic.TestSuite;

namespace Test_Suite_Runner_WP7
{
    public partial class MainPage : PhoneApplicationPage
    {
        private TestSuite suite;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Start running tests in a background thread.
            var thread = new System.Threading.Thread(WorkerThread);
            thread.Start();
        }

        private void WorkerThread()
        {
            this.suite = new TestSuite(path => App.GetResourceStream(new Uri("Files/" + path.Replace('\\', '/'), UriKind.Relative)).Stream);
            this.suite.TestFinished += new EventHandler<TestEventArgs>(OnTestFinished);
            this.suite.Start();
            this.StatusTextBlock.Dispatcher.BeginInvoke(() =>
            {
                this.StatusTextBlock.Text = string.Format("{0} success, {1} skipped, {2} failures, {3} total",
                    this.suite.SuccessfulTestCount, this.suite.SkippedTestCount, this.suite.FailedTestCount, this.suite.ExecutedTestCount);
            });
        }

        private DateTime lastUpdate = DateTime.MinValue;

        private void OnTestFinished(object sender, TestEventArgs e)
        {
            if (e.Status == TestRunStatus.Failed)
            {
                this.FailuresStackPanel.Dispatcher.BeginInvoke(() =>
                {
                    this.FailuresStackPanel.Children.Add(new TextBlock() { Text = "Failed: " + e.Test.Name });
                });
            }
            if (DateTime.Now.Subtract(lastUpdate).TotalSeconds >= 0.1)
            {
                this.StatusTextBlock.Dispatcher.BeginInvoke(() =>
                {
                    this.StatusTextBlock.Text = string.Format("{0} / {1} ({2:p})",
                        this.suite.ExecutedTestCount, this.suite.ApproximateTotalTestCount, (double)this.suite.ExecutedTestCount / this.suite.ApproximateTotalTestCount);
                });
                lastUpdate = DateTime.Now;
            }
        }
    }
}