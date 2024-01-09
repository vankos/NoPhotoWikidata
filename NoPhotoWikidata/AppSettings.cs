using System.ComponentModel;

namespace NoPhotoWikidata
{
    public class AppSettings : INotifyPropertyChanged
    {
        private const string defaultSearchRadiusDegrees = "0.05";
        private const string searchRadiusDegreesSettingsKey = "SearchRadiusDegreesSettingsKey";
        private string searchRadiusDegrees;

        public AppSettings()
        {
            SearchRadiusDegrees = Preferences.Default.Get(searchRadiusDegreesSettingsKey, defaultSearchRadiusDegrees);
        }

        public string SearchRadiusDegrees
        {
            get => searchRadiusDegrees;
            set
            {
                searchRadiusDegrees = value;
                Preferences.Default.Set(searchRadiusDegreesSettingsKey, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchRadiusDegrees)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
