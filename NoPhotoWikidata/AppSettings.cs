using System.ComponentModel;

namespace NoPhotoWikidata
{
    public class AppSettings : INotifyPropertyChanged
    {
        public const string DefaultSearchRadiusDegrees = "0.05";
        public const string SearchRadiusDegreesSettingsKey = "SearchRadiusDegreesSettingsKey";
        private string searchRadiusDegrees;

        public AppSettings()
        {
            SearchRadiusDegrees = Preferences.Default.Get(SearchRadiusDegreesSettingsKey, DefaultSearchRadiusDegrees);
        }

        public string SearchRadiusDegrees
        {
            get => searchRadiusDegrees;
            set
            {
                Preferences.Default.Set(SearchRadiusDegreesSettingsKey, value);
                searchRadiusDegrees = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchRadiusDegrees)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
