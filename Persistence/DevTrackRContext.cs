using DevTrackR.Entities;

namespace DevTrackR.Persistence
{
    public class DevTrackRContext
    {
        public DevTrackRContext()
        {
            Packages = new List<Package>();
        }

        public List<Package> Packages { get; set; }
    }
}
