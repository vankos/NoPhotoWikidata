namespace WikidataCommon
{
    public static class LocationFilter
    {
        public static IEnumerable<Binding> FilterByHaveExlusionsInDescription(IEnumerable<Binding> locations, string[] exludedDescriptionWords)
        {
            List<Binding> filteredLocations = new List<Binding>();
            foreach (Binding location in locations)
            {
                if (location.desc?.value == null)
                {
                    filteredLocations.Add(location);
                    continue;
                }

                if (LocationShouldBeExcluded(location, exludedDescriptionWords))
                {
                    continue;
                }

                filteredLocations.Add(location);
            }

            return filteredLocations;
        }

        public static IEnumerable<Binding> FilterByDoesntHaveImage(IEnumerable<Binding> locations)
        {
            List<Binding> filteredLocations = new List<Binding>();
            foreach (Binding location in locations)
            {
                if (location.image != null)
                {
                    continue;
                }

                filteredLocations.Add(location);
            }

            return filteredLocations;
        }

        private static bool LocationShouldBeExcluded(Binding location, string[] exludedDescriptionWords)
        {
            foreach (string exclusion in exludedDescriptionWords)
            {
                if (location.desc.value.Contains(exclusion, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
