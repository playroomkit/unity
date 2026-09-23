using System.Collections.Generic;

namespace Discord
{
    public class Mapping
    {
        public string Prefix { get; set; }
        public string Target { get; set; }
    }

    public class RemapInput {
        public string URL { get; set; }
        public List<Mapping> Mappings { get; set; }
    }

    public class PatchUrlMappingsConfig
    {
        public bool PatchFetch { get; set; } = true;
        public bool PatchWebSocket { get; set; } = true;
        public bool PatchXhr { get; set; } = true;
        public bool PatchSrcAttributes { get; set; } = false;
    }
}