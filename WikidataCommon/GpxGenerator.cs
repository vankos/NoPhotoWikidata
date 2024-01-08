using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace WikidataCommon
{
    public static class GpxGenerator
    {
        private const string LattittudeRegexp = @" (.*)\)";
        private const string LongitudeRegexp1 = @"\((.*) ";

        public static string GenerateGpxFromWikidataResult(List<Binding> locations) 
        {
            StringBuilder gpxStringBuilder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(gpxStringBuilder, new XmlWriterSettings { Indent = true }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("gpx", "http://www.topografix.com/GPX/1/1");
                writer.WriteAttributeString("version", "1.1");
                writer.WriteAttributeString("creator", "WikiShootMe-to-GPX");

                foreach (var location in locations)
                {
                    writer.WriteStartElement("wpt");
                    Coordinates coordinates = GetCoordinates(location);
                    writer.WriteAttributeString("lat", coordinates.LattittudeString);
                    writer.WriteAttributeString("lon", coordinates.LongitudeString);

                    writer.WriteElementString("name", location.qLabel.value);
                    writer.WriteElementString("desc", GetDescription(location));

                    writer.WriteEndElement(); // wpt
                }

                writer.WriteEndElement(); // gpx
                writer.WriteEndDocument();
            }

            return gpxStringBuilder.ToString();
        }

        private static Coordinates GetCoordinates(Binding location)
        {
            string coordinatesString = location.location.value;
            string lattittude = Regex.Matches(coordinatesString, LattittudeRegexp)[0].Groups[1].Value;
            string longitude = Regex.Matches(coordinatesString, LongitudeRegexp1)[0].Groups[1].Value;
            Coordinates coordinates = new Coordinates(lattittude, longitude);
            return coordinates;
        }

        private static string? GetDescription(Binding location)
        {
            string url = location.Link.value;
            string description = location?.desc?.value;
            string gpxDescription = $"{description}.\n {url}";
            return gpxDescription;
        }
    }
}
