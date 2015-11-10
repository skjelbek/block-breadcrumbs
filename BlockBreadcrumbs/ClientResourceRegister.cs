using System.Web;
using EPiServer.Editor;
using EPiServer.Framework.Web.Resources;

namespace BlockBreadcrumbs
{
    [ClientResourceRegister]
    public class ClientResourceRegister : IClientResourceRegister
    {
        public void RegisterResources(
            IRequiredClientResourceList requiredResources, 
            HttpContextBase context)
        {
            if (!PageEditing.PageIsInEditMode) return;

            requiredResources.Require("blockBreadcrumbs.Scripts").AtFooter();
            requiredResources.Require("blockBreadcrumbs.Styles").AtHeader();
        }
    }
}
