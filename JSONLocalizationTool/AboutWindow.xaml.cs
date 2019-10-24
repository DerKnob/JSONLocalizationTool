using JSONLocalizationTool.Helper;
using System;
using System.Windows;

namespace JSONLocalizationTool
{
    /// <summary>
    /// Interaktionslogik für AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            this.Title = this.Title + " " + EnvironmentUtils.getApplicationName();

            string currentVersion = EnvironmentUtils.GetCurrentVersion();
            textBlockVersion.Text = "JSONLocalizationTool" + " v." + currentVersion;

            textBoxTeam.Text = "Christian Knobloch" + Environment.NewLine + "";
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
