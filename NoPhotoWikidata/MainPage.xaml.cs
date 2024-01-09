using System.Globalization;
using WikidataCommon;
using Binding = WikidataCommon.Binding;

namespace NoPhotoWikidata
{
    public partial class MainPage : ContentPage
    {
        private readonly AppSettings context;

        private const string DefualtGpxFileNamePrefix = "NoPhotoLocations_.";

        public MainPage()
        {
            InitializeComponent();
            context = new AppSettings();
            BindingContext = context;
        }

        private async void GetGpxButton_Clicked(object sender, EventArgs e)
        {
            Microsoft.Maui.Devices.Sensors.Location? location = await Geolocation.Default.GetLastKnownLocationAsync();
            if(location == null)
            {
                await DisplayAlert("No luck", "Unable to get last location", "OK");
                return;
            }

            Coordinates coordinates = new()
            {
                Lattittude = location.Latitude,
                Longitude = location.Longitude,
            };

            double searchRadiusDegrees = double.Parse(context.SearchRadiusDegrees, CultureInfo.InvariantCulture);
            WikidataQueryResult? queryResult = QueryService.GetWikiLocationsForLocation(coordinates, searchRadiusDegrees);
            if (queryResult == null)
            {
                return;
            }

            List<Binding> locations = queryResult.results.bindings;
            IEnumerable<Binding> locationsWithoutImage = LocationFilter.FilterByDoesntHaveImage(locations);
            string[] exclusions = context.DescriptionExclusions.Split(Environment.NewLine);
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
