using System.ComponentModel;

namespace NoPhotoWikidata
{
    public class AppSettings : INotifyPropertyChanged
    {
        private const string defaultSearchRadiusDegrees = "0.05";
        private const string searchRadiusDegreesSettingsKey = "SearchRadiusDegreesSettingsKey";
        private string searchRadiusDegrees;

        private string descriptionExclusions;
        private readonly List<string> DefaultDescriptionExclusions =
        [
            "hotel in",
            "hostel in",
            "guesthouse in",
            "appartments in",
        ];
        private const string exludedDescriptionWordsSettingsKey = "ExludedDescriptionWordsSettingsKey";

        public AppSettings()
        {
            SearchRadiusDegrees = Preferences.Default.Get(searchRadiusDegreesSettingsKey, defaultSearchRadiusDegrees);
            string defaultDescriptionExclusionsJoined = string.Join(Environment.NewLine, DefaultDescriptionExclusions);
            DescriptionExclusions = Preferences.Default.Get(exludedDescriptionWordsSettingsKey, defaultDescriptionExclusionsJoined);
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

        public string DescriptionExclusions
        {
            get => descriptionExclusions;
            set
            {
                descriptionExclusions = value;
                Preferences.Default.Set(exludedDescriptionWordsSettingsKey, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DescriptionExclusions)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
