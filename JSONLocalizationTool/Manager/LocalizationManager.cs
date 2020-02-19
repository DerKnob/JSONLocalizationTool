using JSONLocalizationTool.ListView;
using JSONLocalizationTool.VO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace JSONLocalizationTool.Manager
{
    class LocalizationManager
    {
        static private LocalizationManager instance;

        private ObservableCollection<ListViewEntry> collection = new ObservableCollection<ListViewEntry>();
        private Dictionary<string, List<LocalizationItem>> localizationDictionary = new Dictionary<string, List<LocalizationItem>>();

        private string currentFolder = "";

        public bool IsDirty { get; set; }

        private LocalizationManager()
        {
            IsDirty = false;
        }

        static public LocalizationManager GetInstance()
        {
            if (instance == null)
                instance = new LocalizationManager();

            return instance;
        }

        /********************************************************/

        public ObservableCollection<ListViewEntry> GetCollection()
        {
            return collection;
        }

        public Dictionary<string, List<LocalizationItem>> GetLocalizationDictionary()
        {
            return localizationDictionary;
        }

        /***********************************************************/

        private void Clear()
        {
            currentFolder = "";

            collection.Clear();
            localizationDictionary.Clear();
        }

        public void Sort()
        {
            // oder the listview entries
            collection = new ObservableCollection<ListViewEntry>(collection.OrderBy(i => i.Key));

            // order each lang list
            Dictionary<string, List<LocalizationItem>> newList = new Dictionary<string, List<LocalizationItem>>();
            foreach (KeyValuePair<string, List<LocalizationItem>> entry in localizationDictionary)
            {
                List<LocalizationItem> list = entry.Value;

                list = new List<LocalizationItem>(list.OrderBy(i => i.key));

                newList.Add(entry.Key, list);
            }
            localizationDictionary = newList;

            // set to dirty
            IsDirty = true;
        }

        public string OpenTranslation()
        {
            // select folder
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select a folder to be opened:";
            DialogResult dialogResult = folderBrowserDialog.ShowDialog();
            if (dialogResult != System.Windows.Forms.DialogResult.OK)
            {
                // cancel
                return null;
            }

            string selectedPath = folderBrowserDialog.SelectedPath;
            
            return OpenTranslation(selectedPath);
        }

        public string OpenTranslation(string selectedPath)
        { 
            // reset the lists
            Clear();

            // find all JSONs
            string[] filePaths = Directory.GetFiles(selectedPath, "*.json");

            if (filePaths.Length == 0)
                return null;

            currentFolder = selectedPath;

            for (int i = 0; i < filePaths.Length; i++)
            {
                string filePath = filePaths[i];
                string fileName = Path.GetFileName(filePath);

                // load the JSON
                string dataAsJson = File.ReadAllText(filePath);
                LocalizationData loadedData = JsonConvert.DeserializeObject<LocalizationData>(dataAsJson);

                // create a new list entry
                List<LocalizationItem> list = new List<LocalizationItem>();
                localizationDictionary.Add(fileName, list);

                for (int j = 0; j < loadedData.items.Length; j++)
                {
                    LocalizationItem localizationItem = loadedData.items[j];

                    // add the key to the list
                    if (collection.Any(p => p.Key == localizationItem.key) == false)
                    {
                        collection.Add(new ListViewEntry(localizationItem.key));
                    }
                    list.Add(localizationItem);
                }
            }
           
            return selectedPath;
        }

        public void SaveTranslation()
        {
            if (currentFolder.Length == 0)
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Description = "Please select the folder to save the .json's to:";
                DialogResult dialogResult = folderBrowserDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    currentFolder = folderBrowserDialog.SelectedPath;
                }
                else
                {
                    // cancel
                    return;
                }
            }

            // now as we are sure that we have a folder, we are saving the files
            foreach (KeyValuePair<string, List<LocalizationItem>> entry in localizationDictionary)
            {
                string fileName = Path.Combine(currentFolder, entry.Key);
                List<LocalizationItem> list = entry.Value;

                // create the localizationData entry
                LocalizationData localizationData = new LocalizationData();
                localizationData.items = list.ToArray();

                using (StreamWriter file = File.CreateText(fileName))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    //serialize object directly into file stream
                    serializer.Serialize(file, localizationData);
                }
            }

            IsDirty = false;
        }

        public void Update(string fileName, string key, string text)
        {
            foreach (KeyValuePair<string, List<LocalizationItem>> entry in localizationDictionary)
            {
                // search list
                if (entry.Key.Equals(fileName) == false)
                    continue;

                // correct list found
                List<LocalizationItem> list = entry.Value;

                try
                {
                    LocalizationItem localizationItem = list.Single(s => s.key == key);
                    localizationItem.value = text;
                }
                catch(Exception)
                {
                    // if there is no entry, we will get an exception
                    LocalizationItem localizationItem = new LocalizationItem();
                    localizationItem.key = key;
                    localizationItem.value = text;
                    
                    list.Add(localizationItem);
                }

                IsDirty = true;
                
                // done
                return;
            }
        }

        public void RemoveKey(string key)
        {
            // remove from the localization list
            foreach (KeyValuePair<string, List<LocalizationItem>> entry in localizationDictionary)
            {
                // correct list found
                List<LocalizationItem> list = entry.Value;

                try
                {
                    LocalizationItem localizationItem = list.Single(s => s.key == key);
                    list.Remove(localizationItem);
                }
                catch (Exception)
                {
                    // not in the list
                }
            }

            // remove from the global list
            try
            {
                ListViewEntry listViewEntry = collection.Single(s => s.Key == key);
                collection.Remove(listViewEntry);
            }
            catch (Exception)
            {
                // not in the list
            }

            IsDirty = true;
        }

        public ListViewEntry AddKey(string key)
        {
            // add tothe global list
            try
            {
                ListViewEntry listViewEntry = collection.Single(s => s.Key == key);
                // if no exception, then we already have the key

                return listViewEntry;
            }
            catch (Exception)
            {
                ListViewEntry listViewEntry = new ListViewEntry(key);
                collection.Add(listViewEntry);

                IsDirty = true;
                return listViewEntry;
            }
        }

        public bool RenameKey(string oldKey, string newKey)
        {
            // remove from the global list
            try
            {
                ListViewEntry listViewEntry = collection.Single(s => s.Key == newKey);
                // if no exception, then we already have the new key

                return false;
            }
            catch (Exception)
            {
                // remove from the localization list
                foreach (KeyValuePair<string, List<LocalizationItem>> entry in localizationDictionary)
                {
                    // correct list found
                    List<LocalizationItem> list = entry.Value;

                    try
                    {
                        LocalizationItem localizationItem = list.Single(s => s.key == oldKey);
                        localizationItem.key = newKey;
                    }
                    catch (Exception)
                    {
                        // not in the list
                    }
                }

                // remove from the global list
                try
                {
                    ListViewEntry listViewEntry = collection.Single(s => s.Key == oldKey);
                    listViewEntry.Key = newKey;
                    listViewEntry.FirePropertyChanged("Key");
                }
                catch (Exception)
                {
                    // not in the list
                }
            }
            IsDirty = true;
            return true;
        }

        public void RemoveLanguage(string fileName)
        {
            localizationDictionary.Remove(fileName);
            IsDirty = true;
        }

        public void AddLanguage(string fileName)
        {
            localizationDictionary.Add(fileName, new List<LocalizationItem>());
            IsDirty = true;
        }

        public bool RenameLangauge(string oldFileName, string newFileName)
        {
            try
            {
                localizationDictionary.Single(s => s.Key == newFileName);
                // if no exception, then we already have the new key

                return false;
            }
            catch (Exception)
            {
                KeyValuePair<string, List<LocalizationItem>> entry = localizationDictionary.Single(s => s.Key == oldFileName);
                List<LocalizationItem> list = entry.Value;

                localizationDictionary.Remove(oldFileName);
                localizationDictionary.Add(newFileName, list);
            }
            IsDirty = true;
            return true;
        }

        public List<string> GetLanguage()
        {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, List<LocalizationItem>> entry in localizationDictionary)
            {
                list.Add(entry.Key);
            }

            return list;
        }
    }
}
