# BlockBreadcrumbs for EPiServer

An EPiServer add-on with the purpose of giving editors a better overview and navigation experience when editing blocks.

![BlockBreadcrumbs when editing a block](https://cdn-images-1.medium.com/max/800/1*cStoHkbiNeRB7HxSA8LGpg.png)

Read this blog post for more info: https://medium.com/@skjelbek/blockbreadcrumbs-an-add-on-for-improving-the-episerver-ui-when-editing-blocks-a818aeabdbd2

## Installation
Install the NuGet package into your **web** project.

```
Install-Package BlockBreadcrumbs
```
---
Latest compatible version for your EPiServer project:

| EPiServer.CMS.Core   | BlockBreadcrumbs |
| -------------------- | ---------------- |
| ≥ 11.1.0 && ≤ 12.0.0 | 2.0.0            |
| ≥ 10.0.1             | 1.3.0            |
| ≥ 8.0.0 && ≤ 9.12.3  | 1.2.2            |

## Setup

### 1. Register BlockBreadcrumbs.ContentAreaRenderer
In order to get highlighting of a block in a thumbnail you need to use BlockBreadcrumb's own ContentAreaRenderer:

#### 1a. If your already have your own ContentAreaRenderer...
...make sure you inherit from `BlockBreadcrumbs.ContentAreaRenderer` instead of EPiServer's `EPiServer.Web.Mvc.Html.ContentAreaRenderer`.

#### 1b. If you **don't have your own ContentAreaRenderer**...
...you need to register `BlockBreadcrumbs.ContentAreaRenderer` as the default ContentAreaRenderer in the dependency resolver:

```csharp
[InitializableModule]
[ModuleDependency(typeof(EPiServer.Web.InitializationModule), typeof(NinjectWebCommon))]
public class DependencyResolverInitialization : IConfigurableModule
{
    public void ConfigureContainer(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<EPiServer.Web.Mvc.Html.ContentAreaRenderer, BlockBreadcrumbs.ContentAreaRenderer>();
    }

    public void Initialize(InitializationEngine context) { }
    public void Uninitialize(InitializationEngine context) { }
    public void Preload(string[] parameters) { }
}
```

### 2. Require client resources
Make sure that you are requiring client resources:

1. This line needs to be inside of `<head></head>`: `@Html.RequiredClientResources(RenderingTags.Header)`
2. This line needs to be at the end of `<body></body>`: `@Html.RequiredClientResources(RenderingTags.Footer)`

### 3. Make a PreviewController
The BlockBreadcrumbs is meant to be used in an on-page edit preview of a block. In order to enable this preview your will have to make a `PreviewController`. This is explained [here](http://joelabrahamsson.com/pattern-for-episerver-block-preview-mvc-controller/ "Pattern for EPiServer block preview MVC controller by Joel Abrahamsson") and [here](http://jondjones.com/how-to-preview-a-block-in-episerver/ "How to Preview a Block in Episerver by Jon D. Jones"), and also implemented in EPiServer's Alloy demo project for MVC.

_One important difference from those examples is that you need to add this attribute to the PreviewController:_ `[RequireClientResources]`

### 4. Use the HtmlHelper
The last step is to call `@{ Html.RenderBlockBreadcrumbs(Model.PreviewContent); }` somewhere in your `Index.cshtml` for the PreviewController. You would typically add this line at the top of your view, but you are free to place it wherever you want. You can also style it the way you prefer.

Example of a Views/Preview/Index.cshtml:
```cshtml
@using BlockBreadcrumbs
@model PreviewModel

@{ Html.RenderBlockBreadcrumbs(Model.PreviewContent); } 

@Html.PropertyFor(model => model.PreviewContent)
```
In this example, _PreviewContent_ is the block you are editing.
