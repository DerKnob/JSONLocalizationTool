using JSONLocalizationTool.Manager;
using System.Windows;

namespace JSONLocalizationTool
{
    /// <summary>
    /// Interaktionslogik für WindowLanguages.xaml
    /// </summary>
    public partial class WindowLanguages : Window
    {
        public WindowLanguages()
        {
            InitializeComponent();

            Reload();
        }

        private void Reload()
        {
            listView.ItemsSource = LocalizationManager.GetInstance().GetLanguage();
        }

        private void buttonAddNew_Click(object sender, RoutedEventArgs e)
        {
            DialogInput dialogInput = new DialogInput("Please enter new language file:", ".json");
            dialogInput.Owner = this;
            if (dialogInput.ShowDialog() == true)
            {
                string fileName = dialogInput.Answer;

                LocalizationManager.GetInstance().AddLanguage(fileName);
            }
            Reload();
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            string entry = (string)listView.SelectedItem;
            if (entry != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show(entry, "Do you want to delete the language?", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    LocalizationManager.GetInstance().RemoveLanguage(entry);
                    Reload();
                }
            }
        }

        private void buttonRename_Click(object sender, RoutedEventArgs e)
        {
            string entry = (string)listView.SelectedItem;
            if (entry != null)
            {
                DialogInput dialogInput = new DialogInput("Rename language", entry);
                dialogInput.Owner = this;
                if (dialogInput.ShowDialog() == true)
                {
                    string key = dialogInput.Answer;

                    bool result = LocalizationManager.GetInstance().RenameLangauge(entry, key);

                    if (result == false)
                    {
                        MessageBox.Show("The language already exists!", "Could not rename", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    Reload();
                }
            }
        }
    }
}
