using Newtonsoft.Json;

namespace WikidataCommon
{

    public class Binding
    {
        [JsonProperty("q")]
        public WikimediaLink Link { get; set; }
        public Location location { get; set; }
        public Desc desc { get; set; }
        public QLabel qLabel { get; set; }
        public Image image { get; set; }
        public Commonscat commonscat { get; set; }
    }

    public class Commonscat
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Desc
    {
        [JsonProperty("xml:lang")]
        public string xmllang { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Head
    {
        public List<string> vars { get; set; }
    }

    public class Image
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Location
    {
        public string datatype { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

    public class WikimediaLink
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class QLabel
    {
        [JsonProperty("xml:lang")]
        public string xmllang { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Results
    {
        public List<Binding> bindings { get; set; }
    }

    public class WikidataQueryResult
    {
        public Head head { get; set; }
        public Results results { get; set; }
    }


}
