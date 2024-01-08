using WikidataCommon;
using Binding = WikidataCommon.Binding;

namespace NoPhotoWikidata
{
    public partial class MainPage : ContentPage
    {
        private readonly string[] exludedDescriptionWords =
        [
            "hotel in",
            "hostel in",
            "guesthouse in"
        ];

        private const string DefualtGpxFileNamePrefix = "NoPhotoLocations_.";

        public MainPage()
        {
            InitializeComponent();
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

            WikidataQueryResult queryResult = QueryService.GetWikiLocationsForLocation(coordinates, 0.1);
            List<Binding> locations = queryResult.results.bindings;
            List<Binding> locationsWithoutImage = locations.Where(l => LocationNotAHotelAndDoesntHaveImage(l)).ToList();
            string gpx = GpxGenerator.GenerateGpxFromWikidataResult(locationsWithoutImage);
            string fileName = GetFileName(coordinates);
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            File.WriteAllText(filePath, gpx);
            await Launcher.Default.OpenAsync(new OpenFileRequest(fileName, new ReadOnlyFile(filePath)));
        }

        private bool LocationNotAHotelAndDoesntHaveImage(Binding location)
        {
            if (location.image != null)
                return false;

            if (location.desc?.value == null)
                return true;

            foreach (string exclusion in exludedDescriptionWords)
            {
                if (location.desc.value.Contains(exclusion, StringComparison.CurrentCultureIgnoreCase))
                    return false;
            }

            return true;
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
