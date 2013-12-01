using BruTile.Wmts.Generated;

namespace BruTile.Wmts
{
    public class ResourceUrl
    {
        public string Format { get; set; }
        public URLTemplateTypeResourceType ResourceType { get; set; }
        public string Template { get; set; }
    }
}
