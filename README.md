# BlockBreadcrumbs for EPiServer

An EPiServer add-on with the purpose of giving editors a better overview and navigation experience when editing blocks.

## Installation
Install the NuGet package into your **web** project.

`Install-Package BlockBreadcrumbs`

## Setup
In order to get highlighting of a block in a thumbnail you need to use BlockBreadcrumb's own ContentAreaRenderer:

### If your already have your own ContentAreaRenderer...
...make sure you inherit from `BlockBreadcrumbs.ContentAreaRenderer` instead of EPiServer's `EPiServer.Web.Mvc.Html.ContentAreaRenderer`.

### If you **don't have your own ContentAreaRenderer**...
...you need to register `BlockBreadcrumbs.ContentAreaRenderer` as the default ContentAreaRenderer in the dependency resolver:

```csharp
[InitializableModule]
[ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
public class DependencyResolverInitialization : IConfigurableModule
{
    public void ConfigureContainer(ServiceConfigurationContext context)
    {
        context.Container.Configure(ConfigureContainer);
        DependencyResolver.SetResolver(new StructureMapDependencyResolver(context.Container));
    }

    private static void ConfigureContainer(ConfigurationExpression container)
    {
        //Swap out the default ContentRenderer for our custom
        container.For<ContentAreaRenderer>().Use<BlockBreadcrumbs.ContentAreaRenderer>();
    }

    public void Initialize(InitializationEngine context) {}
    public void Uninitialize(InitializationEngine context) {}
    public void Preload(string[] parameters) {}
}
```
