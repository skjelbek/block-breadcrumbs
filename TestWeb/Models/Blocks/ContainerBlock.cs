using EPiServer.Core;

namespace TestWeb.Models.Blocks
{
    [SiteContentType(GUID = "B04ACCF3-2F88-4F42-87A1-442C43824B67")]
    [SiteImageUrl]
    public class ContainerBlock : SiteBlockData
    {
        public virtual ContentArea ContentArea { get; set; }
    }
}