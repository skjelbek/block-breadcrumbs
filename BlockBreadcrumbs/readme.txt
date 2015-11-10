1. Add [RequireClientResources] to the controller reponsible for 
rendering your blocks in preview mode (e.g. PreviewController).

2. Add the following line somewhere (preferrebly at the top) in 
your preview controller's View:

 @{ Html.RenderBlockBreadcrumbs(Model.PreviewContent); } 

3. Use BlockBreadcrumbs.ContentAreaRenderer