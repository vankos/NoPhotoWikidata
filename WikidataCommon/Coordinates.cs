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

        public override string ToString()
        {
            string lattittude = Lattittude.ToString(COORDINATES_STRING_FORMAT, System.Globalization.CultureInfo.InvariantCulture);
            string longitude = Longitude.ToString(COORDINATES_STRING_FORMAT, System.Globalization.CultureInfo.InvariantCulture);
            return $"{lattittude},{longitude}";
        }
    }
}
