namespace DataGenerator.Models
{
    public class Advertisement
    {
        public string AdvertisementName { get; set; }
        public List<string> LocationPaths { get; set; }

        public Advertisement(string name, List<string> paths)
        {
            AdvertisementName = name;
            LocationPaths = paths;
        }
    }
}