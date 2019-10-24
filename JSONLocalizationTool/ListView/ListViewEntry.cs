using System.ComponentModel;

namespace JSONLocalizationTool.ListView
{
    class ListViewEntry : INotifyPropertyChanged
    {
        public ListViewEntry(string key)
        {
            Key = key;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public string Key { get; set; }

        public void FirePropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
