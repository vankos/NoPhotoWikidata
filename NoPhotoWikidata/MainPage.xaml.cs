using WikidataCommon;
using Binding = WikidataCommon.Binding;

namespace NoPhotoWikidata
{
    public partial class MainPage : ContentPage
    {
        string[] exludedDescriptionWords =
        {
            "hotel in",
            "hostel in",
            "guesthouse in"
        };

        public MainPage()
        {
            InitializeComponent();
        }

        private async void GetGpxButton_Clicked(object sender, EventArgs e)
        {
            Microsoft.Maui.Devices.Sensors.Location location = await Geolocation.Default.GetLastKnownLocationAsync();
            Coordinates coordinates = new Coordinates()
            {
                Lattittude = location.Latitude,
                Longitude = location.Longitude,
            };

            WikidataQueryResult queryResult = QueryService.GetWikiLocationsForLocation(coordinates, 0.1);
            List<Binding> locations = queryResult.results.bindings;
            List<Binding> locationsWithoutImage = locations.Where(l => LocationNotAHotelAndDoesntHaveImage(l)).ToList();
            string gpx = GpxGenerator.GenerateGpxFromWikidataResult(locationsWithoutImage);
            string name = "NoPhotoLocations_" + DateTime.Now + ".gpx";
            string file = Path.Combine(FileSystem.CacheDirectory, name);
            File.WriteAllText(file, gpx);
            await Launcher.Default.OpenAsync(new OpenFileRequest(name, new ReadOnlyFile(file)));
        }

        bool LocationNotAHotelAndDoesntHaveImage(Binding location)
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
    }

}
