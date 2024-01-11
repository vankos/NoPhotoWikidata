using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using WikidataCommon;
using Binding = WikidataCommon.Binding;

namespace NoPhotoWikidata
{
    public class AppSettings : INotifyPropertyChanged
    {
        private const string defaultSearchRadiusDegrees = "0.05";
        private const string searchRadiusDegreesSettingsKey = "SearchRadiusDegreesSettingsKey";
        private string searchRadiusDegrees;

        private string descriptionExclusions;
        private bool isNotBusy;
        private readonly List<string> DefaultDescriptionExclusions =
        [
            "hotel in",
            "hostel in",
            "guesthouse in",
            "apartment in",
        ];
        private const string exludedDescriptionWordsSettingsKey = "ExludedDescriptionWordsSettingsKey";

        private const string DefualtGpxFileNamePrefix = "NoPhotoLocations_.";

        public event EventHandler<string> OnError;

        public AppSettings()
        {
            SearchRadiusDegrees = Preferences.Default.Get(searchRadiusDegreesSettingsKey, defaultSearchRadiusDegrees);
            string defaultDescriptionExclusionsJoined = string.Join(Environment.NewLine, DefaultDescriptionExclusions);
            DescriptionExclusions = Preferences.Default.Get(exludedDescriptionWordsSettingsKey, defaultDescriptionExclusionsJoined);
            isNotBusy = true;
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

        public bool IsNotBusy
        {
            get => isNotBusy;
            set
            {
                isNotBusy = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNotBusy)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ButtonText)));
            }
        }

        public string ButtonText => IsNotBusy ? "Get GPX File" : "Getting GPX...";

        public ICommand GetGpxCommand => new Command(() => GetGpx());

        public event PropertyChangedEventHandler? PropertyChanged;

        private void GetGpx()
        {
            IsNotBusy = false;
            Microsoft.Maui.Devices.Sensors.Location? location = Geolocation.Default.GetLastKnownLocationAsync().Result;
            if (location == null)
            {
                OnError.Invoke(this, "Unable to get last location");
                return;
            }

            Coordinates coordinates = new()
            {
                Lattittude = location.Latitude,
                Longitude = location.Longitude,
            };

            double searchRadiusDegrees = double.Parse(SearchRadiusDegrees, CultureInfo.InvariantCulture);
            WikidataQueryResult? queryResult = QueryService.GetWikiLocationsForLocation(coordinates, searchRadiusDegrees);
            if (queryResult == null)
            {
                OnError.Invoke(this, "Unable to get data from server");
                return;
            }

            List<Binding> locations = queryResult.results.bindings;
            IEnumerable<Binding> locationsWithoutImage = LocationFilter.FilterByDoesntHaveImage(locations);
            string[] exclusions = DescriptionExclusions.Split(Environment.NewLine);
            IEnumerable<Binding> filteredLocations = LocationFilter.FilterByHaveExlusionsInDescription(locationsWithoutImage, exclusions);
            if (!filteredLocations.Any())
            {
                OnError.Invoke(this, "No points with this params");
                return;
            }

            string gpx = GpxGenerator.GenerateGpxFromWikidataResult(filteredLocations);
            string fileName = GetFileName(coordinates);
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            File.WriteAllText(filePath, gpx);
            Launcher.Default.OpenAsync(new OpenFileRequest(fileName, new ReadOnlyFile(filePath)));
            IsNotBusy = true;
        }

        private static string GetFileName(Coordinates coordinates)
        {
            string fileNamePrefix = DefualtGpxFileNamePrefix;
            string? locationName = QueryService.GetLocationNameFromCoordinates(coordinates);
            if (locationName != null)
            {
                fileNamePrefix = locationName + "_";
            }

            string fileName = fileNamePrefix + DateTime.Now + ".gpx";
            return fileName;
        }
    }
}
