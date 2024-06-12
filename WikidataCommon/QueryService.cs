using Newtonsoft.Json;
using System.Globalization;
using System.Net;

namespace WikidataCommon
{
    public static class QueryService
    {
        private const int MAX_LATTITTUDE = 90;
        private const int MIN_LATTITTUDE = -90;
        private const int MAX_LONGITUDE = 180;
        private const int MIN_LONGITUDE = -180;

        public static WikidataQueryResult? GetWikiLocationsForLocation(Coordinates deviceLocation, double searchRadiusDegrees)
        {
            Coordinates southWestcorner = new()
            {
                Lattittude = AddLattittude(deviceLocation.Lattittude, -searchRadiusDegrees),
                Longitude = AddLongitude(deviceLocation.Longitude, -searchRadiusDegrees)
            };

            Coordinates northEastcorner = new()
            {
                Lattittude = AddLattittude(deviceLocation.Lattittude, searchRadiusDegrees),
                Longitude = AddLongitude(deviceLocation.Longitude, searchRadiusDegrees)
            };

            string query = $"SELECT ?q ?qLabel ?location ?image ?reason ?desc ?commonscat ?street WHERE {{ SERVICE wikibase:box {{ ?q wdt:P625 ?location . bd:serviceParam wikibase:cornerSouthWest \"Point({southWestcorner.LongitudeString} {southWestcorner.LattittudeString})\"^^geo:wktLiteral . bd:serviceParam wikibase:cornerNorthEast \"Point({northEastcorner.Longitude.ToString("F6", CultureInfo.InvariantCulture)} {northEastcorner.Lattittude.ToString("F6", CultureInfo.InvariantCulture)})\"^^geo:wktLiteral }} OPTIONAL {{ ?q wdt:P18 ?image }} OPTIONAL {{ ?q wdt:P373 ?commonscat }} OPTIONAL {{ ?q wdt:P969 ?street }} SERVICE wikibase:label {{ bd:serviceParam wikibase:language \"en,en,de,fr,es,it,nl,ru\" . ?q schema:description ?desc . ?q rdfs:label ?qLabel }} }} LIMIT 3000";
            string url = $"https://query.wikidata.org/bigdata/namespace/wdq/sparql?query={Uri.EscapeDataString(query)}&format=json";
            WikidataQueryResult? wikidataQueryResult = null;
            using (HttpClient client = new())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "NoPhotoWikidata");
                try
                {
                    string response = client.GetStringAsync(url).Result;
                    wikidataQueryResult = JsonConvert.DeserializeObject<WikidataQueryResult>(response);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return wikidataQueryResult;
        }

        public static string? GetLocationNameFromCoordinates(Coordinates coordinates)
        {
            string queryUrl = $"https://nominatim.openstreetmap.org/reverse?lat={coordinates.LattittudeString}&lon={coordinates.LongitudeString}&format=json";
            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("User-Agent", "NoPhotoWikidata");
            client.DefaultRequestHeaders.Add("accept-language", CultureInfo.CurrentUICulture.Name);
            try
            {
                string jsonResponse = client.GetStringAsync(queryUrl).Result;
                LocationInfoRequestResult? locationInfo = JsonConvert.DeserializeObject<LocationInfoRequestResult>(jsonResponse);
                string? locationName = GetLocationNameFromLocationInfo(locationInfo); ;
                return locationName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string? GetLocationNameFromLocationInfo(LocationInfoRequestResult? locationInfo)
        {
            if(!string.IsNullOrEmpty(locationInfo?.address?.suburb))
            {
                return locationInfo?.address?.suburb;
            }

            if(!string.IsNullOrEmpty(locationInfo?.address?.city))
            {
                return locationInfo?.address?.city;
            }

            if (!string.IsNullOrEmpty(locationInfo?.address?.state))
            {
                return locationInfo?.address?.state;
            }

            if (!string.IsNullOrEmpty(locationInfo?.address?.country))
            {
                return locationInfo?.address?.country;
            }

            if (!string.IsNullOrEmpty(locationInfo?.display_name))
            {
                return locationInfo?.display_name;
            }

            return null;
        }

        private static double AddLattittude(double firsDegree, double secondDegree)
        {
            double sum = firsDegree + secondDegree;
            if (sum > MAX_LATTITTUDE)
            {
                return sum - MAX_LATTITTUDE;
            }

            if (sum < MIN_LATTITTUDE)
            {
                sum = MIN_LATTITTUDE - sum;
            }

            return sum;
        }

        private static double AddLongitude(double firsDegree, double secondDegree)
        {
            double sum = firsDegree + secondDegree;
            if (sum > MAX_LONGITUDE)
            {
                return sum - MAX_LONGITUDE;
            }

            if (sum < MIN_LONGITUDE)
            {
                sum = MIN_LONGITUDE - sum;
            }

            return sum;
        }
    }
}
