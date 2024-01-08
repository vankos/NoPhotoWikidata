using System.Globalization;

namespace WikidataCommon
{
    public class Coordinates
    {
        const string COORDINATES_STRING_FORMAT = "F6";

        public Coordinates()
        {
        }

        public Coordinates(string lattittude, string longitude) 
        {
            Lattittude = double.Parse(lattittude, CultureInfo.InvariantCulture);
            Longitude = double.Parse(longitude, CultureInfo.InvariantCulture);
        }

        public double Lattittude { get; set; }
        public double Longitude { get; set; }

        public string LattittudeString => CoordinateToString(Lattittude);
       
        public string LongitudeString => CoordinateToString(Longitude);

        public override string ToString()
        {
            string lattittude = CoordinateToString(Lattittude);
            string longitude = CoordinateToString(Longitude);
            return $"{lattittude},{longitude}";
        }

        private static string CoordinateToString(double coordinate)
        {
            return coordinate.ToString(COORDINATES_STRING_FORMAT, CultureInfo.InvariantCulture);
        }
    }
}
