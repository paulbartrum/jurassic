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

namespace Test_Suite_Runner_WP7
{
    public partial class MainPage : PhoneApplicationPage
    {
        private SuiteRunner runner;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.runner = new SuiteRunner();
            this.runner.Suites.Add(new ES5ConformTestSuite());
            this.runner.EnumerateTests();
            this.runner.TestSucceeded += new EventHandler<TestEventArgs>(runner_TestRun);
            this.runner.TestFailed += new EventHandler<TestEventArgs>(runner_TestFailed);
            this.runner.TestSkipped += new EventHandler<TestEventArgs>(runner_TestRun);
            this.runner.Run(null);
        }

        private void runner_TestRun(object sender, TestEventArgs e)
        {
            this.StatusTextBlock.Dispatcher.BeginInvoke(() =>
                {
                    this.StatusTextBlock.Text = string.Format("{0} success, {1} skipped, {2} failures, {3} total",
                        this.runner.OverallSuccessCount, this.runner.OverallSkipCount, this.runner.OverallFailureCount, this.runner.TotalTestCount);
                });
        }

        private void runner_TestFailed(object sender, TestEventArgs e)
        {
            runner_TestRun(sender, e);

            this.FailuresStackPanel.Dispatcher.BeginInvoke(() =>
                {
                    this.FailuresStackPanel.Children.Add(new TextBlock() { Text = e.Test.Name });
                });
        }
    }
}