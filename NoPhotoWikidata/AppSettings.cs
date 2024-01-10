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
        private readonly List<string> DefaultDescriptionExclusions =
        [
            "hotel in",
            "hostel in",
            "guesthouse in",
            "appartments in",
        ];
        private const string exludedDescriptionWordsSettingsKey = "ExludedDescriptionWordsSettingsKey";

        private const string DefualtGpxFileNamePrefix = "NoPhotoLocations_.";

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

        public ICommand GetGpxCommand => new Command(async () => await GetGpx());

        public event PropertyChangedEventHandler? PropertyChanged;

        private async Task GetGpx()
        {
            Microsoft.Maui.Devices.Sensors.Location? location = await Geolocation.Default.GetLastKnownLocationAsync();
            if (location == null)
            {
                //await DisplayAlert("No luck", "Unable to get last location", "OK");
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
                return;
            }

            List<Binding> locations = queryResult.results.bindings;
            IEnumerable<Binding> locationsWithoutImage = LocationFilter.FilterByDoesntHaveImage(locations);
            string[] exclusions = DescriptionExclusions.Split(Environment.NewLine);
            IEnumerable<Binding> filteredLocations = LocationFilter.FilterByHaveExlusionsInDescription(locationsWithoutImage, exclusions);
            string gpx = GpxGenerator.GenerateGpxFromWikidataResult(filteredLocations);
            string fileName = GetFileName(coordinates);
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            File.WriteAllText(filePath, gpx);
            await Launcher.Default.OpenAsync(new OpenFileRequest(fileName, new ReadOnlyFile(filePath)));
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
