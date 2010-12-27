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
    /// Interaction logic for PleaseWaitPage.xaml
    /// </summary>
    public partial class PleaseWaitPage : Page
    {
        private SuiteRunner runner;

        public PleaseWaitPage(SuiteRunner runner)
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
            // Enumerate the tests.
            this.runner.EnumerateTests();

            // Navigate to the next page.
            this.NavigationService.Navigate(new ProgressPage(this.runner));
        }
    }
}
