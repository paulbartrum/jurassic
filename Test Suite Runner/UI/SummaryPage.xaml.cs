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
    /// Interaction logic for SummaryPage.xaml
    /// </summary>
    public partial class SummaryPage : Page
    {
        private SuiteRunner runner;

        public SummaryPage(SuiteRunner runner)
        {
            InitializeComponent();

            this.runner = runner;
            this.DataContext = runner;
        }
    }
}
