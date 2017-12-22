using System.ComponentModel.DataAnnotations;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace TestWeb.Models.Blocks
{
    /// <summary>
    /// Used to insert an XForm
    /// </summary>
    [SiteContentType(
        GroupName = Global.GroupNames.Specialized,
        GUID = "FA326346-4D4C-4E82-AFE8-C36279006179")]
    [SiteImageUrl]
    public class FormBlock : SiteBlockData
    {
        [Display(
            GroupName = SystemTabNames.Content,
            Order = 1)]
        [CultureSpecific]
        public virtual string Heading { get; set; }
    }
}
