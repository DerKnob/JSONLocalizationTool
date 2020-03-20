using JSONLocalizationTool.ListView;
using JSONLocalizationTool.Manager;
using JSONLocalizationTool.VO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace JSONLocalizationTool
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StringCollection recentFolders;
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            InitListView(LocalizationManager.GetInstance().GetCollection());

            // get recent files
            recentFolders = Properties.Settings.Default.RecentFolders;
            if (recentFolders == null)
                recentFolders = new StringCollection();

            ReloadRecentFoldersMenutItems();

            // timer for statusbar
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += timer_Tick;
        }

        private void InitListView(ObservableCollection<ListViewEntry> observableCollection)
        {
            listView.SelectedItem = null;

            listView.ItemsSource = observableCollection;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.Filter = UserFilter;
        }

        /*******************************************************/

        void timer_Tick(object sender, EventArgs e)
        {
            textBlockInfo.Text = "";
            timer.Stop();
        }

        private void ReloadRecentFoldersMenutItems()
        {
            // update recent open folders list
            for (int i = 0; i < recentFolders.Count; i++)
            {
                MenuItem menuItem = new MenuItem();
                menuItem.Header = recentFolders[i];
                menuItem.Click += MenuItemRecentFolders_Click;

                menuItemOpenRecent.Items.Add(menuItem);
            }

            if (recentFolders.Count == 0)
                menuItemOpenRecent.IsEnabled = false;
            else
                menuItemOpenRecent.IsEnabled = true;
        }


        private void MenuItemRecentFolders_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            if (menuItem == null)
                return;

            OpenTranslation((string) menuItem.Header);
        }

        public void SetInfoText(string text)
        {
            textBlockInfo.Text = text;
            timer.Start();
        }

        /*******************************************************/

        private void menuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenTranslation();
        }

        private void menuItemSave_Click(object sender, RoutedEventArgs e)
        {
            SaveTranslation();
        }

        private void menuItemExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menuItemLanguageManager_Click(object sender, RoutedEventArgs e)
        {
            listView.SelectedItem = null;

            WindowLanguages windowLanguages = new WindowLanguages();
            windowLanguages.Owner = this;
            windowLanguages.ShowDialog();
        }

        private void menuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.ShowDialog();
        }

        /*******************************************************/

        private void buttonOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenTranslation();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveTranslation();
        }

        /*******************************************************/

        private void buttonAddNew_Click(object sender, RoutedEventArgs e)
        {
            AddNew();
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelected();
        }

        private void buttonRename_Click(object sender, RoutedEventArgs e)
        {
            RenameSelected();
        }

        private void buttonSort_Click(object sender, RoutedEventArgs e)
        {
            LocalizationManager.GetInstance().Sort();

            InitListView(LocalizationManager.GetInstance().GetCollection());
        }

        /*******************************************************/

        private void textBoxSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(listView.ItemsSource).Refresh();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // save last open files
            Properties.Settings.Default.RecentFolders = recentFolders;
            Properties.Settings.Default.Save();

            // check if something was changed and not saved
            if (LocalizationManager.GetInstance().IsDirty == false)
                return;

            MessageBoxResult messageBoxResult = MessageBox.Show("Do you want to save your changes?", "Changes are not saved", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SaveTranslation();
                return;
            }
        }

        /**********************************************************/

        private bool UserFilter(object item)
        {
            listView.SelectedItem = null;

            if (String.IsNullOrEmpty(textBoxSearch.Text))
                return true;
            else
                return ((item as ListViewEntry).Key.IndexOf(textBoxSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        /**********************************************************/

        private void AddNew()
        {
            DialogInput dialogInput = new DialogInput("Please enter new key:");
            dialogInput.Owner = this;
            if (dialogInput.ShowDialog() == true)
            {
                string key = dialogInput.Answer;
                ListViewEntry listViewEntry = LocalizationManager.GetInstance().AddKey(key);

                listView.SelectedItem = listViewEntry;
                listView.ScrollIntoView(listViewEntry);
            }
        }

        private void DeleteSelected()
        {
            ListViewEntry entry = (ListViewEntry)listView.SelectedItem;
            if (entry != null)
            {

                MessageBoxResult messageBoxResult = MessageBox.Show(entry.Key, "Do you want to delete the entry?", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (messageBoxResult == MessageBoxResult.No)
                    return;

                LocalizationManager.GetInstance().RemoveKey(entry.Key);

                listView.SelectedItem = null;
            }
        }

        private void RenameSelected()
        {
            ListViewEntry entry = (ListViewEntry)listView.SelectedItem;
            if (entry != null)
            {
                DialogInput dialogInput = new DialogInput("Rename Key", entry.Key);
                dialogInput.Owner = this;
                if (dialogInput.ShowDialog() == true)
                {
                    string key = dialogInput.Answer;

                    bool result = LocalizationManager.GetInstance().RenameKey(entry.Key, key);

                    if (result == false)
                    {
                        MessageBox.Show("The key already exists", "Could not rename key", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void OpenTranslation()
        {
            textBlockCurrentFile.Text = "";

            string selectedPath = LocalizationManager.GetInstance().OpenTranslation();

            listView.SelectedItem = null;

            if (selectedPath == null)
                return;

            // add to recent folders list
            recentFolders.Insert(0, selectedPath);
            ReloadRecentFoldersMenutItems();

            textBlockCurrentFile.Text = selectedPath;
            SetInfoText("Folder opend");
        }

        private void OpenTranslation(string folder)
        {
            LocalizationManager.GetInstance().OpenTranslation(folder);

            listView.SelectedItem = null;

            textBlockCurrentFile.Text = folder;
            SetInfoText("Folder opend");
        }

        private void SaveTranslation()
        {
            LocalizationManager.GetInstance().SaveTranslation();

            listView.SelectedItem = null;

            SetInfoText("Files saved");
        }

        /**********************************************************/

        private void listView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ListViewEntry entry = (ListViewEntry) listView.SelectedItem;
            if (entry != null)
                ShowLocalization(entry.Key);
            else
                stackPanelDetails.Visibility = Visibility.Hidden;
        }

        private void ShowLocalization(string key)
        {
            stackPanelDetails.Visibility = Visibility.Visible;

            // set header
            textBlockKey.Text = key;

            // remove all controls
            stackPanelLocalizations.Children.Clear();

            foreach (KeyValuePair<string, List<LocalizationItem>> entry in LocalizationManager.GetInstance().GetLocalizationDictionary())
            {
                string fileName = entry.Key;
                string content = "";

                List<LocalizationItem> list = entry.Value;

                foreach(LocalizationItem localizationItem in list)
                {
                    if (localizationItem.key == key)
                    {
                        content = localizationItem.value;
                        break;
                    }
                }

                // add new controls
                LocalizationControl localizationControl = new LocalizationControl(fileName, key, content);
                stackPanelLocalizations.Children.Add(localizationControl);
            }
        }
    }
}
