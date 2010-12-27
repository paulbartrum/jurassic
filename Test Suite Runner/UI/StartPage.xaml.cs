using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test_Suite_Runner.UI
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        private SuiteRunner runner;

        public StartPage()
        {
            InitializeComponent();

            // Create a new test suite runner.
            this.runner = new SuiteRunner();
            this.runner.RunInPartialTrust = false;
            this.runner.Suites.Add(new ES5ConformTestSuite());
            this.runner.Suites.Add(new SputnikTestSuite());

            // Set up data binding.
            this.DataContext = this.runner;
        }

        /// <summary>
        /// Called when the start button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new PleaseWaitPage(this.runner));
        }
    }
}
