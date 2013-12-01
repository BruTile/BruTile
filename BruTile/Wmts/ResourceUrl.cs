using BruTile.Web.Wmts.Generated;

namespace BruTile.Web.Wmts
{
    public class ResourceUrl
    {
        public string Format { get; set; }
        public URLTemplateTypeResourceType ResourceType { get; set; }
        public string Template { get; set; }
    }
}
