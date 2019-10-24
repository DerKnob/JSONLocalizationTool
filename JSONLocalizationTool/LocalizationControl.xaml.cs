using JSONLocalizationTool.Manager;
using System.Windows.Controls;

namespace JSONLocalizationTool
{
    /// <summary>
    /// Interaktionslogik für LocalizationControl.xaml
    /// </summary>
    public partial class LocalizationControl : UserControl
    {
        private string fileName;
        private string key;

        public LocalizationControl(string fileName, string key, string content)
        {
            InitializeComponent();

            this.fileName = fileName;
            this.key = key;

            textBlockFileName.Text = fileName;
            textBoxContent.Text = content;
        }

        private void textBoxContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            LocalizationManager.GetInstance().Update(fileName, key, textBoxContent.Text);
        }
    }
}
