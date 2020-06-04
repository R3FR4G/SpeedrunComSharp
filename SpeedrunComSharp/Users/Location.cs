using System.Collections.Generic;

namespace SpeedrunComSharp
{
    public class Location
    {
        public Country Country { get; private set; }
        public CountryRegion Region { get; private set; }

        private Location() { }

        public static Location Parse(SpeedrunComClient client, dynamic locationElement)
        {
            Location location = new Location();
            IDictionary<string, dynamic> properties = locationElement as IDictionary<string, dynamic>;

            if (properties != null)
            {
                location.Country = Country.Parse(client, properties["country"]);

                if (properties.ContainsKey("region"))
                {
                    location.Region = CountryRegion.Parse(client, properties["region"]);
                }
            }
            
            return location;
        }

        public override string ToString()
        {
            if (Region == null)
            {
                return Country.Name;
            }
            else
            {
                return Country.Name + " " + Region.Name;
            }
        }
    }
}
